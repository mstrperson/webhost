using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2
{
    public partial class MedsAttendance : BasePage
    {
        protected class MedAttendanceMarking
        {
            protected static Dictionary<string, string> MedAttendanceMarks = new Dictionary<string, string>() { { "Present", "On Time" }, { "Late", "Late" }, { "Cut", "No Show" } };
            
            public String Text
            {
                get;
                protected set;
            }

            public int ID
            {
                get;
                protected set;
            }

            public MedAttendanceMarking(int mkid)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    GradeTableEntry mk = db.GradeTableEntries.Where(m => m.id == mkid).Single();
                    if(!MedAttendanceMarks.ContainsKey(mk.Name))
                    {
                        throw new WebhostException("Not Valid Med Attendance Mark!");
                    }

                    this.Text = MedAttendanceMarks[mk.Name];
                    this.ID = mk.id;
                }
            }

            public static List<MedAttendanceMarking> GetDatasource(int year = -1)
            {
                using(WebhostEntities db  =  new WebhostEntities())
                {
                    if(db.AcademicYears.Where(y => y.id == year).Count() <= 0)
                    {
                        year = DateRange.GetCurrentAcademicYear();
                    }

                    GradeTable gt = db.GradeTables.Where(g => g.AcademicYearID == year && g.Name.Equals("Attendance")).Single();
                    List<MedAttendanceMarking> mks = new List<MedAttendanceMarking>();
                    foreach(int entry in gt.GradeTableEntries.Select(e => e.id).ToList())
                    {
                        try
                        {
                            mks.Add(new MedAttendanceMarking(entry));
                        }
                        catch
                        {
                            // ignore!
                        }
                    }

                    return mks;
                }
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
                Session["att_date"] = value;
            }
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            int uid = ((BasePage)Page).user.ID;
            using (WebhostEntities db = new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Where(f => f.ID == uid).Single();
                int year = DateRange.GetCurrentAcademicYear();

                List<Section> currentSections = db.Sections.Where(sec => sec.Course.Name.Equals("Meds Attendance") && sec.Course.AcademicYearID == year).ToList();
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


                BlockSelectDDL.DataSource = (from section in currentSections
                                             select new
                                             {
                                                 Text = section.Block.LongName,
                                                 ID = section.id
                                             });
                BlockSelectDDL.DataTextField = "Text";
                BlockSelectDDL.DataValueField = "ID";
                BlockSelectDDL.DataBind();

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
            TodayCB.Visible = SectionId != -1;
            TodayCB.Checked = (AttendanceDate.Equals(DateTime.Today));
            DateInput.Text = AttendanceDate.ToShortDateString();
            DateInput.Visible = !TodayCB.Checked;
            DateSelectBtn.Visible = !TodayCB.Checked;
            SubmitBtn.Visible = SectionId != -1;
            SubmitBtn.Enabled = SubmitBtn.Visible;
        }

        protected void LoadClassBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int sectionId = -1;
                try
                {
                    sectionId = Convert.ToInt32(BlockSelectDDL.SelectedValue);
                }
                catch
                {
                    return;
                }

                SectionId = sectionId;
                Response.Redirect("~/MedsAttendance.aspx");
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
                BlockNameLabel.Text = String.Format("{0} Attendance", section.Block.Name);
                foreach (Student student in section.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList())
                {
                    AttendanceMarking mark = null;

                    if (section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Count() > 0)
                    {
                        mark = section.AttendanceMarkings.Where(m => m.StudentID == student.ID && m.AttendanceDate.Equals(date)).Single();
                    }

                    TableRow row = new TableRow();
                    TableCell nameCell = new TableCell();
                    Label nameLabel = new Label()
                    {
                        Text = student.FirstName + " " + student.LastName,
                        ToolTip = "Advisor:  " + student.Advisor.FirstName + " " + student.Advisor.LastName
                    };
                    HiddenField sidf = new HiddenField()
                    {
                        Value = Convert.ToString(student.ID)
                    };

                    nameCell.Controls.Add(sidf);
                    nameCell.Controls.Add(nameLabel);

                    TableCell markingCell = new TableCell();
                    RadioButtonList markddl = new RadioButtonList();
                    markddl.RepeatLayout = RepeatLayout.Flow;
                    markddl.RepeatDirection = RepeatDirection.Horizontal;
                    markddl.RepeatColumns = 4;
                    markddl.DataSource = MedAttendanceMarking.GetDatasource();
                    markddl.DataTextField = "Text";
                    markddl.DataValueField = "ID";
                    markddl.DataBind();

                    markddl.ClearSelection();
                    if (mark != null)
                    {
                        markddl.SelectedValue = Convert.ToString(mark.MarkingIndex);
                    }
                    else
                    {
                        markddl.SelectedIndex = 2;
                    }

                    markingCell.Controls.Add(markddl);

                    row.Cells.Add(nameCell);
                    row.Cells.Add(markingCell);
                    AttendanceTable.Rows.Add(row);
                }
            }
        }

        protected void TodayCB_CheckedChanged(object sender, EventArgs e)
        {
            AttendanceDate = DateTime.Today.AddDays(-1);
            Response.Redirect("~/MedsAttendance.aspx");
        }

        protected void DateSelectBtn_Click(object sender, EventArgs e)
        {
            DateTime date = DateRange.GetDateTimeFromString(DateInput.Text);
            AttendanceDate = date;
            Response.Redirect("~/MedsAttendance.aspx");
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            Dictionary<int, AttendanceControl.ShortMarking> MarkingData = new Dictionary<int, AttendanceControl.ShortMarking>();

            foreach (TableRow row in AttendanceTable.Rows)
            {
                int studentId = Convert.ToInt32(((HiddenField)row.Cells[0].Controls[0]).Value);
                int markId = Convert.ToInt32(((DropDownList)row.Cells[1].Controls[0]).SelectedValue);
                MarkingData.Add(studentId, new AttendanceControl.ShortMarking() { markId = markId, notes = "" });
            }
            DateTime date = TodayCB.Checked ? DateTime.Today : DateRange.GetDateTimeFromString(DateInput.Text);
            AttendanceControl.SubmitAttendance(SectionId, MarkingData, user.ID, date);
        }
    }
}