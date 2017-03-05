using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class DutyScheduleView : LoggingUserControl
    {
        protected bool IsMobile
        {
            get
            {
                return Request.RawUrl.Contains("Mobile");
            }
        }

        public int WeekendId
        {
            get
            {
                if (Session["weekendId"] == null)
                    Session["weekendId"] = DateRange.GetCurrentWeekendId();

                return (int)Session["weekendId"];
            }
            protected set
            {
                Session["weekendId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if(WeekendId == -1)
                {
                    WeekendLabel.Text = "No information Yet!";
                    return;
                }
                Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();

                WeekendLabel.Text = weekend.DutyTeam.Name + " Weekend!";
                ActivitiesTable.Rows.Clear();
                ActivitiesTable.Rows.Add(DutyScheduleRow.HeaderRow);
                List<DutyScheduleRow> rows = new List<DutyScheduleRow>();
                foreach (WeekendActivity activity in weekend.WeekendActivities.Where(act => !act.IsDeleted).ToList())
                {
                    rows.Add(new DutyScheduleRow(activity.id, false, IsMobile, ((BasePage)Page).user.IsTeacher));
                }
                if (((BasePage)Page).user.IsTeacher)
                {
                    foreach (WeekendDuty duty in weekend.WeekendDuties.Where(dut => !dut.IsDeleted).ToList())
                    {
                        rows.Add(new DutyScheduleRow(duty.id, true, IsMobile, true));
                    }
                }
                rows.Sort();

                ActivitiesTable.Rows.AddRange(rows.ToArray());
            }
        }
    }
}