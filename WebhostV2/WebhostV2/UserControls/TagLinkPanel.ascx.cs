using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class TagLinkPanel : LoggingUserControl
    {
        public int TagId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(TagIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.WebPageTags.Where(t => t.id == value).Count() > 0)
                    {
                        TagIdField.Value = Convert.ToString(value);
                        WebPageTag tag = db.WebPageTags.Where(t => t.id == value).Single();
                        TitleLabel.Text = tag.Name;

                        bool hasLinks = false;

                        List<int> permIds = ((BasePage)Page).user.Permissions;
                        List<Permission> permissions = db.Permissions.Where(p => permIds.Contains(p.id)).ToList();
                        foreach(WebPage page in tag.WebPages.ToList())
                        {
                            if(page.Permissions.Count <= 0)
                            {
                                HyperLink link = new HyperLink()
                                {
                                    NavigateUrl = page.RawURL,
                                    ID = page.Name.Replace(" ", "") + "Link",
                                    Text = page.Name
                                };

                                LinkPanel.Controls.Add(link);
                                hasLinks = true;
                                continue;
                            }

                            foreach(Permission permission in permissions)
                            {
                                if(page.Permissions.Contains(permission))
                                {
                                    HyperLink link = new HyperLink()
                                    {
                                        NavigateUrl = page.RawURL,
                                        ID = page.Name.Replace(" ", "") + "Link",
                                        Text = page.Name
                                    };

                                    LinkPanel.Controls.Add(link);
                                    hasLinks = true;
                                    break;
                                }
                            }
                        }

                        this.Visible = hasLinks;
                    }
                    else
                    {
                        this.Visible = false;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}