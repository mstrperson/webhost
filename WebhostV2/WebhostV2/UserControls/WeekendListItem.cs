using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class WeekendListItem
    {
        private int _wkid;

        public int WeekendId
        {
            get
            {
                return _wkid;
            }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.Weekends.Where(w => w.id == value).Count() > 0)
                    {
                        _wkid = value;
                        Weekend weekend = db.Weekends.Where(w => w.id == value).Single();
                        Text = String.Format("{0} : {1} ~ {2}", weekend.DutyTeam.Name, weekend.StartDate.ToShortDateString(), weekend.EndDate.ToShortDateString());
                    }
                    else
                        _wkid = -1;
                }
            }
        }

        public String Text
        {
            get;
            protected set;
        }

        public WeekendListItem(int id)
        {
            WeekendId = id;
        }

        public static List<WeekendListItem> GetDataSource(List<int> ids)
        {
            List<WeekendListItem> weekends = new List<WeekendListItem>();
            foreach (int id in ids)
            {
                WeekendListItem item = new WeekendListItem(id);
                if (item._wkid == -1) continue;
                weekends.Add(new WeekendListItem(id));
            }

            return weekends;
        }
    }
}