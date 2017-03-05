using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class WeekendTripAttendanceList : LoggingUserControl
    {
        public int ActivityId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ActivityIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.WeekendActivities.Where(act => act.id == value).Count() <= 0)
                    {
                        throw new InvalidOperationException("Activity Does not Exist.");
                    }

                    ActivityIdField.Value = value.ToString();
                    WeekendActivity activity = db.WeekendActivities.Where(act => act.id == value).Single();
                    TripNameLabel.Text = String.Format("{0} {1}, {2}", activity.DateAndTime.ToLongDateString().Split(',')[0], activity.Name, activity.DateAndTime.ToShortTimeString());

                    List<ActivityAttendanceTableRow> rows = new List<ActivityAttendanceTableRow>();
                    int count = 0;
                    AttendanceTabel.Rows.Add(ActivityAttendanceTableRow.HeaderRow);
                    foreach(WebhostMySQLConnection.StudentSignup signup in activity.StudentSignups.Where(s => !s.IsBanned && !s.IsRescended).OrderBy(s => s.TimeStamp).ToList())
                    {
                        rows.Add(new ActivityAttendanceTableRow(
                            signup.StudentId,
                            String.Format("{0} {1} [{2}]", signup.Student.FirstName, signup.Student.LastName, signup.Student.GraduationYear),
                            ++count,
                            signup.Attended,
                            ReadOnly));
                    }
                    AttendanceTabel.Rows.AddRange(rows.ToArray());

                    Session["ActivityAttendanceRows"] = rows;
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                return SubmitBtn.Enabled;
            }
            set
            {
                SubmitBtn.Enabled = value;
                SubmitBtn.Visible = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.RawUrl.Contains("Mobile"))
                AttendanceTabel.Width = Unit.Percentage(100);
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                WeekendActivity activity = db.WeekendActivities.Where(act => act.id == ActivityId).Single();
                String students = "";
                foreach(TableRow row in AttendanceTabel.Rows)
                {
                    if(!(row is ActivityAttendanceTableRow))
                    {
                        continue;
                    }

                    ActivityAttendanceTableRow arow = ((ActivityAttendanceTableRow)row);

                    WebhostMySQLConnection.StudentSignup signup = activity.StudentSignups.Where(s => s.StudentId == arow.StudentId).Single();
                    signup.Attended = arow.IsAttending;
                    students += arow.Cells[0].Text + Environment.NewLine;
                }
                List<int> studentIds = AdditionalStudentsSelector.GroupIds;
                foreach (int id in studentIds)
                {
                    if (activity.StudentSignups.Where(s => s.StudentId == id).Count() > 0)
                    {
                        WebhostMySQLConnection.StudentSignup signup = activity.StudentSignups.Where(s => s.StudentId == id).Single();
                        signup.Attended = true;
                    }

                    else
                    {
                        WebhostMySQLConnection.StudentSignup newSignup = new WebhostMySQLConnection.StudentSignup()
                        {
                            StudentId = id,
                            ActivityId = activity.id,
                            IsBanned = false,
                            Attended = true,
                            IsRescended = false,
                            TimeStamp = DateTime.Now
                        };

                        Student student = db.Students.Find(id);
                        students += String.Format("{0} {1}{2}", student.FirstName, student.LastName, Environment.NewLine);

                        db.StudentSignups.Add(newSignup);
                    }
                }

                LogInformation("Submitted Student Trip {2} Attendance:{1}{0}", students, Environment.NewLine, activity.Name);
                db.SaveChanges();
            }
        }

        protected void AddToTripBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                WeekendActivity activity = db.WeekendActivities.Where(act => act.id == ActivityId).Single();
                String students = "";
                foreach (TableRow row in AttendanceTabel.Rows)
                {
                    if (!(row is ActivityAttendanceTableRow))
                    {
                        continue;
                    }

                    ActivityAttendanceTableRow arow = ((ActivityAttendanceTableRow)row);

                    WebhostMySQLConnection.StudentSignup signup = activity.StudentSignups.Where(s => s.StudentId == arow.StudentId).Single();
                    signup.Attended = arow.IsAttending;
                }
                List<int> studentIds = AdditionalStudentsSelector.GroupIds;
                foreach(int id in studentIds)
                {
                    if(activity.StudentSignups.Where(s => s.StudentId == id).Count() > 0)
                    {
                        WebhostMySQLConnection.StudentSignup signup = activity.StudentSignups.Where(s => s.StudentId == id).Single();
                        signup.Attended = true;
                    }

                    else
                    {
                        WebhostMySQLConnection.StudentSignup newSignup = new WebhostMySQLConnection.StudentSignup()
                        {
                            StudentId = id,
                            ActivityId = activity.id,
                            IsBanned = false,
                            Attended = true,
                            IsRescended = false,
                            TimeStamp = DateTime.Now
                        };

                        Student student = db.Students.Find(id);
                        students += String.Format("{0} {1}{2}", student.FirstName, student.LastName, Environment.NewLine);

                        db.StudentSignups.Add(newSignup);
                    }
                }

                LogInformation("Added students to {2} signups:{1}{0}", students, Environment.NewLine, activity.Name);
                db.SaveChanges();
                Response.Redirect(Request.RawUrl);
            }
        }
    }
}