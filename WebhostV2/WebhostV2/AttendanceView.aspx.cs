using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class AttendanceView : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    StudentSelector.DataSource = StudentListItem.GetDataSource(db.Students.Where(s => s.isActive).OrderBy(s => s.LastName).ThenBy(s=>s.FirstName).Select(s => s.ID).ToList());
                    StudentSelector.DataTextField = "Text";
                    StudentSelector.DataValueField = "ID";
                    StudentSelector.DataBind();
                }
            }
        }

        protected void SelectDatesBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Session["attper"] = new DateRange(DateRange.GetDateTimeFromString(StartDateInput.Text), DateRange.GetDateTimeFromString(EndDateInput.Text));
            }
            catch(InvalidCastException)
            {
                ((Default)Page.Master).ShowError("Invalid Date Range Selected.", 
                    String.Format("Could not parse dates from '{0}'~'{1}' to a valid Date Range.", StartDateInput.Text, EndDateInput.Text));
                return;
            }
        }

        protected void LoadBtn_Click(object sender, EventArgs e)
        {
            if (StudentSelector.SelectedValue.Equals("")) return;
            StudentAttendanceTable1.StudentId = Convert.ToInt32(StudentSelector.SelectedValue);
            StudentAttendanceTable1.LoadTable();
        }

        protected void DetentionListBtn_Click(object sender, EventArgs e)
        {
            CSV csv = AttendanceControl.GetDetentionLists(DateRange.ThisAttendanceWeek);
            csv.Save(Server.MapPath("~/Temp/Detention.csv"));
            Response.Redirect("~/Temp/Detention.csv");
        }

        protected void SendEmail_Click(object sender, EventArgs e)
        {
            AttendanceControl.SendDetentionEmail(AttendanceControl.GetDetentionList(DateRange.ThisAttendanceWeek));
            ((Default)Page.Master).ShowSuccess("Detention Email has been Sent.", "");
        }

        protected void SummaryBtn_Click(object sender, EventArgs e)
        {
            if (StartDateInput.Text.Equals(""))
            {
                AttendanceControl.AttendanceDump(new DateRange(DateRange.ThisFriday.AddDays(-7), DateRange.ThisFriday.AddDays(-1))).Save(Server.MapPath("~/Temp/AttendanceDump.csv"));
                Response.Redirect("~/Temp/AttendanceDump.csv");
            }
            else
            {
                AttendanceControl.AttendanceDump(new DateRange(DateRange.GetDateTimeFromString(StartDateInput.Text), DateRange.GetDateTimeFromString(EndDateInput.Text))).Save(Server.MapPath("~/Temp/AttendanceDump.csv"));
                Response.Redirect("~/Temp/AttendanceDump.csv");
            }
        }

        protected void GetHalfTermAttendance_Click(object sender, EventArgs e)
        {
            DateRange dr;
            using(WebhostEntities db = new WebhostEntities())
            {
                // Select a term.

                DateTime midtermDate;
                try
                {
                    midtermDate = DateRange.GetDateTimeFromString(MidTermDateInput.Text);
                }
                catch (InvalidCastException)
                {
                    ((Default)Page.Master).ShowError("Invalid Date Range Selected.",
                        String.Format("Could not parse date from '{0}' to a valid DateTime.", MidTermDateInput.Text));
                    return;
                }

                List<Term> terms = db.Terms.Where(t => t.StartDate < midtermDate && t.EndDate > midtermDate).ToList();

                if (terms.Count < 1) return;

                DateTime termdate = (BeforeOrAfter.SelectedIndex == 0) ? terms[0].StartDate : terms[0].EndDate;

                dr = new DateRange(midtermDate, termdate);
            }

            CSV quick = AttendanceControl.GetQuickOverview(dr);
            quick.Save(Server.MapPath("~/Temp/HalfTermAttendance.csv"));
            Response.Redirect("~/Temp/HalfTermAttendance.csv");

        }

        protected void GetAttendanceDump_Click(object sender, EventArgs e)
        {
            int StudentId;
            try
            {
                StudentId = Convert.ToInt32(StudentSelector.SelectedValue);
            }
            catch
            {
                ((Default)Page.Master).ShowError("No Student Selected", "You must select a Student to generate this report.");
                return;
            }

            DateRange range;
            try
            {
                range = new DateRange(DateRange.GetDateTimeFromString(StartDateInput.Text), DateRange.GetDateTimeFromString(EndDateInput.Text));
            }
            catch(InvalidCastException)
            {
                ((Default)Page.Master).ShowError("Invalid Date Range Selected.", 
                    String.Format("Could not parse dates from '{0}'~'{1}' to a valid Date Range.", StartDateInput.Text, EndDateInput.Text));
                return;
            }

            AttendanceControl.AttendanceDump(range, StudentId).Save(Server.MapPath("~/Temp/dump.csv"));
            Response.Redirect("~/Temp/dump.csv");
        }
    }
}