using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2.Mobile
{
    public partial class CreditView : BasePage
    {
        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            StudentCreditReport1.ReadOnly = true;
            StudentCreditReport1.Visible = false;
            if (!user.IsTeacher) Response.Redirect("~/Home.aspx");

            using(WebhostEntities db = new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Find(user.ID);
                if(faculty == null)
                {
                    LogError("Could not locate faculty id {0}", user.ID);
                    Response.Redirect("~/Home.aspx");
                }

                AdviseeSelectCB.DataSource = StudentListItem.GetDataSource(faculty.Students.Where(s => s.isActive).Select(s => s.ID).ToList());
                AdviseeSelectCB.DataTextField = "Text";
                AdviseeSelectCB.DataValueField = "ID";
                AdviseeSelectCB.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SelectAdviseeBtn_Click(object sender, EventArgs e)
        {
            StudentCreditReport1.Visible = true;
            StudentCreditReport1.SelectedStudentId = Convert.ToInt32(AdviseeSelectCB.SelectedValue);
        }
    }
}