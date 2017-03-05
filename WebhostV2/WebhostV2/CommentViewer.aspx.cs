using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class CommentViewer : BasePage
    {
        protected int SelectedTermId
        {
            get
            {
                if(Session["viewer_sel_term"] == null)
                {
                    return -1;
                }

                return (int)Session["viewer_sel_term"];
            }

            set
            {
                Session["viewer_sel_term"] = value;
            }
        }

        protected int SelectedClassId
        {
            get
            {
                if (Session["viewer_sel_class"] == null)
                {
                    return -1;
                }

                return (int)Session["viewer_sel_class"];
            }

            set
            {
                Session["viewer_sel_class"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if (this.user.IsTeacher) Response.Redirect("~/AdviseeCommentReview.aspx");
                using(WebhostEntities db = new WebhostEntities())
                {
                    int sid = this.user.ID;
                    Student student = db.Students.Where(s => s.ID == sid).Single();
                    List<int> termIds = new List<int>();
                    foreach(StudentComment comment in student.StudentComments.ToList())
                    {
                        if (!termIds.Contains(comment.CommentHeader.TermIndex) && comment.CommentHeader.Term.CommentsDate < DateTime.Today)
                            termIds.Add(comment.CommentHeader.TermIndex);
                    }

                    TermSelect.DataSource = TermListItem.GetDataSource(termIds);
                    TermSelect.DataTextField = "Text";
                    TermSelect.DataValueField = "ID";
                    TermSelect.DataBind();

                    ClassSelect.Enabled = false;
                    ReadonlyCommentViewer1.Visible = false;
                }
            }
        }

        protected void TermSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassSelect.Enabled = true;
            using(WebhostEntities db = new WebhostEntities())
            {
                int tid = Convert.ToInt32(TermSelect.SelectedValue);
                int sid = this.user.ID;
                Student student = db.Students.Where(s => s.ID == sid).Single();
                    
                List<int> sectionIds = new List<int>();
                foreach(StudentComment comment in student.StudentComments.ToList())
                {
                    if(comment.CommentHeader.TermIndex == tid)
                    {
                        sectionIds.Add(comment.CommentHeader.SectionIndex);
                    }
                }

                ClassSelect.DataSource = SectionListItem.GetDataSource(sectionIds);
                ClassSelect.DataTextField = "Text";
                ClassSelect.DataValueField = "ID";
                ClassSelect.DataBind();
                ReadonlyCommentViewer1.Visible = false;
            }
        }

        protected void ClassSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReadonlyCommentViewer1.Visible = true;
            int comid = -1;
            using(WebhostEntities db = new WebhostEntities())
            {
                int tid = Convert.ToInt32(TermSelect.SelectedValue);
                int sid = this.user.ID;
                int cid = Convert.ToInt32(ClassSelect.SelectedValue);
                Student student = db.Students.Where(s => s.ID == sid).Single();

                comid = student.StudentComments.Where(com => com.CommentHeader.TermIndex == tid && com.CommentHeader.SectionIndex == cid).Single().id;
            }

            ReadonlyCommentViewer1.LoadComment(comid);
        }
    }
}