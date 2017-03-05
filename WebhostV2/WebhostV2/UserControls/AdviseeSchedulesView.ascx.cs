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
    public partial class AdviseeSchedulesView : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                ADUser user = ((BasePage)Page).user;

                if (user.IsTeacher)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == user.ID).Single();

                    List<Student> advisees = faculty.Students.Where(s => s.isActive).ToList();

                    AdviseeTable.Rows.Clear();

                    foreach (Student student in advisees)
                    {
                        TableRow row = new TableRow();
                        TableCell cell = new TableCell();
                        StudentScheduleColapsePanel sscp = (StudentScheduleColapsePanel)LoadControl("~/UserControls/StudentScheduleColapsePanel.ascx");
                        sscp.StudentId = student.ID;
                        cell.Controls.Add(sscp);
                        row.Cells.Add(cell);
                        AdviseeTable.Rows.Add(row);
                    }
                }
                else
                {
                    Student student = db.Students.Where(s => s.ID == user.ID).Single();
                    AdviseeTable.Rows.Clear();
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    StudentScheduleColapsePanel sscp = (StudentScheduleColapsePanel)LoadControl("~/UserControls/StudentScheduleColapsePanel.ascx");
                    sscp.StudentId = student.ID;
                    cell.Controls.Add(sscp);
                    row.Cells.Add(cell);
                    AdviseeTable.Rows.Add(row);
                }
            }
        }
    }
}