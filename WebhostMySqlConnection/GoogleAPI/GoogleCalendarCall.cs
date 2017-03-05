using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection.Web;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Util.Store;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Google.Apis.Services;

namespace WebhostMySQLConnection.GoogleAPI
{
    public class GoogleCalendarCall : GoogleAPICall
    {
        private CalendarService service;

        public GoogleCalendarCall() : base() 
        {
            SetCredential(new[] { CalendarService.Scope.Calendar }, "jason");

            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Dublin School Webhost Gmail"
            });

        }

        #region Events

        #region Event Participants

        /// <summary>
        /// Adds or Updates a participant in a Calendar event.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <param name="userName">Student.UserName or Faculty.UserName -- will be converted to email address by this method</param>
        /// <param name="status">Valid status are:  "needsAction", "declined", "tentative" and (default) "accepted"</param>
        /// <param name="optional">is this Optional or not?</param>
        /// <param name="comment">If there is a note that should be attached, put it here (e.g. Student is banned from going on a trip)</param>
        public void AddEventParticipant(String calendarId, String eventId, String userName, String status = "accepted", bool optional = false, String comment = "")
        {
            String email = String.Format("{0}@dublinschool.org", userName);

            Event evt = service.Events.Get(calendarId, eventId).Execute();

            if (evt.Attendees != null && evt.Attendees.Where(att => att.Email.Equals(email)).Count() > 0)
            {
                State.log.WriteLine("{0} is already an attendee for this event.", userName);
                WebhostEventLog.GoogleLog.LogWarning("{0} is already an attendee for {1} will update status.", userName, evt.Summary);
                EventAttendee attendee = evt.Attendees.Where(att => att.Email.Equals(email)).Single();
                evt.Attendees.Remove(attendee);
                attendee.ResponseStatus = status;
                evt.Attendees.Add(attendee);
            }
            else
            {
                EventAttendee attendee = new EventAttendee()
                {
                    Email = email,
                    ResponseStatus = status,
                    Optional = optional,
                    Comment = comment
                };
                if (evt.Attendees == null)
                {
                    evt.Attendees = new List<EventAttendee>() { attendee };
                }
                else
                {
                    evt.Attendees.Add(attendee);
                }

                WebhostEventLog.GoogleLog.LogWarning("Added {0} as an attendee for {1}.", userName, evt.Summary);
                
            }

            service.Events.Update(evt, calendarId, eventId).Execute();
        }

        /// <summary>
        /// Gets a List of Faculty.UserName and Student.UserName from the requested event's attendees.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <returns>List of Faculty.UserName and Student.UserName</returns>
        public List<String> GetEventParticipants(String calendarId, String eventId)
        {
            Event evt;
            try
            {
                evt = service.Events.Get(calendarId, eventId).Execute();
            }
            catch(Exception e)
            {
                throw new GoogleAPIException("Could not get the requested Event.", e);
            }

            if (evt.Attendees == null) return new List<String>();

            List<String> attendees = new List<string>();
            foreach(EventAttendee attendee in evt.Attendees)
            {
                attendees.Add(attendee.Email.Substring(0, attendee.Email.IndexOf('@')));
            }

            return attendees;
        }

        /// <summary>
        /// Remove an attendee from a calendar event.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <param name="userName"></param>
        public void RemoveParticipant(String calendarId, String eventId, String userName)
        {
            String email = String.Format("{0}@dublinschool.org", userName);

            Event evt = service.Events.Get(calendarId, eventId).Execute();

            if (evt.Attendees.Where(att => att.Email.Equals(email)).Count() > 0)
            {
                State.log.WriteLine("{0} is already an attendee for this event.");
                EventAttendee attendee = evt.Attendees.Where(att => att.Email.Equals(email)).Single();
                evt.Attendees.Remove(attendee);
                WebhostEventLog.GoogleLog.LogInformation("Removing {0} from {1}", userName, evt.Summary);
            }
            else
            {
                WebhostEventLog.GoogleLog.LogWarning("{0} is not listed as an attendee for {1}", userName, evt.Summary);
            }

            service.Events.Update(evt, calendarId, eventId).Execute();
        }

        #endregion

        /// <summary>
        /// Insert a new Calendar Event.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="title"></param>
        /// <param name="time"></param>
        /// <param name="duration"></param>
        /// <param name="Description"></param>
        /// <returns>Event Id for accessing</returns>
        public String PostEventToCalendar(string calendarId, String title, DateTime time, TimeSpan duration, String Description)
        {
            if (duration.TotalMinutes == 0) duration = TimeSpan.FromHours(2);
            else if (duration.TotalMinutes < 0) duration = TimeSpan.FromMinutes(30);
            Event evt = new Event()
            {
                Created = DateTime.Now,
                Description = Description,
                End = new EventDateTime() { DateTime = time.Add(duration) },
                Start = new EventDateTime() { DateTime = time },
                Summary = title,
                Status="confirmed"
            };
            
            evt = service.Events.Insert(evt, calendarId).Execute();

            WebhostEventLog.GoogleLog.LogInformation("Posted Calendar Event", typeof(Event), evt);

            return evt.Id;
        }

        /// <summary>
        /// update a calendar event
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="EventId"></param>
        /// <param name="title"></param>
        /// <param name="time"></param>
        /// <param name="duration"></param>
        /// <param name="Description"></param>
        /// <returns>Event Id</returns>
        public String UpdateEvent(String calendarId, string EventId, String title, DateTime time, TimeSpan duration, String Description)
        {
            Event evt;
            try
            {
                evt = service.Events.Get(calendarId, EventId).Execute();
            }
            catch(Exception e)
            {
                throw new GoogleAPIException("Could Not Get Event.  Has it been deleted?", e);
            }

            evt.Status = "confirmed";
            evt.Summary = title;
            evt.Start = new EventDateTime() { DateTime = time};
            evt.End = new EventDateTime() { DateTime = time.Add(duration)};
            evt.Description = Description;
           
            evt = service.Events.Update(evt, calendarId, EventId).Execute();

            WebhostEventLog.GoogleLog.LogInformation("Updated Calendar Event", typeof(Event), evt);

            return evt.Id;
        }

        /// <summary>
        /// Delete a Calendar Event.
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <returns>??? Message from Delete execution.</returns>
        public String DeleteEvent(String calendarId, String eventId)
        {
            String resp = service.Events.Delete(calendarId, eventId).Execute();
            WebhostEventLog.GoogleLog.LogInformation("Deleting Calendar Event:  {0} -> {1}{2}Response:  {3}", calendarId, eventId, Environment.NewLine, resp);
            return resp;
        }

        #endregion

        #region Pull From Google

        /// <summary>
        /// Pulls the Calendars owned by <em>me</em> (jason@dublinschool.org) because
        /// when we initialized the Google Calendars, I used my user (stupid).  Therefore all 
        /// the major school calendars are owned by my user and while the adminj@dublinschool.org 
        /// user has ownership permissions, the actual ownership cannot be passed...
        /// 
        /// Anyway--this information is then dumped into the WebhostMySQLConnection.GoogleCalendars table.
        /// 
        /// Faculty and Student default permissions for newly synced calendars must be set manually in the Database.
        /// </summary>
        public void GetCalendarsFromGoogle()
        {
            List<CalendarListEntry> cals = service.CalendarList.List().Execute().Items.ToList();
            using (WebhostEntities db = new WebhostEntities())
            {
                int newId = db.GoogleCalendars.Count() > 0 ? db.GoogleCalendars.OrderBy(c => c.id).Select(c => c.id).ToList().Last() + 1 : 0;

                foreach (CalendarListEntry entry in cals)
                {
                    State.log.WriteLine("({0}){1} - {2}", entry.AccessRole, entry.Summary, entry.Id);
                    if (entry.AccessRole.Equals("owner"))
                    {
                        String calendarId = entry.Id;
                        GoogleCalendar gcal;
                        bool exists = db.GoogleCalendars.Where(c => c.CalendarId.Equals(calendarId)).Count() > 0;
                        if (!exists)
                        {
                            gcal = new GoogleCalendar()
                            {
                                id = newId++,
                                CalendarId = calendarId,
                                FacultyPermission = "reader",
                                StudentPermission = "none",
                                CalendarName = entry.Summary
                            };

                            State.log.WriteLine("Syncing new calendar {0} to Webhost...id:  {1}", entry.Summary, entry.Id);
                            WebhostEventLog.GoogleLog.LogInformation("Syncing new calendar {0} to Webhost...id:  {1}", entry.Summary, entry.Id);
                        }
                        else
                        {
                            gcal = db.GoogleCalendars.Where(c => c.CalendarId.Equals(calendarId)).Single();
                            State.log.WriteLine("Updating calendar {0} on Webhost...id:  {1}", entry.Summary, entry.Id);
                            WebhostEventLog.GoogleLog.LogInformation("Updating calendar {0} on Webhost...id:  {1}", entry.Summary, entry.Id);
                        }

                        List<AclRule> managers = service.Acl.List(calendarId).Execute().Items.Where(r => r.Scope.Type.Equals("user") && (r.Role.Equals("owner"))).ToList();
                        foreach (AclRule rule in managers)
                        {
                            String userName = rule.Scope.Value.Substring(0, rule.Scope.Value.IndexOf('@'));
                            if (gcal.Owners.Where(f => f.UserName.Equals(userName)).Count() <= 0)
                            {
                                if (db.Faculties.Where(f => f.UserName.Equals(userName)).Count() > 0)
                                {
                                    gcal.Owners.Add(db.Faculties.Where(f => f.UserName.Equals(userName)).Single());
                                    State.log.WriteLine("Added {0} to the calendar owners list", rule.Scope.Value);
                                    WebhostEventLog.GoogleLog.LogInformation("Added {0} to the calendar owners list", rule.Scope.Value);
                                }
                                else
                                {
                                    State.log.WriteLine("{0} is already an owner of this calendar.", rule.Scope.Value);
                                    WebhostEventLog.GoogleLog.LogInformation("{0} is already an owner of this calendar.", rule.Scope.Value);
                                }
                            }
                        }

                        if (!exists)
                        {
                            db.GoogleCalendars.Add(gcal);
                        }
                    }
                }

                State.log.WriteLine("Finished Pulling.  You must set Faculty and Student default permissions manually.");
                db.SaveChanges();
            }
        }

        #endregion

        #region Modify Subscriptions

        /// <summary>
        /// Update the Calendar Subscriptions appropriate to a given User.
        /// </summary>
        /// <param name="id">pass either a Faculty.ID or Student.ID</param>
        /// <param name="isFaculty">Indicate if the id is Faculty or Student--will throw an exception if this is wrong</param>
        public void UpdateCalendarsForUser(int id, bool isFaculty)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                String email;
                if(isFaculty)
                {
                    email = String.Format("{0}@dublinschool.org", db.Faculties.Find(id).UserName);
                }
                else
                {
                    email = String.Format("{0}@dublinschool.org", db.Students.Find(id).UserName);
                }

                foreach(GoogleCalendar gcal in db.GoogleCalendars.ToList())
                {
                    String role = isFaculty ? gcal.FacultyPermission : gcal.StudentPermission;
                    if(gcal.Owners.Select(o => o.ID).ToList().Contains(id))
                    {
                        role = "owner";
                    }
                    
                    List<AclRule> rules = service.Acl.List(gcal.CalendarId).Execute().Items.Where(r => r.Scope.Type.Equals("user")).ToList();
                    if(rules.Where(rule => rule.Scope.Value.Equals(email)).Count() > 0)
                    {
                        if(role.Equals("none"))
                        {
                            State.log.WriteLine("{0} has access to calendar they should not:  {1}.  Removing Access.", email, gcal.CalendarName);
                            Console.WriteLine("{0} has access to calendar they should not:  {1}.  Removing Access.", email, gcal.CalendarName);
                            WebhostEventLog.GoogleLog.LogWarning("{0} has access to calendar they should not:  {1}.  Removing Access.", email, gcal.CalendarName);
                            foreach(AclRule rule in rules.Where(rule => rule.Scope.Value.Equals(email)))
                            {
                                String response = service.Acl.Delete(gcal.CalendarId, rule.Id).Execute();
                                State.log.WriteLine("Deleting Calendar ACL rule for {0} in {1}.  Response:  {2}", email, gcal.CalendarName, response);
                                Console.WriteLine("Deleting Calendar ACL rule for {0} in {1}.  Response:  {2}", email, gcal.CalendarName, response);
                                WebhostEventLog.GoogleLog.LogInformation("Deleting Calendar ACL rule for {0} in {1}.  Response:  {2}", email, gcal.CalendarName, response);
                            }
                        }
                        State.log.WriteLine("{0} already has access to {1}", email, gcal.CalendarName);
                        Console.WriteLine("OK - {0} already has access to {1}", email, gcal.CalendarName);
                        WebhostEventLog.GoogleLog.LogInformation("{0} already has access to {1}", email, gcal.CalendarName);
                    }
                    else if(!role.Equals("none"))
                    {
                        AclRule rule = new AclRule()
                        {
                            Role = role,
                            Scope = new AclRule.ScopeData() { Type = "user", Value = email }
                        };

                        service.Acl.Insert(rule, gcal.CalendarId).Execute();
                        Console.WriteLine("Added {0} as {1} to {2}", email, role, gcal.CalendarName);
                        WebhostEventLog.GoogleLog.LogInformation("Added {0} as {1} to {2}", email, role, gcal.CalendarName);
                    }
                    else // role none.
                    {
                        Console.WriteLine("Skipping Calendar with no Access.");
                        WebhostEventLog.GoogleLog.LogInformation("skipping calendar with no access.");
                    }
                }
            }            
        }

        public void RemoveAllSharedCalendars(List<String> emails)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach (GoogleCalendar gcal in db.GoogleCalendars.ToList())
                {
                    List<AclRule> rules = service.Acl.List(gcal.CalendarId).Execute().Items.Where(r => r.Scope.Type.Equals("user")).ToList();
                    foreach (AclRule rule in rules)
                    { 
                        if(emails.Contains(rule.Scope.Value))
                        {
                            Console.WriteLine("Removing {0} from {1}.", rule.Scope.Value, gcal.CalendarName);
                            String result = service.Acl.Delete(gcal.CalendarId, rule.Id).Execute();
                            if(!result.Equals(""))
                            {
                                Console.WriteLine("Result:  {0}", result);
                            }

                            WebhostEventLog.GoogleLog.LogInformation("Deleted {0} from {1}.", rule.Scope.Value, gcal.CalendarName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates Calendar Permissions.
        /// </summary>
        /// <param name="doRemovals"></param>
        public void UpdateCalendars(bool doRemovals = true)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<String> alreadyMembers = new List<string>();

                List<String> activeStudentEmails = new List<string>();
                foreach (Student student in db.Students.Where(s => s.isActive).ToList())
                {
                    activeStudentEmails.Add(String.Format("{0}@dublinschool.org", student.UserName));
                }

                List<String> activeFacultyEmails = new List<string>();
                foreach (Faculty faculty in db.Faculties.Where(f => f.isActive).ToList())
                {
                    activeFacultyEmails.Add(String.Format("{0}@dublinschool.org", faculty.UserName));
                }

                List<String> inactiveUsers = new List<string>();
                foreach (Faculty faculty in db.Faculties.Where(f => !f.isActive).ToList())
                    inactiveUsers.Add(String.Format("{0}@dublinschool.org", faculty.UserName));

                foreach (Student student in db.Students.Where(f => !f.isActive).ToList())
                    inactiveUsers.Add(String.Format("{0}@dublinschool.org", student.UserName));

                foreach (GoogleCalendar gcal in db.GoogleCalendars.ToList())
                {
                    List<AclRule> rules = service.Acl.List(gcal.CalendarId).Execute().Items.Where(r => r.Scope.Type.Equals("user")).ToList();
                    foreach (AclRule rule in rules)
                    {
                        String userName = rule.Scope.Value.Substring(0, rule.Scope.Value.IndexOf('@'));
                        Regex studentEmail = new Regex("[a-z]+_[a-z]+");
                        if (doRemovals && 
                            ((gcal.StudentPermission.Equals("none")
                                            && studentEmail.IsMatch(userName) && !gcal.StudentCalendarSpecialPermissions.Select(p => p.Student.UserName).ToList().Contains(userName)) 
                            || 
                            inactiveUsers.Contains(rule.Scope.Value)
                            ))
                        {
                            try
                            {
                                service.Acl.Delete(gcal.CalendarId, rule.Id).Execute();
                                WebhostEventLog.GoogleLog.LogInformation("Removed {0} from {1}.", userName, gcal.CalendarName);
                            }
                            catch (Exception e)
                            {
                                WebhostEventLog.GoogleLog.LogError("Could not remove rule {0} for {3} on {4}.{1}{2}", rule.Scope.Value, Environment.NewLine, e.Message, userName, gcal.CalendarName);
                                State.log.WriteLine("Could not remove rule for {0}", rule.Scope.Value);
                                State.log.WriteLine(e.Message);
                            }
                        }

                        if (doRemovals && rule.Scope.Type.Equals("user") && !activeFacultyEmails.Contains(rule.Scope.Value) && !activeStudentEmails.Contains(rule.Scope.Value))
                        {
                            try
                            {
                                service.Acl.Delete(gcal.CalendarId, rule.Id).Execute();
                                WebhostEventLog.GoogleLog.LogInformation("Removed {0} from {1}.", userName, gcal.CalendarName);
                            }
                            catch (Exception e)
                            {
                                State.log.WriteLine("Could not remove rule for {0}", rule.Scope.Value);
                                WebhostEventLog.GoogleLog.LogError("Could not remove rule {0} for {3} on {4}.{1}{2}", rule.Scope.Value, Environment.NewLine, e.Message, userName, gcal.CalendarName);
                                State.log.WriteLine(e.Message);
                            }
                        }
                        else if (activeStudentEmails.Contains(rule.Scope.Value) || activeFacultyEmails.Contains(rule.Scope.Value))
                        {
                            WebhostEventLog.GoogleLog.LogInformation("{0} is already a member of {1}.", rule.Scope.Value, gcal.CalendarName);
                            alreadyMembers.Add(rule.Scope.Value);
                        }
                    }

                    foreach (String email in activeStudentEmails.Concat(activeFacultyEmails))
                    {
                        if (!alreadyMembers.Contains(email))
                        {
                            String userName = email.Substring(0, email.IndexOf('@'));
                            String role = "reader";

                            if (gcal.Owners.Where(f => f.UserName.Equals(userName)).Count() > 0)
                                role = "owner";
                            else if (gcal.StudentCalendarSpecialPermissions.Where(csp => csp.Student.UserName.Equals(userName)).Count() > 0)
                            {
                                StudentCalendarSpecialPermission perm = gcal.StudentCalendarSpecialPermissions.Where(csp => csp.Student.UserName.Equals(userName)).Single();
                                role = perm.Role;
                            }
                            else if (email.Contains('_'))
                            {
                                role = gcal.StudentPermission;
                            }
                            else
                            {
                                role = gcal.FacultyPermission;
                            }

                            if (role.Equals("none"))
                            {
                                State.log.WriteLine("Do not need to create a 'none' rule for {0}", email);
                                continue;
                            }
                            AclRule rule = new AclRule()
                            {
                                Role = role,
                                Scope = new AclRule.ScopeData() { Type = "user", Value = email }
                            };
                            WebhostEventLog.GoogleLog.LogInformation("Added {0} to access {1} with role {2}.", email, gcal.CalendarName, role);
                            service.Acl.Insert(rule, gcal.CalendarId).Execute();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add user to Calendar!
        /// </summary>
        /// <param name="userName">Webhost User Name</param>
        /// <param name="calendarId">Webhost Calendar Id</param>
        /// <param name="role">Role in the Calendar, "reader" is default, "writer", "none" and "owner" are others.</param>
        public void AddUserToCalendar(String userName, String calendarId, String role = "reader")
        {
            AclRule rule = new AclRule()
            {
                Role = role,
                Scope = new AclRule.ScopeData() { Type = "user", Value = String.Format("{0}@dublinschool.org", userName) }
            };

            WebhostEventLog.GoogleLog.LogInformation("Added {0} to access {1} with role {2}.", userName, calendarId, role);
            service.Acl.Insert(rule, calendarId);
        }
        #endregion
    }
}
