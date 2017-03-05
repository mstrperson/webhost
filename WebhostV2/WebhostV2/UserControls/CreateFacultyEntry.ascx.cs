using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection.Web;
using System.Text.RegularExpressions;

namespace WebhostV2.UserControls
{
    public partial class CreateFacultyEntry : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateFacultyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                List<String> otherGroups = new List<String>();
                Regex groupNameExp = new Regex(@"[a-zA-Z][a-zA-Z \-_]+");
                foreach(Match match in groupNameExp.Matches(OtherGroupsInput.Text))
                {
                    otherGroups.Add(match.Value);
                }
                Import.NewFaculty(Convert.ToInt32(EmployeeIdInput.Text), FirstNameInput.Text, LastNameInput.Text, primaryGroupDDL.SelectedValue, otherGroups);
                FirstNameInput.Text = "";
                LastNameInput.Text = "";
                EmployeeIdInput.Text = "";
            }
            catch(ImportException ie)
            {
                ((BasePage)Page).log.WriteLine(ie.Message);
                FirstNameInput.Text = "error...";
            }
            catch(FormatException)
            {
                EmployeeIdInput.Text = "Invalid!";
                EmployeeIdInput.Focus();
            }
        }
    }
}