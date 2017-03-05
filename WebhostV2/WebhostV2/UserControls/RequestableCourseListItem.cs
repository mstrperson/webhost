using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class RequestableCourseListItem
    {
        public String Text
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
                    if (db.RequestableCourses.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        RequestableCourse activity = db.RequestableCourses.Where(act => act.id == value).Single();


                        Text = activity.Course.Name;
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
        public RequestableCourseListItem(int id)
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
        /// <param name="actids">List of RequestableCourse.id</param>
        /// <returns></returns>
        public static List<RequestableCourseListItem> GetDataSource(List<int> actids)
        {
            List<RequestableCourseListItem> items = new List<RequestableCourseListItem>();
            foreach(int id in actids)
            {
                RequestableCourseListItem item = new RequestableCourseListItem(id);
                if(item.ID != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}