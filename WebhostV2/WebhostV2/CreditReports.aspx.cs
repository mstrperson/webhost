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
    public partial class CreditReports : BasePage
    {
        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e); 
            
            using (WebhostEntities db = new WebhostEntities())
            {
                StudentComboBox.DataSource = StudentListItem.GetDataSource(
                    db.Students.Where(s => s.isActive).OrderBy(s => s.GraduationYear).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).Select(s => s.ID).ToList());
                StudentComboBox.DataTextField = "Text";
                StudentComboBox.DataValueField = "ID";
                StudentComboBox.DataBind();

                
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SelectBtn_Click(object sender, EventArgs e)
        {
            StudentCreditReport1.SelectedStudentId = Convert.ToInt32(StudentComboBox.SelectedValue);
        }

        protected void CreditAuditReportBtn_Click(object sender, EventArgs e)
        {
            CSV report = CreditAudit.GetStudentCredits();
            report.Save(Server.MapPath("~/Temp/CreditReport.csv"));
            Response.Redirect("~/Temp/CreditReport.csv");
        }
    }
}