using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2
{
    public partial class CommentEditor : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Faculty fac = db.Faculties.Where(f => f.ID == user.ID).Single();
                    DepartmentalComments1.Visible = fac.Departments.Count > 0;
                }
            }
        }

        protected void SaveSigBtn_Click(object sender, EventArgs e)
        {
            if (SignatureUpload.HasFile)
            {
                Signature signature = new Signature(CommentEditor1.FacultyId);
                signature.SaveNewSignature(SignatureUpload.FileBytes);
            }
        }
    }
}