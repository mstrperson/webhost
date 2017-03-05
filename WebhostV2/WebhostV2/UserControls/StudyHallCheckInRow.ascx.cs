using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class StudyHallCheckInRow : LoggingUserControl
    {
        public int StudentId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(StudentIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                StudentIdField.Value = Convert.ToString(value);
            }
        }

        public DateTime Date
        {
            get
            {
                try
                {
                    return DateRange.GetDateTimeFromString(DateField.Value);
                }
                catch
                {
                    return new DateTime();
                }
            }
            set
            {
                DateField.Value = value.ToShortDateString();
            }
        }

        protected int StudyHallId
        {
            get
            {
                return (int)Session["ssh"];
            }
        }

        public void LoadRow()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int yr = DateRange.GetCurrentAcademicYear();
                Student student = db.Students.Where(s => s.ID == StudentId).Single();
                List<AttendanceMarking> atts = student.AttendanceMarkings.Where(mk => mk.AttendanceDate.Equals(Date) && mk.SectionIndex == StudyHallId).ToList();
                NameLabel.Text = String.Format("{0} {1} [{2}]", student.FirstName, student.LastName, student.GraduationYear);
                AttendanceDDL.DataSource = (from lbl in db.GradeTableEntries
                                            where lbl.GradeTable.AcademicYearID == yr && lbl.GradeTable.Name.Equals("Attendance")
                                            select lbl).ToList();
                AttendanceDDL.DataTextField = "Name";
                AttendanceDDL.DataValueField = "id";
                AttendanceDDL.DataBind();

                if(atts.Count() > 0)
                {
                    AttendanceDDL.ClearSelection();
                    AttendanceDDL.SelectedValue = Convert.ToString(atts[0].MarkingIndex);
                    MarkBtn.Text = "Checked In--Click to Update Status";
                }

                if (student.AcademicLevel > 1)
                {
                    LibraryPanel.Visible = true;

                    if(student.AcademicLevel == 3)
                    {
                        SignOutBtn.Visible = false;
                        SignedOutInfo.Visible = false;
                        LibraryLabel.Text = "Independent";
                        return;
                    }

                    DateTime Sunday = DateTime.Today.DayOfWeek.Equals(DayOfWeek.Sunday) ? DateTime.Today : DateRange.ThisFriday.AddDays(-5);
                    DateRange thisWeek = new DateRange(Sunday, Sunday.AddDays(6));
                    List<LibraryPass> passes = student.LibraryPasses.Where(p => thisWeek.Contains(p.LibraryDay)).ToList();
                    List<DateTime> dates = new List<DateTime>();
                    foreach (LibraryPass pass in passes)
                    {
                        if (!dates.Contains(pass.LibraryDay))
                            dates.Add(pass.LibraryDay);
                    }

                    if(dates.Count > 1)
                    {
                        LibraryLabel.Text = "No Library Passes Remaining!";
                        SignOutBtn.Enabled = false;
                    }
                    else
                    {
                        LibraryLabel.Text = String.Format("{0} Libary Passes Remaining.", 2 - dates.Count);
                        SignOutBtn.Enabled = true;
                    }

                    if(passes.Where(p => p.LibraryDay.Equals(DateTime.Today)).Count() > 0)
                    {
                        LibraryPass pass = passes.Where(p => p.LibraryDay.Equals(DateTime.Today)).FirstOrDefault();
                        if(pass.StudyHallSignatureId.HasValue)
                        {
                            SignOutBtn.Visible = false;
                            CancelPassBtn.Visible = true;
                            SignedOutInfo.Visible = true;
                            SignedOutInfo.Text = String.Format("Left Study Hall {0}{3}Signed {1} {2}{3}", pass.StudyHallCheckOutSignature.TimeStamp.ToShortTimeString(), pass.StudyHallCheckOutSignature.Faculty.FirstName, pass.StudyHallCheckOutSignature.Faculty.LastName, Environment.NewLine);
                            if(pass.LibrarySignatureId.HasValue)
                            {
                                SignedOutInfo.Text += String.Format("Left Study Hall {0}{3}Signed {1} {2}{3}", pass.LibraryCheckInSignature.TimeStamp.ToShortTimeString(), pass.LibraryCheckInSignature.Faculty.FirstName, pass.LibraryCheckInSignature.Faculty.LastName, Environment.NewLine);
                            }
                        }
                    }
                    else
                    {
                        SignOutBtn.Visible = true;
                        CancelPassBtn.Visible = false;
                    }
                }
                else
                {
                    LibraryPanel.Visible = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void MarkBtn_Click(object sender, EventArgs e)
        {
            ((BasePage)Page).log.WriteLine("{0} Beginning Marking {1}", Log.TimeStamp(), NameLabel.Text);
            Dictionary<int, AttendanceControl.ShortMarking> mark = new Dictionary<int, AttendanceControl.ShortMarking>()
            {
                {
                    StudentId,
                    new AttendanceControl.ShortMarking()
                    {
                        markId=Convert.ToInt32(AttendanceDDL.SelectedValue), 
                        notes=""
                    }
                }
            };

            MarkBtn.Text = "Checked In--Click to Update Status";

            AttendanceControl.SubmitAttendance(StudyHallId, mark, ((BasePage)Page).user.ID, Date);

            ((BasePage)Page).log.WriteLine("{0} Completed Marking {1} as {2}", Log.TimeStamp(), NameLabel.Text, AttendanceDDL.SelectedItem.Text);
        }

        protected void SignOutBtn_Click(object sender, EventArgs e)
        {
            ((BasePage)Page).log.WriteLine("{0} Signing out {1} to Library.", Log.TimeStamp(), NameLabel.Text);
            using(WebhostEntities db = new WebhostEntities())
            {
                int pid = db.LibraryPasses.Count() > 0 ? db.LibraryPasses.OrderBy(p => p.id).ToList().Last().id + 1 : 0;
                bool update = false;
                if(db.LibraryPasses.Where(p => p.LibraryDay == Date && p.StudentId == StudentId).Count() > 0)
                {
                    pid = db.LibraryPasses.Where(p => p.LibraryDay == Date && p.StudentId == StudentId).ToList().FirstOrDefault().id;
                    update = true;
                    ((BasePage)Page).log.WriteLine("{0} updating Library Pass", Log.TimeStamp());
                }

                LibraryPass pass = new LibraryPass();

                if(!update)
                {
                    ((BasePage)Page).log.WriteLine("{0} Creating new Library Pass", Log.TimeStamp());
                    pass.id = pid;
                    pass.LibraryDay = Date;
                    pass.Notes = "None";
                    pass.StudentId = StudentId;
                    db.LibraryPasses.Add(pass);
                }
                else
                {
                    pass = db.LibraryPasses.Where(p => p.id == pid).Single();
                }

                if(pass.StudyHallSignatureId.HasValue)
                {
                    pass.StudyHallCheckOutSignature.TimeStamp = DateTime.Now;
                }
                else
                {
                    int sigid = db.TimeStampedSignatures.Count() > 0 ? db.TimeStampedSignatures.OrderBy(s => s.id).ToList().Last().id + 1 : 0;
                    TimeStampedSignature sig = new TimeStampedSignature()
                    {
                        FacultyId = ((BasePage)Page).user.ID,
                        TimeStamp = DateTime.Now,
                        id = sigid
                    };

                    db.TimeStampedSignatures.Add(sig);
                    pass.StudyHallSignatureId = sigid;
                }

                db.SaveChanges();
                LoadRow();


                ((BasePage)Page).log.WriteLine("{0} Done Signing Library Pass.", Log.TimeStamp());
            }
        }

        protected void CancelPassBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int pid = db.LibraryPasses.Count() > 0 ? db.LibraryPasses.OrderBy(p => p.id).ToList().Last().id + 1 : 0;
                if (db.LibraryPasses.Where(p => p.LibraryDay == Date && p.StudentId == StudentId).Count() > 0)
                {
                    pid = db.LibraryPasses.Where(p => p.LibraryDay == Date && p.StudentId == StudentId).ToList().FirstOrDefault().id;
                }
                else
                    return;

                LibraryPass pass = db.LibraryPasses.Where(p => p.id == pid).Single();
                db.LibraryPasses.Remove(pass);
                db.SaveChanges();
                SignedOutInfo.Text = "Library Pass has been Canceled.";
                LoadRow();
            }
        }
    }
}