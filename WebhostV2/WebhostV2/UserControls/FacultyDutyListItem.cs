using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class FacultyDutyListItem : DutyListItem
    {
        public override int Id
        {
            get
            {
                return _actid;
            }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.WeekendDuties.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        WeekendDuty activity = db.WeekendDuties.Where(act => act.id == value).Single();

                        sortDateTime = activity.DateAndTime;

                        Text = String.Format("{0} {1} {2}",
                            activity.DateAndTime.DayOfWeek == DayOfWeek.Friday ? "Friday" : activity.DateAndTime.DayOfWeek == DayOfWeek.Saturday ? "Saturday" : "Sunday",
                            activity.DateAndTime.Hour == 0 ? "" : activity.DateAndTime.ToShortTimeString(),
                            activity.DateAndTime.Hour == 0 || activity.Duration == 0 ? "" : activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString());

                        Text += " ~ " + activity.Name;
                    }
                    else
                    {
                        _actid = -1;
                        Text = "Error!";
                    }

                    Value = String.Format("D{0}", _actid);
                }
            }
        }

        public FacultyDutyListItem(int id) : base(id) { }

        /// <summary>
        /// Get a Datasource List for use in Web Controls.
        /// 
        /// DataTextField = "Text"
        /// DataValueField = "Id"
        /// 
        /// Invalid IDs are dropped without notification.
        /// 
        /// </summary>
        /// <param name="actids">List of WeekendActivity.id</param>
        /// <returns></returns>
        public static List<FacultyDutyListItem> GetDataSource(List<int> actids)
        {
            List<FacultyDutyListItem> items = new List<FacultyDutyListItem>();
            foreach (int id in actids)
            {
                FacultyDutyListItem item = new FacultyDutyListItem(id);
                if (item.Id != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}