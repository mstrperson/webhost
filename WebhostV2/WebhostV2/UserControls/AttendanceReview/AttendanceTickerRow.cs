using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls.AttendanceReview
{
    public class AttendanceTickerRow : TableRow
    {
        public int AttendanceMarkId
        {
            get;
            protected set;
        }

        public AttendanceTickerRow()
        {
            this.Cells.Add(new TableHeaderCell() { Text = "Student Name" });
            this.Cells.Add(new TableHeaderCell() { Text = "Section" });
            this.Cells.Add(new TableHeaderCell() { Text = "Marking" });
            this.Cells.Add(new TableHeaderCell() { Text = "Time" });
            this.Cells.Add(new TableHeaderCell() { Text = "Entered By" });
        }

        public AttendanceTickerRow(int id)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.AttendanceMarkings.Where(mk => mk.id == id).Count() <= 0) throw new WebhostMySQLConnection.Web.WebhostException("Invalid AttendanceMarkingId.");

                AttendanceMarking marking = db.AttendanceMarkings.Where(mk => mk.id == id).Single();
                this.AttendanceMarkId = id;

                TableCell nameCell = new TableCell()
                {
                    Text = String.Format("{0} {1} [{2}]", marking.Student.FirstName, marking.Student.LastName, marking.Student.GraduationYear),
                    ToolTip = String.Format("{0} {1}", marking.Student.Advisor.FirstName, marking.Student.Advisor.LastName)
                };
                this.Cells.Add(nameCell);

                TableCell classCell = new TableCell();
                List<String> blocks = new List<string>() {"A", "B", "C", "D", "E", "F"};
                if(blocks.Contains(marking.Section.Block.Name))
                {
                    classCell.Text = String.Format("[{0}] {1}", marking.Section.Block.LongName, marking.Section.Course.Name);
                    bool first = true;
                    foreach(Faculty teacher in marking.Section.Teachers)
                    {
                        classCell.ToolTip += String.Format("{0}{1} {2}", first ? "" : Environment.NewLine, teacher.FirstName, teacher.LastName);
                        first = false;
                    }
                }
                else
                {
                    classCell.Text = marking.Section.Course.Name;
                }

                this.Cells.Add(classCell);

                TableCell markingCell = new TableCell()
                {
                    Text = marking.Marking.Name,
                    ToolTip = marking.Notes
                };
                this.Cells.Add(markingCell);

                TableCell timeCell = new TableCell()
                {
                    Text = marking.SubmissionTime.ToLongTimeString(),
                    ToolTip = marking.AttendanceDate.ToLongDateString()
                };
                this.Cells.Add(timeCell);


                TableCell facCell = new TableCell()
                {
                    Text = marking.Notes.IndexOf("]") > 0 ? marking.Notes.Substring(1, marking.Notes.IndexOf("]")-1) : "Unknown"
                };
                this.Cells.Add(facCell);
            }
        }
    }
}