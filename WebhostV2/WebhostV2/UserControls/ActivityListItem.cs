using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class ActivityListItem : DutyListItem
    {
        /*public String Text
        {
            get;
            protected set;
        }

        private int _actid;*/
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
                    if (db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        WeekendActivity activity = db.WeekendActivities.Where(act => act.id == value).Single();

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

                    Value = String.Format("A{0}", _actid);
                }
            }
        }

        /// <summary>
        /// Use a list of this class as a Datasource for Controls.
        /// List Displays the Activity Day, Time, and Name in the Text field
        /// </summary>
        /// <param name="id">WeekendActivity.id</param>
        public ActivityListItem(int id) : base(id)
        {
        }

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
        public static List<ActivityListItem> GetDataSource(List<int> actids)
        {
            List<ActivityListItem> items = new List<ActivityListItem>();
            foreach(int id in actids)
            {
                ActivityListItem item = new ActivityListItem(id);
                if(item.Id != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}