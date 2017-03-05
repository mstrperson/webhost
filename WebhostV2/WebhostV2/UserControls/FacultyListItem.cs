using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class FacultyListItem
    {
        private int _secid;

        public int ID
        {
            get
            {
                return _secid;
            }
            protected set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.Faculties.Where(sec => sec.ID == value).Count() > 0)
                    {
                        _secid = value;
                        Faculty section = db.Faculties.Where(sec => sec.ID == value).Single();
                        Text = String.Format("{0} {1}", section.FirstName, section.LastName);
                    }
                    else
                    {
                        _secid = -1;
                    }
                }
            }
        }

        public String Text
        {
            get;
            protected set;
        }

        public FacultyListItem(int id)
        {
            ID = id;
        }

        public static List<FacultyListItem> GetDataSource(List<int> ids)
        {
            List<FacultyListItem> items = new List<FacultyListItem>();
            foreach(int id in ids)
            {
                FacultyListItem item = new FacultyListItem(id);
                if (item.ID != -1)
                    items.Add(item);
            }
            return items;
        }
    }
}