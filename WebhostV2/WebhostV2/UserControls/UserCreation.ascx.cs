using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class UserCreation : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    StudentSelectCBL.DataSource = (from student in db.Students
                                                   orderby student.LastName, student.FirstName, student.UserName
                                                   select new
                                                   {
                                                       Text = student.FirstName + " " + student.LastName + " (" + student.UserName + ")",
                                                       ID = student.ID
                                                   }).ToList();
                    StudentSelectCBL.DataTextField = "Text";
                    StudentSelectCBL.DataValueField = "ID";
                    StudentSelectCBL.DataBind();
                }
            }
        }

        protected List<int> SelectedIds
        {
            get
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    List<int> ids = new List<int>();
                    foreach(ListItem item in StudentSelectCBL.Items)
                    {
                        if(item.Selected)
                        {
                            ids.Add(Convert.ToInt32(item.Value));
                        }
                    }

                    return ids;
                }
            }
        }

        protected void GetAccountsBtn_Click(object sender, EventArgs e)
        {
            List<String> files = new List<string>();
            List<CSV> csvs = Import.GetNewAccountsCSVs(SelectedIds);
            csvs[0].Save(Server.MapPath("~/Temp/gmail_users.csv"));
            csvs[1].Save(Server.MapPath("~/Temp/ad_users.csv"));
            Response.Redirect(MailControler.PackForDownloading(new List<string>() { "~/Temp/gmail_users.csv", "~/Temp/ad_users.csv" }, "new users", Server));
        }
    }
}