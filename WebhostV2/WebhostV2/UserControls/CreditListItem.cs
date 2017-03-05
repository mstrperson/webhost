using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class CreditListItem
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
                    if (db.Credits.Where(act => act.id == value).Count() > 0)
                    {
                        this._actid = value;

                        Credit credit = db.Credits.Where(act => act.id == value).Single();
                        
                        if(credit.Sections.Count > 0)
                        {
                            Section section = credit.Sections.Single();
                            Text = String.Format("[{4}] {0} {1} {2}-{3}", section.Course.Name, section.Course.LengthInTerms > 1 ? "Full Year" : section.Terms.Single().Name, section.Course.AcademicYearID - 1, section.Course.AcademicYearID, credit.CreditType.Name);
                        }
                        else
                        {
                            Text = String.Format("[{0}] {1}", credit.CreditType.Name, credit.Notes);
                        }
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
        public CreditListItem(int id)
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
        public static List<CreditListItem> GetDataSource(List<int> actids)
        {
            List<CreditListItem> items = new List<CreditListItem>();
            foreach(int id in actids)
            {
                CreditListItem item = new CreditListItem(id);
                if(item.ID != -1)
                    items.Add(item);
            }

            return items;
        }
    }
}