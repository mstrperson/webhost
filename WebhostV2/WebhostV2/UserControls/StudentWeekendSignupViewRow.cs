using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class StudentWeekendSignupViewRow : TableRow
    {
        private int _actid;
        public int ActivityId
        {
            get
            {
                return _actid;
            }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if(db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        WeekendActivity activity = db.WeekendActivities.Where(act => act.id == value).Single();

                        TableCell DayCell = new TableCell();
                        TableCell ActivityCell = new TableCell();
                        TableCell DescCell = new TableCell();
                        
                        String DayText = String.Format("{0} {1} {2}",
                            activity.DateAndTime.DayOfWeek == DayOfWeek.Friday ? "Friday" : activity.DateAndTime.DayOfWeek == DayOfWeek.Saturday ? "Saturday" : "Sunday",
                            activity.DateAndTime.Hour == 0 ? "" : activity.DateAndTime.ToShortTimeString(),
                            activity.DateAndTime.Hour == 0 || activity.Duration == 0 ? "" : activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString());

                        DayCell.Text = DayText;

                        ActivityCell.Text = activity.Name;
                        DescCell.Text = activity.Description;
                        
                        if(activity.IsDeleted)
                        {
                            DayCell.BackColor = System.Drawing.Color.DarkRed;
                            ActivityCell.BackColor = System.Drawing.Color.DarkRed;
                            DescCell.BackColor = System.Drawing.Color.DarkRed;
                        }

                        this.Cells.Add(DayCell);
                        this.Cells.Add(ActivityCell);
                        this.Cells.Add(DescCell);
                    }
                }
            }
        }

        public StudentWeekendSignupViewRow(int activityId)
        {
            this.ActivityId = activityId;
        }
    }
}