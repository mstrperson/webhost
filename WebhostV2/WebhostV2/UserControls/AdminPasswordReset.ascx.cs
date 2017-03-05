using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostV2.UserControls;
using WebhostMySQLConnection.GoogleAPI;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.AccountManagement;

namespace WebhostV2.UserControls
{
    public partial class AdminPasswordReset : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                EmailSelect.DataSource = EmailListItem.GetDataSource(true, true, false);
                EmailSelect.DataTextField = "Text";
                EmailSelect.DataValueField = "Value";
                EmailSelect.DataBind();
            }
        }

        protected void ResetBtn_Click(object sender, EventArgs e)
        {
            String response = "";
            bool success = true;
            String passwd = RandomizeCB.Checked ? WebhostMySQLConnection.AccountManagement.AccountManagement.GenerateRandomPassword() : NewPasswordInput.Text;
            try
            {
                PasswordReset.ChangeAllPasswords(EmailSelect.SelectedValue, "", passwd, "dublinschool.org", true);
                                    response = String.Format("Password for {0}@dublinschool.org has been set to {1}{2}Please visit https://webhost.dublinschool.org/PasswordInitialization.aspx to set your own password.", EmailSelect.SelectedValue, passwd, Environment.NewLine);

            }
            catch (GoogleAPICall.GoogleAPIException ge)
            {
                response = ge.Message;
                Exception inner = ge.InnerException;
                while(inner != null)
                {
                    response += Environment.NewLine + inner.Message;
                    inner = inner.InnerException;
                }
                success = false;
                LogError("Failed to reset password:{0}{0}{1}", Environment.NewLine, response);
            }

            if(success && !ResetInfoEmail.Text.Equals(""))
            {
                MailControler.MailToUser("Password Reset", response, ResetInfoEmail.Text, EmailSelect.SelectedValue);
            }

            MailControler.MailToWebmaster("Password Reset Info", response);
            LogInformation("Password for {0} has been reset by {1}", EmailSelect.SelectedValue, bPage.user.Name);    
        }

        protected void RandomizeCB_CheckedChanged(object sender, EventArgs e)
        {
            NewPasswordInput.Visible = !RandomizeCB.Checked;
        }
    }
}