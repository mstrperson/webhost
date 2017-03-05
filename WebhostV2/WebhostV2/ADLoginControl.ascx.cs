using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class ADLoginControl : LoggingUserControl
    {
        public event System.EventHandler StateChanged;

        public String Message
        {
            set
            {
                ErrorLabel.Text = value;
                ErrorLabel.Visible = !value.Equals("");
            }
            get
            {
                return ErrorLabel.Text;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((ADUser)Session[State.AuthUser] != null && ((ADUser)Session[State.AuthUser]).Authenticated)
            {
                if (Session["RedURL"] != null)
                {
                    LogInformation("{0} is already authenticated--passing along to {1}", ((ADUser)Session[State.AuthUser]).Name, Session["RedURL"]);
                    Response.Redirect((String)Session["RedURL"]);
                }
                else
                {
                    LogInformation("{0} is already authenticated.", ((ADUser)Session[State.AuthUser]).Name);
                    Response.Redirect("~/Home.aspx");
                }
            }
        }

        protected void LoginoutBtn_Click(object sender, EventArgs e)
        {
            ErrorLabel.Visible = false;
            if (LoginoutBtn.Text.Equals("Login"))
            {
                LogInformation("Processing Loging Request for {0}.", UserNameInput.Text);
                using (SecureString sstr = new SecureString())
                {
                    foreach (char ch in PasswordInput.Text)
                    {
                        sstr.AppendChar(ch);
                    }

                    sstr.MakeReadOnly();

                    String username = UserNameInput.Text;
                    if(username.Contains("@"))
                    {
                        username = username.Split('@')[0];
                    }

                    if ((new ADUser(username, sstr)).Authenticated)
                    {
                        LogInformation("Login Accepted for {0}", username);
                        LoginoutBtn.Text = "Logout";
                        PasswordInput.Text = "*************************";
                        Session["log"] = new Log(String.Format("{0}_SessionLog", username), Server);
                        State.log.WriteLine("Successfully Authenticated at {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
                        if (this.StateChanged != null)
                        {
                            StateChanged(this, new EventArgs());
                        }


                    }
                    else
                    {
                        LogWarning("{0} was denied login--Invalid Credentials.", username);
                        ErrorLabel.Visible = true;
                        ErrorLabel.Text = "Invalid Credentials.";
                        Session.Clear();
                    }
                }
            }
            else
            {
                LogInformation("{0} logged out.", UserNameInput.Text);
                UserNameInput.Text = "";
                PasswordInput.Text = "";
                LoginoutBtn.Text = "Login";

                if (this.StateChanged != null)
                {
                    StateChanged(this, new EventArgs());
                }

                try
                {
                    ((Log)Session["log"]).Close();
                }
                catch(Exception ex)
                {
                    LogError("Failed to close Log file...{0}{1}", Environment.NewLine, ex.Message);
                }
                Session.Clear();
            }
        }
    }
}