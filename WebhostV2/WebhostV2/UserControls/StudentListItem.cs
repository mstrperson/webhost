using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class StudentListItem
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
                    if(db.Students.Where(sec => sec.ID == value).Count() > 0)
                    {
                        _secid = value;
                        Student section = db.Students.Where(sec => sec.ID == value).Single();
                        Text = String.Format("{0} {1} ({2})", section.FirstName, section.LastName, section.GraduationYear);
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

        public StudentListItem(int id)
        {
            ID = id;
        }

        public static List<StudentListItem> GetDataSource(List<int> ids)
        {
            List<StudentListItem> items = new List<StudentListItem>();
            foreach(int id in ids)
            {
                StudentListItem item = new StudentListItem(id);
                if (item.ID != -1)
                    items.Add(item);
            }
            return items;
        }
    }
}