using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection.Web;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Admin.Directory.directory_v1;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Admin.Directory.directory_v1.Data;
using System.Text.RegularExpressions;

namespace WebhostMySQLConnection.GoogleAPI
{
    public class GoogleDirectoryCall : GoogleAPICall
    {
        public GoogleDirectoryCall() : base() { }

        /// <summary>
        /// Add a user to a Google Group!
        /// </summary>
        /// <param name="userName">UserName field from Webhost</param>
        /// <param name="groupId">Google Group Email Address</param>
        /// <param name="role">Role in the group, "MEMBER" is default, "OWNER" and "MANAGER" are alternatives.</param>
        public void AddUserToGroup(String userName, String groupId, String role = "MEMBER")
        {
            SetCredential(new[] { DirectoryService.Scope.AdminDirectoryGroupMember });
            DirectoryService service = new DirectoryService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Dublin School Webhost Gmail"
            });

            Member newEntry = new Member()
            {
                Email = string.Format("{0}@dublinschool.org", userName),
                Role = role
            };
            try
            {
                service.Members.Insert(newEntry, groupId).Execute();
                WebhostEventLog.GoogleLog.LogInformation("Inserted {0} into group {1} as a {2}", userName, groupId, role);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Member already exists."))
                {
                    State.log.WriteLine("{0} is already a member of {1}", userName, groupId);
                    WebhostEventLog.GoogleLog.LogInformation("{0} is already a member of {1}", userName, groupId);
                }
                else
                {
                    WebhostEventLog.GoogleLog.LogError("Failed to insert {0} into group {1} as a {2}.  {3}", userName, groupId, role, e.Message);
                    throw e;
                }
            }
        }

        /// <summary>
        /// Takes the Active Students and Faculty and puts them in the Commons and Faculty groups appropriately.
        /// </summary>
        public void UpdateCommonsAndFacultyGroups()
        {
            SetCredential(new[] { DirectoryService.Scope.AdminDirectoryGroupMember });

            DirectoryService service = new DirectoryService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Dublin School Webhost Gmail"
            });

            WebhostEventLog.GoogleLog.LogInformation("Updating Commons and Faculty groups on Gmail.");

            Members commonsMembers = service.Members.List("all-school@dublinschool.org").Execute();
            Members facultyMembers = service.Members.List("faculty@dublinschool.org").Execute();

            using (WebhostEntities db = new WebhostEntities())
            {
                List<String> alreadyMembers = new List<string>();

                List<String> activeStudentEmails = new List<string>();
                foreach (Student student in db.Students.Where(s => s.isActive).ToList())
                {
                    activeStudentEmails.Add(String.Format("{0}@dublinschool.org", student.UserName));
                }

                List<String> activeFacultyEmails = new List<string>();
                foreach (Faculty faculty in db.Faculties.Where(f => f.isActive).ToList())
                {
                    activeFacultyEmails.Add(String.Format("{0}@dublinschool.org", faculty.UserName));
                }

                if (activeFacultyEmails.Count <= 0 || activeStudentEmails.Count <= 0)
                {
                    WebhostEventLog.GoogleLog.LogError("Aborting--something is wrong with the Active Students/Faculty lists.");
                    throw new GoogleAPIException("Aborting--something is wrong with the Active Students/Faculty lists.");
                }

                #region Commons Members

                foreach (Member member in commonsMembers.MembersValue)
                {
                    if (!activeFacultyEmails.Contains(member.Email) && !activeStudentEmails.Contains(member.Email))
                    {
                        service.Members.Delete("all-school@dublinschool.org", member.Email).Execute();
                        WebhostEventLog.GoogleLog.LogWarning("Removing {0} from Commons.", member.Email);
                    }
                    else
                    {
                        alreadyMembers.Add(member.Email);
                        WebhostEventLog.GoogleLog.LogInformation("{0} is already subscribed to commons.", member.Email);
                    }
                }

                foreach (String email in activeStudentEmails.Concat(activeFacultyEmails))
                {
                    if (!alreadyMembers.Contains(email))
                    {
                        Member newEntry = new Member()
                        {
                            Email = email,
                            Role = "MEMBER"
                        };

                        try
                        {
                            service.Members.Insert(newEntry, "all-school@dublinschool.org").Execute();
                            WebhostEventLog.GoogleLog.LogInformation("Added {0} to Commons.", email);
                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("Member already exists."))
                            {
                                State.log.WriteLine("{0} is already a member of Commons.", email);
                                WebhostEventLog.GoogleLog.LogInformation("{0} is already a member of Commons", email);
                            }
                            else
                            {
                                WebhostEventLog.GoogleLog.LogError("Failed to insert {0} into Commons.  {1}", email, e.Message);
                                throw e;
                            }
                        }
                    }
                }

                #endregion

                alreadyMembers.Clear();

                #region Faculty Members

                // Currently I am not automatically removing anyone from the faculty group.  Some members of the faculty group are not in the Webhost database.

                foreach (Member member in facultyMembers.MembersValue)
                {
                    if (activeFacultyEmails.Contains(member.Email))
                    {
                        alreadyMembers.Add(member.Email);
                        WebhostEventLog.GoogleLog.LogInformation("{0} is already subscribed to commons.", member.Email);
                    }
                }

                foreach (String email in activeFacultyEmails)
                {
                    if (!alreadyMembers.Contains(email))
                    {
                        Member newEntry = new Member()
                        {
                            Email = email,
                            Role = "MEMBER"
                        };
                        try
                        {
                            service.Members.Insert(newEntry, "faculty@dublinschool.org").Execute();
                        }
                        catch (Exception e)
                        {
                            if (e.Message.Contains("Member already exists."))
                            {
                                State.log.WriteLine("{0} is already a member of Faculty.", email);
                                WebhostEventLog.GoogleLog.LogInformation("{0} is already a member of Faculty", email);
                            }
                            else
                            {
                                WebhostEventLog.GoogleLog.LogError("Failed to insert {0} into Faculty.  {1}", email, e.Message);
                                throw e;
                            }
                        }
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// Set the user's GMail Password
        /// </summary>
        /// <param name="username">Pass only the UserName here--not the full email</param>
        /// <param name="Password">New Password to be set</param>
        public void SetPassword(String username, String Password)
        {
            State.log.WriteLine("Attempting to retrieve UserResource.");
            WebhostEventLog.GoogleLog.LogInformation("{0} is attempting to reset their password.", username);
            SetCredential(new[] { DirectoryService.Scope.AdminDirectoryUser }, "jason");

            try
            {
                // Open the Directory Service
                DirectoryService service = new DirectoryService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Webhost Google Directory"
                });

                State.log.WriteLine("Service Connected.");
                UsersResource.GetRequest gr = service.Users.Get(String.Format("{0}@dublinschool.org", username));
                User user = gr.Execute();
                State.log.WriteLine("Got User object.");
                user.Password = Password;
                service.Users.Update(user, String.Format("{0}@dublinschool.org", username)).Execute();
                State.log.WriteLine("User password updated.");
                WebhostEventLog.GoogleLog.LogInformation("{0} successfully changed their password.");
            }
            catch (Exception e)
            {
                WebhostEventLog.GoogleLog.LogError("{0} failed to update their password:  {1}", username, e.Message);
                throw new GoogleAPIException(String.Format("Failed to Update password for {0}.", username), e);
            }
        }

        public bool SetUsersFlagAcceptTerms(String username)
        {
            State.log.WriteLine("Attempting to retrieve UserResource.");

            SetCredential(new[] { DirectoryService.Scope.AdminDirectoryUser }, "jason");

            try
            {
                // Open the Directory Service
                DirectoryService service = new DirectoryService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Webhost Google Directory"
                });

                State.log.WriteLine("Service Connected.");
                UsersResource.GetRequest gr = service.Users.Get(String.Format("{0}@dublinschool.org", username));
                User user = gr.Execute();
                State.log.WriteLine("Got User object.");
                if (user.AgreedToTerms.HasValue && user.AgreedToTerms.Value)
                    return false;
                user.AgreedToTerms = true;
                service.Users.Update(user, String.Format("{0}@dublinschool.org", username)).Execute();
                State.log.WriteLine("User AgreedToTerms updated.");
            }
            catch (Exception e)
            {
                throw new GoogleAPIException(String.Format("Failed to Update {0} status.", username), e);
            }
            return true;
        }

        public void UpdateStudentOUs()
        {
            SetCredential(new[] { DirectoryService.Scope.AdminDirectoryUser });
            DirectoryService service = new DirectoryService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Webhost Google Directory"
            });

            int currentYear = DateRange.GetCurrentAcademicYear();

            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Student student in db.Students.Where(s => s.GraduationYear < currentYear))
                {
                    String email = String.Format("{0}@dublinschool.org", student.UserName);
                    User user;
                    try
                    {
                        user = service.Users.Get(email).Execute();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to get Email for {0}", email);
                        Console.WriteLine(e.Message);
                        continue;
                    }

                    if(user.OrgUnitPath.Equals("/students"))
                    {
                        user.OrgUnitPath = "/Graduated Seniors";
                        service.Users.Update(user, email).Execute();
                        Console.WriteLine("Moved {0} {1} to Graduated Seniors", student.FirstName, student.LastName);
                    }
                }

                foreach(Student student in db.Students.Where(s => s.GraduationYear >= currentYear && !s.isActive))
                {
                    String email = String.Format("{0}@dublinschool.org", student.UserName);
                    User user;
                    try
                    {
                        user = service.Users.Get(email).Execute();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to get Email for {0}", email);
                        Console.WriteLine(e.Message);
                        continue;
                    }
                    if (user.OrgUnitPath.Equals("/students"))
                    {
                        user.OrgUnitPath = "/Suspended-Left";
                        service.Users.Update(user, email).Execute();
                        Console.WriteLine("Moved {0} {1} to Suspended-Left", student.FirstName, student.LastName);
                    }
                }

                foreach(Faculty faculty in db.Faculties.Where(f => !f.isActive))
                {
                    String email = String.Format("{0}@dublinschool.org", faculty.UserName);
                    User user;
                    try
                    {
                        user = service.Users.Get(email).Execute();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to get Email for {0}", email);
                        Console.WriteLine(e.Message);
                        continue;
                    }

                    user.OrgUnitPath = "/Suspended-Left";
                    service.Users.Update(user, email).Execute();
                    Console.WriteLine("Moved {0} {1} to Suspended-Left", faculty.FirstName, faculty.LastName);
                    
                }
            }
        }

        /// <summary>
        /// Create a new Gmail Account
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="OU">One of { "students", "faculty", "staff" }</param>
        public void CreateEmail(String firstName, String lastName, String username, String password, String OU = "/")
        {
            SetCredential(new[] { DirectoryService.Scope.AdminDirectoryUser });
            // Open the Directory Service
            DirectoryService service = new DirectoryService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Webhost Google Directory"
            });

            User newUser = new User()
            {
                PrimaryEmail = String.Format("{0}@dublinschool.org", username),
                Password = password,
                OrgUnitPath = String.Format("/{0}", OU),
                AgreedToTerms = true,
                Name = new UserName() { FamilyName = lastName, GivenName = firstName, FullName = String.Format("{0} {1}", firstName, lastName) }
            };

            try
            {
                newUser = service.Users.Insert(newUser).Execute();
                WebhostEventLog.GoogleLog.LogInformation("Successfully created new user.", typeof(User), newUser);
            }
            catch (Exception re)
            {
                if (re.Message.Contains("Entity already exists."))
                {
                    WebhostEventLog.GoogleLog.LogError("User already exists.", typeof(User), newUser);
                    throw new GoogleAPIException("User already has an email~");
                }
                else
                {
                    WebhostEventLog.GoogleLog.LogError("Could not create user. {0}", re.Message);
                    throw new GoogleAPIException(String.Format("Could not create user. {0}", re.Message), re);
                }
            }
        }
    }
}
