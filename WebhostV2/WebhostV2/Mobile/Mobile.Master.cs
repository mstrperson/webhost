using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2.Mobile
{
    public partial class Mobile : System.Web.UI.MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            //Page.RegisterStartupScript("MyScript", "<script language=javascript> function background() { var backgroundNumber = Math.floor((Math.random() * 25)); document.body.style.background = \"url(images/Backgrounds/'\" + backgroundNumber + \".jpg')\"; alert(backgroundNumber);}</script>");
            //form1.Attributes.Add("onload", "background()");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[State.AuthUser] == null || !((ADUser)Session[State.AuthUser]).Authenticated)
                Response.Redirect("~/Login.aspx");

            else
            {
                ADUser user = (ADUser)Session[State.AuthUser];
                username_label.Text = String.Format("Logged in as:  {0} ({1}@dublinschool.org)", user.Name, user.UserName);
                using (WebhostEntities db = new WebhostEntities())
                {
                    int termid = DateRange.GetCurrentOrLastTerm();
                    Term term = db.Terms.Where(t => t.id == termid).Single();

                    ActiveTermLabel.Text = String.Format("{0} {1} ({2})", term.Name, term.StartDate.Year, term.AcademicYearID);
                }
            }
        }

        protected void LogoutBtn_Click(object sender, EventArgs e)
        {
            ((BasePage)Page).user.Logout();
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordInitialization.aspx");
        }
    }
}