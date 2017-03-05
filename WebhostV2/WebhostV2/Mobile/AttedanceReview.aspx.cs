using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostV2.UserControls;

namespace WebhostV2.Mobile
{
    public partial class AttedanceReview : BasePage
    {
        protected DateRange thisWeek
        {
            get
            {
                if (Session["att_week"] == null)
                    return DateRange.ThisAttendanceWeek;
                else
                    return (DateRange)Session["att_week"];
            }
            set
            {
                Session["att_week"] = value;
            }
        }

        protected int StudentMasq
        {
            get
            {
                if (Session["student_masq"] == null)
                    return -1;
                else return (int)Session["student_masq"];
            }
            set
            {
                Session["student_masq"] = value;
                Session["teacher_masq"] = null;
            }
        }

        protected int TeacherMasq
        {
            get
            {
                if (Session["teacher_masq"] == null)
                    return -1;
                else return (int)Session["teacher_masq"];
            }
            set
            {
                Session["teacher_masq"] = value;
                Session["student_masq"] = null;
            }
        }

        protected void LoadAdminPanel(WebhostEntities db, Faculty teacher, int curTerm)
        {
            PingBtn.Visible = false;
            if (teacher.Permissions.Where(p => p.Name.Contains("Admin") && p.AcademicYear==DateRange.GetCurrentAcademicYear()).Count() > 0)
            {
                PingBtn.Visible = true;
                TeacherSelect.DataSource = FacultyListItem.GetDataSource(db.Faculties.Where(f => f.isActive).OrderBy(f => f.LastName).ThenBy(f => f.FirstName).Select(f => f.ID).ToList());
                TeacherSelect.DataTextField = "Text";
                TeacherSelect.DataValueField = "ID";
                TeacherSelect.DataBind();

                StudentSelect.DataSource = StudentListItem.GetDataSource(db.Students.Where(s => s.isActive).OrderBy(f => f.LastName).ThenBy(f=>f.FirstName).Select(s => s.ID).ToList());
                StudentSelect.DataTextField = "Text";
                StudentSelect.DataValueField = "ID";
                StudentSelect.DataBind();
            }
            else if (db.Departments.Where(d => d.DeptHeadId == teacher.ID).Count() > 0)
            {
                List<int> facids = new List<int>();
                Term term = db.Terms.Find(curTerm);
                foreach (Department dept in teacher.Departments.ToList())
                {
                    foreach (Section section in term.Sections.Where(s => s.Course.DepartmentID == dept.id).ToList())
                    {
                        foreach (Faculty t in section.Teachers.ToList())
                        {
                            if (!facids.Contains(t.ID))
                                facids.Add(t.ID);
                        }
                    }
                }

                TeacherSelect.DataSource = FacultyListItem.GetDataSource(facids);
                TeacherSelect.DataTextField = "Text";
                TeacherSelect.DataValueField = "ID";
                TeacherSelect.DataBind();

                StudentSelect.DataSource = StudentListItem.GetDataSource(teacher.Students.Where(s => s.isActive).Select(s => s.ID).ToList());
                StudentSelect.DataTextField = "Text";
                StudentSelect.DataValueField = "ID";
                StudentSelect.DataBind();
            }
            else
            {
                TeacherSelect.DataSource = FacultyListItem.GetDataSource(new List<int>() { user.ID });
                TeacherSelect.DataTextField = "Text";
                TeacherSelect.DataValueField = "ID";
                TeacherSelect.DataBind();

                StudentSelect.DataSource = StudentListItem.GetDataSource(teacher.Students.Where(s => s.isActive).Select(s => s.ID).ToList());
                StudentSelect.DataTextField = "Text";
                StudentSelect.DataValueField = "ID";
                StudentSelect.DataBind();
            }
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            WeekLabel.Text = string.Format("Week of {0} through {1}", thisWeek.Start.ToString("dddd, dd MMM"), thisWeek.End.ToString("dddd, dd MMM"));
            using(WebhostEntities db = new WebhostEntities())
            {
                TableHeaderRow hr = new TableHeaderRow();
                hr.Cells.Add(new TableHeaderCell() { Text = "Class" });
                foreach(DateTime date in thisWeek.ToList())
                {
                    if (date.DayOfWeek.Equals(DayOfWeek.Saturday) || date.DayOfWeek.Equals(DayOfWeek.Sunday)) continue;
                    hr.Cells.Add(new TableHeaderCell() { Text = date.ToString("dddd, dd MMM") });
                }

                ClassesTable.Rows.Add(hr);
                
                int curTerm = DateRange.GetCurrentOrLastTerm();
                
                if(user.IsStudent || StudentMasq != -1)
                {
                    AdminPanel.Visible = false;
                    Student student = db.Students.Find(user.IsStudent ? user.ID : StudentMasq);
                    if(StudentMasq != -1)
                    {
                        Faculty teacher = db.Faculties.Find(user.ID);

                        LoadAdminPanel(db, teacher, curTerm);
                        AdminPanel.Visible = true;

                        WeekLabel.Text += String.Format(".  For {0} {1}", student.FirstName, student.LastName);
                    }
                    foreach(Section section in student.Sections.Where(s => s.Terms.Where(t => t.id == curTerm).Count() > 0).ToList())
                    {
                        TableRow row = new TableRow(); 
                        String teachersNames = "";
                        foreach (Faculty fac in section.Teachers.ToList())
                        {
                            teachersNames += String.Format("{0} {1}  ", fac.FirstName, fac.LastName);
                        }
                        row.Cells.Add(new TableCell() { Text = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name), ToolTip = teachersNames });
                        foreach (DateTime date in thisWeek.ToList())
                        {
                            if (date.DayOfWeek.Equals(DayOfWeek.Saturday) || date.DayOfWeek.Equals(DayOfWeek.Sunday)) continue;
                            bool meetsThisDay = false;
                            switch (date.DayOfWeek)
                            {
                                case DayOfWeek.Monday: meetsThisDay = section.Block.MeetsMonday; break;
                                case DayOfWeek.Tuesday: meetsThisDay = section.Block.MeetsTuesday; break;
                                case DayOfWeek.Wednesday:
                                    if (!section.Block.MeetsWednesday || section.Course.Name.Contains("Tutorial")) meetsThisDay = false;
                                    else if (section.Block.IsSpecial || db.WednesdaySchedules.Where(w => w.Day.Equals(date)).Count() <= 0) meetsThisDay = section.Block.MeetsWednesday;
                                    else
                                    {
                                        meetsThisDay = DateRange.BlockOrderByDayOfWeek(date).Contains(section.Block.Name[0]);
                                    }
                                    break;
                                case DayOfWeek.Thursday: meetsThisDay = section.Block.MeetsThursday; break;
                                case DayOfWeek.Friday: meetsThisDay = section.Block.MeetsFriday; break;
                                default: break;
                            }
                            
                            String attendanceMarking = meetsThisDay?"*":"No Meeting";
                            String note = "No Data";
                            if(section.AttendanceMarkings.Where(att => att.AttendanceDate.Equals(date) && att.StudentID.Equals(StudentMasq == -1 ? user.ID: StudentMasq)).Count() > 0)
                            {
                                AttendanceMarking mark = section.AttendanceMarkings.Where(att => att.AttendanceDate.Equals(date) && att.StudentID.Equals(StudentMasq == -1 ? user.ID : StudentMasq)).Single();
                                attendanceMarking = mark.Marking.Name;
                                note = mark.Notes;
                            }
                            row.Cells.Add(new TableCell()
                            {
                                Text = attendanceMarking,
                                BackColor = attendanceMarking.Equals("Present") ?
                                    System.Drawing.Color.Green :
                                    attendanceMarking.Equals("Late") ?
                                        System.Drawing.Color.LightGoldenrodYellow :
                                        attendanceMarking.Equals("Cut") ?
                                            System.Drawing.Color.Red :
                                                attendanceMarking.Equals("Excused") ? System.Drawing.Color.LightSeaGreen :
                                                meetsThisDay ?
                                                    System.Drawing.Color.LightCoral : System.Drawing.Color.LavenderBlush,
                                ToolTip = note
                            });
                        }

                        ClassesTable.Rows.Add(row);
                    }
                }
                else
                {
                    AdminPanel.Visible = true;
                    Faculty teacher = db.Faculties.Find(user.ID);

                    //int curTerm = DateRange.GetCurrentOrLastTerm();

                    LoadAdminPanel(db, teacher, curTerm);

                    if(TeacherMasq != -1)
                    {
                        teacher = db.Faculties.Find(TeacherMasq);
                        WeekLabel.Text += String.Format(".  For {0} {1}", teacher.FirstName, teacher.LastName);
                    }

                    foreach (Section section in teacher.Sections.Where(s => s.Terms.Where(t => t.id == curTerm).Count() > 0).ToList())
                    {
                        TableRow row = new TableRow();
                        String teachersNames = "";
                        foreach(Faculty fac in section.Teachers.ToList())
                        {
                            teachersNames += String.Format("{0} {1}  ", fac.FirstName, fac.LastName);
                        }
                        row.Cells.Add(new TableCell() { Text = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name), ToolTip = teachersNames });
                        foreach (DateTime date in thisWeek.ToList())
                        {
                            if (date.DayOfWeek.Equals(DayOfWeek.Saturday) || date.DayOfWeek.Equals(DayOfWeek.Sunday)) continue;
                            bool attendanceTaken = true;
                            foreach(Student student in section.Students.ToList())
                            {
                                if(section.AttendanceMarkings.Where(att => att.AttendanceDate.Equals(date) && att.StudentID.Equals(student.ID)).Count() <= 0)
                                {
                                    attendanceTaken = false;
                                    break;
                                }
                            }
                            bool meetsThisDay = false;
                            switch(date.DayOfWeek)
                            {
                                case DayOfWeek.Monday: meetsThisDay = section.Block.MeetsMonday; break;
                                case DayOfWeek.Tuesday: meetsThisDay = section.Block.MeetsTuesday; break;
                                case DayOfWeek.Wednesday:
                                    if (!section.Block.MeetsWednesday || section.Course.Name.Contains("Tutorial")) meetsThisDay = false;
                                    else if(section.Block.IsSpecial || db.WednesdaySchedules.Where(w => w.Day.Equals(date)).Count() <= 0) meetsThisDay = section.Block.MeetsWednesday; 
                                    else
                                    {
                                        meetsThisDay = DateRange.BlockOrderByDayOfWeek(date).Contains(section.Block.Name[0]);                                        
                                    }
                                    break;
                                case DayOfWeek.Thursday: meetsThisDay = section.Block.MeetsThursday; break;
                                case DayOfWeek.Friday: meetsThisDay = section.Block.MeetsFriday; break;
                                default: break;
                            }
                            row.Cells.Add(new TableCell()
                            {
                                Text = attendanceTaken ? "Complete" : meetsThisDay? "Incomplete": "No Meeting",
                                BackColor = attendanceTaken ? System.Drawing.Color.Green : meetsThisDay? System.Drawing.Color.Red : System.Drawing.Color.AliceBlue
                            });
                        }

                        ClassesTable.Rows.Add(row);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PreviousWeekBtn_Click(object sender, EventArgs e)
        {
            thisWeek = thisWeek.MoveByDays(-7);
            Response.Redirect(Request.RawUrl);
        }

        protected void NextWeekBtn_Click(object sender, EventArgs e)
        {
            thisWeek = thisWeek.MoveByDays(7);
            Response.Redirect(Request.RawUrl);
        }

        protected void TeacherBtn_Click(object sender, EventArgs e)
        {
            try
            {
                TeacherMasq = Convert.ToInt32(TeacherSelect.SelectedValue);
                Response.Redirect(Request.RawUrl);
            }
            catch { }
        }

        protected void StudentBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StudentMasq = Convert.ToInt32(StudentSelect.SelectedValue);
                Response.Redirect(Request.RawUrl);
            }
            catch { }
        }

