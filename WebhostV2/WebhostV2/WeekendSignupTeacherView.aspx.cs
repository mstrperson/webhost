using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostV2.UserControls;
using WebhostMySQLConnection.EVOPublishing;

namespace WebhostV2
{
    public partial class WeekendSignupTeacherView : BasePage
    {
        protected int WeekendId
        {
            get
            {
                return DateRange.GetCurrentWeekendId();
            }
        }

        protected int SelectedActivity
        {
            get
            {
                if(Session["sel_act"] == null)
                {
                    return -1;
                }
                else
                {
                    try
                    {
                        return (int)Session["sel_act"];
                    }
                    catch
                    {
                        Session["sel_act"] = null;
                        return -1;
                    }
                }
            }
            set
            {
                Session["sel_act"] = value;
            }
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            if(SelectedActivity != -1)
            {
                WeekendTripAttendanceList1.ActivityId = SelectedActivity;
            }

            base.Page_Init(sender, e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.QueryString.AllKeys.Contains("act_id"))
            {
                SelectedActivity = Convert.ToInt32(Request.QueryString["act_id"]);
                Response.Redirect("~/WeekendSignupTeacherView.aspx");

                if (Request.Browser.IsMobileDevice)
                {
                    Response.Redirect(String.Format("~/Mobile/Weekend.aspx?activity={0}", Request.QueryString["act_id"]));
                }
            }

            if (Request.Browser.IsMobileDevice)
            {
                Response.Redirect("~/Mobile/Weekend.aspx");
            }
            if(!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    int wkid = DateRange.GetCurrentWeekendId();
                    if(wkid != -1)
                    {
                        Weekend weekend = db.Weekends.Where(w => w.id == wkid).Single();
                        ActivitySelectDDL.DataSource = ActivityListItem.GetDataSource(weekend.WeekendActivities.Where(act => act.showStudents && !act.IsDeleted).Select(act => act.id).ToList());
                        ActivitySelectDDL.DataTextField = "Text";
                        ActivitySelectDDL.DataValueField = "Id";
                        ActivitySelectDDL.DataBind();
                    }
                }
            }
        }

        protected void LoadActivityBtn_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedActivity = Convert.ToInt32(ActivitySelectDDL.SelectedValue);
                Response.Redirect(Request.RawUrl);
            }
            catch
            {
                // nothing selected
            }
        }

        protected void DownloadSchedule_Click(object sender, EventArgs e)
        {
            //WeekendDutySchedule wds = new WeekendDutySchedule(DateRange.GetCurrentWeekendId());
            //Response.Redirect(wds.Publish());
        }
    }
}