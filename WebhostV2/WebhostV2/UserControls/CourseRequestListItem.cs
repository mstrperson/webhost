using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class CourseRequestListItem
    {public String Text
        {
            get;
            protected set;
        }

        private int _actid;
        public int ID
        {
            get
            {
                return _actid;
            }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.CourseRequests.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        WebhostMySQLConnection.CourseRequest activity = db.CourseRequests.Where(act => act.id == value).Single();


                        Text = String.Format(
                            "{0} ({1})", activity.RequestableCourse.Course.Name,
                            activity.APRequests.Count <= 0? "No Form Filed" : activity.APRequests.First().GradeTableEntry.Name
                            );
                    }
                    else
                    {
                        _actid = -1;
                        Text = "Error!";
                    }
                }
            }
        }

        /// <summary>
        /// Use a list of this class as a Datasource for Controls.
        /// List Displays the Activity Day, Time, and Name in the Text field
        /// </summary>
        /// <param name="id">WeekendActivity.id</param>
        public CourseRequestListItem(int id)
        {
            this.ID = id;
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
        public static List<CourseRequestListItem> GetDataSource(List<int> actids)
        {
            List<CourseRequestListItem> items = new List<CourseRequestListItem>();
            foreach(int id in actids)
            {
                CourseRequestListItem item = new CourseRequestListItem(id);
                if(item.ID != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}