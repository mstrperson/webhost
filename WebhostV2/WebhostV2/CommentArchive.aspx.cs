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
    public partial class CommentArchive : BasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            using (WebhostEntities db = new WebhostEntities())
            {
                int tid = this.user.ID;
                Faculty faculty = db.Faculties.Where(f => f.ID == tid).Single();
                List<int> commentTerms = new List<int>();
                foreach (Section section in faculty.Sections.ToList())
                {
                    foreach (Term term in section.Terms.ToList())
                    {
                        if (commentTerms.Contains(term.id))
                            continue;

                        if (section.CommentHeaders.Where(c => c.TermIndex == term.id).Count() > 0)
                            commentTerms.Add(term.id);
                    }
                }

                TermSelectDDL.DataSource = TermListItem.GetDataSource(commentTerms);
                TermSelectDDL.DataTextField = "Text";
                TermSelectDDL.DataValueField = "ID";
                TermSelectDDL.DataBind();

                CommentEditor1.ArchiveMode = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoadTerm_Click(object sender, EventArgs e)
        {
            CommentEditor1.SelectedTermId = Convert.ToInt32(TermSelectDDL.SelectedValue);
            CommentEditor1.ResetPage();
        }
    }
}