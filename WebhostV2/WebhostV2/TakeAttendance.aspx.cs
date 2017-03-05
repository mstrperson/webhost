using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;
using WebhostMySQLConnection.Web;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WebhostV2
{
    public partial class TakeAttendance : BasePage
    {
        protected int SectionId
        {
            get
            {
                return Session["att_secid"] == null ? -1 : (int)Session["att_secid"];
            }
            set
            {
                if (SectionId != value && Session["required_update_info"] != null)
                {
                    LogWarning("Ditching unfulfilled required_update_info because a new section is being loaded.",
                        typeof(Dictionary<int, AttendanceControl.AdditionalInfoRequest>),
                        (Dictionary<int, AttendanceControl.AdditionalInfoRequest>)Session["required_update_info"]);
                    Session["required_update_info"] = null;
                }
                Session["att_secid"] = value;
            }
        }

        protected DateTime AttendanceDate
        {
            get
            {
                return Session["att_date"] == null ? DateTime.Today : (DateTime)Session["att_date"];
            }
            set
            {
                if (Session["required_update_info"] != null && !AttendanceDate.Equals(value))
                {
                    LogWarning("Ditching unfulfilled required_update_info because the date is being changed.",
                        typeof(Dictionary<int, AttendanceControl.AdditionalInfoRequest>),
                        (Dictionary<int, AttendanceControl.AdditionalInfoRequest>)Session["required_update_info"]);
                    Session["required_update_info"] = null;
                }
                Session["att_date"] = value;
            }
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            int uid = user.ID;
            using (WebhostEntities db = new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Find(uid);
                if (faculty == null)
                {
                    LogError("Unable to locate faculty id {0}", uid);
                    return;
                } 
                
                int term = Import.GetCurrentOrLastTerm();

                List<Section> currentSections = faculty.Sections.Where(sec => sec.Terms.Where(t => t.id == term).Count() > 0).ToList();
                State.log.WriteLine("{1} {0}:  Found {2} current sections.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), currentSections.Count);
                
                if (SectionId == -1)
                {
                    int currentBlockId = -1;
                    Dictionary<DateRange, int> data = DateRange.BlockIdsByTime(DateTime.Today);
                    foreach (DateRange classtime in data.Keys)
                    {
                        if (classtime.Contains(DateTime.Now))
                        {
                            currentBlockId = data[classtime];
                            if (currentSections.Where(sec => sec.BlockIndex == currentBlockId).Count() > 0)
                            {
                                SectionId = currentSections.Where(sec => sec.BlockIndex == currentBlockId).ToList().First().id;
                                break;
                            }
                        }
                    }
                    if (SectionId == -1)
                    {
                        foreach (DateRange classtime in data.Keys)
                        {
                            if (classtime.Intersects(DateTime.Now.AddMinutes(-30), DateTime.Now))
                            {
                                currentBlockId = data[classtime];
                                if (currentSections.Where(sec => sec.BlockIndex == currentBlockId).Count() > 0)
                                {
                                    SectionId = currentSections.Where(sec => sec.BlockIndex == currentBlockId).ToList().First().id;
                                    break;
                                }
                            }
                        }
                    }
                }
                

                ClassSelectCB.DataSource = SectionListItem.GetDataSource(currentSections.Select(sec => sec.id).ToList());
                ClassSelectCB.DataTextField = "Text";
                ClassSelectCB.DataValueField = "ID";
                ClassSelectCB.DataBind();

                TodayCB.Visible = false;
                DateInput.Visible = false;
                DateSelectBtn.Visible = false;
                SubmitBtn.Visible = false;
            }

            if (this.SectionId != -1)
                LoadInfo(SectionId, AttendanceDate);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Browser.IsMobileDevice)
                Response.Redirect("~/Mobile/Attendance.aspx");
            if (!Page.IsPostBack)
            {
                TodayCB.Visible = SectionId != -1;
                TodayCB.Checked = (AttendanceDate.Equals(DateTime.Today));
                DateInput.Text = AttendanceDate.ToShortDateString();
                DateInput.Visible = !TodayCB.Checked;
                DateSelectBtn.Visible = !TodayCB.Checked;
                SubmitBtn.Visible = SectionId != -1;
                SubmitBtn.Enabled = SubmitBtn.Visible;
            }
        }

        protected void LoadClassBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int sectionId = -1;
                try
                {
                    sectionId = Convert.ToInt32(ClassSelectCB.SelectedValue);
                }
                catch
                {
                    return;
                }

                SectionId = sectionId;
                Response.Redirect("~/TakeAttendance.aspx");
            }
        }

        protected void LoadInfo(int sectionId, DateTime date)
        {
            using(WebhostEntities db =new WebhostEntities())
            {
                int year = DateRange.GetCurrentAcademicYear();
                GradeTable attMarkings = db.GradeTables.Where(t => t.AcademicYearID == year && t.Name.Equals("Attendance")).Single();
                AttendanceTable.Rows.Clear();
                Section section = db.Sections.Where(sec => sec.id == sectionId).Single();
                ClassNameLabel.Text = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name);

                AttendancePageInfo api = new AttendancePageInfo()
                {
                    SectionId = sectionId,
                    Date = date,
                    Name = ClassNameLabel.Text,
                    Attendances = new List<AttendanceData>()
                };

                List<Student> StudentRoster = section.Course.Name.Equals("Morning Meeting") ?
                    section.Students.OrderBy(s => s.GraduationYear).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList() :
                    section.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();

                bool allSubmitted = true;

                foreach(Student student in StudentRoster)
                {
                    AttendanceMarking mark = null;
                    AttendanceData atd = new AttendanceData() { StudentId = student.ID, Name = String.Format("{0} {1}", student.FirstName, student.LastName) };

                    if(section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Count() > 0)
                    {
                        mark = section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Single();
                    }
                    else
                    {
                        allSubmitted = false;
                    }

                    TableRow row = new TableRow();
                    TableCell nameCell = new TableCell();
                    Label nameLabel = new Label()
                    {
                        Text = student.FirstName + " " + student.LastName,
                        ToolTip = "Advisor:  " + student.Advisor.FirstName + " " + student.Advisor.LastName,
                        Width=Unit.Percentage(50)
                    };
                    HiddenField sidf = new HiddenField()
                    {
                        Value = Convert.ToString(student.ID)
                    };

                    nameCell.Controls.Add(sidf);
                    nameCell.Controls.Add(nameLabel);

                    TableCell markingCell = new TableCell();
                    DropDownList markddl = new DropDownList();
                    int presentId = attMarkings.GradeTableEntries.Where(m => m.Name.Equals("Present")).Single().id;
                    markddl.DataSource = attMarkings.GradeTableEntries.ToList();
                    markddl.DataTextField = "Name";
                    markddl.DataValueField = "id";
                    markddl.DataBind();

                    if(mark != null)
                    {
                        markddl.ClearSelection();
                        markddl.SelectedValue = Convert.ToString(mark.MarkingIndex);
                        atd.Marking = mark.Marking.Name;
                    }
                    else
                    {
                        markddl.ClearSelection();
                        markddl.SelectedValue = Convert.ToString(presentId);
                        atd.Marking = "Present";
                    }

                    markingCell.Controls.Add(markddl);

                    TableCell noteCell = new TableCell() { Width = Unit.Percentage(25) };
                    TextBox noteinput = new TextBox()
                    {
                        TextMode = TextBoxMode.MultiLine
                    };

                    if(mark != null)
                    {
                        noteinput.Text = mark.Notes;
                    }
                    row.BorderWidth = Unit.Pixel(2);
                    row.BorderStyle = BorderStyle.Solid;
                    noteCell.Controls.Add(noteinput);
                    row.Cells.Add(nameCell);
                    row.Cells.Add(markingCell);
                    row.Cells.Add(noteCell);
                    AttendanceTable.Rows.Add(row);
                }

                if (allSubmitted)
                {
                    SubmittedLabel.Text = "Successfully Submitted.";
                    SubmittedLabel.CssClass = "success_highlight";
                }
                else
                {
                    SubmittedLabel.Text = "You have not yet entered attendance data.";
                    SubmittedLabel.CssClass = "incomplete_highlight";
                }

                LogCurrentData(api);
            }
        }

        protected void LogCurrentData(AttendancePageInfo api)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(AttendancePageInfo));
            MemoryStream mstr = new MemoryStream();
            json.WriteObject(mstr, api);
            mstr.Position = 0;
            StreamReader sr = new StreamReader(mstr);
            String info = sr.ReadToEnd();
            sr.Close();
            mstr.Close();
            sr.Dispose();
            mstr.Dispose();
            LogInformation(info);
        }

        protected void TodayCB_CheckedChanged(object sender, EventArgs e)
        {
            if (TodayCB.Checked)
            {
                AttendanceDate = DateTime.Today;
            }
            else
            {
                AttendanceDate = DateTime.Today.AddDays(-1);
            }
            Redirect("~/TakeAttendance.aspx");
        }

        protected void DateSelectBtn_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Today;
            try
            {
                date = DateRange.GetDateTimeFromString(DateInput.Text);
            }
            catch (InvalidCastException ex)
            {
                LogWarning("Failed to parse date input \"{0}\"{1}{2}", DateInput.Text, Environment.NewLine, ex.InnerException.Message);
                DateInput.Text = "use date format: [mm/dd/yyyy]";
                return;
            } 
            AttendanceDate = date;
            LogInformation("Switching date to {0}", DateInput.Text);
            Redirect("~/TakeAttendance.aspx");
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            Dictionary<int, AttendanceControl.ShortMarking> MarkingData = new Dictionary<int, AttendanceControl.ShortMarking>();
            LogInformation("Submitting Attendance.");
            DateTime date = TodayCB.Checked ? DateTime.Today : DateRange.GetDateTimeFromString(DateInput.Text);
            AttendancePageInfo api = new AttendancePageInfo() { SectionId = this.SectionId, Name = ClassNameLabel.Text, Date = date, Attendances = new List<AttendanceData>() };

            foreach(TableRow row in AttendanceTable.Rows)
            {
                int studentId = Convert.ToInt32(((HiddenField)row.Cells[0].Controls[0]).Value);
                int markId = Convert.ToInt32(((DropDownList)row.Cells[1].Controls[0]).SelectedValue);
                String notes = ((TextBox)row.Cells[2].Controls[0]).Text;
                MarkingData.Add(studentId, new AttendanceControl.ShortMarking() { markId = markId, notes = notes });
                AttendanceData atd = new AttendanceData() { StudentId = studentId, Name = row.Cells[0].Text, Marking = ((DropDownList)row.Cells[1].Controls[0]).SelectedItem.Text };
                ((List<AttendanceData>)api.Attendances).Add(atd);
            }

            AttendanceControl.SubmitAttendance(SectionId, MarkingData, user.ID, date); LogCurrentData(api);

            Dictionary<int, AttendanceControl.AdditionalInfoRequest> request = AttendanceControl.SubmitAttendance(SectionId, MarkingData, user.ID, date);
            if (request.Count != 0)
            {
                Session["required_update_info"] = request;
                AttendanceDate = date;
                Redirect("~/Mobile/UpdateInformation.aspx");
            }

            Redirect(Request.RawUrl);
        }
    }
}