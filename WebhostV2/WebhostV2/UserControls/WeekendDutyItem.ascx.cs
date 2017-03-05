using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.GoogleAPI;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class WeekendDutyItem : LoggingUserControl
    {
        public int WeekendId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(WeekendIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.Weekends.Where(w => w.id == value).Count() > 0)
                    {
                        WeekendIdField.Value = Convert.ToString(value);
                    }
                }
            }
        }

        public int DutyItemId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(DutyIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        WeekendDuty activity = db.WeekendDuties.Where(act => act.id == value).Single();
                        DutyIdField.Value = Convert.ToString(value);
                        DaySelect.ClearSelection();
                        switch (activity.DateAndTime.DayOfWeek)
                        {
                            case DayOfWeek.Friday: DaySelect.SelectedValue = "Friday"; break;
                            case DayOfWeek.Saturday: DaySelect.SelectedValue = "Saturday"; break;
                            case DayOfWeek.Sunday: DaySelect.SelectedValue = "Sunday"; break;
                            default: DaySelect.SelectedValue = "Friday"; break;
                        }

                        DutyInput.Text = activity.Name;
                        StartTimeSelector.SetTime(activity.DateAndTime);
                        EndTimeSelector.SetTime(activity.DateAndTime.AddMinutes(activity.Duration));

                        DutyAssignmentSelector.AddFaculty(activity.DutyTeamMembers.Select(fac => fac.ID).ToList());
                        NotesInput.Text = activity.Description.Length > 0 ? activity.Description : "Notes";
                    }
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            StartTimeSelector.TimeUpdated += StartTimeSelector_TimeUpdated;
        }

        void StartTimeSelector_TimeUpdated(object sender, EventArgs e)
        {
            EndTimeSelector.SetTime(StartTimeSelector.Hour + 2, StartTimeSelector.Minute);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                DutyAssignmentSelector.ActiveOnly = true;
        }

        protected void AllTeamCB_CheckedChanged(object sender, EventArgs e)
        {
            DutyAssignmentSelector.Visible = !AllTeamCB.Checked;
            if(AllTeamCB.Checked)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                    DutyAssignmentSelector.Clear();
                    DutyAssignmentSelector.AddFaculty(weekend.DutyTeam.Members.Select(f => f.ID).ToList());
                }
            }
            else
            {
                DutyAssignmentSelector.Clear();
            }
        }

        public bool Save()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                State.log.WriteLine("Save Weekend Activity Called.");
                int id = DutyItemId;
                Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                WeekendDuty activity = new WeekendDuty();
                bool update = true;

                if (id == -1)
                {
                    State.log.WriteLine("Creating a New Activity.");
                    update = false;
                    id = db.WeekendDuties.Count() > 0 ? db.WeekendDuties.OrderBy(act => act.id).ToList().Last().id + 1 : 0;
                    activity.id = id;
                }
                else
                {
                    State.log.WriteLine("Updating existing activity.");
                    activity = db.WeekendDuties.Where(act => act.id == id).Single();
                }

                activity.Name = DutyInput.Text;
                activity.Description = NotesInput.Text.Equals("Notes") ? "" : NotesInput.Text;

                activity.DutyTeamMembers.Clear();

                foreach (int aid in DutyAssignmentSelector.GroupIds)
                {
                    Faculty fac = db.Faculties.Where(f => f.ID == aid).Single();
                    activity.DutyTeamMembers.Add(fac);
                }

                activity.WeekendId = WeekendId;
                DateTime date = weekend.StartDate.Date.AddDays(DaySelect.SelectedIndex);
                try
                {
                    date = date.AddHours(StartTimeSelector.Hour);
                    date = date.AddMinutes(StartTimeSelector.Minute);
                }
                catch (WebhostException we)
                {
                    State.log.WriteLine(we.Message);
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert_" + UniqueID, String.Format("alert('{0}');", we.Message), true);
                    return false;
                }
                
                activity.DateAndTime = date;
                try
                {
                    int hours = EndTimeSelector.Hour - StartTimeSelector.Hour;
                    int minutes = EndTimeSelector.Minute - StartTimeSelector.Minute;
                    if (minutes < 0)
                    {
                        hours--;
                        minutes = 60 + minutes;
                    }
                    activity.Duration = 60 * hours + minutes;
                }
                catch (WebhostException we)
                {
                    State.log.WriteLine(we.Message);
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert_" + UniqueID, String.Format("alert('{0}');", we.Message), true);
                    return false;
                }

                if (!update)
                {
                    activity.GoogleCalendarEventId = "";

                    db.WeekendDuties.Add(activity);
                    State.log.WriteLine("Added New Activty to Database.");
                }

                db.SaveChanges();
                State.log.WriteLine("Saved Changes to Database.");
                return true;
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                SuccessLabel.Text = "Save Successful.";
                SuccessPanel.Visible = true;
            }
        }

        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                WeekendDuty activity = db.WeekendDuties.Where(act => act.id == DutyItemId).Single();
                activity.IsDeleted = true;
                if(!activity.GoogleCalendarEventId.Equals(""))
                {
                    using(GoogleCalendarCall call = new GoogleCalendarCall())
                    {
                        String message = call.DeleteEvent(db.GoogleCalendars.Where(cal => cal.CalendarName.Equals("Weekend Duty")).Single().CalendarId, activity.GoogleCalendarEventId);
                        State.log.WriteLine("Deleted un-wanted calendar event with response:  {0}", message);
                        activity.GoogleCalendarEventId = "";
                    }
                }
                db.SaveChanges();
                SuccessLabel.Text = "Delete Successful";
                SuccessPanel.Visible = true;
            }
        }

        protected void ConfirmBtn_Click(object sender, EventArgs e)
        {
            SuccessPanel.Visible = false;
        }

    }
}