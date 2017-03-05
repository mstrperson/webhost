using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2
{
    public partial class ScheduleEditor : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    StudentSelectDDL.DataSource = (from student in db.Students
                                                   orderby student.LastName, student.FirstName
                                                   select new
                                                   {
                                                       Text = student.FirstName + " " + student.LastName + " (" + student.GraduationYear + ")",
                                                       id = student.ID
                                                   }).ToList();
                    StudentSelectDDL.DataTextField = "Text";
                    StudentSelectDDL.DataValueField = "id";
                    StudentSelectDDL.DataBind();

                    FacultySelectDDL.DataSource = (from teacher in db.Faculties
                                                   orderby teacher.LastName, teacher.FirstName
                                                   select new
                                                   {
                                                       Text = teacher.FirstName + " " + teacher.LastName,
                                                       id = teacher.ID
                                                   }).ToList();
                    FacultySelectDDL.DataTextField = "Text";
                    FacultySelectDDL.DataValueField = "id";
                    FacultySelectDDL.DataBind();
                }
            }
        }

        protected void StudentSelectBtn_Click(object sender, EventArgs e)
        {
            ScheduleView1.UserId = Convert.ToInt32(StudentSelectDDL.SelectedValue);
            ScheduleView1.IsStudent = true;
            ScheduleView1.PopulateTable();
        }

        protected void FacultySelectBtn_Click(object sender, EventArgs e)
        {
            ScheduleView1.UserId = Convert.ToInt32(FacultySelectDDL.SelectedValue);
            ScheduleView1.IsStudent = false;
            ScheduleView1.PopulateTable();
        }
    }
}