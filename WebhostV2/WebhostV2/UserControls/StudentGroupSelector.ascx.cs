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
    public partial class StudentGroupSelector : LoggingUserControl
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

        public bool ActiveOnly
        {
            get
            {
                return ActiveOnlyField.Value.Equals("yes");
            }
            set
            {
                ActiveOnlyField.Value = value ? "yes" : "no";
            }
        }

        public List<int> GroupIds
        {
            get
            {
                try
                {
                    List<int> ids = new List<int>();
                    String[] strs = StudentListField.Value.Split(',');
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
                        if (db.Students.Where(f => f.ID == id).Count() <= 0) continue;

                        StudentListField.Value += (first ? "" : ",") + id;
                        first = false;
                    }
                }
                LoadTable();
            }
        }

        public void AddStudent(List<int> ids)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = StudentListField.Value.Length <= 0;
                foreach (int id in ids)
                {
                    if (GroupIds.Contains(id)) continue;
                    if (db.Students.Where(f => f.ID == id).Count() <= 0) continue;

                    StudentListField.Value += (first ? "" : ",") + id;
                    first = false;
                }

            }
            LoadTable();
        }

        public void Clear()
        {
            StudentListField.Value = "";
            LoadTable();
        }

        public void AddStudent(int id)
        {
            if (GroupIds.Contains(id))
            {
                LoadTable();
                return;
            }

            using (WebhostEntities db = new WebhostEntities())
            {
                bool first = StudentListField.Value.Length <= 0;
                if (db.Students.Where(f => f.ID == id).Count() > 0)
                    StudentListField.Value += (first ? "" : ",") + id;

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
                    Student Student = db.Students.Where(f => f.ID == id).Single();

                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    Label lbl = new Label()
                    {
                        Text = String.Format("{0} {1} ({2})", Student.FirstName, Student.LastName, Student.GraduationYear)
                    };

                    cell.Controls.Add(lbl);

                    row.Cells.Add(cell);
                    SelectedTable.Rows.Add(row);
                }

                RemoveList.Visible = true;
                RemoveBtn.Visible = true;

                RemoveList.DataSource = StudentListItem.GetDataSource(GroupIds);
                RemoveList.DataTextField = "Text";
                RemoveList.DataValueField = "ID";
                RemoveList.DataBind();
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                StudentSelector.DataSource = StudentListItem.GetDataSource(db.Students.Where(s => s.isActive).Select(s => s.ID).ToList());
                StudentSelector.DataTextField = "Text";
                StudentSelector.DataValueField = "ID";
                StudentSelector.DataBind();

                RemoveList.DataSource = StudentListItem.GetDataSource(GroupIds);
                RemoveList.DataTextField = "Text";
                RemoveList.DataValueField = "ID";
                RemoveList.DataBind();
            }
        }

        protected void AddStudent_Click(object sender, EventArgs e)
        {
            if (!StudentSelector.SelectedValue.Equals(String.Empty) && !GroupIds.Contains(Convert.ToInt32(StudentSelector.SelectedValue)))
                StudentListField.Value += (StudentListField.Value.Length > 0 ? "," : "") + StudentSelector.SelectedValue;
            LoadTable();
            StudentSelector.ClearSelection();
        }

        protected void RemoveBtn_Click(object sender, EventArgs e)
        {
            String remExpStr = String.Format("\\b{0}\\b,?|,{0}\\b", RemoveList.SelectedValue);
            Regex remExp = new Regex(remExpStr);
            foreach (Match match in remExp.Matches(StudentListField.Value))
            {
                StudentListField.Value = StudentListField.Value.Replace(match.Value, "");
            }

            LoadTable();
        }

        protected void UpdatePanel1_Load(object sender, EventArgs e)
        {
        }
    }
}