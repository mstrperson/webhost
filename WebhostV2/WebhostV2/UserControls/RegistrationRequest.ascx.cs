using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class RegistrationRequest : LoggingUserControl
    {
        protected static readonly bool EmailEnabled = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                OtherInput.Attributes.Add("onfocus", "CheckValue(this, 'What kind of device is this?')");
                OtherInput.Attributes.Add("onblur", "CheckValue(this, 'What kind of device is this?')");
            }
        }

        protected bool ValidateMAC()
        {
            Regex validMac = new Regex(@"([a-fA-F0-9]{2}[:\- ]?){5}[a-fA-F0-9]{2}");
            return validMac.IsMatch(MACAddrInput.Text);
        }

        protected String MACAddress
        {
            get
            {
                if (!ValidateMAC()) return "000000000000";
                string mac = MACAddrInput.Text;
                Regex nondigit = new Regex("[^a-fA-F0-9]");
                foreach (Match match in nondigit.Matches(mac))
                {
                    mac = mac.Replace(match.Value, "");
                }

                return mac;
            }
        }

        protected void DeviceTypeDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            OtherInput.Visible = DeviceTypeDDL.SelectedValue.Equals("Other");
            OtherLabel.Visible = OtherInput.Visible;
            if (OtherInput.Visible) OtherInput.Focus();
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            ShortRequest shreq = new ShortRequest() { StudentName = user.Name, DeviceType = DeviceType, MACAddress = MACAddrInput.Text };
            if(!ValidateMAC())
            {
                ErrorMessage.Text = "The MAC Address you entered is not valid--please check to make sure it is correct.";
                LogError("Invalid MAC Address for {0}", shreq.ToString());
                ErrorPanel.Visible = true;
                return;
            }
            using(WebhostEntities db = new WebhostEntities())
            {
                //Check for Existing Mac Address Request.
                if(db.RegistrationRequests.Where(req => req.MacAddress.Equals(MACAddress)).Count() > 0)
                {
                    WebhostMySQLConnection.RegistrationRequest request = db.RegistrationRequests.Where(req => req.MacAddress.Equals(MACAddress)).Single();
                    if(request.RequestCompleted)
                    {
                        ErrorMessage.Text = "This Device has already been registered.  If you are experiencing difficulties, email Mr. Cox or Mr. Harrison.";
                        LogError("The device has already been registered: {0}", shreq.ToString());
                        MailControler.MailToUser("Your Registration Request has already been registered.", shreq.ToString(), user);
                    }
                    else if(request.RequestDenied)
                    {
                        ErrorMessage.Text = "This device has been rejected.  Please see Mr. Cox or Mr. Harrison for details.";
                        LogError("Registration Request has been rejected: {0}", shreq.ToString());
                        MailControler.MailToUser("Your Registration Request has been rejected.", shreq.ToString(), user);
                    }
                    else
                    {
                        ErrorMessage.Text = "This Device is pending registration.  Don't worry--we'll get to it soon =)";
                        LogError("Impatient User: {0}", shreq);
                        MailControler.MailToUser("Your Registration Request is pending review.", shreq.ToString(), user);
                    }

                    ErrorPanel.Visible = true;
                    return;
                }

                int reqid = db.RegistrationRequests.Count() > 0 ? db.RegistrationRequests.OrderBy(req => req.id).ToList().Last().id + 1 : 0;
                WebhostMySQLConnection.RegistrationRequest nrequest = new WebhostMySQLConnection.RegistrationRequest()
                {
                    id = reqid,
                    StudentId = ((BasePage)Page).user.IsStudent?((BasePage)Page).user.ID:10,
                    MacAddress = MACAddress,
                    DeviceType = DeviceType,
                    RequestCompleted = false,
                    RequestDenied = false
                };

                db.RegistrationRequests.Add(nrequest);
                db.SaveChanges();

                LogInformation("New Request submitted: {0}", shreq.ToString());
                if (EmailEnabled)
                {
                    MailControler.MailToWebmaster("New Device Registration Request.", shreq.ToString(), user);
                    MailControler.MailToUser("New Device Registration Request.", shreq.ToString(), "jeff@dublinschool.org", "Jeff Harrison", user);
                    MailControler.MailToUser("Your Registration Request has been successfully submitted.", shreq.ToString(), user);
                }
                SuccessPanel.Visible = true;
            }
        }

        public class ShortRequest
        {
            public String StudentName { get; set; }
            public String MACAddress { get; set; }
            public String DeviceType { get; set; }

            public override string ToString()
            {
                return String.Format("{0}{1}MAC:\t{2}{1}Type:\t{3}{1}", StudentName, Environment.NewLine, MACAddress, DeviceType);
            }
        }

        protected String DeviceType
        {
            get
            {
                return DeviceTypeDDL.SelectedValue.Equals("Other") ?
                OtherInput.Text :
                DeviceTypeDDL.SelectedValue;
            }
        }

        protected void HelpBtn_Click(object sender, EventArgs e)
        {
            String query = String.Format("http://www.google.com/search?q=Get+{0}+MAC+Address&ie=utf-8&oe=utf-8", DeviceType);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + query + "','_blank')", true);
        }

        protected void ClearErrorBtn_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = false;
        }

        protected void OkBtn_Click(object sender, EventArgs e)
        {
            MACAddrInput.Text = "";
            SuccessPanel.Visible = false;
        }
    }
}