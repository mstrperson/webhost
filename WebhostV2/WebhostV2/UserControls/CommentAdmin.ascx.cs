using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.EVOPublishing;

namespace WebhostV2.UserControls
{
    public partial class CommentAdmin : LoggingUserControl
    {
        public event EventHandler Masquerade;

        public int MasqeradeId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(FacultyCmbBx.SelectedValue);
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    FacultyCmbBx.DataSource = (from faculty in db.Faculties
                                               orderby faculty.LastName, faculty.FirstName
                                               select new
                                               {
                                                   Name = faculty.FirstName + " " + faculty.LastName,
                                                   ID = faculty.ID
                                               }).ToList();
                    FacultyCmbBx.DataTextField = "Name";
                    FacultyCmbBx.DataValueField = "ID";
                    FacultyCmbBx.DataBind();
                }
            }
        }

        protected void DownloadAllBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int Termid = DateRange.GetCurrentOrLastTerm();
                List<int> activeStudents = db.Students.Where(s => s.StudentComments.Where(com => com.CommentHeader.TermIndex == Termid).Count() > 0).Select(s => s.ID).ToList();
                PublishRequest.RequestByStudent(((BasePage)Page).user, activeStudents, Server.MapPath("~/Temp/all_comments"));
            }
        }

        protected void MasqBtn_Click(object sender, EventArgs e)
        {
            if(Masquerade != null)
            {
                Masquerade(sender, e);
            }
        }

        protected void GetHeadersBtn_Click(object sender, EventArgs e)
        {
            ///Response.Redirect((new CommentHeaderReviewPDF()).Publish());
        }
    }
}