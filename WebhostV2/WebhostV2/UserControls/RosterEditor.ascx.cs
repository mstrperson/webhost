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
    public partial class RosterEditor : LoggingUserControl
    {
        public int SectionID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(SectionIDField.Value);
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
                    if(db.Sections.Where(sec => sec.id == value).Count() > 0)
                    {
                        SectionIDField.Value = Convert.ToString(value);
                        Section section = db.Sections.Where(sec => sec.id == value).Single();

                        State.log.WriteLine("Loading [{0}] {1} for Editing.", section.Block.LongName, section.Course.Name);

                        BlockSelectList.DataSource = section.Course.AcademicYear.Blocks.ToList();
                        BlockSelectList.DataTextField = "Name";
                        BlockSelectList.DataValueField = "id";
                        BlockSelectList.DataBind();

                        BlockSelectList.ClearSelection();
                        BlockSelectList.SelectedValue = Convert.ToString(section.BlockIndex);


                        TermSelectList.DataSource = section.Course.AcademicYear.Terms.ToList();
                        TermSelectList.DataTextField = "Name";
                        TermSelectList.DataValueField = "id";
                        TermSelectList.DataBind();

                        SectionLabel.Text = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name);

                        TeacherSelect.DataSource = (from teacher in db.Faculties
                                                    orderby teacher.LastName, teacher.FirstName
                                                    select new
                                                    {
                                                        Name = teacher.FirstName + " " + teacher.LastName,
                                                        ID = teacher.ID
                                                    }).ToList();
                        TeacherSelect.DataTextField = "Name";
                        TeacherSelect.DataValueField = "ID";
                        TeacherSelect.DataBind();
                        
                        RemoveTeacherDDL.DataSource = (from teacher in section.Teachers
                                                       orderby teacher.LastName, teacher.FirstName
                                                       select new
                                                       {
                                                           Name = teacher.FirstName + " " + teacher.LastName,
                                                           ID = teacher.ID
                                                       }).ToList();
                        RemoveTeacherDDL.DataTextField = "Name";
                        RemoveTeacherDDL.DataValueField = "ID";
                        RemoveTeacherDDL.DataBind();

                        StudentGroupSelector1.Clear();
                        StudentGroupSelector1.AddStudent(section.Students.OrderBy(st => st.LastName).ThenBy(st => st.FirstName).Select(s => s.ID).ToList());
                        foreach (Student student in section.Students.OrderBy(st => st.LastName).ThenBy(st => st.FirstName))
                        {
                            TableRow row = new TableRow();
                            TableCell cell = new TableCell();
                            cell.Text = String.Format("{0} {1}", student.FirstName, student.LastName);
                            row.Cells.Add(cell);
                            StudentTable.Rows.Add(row);
                        }

                        foreach(Term term in section.Terms)
                        {
                            foreach(ListItem item in TermSelectList.Items)
                            {
                                if (item.Text.Equals(term.Name))
                                    item.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        protected void RemoveClick(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int id = Convert.ToInt32(RemoveTeacherDDL.SelectedValue);
                Section section = db.Sections.Where(sec => sec.id == SectionID).Single();
                Faculty teacher = db.Faculties.Where(f => f.ID == id).Single();
                State.log.WriteLine("Removing {0} {1} from [{2}] {3}", teacher.FirstName, teacher.LastName, section.Block.LongName, section.Course.Name);
                section.Teachers.Remove(teacher);
                db.SaveChanges();
                State.log.WriteLine("Saved Changes!");
            }
        }

        private void ReloadTables()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == SectionID).Single();


                RemoveTeacherDDL.DataSource = (from teacher in section.Teachers
                                               orderby teacher.LastName, teacher.FirstName
                                               select new
                                               {
                                                   Name = teacher.FirstName + " " + teacher.LastName,
                                                   ID = teacher.ID
                                               }).ToList();
                RemoveTeacherDDL.DataTextField = "Name";
                RemoveTeacherDDL.DataValueField = "ID";
                RemoveTeacherDDL.DataBind();

                StudentTable.Rows.Clear();

                StudentGroupSelector1.AddStudent(section.Students.OrderBy(st => st.LastName).ThenBy(st => st.FirstName).Select(s => s.ID).ToList());
                foreach (Student student in section.Students.OrderBy(st => st.LastName).ThenBy(st => st.FirstName))
                {
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    cell.Text = String.Format("{0} {1}", student.FirstName, student.LastName);
                    row.Cells.Add(cell);
                    StudentTable.Rows.Add(row);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SetTermBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == SectionID).Single();
                State.log.WriteLine("Attempting to set Terms for [{0}] {1}", section.Block.LongName, section.Course.Name);
                section.Terms.Clear();
                foreach (ListItem item in TermSelectList.Items)
                {
                    if(item.Selected)
                    {
                        int id = Convert.ToInt32(item.Value);
                        section.Terms.Add(db.Terms.Where(t => t.id == id).Single());
                        State.log.WriteLine("Added {0} Term", item.Text);
                    }
                }

                db.SaveChanges();
                State.log.WriteLine("Saved Terms to Section in Database.");
            }
        }

        protected void AddStudentsBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == SectionID).Single();
                State.log.WriteLine("Updating Student roster for [{0}] {1}", section.Block.LongName, section.Course.Name);
                section.Students.Clear();
                foreach (int id in StudentGroupSelector1.GroupIds)
                {
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    State.log.WriteLine("Added {0} {1}", student.FirstName, student.LastName);
                    section.Students.Add(student);
                }
                

                db.SaveChanges();
                State.log.WriteLine("Saved Updated Roster to Database.");
            }
        }

        protected void AddTeacherBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == SectionID).Single();
                int id = Convert.ToInt32(TeacherSelect.SelectedValue);
                Faculty teacher = db.Faculties.Where(f => f.ID == id).Single();
                if (!section.Teachers.Contains(teacher))
                {
                    State.log.WriteLine("Adding {0} {1} to [{2}] {3}", teacher.FirstName, teacher.LastName, section.Block.LongName, section.Course.Name);
                    section.Teachers.Add(teacher);
                    db.SaveChanges();
                    State.log.WriteLine("Saved!");
                }
                else
                {
                    State.log.WriteLine("{0} {1} is already a teacher for [{2}] {3}", teacher.FirstName, teacher.LastName, section.Block.LongName, section.Course.Name);
                }
            }
        }

        protected void SetBlock_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == SectionID).Single();
                int id = Convert.ToInt32(BlockSelectList.SelectedValue);
                section.BlockIndex = id;
                db.SaveChanges();
            }
        }
    }
}