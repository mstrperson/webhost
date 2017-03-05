using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.GoogleAPI;

namespace WebhostV2
{
    public partial class Import1 : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UpdateFacultyCommonsGroupsBtn_Click(object sender, EventArgs e)
        {
            using (GoogleDirectoryCall call = new GoogleDirectoryCall())
            {
                try
                {
                    call.UpdateCommonsAndFacultyGroups();
                }
                catch(GoogleAPICall.GoogleAPIException ex)
                {
                    log.WriteLine(ex.Message);
                }
            }
        }

        protected void GoogleCalendarUpdateBtn_Click(object sender, EventArgs e)
        {
            using (GoogleCalendarCall call = new GoogleCalendarCall())
            {
                call.UpdateCalendars();
            }
        }
    }
}