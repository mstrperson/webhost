using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;
using System.Web.UI.WebControls;

namespace WebhostV2.UserControls
{
    public class ActivityAttendanceTableRow : TableRow
    {
        public static TableHeaderRow HeaderRow
        {
            get
            {
                TableHeaderRow hr = new TableHeaderRow();

                hr.Cells.AddRange(new TableHeaderCell[]
                                    {   
                                        new TableHeaderCell() {Text="#"},
                                        new TableHeaderCell() { Text="Student"}
                                    });

                return hr;
            }
        }

        protected CheckBox AttendedCB;
        protected TableCell CBCell;
        protected TableCell NumberCell;

        public int StudentId { get; protected set; }

        public bool IsAttending
        {
            get
            {
                return AttendedCB.Checked;
            }
        }

        public ActivityAttendanceTableRow(int studentId, String studentName, int position, bool attended = false, bool active = true)
        {
            AttendedCB = new CheckBox() { Text = studentName, Checked = attended, Enabled = active };
            CBCell = new TableCell();
            NumberCell = new TableCell() { Text = String.Format("[{0}]", position) };
            this.Cells.Add(NumberCell);
            CBCell.Controls.Add(AttendedCB);
            this.Cells.Add(CBCell);

            this.StudentId = studentId;
        }
    }
}