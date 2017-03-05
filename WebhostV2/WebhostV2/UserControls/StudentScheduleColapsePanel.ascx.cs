using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class StudentScheduleColapsePanel : LoggingUserControl
    {
        public int StudentId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(StudentIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                int year = DateRange.GetCurrentAcademicYear();
                List<int> sections = new List<int>();
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.Students.Where(s => s.ID == value).Count() > 0)
                    {
                        StudentIdField.Value = Convert.ToString(value);
                        Student student = db.Students.Where(s => s.ID == value).Single();
                        StudentNameLabel.Text = String.Format("{0} {1} ({2})", student.FirstName, student.LastName, student.GraduationYear);
                        sections = student.Sections.Where(sec => sec.Course.AcademicYearID == year).OrderBy(sec => sec.BlockIndex).Select(sec => sec.id).ToList();

                        ScheduleTable.Rows.Clear();
                        ScheduleTable.Rows.Add(new ScheduleTableRow());
                        foreach (int id in sections)
                        {
                            Section section = db.Sections.Where(sec => sec.id == id).Single();
                            if (!section.Block.ShowInSchedule) continue;
                            ScheduleTable.Rows.Add(new ScheduleTableRow(id));
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}