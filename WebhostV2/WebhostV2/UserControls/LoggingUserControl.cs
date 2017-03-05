using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.IO;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public class LoggingUserControl : System.Web.UI.UserControl
    {
        public EventLog syslog
        {
            get
            {
                if (!EventLog.SourceExists("Webhost Page Log"))
                {
                    EventLog.CreateEventSource("Webhost Page Log", "Application");
                }
                if (Session["syslog"] == null)
                {
                    Session["syslog"] = new EventLog() { Source = "Webhost Page Log" };
                }

                return (EventLog)Session["syslog"];
            }
        }

        public ADUser user
        {
            get
            {
                if (Page is BasePage)
                {
                    return bPage.user;
                }
                else return null;
            }        
        }

        public String AddInfoTag(String message)
        {
            if (user != null)
            {
                // get Json Data Contract info from the ADUser logged in currently.
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(ADUser));
                MemoryStream mstr = new MemoryStream();
                json.WriteObject(mstr, user);
                mstr.Position = 0;
                StreamReader sr = new StreamReader(mstr);
                String info = sr.ReadToEnd();
                sr.Close();
                mstr.Close();
                sr.Dispose();
                mstr.Dispose();
                return String.Format("{0}{1}____________________________{1}{1}{2}", message, Environment.NewLine, info);
            }
            else
                return String.Format("{0}{1}***No User Details Available.", message, Environment.NewLine);
        }

        public void LogError(String message)
        {
            syslog.WriteEntry(AddInfoTag(TimeStampMessage(message)), EventLogEntryType.Error);
        }

        public void LogError(String format, params object[] arg)
        {
            syslog.WriteEntry(AddInfoTag(TimeStampMessage(format, arg)), EventLogEntryType.Error);
        }

        public void LogWarning(String message)
        {
            syslog.WriteEntry(AddInfoTag(TimeStampMessage(message)), EventLogEntryType.Warning);
        }

        public void LogWarning(String format, params object[] arg)
        {
            syslog.WriteEntry(AddInfoTag(TimeStampMessage(format, arg)), EventLogEntryType.Warning);
        }

        public void LogInformation(String message)
        {
            syslog.WriteEntry(AddInfoTag(TimeStampMessage(message)), EventLogEntryType.Information);
        }

        public void LogInformation(String format, params object[] arg)
        {
            syslog.WriteEntry(AddInfoTag(TimeStampMessage(format, arg)), EventLogEntryType.Information);
        }

        public String TimeStampMessage(String message)
        {
            return BasePage.TimeStampMessage(String.Format("{1}[{3} -> {0}]{1}{2}", this.ID, Environment.NewLine, message, Request.RawUrl));
        }

        public String TimeStampMessage(String format, params object[] arg)
        {
            return TimeStampMessage(String.Format(format, arg));
        }

        public BasePage bPage
        {
            get
            {
                return (BasePage)base.Page;
            }
        }
    }
}