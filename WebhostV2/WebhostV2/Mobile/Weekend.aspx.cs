using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.Mobile
{
    public partial class Weekend : BasePage
    {
        protected int WeekendId
        {
            get
            {
                if (Session["m_wk_id"] == null)
                    Session["m_wk_id"] = DateRange.GetCurrentWeekendId();

                return (int)Session["m_wk_id"];
            }
        }

        protected int SelectedActivity
        {
            get
            {
                if (Session["m_a_id"] == null) return -1;

                return (int)Session["m_a_id"];
            }
            set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if (db.WeekendActivities.Where(act => act.id == value).Count() <= 0)
                        return;
                }
                Session["m_a_id"] = value;
                if (user.IsTeacher)
                {
                    WeekendTripAttendanceList1.ActivityId = value;
                    WeekendTabs.ActiveTabIndex = 2;
                }
                else
                {
                    StudentSignup1.ActivityId = value;
                    WeekendTabs.ActiveTabIndex = 1;
                }
            }
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);

            if (WeekendId == -1)
            {
                StudentSignupTab.Enabled = false;
                DutyMemberSingupView.Enabled = false;
                LogInformation("No Weekend Activities have been input yet.  Aborting.");
            }
            if(user.IsTeacher)
            {
                StudentSignupTab.Visible = false;
                DutyMemberSingupView.Visible = true;
                LogInformation("Loading Teacher View.");
            }
            else
            {
                StudentSignupTab.Visible = true;
                DutyMemberSingupView.Visible = false;
                LogInformation("Loading Student View");
            }

            if (Request.QueryString.AllKeys.Contains("activity"))
            {
                SelectedActivity = Convert.ToInt32(Request.QueryString["activity"]);
                LogInformation("Getting activity {0} from QueryString.", SelectedActivity);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if(user.IsStudent && (!DateRange.WeekendDays.Contains(DateTime.Today.DayOfWeek) || (DateTime.Today.DayOfWeek == DayOfWeek.Friday && (DateTime.Now.Hour < 11 ||(DateTime.Now.Hour == 11 && DateTime.Now.Minute < 30)))))
                {
                    LogInformation("Student Signups Hidden due to DateTime restrictions.");
                    StudentSignup1.Visible = false;
                    NotAvailablePanel.Visible = true;
                }
            }
        }
    }
}