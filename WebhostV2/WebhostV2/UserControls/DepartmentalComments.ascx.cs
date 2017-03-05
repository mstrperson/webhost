using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class DepartmentalComments : LoggingUserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int id = ((BasePage)Page).user.ID;
                Faculty faculty = db.Faculties.Where(fac => fac.ID == id).Single();
                List<int> depts = faculty.Departments.Select(d => d.id).ToList();
                DepartmentDDL.DataSource = DepartmentListItem.GetDataSource(depts);
                DepartmentDDL.DataTextField = "Text";
                DepartmentDDL.DataValueField = "ID";
                DepartmentDDL.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void LoadDept_Click(object sender, EventArgs e)
        {
            Session["dhmode"] = Convert.ToInt32(DepartmentDDL.SelectedValue);
            Response.Redirect(Request.RawUrl);
        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            Session["dhmode"] = -1;
            Response.Redirect(Request.RawUrl);
        }
    }
}