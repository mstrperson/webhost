using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class PermissionSelector : LoggingUserControl
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
                    String[] strs = PermissionListField.Value.Split(',');
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
                        if (db.Permissions.Where(f => f.id == id).Count() <= 0) continue;

                        PermissionListField.Value += (first ? "" : ",") + id;
                        first = false;
                    }
                }
                LoadTable();
            }
        }

        public void AddPermissions(List<int> ids)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = PermissionListField.Value.Length <= 0;
                foreach (int id in ids)
                {
                    if (GroupIds.Contains(id)) continue;
                    if (db.Permissions.Where(f => f.id == id).Count() <= 0) continue;

                    PermissionListField.Value += (first ? "" : ",") + id;
                    first = false;
                }

            }
            LoadTable();
        }

        public void Clear()
        {
            PermissionListField.Value = "";
            LoadTable();
        }

        public void AddPermission(int id)
        {
            if (GroupIds.Contains(id))
            {
                LoadTable();
                return;
            }

            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = PermissionListField.Value.Length <= 0;
                if (db.Permissions.Where(f => f.id == id).Count() > 0)
                    PermissionListField.Value += (first ? "" : ",") + id;

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
                    Permission permission = db.Permissions.Where(f => f.id == id).Single();

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    Label lbl = new Label()
                    {
                        Text = permission.Name
                    };

                    cell.Controls.Add(lbl);

                    row.Cells.Add(cell);
                    SelectedTable.Rows.Add(row);
                }

                RemoveList.Visible = true;
                RemoveBtn.Visible = true;

                RemoveList.DataSource = db.Permissions.Where(p => GroupIds.Contains(p.id)).ToList();
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
                    PermissionSelectionCB.DataSource = db.Permissions.ToList();
                    PermissionSelectionCB.DataTextField = "Name";
                    PermissionSelectionCB.DataValueField = "id";
                    PermissionSelectionCB.DataBind();

                    RemoveList.DataSource = db.Permissions.Where(p=> GroupIds.Contains(p.id)).ToList();
                    RemoveList.DataTextField = "Name";
                    RemoveList.DataValueField = "id";
                    RemoveList.DataBind();
                }
            }

            LoadTable();
        }

        protected void AddStudent_Click(object sender, EventArgs e)
        {
            if (!PermissionSelectionCB.SelectedValue.Equals(String.Empty) && !GroupIds.Contains(Convert.ToInt32(PermissionSelectionCB.SelectedValue)))
                PermissionListField.Value += (PermissionListField.Value.Length > 0 ? "," : "") + PermissionSelectionCB.SelectedValue;
            LoadTable();
            PermissionSelectionCB.ClearSelection();
        }

        protected void RemoveBtn_Click(object sender, EventArgs e)
        {
            String remExpStr = String.Format("\\b{0}\\b,?|,{0}\\b", RemoveList.SelectedValue);
            Regex remExp = new Regex(remExpStr);
            foreach (Match match in remExp.Matches(PermissionListField.Value))
            {
                PermissionListField.Value = PermissionListField.Value.Replace(match.Value, "");
            }

            LoadTable();
        }

        protected void UpdatePanel1_Load(object sender, EventArgs e)
        {
        }
    }
}