using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls.AttendanceReview
{
    public partial class StudentAttendanceTable : LoggingUserControl
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
                StudentIdField.Value = Convert.ToString(value);
            }
        }

        public DateRange AttendancePeriod
        {
            get
            {
                if (Session["attper"] == null)
                    Session["attper"] = new DateRange(DateRange.ThisFriday.AddDays(-7), DateRange.ThisFriday.AddDays(-1));

                return (DateRange)Session["attper"];
            }
        }

        public void LoadTable()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int termId = DateRange.GetCurrentOrLastTerm();
                Student student = db.Students.Where(s => s.ID == StudentId).Single();

                ((BasePage)Page).log.WriteLine("{0} Loading Attendance Table for {1} {2} [{3}].", Log.TimeStamp(), student.FirstName, student.LastName, student.GraduationYear);
                List<AttendanceMarking> markings = student.AttendanceMarkings.Where(mk => mk.AttendanceDate >= AttendancePeriod.Start && mk.AttendanceDate <= AttendancePeriod.End).ToList();
                ((BasePage)Page).log.WriteLine("{0} Grabbed {1} attendances.", Log.TimeStamp(), markings.Count);
                int lates = 0, cuts=0, excused=0;

                foreach(AttendanceMarking mark in markings)
                {
                    switch(mark.Marking.Name)
                    {
                        case "Late": lates++;
                            break;
                        case "Cut": cuts++;
                            break;
                        case "Excused": excused++;
                            break;
                        default: break;
                    }
                }
                ((BasePage)Page).log.WriteLine("{0} Counted:  {1} lates, {2} cuts, and {3} excused.", Log.TimeStamp(), lates, cuts, excused);
                NameLabel.Text = String.Format("{0} {1} [{2}]", student.FirstName, student.LastName, student.GraduationYear);
                LatesLabel.Text = Convert.ToString(lates);
                CutsLabel.Text = Convert.ToString(cuts);
                ExcusedLabel.Text = Convert.ToString(excused);
                ((BasePage)Page).log.WriteLine("{0} Generating Table", Log.TimeStamp());
                TableHeaderRow hrow = new TableHeaderRow();
                TableHeaderCell ncell = new TableHeaderCell()
                {
                    Text = "Class"
                };
                hrow.Cells.Add(ncell);
                foreach(DateTime date in AttendancePeriod.ToList())
                {
                    TableHeaderCell dc = new TableHeaderCell()
                    {
                        Text = date.ToLongDateString()
                    };
                    hrow.Cells.Add(dc);
                }

                ScheduleTable.Rows.Add(hrow);
                ((BasePage)Page).log.WriteLine("{0} Header Added.", Log.TimeStamp());

                foreach (Section section in student.Sections.Where(sec => sec.Terms.Count <= 0 || sec.Terms.Contains(db.Terms.Where(t => t.id == termId).Single())).OrderBy(sec => sec.BlockIndex).ToList())
                {
                    ((BasePage)Page).log.WriteLine("{0} Adding Row for [{1}] {2}", Log.TimeStamp(), section.Block.LongName, section.Course.Name);
                    TableRow row = new TableRow();
                    TableHeaderCell sectionCell = new TableHeaderCell()
                    {
                        Text = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name)
                    };
                    row.Cells.Add(sectionCell);
                    foreach (DateTime date in AttendancePeriod.ToList())
                    {
                        ((BasePage)Page).log.WriteLine("{0} Examining {1}", Log.TimeStamp(), date.ToShortDateString());
                        TableCell cell = new TableCell();
                        if(student.AttendanceMarkings.Where(mk => mk.Section.Equals(section) && mk.AttendanceDate.Equals(date)).Count() == 1)
                        {
                            AttendanceMarking mark = student.AttendanceMarkings.Where(mk => mk.Section.Equals(section) && mk.AttendanceDate.Equals(date)).Single();
                            cell.Text = mark.Marking.Name;
                            cell.ToolTip = mark.Notes;
                            ((BasePage)Page).log.WriteLine("{0} Displaying {1}.", Log.TimeStamp(), mark.Marking.Name);
                        }
                        else if(student.AttendanceMarkings.Where(mk => mk.Section.Equals(section) && mk.AttendanceDate.Equals(date)).Count() > 1)
                        {

                            ((BasePage)Page).log.WriteLine("{0} Too many markings!.", Log.TimeStamp());
                            cell.Text = "Multiple attendance markings!!!";
                            foreach(AttendanceMarking mark in student.AttendanceMarkings.Where(mk => mk.Section.Equals(section) && mk.AttendanceDate.Equals(date)).ToList())
                            {
                                cell.ToolTip += mark.Marking.Name + Environment.NewLine;
                            }
                        }
                        else
                        {
                            ((BasePage)Page).log.WriteLine("{0} No Data.", Log.TimeStamp());
                            cell.Text = "*";
                            cell.ToolTip = "No Data.";
                        }
                        row.Cells.Add(cell);
                    }

                    ScheduleTable.Rows.Add(row);

                    ((BasePage)Page).log.WriteLine("{0} Added Row to Table.", Log.TimeStamp());
                }
            }

            ((BasePage)Page).log.WriteLine("{0} Done.", Log.TimeStamp());
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}