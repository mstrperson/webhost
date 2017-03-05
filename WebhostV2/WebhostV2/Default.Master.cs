using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2
{
    public partial class Default : System.Web.UI.MasterPage
    {
        public event EventHandler ErrorCleared;
        public event EventHandler SuccessConfirmed;

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
                WebPagePermissionsEditor1.Visible = user.Permissions.Contains(PermissionControl.GetPermissionByName("Administrator").id);
                username_label.Text = String.Format("Logged in as:  {0} ({1}@dublinschool.org)", user.Name, user.UserName);
                using(WebhostEntities db = new WebhostEntities())
                {
                    int termid = DateRange.GetCurrentOrLastTerm();
                    Term term = db.Terms.Where(t => t.id == termid).Single();

                    ActiveTermLabel.Text = String.Format("{0} {1} ({2})", term.Name, term.StartDate.Year, term.AcademicYearID);
                }
            }
        }

        public void ShowError(String header, string message)
        {
            MasterErrorHeader.Text = header;
            MasterErrorMessage.Text = message;
            MasterError.Visible = true;
            WebhostEventLog.Syslog.LogError("{0}:  {1}", header, message);
        }

        public void ShowSuccess(String header, string message)
        {
            MasterSuccess.Visible = true;
            MasterSuccessHeader.Text = header;
            MasterSuccessMessage.Text = message;
            WebhostEventLog.Syslog.LogInformation("{0}:  {1}", header, message);
        }

        protected void LogoutBtn_Click(object sender, EventArgs e)
        {
            ((BasePage)Page).user.Logout();
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordInitialization.aspx");
        }

        protected void ErrorClearBtn_Click(object sender, EventArgs e)
        {
            MasterError.Visible = false;
            if(ErrorCleared != null)
                ErrorCleared(this, new EventArgs());
        }

        protected void ConfirmBtn_Click(object sender, EventArgs e)
        {
            MasterSuccess.Visible = false;
            if (SuccessConfirmed != null)
                SuccessConfirmed(this, new EventArgs());
        }

    }
}