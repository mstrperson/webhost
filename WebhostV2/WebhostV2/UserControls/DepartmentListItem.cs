using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class DepartmentListItem
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
                    if (db.Departments.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        Department activity = db.Departments.Where(act => act.id == value).Single();


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
        public DepartmentListItem(int id)
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
        public static List<DepartmentListItem> GetDataSource(List<int> actids)
        {
            List<DepartmentListItem> items = new List<DepartmentListItem>();
            foreach(int id in actids)
            {
                DepartmentListItem item = new DepartmentListItem(id);
                if(item.ID != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}