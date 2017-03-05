using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class CreateSection : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                int year = DateRange.GetCurrentAcademicYear();
                using(WebhostEntities db = new WebhostEntities())
                {
                    TermCBL.DataSource = db.Terms.Where(t => t.AcademicYearID == year).ToList();
                    TermCBL.DataTextField = "Name";
                    TermCBL.DataValueField = "id";
                    TermCBL.DataBind();

                    CourseSelectComboBox.DataSource = db.Courses.Where(c => c.AcademicYearID == year).ToList();
                    CourseSelectComboBox.DataTextField = "Name";
                    CourseSelectComboBox.DataValueField = "id";
                    CourseSelectComboBox.DataBind();

                    BlockSelectDDL.DataSource = db.Blocks.Where(b => b.AcademicYearID == year).ToList();
                    BlockSelectDDL.DataTextField = "Name";
                    BlockSelectDDL.DataValueField = "id";
                    BlockSelectDDL.DataBind();

                    DepartmentDDL.DataSource = db.Departments.ToList();
                    DepartmentDDL.DataTextField = "Name";
                    DepartmentDDL.DataValueField = "id";
                    DepartmentDDL.DataBind();
                }
            }
        }

        protected void ExistingCourseCB_CheckedChanged(object sender, EventArgs e)
        {
            CourseSelectComboBox.Visible = ExistingCourseCB.Checked;
            NewCoursePanel.Visible = !ExistingCourseCB.Checked;
        }

        protected void CreateSectionBtn_Click(object sender, EventArgs e)
        {
            int year = DateRange.GetCurrentAcademicYear();

            using(WebhostEntities db = new WebhostEntities())
            {
                int sectionId = db.Sections.Count() > 0 ? db.Sections.OrderBy(sec => sec.id).ToList().Last().id + 1 : 0;
                int courseId = db.Courses.Count() > 0 ? db.Courses.OrderBy(c => c.id).ToList().Last().id + 1 : 0;
                List<int> selectedTerms = new List<int>();
                foreach(ListItem item in TermCBL.Items)
                {
                    if(item.Selected)
                        selectedTerms.Add(Convert.ToInt32(item.Value));
                }
                if(ExistingCourseCB.Checked)
                {
                    int cid = Convert.ToInt32(CourseSelectComboBox.SelectedValue);
                    courseId = db.Courses.Where(c => c.id == cid).Single().id;
                }
                else // create course!
                {
                    if (CourseNameInput.Text.Equals(String.Empty)) throw new WebhostMySQLConnection.Web.WebhostException("Course Name cannot be empty.");
                    String bbcid = Convert.ToString(courseId);
                    char ch = 'A';
                    while(db.Courses.Where(c => c.AcademicYearID == year && c.BlackBaudID.Equals(bbcid)).Count() > 0)
                    {
                        bbcid = String.Format("{0}-{1}", courseId, ch);
                        ch++;
                    }

                    Course course = new Course()
                    {
                        id = courseId,
                        Name = CourseNameInput.Text,
                        BlackBaudID = bbcid,
                        AcademicYearID = year,
                        DepartmentID = Convert.ToInt32(DepartmentDDL.SelectedValue),
                        goesToSchoology = SendToSchoologyCB.Checked,
                        SchoologyId = -1,
                        LengthInTerms = selectedTerms.Count
                    };

                    db.Courses.Add(course);
                }

                int secNumber = db.Sections.Where(sec => sec.CourseIndex == courseId).Count() > 0?db.Sections.Where(sec => sec.CourseIndex == courseId).OrderBy(sec => sec.SectionNumber).ToList().Last().SectionNumber + 1:0;
                Section section = new Section()
                {
                    id = sectionId,
                    SectionNumber = secNumber,
                    CourseIndex = courseId,
                    BlockIndex = Convert.ToInt32(BlockSelectDDL.SelectedValue),
                    getsComments = GetsCommentsCB.Checked,
                    SchoologyId = -1
                };

                foreach(int tid in FacultyGroupSelector1.GroupIds)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == tid).Single();
                    section.Teachers.Add(faculty);
                }

                foreach(int termid in selectedTerms)
                {
                    Term term = db.Terms.Where(t => t.id == termid).Single();
                    section.Terms.Add(term);
                }

                db.Sections.Add(section);
                db.SaveChanges();

                Response.Redirect("~/Admin.aspx");
            }
        }
    }
}