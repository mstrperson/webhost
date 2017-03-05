using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using System.Text.RegularExpressions;

namespace WebhostV2.UserControls
{
    public partial class TagSelector : LoggingUserControl
    {
        public String Title
        {
            get
            {
                return TitleLabel.Text;
            }
            set
            {
                TitleLabel.Text = value;
            }
        }

        public List<int> GroupIds
        {
            get
            {
                try
                {
                    List<int> ids = new List<int>();
                    String[] strs = TagListField.Value.Split(',');
                    foreach (String idstr in strs)
                    {
                        ids.Add(Convert.ToInt32(idstr));
                    }

                    return ids;
                }
                catch
                {
                    return new List<int>();
                }
            }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    bool first = true;
                    foreach (int id in value)
                    {
                        if (GroupIds.Contains(id)) continue;
                        if (db.WebPageTags.Where(f => f.id == id).Count() <= 0) continue;

                        TagListField.Value += (first ? "" : ",") + id;
                        first = false;
                    }
                }
                LoadTable();
            }
        }

        public void AddWebPageTags(List<int> ids)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = TagListField.Value.Length <= 0;
                foreach (int id in ids)
                {
                    if (GroupIds.Contains(id)) continue;
                    if (db.WebPageTags.Where(f => f.id == id).Count() <= 0) continue;

                    TagListField.Value += (first ? "" : ",") + id;
                    first = false;
                }

            }
            LoadTable();
        }

        public void Clear()
        {
            TagListField.Value = "";
            LoadTable();
        }

        public void AddTag(int id)
        {
            if (GroupIds.Contains(id))
            {
                LoadTable();
                return;
            }

            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = TagListField.Value.Length <= 0;
                if (db.WebPageTags.Where(f => f.id == id).Count() > 0)
                    TagListField.Value += (first ? "" : ",") + id;

            }
            LoadTable();
        }

        protected void LoadTable()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                SelectedTable.Rows.Clear();
                if (GroupIds.Count == 0)
                {
                    RemoveBtn.Visible = false;
                    RemoveList.Visible = false;
                    return;
                }
                foreach (int id in GroupIds)
                {
                    WebPageTag tag = db.WebPageTags.Where(f => f.id == id).Single();

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    Label lbl = new Label()
                    {
                        Text = tag.Name
                    };

                    cell.Controls.Add(lbl);

                    row.Cells.Add(cell);
                    SelectedTable.Rows.Add(row);
                }

                RemoveList.Visible = true;
                RemoveBtn.Visible = true;

                RemoveList.DataSource = db.WebPageTags.Where(p => GroupIds.Contains(p.id)).ToList();
                RemoveList.DataTextField = "Name";
                RemoveList.DataValueField = "id";
                RemoveList.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    TagSelectionCB.DataSource = db.WebPageTags.ToList();
                    TagSelectionCB.DataTextField = "Name";
                    TagSelectionCB.DataValueField = "id";
                    TagSelectionCB.DataBind();

                    RemoveList.DataSource = db.WebPageTags.Where(p => GroupIds.Contains(p.id)).ToList();
                    RemoveList.DataTextField = "Name";
                    RemoveList.DataValueField = "id";
                    RemoveList.DataBind();
                }
            }

            LoadTable();
        }

        protected void AddStudent_Click(object sender, EventArgs e)
        {
            if (!TagSelectionCB.SelectedValue.Equals(String.Empty) && !GroupIds.Contains(Convert.ToInt32(TagSelectionCB.SelectedValue)))
                TagListField.Value += (TagListField.Value.Length > 0 ? "," : "") + TagSelectionCB.SelectedValue;
            LoadTable();
            TagSelectionCB.ClearSelection();
        }

        protected void RemoveBtn_Click(object sender, EventArgs e)
        {
            String remExpStr = String.Format("\\b{0}\\b,?|,{0}\\b", RemoveList.SelectedValue);
            Regex remExp = new Regex(remExpStr);
            foreach (Match match in remExp.Matches(TagListField.Value))
            {
                TagListField.Value = TagListField.Value.Replace(match.Value, "");
            }

            LoadTable();
        }

        protected void UpdatePanel1_Load(object sender, EventArgs e)
        {
        }

        protected void CreateTagBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if(db.WebPageTags.Where(t => t.Name.Equals(NewTagNameInput.Text)).Count() > 0)
                {
                    WebPageTag tag = db.WebPageTags.Where(t => t.Name.Equals(NewTagNameInput.Text)).Single();
                    AddTag(tag.id);
                    return;
                }

                WebPageTag newTag = new WebPageTag()
                {
                    id = db.WebPageTags.Count() > 0 ? db.WebPageTags.OrderBy(t => t.id).ToList().Last().id + 1 : 0,
                    Name = NewTagNameInput.Text
                };

                db.WebPageTags.Add(newTag);
                db.SaveChanges();
                AddTag(newTag.id);
            }
        }
    }
}