using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class MorningMeetingAttendanceChart : LoggingUserControl
    {
        protected List<MorningMeetingSeatClicker> seatControls
        {
            get
            {
                return new List<MorningMeetingSeatClicker>()
                {
                    A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,
                    B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,
                    C1,C2,C3,C4,C5,C6,C7,C8,C9,C10,C11,C12,C13,C14,
                    D1,D2,D3,D4,D5,D6,D7,D8,D9,D10,D11,D12,D13,D14,
                    E1,E2,E3,E4,E5,E6,E7,E8,E9,E10,E11,E12,E13,E14,
                    F1,F2,F3,F4,F5,F6,F7,F8,F9,F10,F11,F12,F13,F14,
                    G1,G2,G3,G4,G5,G6,G7,G8,G9,G10,G11,G12,G13,G14,
                    H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,
                    I1,I2,I3,I4,I5,I6,I7,I8,I9,I10,//I11,I12,I13,I14,
                    J1,J2,J3,J4,J5,J6,J7,J8,J9,J10,
                    K1,K2,K3,K4,K5,K6,K7,K8,K9,K10,K11,
                    L1,L2,L3,L4,L5,L6,L7,L8,L9,L10,L11/*,
                    M1,M2,M3,M4,M5,M6,M7,M8,M9,M10,M11,M12,M13,M14*/
                };
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                try
                {
                    return DateRange.GetDateTimeFromString(DateInput.Text);
                }
                catch
                {
                    DateInput.Text = DateTime.Today.ToShortDateString();
                    return DateTime.Today;
                }
            }
            set
            {
                DateInput.Text = value.ToShortDateString();
            }
        }

        protected void LoadTable()
        {
            Dictionary<String, int> map = MorningMeetingSeatClicker.MapMarkingToGradeTableEntry();
            Dictionary<int, String> revmap = new Dictionary<int,string>();
            foreach(String key in map.Keys)
            {
                revmap.Add(map[key], key);
            }

            using(WebhostEntities db = new WebhostEntities())
            {
                int mmid = AttendanceControl.MorningMeetingSectionId();
                Section morningmeeting = db.Sections.Where(sec => sec.id == mmid).Single(); 
                if (morningmeeting.SeatingCharts.Count <= 0) 
                    MailControler.MailToUser("Morning Meeting Seating Chart.", AttendanceControl.GenerateMorningMeetingSeatingChart(), ((BasePage)Page).user);

                String rows = "ABCDEFGHIJKL";
                SeatingChart chart = db.Sections.Where(sec => sec.id == mmid).Single().SeatingCharts.ToList().First();
                foreach(SeatingChartSeat seat in chart.SeatingChartSeats.Where(s => s.StudentId.HasValue).ToList())
                {
                    String id = String.Format("{0}{1}", rows[seat.Row], seat.Column + 1);
                    seatControls.Where(sc => sc.ID.Equals(id)).Single().StudentId = seat.StudentId.Value;
                    seatControls.Where(sc => sc.ID.Equals(id)).Single().ToolTip = String.Format("{0} {1}", seat.Student.FirstName, seat.Student.LastName);
                    if(morningmeeting.AttendanceMarkings.Where(mk => mk.AttendanceDate.Equals(SelectedDate) && mk.StudentID == seat.StudentId.Value).Count() > 0)
                    {
                        seatControls.Where(sc => sc.ID.Equals(id)).Single().Marking = revmap[morningmeeting.AttendanceMarkings.Where(mk => mk.AttendanceDate.Equals(SelectedDate) && mk.StudentID == seat.StudentId.Value).Single().MarkingIndex];
                    }
                    else
                    {
                        seatControls.Where(sc => sc.ID.Equals(id)).Single().Marking = "OK";
                    }
                }

                foreach(MorningMeetingSeatClicker clicker in seatControls)
                {
                    clicker.Enabled = clicker.StudentId != -1;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!((BasePage)Page).user.IsTeacher) Response.Redirect("~/Home.aspx");

            if(!Page.IsPostBack)
            {
                SelectedDate = DateTime.Today;
                LoadTable();
            }
        }

        protected void MarkLatesBtn_Click(object sender, EventArgs e)
        {
            foreach(int id in LateStudentsSelector.GroupIds)
            {
                seatControls.Where(ctrl => ctrl.StudentId == id).Single().Marking = "LATE";
            }
            LateStudentsSelector.Clear();
        }

        protected void DateSelectBtn_Click(object sender, EventArgs e)
        {
            LoadTable();
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            Dictionary<String, int> map = MorningMeetingSeatClicker.MapMarkingToGradeTableEntry();

            int mmid = AttendanceControl.MorningMeetingSectionId();
                
            Dictionary<int, AttendanceControl.ShortMarking> list = new Dictionary<int, AttendanceControl.ShortMarking>();
            foreach(MorningMeetingSeatClicker clicker in seatControls)
            {
                if (!clicker.Enabled || clicker.StudentId == -1) continue;
                list.Add(clicker.StudentId, new AttendanceControl.ShortMarking() { notes = "", markId = map[clicker.Marking] });
            }

            AttendanceControl.SubmitAttendance(mmid, list, 9997, SelectedDate);
        }
    }
}