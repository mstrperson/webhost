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
    public partial class GradeReportView : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    StudentSelectBox.DataSource = StudentListItem.GetDataSource(db.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).Select(s => s.ID).ToList());
                    StudentSelectBox.DataTextField = "Text";
                    StudentSelectBox.DataValueField = "ID";
                    StudentSelectBox.DataBind();
                }

            }
        }

        protected void SelectBtn_Click(object sender, EventArgs e)
        {
            GradeReportDisplay1.StudentId = Convert.ToInt32(StudentSelectBox.SelectedValue);
        }
    }
}