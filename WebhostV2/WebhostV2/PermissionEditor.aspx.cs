using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2
{
    public partial class PermissionEditor : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    int yr = DateRange.GetCurrentAcademicYear();
                    PermissionSelectionCB.DataSource = db.Permissions.Where(p => p.AcademicYear == yr).ToList();
                    PermissionSelectionCB.DataTextField = "Name";
                    PermissionSelectionCB.DataValueField = "id";
                    PermissionSelectionCB.DataBind();
                }
            }
        }

        protected void SelectPermissionBtn_Click(object sender, EventArgs e)
        {
            try
            {
                PermissionEditor1.PermissionId = Convert.ToInt32(PermissionSelectionCB.SelectedValue);
            }
            catch
            {
                PermissionSelectionCB.ClearSelection();
            }
        }
    }
}