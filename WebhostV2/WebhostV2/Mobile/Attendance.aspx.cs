using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostV2.UserControls;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WebhostV2.Mobile
{
    public partial class Attendance : BasePage
    {
        protected int MasqId
        {
            get
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Faculty faculty = db.Faculties.Find(user.ID);
                    if (faculty == null || faculty.Permissions.Where(p => p.Name.Equals("Administrator") || p.Name.Equals("Dean of Students")).Count() <= 0)
                        return -1;

                }
                return Session["teacher_masq"] == null ? -1 : (int)Session["teacher_masq"];
            }
        }

        protected int SectionId
        {
            get
            {
                return Session["att_secid"] == null ? -1 : (int)Session["att_secid"];
            }
            set
            {
                if(SectionId != value && Session["required_update_info"] != null)
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
                Faculty faculty = db.Faculties.Find(MasqId == -1 ? uid : MasqId);
                if(faculty == null)
                {
                    LogError("Unable to locate faculty id {0}", uid);
                    return;
                }

                int term = DateRange.GetCurrentOrLastTerm();

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

                foreach(ListItem classItem in ClassSelectCB.Items)
                {
                    int sectionId = Convert.ToInt32(classItem.Value);
                    Section section = db.Sections.Find(sectionId);
                    if (DateTime.Today.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Today.DayOfWeek.Equals(DayOfWeek.Sunday)) continue;
                    bool attendanceTaken = true;
                    foreach (Student student in section.Students.ToList())
                    {
                        if (section.AttendanceMarkings.Where(att => att.AttendanceDate.Equals(DateTime.Today) && att.StudentID.Equals(student.ID)).Count() <= 0)
                        {
                            attendanceTaken = false;
                            break;
                        }
                    }
                    bool meetsThisDay = false;
                    switch (DateTime.Today.DayOfWeek)
                    {
                        case DayOfWeek.Monday: meetsThisDay = section.Block.MeetsMonday; break;
                        case DayOfWeek.Tuesday: meetsThisDay = section.Block.MeetsTuesday; break;
                        case DayOfWeek.Wednesday:
                            if (!section.Block.MeetsWednesday) meetsThisDay = false;
                            else if (section.Block.IsSpecial || db.WednesdaySchedules.Where(w => w.Day.Equals(DateTime.Today)).Count() <= 0) meetsThisDay = section.Block.MeetsWednesday;
                            else
                            {
                                meetsThisDay = DateRange.BlockOrderByDayOfWeek(DateTime.Today).Contains(section.Block.Name[0]);
                            }
                            break;
                        case DayOfWeek.Thursday: meetsThisDay = section.Block.MeetsThursday; break;
                        case DayOfWeek.Friday: meetsThisDay = section.Block.MeetsFriday; break;
                        default: break;
                    }

                    if (attendanceTaken) classItem.Text += " [Done]";
                    else if (!meetsThisDay) classItem.Text += " [No Meeting Today]";
                    else classItem.Text += " [Incomplete]";
                }

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
            if (!Page.IsPostBack)
            {
                TodayCB.Visible = SectionId != -1;
                TodayCB.Checked = (AttendanceDate.Equals(DateTime.Today));
                DateInput.Text = AttendanceDate.ToShortDateString();
                DateInput.Visible = !TodayCB.Checked;
                DateSelectBtn.Visible = !TodayCB.Checked;
                SubmitBtn.Visible = SectionId != -1;
                SubmitBtn.Enabled = SubmitBtn.Visible;
                SubmitBtn0.Visible = SubmitBtn.Visible;
                SubmitBtn0.Enabled = SubmitBtn.Enabled;
            }
        }

        protected void LoadClassBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
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
                Redirect("~/Mobile/Attendance.aspx");
            }
        }

        protected void LoadInfo(int sectionId, DateTime date)
        {
            using (WebhostEntities db = new WebhostEntities())
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

                foreach (Student student in StudentRoster)
                {
                    AttendanceMarking mark = null;
                    AttendanceData atd = new AttendanceData() { StudentId = student.ID, Name = String.Format("{0} {1}", student.FirstName, student.LastName) };

                    if (section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Count() == 1)
                    {
                        mark = section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Single();
                    }
                    else if (section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Count() > 1)
                    {
                        LogError("Multiple Attendance Markings found for {0} {1} in [{2}] {3} on {4}", student.FirstName, student.LastName, section.Block.LongName, section.Course.Name, date.ToString("DDD dd MMM, yyyy"));
                        AttendanceMarking toKeep = null;
                        List<AttendanceMarking> toDelete = section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).ToList();
                        foreach(AttendanceMarking errMark in toDelete)
                        {
                            if (toKeep == null) toKeep = errMark;

                            if (toKeep.Marking.Name.Equals("Excused")) break;

                            if(errMark.SubmissionTime > toKeep.SubmissionTime)
                            {
                                toKeep = errMark;
                            }
                        }

                        toDelete.Remove(toKeep);

                        db.AttendanceMarkings.RemoveRange(toDelete);
                        db.SaveChanges();
                        foreach(Faculty teacher in section.Teachers.ToList())
                        {
                            MailControler.MailToUser("Attendance Needs Checking.",
                                String.Format("There was a problem with the marking for {0} {1} in [{2}] {3} on {4}.  I have attempted to correct the error, but you should double check it.  Currently, they are marked as {5}.", student.FirstName, student.LastName, section.Block.LongName, section.Course.Name, date.ToString("DDD dd MMM, yyyy"), toKeep.Marking.Name),
                                String.Format("{0}@dublinschool.org", teacher.UserName),
                                String.Format("{0} {1}", teacher.FirstName, teacher.LastName));
                        }
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
                        Width=Unit.Percentage(60)
                    };
                    HiddenField sidf = new HiddenField()
                    {
                        Value = Convert.ToString(student.ID)
                    };

                    nameCell.Controls.Add(sidf);
                    nameCell.Controls.Add(nameLabel);

                    TableCell markingCell = new TableCell();
                    RadioButtonList markddl = new RadioButtonList() { Width=Unit.Percentage(100), RepeatLayout = RepeatLayout.Table, RepeatDirection = RepeatDirection.Horizontal, RepeatColumns = 2, CssClass="table_fixed" };
                    int presentId = attMarkings.GradeTableEntries.Where(m => m.Name.Equals("Present")).Single().id;
                    markddl.DataSource = attMarkings.GradeTableEntries.ToList();
                    markddl.DataTextField = "Name";
                    markddl.DataValueField = "id";
                    markddl.DataBind();

                    if (mark != null)
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
                    ((List<AttendanceData>)api.Attendances).Add(atd);
                    row.BorderWidth = Unit.Pixel(2);
                    row.BorderStyle = BorderStyle.Solid;
                    
                    row.Cells.Add(nameCell);
                    row.Cells.Add(markingCell);
                    AttendanceTable.Rows.Add(row);
                }

                if (section.AttendanceSubmissionStatuses.Where(s => s.Day.Equals(date) && s.AttendanceStatus.Blocking).Count() > 0)
                {
                    AttendanceSubmissionStatus status = section.AttendanceSubmissionStatuses.Where(s => s.Day.Equals(date)).Single();
                    if (status.TimeStamp.AddSeconds(5 * section.Students.Count) < DateTime.Now)
                    {

                        LogError("There was a problem entering attendance for [{0}] {1} at {2}. I will let the user know that there was a problem.", 
                            section.Block.LongName, section.Course.Name, status.TimeStamp.ToString("dddd, dd MM - h:mm:ss"));
                            
                        if(allSubmitted) // calculated manually above!
                        {
                            LogInformation("Attendance was submitted earlier--but may not have been updated correctly.");
                            MailControler.MailToUser("Attendance Problem",
                                String.Format("Hello,{0}" +
                                              "Your attendance entry for [{1}] {2} had a problem when you tried to submit it at {3}.  There are saved attendance entries, but they may not be correct.  Please double check the entries and resubmit if necessary.{0}{0}Thanks!",
                                              Environment.NewLine, section.Block.LongName, section.Course.Name, status.TimeStamp.ToString("dddd, dd MM - h:mm:ss")), user, "noreply@dublinschool.org", "Attendance Bot");
                            SubmittedLabel.Text = "You have not yet entered attendance data.";
                            SubmittedLabel.CssClass = "incomplete_highlight";
                            status.TimeStamp = DateTime.Now;
                            status.StatusId = db.AttendanceStatuses.Where(st => st.Name.Equals("Not Submitted")).Single().id;
                        }
                        else
                        {
                            LogInformation("Attendance was not submitted prior to this attempt.");
                            MailControler.MailToUser("Attendance Problem",
                                String.Format("Hello,{0}" +
                                              "Your attendance entry for [{1}] {2} had a problem when you tried to submit it at {3}.  There are incomplete attendance records right now.  Please double check the entries and resubmit your attendance for this class.{0}{0}Thanks!",
                                              Environment.NewLine, section.Block.LongName, section.Course.Name, status.TimeStamp.ToString("dddd, dd MM - h:mm:ss")), user, "noreply@dublinschool.org", "Attendance Bot");
                            SubmittedLabel.Text = "Successfully Submitted.";
                            SubmittedLabel.CssClass = "success_highlight";
                            status.TimeStamp = DateTime.Now;
                            status.StatusId = db.AttendanceStatuses.Where(st => st.Name.Equals("Submitted")).Single().id;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        LogWarning("The attendance for [{0}] {1} is still processing (less than the alotted {2} seconds have passed).", section.Block.LongName, section.Course.Name, (5 * section.Students.Count));
                        MailControler.MailToUser(String.Format("Attendance Submission Status for [{0}] {1}", section.Block.LongName, section.Course.Name),
                            String.Format("Hello,{0}Your attendance for [{1}] {2} is still processing.  Because of the number of students in your class, this may take as long as {3}:{4} minutes.  Wait at least that long and check again to see if it has submitted properly.",
                        Environment.NewLine, section.Block.LongName, section.Course.Name, (5 * section.Students.Count) / 60, (5 * section.Students.Count) % 60), user, "noreply@dublinschool.org", "Attendance Bot");
                        SubmittedLabel.Text = "Attendance for this section is Currently Processing.";
                        SubmittedLabel.CssClass = "working_highlight";
                    }
                } 
                else if (allSubmitted)
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

            Redirect("~/Mobile/Attendance.aspx");
        }

        protected void DateSelectBtn_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Today;
            try
            {
                date = DateRange.GetDateTimeFromString(DateInput.Text);
            }
            catch(InvalidCastException ex)
            {
                LogWarning("Failed to parse date input \"{0}\"{1}{2}", DateInput.Text, Environment.NewLine, ex.InnerException.Message);
                DateInput.Text = "use date format: [mm/dd/yyyy]";
                return;
            }
            AttendanceDate = date;
            LogInformation("Switching date to {0}", DateInput.Text);
            Redirect("~/Mobile/Attendance.aspx");
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            Dictionary<int, AttendanceControl.ShortMarking> MarkingData = new Dictionary<int, AttendanceControl.ShortMarking>();
            LogInformation("Submitting Attendance.");
            
            DateTime date = TodayCB.Checked ? DateTime.Today : DateRange.GetDateTimeFromString(DateInput.Text);
            AttendancePageInfo api = new AttendancePageInfo() { SectionId = this.SectionId, Name = ClassNameLabel.Text, Date = date, Attendances = new List<AttendanceData>() };

            foreach (TableRow row in AttendanceTable.Rows)
            {
                int studentId = Convert.ToInt32(((HiddenField)row.Cells[0].Controls[0]).Value);
                int markId = Convert.ToInt32(((RadioButtonList)row.Cells[1].Controls[0]).SelectedValue);
                MarkingData.Add(studentId, new AttendanceControl.ShortMarking() { markId = markId, notes = "" });
                AttendanceData atd = new AttendanceData() { StudentId = studentId, Name = row.Cells[0].Text, Marking = ((RadioButtonList)row.Cells[1].Controls[0]).SelectedItem.Text };
                ((List<AttendanceData>)api.Attendances).Add(atd);
            }

            LogCurrentData(api);

            try
            {
                Dictionary<int, AttendanceControl.AdditionalInfoRequest> request = AttendanceControl.SubmitAttendance(SectionId, MarkingData, user.ID, date);
                if (request.Count != 0)
                {
                    Session["required_update_info"] = request;
                    AttendanceDate = date;
                    Redirect("~/Mobile/UpdateInformation.aspx");
                }

                Redirect(Request.RawUrl);
            }
            catch(WebhostException we)
            {
                SubmittedLabel.Text = we.Message;
                SubmittedLabel.CssClass = "working_highlight";
            }
        }
    }
}