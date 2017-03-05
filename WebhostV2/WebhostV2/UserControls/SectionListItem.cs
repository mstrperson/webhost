using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class SectionListItem
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
                    if(db.Sections.Where(sec => sec.id == value).Count() > 0)
                    {
                        _secid = value;
                        Section section = db.Sections.Where(sec => sec.id == value).Single();
                        if(section.Terms.Count <= 0)
                        {
                            _secid = -1;
                            return;
                        }
                        Text = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name);
                        if(section.Course.LengthInTerms < 3)
                        {
                            Text += String.Format(" ({0})", section.Terms.ToList().First().Name);
                        }

                        ExtendedText = Text;

                        if(section.Teachers.Count > 0)
                        {
                            foreach(Faculty teacher in section.Teachers.ToList())
                            {
                                ExtendedText += String.Format(" [{0} {1}]", teacher.FirstName, teacher.LastName);
                            }
                        }
                    }
                    else
                    {
                        _secid = -1;
                    }
                }
            }
        }

        public String ExtendedText
        {
            get;
            protected set;
        }

        public String Text
        {
            get;
            protected set;
        }

        public SectionListItem(int id)
        {
            ID = id;
        }

        public static List<SectionListItem> GetDataSource(List<int> ids)
        {
            List<SectionListItem> items = new List<SectionListItem>();
            foreach(int id in ids)
            {
                SectionListItem item = new SectionListItem(id);
                if (item.ID != -1)
                    items.Add(item);
            }
            return items;
        }
    }
}