using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2
{
    public partial class Home : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(user == null || !user.Authenticated)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (user.IsStudent)
                Response.Redirect("~/WeekendSignups.aspx");

            switch(DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Saturday: HomeTabs.ActiveTabIndex = 1; break;
                case DayOfWeek.Sunday: HomeTabs.ActiveTabIndex = 1; break;
                case DayOfWeek.Friday:
                    if (DateTime.Now.Hour > 14)
                        HomeTabs.ActiveTabIndex = 1;
                    else
                        HomeTabs.ActiveTabIndex = 0;
                    break;
                default: HomeTabs.ActiveTabIndex = 0; break;
            }


        }
    }
}