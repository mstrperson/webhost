using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class CourseListItem
    {
        public String Text
        {
            get;
            protected set;
        }

        private int _actid;
        public int CourseId
        {
            get
            {
                return _actid;
            }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.Courses.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        Course activity = db.Courses.Where(act => act.id == value).Single();


                        Text = activity.Name;
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
        public CourseListItem(int id)
        {
            this.CourseId = id;
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
        public static List<CourseListItem> GetDataSource(List<int> actids)
        {
            List<CourseListItem> items = new List<CourseListItem>();
            foreach(int id in actids)
            {
                CourseListItem item = new CourseListItem(id);
                if(item.CourseId != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}