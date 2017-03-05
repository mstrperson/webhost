using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection;
using System.Security;

namespace WebhostV2
{
    public partial class Login : System.Web.UI.Page
    {
        private void ADLoginControl1_StateChanged(object o, EventArgs e)
        {
            if (Session[State.AuthUser] == null)
            {
                return;
            }

            String redirectedURL = (String)Session["RedURL"];

            if (!String.IsNullOrEmpty(redirectedURL))
            {
                Session["RedURL"] = null;
                Response.Redirect(redirectedURL);
            }
            ADUser user = (ADUser)Session[State.AuthUser];
            if (user.IsTeacher && (DateTime.Today.DayOfWeek != DayOfWeek.Saturday && DateTime.Today.DayOfWeek != DayOfWeek.Sunday))
            {
                DateRange classDay = new DateRange(DateTime.Today.AddHours(8).AddMinutes(30), DateTime.Today.AddHours(16));
                if (DateTime.Today.DayOfWeek == DayOfWeek.Thursday) classDay = classDay.MoveByHours(1);
                if(Session["RedURL"] != null)
                {
                    Response.Redirect((String)Session["RedURL"]);
                }
                else if(user.IsTeacher)
                {
                    using(WebhostEntities db = new WebhostEntities())
                    {
                        Faculty faculty = db.Faculties.Where(f => f.ID == user.ID).Single();
                        if (faculty.Preferences.Where(p => p.Name.Equals("HomePage")).Count() > 0)
                            Response.Redirect(faculty.Preferences.Where(p => p.Name.Equals("HomePage")).Single().Value);
                        else if (classDay.Contains(DateTime.Now))
                            Response.Redirect("~/TakeAttendance.aspx");
                        else
                            Response.Redirect("~/Home.aspx");
                    }
                }                
            }
            else
            {
                Response.Redirect("~/Home.aspx");
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(Request.QueryString["email"]) && !String.IsNullOrEmpty(Request.QueryString["pwd"]))
            {
                using (SecureString sstr = new SecureString())
                {
                    foreach (char ch in Request.QueryString["pwd"])
                    {
                        sstr.AppendChar(ch);
                    }

                    sstr.MakeReadOnly();

                    String username = Request.QueryString["email"];
                    if (username.Contains("@"))
                    {
                        username = username.Split('@')[0];
                    }

                    using(WebhostEntities db = new WebhostEntities())
                    {
                        Variable lockedOut;
                        String lockvn = String.Format("locked_out:{0}", username);
                        if (db.Variables.Where(v => v.Name.Equals(lockvn)).Count() > 0)
                        {
                            lockedOut = db.Variables.Where(v => v.Name.Equals(lockvn)).Single();
                            if (lockedOut.Value.Equals("true"))
                            {
                                // Send Forbidden status code.
                                Response.StatusCode = 403;
                                Response.ClearContent();
                                Response.End();
                            }
                        }
                        else
                        {
                            lockedOut = new Variable()
                            {
                                id = db.Variables.OrderBy(v => v.id).ToList().Last().id + 1,
                                Name = String.Format("locked_out:{0}", username),
                                Value = "false"
                            };
                            db.Variables.Add(lockedOut);
                            db.SaveChanges();
                        }
                        String authtimevn = String.Format("auth_time:{0}", username);
                        String spamvn = String.Format("spam_count:{0}", username);
                        if (db.Variables.Where(v => v.Name.Equals(authtimevn)).Count() > 0)
                        {
                            Variable auth_time = db.Variables.Where(v => v.Name.Equals(authtimevn)).Single();
                            Variable spam_count = db.Variables.Where(v => v.Name.Equals(spamvn)).Single();
                            if((DateTime.Now - DateTime.FromBinary(Convert.ToInt64(auth_time.Value))).TotalSeconds < 5)
                            {
                                auth_time.Value = Convert.ToString(DateTime.Now.ToBinary());

                                int scount = Convert.ToInt32(spam_count.Value);
                                spam_count.Value = Convert.ToString(++scount);

                                db.SaveChanges();

                                if (scount > 5)
                                {
                                    MailControler.MailToWebmaster("Login API Spam", String.Format("{0} has been sending authentication requests very quickly...", username));

                                }

                                Response.StatusCode = 429;
                                Response.ClearContent();
                                Response.End();
                            }

                            auth_time.Value = Convert.ToString(DateTime.Now.ToBinary());
                            db.SaveChanges();
                        }
                        else
                        {
                            Variable new_auth_time = new Variable() 
                            { 
                                id = db.Variables.OrderBy(v => v.id).ToList().Last().id + 1, 
                                Name = String.Format("auth_time:{0}", username), 
                                Value = Convert.ToString(DateTime.Now.ToBinary())
                            };

                            Variable new_spam_count = new Variable()
                            {
                                id = new_auth_time.id + 1,
                                Name = String.Format("spam_count:{0}", username),
                                Value = "0"
                            };

                            db.Variables.Add(new_auth_time);
                            db.Variables.Add(new_spam_count);
                            db.SaveChanges();
                        }
                    
                        Variable failCount;
                        string fcvn = String.Format("auth_fail_count:{0}", username);
                        if (db.Variables.Where(v => v.Name.Equals(fcvn)).Count() > 0)
                        {
                            failCount = db.Variables.Where(v => v.Name.Equals(fcvn)).Single();
                        }
                        else
                        {
                            failCount = new Variable()
                            {
                                id = db.Variables.OrderBy(v => v.id).ToList().Last().id + 1,
                                Name = String.Format("auth_fail_count:{0}", username),
                                Value = "0"
                            };
                        }

                        int count = Convert.ToInt32(failCount.Value);
                        if (count > 5)
                        {
                            MailControler.MailToWebmaster("I'm a Teapot", String.Format("There have been too many failed password attempts by {0} against the Login API.  You have to manually fix this.", username));
                            lockedOut.Value = "true";
                            db.SaveChanges();
                            // If you fail password more than 5 times, you get nonsense until that is fixed.
                            Response.StatusCode = 418;
                            Response.ClearContent();
                            Response.End();
                        }
                        ADUser user = (new ADUser(username, sstr));
                        if (user.Authenticated)
                        {
                            byte[] fingerprint = {};

                            if(user.IsStudent)
                            {
                                Student student = db.Students.Find(user.ID);
                                if(student.Fingerprints.Where(f => !f.IsDeactivated && !f.IsDeleted).Count() > 0)
                                {
                                    fingerprint = student.Fingerprints.Where(f => !f.IsDeactivated && !f.IsDeleted).ToList().First().Value;
                                }
                                else
                                {
                                    Fingerprint fp = new Fingerprint()
                                    {
                                        Id = db.Fingerprints.OrderBy(f => f.Id).ToList().Last().Id + 1,
                                        IsDeleted = false,
                                        IsDeactivated = false,
                                        IsInUse = true,
                                        Value = WebhostMySQLConnection.AccountManagement.AccountManagement.GenerateNewFingerprint()
                                    };

                                    db.Fingerprints.Add(fp);

                                    student.Fingerprints.Add(fp);
                                    db.SaveChanges();

                                    fingerprint = fp.Value;
                                }
                            }
                            else
                            {
                                Faculty faculty = db.Faculties.Find(user.ID);
                                if(faculty.Fingerprint.Id > 0 && !faculty.Fingerprint.IsDeactivated && !faculty.Fingerprint.IsDeleted)
                                {
                                    fingerprint = faculty.Fingerprint.Value;
                                }
                                else if (faculty.Fingerprints.Where(f => !f.IsDeactivated && !f.IsDeleted).Count() > 0)
                                {
                                    fingerprint = faculty.Fingerprints.Where(f => !f.IsDeactivated && !f.IsDeleted).ToList().First().Value;
                                }
                                else
                                {
                                    Fingerprint fp = new Fingerprint()
                                    {
                                        Id = db.Fingerprints.OrderBy(f => f.Id).ToList().Last().Id + 1,
                                        IsDeleted = false,
                                        IsDeactivated = false,
                                        IsInUse = true,
                                        Value = WebhostMySQLConnection.AccountManagement.AccountManagement.GenerateNewFingerprint()
                                    };

                                    db.Fingerprints.Add(fp);

                                    faculty.CurrentFingerprintId = fp.Id;
                                    faculty.Fingerprints.Add(fp);
                                    db.SaveChanges();

                                    fingerprint = fp.Value;
                                }
                            }

                            failCount.Value = "0";
                            db.SaveChanges();
                            Response.StatusCode = 200;
                            Response.ClearContent();
                            Response.ContentType = "text/plain";
                            Response.Write(Convert.ToBase64String(fingerprint));

                            Response.End();
                        }
                        else
                        {
                            failCount.Value = Convert.ToString(++count);
                            db.SaveChanges();
                            
                            Response.StatusCode = 401;
                            Response.ClearContent();
                            Response.End();
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string redurl = (String)Session["RedURL"];
            if (!String.IsNullOrEmpty(redurl))
            {

                ADLoginControl1.Message = String.Format("You must login before accessing {0}", redurl);
            }

            ADLoginControl1.StateChanged += new EventHandler(ADLoginControl1_StateChanged);

            if(!Request.Browser.Type.Contains("Chrome"))
                BrowserMessage.Text = String.Format("As hard as it is for me to admit--the best browser to use for this site is Chrome.{0}Some elements may not display correctly in {1}", Environment.NewLine, Request.Browser.Type);
        }
    }
}