        protected void ConfirmBtn_Click(object sender, EventArgs e)
        {
            if(DateTime.Today.DayOfWeek.Equals(DayOfWeek.Saturday) || DateTime.Today.Day.Equals(DayOfWeek.Sunday))
            {
                LogInformation("No Email Pings on the weekend!");
                return;
            }

            using(WebhostEntities db = new WebhostEntities())
            {
                Dictionary<Faculty, List<Section>> needAttendance = new Dictionary<Faculty, List<Section>>();
                int tid = DateRange.GetCurrentOrLastTerm();
                Term term = db.Terms.Find(tid);

                foreach(Section section in term.Sections.ToList())
                {
                    if (section.Block.IsSpecial) continue; // don't mark these blocks...

                    switch(DateTime.Today.DayOfWeek)
                    {
                        case DayOfWeek.Monday: if (!section.Block.MeetsMonday) continue; break;
                        case DayOfWeek.Tuesday: if (!section.Block.MeetsTuesday) continue; break;
                        case DayOfWeek.Wednesday:
                            if (!section.Block.MeetsWednesday || section.Course.Name.Contains("Tutorial")) continue;
                            else if (db.WednesdaySchedules.Where(w => w.Day.Equals(DateTime.Today)).Count() > 0)
                            {
                                WednesdaySchedule wed = db.WednesdaySchedules.Where(w => w.Day.Equals(DateTime.Today)).Single();
                                if (wed.IsABC && (new Regex("[DEF]")).IsMatch(section.Block.Name))
                                    continue;
                            }
                            break;
                        case DayOfWeek.Thursday: if (!section.Block.MeetsThursday) continue; break;
                        case DayOfWeek.Friday: if (!section.Block.MeetsFriday) continue; break;
                        default: break;
                    }

                    if(section.AttendanceMarkings.Where(at => at.AttendanceDate.Equals(DateTime.Today)).Count() <= 0)
                    {
                        LogWarning("No attendance has been submitted for [{0}] {1}", section.Block.LongName, section.Course.Name);
                        foreach(Faculty teacher in section.Teachers.ToList())
                        {
                            if(!needAttendance.ContainsKey(teacher))
                            {
                                LogWarning("Adding {0} {1} to the Notify list.", teacher.FirstName, teacher.LastName);
                                needAttendance.Add(teacher, new List<Section>() { section });
                            }
                            else
                            {
                                needAttendance[teacher].Add(section);
                            }
                        }
                    }
                }

                String Subject = "Please submit attendance";
                String BodyTemplate = "You have not entered attendance today for the following classes:" + Environment.NewLine;
                String DigestBody = "The following Teachers have been informed that their attendance has not been taken for the indicated classes.";

                foreach(Faculty teacher in needAttendance.Keys)
                {
                    LogInformation("Sending Request Email to {0} {1} for {2} classes.", teacher.FirstName, teacher.LastName, needAttendance[teacher].Count);
                    String Body = String.Format("Dear {0},{1}{1}{2}", teacher.FirstName, Environment.NewLine, BodyTemplate);

                    DigestBody += String.Format("{0}{0}____________________________________{0}{1} {2}:", Environment.NewLine, teacher.FirstName, teacher.LastName);

                    foreach(Section section in needAttendance[teacher])
                    {
                        Body += String.Format("{0}[{1}] {2}", Environment.NewLine, section.Block.LongName, section.Course.Name);
                        DigestBody += String.Format("{0}[{1}] {2}", Environment.NewLine, section.Block.LongName, section.Course.Name);
                    }

                    Body += String.Format("{0}{0}Thanks,{0}Dean of Students Office{0}{0}CC:smcfall@dublinschool.org, {1}@dublinschool.org", Environment.NewLine, teacher.UserName);

                    MailControler.MailToUser(Subject, Body, String.Format("{0}@dublinschool.org", teacher.UserName), String.Format("{0} {1}", teacher.FirstName, teacher.LastName));
                    //MailControler.MailToUser(Subject, Body, MailControler.DeanOfStudents.Email, MailControler.DeanOfStudents.Name);
                    //MailControler.MailToWebmaster(Subject, Body);
                }

                MailControler.MailToWebmaster("Attendance Reminder Digest", DigestBody);
                MailControler.MailToUser("Attendance Reminder Digest", DigestBody, MailControler.DeanOfStudents.Email, MailControler.DeanOfStudents.Name);
                LogInformation("Done sending Attendance Reminders.");
            }
            ConfirmPanel.Visible = false;
        }

        protected void PingBtn_Click(object sender, EventArgs e)
        {
            LogInformation("Asking user to confirm email sending.");
            ConfirmPanel.Visible = true;
        }

        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            LogInformation("Canceled Attendance Reminder Email.");
            ConfirmPanel.Visible = false;
        }

    }
}