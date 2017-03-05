using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.AccountManagement
{
    public class AccountManagement
    {

        private static String _acctPwd;
        private static String AcctPwd
        {
            get
            {
                if (String.IsNullOrEmpty(_acctPwd))
                {
                    using (WebhostEntities db = new WebhostEntities())
                    {
                        _acctPwd = db.Variables.Where(v => v.Name.Equals("accounts")).Single().Value;
                    }
                }

                return _acctPwd;
            }
        }

        protected static String lower = "abcdefghijklmnopqrstuvwxyz";
        protected static String upper = lower.ToUpper();
        protected static String numeric = "0123456789";
        protected static String symbols = "~!@#$%^&*+=?<>";

        protected static Random rand = new Random();

        public static void GeneratePasswords(ref CSV newUsers)
        {
            foreach(Dictionary<String,String> row in newUsers.Data)
            {
                row["password"] = GenerateRandomPassword();
            }
        }

        private enum objectClass
        {
            user, group, computer
        }
        private enum returnType
        {
            distinguishedName, ObjectGUID
        }

        private static string GetObjectDistinguishedName(objectClass objectCls, returnType returnValue, string objectName, string LdapDomain = "192.168.76.3")
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

        public struct ADUserTemplate
        {
            public String FirstName;
            public String LastName;
            public String UserName;
            public String Password;
            public String Description;
            public String PrimaryGroup;
            public List<String> AdditionalGroups;
        }

        public static String GetGroupDN(String groupName)
        {
            return GetObjectDistinguishedName(objectClass.group, returnType.distinguishedName, groupName).Replace("LDAP://", "LDAP://192.168.76.3/");
        }

        public static bool ADUserExists(String userName)
        {
            String DN = "";
            try
            {
                DN = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, userName);
            }
            catch
            {
                return false;
            }

            return !DN.Equals("");
        }

        private static String GetHomeDirectory(String primaryGroup, String userName)
        {
            String server = @"\\zoe.dublinschool.org\";
            if (primaryGroup.Equals("Staff"))
                server = @"\\admin-dms.dublinschool.org\Personal\";
            else
            {
                if (primaryGroup.Equals("Faculty"))
                {
                    server += @"faculty\";
                }
                else if (primaryGroup.Equals("Students"))
                {
                    server += @"students\";
                }
                else
                    throw new FormatException("Invalid Primary Group " + primaryGroup);
            }

            return server + userName;
        }

        private static String GetProfileDirectory(String primaryGroup, String userName)
        {
            String server = @"\\zoe.dublinschool.org\";
            if (primaryGroup.Equals("Staff"))
                server = @"\\admin-dms.dublinschool.org\Profiles\";
            else
            {
                if (primaryGroup.Equals("Faculty"))
                {
                    server += @"faculty profiles\";
                }
                else if (primaryGroup.Equals("Students"))
                {
                    server += @"student profiles\";
                }
                else
                    throw new FormatException("Invalid Primary Group " + primaryGroup);
            }

            return server + userName;
        }

        public static bool DeleteADUser(String userName)
        {
            if(!ADUserExists(userName)) return false;

            String DN = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, userName).Replace("LDAP://", "LDAP://192.168.76.3/");
            DirectoryEntry user = new DirectoryEntry(DN, "accounts", AcctPwd);
            user.DeleteTree();
            user.Close();

            return true;
        }

        public static bool VerifyADUserAccountDirectories(string username)
        {
            if(!username.Contains('_'))
            {
                return false;
            }

            DirectoryEntry user = new DirectoryEntry(GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, username).Replace("LDAP://", "LDAP://192.168.76.3/"), "accounts", AcctPwd);

            user.Properties["HomeDirectory"].Value = GetHomeDirectory("Students", username);
            user.Properties["HomeDrive"].Value = "H:";
            user.Properties["ProfilePath"].Value = GetProfileDirectory("Students", username);
            user.CommitChanges();
            return true;
        }

        public static void CreateADUser(ADUserTemplate template)
        {
            DirectoryEntry dirEntry = new DirectoryEntry("LDAP://192.168.76.3", "accounts", AcctPwd);
            DirectoryEntry newUser = dirEntry.Children.Add(String.Format("CN={0},CN=Users", template.UserName), "user");
            newUser.CommitChanges();
            newUser.Properties["sAMAccountName"].Value = template.UserName;
            newUser.CommitChanges();
            int val = (int)newUser.Properties["userAccountControl"].Value;
            val = val | 0x10000;
            val = val & ~0x2;
            newUser.Properties["userAccountControl"].Value = val;
            newUser.CommitChanges();

            /*newUser.InvokeSet("Description", template.Description);
            newUser.InvokeSet("DisplayName", String.Format("{0} {1}", template.FirstName, template.LastName));
            newUser.InvokeSet("HomeDirectory", String.Format("\\\\zoe.dublinschool.org\\students\\{0}", template.UserName));
            newUser.InvokeSet("HomeDrive", "H");
            newUser.InvokeSet("ProfilePath", String.Format("\\\\zoe.dublinschool.org\\student profiles\\{0}", template.UserName));*/
            if (!template.Description.Equals(""))
                newUser.Properties["Description"].Value = template.Description;
            newUser.Properties["DisplayName"].Value = String.Format("{0} {1}", template.FirstName, template.LastName);
            newUser.Properties["HomeDirectory"].Value = GetHomeDirectory(template.PrimaryGroup, template.UserName);
            newUser.Properties["HomeDrive"].Value = "H:";
            newUser.Properties["ProfilePath"].Value = GetProfileDirectory(template.PrimaryGroup, template.UserName);            

            newUser.CommitChanges();
            newUser.Invoke("SetPassword", template.Password);

            DirectoryEntry primaryGroup = new DirectoryEntry(GetGroupDN(template.PrimaryGroup), "accounts", AcctPwd);
            primaryGroup.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
            primaryGroup.CommitChanges();
            primaryGroup.Close();

            foreach(String groupName in template.AdditionalGroups)
            {
                DirectoryEntry grp = new DirectoryEntry(GetGroupDN(groupName), "accounts", AcctPwd);
                grp.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
                grp.CommitChanges();
                grp.Close();
            }
        }

        public static List<byte[]> GenerateBulkFingerprints(int count, int length = 256)
        {
            List<byte[]> prints = new List<byte[]>();

            Random rand = new Random();
            for (int i = 0; i < count; i++ )
            {
                byte[] print = new byte[length];
                rand.NextBytes(print);

                prints.Add(print);
            }
            
            return prints;
        }

        public static byte[] GenerateNewFingerprint(int length = 256)
        {
            byte[] fingerprint = new byte[length];

            Random rand = new Random();
            rand.NextBytes(fingerprint);

            return fingerprint;
        }

        public static String GenerateRandomPassword()
        {
            String pwd = "";
            
            List<String> charsets = new List<string>() {lower, upper, numeric, symbols};
            List<String> chosen = new List<string>();

            for (int i = 0; i < 3; i++ )
            {
                int n = rand.Next(charsets.Count);
                chosen.Add(charsets[n]);
                charsets.RemoveAt(n);
            }

            foreach(String lst in chosen)
            {
                for(int i=0; i<3;i++)
                {
                    int choice = rand.Next(lst.Length);
                    pwd += lst[choice];
                }
            }

            return pwd;
        }
    }
}
