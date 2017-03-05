using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using WebhostMySQLConnection.AccountManagement;

using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class PasswordResetForm : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                UserNameLabel.Text = String.Format("{0} <{1}@dublinschool.org>", ((BasePage)Page).user.Name, ((BasePage)Page).user.UserName);
                ErrorPanel.Visible = false;
                SuccessPanel.Visible = false;
                RegisterWifi.Visible = ((BasePage)Page).user.IsStudent;
            }
        }

        protected void ResetBtn_Click(object sender, EventArgs e)
        {
            State.log.WriteLine("Attempting to reset password.");
            LogInformation("Attempting to reset password.");
            ADSuccessMessage.Text = "";
            GoogleSuccessMessage.Text = "";
            if(OldPwdInput.Text.Equals(""))
            {
                ErrorMessage.Text = "You must type your old password.";
                LogError("User did not enter their old password.");
                State.log.WriteLine(ErrorMessage.Text);
                ErrorPanel.Visible = true;
                return;
            }

            if(!NewPwdInput.Text.Equals(RepeatPwdInput.Text))
            {
                ErrorMessage.Text = "New Password doesn't match Re-entered Password.";
                LogError("New Password doesn't match.");
                State.log.WriteLine(ErrorMessage.Text);
                ErrorPanel.Visible = true;
                return;
            }

            try
            {
                State.log.WriteLine("Calling PasswordReset.ChangeAllPasswords");
                PasswordReset.ChangeAllPasswords(((BasePage)Page).user.UserName, OldPwdInput.Text, NewPwdInput.Text);
            }
            catch(PasswordReset.PasswordException ex)
            {
                String Message = ex.Message;
                Exception inner = ex.InnerException;
                while(inner != null)
                {
                    Message += Environment.NewLine + "____________________________________" + Environment.NewLine + inner.Message;
                    inner = inner.InnerException;
                }

                ErrorMessage.Text = Message;
                State.log.WriteLine(ErrorMessage.Text);
                ErrorPanel.Visible = true;
                LogError(Message);
                MailControler.MailToWebmaster("Failed to change user password.", String.Format("{0} failed to change their active directory password:  {1}", user.UserName, Message), user);
                return;
            }
            catch(WebhostMySQLConnection.GoogleAPI.GoogleAPICall.GoogleAPIException gae)
            {
                String message = gae.Message;
                Exception inner = gae.InnerException;
                while(inner != null)
                {
                    message += Environment.NewLine + "_____________________________________" + Environment.NewLine + inner.Message;
                    inner = inner.InnerException;
                }
                
                MailControler.MailToWebmaster("Google Password Reset Failed...", message, ((BasePage)Page).user);
                State.log.WriteLine(message);
                LogError(message);
                GoogleSuccessMessage.Text = "Failed to Change Google Password.";
            }

            ADSuccessMessage.Text = "Successfully Changed Active Directory Password.";
            State.log.WriteLine("AD Password Changed Successfully.");
            if(GoogleSuccessMessage.Text.Equals(""))
            {
                GoogleSuccessMessage.Text = "Successfully Changed Google Password.";
                State.log.WriteLine("Google Password Changed Successfully.");
            }

            MailControler.MailToWebmaster("Successfully Changed Password", String.Format("for {0}", user.UserName), user);
            SuccessPanel.Visible = true;
        }

        protected void DismissBtn_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = false;
            OldPwdInput.Text = "";
            NewPwdInput.Text = "";
            RepeatPwdInput.Text = "";
        }

        protected void DoneBtn_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("http://mail.dublinschool.org");
        }

        protected void RegisterWifi_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RequestDeviceRegistration.aspx");
        }        
    }
}