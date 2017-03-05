using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class TermListItem
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
                    if (db.Terms.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        Term term = db.Terms.Where(act => act.id == value).Single();


                        Text = String.Format("{0} {1}", term.Name, term.StartDate.Year);
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
        /// List Displays the term Day, Time, and Name in the Text field
        /// </summary>
        /// <param name="id">Weekendterm.id</param>
        public TermListItem(int id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Get a Datasource List for use in Web Controls.
        /// 
        /// DataTextField = "Text"
        /// DataValueField = "termId"
        /// 
        /// Invalid IDs are dropped without notification.
        /// 
        /// </summary>
        /// <param name="actids">List of Weekendterm.id</param>
        /// <returns></returns>
        public static List<TermListItem> GetDataSource(List<int> actids)
        {
            List<TermListItem> items = new List<TermListItem>();
            foreach(int id in actids)
            {
                TermListItem item = new TermListItem(id);
                if(item.ID != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}