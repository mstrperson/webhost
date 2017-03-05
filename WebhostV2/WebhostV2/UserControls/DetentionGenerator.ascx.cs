using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class DetentionGenerator : LoggingUserControl
    {
        protected int lpc
        {
            get
            {
                try
                {
                    return Convert.ToInt32(LPCInput.Text);
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected int c1h
        {
            get
            {
                try
                {
                    return Convert.ToInt32(OneHrCutsInput.Text);
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected int c2h
        {
            get
            {
                try
                {
                    return Convert.ToInt32(TwoHrCutsInput.Text);
                }
                catch
                {
                    return -1;
                }
            }
        }
        protected int cc
        {
            get
            {
                try
                {
                    return Convert.ToInt32(CampusCutsInput.Text);
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                LPCInput.Text = Convert.ToString(AttendanceControl.LatesPerCut);
                OneHrCutsInput.Text = Convert.ToString(AttendanceControl.CutsPer1Hr);
                TwoHrCutsInput.Text = Convert.ToString(AttendanceControl.CutsPer2Hr);
                int campus = AttendanceControl.CutsPerCampus;
                CampusCutsInput.Text = Convert.ToString(campus);
                CampusCB.Checked = campus > 0;
                CampusCutsInput.Visible = campus > 0;
            }
        }

        protected void GenerateBtn_Click(object sender, EventArgs e)
        {
            OneHrDetentionListSelector.Clear();
            TwoHrDetentionListSelector.Clear();
            CampusedListSelector.Clear();
            Dictionary<String, List<int>> lists = AttendanceControl.GetWeekendDisciplineLists(new DateRange(DateRange.GetDateTimeFromString(StartDateInput.Text), DateRange.GetDateTimeFromString(EndDateInput.Text)), lpc, c1h, c2h, cc);
            OneHrDetentionListSelector.AddStudent(lists["1Hr"]);
            TwoHrDetentionListSelector.AddStudent(lists["2Hr"]);
            CampusedListSelector.AddStudent(lists["Campused"]);
        }
    }
}