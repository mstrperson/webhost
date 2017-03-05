using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebhostV2
{
    public partial class CommentAdministration : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void CommentAdmin1_Masquerade(object sender, EventArgs e)
        {
            CommentEditor1.FacultyId = CommentAdmin1.MasqeradeId;
        }
    }
}