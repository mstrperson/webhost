using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.GoogleAPI;

namespace WebhostV2.UserControls
{
    public partial class StudentSignup : LoggingUserControl
    {
        protected bool ConflictsWith(int otherSignupId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                WeekendActivity thisActivity = db.WeekendActivities.Where(act => act.id == ActivityId).Single();
                WeekendActivity other = db.WeekendActivities.Where(act => act.id == otherSignupId).Single();

                DateRange thisRange = new DateRange(thisActivity.DateAndTime, thisActivity.Duration == 0 ? thisActivity.DateAndTime.AddHours(2) : thisActivity.DateAndTime.AddMinutes(thisActivity.Duration));
                DateRange otherRange = new DateRange(other.DateAndTime, other.Duration == 0 ? other.DateAndTime.AddHours(2) : other.DateAndTime.AddMinutes(other.Duration));

                return thisRange.Intersects(otherRange);
            }
        }

        public int ActivityId
        {
            get
            {
                try
                {
                    return (int)Session["signact"];
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if(db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        LogInformation("Loading activity id {0}", value);
                        WeekendActivity activity = db.WeekendActivities.Find(value);
                        Signup.Enabled = true;
                        Session["signact"] = value;
                        ActivityNameLabel.Text = String.Format("{0} [{1}, {2}{3}]", activity.Name, activity.DateAndTime.ToLongDateString(), activity.DateAndTime.ToShortTimeString(),
                                                                                    activity.Duration == 0?"": String.Format(" ~ {0}", activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString()));
                        CurrentSignups.Text = "";
                        int count = 0;

                        Signup.Text = "Sign me up!";

                        foreach(WebhostMySQLConnection.StudentSignup signup in activity.StudentSignups.Where(s => !s.IsBanned && !s.IsRescended).OrderBy(s => s.TimeStamp))
                        {
                            CurrentSignups.Text += String.Format("[{0}] {1} {2}{3}", ++count, signup.Student.FirstName, signup.Student.LastName, Environment.NewLine);
                            if (count == activity.MaxSignups)
                                CurrentSignups.Text += "************************************************************" + Environment.NewLine;

                            if(signup.StudentId == ((BasePage)Page).user.ID)
                            {
                                Signup.Text = "Remove me from this List.";
                            }
                        }

                        if (((BasePage)Page).user.IsTeacher) Response.Redirect("~/WeekendSignupTeacherView.aspx");

                        // Check for conflicting Signup
                        int studentId = ((BasePage)Page).user.ID;
                        Student student = db.Students.Where(stu => stu.ID == studentId).Single();
                        foreach(WebhostMySQLConnection.StudentSignup signup in student.StudentSignups.Where(sig => !sig.IsRescended && !sig.IsBanned && sig.WeekendActivity.WeekendIndex == activity.WeekendIndex && sig.ActivityId != value))
                        {
                            if(this.ConflictsWith(signup.WeekendActivity.id))
                            {
                                LogError("Prevented Conflicting sign-up.");
                                Signup.Text = String.Format("Conflict:  You are already signed up for {0}", signup.WeekendActivity.Name);
                                Signup.Enabled = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected void reload()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.WeekendActivities.Where(act => act.id == ActivityId).Count() > 0)
                {
                    WeekendActivity activity = db.WeekendActivities.Where(act => act.id == ActivityId).Single();
                    ActivityNameLabel.Text = String.Format("{0} [{1}, {2}{3}]", activity.Name, activity.DateAndTime.ToLongDateString(), activity.DateAndTime.ToShortTimeString(),
                                                                                activity.Duration == 0 ? "" : String.Format(" ~ {0}", activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString()));
                    CurrentSignups.Text = "";
                    int count = 0;
                    foreach (WebhostMySQLConnection.StudentSignup signup in activity.StudentSignups.Where(s => !s.IsBanned && !s.IsRescended).OrderBy(s => s.TimeStamp))
                    {
                        CurrentSignups.Text += String.Format("[{0}] {1} {2}{3}", ++count, signup.Student.FirstName, signup.Student.FirstName, Environment.NewLine);
                        if (count == activity.MaxSignups)
                            CurrentSignups.Text += "************************************************************" + Environment.NewLine;

                        if (signup.StudentId == ((BasePage)Page).user.ID)
                        {
                            Signup.Text = "Remove me from this List.";
                        }
                    }

                    // Check for conflicting Signup
                    int studentId = ((BasePage)Page).user.ID;
                    Student student = db.Students.Where(stu => stu.ID == studentId).Single();
                    foreach (WebhostMySQLConnection.StudentSignup signup in student.StudentSignups.Where(sig => sig.WeekendActivity.WeekendIndex == activity.WeekendIndex && sig.ActivityId != ActivityId))
                    {
                        if (this.ConflictsWith(signup.WeekendActivity.id))
                        {
                            Signup.Text = String.Format("Conflict:  You are already signed up for {0}", signup.WeekendActivity.Name);
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Signup_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                WeekendActivity activity = db.WeekendActivities.Where(act => act.id == ActivityId).Single();
                int studentId = ((BasePage)Page).user.ID;
                String calendarId = db.GoogleCalendars.Where(c =>c.CalendarName.Equals("Weekend Activities")).Single().CalendarId;
                Student student = db.Students.Where(s => s.ID == studentId).Single();
                LogInformation("Processing {0} click for {1}.", Signup.Text, activity.Name);
                /*
                // Check Campused
                if(activity.Weekend.CampusedStudents.Contains(student))
                {
                    State.log.WriteLine("Signup was blocked.  Campused!");
                    MailControler.MailToUser("Signup was blocked.", String.Format("You are not allowed to sign up for {0} because you have been campused this weekend.", activity.Name), ((BasePage)Page).user);
                    return;
                }

                // Check Detention
                DateRange activityTimes = activity.DateAndTime.Hour == 0 ? new DateRange(activity.DateAndTime, activity.DateAndTime.AddDays(1)) :
                                                activity.Duration == 0 ? new DateRange(activity.DateAndTime, activity.DateAndTime.AddHours(3)) :
                                                                         new DateRange(activity.DateAndTime, activity.DateAndTime.AddMinutes(activity.Duration));

                if(activityTimes.Intersects(DateRange.Detention) && activity.Weekend.DetentionList.Contains(student))
                {
                    State.log.WriteLine("Signup was blocked.  Detention!");
                    MailControler.MailToUser("Signup was blocked.", String.Format("You are not allowed to sign up for {0} because you are in Detention this weekend.", activity.Name), ((BasePage)Page).user);
                    return;
                }
                */
                if (Signup.Text.Contains("Remove"))
                {
                    State.log.WriteLine("Removing {0} {1} from {2}", student.FirstName, student.LastName, activity.Name);
                    LogInformation("Removing {0} {1} from {2}", student.FirstName, student.LastName, activity.Name);
                    try
                    {
                        WebhostMySQLConnection.StudentSignup signup = student.StudentSignups.Where(sig => sig.ActivityId == ActivityId).Single();
                        signup.IsRescended = true;
                        db.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        State.log.WriteLine("Failed to remove...\r\n{0}", ex.Message);
                        LogError("Failed to remove {0} {1} from {2}\r\n{3}", student.FirstName, student.LastName, activity.Name, ex.Message);
                        MailControler.MailToWebmaster("Webhost Error:  Removing Signup", String.Format(
                              "Could not remove {0} {1} from signup {2}\r\n{3}", student.FirstName, student.LastName, activity.Name, ex.Message
                            ));

                        return;
                    }

                    if (!activity.GoogleCalendarEventId.Equals(""))
                    {
                        using (GoogleCalendarCall gcal = new GoogleCalendarCall())
                        {
                            gcal.RemoveParticipant(calendarId, activity.GoogleCalendarEventId, student.UserName);
                            State.log.WriteLine("Removed {0} {1} from Calendar Event.");
                        }
                    }
                }
                else
                {
                    State.log.WriteLine("Atempting to sign up {0} {1} for {2}", student.FirstName, student.LastName, activity.Name);
                    if(student.StudentSignups.Where(sig => sig.ActivityId == ActivityId).Count() > 0)
                    {
                        WebhostMySQLConnection.StudentSignup signup = student.StudentSignups.Where(sig => sig.ActivityId == ActivityId).Single();
                        if(signup.IsBanned)
                        {
                            State.log.WriteLine("Signup was blocked.");
                            LogWarning("{0} {1} was blocked from signing up for {2} because they have been banned.", student.FirstName, student.LastName, activity.Name);
                            MailControler.MailToUser("Signup was blocked.", String.Format("You are not allowed to sign up for {0}", activity.Name), ((BasePage)Page).user);
                            return;
                        }

                        signup.IsRescended = false;
                        signup.TimeStamp = DateTime.Now;
                        Signup.Text = "Sign me up!";
                        State.log.WriteLine("Re-signed up!");
                        LogInformation("{0} {1} has resigned up for {2}.", student.FirstName, student.LastName, activity.Name);
                    }
                    else
                    {
                        WebhostMySQLConnection.StudentSignup newSig = new WebhostMySQLConnection.StudentSignup()
                        {
                            StudentId = studentId,
                            ActivityId = ActivityId,
                            IsBanned = false,
                            IsRescended = false,
                            TimeStamp = DateTime.Now
                        };

                        db.StudentSignups.Add(newSig);
                        State.log.WriteLine("New Signup created.");
                        LogInformation("{0} {1} has signed up for {2}.", student.FirstName, student.LastName, activity.Name);
                    }

                    if (!activity.GoogleCalendarEventId.Equals(""))
                    {
                        using (GoogleCalendarCall call = new GoogleCalendarCall())
                        {
                            call.AddEventParticipant(calendarId, activity.GoogleCalendarEventId, student.UserName);
                            State.log.WriteLine("Updated calendar Event to include {0}", student.UserName);
                        }
                    }
                    db.SaveChanges();
                }

                State.log.WriteLine("Signup Changes Saved to Database.");
                reload();
            }
        }
    }
}