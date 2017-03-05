using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WebhostV2
{
    public class BasePage : Page
    {
        public BasePage() :base()
        {
            //unnecessary - set in Web.config
            //Server.ScriptTimeout = 600;
        }

        //private static EventLog _syslog;
        public EventLog syslog
        {
            get
            {
                if(!EventLog.SourceExists("Webhost Page Log"))
                {
                    EventLog.CreateEventSource("Webhost Page Log", "Application");
                }
                if(Session["syslog"] == null)
                {
                    Session["syslog"] = new EventLog() { Source = "Webhost Page Log" };
                }

                return (EventLog)Session["syslog"];
            }
        }

        /// <summary>
        /// List of Permissions which will grant access to this page.
        /// All permissions are OR.
        /// </summary>
        public List<Permission> RequiredPermissions
        {
            get
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if (db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Count() <= 0) return new List<Permission>();

                    WebPage page = db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Single();
                    return page.Permissions.ToList();
                }
            }
        }

        public Log log
        {
            get
            {
                try
                {
                    return (Log)Session["log"];
                }
                catch
                {
                    LogWarning("Failed to get Session Log.");
                    Log temp = new Log(String.Format("UnnamedLog{0}", DateTime.Now.ToFileTimeUtc()), Server);
                    temp.WriteLine("Unknown Error Log!");
                    return temp;
                }
            }
        }

        public ADUser user
        {
            get
            {
                return (ADUser)Session[State.AuthUser];
            }
        }

        public int AdminMasqueradeTeacherId
        {

            get
            {
                //ADUser user = (ADUser)Session[State.AuthUser];
                if (!(AccessLevel.Administrator(user) || AccessLevel.DeanOfAcademics(user)) || Session["amtid"] == null)
                    return -1;
                else
                    return (int)Session["amtid"];
            }
            set
            {
                syslog.WriteEntry(TimeStampMessage("{0} is attempting to masquerading as Teacher Id:  {1}", user.UserName, value), EventLogEntryType.Warning);
                //ADUser user = (ADUser)Session[State.AuthUser];
                if (AccessLevel.Administrator(user) || AccessLevel.DeanOfAcademics(user))
                {
                    Session["amtid"] = value;
                    LogInformation("Masquerade allowed.");
                }
                else
                {
                    Session["amtid"] = -1;//throw new FieldAccessException("Illegal Access of Admin Masquerade by " + user.UserName);
                    LogError("Masquerade denied.");
                    MailControler.MailToWebmaster("Masquerade Access Denied.", String.Format("{0} is attempting to masquerading as Teacher Id:  {1}", user.UserName, value), user);
                }
            }
        }
        #region Logging

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
                return String.Format("{0}{1}____________________________{1}[{3}]{1}____________________________{1}{1}{2}", message, Environment.NewLine, info, Request.RawUrl);
            }
            else
                return String.Format("{0}{1}[{2}]", message, Environment.NewLine, Request.RawUrl);
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

        public static String TimeStampMessage(String value)
        {
            return String.Format("{0}{2}{1}", Log.TimeStamp(), value, Environment.NewLine);
        }

        private static String TimeStampMessage(String format, params object[] list)
        {
            return TimeStampMessage(String.Format(format, list));
        }
        #endregion
        public Boolean IsRedirect
        {
            get
            {
                return Session["RedFrom"] != null;
            }
        }

        public void Redirect(String URL)
        {
            Session["RedURL"] = Request.RawUrl;
            Response.Redirect(URL);
        }

        public String RedirectedUrl
        {
            get
            {
                return Session["RedURL"] == null ? "" : (String)Session["RedURL"];
            }
        }

        public String UserFingerprint
        {
            get
            {
                return (String)Session["fingerprint"];
            }
            protected set
            {
                Session["fingerprint"] = value;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if(Request.QueryString.AllKeys.Contains("fingerprint"))
            {
                UserFingerprint = Request.QueryString["fingerprint"];

                using(WebhostEntities db = new WebhostEntities())
                {
                    byte[] fingerprintData = Convert.FromBase64String(UserFingerprint);
                    bool found = false;
                    foreach (Fingerprint fp in db.Fingerprints.ToList())
                    {
                        if (fp.Value.SequenceEqual(fingerprintData))
                        {
                            try
                            {
                                Session[State.AuthUser] = new ADUser(fp);
                                found = true;
                                Session["log"] = new Log(String.Format("{0}_SessionLog", ((ADUser)Session[State.AuthUser]).UserName), Server);
                            }
                            catch (InvalidDataException)
                            {
                                // Invalid Fingerprint.
                                Response.ClearContent();
                                Response.StatusCode = 403;
                                Response.End();
                            }
                        }
                    }

                    if (!found)
                    {
                        // Bad Fingerprint.
                        Response.ClearContent();
                        Response.StatusCode = 403;
                        Response.End();
                    }
                }
            }

            try
            {
                log.WriteLine("Page_Init @ {0}", Request.RawUrl);
            }
            catch(Exception ex)
            {
                LogError("Failed to write to log file: {0}", ex.Message);
            }


            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            if (user == null || !user.Authenticated)
            {
                LogWarning("User Must Login before accessing {0}, redirecting to login.", Request.RawUrl);
                Redirect("~/Login.aspx");
            }

            // Check Permissions
            using (WebhostEntities db = new WebhostEntities())
            {
                bool ok = RequiredPermissions.Count == 0;
                if (!ok && user.IsTeacher)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == user.ID).Single();
                    foreach(Permission requiredPermission in RequiredPermissions)
                    {
                        if (faculty.Permissions.Where(p => p.id ==requiredPermission.id).Count() > 0)
                        {
                            ok = true;
                            LogInformation("{0} has required permission {1} to access {2}", user.Name, requiredPermission.Name, Request.RawUrl);
                            break;
                        }
                    }
                } 
                else if (!ok)
                {
                    Student student = db.Students.Where(f => f.ID == user.ID).Single();
                    foreach (Permission requiredPermission in RequiredPermissions)
                    {
                        if (student.Permissions.Where(p => p.id == requiredPermission.id).Count() > 0)
                        {
                            ok = true;
                            LogInformation("{0} has required permission {1} to access {2}", user.Name, requiredPermission.Name, Request.RawUrl);
                            break;
                        }
                    }
                }

                if (!ok)
                {
                    MailControler.MailToWebmaster("Access Denied", String.Format("{0} was refused access to {1} due to insufficient permissions.", user.Name, Request.RawUrl));
                    LogError("{0} was denied access to {1} due to insufficient permissions.", user.Name, Request.RawUrl);
                    Response.Redirect("~/Home.aspx");
                }

                Session["RedURL"] = Request.RawUrl;
                LogInformation("Page_Init @ {0} by {1}", Request.RawUrl, user.Name);
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            try
            {
                log.WriteLine("Page_Error @ {0}", Request.RawUrl);
            }
            catch (Exception ex)
            {
                LogError("Failed to write to log file: {0}", ex.Message);
            }


            Exception Err = Server.GetLastError();

            //ADUser user = (ADUser)Session[State.AuthUser];
            if (user == null)
            {
                LogError("Page error with no user logged in on {1}.{0}{0}Error details:{0}{2}{0}____________________________{0}{0}Stack Trace:{0}{3}",
                    Environment.NewLine, Request.RawUrl, Err.Message, Err.StackTrace);
                return;
            }
            MailControler.MailToWebmaster("Page Error in Webhost.", "Webhost had an Page error somewhere...", user);

            String ErrString = Err.ToString();

            String StackTrace = Err.StackTrace;

            String AllMessages = Err.Message;

            while (Err.InnerException != null)
            {
                Err = Err.InnerException;
                AllMessages += "\n\nInner Exception:\n" + Err.Message;
                StackTrace += "\n\nInner Exception:\n" + Err.StackTrace;
                ErrString += "\n\nInner Exception:\n" + Err.ToString();
            }
            String Message = "Error Produced by " + user.Name + " at " + DateTime.Now.ToLongTimeString() + "\n"
                               + "Page:\t" + Request.Url + "\n\n"
                               + "Error Details:\n\n" + ErrString + "\n\n"
                               + "__________________________________________________________________________________________________________\n\n"
                               + "All Messages:\n\n" + AllMessages + "\n\n"
                               + "__________________________________________________________________________________________________________\n\n"
                               + "Stack Traces:\n\n" + StackTrace;

            MailControler.MailToWebmaster("Page Error Details on " + Request.Url, Message, user);
            LogError(Message);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                log.WriteLine("Application_Error @ {0}", Request.RawUrl);
            }
            catch (Exception ex)
            {
                LogError("Failed to write to log file: {0}", ex.Message);
            }//ADUser user = (ADUser)Session[State.AuthUser];

            Exception Err = Server.GetLastError();
            String ErrString = Err.ToString();

            String StackTrace = Err.StackTrace;

            String AllMessages = Err.Message;

            while (Err.InnerException != null)
            {
                Err = Err.InnerException;
                AllMessages += "\n\nInner Exception:\n" + Err.Message;
                StackTrace += "\n\nInner Exception:\n" + Err.StackTrace;
                ErrString += "\n\nInner Exception:\n" + Err.ToString();
            }

            if(user == null)
            {
                LogError("Application error with no user logged in on {1}.{0}{0}Error details:{0}{2}{0}____________________________{0}{0}Stack Trace:{0}{3}",
                    Environment.NewLine, Request.RawUrl, AllMessages, Err.StackTrace);
                return;
            }
            

            String Message = "Application Error Produced by " + user.Name + " at " + DateTime.Now.ToLongTimeString() + "\n"
                                   + "Page:\t" + Request.RawUrl + "\n\n"
                                   + "Error Details:\n\n" + ErrString + "\n\n"
                                   + "__________________________________________________________________________________________________________\n\n"
                                   + "All Messages:\n\n" + AllMessages + "\n\n"
                                   + "__________________________________________________________________________________________________________\n\n"
                                   + "Stack Traces:\n\n" + StackTrace;
            MailControler.MailToWebmaster("Application Error Details on " + Request.Url, Message, user);
            LogError(Message);
        }
    }
}