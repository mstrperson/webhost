using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class StudentSignupManager : LoggingUserControl
    {
        public int ActivityId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(SignupIdField.Value);
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
                    if (db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        WeekendActivity activity = db.WeekendActivities.Where(act => act.id == value).Single();
                        ActivityNameLabel.Text = String.Format("{0} [{1}, {2}{3}]", activity.Name, activity.DateAndTime.ToLongDateString(), activity.DateAndTime.ToShortTimeString(),
                                                                                    activity.Duration == 0 ? "" : String.Format(" ~ {0}", activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString()));
                        CurrentSignups.Text = "";
                        int count = 0;
                        foreach (WebhostMySQLConnection.StudentSignup signup in activity.StudentSignups.Where(s => !s.IsBanned && !s.IsRescended).OrderBy(s => s.TimeStamp))
                        {
                            CurrentSignups.Text += String.Format("[{0}] {1} {2}{3}", ++count, signup.Student.FirstName, signup.Student.LastName, Environment.NewLine);
                            if (count == activity.MaxSignups)
                                CurrentSignups.Text += "************************************************************" + Environment.NewLine;

                        }

                        StudentsSignedUpDDL.DataSource = StudentSignupListItem.GetDataSource(activity.id, activity.StudentSignups.Select(s => s.StudentId).ToList());
                        StudentsSignedUpDDL.DataTextField = "Text";
                        StudentsSignedUpDDL.DataValueField = "IDStr";
                        StudentsSignedUpDDL.DataBind();
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void KickBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int studentId = -1;
                try
                {
                    studentId = Convert.ToInt32(StudentsSignedUpDDL.SelectedValue.Split(',')[1]);
                }
                catch
                {
                    LogError("Invalid Student Id in {0}", StudentsSignedUpDDL.SelectedValue);
                    return;
                }

                WebhostMySQLConnection.StudentSignup signup = db.StudentSignups.Where(s => s.ActivityId == ActivityId && s.StudentId == studentId).Single();
                signup.IsBanned = true;
                LogWarning("{0} {1} has been banned from {2}", signup.Student.FirstName, signup.Student.LastName, signup.WeekendActivity.Name);
                db.SaveChanges();
            }
        }

        protected void UnBanBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int studentId = -1;
                try
                {
                    studentId = Convert.ToInt32(StudentsSignedUpDDL.SelectedValue.Split(',')[1]);
                }
                catch
                {
                    LogError("Invalid Student Id in {0}", StudentsSignedUpDDL.SelectedValue);
                    return;
                }

                WebhostMySQLConnection.StudentSignup signup = db.StudentSignups.Where(s => s.ActivityId == ActivityId && s.StudentId == studentId).Single();
                signup.IsBanned = false;
                LogWarning("{0} {1} has been un-banned from {2}", signup.Student.FirstName, signup.Student.LastName, signup.WeekendActivity.Name);
                db.SaveChanges();
            }
        }
    }
}