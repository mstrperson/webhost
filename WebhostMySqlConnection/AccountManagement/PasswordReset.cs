using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using WebhostMySQLConnection.GoogleAPI;
using System.Text.RegularExpressions;
using WebhostMySQLConnection.Web;
using System.DirectoryServices.AccountManagement;

namespace WebhostMySQLConnection.AccountManagement
{
    public class PasswordReset
    {
        private static String _acctPwd;
        private static String AcctPwd
        {
            get
            {
                if(String.IsNullOrEmpty(_acctPwd))
                {
                    using (WebhostEntities db = new WebhostEntities())
                    {
                        _acctPwd = db.Variables.Where(v => v.Name.Equals("accounts")).Single().Value;
                    }
                }

                return _acctPwd;
            }
        }

        private static String LdapRootPath
        {
            get
            {
                return "LDAP://192.168.76.3/";
            }
        }

        /// <summary>
        /// Throws a PasswordException if password does not meet minimum windows requirements.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="Password"></param>
        public static void AssertPasswordRequirements(String Password)
        {
            if (Password.Length < 8) throw new PasswordException("Password must be at least 8 characters long.");

            Regex complexity = new Regex(@"(?=^.{8,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*");
            if (!complexity.IsMatch(Password)) throw new PasswordException("Password does not meet minimum complexity requirements.");
        }
        private enum objectClass
        {
            user, group, computer
        }
        private enum returnType
        {
            distinguishedName, ObjectGUID
        }

        private static string GetObjectDistinguishedName(objectClass objectCls, returnType returnValue, string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;
            string connectionPrefix = "LDAP://" + LdapDomain;
            //objectName = "dublinschool.org\\" + objectName;
            DirectoryEntry entry = new DirectoryEntry(connectionPrefix, "accounts", AcctPwd);
            DirectorySearcher mySearcher = new DirectorySearcher(entry);

            switch (objectCls)
            {
                case objectClass.user:
                    mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
                    break;
                case objectClass.group:
                    mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
                case objectClass.computer:
                    mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
                    break;
            }
            SearchResult result = mySearcher.FindOne();

            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                objectName + " in the " + LdapDomain + " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            if (returnValue.Equals(returnType.distinguishedName))
            {
                distinguishedName = "LDAP://" + directoryObject.Properties
                    ["distinguishedName"].Value;
            }
            if (returnValue.Equals(returnType.ObjectGUID))
            {
                distinguishedName = directoryObject.Guid.ToString();
            }
            entry.Close();
            entry.Dispose();
            mySearcher.Dispose();
            return distinguishedName;
        }

        public static void ForceChangeADPassword(String username, String newPassword)
        {
            String DN = "";
            try
            {
                DN = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, username, "192.168.76.3");
            }
            catch(Exception e)
            {
                throw new PasswordException(String.Format("Could not find AD User {0}", username), e);
            }

            if(DN.Equals(""))
                throw new PasswordException(String.Format("Could not find AD User {0}", username));

            DirectoryEntry userEntry = new DirectoryEntry(DN.Replace("LDAP://", LdapRootPath), "accounts", AcctPwd);
            userEntry.Invoke("SetPassword", new object[] { newPassword });
            userEntry.Properties["LockOutTime"].Value = 0;

            userEntry.CommitChanges();
            userEntry.Close();
        }

        public static void ForceChangeGooglePassword(String userName, String newPassword)
        {
            try
            {
                State.log.WriteLine("Initiating Google API Call");
                using (GoogleDirectoryCall google = new GoogleDirectoryCall())
                {
                    State.log.WriteLine("Google API Call instantiated.  Calling SetPassword");
                    google.SetPassword(userName, newPassword);
                    State.log.WriteLine("SetPassword Completed.");
                }
            }
            catch (GoogleAPICall.GoogleAPIException gae)
            {
                throw gae;
            }
        }

