using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls.AttendanceReview
{
    public partial class AttendanceTicker : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TickerTimer_Tick(sender, e);
        }

        protected void TickerTimer_Tick(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<AttendanceMarking> recentAttendances = (from att in db.AttendanceMarkings
                                                             where !att.Marking.Name.Equals("Present")
                                                             orderby att.SubmissionTime descending
                                                             select att).ToList();
                    //db.AttendanceMarkings.Where(att => !att.Marking.Name.Equals("Present")).OrderByDescending(att => att.AttendanceDate).ToList();
                TickerTable.Rows.Clear();
                TickerTable.Rows.Add(new AttendanceTickerRow());
                foreach(AttendanceMarking marking in recentAttendances)
                {
                    if(marking.AttendanceDate.Date.Equals(DateTime.Today))
                        TickerTable.Rows.Add(new AttendanceTickerRow(marking.id));
                }
            }
        }
    }
}