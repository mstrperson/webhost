using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebhostV2.UserControls
{
    public partial class TextEditor : LoggingUserControl
    {
        public String Html
        {
            get
            {
                return TheEditor.Content;
            }
            set
            {
                TheEditor.Content = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}