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
    public partial class ScheduleView : LoggingUserControl
    {
        public int UserId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(UserIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }

            set
            {
                UserIdField.Value = Convert.ToString(value);
            }
        }

        public bool IsStudent
        {
            get
            {
                return IsStudentField.Value.Equals("Yes");
            }
            set
            {
                IsStudentField.Value = value ? "Yes" : "No";
            }
        }

        public void PopulateTable()
        {
            List<int> sections = new List<int>(); 
            int year = DateRange.GetCurrentAcademicYear();
            using(WebhostEntities db = new WebhostEntities())
            {
                try
                {
                    if (IsStudent)
                    {
                        Student student = db.Students.Where(s => s.ID == UserId).Single();
                        NameLabel.Text = String.Format("{0} {1} ({2})", student.FirstName, student.LastName, student.GraduationYear);
                        sections = student.Sections.Where(sec => sec.Course.AcademicYearID == year).OrderBy(sec => sec.BlockIndex).Select(sec => sec.id).ToList();
                    }
                    else
                    {
                        Faculty faculty = db.Faculties.Where(f => f.ID == UserId).Single();
                        NameLabel.Text = String.Format("{0} {1}", faculty.FirstName, faculty.LastName);
                        sections = faculty.Sections.Where(sec => sec.Course.AcademicYearID == year).OrderBy(sec => sec.BlockIndex).Select(sec => sec.id).ToList();
                    }
                }
                catch(Exception e)
                {
                    State.log.WriteLine("Failed to load ScheduleTable.{0}{1}", Environment.NewLine, e.Message);
                    AddClassBtn.Enabled = false;
                    DropClassBtn.Enabled = false;
                    return;
                }
            }

            AddClassBtn.Enabled = true;
            DropClassBtn.Enabled = true;

            ScheduleTable.Rows.Clear();
            ScheduleTable.Rows.Add(new ScheduleTableRow());
            using(WebhostEntities db = new WebhostEntities())
                foreach(int id in sections)
                {
                    Section section = db.Sections.Where(sec => sec.id == id).Single();
                    if (!section.Block.ShowInSchedule) continue;
                    ScheduleTable.Rows.Add(new ScheduleTableRow(id));
                }

            DropClassDDL.DataSource = SectionListItem.GetDataSource(sections);
            DropClassDDL.DataTextField = "Text";
            DropClassDDL.DataValueField = "ID";
            DropClassDDL.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                List<int> allSections = new List<int>();
                int year = DateRange.GetCurrentAcademicYear();
                using(WebhostEntities db =new WebhostEntities())
                {
                    allSections = db.Sections.Where(sec => sec.Course.AcademicYearID == year).OrderBy(sec => sec.Course.Name).Select(sec => sec.id).ToList();
                }

                AddClassDDL.DataSource = SectionListItem.GetDataSource(allSections);
                AddClassDDL.DataTextField = "Text";
                AddClassDDL.DataValueField = "ID";
                AddClassDDL.DataBind();
            }
        }

        protected void AddClassBtn_Click(object sender, EventArgs e)
        {
            int secid = Convert.ToInt32(AddClassDDL.SelectedValue);
            using (WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == secid).Single();
                if (IsStudent)
                {
                    Student student = db.Students.Where(s => s.ID == UserId).Single();
                    if(!student.Sections.Contains(section)) student.Sections.Add(section);
                }
                else
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == UserId).Single();
                    if (!faculty.Sections.Contains(section)) faculty.Sections.Add(section);
                }

                db.SaveChanges();
            }
            PopulateTable();
        }

        protected void DropClassBtn_Click(object sender, EventArgs e)
        {
            int secid = Convert.ToInt32(DropClassDDL.SelectedValue);
            using (WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == secid).Single();
                if (IsStudent)
                {
                    Student student = db.Students.Where(s => s.ID == UserId).Single();
                    if (student.Sections.Contains(section)) student.Sections.Remove(section);
                }
                else
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == UserId).Single();
                    if (faculty.Sections.Contains(section)) faculty.Sections.Remove(section);
                }

                db.SaveChanges();
            }
            PopulateTable();
        }
    }
}