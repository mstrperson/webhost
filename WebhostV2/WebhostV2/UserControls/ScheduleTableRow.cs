using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class ScheduleTableRow:TableRow
    {
        public int SectionId
        {
            get;
            protected set;
        }

        public ScheduleTableRow()
        {
            TableHeaderCell blockCell = new TableHeaderCell() { Text = "Block" };
            TableHeaderCell summerCell = new TableHeaderCell() { Text = "Summer" };
            TableHeaderCell fallCell = new TableHeaderCell() { Text = "Fall" };
            TableHeaderCell winterCell = new TableHeaderCell() { Text = "Winter" };
            TableHeaderCell springCell = new TableHeaderCell() { Text = "Spring" };
            this.Cells.Add(blockCell);
            this.Cells.Add(summerCell);
            this.Cells.Add(fallCell);
            this.Cells.Add(winterCell);
            this.Cells.Add(springCell);
        }

        protected String SectionInfo(Section section)
        {
            String info = section.Course.Name;
            if(section.Teachers.Count > 0)
            {
                info += Environment.NewLine + "(";
                bool first = true;
                foreach(Faculty tchr in section.Teachers.ToList())
                {
                    info += String.Format("{0}{1} {2}", first ? "" : ", ", tchr.FirstName, tchr.LastName);
                }

                info += ")";
            }

            return info;
        }

        public ScheduleTableRow(int secid)
        {
            this.SectionId = secid;
            using (WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == secid).Single();

                TableCell blockCell = new TableCell()
                {
                    Text = section.Block.LongName
                };
                TableCell summerCell = new TableCell()
                {
                    Text = section.Terms.Where(t => t.Name.Equals("Summer")).Count() > 0 ? SectionInfo(section) : ""
                };
                TableCell fallCell = new TableCell()
                {
                    Text = section.Terms.Where(t => t.Name.Equals("Fall")).Count() > 0 ? SectionInfo(section) : ""
                };
                TableCell winterCell = new TableCell()
                {
                    Text = section.Terms.Where(t => t.Name.Equals("Winter")).Count() > 0 ? SectionInfo(section) : ""
                };
                TableCell springCell = new TableCell()
                {
                    Text = section.Terms.Where(t => t.Name.Equals("Spring")).Count() > 0 ? SectionInfo(section) : ""
                };

                this.Cells.Add(blockCell);
                this.Cells.Add(summerCell);
                this.Cells.Add(fallCell);
                this.Cells.Add(winterCell);
                this.Cells.Add(springCell);
            }
        }
    }
}