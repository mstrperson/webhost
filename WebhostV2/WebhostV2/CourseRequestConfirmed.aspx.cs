using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2
{
    public partial class CourseRequestConfirmed : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CR_SID"] == null) Response.Redirect("~/CourseRequest.aspx");
            using (WebhostEntities db = new WebhostEntities())
            {
                int sid = (int)Session["CR_SID"];
                Student student = db.Students.Where(s => s.ID == sid).Single();
                List<WebhostMySQLConnection.CourseRequest> coursereq = student.CourseRequests.Where(cr => cr.TermId == 4).ToList();

                Output.Text = String.Format("Course Request for {0} {1} ({2}){3}{3}", student.FirstName, student.LastName, student.GraduationYear, Environment.NewLine);

                Output.Text += String.Format("English:{0}_________________________________{0}", Environment.NewLine);
                foreach(WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 2))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}History:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 3))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}Science:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 0))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}Math:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 4))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}World Language:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 5))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}Technology:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 1))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}Arts:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 9))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}Other:{0}_________________________________{0}", Environment.NewLine);
                foreach (WebhostMySQLConnection.CourseRequest cr in coursereq.Where(c => c.RequestableCourse.Course.DepartmentID == 8 || c.RequestableCourse.Course.DepartmentID == 6))
                {
                    Output.Text += String.Format("{0}\t{1}{2}", cr.RequestableCourse.Course.Name, cr.IsSecondary ? "Alternative" : (cr.IsGlobalAlternate ? "If Nothing Else" : ""), Environment.NewLine);
                }

                Output.Text += String.Format("{0}Notes:{0}_________________________________{0}{1}", Environment.NewLine,
                    student.CourseRequestComments.Where(crn => crn.TermId==4).Count() > 0? student.CourseRequestComments.Where(crn => crn.TermId==4).Single().Notes:"");

            }
        }

        protected void GoBack_Click(object sender, EventArgs e)
        {
            Session["CR_SID"] = null;
            Response.Redirect("~/CourseRequest.aspx");
        }
    }
}