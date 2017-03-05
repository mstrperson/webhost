using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using System.Text.RegularExpressions;

namespace WebhostV2.UserControls
{
    public partial class MorningMeetingSeatClicker : LoggingUserControl
    {
        private static Regex markex = new Regex("(CUT|EXC|LATE|OK)");

        public static Dictionary<String, int> MapMarkingToGradeTableEntry()
        {
            int year = DateRange.GetCurrentAcademicYear();
            Dictionary<String, int> data = new Dictionary<string, int>();
            using (WebhostEntities db = new WebhostEntities())
            {
                GradeTable attendance = db.GradeTables.Where(tbl => tbl.AcademicYearID == year && tbl.Name.Equals("Attendance")).Single();

                data.Add("OK", attendance.GradeTableEntries.Where(ent => ent.Name.Equals("Present")).Single().id);
                data.Add("LATE", attendance.GradeTableEntries.Where(ent => ent.Name.Equals("Late")).Single().id);
                data.Add("CUT", attendance.GradeTableEntries.Where(ent => ent.Name.Equals("Cut")).Single().id);
                data.Add("EXC", attendance.GradeTableEntries.Where(ent => ent.Name.Equals("Excused")).Single().id);
            }

            return data;
        }

        public String ToolTip
        {
            get
            {
                return MarkingBtn.ToolTip;
            }
            set
            {
                MarkingBtn.ToolTip = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return MarkingBtn.Enabled;
            }
            set
            {
                if(!value) Marking = "EXC";
                MarkingBtn.Enabled = value;
            }
        }

        public int StudentId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(StudentIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                StudentIdField.Value = Convert.ToString(value);
            }
        }

        public String Marking
        {
            get
            {
                return markex.Match(MarkingBtn.ImageUrl).Value;
            }
            set
            {
                MarkingBtn.ImageUrl = String.Format("~/images/{0}.png", value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void MarkingBtn_Click(object sender, ImageClickEventArgs e)
        {
            switch(Marking)
            {
                case "OK": Marking = "LATE"; break;
                case "LATE": Marking = "CUT"; break;
                case "CUT": Marking = "EXC"; break;
                default: Marking = "OK"; break;
            }
        }
    }
}