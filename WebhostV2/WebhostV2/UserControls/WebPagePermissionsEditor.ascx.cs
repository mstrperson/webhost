using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class WebPagePermissionsEditor : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                PagePermissionSelector.AddPermissions(((BasePage)Page).RequiredPermissions.Select(p => p.id).ToList());
                using (WebhostEntities db = new WebhostEntities())
                {
                    try
                    {
                        WebPage wpg = db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Single();
                        TitleInput.Text = wpg.Name;
                        TagSelector1.AddWebPageTags(wpg.Tags.Select(t => t.id).ToList());
                    }
                    catch
                    {
                        // do nothing!
                    }
                }
            }
        }

        protected void SetPermissionsBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if(db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Count() <=0)
                {
                    WebPage newPage = new WebPage()
                    {
                        id = db.WebPages.Count() > 0 ? db.WebPages.OrderBy(p => p.id).ToList().Last().id + 1 : 0,
                        RawURL = Request.RawUrl,
                        Name = TitleInput.Text
                    };
                    db.WebPages.Add(newPage);
                    db.SaveChanges();
                }

                if(PagePermissionSelector.GroupIds.Count > 0)
                {
                    int AdminId = PermissionControl.GetPermissionByName("Administrator").id;
                    if (!PagePermissionSelector.GroupIds.Contains(AdminId))
                        PagePermissionSelector.AddPermission(AdminId);
                }

                WebPage page = db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Single();
                page.Name = TitleInput.Text;
                page.Permissions.Clear();

                foreach(int id in PagePermissionSelector.GroupIds)
                {
                    Permission permission = db.Permissions.Where(p => p.id == id).Single();
                    page.Permissions.Add(permission);
                }

                db.SaveChanges();
            }
        }

        protected void SaveTagsBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Count() <= 0)
                {
                    WebPage newPage = new WebPage()
                    {
                        id = db.WebPages.Count() > 0 ? db.WebPages.OrderBy(p => p.id).ToList().Last().id + 1 : 0,
                        RawURL = Request.RawUrl,
                        Name = TitleInput.Text
                    };
                    db.WebPages.Add(newPage);
                    db.SaveChanges();
                }

                WebPage page = db.WebPages.Where(p => p.RawURL.Equals(Request.RawUrl)).Single();

                page.Tags.Clear();

                foreach (int id in TagSelector1.GroupIds)
                {
                    WebPageTag tag = db.WebPageTags.Where(p => p.id == id).Single();
                    page.Tags.Add(tag);
                }

                db.SaveChanges();
            }
        }
    }
}