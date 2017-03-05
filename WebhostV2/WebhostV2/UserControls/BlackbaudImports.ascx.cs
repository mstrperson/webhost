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
    public partial class BlackbaudImports : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                int year = DateRange.GetCurrentAcademicYear();
                using (WebhostEntities db = new WebhostEntities())
                {
                    TermSelect.DataSource = (from term in db.Terms
                                             where term.AcademicYearID == year
                                             orderby term.StartDate descending
                                             select new
                                             {
                                                 Text = term.Name + " " + term.StartDate.Year,
                                                 ID = term.id
                                             }).ToList();
                    TermSelect.DataTextField = "Text";
                    TermSelect.DataValueField = "ID";
                    TermSelect.DataBind();
                }
            }
        }

        protected void ImportBtn_Click(object sender, EventArgs e)
        {
            LogInformation("Processing Imports.  Selected Term is: {0}", TermSelect.SelectedItem == null ? "[None Term Selected]" : TermSelect.SelectedItem.Text);

            if(AcademicYearCB.Checked)
            {
                LogInformation("Initializing new Academic Year");
                Import.CreateAcademicYear(new CSV(AcademicYearUpload.FileContent));
            }
            if(FacultyCB.Checked)
            {
                LogInformation("Importing Faculty Data.");
                Import.Faculty(new CSV(FacultyUpload.FileContent));
            }
            if(StudentsCB.Checked)
            {
                LogInformation("Importing Student Data.");
                Import.Students(new CSV(StudentsUpload.FileContent));
            }
            if(CoursesCB.Checked)
            {
                LogInformation("Importing Course Data.");
                Import.Courses(new CSV(CoursesUpload.FileContent));
            }
            if(CourseRequestsCB.Checked)
            {
                LogInformation("Importing Courses for Course Requests.");
                Import.RequestableCourses(new CSV(CoursesUpload.FileContent));
            }
            if(SectionsCB.Checked)
            {
                LogInformation("Importing Section information.");
                Import.Sections(new CSV(SectionsUpload.FileContent));
            }
            if(EnrollmentCB.Checked)
            {
                LogInformation("Clearing Rosters for the current term.");
                Import.ClearRosters(Convert.ToInt32(TermSelect.SelectedValue));
                LogInformation("Importing Student Rosters.");
                Import.StudentSchedules(new CSV(StudentRosterUpload.FileContent));
                LogInformation("Importing Teacher Rosters");
                Import.TeacherSchedules(new CSV(FacultyRosterUpload.FileContent));
            }
            if(DormCB.Checked)
            {
                LogInformation("Importing Dorm information.");
                Import.DormRosters(new CSV(DormUpload.FileContent));
            }
            if(DutyRosterCB.Checked)
            {
                LogInformation("Importing Duty Team Rosters");
                Import.DutyTeamRosters(new CSV(DutyRosterUpload.FileContent));
            }
        }

        protected void ForcePasswords_Click(object sender, EventArgs e)
        {
            Import.SetNewStudentPasswords(new CSV(StudentsUpload.FileContent)).Save(Server.MapPath("~/Temp/NewStudentInfo.csv"));
            Response.Redirect("~/Temp/NewStudentInfo.csv");
        }
    }
}