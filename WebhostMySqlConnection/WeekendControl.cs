using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection.GoogleAPI;

namespace WebhostMySQLConnection
{
    public class WeekendControl
    {
        /// <summary>
        /// Gets a given WeekendActivity, looks for a WeekendDuty Detention and then 
        /// compares to see if their time-frames overlap.
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static bool ActivityConflictsWithDetention(int activityId, bool twoHour = true)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                WeekendActivity activity = db.WeekendActivities.Where(act => act.id == activityId).Single();

                List<WeekendDuty> detentions = activity.Weekend.WeekendDuties.Where(duty => duty.Name.Contains("Detention")).ToList();

                if (detentions.Count <= 0) return false;  // No Detention this weekend?

                DateRange activityRange = new DateRange(activity.DateAndTime, activity.DateAndTime.AddMinutes(activity.Duration == 0 ? 120 : activity.Duration));
                foreach(WeekendDuty detention in detentions)
                {
                    DateRange detentionRange = new DateRange(detention.DateAndTime, detention.DateAndTime.AddMinutes(twoHour?120:60));

                    if (activityRange.Intersects(detentionRange)) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Synchronize all student signups to the Google Calendar.
        /// </summary>
        /// <param name="WeekendId"></param>
        /// <param name="log"></param>
        public static void SyncronizeStudentSignups(int WeekendId, StreamWriter log = null)
        {
            if (log == null)
            {
                log = new StreamWriter(Console.OpenStandardOutput());
                log.AutoFlush = true;
                log.WriteLine("Hello Console!");
            }

            using(WebhostEntities db = new WebhostEntities())
            {
                using(GoogleCalendarCall gcal = new GoogleCalendarCall())
                {
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                    log.WriteLine("Syncing Student Signups for {0} Weekend {1}", weekend.DutyTeam.Name, weekend.StartDate.ToShortDateString());
                    String activitiesCalendarId = db.GoogleCalendars.Where(c => c.CalendarName.Equals("Weekend Activities")).Single().CalendarId;
                    foreach(WeekendActivity activity in weekend.WeekendActivities.ToList())
                    {
                        log.WriteLine("Updating Student Signups for {0}, {1} {2}", activity.Name, activity.DateAndTime.ToLongDateString(), activity.DateAndTime.ToShortTimeString());
                        List<String> students = gcal.GetEventParticipants(activitiesCalendarId, activity.GoogleCalendarEventId).Where(un => un.Contains("_")).ToList();

                        foreach(StudentSignup signup in activity.StudentSignups.ToList())
                        {
                            if(students.Contains(signup.Student.UserName))
                            {
                                log.WriteLine("{0} {1} is already on the calendar.", signup.Student.FirstName, signup.Student.LastName);
                                students.Remove(signup.Student.UserName);
                            }
                            String status = signup.IsBanned?"declined":"accepted";
                            log.WriteLine("Status:  {0}", status);
                            gcal.AddEventParticipant(activitiesCalendarId, activity.GoogleCalendarEventId, signup.Student.UserName, status, false, signup.IsBanned ? "Not Allowed" : "");
                        }

                        foreach(String toRemove in students)
                        {
                            gcal.RemoveParticipant(activitiesCalendarId, activity.GoogleCalendarEventId, toRemove);
                            log.WriteLine("Removed {0} from Event.", toRemove);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Publish all the Weekend Items to the Weekend Duty and Weekend Activities calendars.
        /// </summary>
        /// <param name="WeekendId"></param>
        /// <param name="log">Pass your favorite Log object here in the Web context, or leave it blank to use the Console Standard output.</param>
        public static void PublishWeekendScheduleToGoogleCalendars(int WeekendId, StreamWriter log = null)
        {
            if(log == null)
            {
                log = new StreamWriter(Console.OpenStandardOutput());
                log.AutoFlush = true;
                log.WriteLine("Hello Console!");
            }
            using(WebhostEntities db = new WebhostEntities())
            {
                using(GoogleCalendarCall gcal = new GoogleCalendarCall())
                {
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                    log.WriteLine("Publishing {0} Weekend {1}", weekend.DutyTeam.Name, weekend.StartDate.ToShortDateString());
                    String dutyCalendarId = db.GoogleCalendars.Where(c => c.CalendarName.Equals("Weekend Duty")).Single().CalendarId;
                    String activitiesCalendarId = db.GoogleCalendars.Where(c => c.CalendarName.Equals("Weekend Activities")).Single().CalendarId;

                    foreach(WeekendDuty duty in weekend.WeekendDuties.ToList())
                    {
                        if(duty.GoogleCalendarEventId.Equals(""))
                        {
                            log.WriteLine("Creating New Event for {0}, {1} {2}", duty.Name, duty.DateAndTime.ToLongDateString(), duty.DateAndTime.ToShortTimeString());
                            duty.GoogleCalendarEventId = gcal.PostEventToCalendar(dutyCalendarId, duty.Name, duty.DateAndTime, TimeSpan.FromMinutes(duty.Duration), duty.Description);
                            log.WriteLine("Got EventId:  {0}", duty.GoogleCalendarEventId);
                        }
                        else
                        {
                            log.WriteLine("Updating EventId: {0}", duty.GoogleCalendarEventId);
                            try
                            {
                                gcal.UpdateEvent(dutyCalendarId, duty.GoogleCalendarEventId, duty.Name, duty.DateAndTime, TimeSpan.FromMinutes(duty.Duration), duty.Description);
                            }
                            catch(GoogleAPICall.GoogleAPIException gae)
                            {
                                log.WriteLine(gae.Message);
                                log.WriteLine(gae.InnerException.Message);
                                log.WriteLine("Creating New Event for {0}, {1} {2}", duty.Name, duty.DateAndTime.ToLongDateString(), duty.DateAndTime.ToShortTimeString());
                                duty.GoogleCalendarEventId = gcal.PostEventToCalendar(dutyCalendarId, duty.Name, duty.DateAndTime, TimeSpan.FromMinutes(duty.Duration), duty.Description);
                                log.WriteLine("Got EventId:  {0}", duty.GoogleCalendarEventId);
                            }
                        }

                        List<String> participants = gcal.GetEventParticipants(dutyCalendarId, duty.GoogleCalendarEventId);
                        foreach(Faculty adult in duty.DutyTeamMembers.ToList())
                        {
                            if(participants.Contains(adult.UserName))
                            {
                                log.WriteLine("{0} {1} is already assigned to this Event.", adult.FirstName, adult.LastName);
                                participants.Remove(adult.UserName);
                            }
                            else
                            {
                                gcal.AddEventParticipant(dutyCalendarId, duty.GoogleCalendarEventId, adult.UserName);
                                log.WriteLine("Added {0} {1} to this event.", adult.FirstName, adult.LastName);
                            }
                        }

                        foreach(String toRemove in participants)
                        {
                            gcal.RemoveParticipant(dutyCalendarId, duty.GoogleCalendarEventId, toRemove);
                            log.WriteLine("Removed no longer assigned adult {0}", toRemove);
                        }
                    }

                    foreach (WeekendActivity activity in weekend.WeekendActivities.ToList())
                    {
                        if (activity.GoogleCalendarEventId.Equals(""))
                        {
                            log.WriteLine("Creating New Event for {0}, {1} {2}", activity.Name, activity.DateAndTime.ToLongDateString(), activity.DateAndTime.ToShortTimeString());
                            activity.GoogleCalendarEventId = gcal.PostEventToCalendar(activitiesCalendarId, activity.Name, activity.DateAndTime, TimeSpan.FromMinutes(activity.Duration), activity.Description);
                            log.WriteLine("Got EventId:  {0}", activity.GoogleCalendarEventId);
                        }
                        else
                        {
                            log.WriteLine("Updating EventId: {0}", activity.GoogleCalendarEventId);
                            try
                            {
                                gcal.UpdateEvent(activitiesCalendarId, activity.GoogleCalendarEventId, activity.Name, activity.DateAndTime, TimeSpan.FromMinutes(activity.Duration), activity.Description);
                            }
                            catch(GoogleAPICall.GoogleAPIException gae)
                            {
                                log.WriteLine(gae.Message);
                                log.WriteLine(gae.InnerException.Message);
                                log.WriteLine("Creating New Event for {0}, {1} {2}", activity.Name, activity.DateAndTime.ToLongDateString(), activity.DateAndTime.ToShortTimeString());
                                activity.GoogleCalendarEventId = gcal.PostEventToCalendar(activitiesCalendarId, activity.Name, activity.DateAndTime, TimeSpan.FromMinutes(activity.Duration), activity.Description);
                                log.WriteLine("Got EventId:  {0}", activity.GoogleCalendarEventId);
                            }
                        }

                        List<String> participants = gcal.GetEventParticipants(activitiesCalendarId, activity.GoogleCalendarEventId).Where(un => !un.Contains("_")).ToList();
                        foreach (Faculty adult in activity.Adults.ToList())
                        {
                            if (participants.Contains(adult.UserName))
                            {
                                log.WriteLine("{0} {1} is already assigned to this Event.", adult.FirstName, adult.LastName);
                                participants.Remove(adult.UserName);
                            }
                            else
                            {
                                gcal.AddEventParticipant(activitiesCalendarId, activity.GoogleCalendarEventId, adult.UserName);
                                log.WriteLine("Added {0} {1} to this event.", adult.FirstName, adult.LastName);
                            }
                        }

                        foreach (String toRemove in participants)
                        {
                            gcal.RemoveParticipant(activitiesCalendarId, activity.GoogleCalendarEventId, toRemove);
                            log.WriteLine("Removed no longer assigned adult {0}", toRemove);
                        }
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
