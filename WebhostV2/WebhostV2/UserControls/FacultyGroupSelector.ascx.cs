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
    public partial class FacultyGroupSelector : LoggingUserControl
    {
        public List<int> GroupIds
        {
            get
            {
                try
                {
                    List<int> ids = new List<int>();
                    String[] strs = FacultyListField.Value.Split(',');
                    foreach(String idstr in strs)
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
                using(WebhostEntities db = new WebhostEntities())
                {
                    bool first = true;
                    foreach (int id in value)
                    {
                        if (GroupIds.Contains(id)) continue;
                        if (db.Faculties.Where(f => f.ID == id).Count() <= 0) continue;

                        FacultyListField.Value += (first ? "" : ",") + id;
                        first = false;
                    }
                }
                LoadTable();
            }
        }

        public void Clear()
        {
            FacultyListField.Value = "";
            LoadTable();
        }

        public void AddFaculty(List<int> ids)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = FacultyListField.Value.Length <= 0;
                foreach (int id in ids)
                {
                    if (GroupIds.Contains(id)) continue;
                    if (db.Faculties.Where(f => f.ID == id).Count() <= 0) continue;

                    FacultyListField.Value += (first ? "" : ",") + id;
                    first = false;
                }

            }
            LoadTable();
        }

        public void AddFaculty(int id)
        {
            if (GroupIds.Contains(id))
            {
                LoadTable();
                return;
            }

            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = FacultyListField.Value.Length <= 0;   
                if (db.Faculties.Where(f => f.ID == id).Count() > 0)
                    FacultyListField.Value += (first ? "" : ",") + id;

            }
            LoadTable();
        }

        protected void LoadTable()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                SelectedTable.Rows.Clear();
                if(GroupIds.Count == 0)
                {
                    RemoveBtn.Visible = false;
                    RemoveList.Visible = false;
                    return;
                }
                foreach (int id in GroupIds)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == id).Single();

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    Label lbl = new Label()
                    {
                        Text = String.Format("{0} {1}", faculty.FirstName, faculty.LastName)
                    };

                    cell.Controls.Add(lbl);

                    row.Cells.Add(cell);
                    SelectedTable.Rows.Add(row);
                }

                RemoveList.Visible = true;
                RemoveBtn.Visible = true;

               /* RemoveList.DataSource = (from faculty in db.Faculties.Where(f => GroupIds.Contains(f.ID))
                                         orderby faculty.LastName, faculty.FirstName
                                         select new
                                         {
                                             Name = faculty.FirstName + " " + faculty.LastName,
                                             ID = faculty.ID
                                         }).ToList();*/
                RemoveList.DataSource = FacultyListItem.GetDataSource(GroupIds);
                RemoveList.DataTextField = "Text";
                RemoveList.DataValueField = "ID";
                RemoveList.DataBind();
            }
        }

        public bool ActiveOnly
        {
            get
            {
                return ActiveOnlyField.Value.Equals("yes");
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    List<int> ids;
                    if (value)
                    {
                        ids = db.Faculties.Where(f => f.isActive).OrderBy(f => f.LastName).ThenBy(f => f.FirstName).Select(f => f.ID).ToList();
                        ActiveOnlyField.Value = "yes";
                    }
                    else
                    {
                        ids = db.Faculties.OrderBy(f => f.LastName).ThenBy(f => f.FirstName).Select(f => f.ID).ToList();
                        ActiveOnlyField.Value = "no";
                    }
                    FacultySelector.DataSource = FacultyListItem.GetDataSource(ids);
                    FacultySelector.DataTextField = "Text";
                    FacultySelector.DataValueField = "ID";
                    FacultySelector.DataBind();
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<int> ids = db.Faculties.OrderBy(f => f.LastName).ThenBy(f => f.FirstName).Select(f => f.ID).ToList();
                FacultySelector.DataSource = FacultyListItem.GetDataSource(ids);
                FacultySelector.DataTextField = "Text";
                FacultySelector.DataValueField = "ID";
                FacultySelector.DataBind();
            }
        }

        protected void AddFaculty_Click(object sender, EventArgs e)
        {
            if (!FacultySelector.SelectedValue.Equals(String.Empty) && !GroupIds.Contains(Convert.ToInt32(FacultySelector.SelectedValue)))
                FacultyListField.Value += (FacultyListField.Value.Length > 0 ? "," : "") + FacultySelector.SelectedValue;
            LoadTable();
            FacultySelector.ClearSelection();
        }

        protected void RemoveBtn_Click(object sender, EventArgs e)
        {
            String remExpStr = String.Format("\\b{0}\\b,?|,{0}\\b", RemoveList.SelectedValue);
            Regex remExp = new Regex(remExpStr);
            foreach(Match match in remExp.Matches(FacultyListField.Value))
            {
                FacultyListField.Value = FacultyListField.Value.Replace(match.Value, "");
            }

            LoadTable();
        }

        protected void UpdatePanel1_Load(object sender, EventArgs e)
        {
        }

    }
}