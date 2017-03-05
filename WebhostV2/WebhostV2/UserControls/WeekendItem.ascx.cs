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
    public partial class WeekendItem : LoggingUserControl
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
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.Weekends.Where(w => w.id == value).Count() > 0)
                    {
                        WeekendIdField.Value = Convert.ToString(value);
                    }
                }
            }
        }

        public int ActivityId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ActivityIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        WeekendActivity activity = db.WeekendActivities.Where(act => act.id == value).Single();
                        ActivityIdField.Value = Convert.ToString(value);
                        DaySelect.ClearSelection();
                        switch(activity.DateAndTime.DayOfWeek)
                        {
                            case DayOfWeek.Friday: DaySelect.SelectedValue = "Friday"; break;
                            case DayOfWeek.Saturday: DaySelect.SelectedValue = "Saturday"; break;
                            case DayOfWeek.Sunday: DaySelect.SelectedValue = "Sunday"; break;
                            default: DaySelect.SelectedValue = "Friday"; break;
                        }

                        ActivityName.Text = activity.Name;
                        AllDayCB.Checked = activity.DateAndTime.Hour == 0;
                        if(AllDayCB.Checked)
                        {
                            StartTimeSelector.Visible = false;
                            DurationCB.Checked = false;
                            DurationCB.Visible = false;
                            DurationSelector.Visible = false;
                        }
                        else
                        {
                            StartTimeSelector.Visible = true;
                            StartTimeSelector.SetTime(activity.DateAndTime);
                            if(activity.Duration > 0)
                            {
                                DurationCB.Checked = true;
                                DurationCB.Visible = true;
                                DurationSelector.Visible = true;
                                DurationSelector.SetTime(activity.DateAndTime.AddMinutes(activity.Duration));
                            }
                            else
                            {
                                DurationCB.Checked = false;
                                DurationCB.Visible = true;
                                DurationSelector.Visible = false;
                            }
                        }

                        FacultyGroupSelector1.AddFaculty(activity.Adults.Select(fac => fac.ID).ToList());
                        isSignupCB.Checked = activity.isSignup;
                        DescriptionInput.Text = activity.Description.Length > 0 ? activity.Description : "Description";
                        MaxSignupDL.ClearSelection();
                        MaxSignupDL.SelectedValue = Convert.ToString(activity.MaxSignups);
                    }
                }
            }
        }

        public bool Save()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                State.log.WriteLine("Save Weekend Activity Called.");
                int id = ActivityId;
                Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                WeekendActivity activity = new WeekendActivity();
                bool update = true;

                if (id == -1)
                {
                    State.log.WriteLine("Creating a New Activity.");
                    update = false;
                    id = db.WeekendActivities.Count() > 0 ? db.WeekendActivities.OrderBy(act => act.id).ToList().Last().id + 1 : 0;
                    activity.id = id;
                    activity.isMandatory = false;
                }
                else
                {
                    State.log.WriteLine("Updating existing activity.");
                    activity = db.WeekendActivities.Where(act => act.id == id).Single();
                }

                activity.Name = ActivityName.Text;
                activity.showStudents = isSignupCB.Checked;
                activity.isSignup = !activity.isMandatory;
                activity.IsOffCampus = isSignupCB.Checked && !MaxSignupDL.SelectedValue.Equals("0");
                activity.Description = DescriptionInput.Text.Equals("Description") ? "" : DescriptionInput.Text;

                activity.Adults.Clear();

                    
                foreach (int aid in FacultyGroupSelector1.GroupIds)
                {
                    Faculty fac = db.Faculties.Where(f => f.ID == aid).Single();
                    activity.Adults.Add(fac);
                }

                activity.WeekendIndex = WeekendId;
                DateTime date = weekend.StartDate.Date.AddDays(DaySelect.SelectedIndex);
                if (!AllDayCB.Checked)
                {
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
                }

                activity.DateAndTime = date;
                if (!AllDayCB.Checked && DurationCB.Checked)
                {
                    try
                    {
                        int hours = DurationSelector.Hour - StartTimeSelector.Hour;
                        int minutes = DurationSelector.Minute - StartTimeSelector.Minute;
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
                }
                else
                {
                    activity.Duration = 0;
                }

                if (!update)
                {
                    activity.GoogleCalendarEventId = "";

                    db.WeekendActivities.Add(activity);
                    State.log.WriteLine("Added New Activty to Database.");
                }

                db.SaveChanges();
                State.log.WriteLine("Saved Changes to Database.");
                return true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                FacultyGroupSelector1.ActiveOnly = true;
        }

        protected void DurationCB_CheckedChanged(object sender, EventArgs e)
        {
            DurationSelector.Visible = DurationCB.Checked;
            DurationCB.Text = DurationCB.Checked ? "" : "Add End Time";
        }

        protected void AllDayCB_CheckedChanged(object sender, EventArgs e)
        {
            StartTimeSelector.Visible = !AllDayCB.Checked;
            DurationCB.Visible = !AllDayCB.Checked;
            if (AllDayCB.Checked)
            {
                DurationCB.Checked = false;
                DurationCB.Text = "Add End Time";
            }
            DurationSelector.Visible = DurationCB.Checked;

            AllDayCB.Text = AllDayCB.Checked ? "All Day" : "";
            isSignupCB.Checked = !AllDayCB.Checked;
            isSignupCB_CheckedChanged(sender, e);
        }

        protected void isSignupCB_CheckedChanged(object sender, EventArgs e)
        {
            MaxSignupDL.Visible = isSignupCB.Checked;
            DescriptionInput.Visible = isSignupCB.Checked;
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
            using(WebhostEntities db = new WebhostEntities())
            {
                WeekendActivity activity = db.WeekendActivities.Where(act => act.id == ActivityId).Single();
                activity.IsDeleted = true;
                /*if(!activity.GoogleCalendarEventId.Equals(""))
                {
                    using(GoogleCalendarCall call = new GoogleCalendarCall())
                    {
                        String message = call.DeleteEvent(db.GoogleCalendars.Where(cal => cal.CalendarName.Equals("Weekend Activities")).Single().CalendarId, activity.GoogleCalendarEventId);
                        State.log.WriteLine("Deleted un-wanted calendar event with response:  {0}", message);
                        activity.GoogleCalendarEventId = "";
                    }
                }*/
                db.SaveChanges();
                SuccessLabel.Text = "Delete Successful";
                SuccessPanel.Visible = true;
            }
        }

        protected void ConfirmBtn_Click(object sender, EventArgs e)
        {
            SuccessPanel.Visible = false;
            Response.Redirect("~/DTL.aspx");
        }
    }
}