        public static void ChangeAllPasswords(string userName, string currentPassword, string newPassword, string domainName = "dublinschool.org", bool force = false)
        {
            
            AssertPasswordRequirements(newPassword);
            State.log.WriteLine("Password Requirements Met.");
            WebhostEventLog.Syslog.LogInformation("Password Requirements are met for {0} new password.", userName);
            if (force)
            {
                WebhostEventLog.Syslog.LogInformation("Forcibly changing password for {0}", userName);
                ForceChangeADPassword(userName, newPassword);
            }
            else
            {
                try
                {
                    State.log.WriteLine("Connecting LDAP.");
                    DirectoryEntry directionEntry = new DirectoryEntry("LDAP://192.168.76.3" , domainName + "\\" + userName, currentPassword);
                    if (directionEntry != null)
                    {
                        DirectorySearcher search = new DirectorySearcher(directionEntry);
                        State.log.WriteLine("LDAP Connected, searching directory for SAMAccountName");
                        search.Filter = "(SAMAccountName=" + userName + ")";
                        SearchResult result = search.FindOne();
                        if (result != null)
                        {
                            State.log.WriteLine("Getting User Entry.");
                            DirectoryEntry userEntry = result.GetDirectoryEntry();
                            if (userEntry != null)
                            {
                                State.log.WriteLine("Setting Password");
                                if (force)
                                {
                                    userEntry.Invoke("SetPassword", new[] { newPassword });
                                }
                                else
                                {
                                    // Windows update broke this August 2016
                                    // userEntry.Invoke("ChangePassword", new object[] { currentPassword, newPassword });

                                    // This is a fix that should work...
                                    ForceChangeADPassword(userName, newPassword);
                                }
                                userEntry.CommitChanges();
                                State.log.WriteLine("Changes Committed to ActiveDirectory.");
                                WebhostEventLog.Syslog.LogInformation("Changes committed to ActiveDirectory for {0}", userName);
                            }
                            else
                            {
                                State.log.WriteLine("Could not get user Entry...");
                                WebhostEventLog.Syslog.LogError("Could not get user entry for {0}.", userName);
                            }
                        }
                        else
                        {
                            State.log.WriteLine("Search returned no results.");
                            WebhostEventLog.Syslog.LogError("Directory Search returned no results for {0}.", userName);
                        }
                    }
                    else
                    {
                        State.log.WriteLine("Could not connect to LDAP with given username and passwd");
                        WebhostEventLog.Syslog.LogError("Could not connect to LDAP with the given password for {0}.", userName);
                    }
                }
                catch (Exception ex)
                {
                    WebhostEventLog.Syslog.LogError("Could not reset Windows password for {0}.{1}{2}", userName, Environment.NewLine, ex.Message);
                    throw new PasswordException(String.Format("Failed to reset Windows Password for {0}.", userName), ex);
                }
            }
            try
            {
                State.log.WriteLine("Initiating Google API Call");
                WebhostEventLog.GoogleLog.LogInformation("Connecting to Google API for a password change for {0}.", userName);
                using (GoogleDirectoryCall google = new GoogleDirectoryCall())
                {
                    State.log.WriteLine("Google API Call instantiated.  Calling SetPassword");
                    WebhostEventLog.GoogleLog.LogInformation("Connected successfully with {0}.", userName);
                    google.SetPassword(userName, newPassword);
                    WebhostEventLog.GoogleLog.LogInformation("Successfully changed {0} password for Gmail.", userName);
                    State.log.WriteLine("SetPassword Completed.");
                }
            }
            catch(GoogleAPICall.GoogleAPIException gae)
            {
                WebhostEventLog.GoogleLog.LogError("Failed to change password for {0}.{1}{2}", userName, Environment.NewLine, gae.Message);
                throw gae;
            }
        }

        public String GenerateNewFingerprint(byte[] signatureImgData)
        {
            Random rand = new Random();
            return Convert.ToBase64String(signatureImgData);
        }

        public class PasswordException : Exception
        {
            public PasswordException(String message, Exception innerException = null) : base(message, innerException)
            {

            }
        }
    }
}
