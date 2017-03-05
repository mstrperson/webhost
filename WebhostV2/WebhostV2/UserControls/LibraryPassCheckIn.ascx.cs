using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class LibraryPassCheckIn : LoggingUserControl
    {
        public int PassId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(PassIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    LibraryPass pass = db.LibraryPasses.Where(p => p.id == value).Single();
                    PassIdField.Value = Convert.ToString(value);
                    NameLabel.Text = String.Format("{0} {1} ({2})", pass.Student.FirstName, pass.Student.LastName, pass.Student.GraduationYear);
                    if(pass.StudyHallSignatureId.HasValue)
                    {
                        Info.Text = String.Format("Left Study Hall at {0}--{1} {2}", pass.StudyHallCheckOutSignature.TimeStamp.ToShortTimeString(), pass.StudyHallCheckOutSignature.Faculty.FirstName, pass.StudyHallCheckOutSignature.Faculty.LastName);
                        if(pass.LibrarySignatureId.HasValue)
                        {
                            Info.Text += String.Format("{0}Signed Into Library at {1}--{2} {3}", Environment.NewLine, pass.LibraryCheckInSignature.TimeStamp.ToShortTimeString(), pass.LibraryCheckInSignature.Faculty.FirstName, pass.LibraryCheckInSignature.Faculty.LastName);
                            if (pass.LibraryCheckInSignature.AltTimeStamp.HasValue)
                            {
                                SignBtn.Text = "Sign Back In";
                            }
                            else
                            {
                                SignBtn.Text = "Sign Out";
                            }
                        }
                    }
                    else
                    {
                        Info.Text = "Pass does not have a valid Study Hall Signature!";
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SignBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                LibraryPass pass = db.LibraryPasses.Where(p => p.id == PassId).Single();
                if(pass.LibrarySignatureId.HasValue && !pass.LibraryCheckInSignature.AltTimeStamp.HasValue)
                {
                    pass.LibraryCheckInSignature.AltTimeStamp = DateTime.Now;
                    SignBtn.Text = "Signed Back In";
                }
                else if(pass.LibrarySignatureId.HasValue)
                {
                    pass.LibraryCheckInSignature.AltTimeStamp = null;
                    SignBtn.Text = "Sign Out Again";
                }
                else
                {
                    int sigid = db.TimeStampedSignatures.Count() > 0? db.TimeStampedSignatures.OrderBy(i => i.id).ToList().Last().id +1:0;
                    TimeStampedSignature sig = new TimeStampedSignature()
                    {
                        id = sigid,
                        FacultyId = ((BasePage)Page).user.ID,
                        TimeStamp = DateTime.Now
                    };
                    db.TimeStampedSignatures.Add(sig);
                    pass.LibrarySignatureId = sigid;
                    SignBtn.Text = "Sign Out";
                }

                db.SaveChanges();
                PassId = pass.id;
            }
        }
    }
}