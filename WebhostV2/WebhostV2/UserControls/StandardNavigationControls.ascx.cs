using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class StandardNavigationControls : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BasePage page = (BasePage)Page;
            HomeLink.NavigateUrl = page.user.HomePage;
            WeekendLink.NavigateUrl = page.user.WeekendPage;
            CommentLink.NavigateUrl = page.user.CommentLettersPage;
            if(page.user.IsTeacher)
            {
                CourseRequestLink.Visible = true;
                CourseRequestLink.NavigateUrl = "~/MidYearCourseRequest.aspx";
                DeviceRegistration.Visible = false;
            }
            else
            {
                CourseRequestLink.Visible = false;
                DeviceRegistration.Visible = true;
                DeviceRegistration.NavigateUrl = "~/RequestDeviceRegistration.aspx";
            }
            using (WebhostEntities db = new WebhostEntities())
            {
                foreach(WebPageTag tag in db.WebPageTags.ToList())
                {
                    TagLinkPanel tlp = (TagLinkPanel)LoadControl("~/UserControls/TagLinkPanel.ascx");
                    tlp.TagId = tag.id;
                    LinkPanel.Controls.Add(tlp);
                }
            }
        }
    }
}