using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class WeekendSignups : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            /*if(Request.Browser.IsMobileDevice)
            {
                Response.Redirect("~/Mobile/Weekend.aspx");
            }*/

            int id = DateRange.GetCurrentWeekendId();
            
            if(!Page.IsPostBack)
            {
                LogInformation("Loading Weekend Signpus Page.");
                if(!DateRange.WeekendDays.Contains(DateTime.Today.DayOfWeek) || (DateTime.Today.DayOfWeek == DayOfWeek.Friday && (DateTime.Now.Hour < 11 || (DateTime.Now.Hour == 11 && DateTime.Now.Minute < 30))))
                {
                    LogInformation("Signup's are not open yet!");
                    StudentSignupSelector1.Visible = false;
                    NotAvailablePanel.Visible = true;
                } 
                else if (id != -1)
                {
                    LogInformation("Loading Weekend!");
                    StudentSignupSelector1.WeekendId = id;
                    if (Request.QueryString.AllKeys.Contains("act_id"))
                    {
                        int actid = Convert.ToInt32(Request.QueryString["act_id"]);
                        LogInformation("Activity id {0} is selected.", actid);
                        StudentSignupSelector1.SetActiity(actid);
                    }
                    else
                    {
                        LogInformation("No activity is selected.");
                    }
                }
            }

            if(id != -1 && NotAvailablePanel.Visible)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(w => w.id == id).Single();

                    foreach(WeekendActivity activity in weekend.WeekendActivities.Where(act => act.showStudents && !act.IsDeleted).OrderBy(act=> act.DateAndTime))
                    {
                            ViewTable.Rows.Add(new StudentWeekendSignupViewRow(activity.id));
                    }
                }
            }
        }
    }
}