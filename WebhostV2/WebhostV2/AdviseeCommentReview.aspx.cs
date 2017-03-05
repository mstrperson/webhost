using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.EVOPublishing;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class AdviseeCommentReview : BasePage
    {
        protected int SelectedAdvisee
        {
            get
            {
                if (Session["sel_adv_rev"] == null)
                    return -1;
                else
                    return (int)Session["sel_adv_rev"];
            }
            set
            {
                Session["sel_adv_rev"] = value;
            }
        }

        protected int SelectedTerm
        {
            get
            {
                if (Session["sel_term_rev"] == null) Session["sel_term_rev"] = DateRange.GetCurrentOrLastTerm();

                return (int)Session["sel_term_rev"];
            }
            set
            {
                Session["sel_term_rev"] = value;
            }
        }

        protected int SelectedComment
        {
            get
            {
                if (Session["sel_com_rev"] == null)
                    return -1;
                else
                    return (int)Session["sel_com_rev"];
            }
            set
            {
                Session["sel_com_rev"] = value;
            }
        }

        public class MiniTerm
        {
            public int id
            {
                get;
                set;
            }
            public String name
            {
                get;
                set;
            }
        }

        public class MiniComment
        {
            public int id
            {
                get;
                set;
            }
            public String className
            {
                get;
                set;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Faculty currentUser = db.Faculties.Where(f => f.ID == this.user.ID).Single();
                    List<int> advisees = currentUser.Students.Where(s => s.isActive).Select(s => s.ID).ToList();

                    int year = DateRange.GetCurrentAcademicYear();
                    int currentTerm = DateRange.GetCurrentOrLastTerm();

                    AcademicYear ay = db.AcademicYears.Where(y => y.id == year).Single();
                    
                    AdviseeSelectDDL.DataSource = StudentListItem.GetDataSource(advisees);
                    AdviseeSelectDDL.DataTextField = "Text";
                    AdviseeSelectDDL.DataValueField = "ID";
                    AdviseeSelectDDL.DataBind();

                    if (advisees.Contains(SelectedAdvisee))
                    {
                        AdviseeSelectDDL.ClearSelection();
                        AdviseeSelectDDL.SelectedValue = Convert.ToString(SelectedAdvisee);
                    }
                    else
                    {
                        SelectedAdvisee = Convert.ToInt32(AdviseeSelectDDL.SelectedValue);
                    }

                    TermSelectDDL.DataSource = TermListItem.GetDataSource(ay.Terms.Select(t => t.id).ToList());
                    TermSelectDDL.DataTextField = "Text";
                    TermSelectDDL.DataValueField = "ID";
                    TermSelectDDL.DataBind();

                    TermSelectDDL.ClearSelection();
                    TermSelectDDL.SelectedValue = Convert.ToString(SelectedTerm);

                    Student selectedAdvisee = currentUser.Students.Where(s => s.ID == SelectedAdvisee).Single();
                    List<MiniComment> comments = new List<MiniComment>();
                    
                    foreach(StudentComment comment in selectedAdvisee.StudentComments.Where(c => c.CommentHeader.TermIndex == SelectedTerm).ToList())
                    {
                        comments.Add(new MiniComment()
                            {
                                id = comment.id,
                                className = comment.CommentHeader.Section.Course.Name
                            });
                    }

                    if (comments.Count == 0)
                    {
                        ClassSelectDDL.Enabled = false;
                    }
                    else
                    {
                        ClassSelectDDL.Enabled = true;
                        ClassSelectDDL.DataSource = comments;
                        ClassSelectDDL.DataTextField = "className";
                        ClassSelectDDL.DataValueField = "id";
                        ClassSelectDDL.DataBind();

                        if (SelectedComment != -1)
                        {
                            ClassSelectDDL.ClearSelection();
                            ClassSelectDDL.SelectedValue = Convert.ToString(SelectedComment);
                        }
                        else
                        {
                            SelectedComment = Convert.ToInt32(ClassSelectDDL.SelectedValue);
                        }

                        AdviseeCommentViewer.LoadComment(SelectedComment);
                    }
                }
            }
        }

        protected void AdviseeSelectDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAdvisee = Convert.ToInt32(AdviseeSelectDDL.SelectedValue);
            SelectedComment = -1;
            Response.Redirect(Request.RawUrl);
        }

        protected void TermSelectDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTerm = Convert.ToInt32(TermSelectDDL.SelectedValue);
            SelectedComment = -1;
            Response.Redirect(Request.RawUrl);
        }

        protected void ClassSelectDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedComment = Convert.ToInt32(ClassSelectDDL.SelectedValue);
            AdviseeCommentViewer.LoadComment(SelectedComment);
        }

        protected void DownloadCurrentBtn_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();
            foreach(ListItem item in AdviseeSelectDDL.Items)
            {
                ids.Add(Convert.ToInt32(item.Value));
            }

            PublishRequest.RequestByStudent(user, ids, Server.MapPath(String.Format("~/Temp/{0}_advisees", user.UserName)));
        }
    }
}