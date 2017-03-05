using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using System.Text.RegularExpressions;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.GoogleAPI;

namespace WebhostV2.UserControls
{
    public partial class WeekendBuilder : LoggingUserControl
    {
        //protected ScriptResourceMapping jquery = new ScriptResourceMapping();

        public int DutyTeamID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(DutyTeamIDField.Value);
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
                    DutyTeam team = db.DutyTeams.Find(value);
                    if(team == null)
                    {
                        LogError("Invalid DutyTeam.id {0}", value);
                        BuilderPanel.Visible = false;
                        return;
                    }
                    DutyTeamIDField.Value = Convert.ToString(value);
                    WeekendLabel.Text = team.Name + " Weekend!";                    
                }
            }
        }

        public int WeekendID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(IDField.Value);
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
                        IDField.Value = Convert.ToString(value);
                        WeekendItem1.WeekendId = value;
                        WeekendDutyItem1.WeekendId = value;
                    }
                }

                LoadWeekend();
            }
        }

        protected void LoadWeekend()
        {
            NewWeekendPnl.Visible = false;
            BuilderPanel.Visible = true;
            using(WebhostEntities db = new WebhostEntities())
            {
                Weekend weekend = db.Weekends.Find(WeekendID);

                //StartDate.Text = weekend.StartDate.ToShortDateString();
                DutyTeamID = weekend.DutyTeamIndex;

                // Load Details!
                WeekendLabel.Text = weekend.DutyTeam.Name + " Weekend!";
                ActivitiesTable.Rows.Clear();
                ActivitiesTable.Rows.Add(DutyScheduleRow.HeaderRow);
                List<DutyScheduleRow> rows = new List<DutyScheduleRow>();
                foreach(WeekendActivity activity in weekend.WeekendActivities.Where(act => !act.IsDeleted).ToList())
                {
                    rows.Add(new DutyScheduleRow(activity.id));
                }

                foreach(WeekendDuty duty in weekend.WeekendDuties.Where(dut => !dut.IsDeleted).ToList())
                {
                    rows.Add(new DutyScheduleRow(duty.id, true));
                }

                rows.Sort();

                ActivitiesTable.Rows.AddRange(rows.ToArray());

                if (!Page.IsPostBack)
                {
                    DeleteActivityDDL.DataSource = DutyListItem.GetDataSource(weekend.WeekendActivities.Where(act => !act.IsDeleted).Select(act => act.id).ToList(), 
                                                                                    weekend.WeekendDuties.Where(dut => !dut.IsDeleted).Select(dut => dut.id).ToList());
                    DeleteActivityDDL.DataTextField = "Text";
                    DeleteActivityDDL.DataValueField = "Value";
                    DeleteActivityDDL.DataBind();
                }

                NotesInput.Text = weekend.Notes;
                LogInformation("Loaded {0}", WeekendLabel.Text);
            }
        }

        public DateRange WeekendRange
        {
            get
            {
                Regex dateEx = new Regex("^(0?[1-9]|1[0-2])/(0?[1-9]|[1-2][0-9]|3[0-1])/20[0-9]{2}$");

                if(dateEx.IsMatch(StartDate.Text))
                {
                    DateTime friday = DateRange.GetDateTimeFromString(StartDate.Text);

                    return new DateRange(friday, friday.AddDays(2));
                }

                return new DateRange(DateRange.ThisFriday, DateRange.ThisFriday.AddDays(2));
            }
        }

        public bool LoadWeekendFromDate()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Weekend weekend in db.Weekends)
                {
                    if(WeekendRange.Intersects(weekend.StartDate, weekend.EndDate))
                    {
                        LogInformation("Loading Weekend.id {0} based on selected date.", weekend.id);
                        WeekendID = weekend.id;
                        return true;
                    }
                }

                LogWarning("No saved weekend matches the selected dates {0}.", WeekendRange);
                BuilderPanel.Visible = false;
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    int year = DateRange.GetCurrentAcademicYear();
                    TemplateDDL.DataSource = WeekendListItem.GetDataSource(db.Weekends.Where(w => w.DutyTeam.AcademicYearID == year).OrderByDescending(w => w.StartDate).Select(w => w.id).ToList());
                    TemplateDDL.DataTextField = "Text";
                    TemplateDDL.DataValueField = "WeekendId";
                    TemplateDDL.DataBind();
                }

                BuilderPanel.Visible = WeekendID != -1;
            }
        }
        
        protected void StartBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if(!LoadWeekendFromDate())
                {
                    // Initialize a new weekend!
                    int id = db.Weekends.Count() > 0 ? db.Weekends.OrderBy(w => w.id).ToList().Last().id + 1 : 0;

                    Weekend weekend = new Weekend()
                    {
                        id = id,
                        DutyTeamIndex = DutyTeamID,
                        StartDate = WeekendRange.Start,
                        EndDate = WeekendRange.End,
                        Notes = ""
                    };

                    db.Weekends.Add(weekend);
                    db.SaveChanges();
                    WeekendID = id;
                }
                LogInformation("Created a new Weekend from scratch.");
                LoadWeekend();
            }
        }

        protected void NotesCB_CheckedChanged(object sender, EventArgs e)
        {
            NotesInput.Visible = NotesCB.Checked;
            SaveNotes.Visible = NotesCB.Checked && WeekendID != -1;
        }

        protected void SaveNotes_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Weekend weekend = db.Weekends.Find(WeekendID);
                weekend.Notes = NotesInput.Text;
                db.SaveChanges();
                LogInformation("Updated Weekend Notes:  {0}", weekend.Notes);
            }
            LoadWeekend();
        }

        protected void DeleteActivityBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int id = -1;
                try
                {
                    id = Convert.ToInt32(DeleteActivityDDL.SelectedValue.Substring(1));
                }
                catch
                {
                    State.log.WriteLine("Invalid Activity '{0}' Cannot Delete.", DeleteActivityDDL.SelectedValue);
                    LogError("Invalid Activity '{0}' Cannot Delete.", DeleteActivityDDL.SelectedValue);
                    return;
                }

                if (DeleteActivityDDL.SelectedValue[0] == 'A')
                {
                    WeekendActivity activity = db.WeekendActivities.Find(id);
                    activity.IsDeleted = true;
                    LogInformation("Deleted Weekend Activity {0}", activity.Name);
                    if (!activity.GoogleCalendarEventId.Equals(""))
                    {
                        using (GoogleCalendarCall call = new GoogleCalendarCall())
                        {
                            String message = call.DeleteEvent(db.GoogleCalendars.Where(cal => cal.CalendarName.Equals("Weekend Activities")).Single().CalendarId, activity.GoogleCalendarEventId);
                            State.log.WriteLine("Deleted un-wanted calendar event with response:  {0}", message);
                            activity.GoogleCalendarEventId = "";
                        }
                    }
                }
                else
                {
                    WeekendDuty duty = db.WeekendDuties.Find(id);
                    duty.IsDeleted = true;

                    if(!duty.GoogleCalendarEventId.Equals(""))
                    {
                        using (GoogleCalendarCall call = new GoogleCalendarCall())
                        {
                            String message = call.DeleteEvent(db.GoogleCalendars.Where(cal => cal.CalendarName.Equals("Weekend Duty")).Single().CalendarId, duty.GoogleCalendarEventId);
                            State.log.WriteLine("Deleted un-wanted calendar event with response:  {0}", message);
                            duty.GoogleCalendarEventId = "";
                        }
                    }

                    LogInformation("Deleted Weekend Duty {0}", duty.Name);
                }
                db.SaveChanges();
            }
            LoadWeekend();
        }
        
        protected void UpdatePanel1_Load(object sender, EventArgs e)
        {
            DateTime prev = Session["friday"] == null ? new DateTime() : (DateTime)Session["friday"];
            DateTime friday = new DateTime();
            try
            {
                friday = DateRange.FridayOf(DateRange.GetDateTimeFromString(StartDate.Text));
            }
            catch
            {
                friday = DateRange.ThisFriday;
            }

            if (!prev.Equals(friday))
            {
                StartDate.Text = friday.ToShortDateString();
                StartDate_CalendarExtender.SelectedDate = friday;

                Session["friday"] = friday;
            }

            NewWeekendPnl.Visible = !LoadWeekendFromDate();
            BuilderPanel.Visible = !NewWeekendPnl.Visible;
        }

        protected void LoadTemplate_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (!LoadWeekendFromDate())
                {
                    // Initialize a new weekend!
                    int id = db.Weekends.Count() > 0 ? db.Weekends.OrderBy(w => w.id).ToList().Last().id + 1 : 0;

                    Weekend weekend = new Weekend()
                    {
                        id = id,
                        DutyTeamIndex = DutyTeamID,
                        StartDate = WeekendRange.Start,
                        EndDate = WeekendRange.End,
                        Notes = ""
                    };

                    // Get the Weekend to Copy!
                    int otherId = Convert.ToInt32(TemplateDDL.SelectedValue);
                    LogInformation("Copying Weekend.id {0} to the current weekend.", otherId);
                    Weekend other = db.Weekends.Where(w => w.id == otherId).Single();

                    //Copy Activities
                    int actId = db.WeekendActivities.OrderBy(act => act.id).ToList().Last().id;
                    int dutyId = db.WeekendDuties.Count() > 0 ? db.WeekendDuties.OrderBy(act => act.id).ToList().Last().id : 0;
                    foreach(WeekendActivity activity in other.WeekendActivities.Where(act => !act.IsDeleted))
                    {
                        // Adjust date and time.
                        TimeSpan diff = activity.DateAndTime - activity.Weekend.StartDate;

                        WeekendActivity newActivity = new WeekendActivity()
                        {
                            id = ++actId,
                            Name = activity.Name,
                            isMandatory = activity.isMandatory,
                            IsDeleted = false,
                            IsOffCampus = activity.IsOffCampus,
                            MaxSignups = activity.MaxSignups,
                            DateAndTime = weekend.StartDate + diff,
                            showStudents = activity.showStudents,
                            WeekendIndex = id,
                            isSignup = activity.isSignup,
                            Duration = activity.Duration,
                            Description = activity.Description,
                            GoogleCalendarEventId = ""
                        };

                        db.WeekendActivities.Add(newActivity);                        
                    }

                    foreach(WeekendDuty duty in other.WeekendDuties.Where(duty => !duty.IsDeleted))
                    {
                        // Adjust date and time.
                        TimeSpan diff = duty.DateAndTime - duty.Weekend.StartDate;

                        WeekendDuty newDuty = new WeekendDuty()
                        {
                            id = ++dutyId,
                            Name = duty.Name,
                            DateAndTime = weekend.StartDate + diff,
                            WeekendId = id,
                            Duration = duty.Duration,
                            Description = duty.Description,
                            GoogleCalendarEventId = "",
                            IsDeleted = false
                        };

                        /*if(duty.DutyTeamMembers.Count > 5)
                        {
                            foreach(Faculty member in weekend.DutyTeam.Members.ToList())
                            {
                                newDuty.DutyTeamMembers.Add(member);
                            }
                        }*/

                        db.WeekendDuties.Add(newDuty);
                    }

                    db.Weekends.Add(weekend);
                    db.SaveChanges();
                    WeekendID = id;
                }

                LoadWeekend();
            }
        }

        protected void EditBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DeleteActivityDDL.SelectedValue[0] == 'A')
                {
                    WeekendItem1.ActivityId = Convert.ToInt32(DeleteActivityDDL.SelectedValue.Substring(1));
                    TabContainer1.ActiveTabIndex = 0;
                    LogInformation("Loading Weekend Activity Editor for id {0}", WeekendItem1.ActivityId);
                }
                else
                {
                    WeekendDutyItem1.DutyItemId = Convert.ToInt32(DeleteActivityDDL.SelectedValue.Substring(1));
                    TabContainer1.ActiveTabIndex = 1;
                    LogInformation("Loading Weekend Duty Editor for id {0}", WeekendDutyItem1.DutyItemId);
                }
            }
            catch
            {
                State.log.WriteLine("No Activity Selected!");
                LogError("User must select an activity before Editing.");
            }
        }

        protected void PublishCalendarBtn_Click(object sender, EventArgs e)
        {
            try
            {
                WeekendControl.PublishWeekendScheduleToGoogleCalendars(WeekendID, State.log);
                SuccessLabel.Text = "Successfully Posted to Google Calendar";
            }
            catch (Exception er)
            {
                SuccessLabel.Text = "There was a Problem, I'm emailing Jason now!";
                String message = er.Message;
                while(er.InnerException != null)
                {
                    er = er.InnerException;
                    message += Environment.NewLine + er.Message;
                }
                MailControler.MailToWebmaster("Problem Publishing Weekend Calendar", message, ((BasePage)Page).user);
            }

            SuccessPanel.Visible = true;
        }

        protected void OKBtn_Click(object sender, EventArgs e)
        {
            SuccessPanel.Visible = false;
        }
    }
}