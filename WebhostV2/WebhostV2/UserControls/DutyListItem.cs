using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebhostV2.UserControls
{
    public abstract class DutyListItem : IComparable
    {
        public String Text
        {
            get;
            protected set;
        }

        public String Value
        {
            get;
            protected set;
        }

        public DateTime sortDateTime
        {
            get;
            protected set;
        }

        protected int _actid;
        public abstract int Id
        {
            get;
            protected set;
        }

        public DutyListItem(int id)
        {
            this.Id = id;
        }

        public static List<DutyListItem> GetDataSource(List<int> activityIds, List<int> dutyIds)
        {
            List<DutyListItem> items = new List<DutyListItem>();
            items.AddRange(ActivityListItem.GetDataSource(activityIds));
            items.AddRange(FacultyDutyListItem.GetDataSource(dutyIds));
            items.Sort();
            return items;
        }

        public int CompareTo(object obj)
        {
            if(obj is DutyListItem)
            {
                return this.sortDateTime.CompareTo(((DutyListItem)obj).sortDateTime);
            }
            else
            {
                throw new InvalidOperationException("Cannot Compare these types of objects.");
            }
        }
    }
}