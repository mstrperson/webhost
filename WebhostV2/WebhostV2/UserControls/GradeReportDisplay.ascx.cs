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
    public partial class GradeReportDisplay : LoggingUserControl
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
                using (WebhostEntities db = new WebhostEntities())
                {
                    Student student = db.Students.Find(value);
                    if (student == null) throw new WebhostException("No such student ID");

                    GradeCalculation.GradeReport report = new GradeCalculation.GradeReport(value);
                    StudentNameLabel.Text = String.Format("{0} {1} [{2}]", student.FirstName, student.LastName, student.GraduationYear);
                    TotalCreditsLabel.Text = String.Format("{0} Credits", report.TotalCredits);
                    GPALabel.Text = Convert.ToString(report.GPA);

                    CreditBreakdown.Rows.Clear();
                    TableHeaderRow hr = new TableHeaderRow();
                    hr.Cells.Add(new TableHeaderCell() { Text = "Class" });
                    hr.Cells.Add(new TableHeaderCell() { Text = "Year/Term" });
                    hr.Cells.Add(new TableHeaderCell() { Text = "Credits" });
                    hr.Cells.Add(new TableHeaderCell() { Text = "Grade Earned" });
                    hr.Cells.Add(new TableHeaderCell() { Text = "Grade Points" });

                    CreditBreakdown.Rows.Add(hr);

                    foreach(int secid in report.Grades.Keys)
                    {
                        Section section = db.Sections.Find(secid);

                        TableRow row = new TableRow();
                        row.Cells.Add(new TableCell() { Text = section.Course.Name });
                        String terminfo = "";
                        if (section.Course.LengthInTerms > 1)
                            terminfo = String.Format("{0}-{1}", section.Course.AcademicYearID - 1, section.Course.AcademicYearID);
                        else
                            terminfo = String.Format("{0} {1}", section.Terms.FirstOrDefault().Name, section.Terms.FirstOrDefault().StartDate.Year);

                        row.Cells.Add(new TableCell() { Text = terminfo });

                        row.Cells.Add(new TableCell() { Text = report.Grades[secid].CreditWeight.ToString() });
                        row.Cells.Add(new TableCell() { Text = report.Grades[secid].DisplayName });
                        row.Cells.Add(new TableCell() { Text = report.Grades[secid].Value.ToString() });

                        CreditBreakdown.Rows.Add(row);
                    }
                }
                StudentIdField.Value = Convert.ToString(value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}