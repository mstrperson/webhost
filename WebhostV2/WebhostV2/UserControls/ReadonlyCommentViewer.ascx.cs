using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class ReadonlyCommentViewer : LoggingUserControl
    {
        public void LoadComment(int CommentId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                StudentComment comment = db.StudentComments.Where(c => c.id == CommentId).Single();

                CommentLabel.Text = String.Format("Comment for {0} {1} in {2}", comment.Student.FirstName, comment.Student.LastName, comment.CommentHeader.Section.Course.Name);

                EffortGradeLabel.Text = comment.EffortGrade.Name;
                TrimesterGradeLabel.Text = comment.TermGrade.Name;
                ExamGradeLabel.Text = comment.ExamGrade.Name;
                FinalGradeLabel.Text = comment.FinalGrade.Name;

                CommentPreview.Content = comment.CommentHeader.HTML + comment.HTML;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}