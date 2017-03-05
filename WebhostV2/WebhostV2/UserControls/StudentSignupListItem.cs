using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class StudentSignupListItem
    {
        private int[] _secid;

        public String IDStr
        {
            get
            {
                String str = "";
                foreach(int i in _secid)
                {
                    str += String.Format("{0}{1}", i == 0 ? "" : ",", i);
                }
                return str;
            }
        }

        /// <summary>
        /// Array of two values { WeekendActivity.id, Student.id }
        /// </summary>
        public int[] ID
        {
            get
            {
                return _secid;
            }
            protected set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    int actid = value[0];
                    int sid = value[1];
                    if(db.StudentSignups.Where(sec => sec.ActivityId == actid && sec.StudentId == sid).Count() > 0)
                    {
                        _secid = value;
                        WebhostMySQLConnection.StudentSignup section = db.StudentSignups.Where(sec => sec.ActivityId == actid && sec.StudentId == sid).Single();
                        Text = String.Format("{0} {1} ({2}){3}{4}", section.Student.FirstName, section.Student.LastName, section.Student.GraduationYear, section.IsBanned?" [BANNED]":"", section.IsRescended && !section.IsBanned?" [Not Going]":"");
                    }
                    else
                    {
                        _secid = new int[] { -1, -1 };
                    }
                }
            }
        }

        public String Text
        {
            get;
            protected set;
        }

        public StudentSignupListItem(int[] id)
        {
            ID = id;
        }

        public static List<StudentSignupListItem> GetDataSource(int activityId, List<int> ids)
        {
            List<StudentSignupListItem> items = new List<StudentSignupListItem>();
            foreach(int id in ids)
            {
                StudentSignupListItem item = new StudentSignupListItem(new int[] {activityId, id});
                if (item.ID[0] != -1)
                    items.Add(item);
            }
            return items;
        }
    }
}