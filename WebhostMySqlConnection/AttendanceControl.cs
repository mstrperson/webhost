using System;
using System.Transactions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebhostMySQLConnection.SchoologyAPI;
using WebhostMySQLConnection.Web;

namespace WebhostMySQLConnection
{
    public class AttendanceControl
    {
        protected static readonly bool EmailEnabled =true;

        public struct Detention
        {
            public int studentId;
            public int lates;
            public int cuts;
            public List<int> markingIDs;
            public bool twoHour
            {
                get
                {
                    return (lates / 4) + cuts > 1;
                }
            }
        }

        enum AttendanceStatus
        {
            NotTaken = -1,
            NoMeeting = 0,
            Completed = 1
        }

        class CompData
        {
            public int SectionId;
            public Dictionary<DateTime, AttendanceStatus> DailyStats;


        }

        public static CSV ComplianceReport(DateRange thisWeek = null)
        {
            if(thisWeek == null)
            {
                thisWeek = DateRange.ThisAttendanceWeek;
            }
            CSV csv = new CSV(String.Format("Attendance Compliance Report for {0}", thisWeek).Remove(','));

            int termId = DateRange.GetCurrentOrLastTerm();

            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Find(termId);

                Dictionary<Faculty, List<CompData>> data = new Dictionary<Faculty, List<CompData>>();
                foreach(Section section in term.Sections.ToList())
                {
                    CompData compData = new CompData() { SectionId = section.id, DailyStats = new Dictionary<DateTime,AttendanceStatus>() };
                    foreach(DateTime date in thisWeek.ToList())
                    {
                        if (date.DayOfWeek.Equals(DayOfWeek.Saturday) || date.DayOfWeek.Equals(DayOfWeek.Sunday)) continue;
                        bool meetsThisDay = false;
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday: meetsThisDay = section.Block.MeetsMonday; break;
                            case DayOfWeek.Tuesday: meetsThisDay = section.Block.MeetsTuesday; break;
                            case DayOfWeek.Wednesday:
                                if (!section.Block.MeetsWednesday || section.Course.Name.Contains("Tutorial")) meetsThisDay = false;
                                else if (section.Block.IsSpecial || db.WednesdaySchedules.Where(w => w.Day.Equals(date)).Count() <= 0) meetsThisDay = section.Block.MeetsWednesday;
                                else
                                {
                                    meetsThisDay = DateRange.BlockOrderByDayOfWeek(date).Contains(section.Block.Name[0]);
                                }
                                break;
                            case DayOfWeek.Thursday: meetsThisDay = section.Block.MeetsThursday; break;
                            case DayOfWeek.Friday: meetsThisDay = section.Block.MeetsFriday; break;
                            default: break;
                        }

                        AttendanceStatus attendanceMarking = meetsThisDay ? AttendanceStatus.Completed : AttendanceStatus.NoMeeting;
                        //String note = "No Data";

                        foreach(Student student in section.Students.ToList())
                        {
                            if(section.AttendanceMarkings.Where(at => at.AttendanceDate.Equals(date) && at.StudentID.Equals(student.ID)).Count() <= 0)
                            {
                                attendanceMarking = AttendanceStatus.NotTaken;
                                break;
                            }
                        }

                        compData.DailyStats.Add(date, attendanceMarking);
                    }

                    foreach(Faculty teacher in section.Teachers.ToList())
                    {
                        if(data.ContainsKey(teacher))
                        {
                            data[teacher].Add(compData);
                        }
                        else
                        {
                            data.Add(teacher, new List<CompData>() { compData });
                        }
                    }

                }

                foreach (Faculty teacher in data.Keys.OrderBy(f => f.LastName).ThenBy(f => f.FirstName).ToList())
                {
                    String teacherName = String.Format("{0} {1}", teacher.FirstName, teacher.LastName);

                    int AttendancesRequired = 0;
                    int AttendancesTaken = 0;
                    int AttendancesNotTaken = 0;

                    foreach(CompData cd in data[teacher])
                    {
                        foreach(AttendanceStatus status in cd.DailyStats.Values)
                        {
                            if(status != AttendanceStatus.NoMeeting)
                                AttendancesRequired++;

                            if(status == AttendanceStatus.Completed)
                                AttendancesTaken++;

                            if(status == AttendanceStatus.NotTaken)
                                AttendancesNotTaken++;
                        }
                    }

                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        { "Name", teacherName },
                        { "Total Sections", String.Format("{0}", data[teacher].Count)},
                        { "Attendance Score", String.Format("{0:P}", (double)AttendancesTaken/(double)AttendancesRequired)}
                    };

                    csv.Add(row);
                }
            }

            return csv;
        }

        public static CSV GetQuickOverview(DateRange week)
        {
            using( WebhostEntities db = new WebhostEntities())
            {
                CSV csv = new CSV();

                foreach (Student student in db.Students.Where(s => s.isActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList())
                {
                    List<AttendanceMarking> marks = student.AttendanceMarkings.Where(mk => mk.AttendanceDate >= week.Start && mk.AttendanceDate <= week.End).ToList();
                    int lates = 0, cuts = 0;

                    foreach (AttendanceMarking mark in marks)
                    {
                        switch (mark.Marking.Name)
                        {
                            case "Late": lates++; break;
                            case "Cut": cuts++; break;
                            default: break;
                        }
                    }
                    int score = cuts + (lates / 4);
                    bool fridayDet = score > 0;

                    if (fridayDet)
                    {
                        csv.Add(new Dictionary<string, string>
                            {
                                {"Student", String.Format("{0} {1} [{2}]", student.FirstName, student.LastName, student.GraduationYear)},
                                {"Lates", String.Format("{0}", lates)},
                                {"Cuts", String.Format("{0}", cuts)}
                            });
                    }
                }

                return csv;
            }
        }



        public static List<Detention> GetDetentionList(DateRange week)
        {
            List<Detention> list = new List<Detention>();
            using (WebhostEntities db = new WebhostEntities())
            {
                foreach (Student student in db.Students.Where(s => s.isActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList())
                {
                    List<AttendanceMarking> marks = student.AttendanceMarkings.Where(mk => mk.AttendanceDate >= week.Start && mk.AttendanceDate <= week.End).ToList();
                    int lates = 0, cuts = 0;

                    foreach (AttendanceMarking mark in marks)
                    {
                        switch (mark.Marking.Name)
                        {
                            case "Late": lates++; break;
                            case "Cut": cuts++; break;
                            default: break;
                        }
                    }

                    int score = cuts + (lates / 4);
                    bool fridayDet = score > 0;
                    bool saturayDet = score > 1;

                    if (fridayDet)
                    {
                        Detention det = new Detention()
                        {
                            studentId = student.ID,
                            lates = lates,
                            cuts = cuts,
                            markingIDs = marks.Where(mk => mk.Marking.Name.Equals("Late") || mk.Marking.Name.Equals("Cut")).Select(mk => mk.id).ToList()
                        };

                        list.Add(det);
                    }
                }
            }
            return list;
        }



        public static void SendDetentionEmail(List<Detention> list)
        {
            using(WebhostEntities db=new WebhostEntities())
            {
                foreach (Detention detention in list)
                {
                    Student student = db.Students.Where(s => s.ID == detention.studentId).Single();
                    String subject = String.Format("{0} Hour Detention Notification", detention.twoHour ? "Two" : "One");
                    String body = String.Format("{0} {1} [{2}],{3}{3}", student.FirstName, student.LastName, student.GraduationYear, Environment.NewLine) +
                        String.Format("You have been assigned to {0} hour detention this week for the following markings:{1}", detention.twoHour ? "two" : "one", Environment.NewLine);

                    foreach (int markid in detention.markingIDs)
                    {
                        AttendanceMarking mark = db.AttendanceMarkings.Where(mk => mk.id == markid).Single();
                        body += String.Format("\t{0} : [{1}] {2} on {3}.{4}", mark.Marking.Name, mark.Section.Block.LongName, mark.Section.Course.Name, mark.AttendanceDate.ToLongDateString(), Environment.NewLine);
                    }

                    body += String.Format("{0}If you have any questions please see Mr. McFall or Ms. Hammond.", Environment.NewLine) +
                            String.Format("{0}{0}Attendance Bot{0}CC: {1} {2} (Advisor)", Environment.NewLine, student.Advisor.FirstName, student.Advisor.LastName);

                    MailControler.MailToUser(subject, body, string.Format("{0}@dublinschool.org", student.Advisor.UserName), String.Format("{0} {1}", student.Advisor.FirstName, student.Advisor.LastName));
                    MailControler.MailToUser(subject, body, string.Format("{0}@dublinschool.org", student.UserName), String.Format("{0} {1}", student.FirstName, student.LastName));
                }
            }
        }

        public static CSV GetDetentionLists(DateRange week)
        {
            CSV csv = new CSV();

            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Student student in db.Students.Where(s => s.isActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList())
                {
                    List<AttendanceMarking> marks = student.AttendanceMarkings.Where(mk => mk.AttendanceDate >= week.Start && mk.AttendanceDate <= week.End).ToList();
                    int lates = 0, cuts = 0;

                    foreach(AttendanceMarking mark in marks)
                    {
                        switch(mark.Marking.Name)
                        {
                            case "Late": lates++; break;
                            case "Cut": cuts++; break;
                            default: break;
                        }
                    }

                    int score = cuts + (lates / 4);
                    bool fridayDet = score > 0;
                    bool saturayDet = score > 1;

                    if(fridayDet)
                    {
                        Dictionary<String, String> row = new Dictionary<string, string>()
                        {
                            {"Student", String.Format("{0} {1} [{2}]", student.FirstName, student.LastName, student.GraduationYear)},
                            {"Two Hour Detention", saturayDet?"X":""}
                        };

                        csv.Add(row);
                    }
                }
            }
            return csv;
        }

        public static String AttendanceWeekCheckIn(int studentId)
        {
            int year = DateRange.GetCurrentAcademicYear();
            String warning = "";
            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Where(s => s.ID == studentId).Single();
                DateRange thisWeek = new DateRange(DateRange.ThisFriday.AddDays(-7), DateRange.ThisFriday.AddDays(-1));

                int lates = 0;
                int cuts = 0;
                int totalCuts = student.AttendanceMarkings.Where(mk => mk.Marking.Name.Equals("Cut") && mk.Section.Course.AcademicYearID == year).Count();
                List<AttendanceMarking> marks = student.AttendanceMarkings.Where(mk => mk.AttendanceDate >= thisWeek.Start && mk.AttendanceDate <= thisWeek.End).ToList();
                foreach(AttendanceMarking mark in marks)
                {
                    if (mark.Marking.Name.Equals("Late"))
                        lates++;
                    else if (mark.Marking.Name.Equals("Cut"))
                        cuts++;
                }

                if(lates > 0)
                {
                    warning += String.Format("You have accumulated {0} Lates this week.  ", lates);
                }

                if(lates > 3)
                {
                    warning += String.Format("This counts as {0} additional unexcused absences.", lates / 4);
                }

                if(cuts > 0)
                {
                    warning += String.Format("{2}You have accumulated {0} Unexcused Absences this week{1}.", cuts, lates > 3 ? " (Not including those incurred from lates)" : "", Environment.NewLine);
                }

                if(totalCuts > 4 && cuts > 0)
                {
                    warning += String.Format("{0}****You have accumulated {0} unexcused absences this year.", Environment.NewLine, totalCuts);
                }
            }

            return warning;
        }

        public static int LatesPerCut
        {
            get
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("LatesPerCut")).Single();
                    return Convert.ToInt32(lpc.Value);
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("LatesPerCut")).Single();
                    lpc.Value = Convert.ToString(value);
                    db.SaveChanges();
                }
            }
        }

        public static int CutsPer1Hr
        {
            get
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("CutsPer1Hr")).Single();
                    return Convert.ToInt32(lpc.Value);
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("CutsPer1Hr")).Single();
                    lpc.Value = Convert.ToString(value);
                    db.SaveChanges();
                }
            }
        }
        public static int CutsPer2Hr
        {
            get
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("CutsPer2Hr")).Single();
                    return Convert.ToInt32(lpc.Value);
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("CutsPer2Hr")).Single();
                    lpc.Value = Convert.ToString(value);
                    db.SaveChanges();
                }
            }
        }
        public static int CutsPerCampus
        {
            get
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("CutsPerCampus")).Single();
                    return Convert.ToInt32(lpc.Value);
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Variable lpc = db.Variables.Where(v => v.Name.Equals("CutsPerCampus")).Single();
                    lpc.Value = Convert.ToString(value);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Sends Emails To Students and their adivsors about attendance changes.
        /// </summary>
        /// <param name="markingInfo">Pairs are (AttendanceMarking.id, isUpdate)</param>
        protected static void SendAttendanceWarningEmails(Dictionary<int, bool> markingInfo)
        {
            foreach (int markId in markingInfo.Keys)
            {
                bool isUpdate = markingInfo[markId];
                String Subject = isUpdate ? "Attendance Updated" : "Attendance Warning";
                using (WebhostEntities db = new WebhostEntities())
                {
                    AttendanceMarking marking = db.AttendanceMarkings.Where(mk => mk.id == markId).Single();

                    if (!isUpdate && (marking.Marking.Name.Equals("Present")))
                    {
                        // Don't send an email for present markings, unless it is a correction!
                        continue;
                    }

                    Subject += String.Format(" for {0}", marking.Section.Block.Name.Equals("Morning Meeting") ?
                        "Morning Meeting" : String.Format("[{0}] {1}", marking.Section.Block.LongName, marking.Section.Course.Name));


                    String Body = String.Format(
                        "Dear {1},{0}{0}" +
                        "Your attendance for {2} has been marked as \"{3}\" for {4}.{0}{0}" +
                        AttendanceWeekCheckIn(marking.StudentID) + "{0}" +
                        "If you believe this to be an error, please contact your teacher, Ms. LeClaire, or Mr. McFall.{0}{0}" + 
                        "--Attendance Bot.{0}{0}" + 
                        "C.C. {5} {6} (Advisor)", 
                        Environment.NewLine,
                        marking.Student.FirstName,
                        marking.Section.Block.Name.Equals("Morning Meeting") ?
                        "Morning Meeting" : String.Format("[{0}] {1}", marking.Section.Block.LongName, marking.Section.Course.Name),
                        marking.Marking.Name,
                        marking.AttendanceDate.ToLongDateString(),
                        marking.Student.Advisor.FirstName,
                        marking.Student.Advisor.LastName
                        );

                    MailControler.MailToUser(Subject, Body, String.Format("{0}@dublinschool.org", marking.Student.UserName), String.Format("{0} {1}", marking.Student.FirstName, marking.Student.LastName));
                    MailControler.MailToUser(Subject, Body, String.Format("{0}@dublinschool.org", marking.Student.Advisor.UserName), String.Format("{0} {1}", marking.Student.Advisor.FirstName, marking.Student.Advisor.LastName));
                    MailControler.MailToUser(Subject, Body, MailControler.DeanOfStudents.Email, MailControler.DeanOfStudents.Name, "attendance@dublinschool.org", "Attendance Bot");
                    foreach(Faculty faculty in marking.Section.Teachers.ToList())
                    {
                        MailControler.MailToUser(Subject, Body, String.Format("{0}@dublinschool.org", faculty.UserName), String.Format("{0} {1}", faculty.FirstName, faculty.LastName), "attendance@dublinschool.org", "Attendance Bot");
                    }
                    //MailControler.MailToUser(Subject, Body, MailControler.AsstDeanOfStudents.Email, MailControler.AsstDeanOfStudents.Name, "attendance@dublinschool.org", "Attendance Bot");
                }
            }
        }

        public struct ShortMarking
        {
            public String notes;
            public int markId;
        }

        [DataContract]
        public struct AdditionalInfoRequest
        {
            /// <summary>
            /// AttendanceMarking.id
            /// </summary>
            [DataMember(Name="attendance_mark_id")]
            public int markId;
            /// <summary>
            /// GradeTableEntry.id
            /// </summary>
            [DataMember(Name="attendance_gradetable_entry_id")]
            public int markingId;
            /// <summary>
            /// Reason to display to the submitter.
            /// </summary>
            [DataMember(Name="reason_for_request")]
            public String reason;
        }

        public static int LookupAttendanceIdByName(String name)
        {
            int year = DateRange.GetCurrentAcademicYear();
            using(WebhostEntities db = new WebhostEntities())
            {
                GradeTable AttendanceTable = db.GradeTables.Where(gt => gt.AcademicYearID == year && gt.Name.Equals("Attendance")).Single();

                if (AttendanceTable.GradeTableEntries.Where(e => e.Name.Equals(name)).Count() <= 0) throw new InvalidOperationException("No Marking named " + name);

                return AttendanceTable.GradeTableEntries.Where(e => e.Name.Equals(name)).Single().id;
            }
        }

        public static String PullFromSchoology(DateRange dates = null)
        {
            String report = "";
            Dictionary<int, bool> updateInfo = new Dictionary<int, bool>();
            if (dates == null)
            {
                report += "Getting data for this week." + Environment.NewLine;
                dates = new DateRange(DateRange.ThisFriday.AddDays(-5), DateRange.ThisFriday);
            }
            else
            {
                report += "Getting data for " + dates.ToString() + Environment.NewLine;
            }
            using(WebhostEntities db = new WebhostEntities())
            {
                int markId = db.AttendanceMarkings.Count() > 0 ? db.AttendanceMarkings.OrderBy(mark => mark.id).ToList().Last().id : 0;
                foreach(Term term in db.Terms.ToList())
                {
                    if (!dates.Intersects(term.StartDate, term.EndDate))
                    {
                        continue;
                    }

                    foreach(Section section in term.Sections.Where(sec => sec.SchoologyId > 0).ToList())
                    {
                        report += String.Format("Pulling attendance data for [{0}] {1}" + Environment.NewLine, section.Block.LongName, section.Course.Name);
                        List<SchoologyAttendance> schoology_attendances = SchoologyAttendance.Download(section.SchoologyId, dates);
                        report += String.Format("Schoology returned {0} attendance marking for the given date range." + Environment.NewLine, schoology_attendances.Count);
                        List<AttendanceMarking> existingMarkings = section.AttendanceMarkings.Where(att => att.AttendanceDate >= dates.Start && att.AttendanceDate <= dates.End).ToList();
                        foreach(SchoologyAttendance attendance in schoology_attendances)
                        {
                            if(db.Students.Where(s => s.ID == attendance.enrollment.user_id).Count() <= 0)
                            {
                                report += String.Format("Skipping bad data:  {0}", attendance.ToString());
                                continue;
                            }
                            report += "Processing: " + attendance.ToString();
                            int studentId = attendance.enrollment.user_id;
                            Student student = db.Students.Find(studentId);


                            if(existingMarkings.Where(mk => mk.StudentID == studentId && mk.AttendanceDate.Equals(attendance.date)).Count() > 0)
                            {
                                AttendanceMarking mark = existingMarkings.Where(mk => mk.StudentID == studentId && mk.AttendanceDate.Equals(attendance.date)).Single();
                                if (mark.Marking.Name.Equals("Excused") && !attendance.AttendanceMarking.Equals("Present"))
                                {
                                    report += "Skipping excused marking on Webhost side." + Environment.NewLine;
                                    WebhostEventLog.Syslog.LogWarning("Skipping excused marking on the webhost side.");
                                    continue; // don't overwrite an excused absence!
                                }
                                
                                if(!attendance.AttendanceMarking.Equals(mark.Marking.Name))
                                {
                                    updateInfo.Add(mark.id, true);
                                    mark.SubmissionTime = DateTime.Now;
                                    report += String.Format("Updating record for {0}" + Environment.NewLine, attendance.ToString());
                                    mark.MarkingIndex = LookupAttendanceIdByName(attendance.AttendanceMarking);
                                    mark.Notes = attendance.Notes;

                                    WebhostEventLog.Syslog.LogInformation("Updating record.",
                                        typeof(AttendanceData), new AttendanceData()
                                        {
                                            StudentId = studentId,
                                            Name = string.Format("{0} {1}", student.FirstName, student.LastName),
                                            Marking = mark.Marking.Name,
                                            SectionName = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                            Date = attendance.date.ToShortDateString()
                                        });
                                }
                                else
                                {
                                    report += String.Format("No Change to record {0}.{1}", attendance.ToString(), Environment.NewLine);
                                }
                            }
                            else
                            {
                                String teacherName = "";
                                int tid = 9997;
                                if(section.Teachers.Count > 0)
                                {
                                    Faculty teacher = section.Teachers.FirstOrDefault();
                                    teacherName = String.Format("[{0} {1}] ", teacher.FirstName, teacher.LastName);
                                    tid = teacher.ID;
                                }
                                AttendanceMarking mark = new AttendanceMarking()
                                {
                                    id = ++markId,
                                    MarkingIndex = LookupAttendanceIdByName(attendance.AttendanceMarking),
                                    StudentID = studentId,
                                    SectionIndex = section.id,
                                    Notes = teacherName + attendance.Notes,
                                    SubmissionTime = DateTime.Now,
                                    AttendanceDate = attendance.date,
                                    SubmittedBy = tid                                   
                                };

                                updateInfo.Add(mark.id, false);
                                db.AttendanceMarkings.Add(mark);
                                report += String.Format("Created new record for {0}" + Environment.NewLine, attendance.ToString());
                                Type type = typeof(AttendanceData);
                                AttendanceData data = new AttendanceData()
                                    {
                                        StudentId = studentId,
                                        Name = string.Format("{0} {1}", student.FirstName, student.LastName),
                                        Marking = attendance.AttendanceMarking,
                                        SectionName = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                        Date = attendance.date.ToShortDateString()
                                    };
                                WebhostEventLog.Syslog.LogInformation("Created new record.", type, data);
                            }
                        }
                    }
                }
                db.SaveChanges();
            }
            if(EmailEnabled)
            {
                report += String.Format("Sending {0} emails concerning attendance.{1}", updateInfo.Keys.Count, Environment.NewLine);
                SendAttendanceWarningEmails(updateInfo);
            }

            report += "Done!";
            return report;
        }

        /// <summary>
        /// Submit Daily Attendance for a given section.
        /// </summary>
        /// <param name="sectionId">What section?</param>
        /// <param name="StudentAttendanceMarkings">Dictionary mapping Student.id => GradeTableEntry.id</param>
        /// <param name="attendanceDate">Default: DateTime.Today</param>
        /// <returns>A Report of Data Entered into the Database.</returns>
        public static Dictionary<int, AdditionalInfoRequest> SubmitAttendance(int sectionId, Dictionary<int,ShortMarking> StudentAttendanceMarkings, int submitter_id = 9997, DateTime attendanceDate = new DateTime())
        {
            //String report = "";
            Dictionary<int, AdditionalInfoRequest> InfoRequest = new Dictionary<int, AdditionalInfoRequest>();
            Dictionary<int, bool> updateInfo = new Dictionary<int, bool>();
            if(attendanceDate.Year < 2000) attendanceDate = DateTime.Today;
            using (WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == sectionId).Single();
                using (var tran = new TransactionScope())
                {
                    if (section.AttendanceSubmissionStatuses.Where(substat => substat.SectionId == sectionId && substat.Day.Equals(attendanceDate) && substat.AttendanceStatus.Blocking).Count() > 0)
                    {
                        AttendanceSubmissionStatus sta = section.AttendanceSubmissionStatuses.Where(substat => substat.SectionId == sectionId && substat.Day.Equals(attendanceDate) && substat.AttendanceStatus.Blocking).Single();
                        if (sta.TimeStamp.AddSeconds(5 * section.Students.Count) < DateTime.Now)
                        {
                            WebhostEventLog.Syslog.LogError("Ignoring strange status flag on [{0}] {1}.  Status \"{2}\" has been active for more than {3}:{4} minutes",
                                section.Block.LongName, section.Course.Name,
                                sta.AttendanceStatus.Name,
                                (5 * section.Students.Count) / 60, (5 * section.Students.Count) % 60);
                            sta.StatusId = db.AttendanceStatuses.Where(s => s.Name.Equals("Currently Submitting")).Single().id;
                            sta.TimeStamp = DateTime.Now;
                            sta.UpdateCount++;
                        }
                        else
                        {
                            WebhostEventLog.Syslog.LogError("Cannot submit attendance for [{0}] {1}.  It has had status {2}.", section.Block.LongName, section.Course.Name,
                                sta.AttendanceStatus.Name);

                            throw new WebhostException(String.Format("Cannot submit attendance for [{0}] {1}.  It has had status {2}.", section.Block.LongName, section.Course.Name,
                                sta.AttendanceStatus.Name));
                        }
                    }
                    else if (section.AttendanceSubmissionStatuses.Where(substat => substat.SectionId == sectionId && substat.Day.Equals(attendanceDate)).Count() > 0)
                    {
                        AttendanceSubmissionStatus status = section.AttendanceSubmissionStatuses.Where(substat => substat.SectionId == sectionId && substat.Day.Equals(attendanceDate)).Single();
                        status.StatusId = db.AttendanceStatuses.Where(s => s.Name.Equals("Currently Submitting")).Single().id;
                        status.TimeStamp = DateTime.Now;
                        status.UpdateCount++;
                    }
                    else
                    {
                        db.AttendanceSubmissionStatuses.Add(new AttendanceSubmissionStatus()
                            {
                                StatusId = db.AttendanceStatuses.Where(s => s.Name.Equals("Currently Submitting")).Single().id,
                                Day = attendanceDate,
                                SectionId = sectionId,
                                Section = section,
                                UpdateCount = 0,
                                AttendanceStatus = db.AttendanceStatuses.Where(s => s.Name.Equals("Currently Submitting")).Single(),
                                id = db.AttendanceSubmissionStatuses.Count() > 0 ? db.AttendanceSubmissionStatuses.OrderBy(s => s.id).ToList().Last().id + 1 : 0,
                                TimeStamp = DateTime.Now
                            });
                    }
                    db.SaveChanges();
                    tran.Complete();
                }

                int markId = db.AttendanceMarkings.Count() > 0 ? db.AttendanceMarkings.OrderBy(mark => mark.id).ToList().Last().id : 0;
                int oldEnd = markId;
                List<AttendanceMarking> newMarkings = new List<AttendanceMarking>();
                Faculty submitter = db.Faculties.Find(submitter_id);
                if(submitter == null)
                {
                    WebhostEventLog.Syslog.LogError("Invalid Attendance Submitter Id was passed to SubmitAttendance.  id:  {0}", submitter_id);
                    throw new ArgumentException("Invalid Submitter Id " + submitter_id);
                }

                // check for existing attendances
                List<AttendanceMarking> markings = section.AttendanceMarkings.Where(att => att.AttendanceDate.Equals(attendanceDate)).ToList();
                foreach (int studentId in StudentAttendanceMarkings.Keys)
                {
                    if(section.Students.Where(s => s.ID == studentId).Count() <= 0)
                    {
                        //No idea why I wrote this....
                        //section.Students.Add(db.Students.Where(s => s.ID == studentId).Single());
                        WebhostEventLog.Syslog.LogError("Student {0} is not enrolled in section {1} [{2}] {3}.", studentId, sectionId, section.Block.LongName, section.Course.Name);
                        throw new InvalidDataException(String.Format("Student {0} is not enrolled in section {1} [{2}] {3}.", studentId, sectionId, section.Block.LongName, section.Course.Name));
                    }

                    
                    String teacherName = "";
                    if (section.Teachers.Count > 0)
                    {
                        Faculty teacher = section.Teachers.FirstOrDefault();
                        teacherName = String.Format("[{0} {1}] ", teacher.FirstName, teacher.LastName);
                    }

                    Student student = section.Students.Where(s => s.ID == studentId).Single();

                    bool holdSubmission = false;

                    if (markings.Where(att => att.StudentID == studentId).Count() <= 0)
                    {
                        #region New Attendance Markings
                        //report += String.Format("No attendance marking was recorded for {0} {1}.  Creating one.{2}", student.FirstName, student.LastName, Environment.NewLine);
                        AttendanceMarking newMark = new AttendanceMarking()
                        {
                            id = ++markId,
                            StudentID = studentId,
                            MarkingIndex = StudentAttendanceMarkings[studentId].markId,
                            AttendanceDate = attendanceDate,
                            SectionIndex = sectionId,
                            Notes = teacherName + StudentAttendanceMarkings[studentId].notes,
                            SubmissionTime = DateTime.Now,
                            SubmittedBy = submitter_id
                        };

                        GradeTableEntry themarking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId);
                        if(themarking.Name.Equals("Excused"))
                        {
                            // // Added this for Simon, so we know who is changing an attendance marking.
                            // This code requests additional information before saving changes that are not documented.
                            WebhostEventLog.Syslog.LogWarning(String.Format("{0} {1} submitted an Excused marking through the standard submission channel.", submitter.FirstName, submitter.LastName), 
                                typeof(AttendanceData), new AttendanceData()
                                {
                                    Date = attendanceDate.ToShortDateString(),
                                    SectionName = string.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                    Marking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId).Name,
                                    Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                    StudentId = studentId
                                });

                            String notes = StudentAttendanceMarkings[studentId].notes;
                            Regex noInfo = new Regex(@"\s*\[.+\]\s*");
                            foreach(Match match in noInfo.Matches(notes))
                            {
                                notes = notes.Replace(match.Value, "");
                            }

                            if (notes.Equals(""))
                            {
                                holdSubmission = true;
                                WebhostEventLog.Syslog.LogError("Holding attendance submission because no information was provided.", typeof(AttendanceData), new AttendanceData()
                                {
                                    Date = attendanceDate.ToShortDateString(),
                                    SectionName = string.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                    Marking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId).Name,
                                    Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                    StudentId = studentId
                                });
                                InfoRequest.Add(studentId, new AdditionalInfoRequest()
                                {
                                    markId = newMark.id,
                                    markingId = newMark.MarkingIndex,
                                    reason = String.Format("Why was {0} {1} excused?", student.FirstName, student.LastName)
                                });
                            }
                        }
                        if (!holdSubmission)
                        {
                            updateInfo.Add(newMark.id, false);
                            WebhostEventLog.Syslog.LogInformation("Created new attendance.", typeof(AttendanceData), new AttendanceData()
                                {
                                    Date = attendanceDate.ToShortDateString(),
                                    SectionName = string.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                    Marking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId).Name,
                                    Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                    StudentId = studentId
                                });
                            db.AttendanceMarkings.Add(newMark);
                            newMarkings.Add(newMark);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Update Attendance Marking.
                        //report += String.Format("Updating record for {0} {1}.{2}", student.FirstName, student.LastName, Environment.NewLine);
                        AttendanceMarking existingMark = markings.Where(att => att.StudentID == studentId).Single();
                        
                        if (StudentAttendanceMarkings[studentId].markId != existingMark.MarkingIndex)
                        {
                            GradeTableEntry newMarking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId);
                            if(existingMark.Marking.Name.Equals("Excused"))
                            {
                                // Added this for Simon, so we know who is changing an attendance marking.
                                // This code assumes that any updates have been apporoved before they make it to this point.
                                // Authority Checks need to happen before this method is called.
                                // BUT the information of who makes the change and what was changed is logged!
                                WebhostEventLog.Syslog.LogWarning("{0} {1} has updated an Excused Absence to {3}.  Notes:  {2}",
                                        submitter.FirstName, submitter.LastName, StudentAttendanceMarkings[studentId].notes, newMarking.Name);
                                if (StudentAttendanceMarkings[studentId].notes.Equals(""))
                                {
                                    holdSubmission = true; 
                                    WebhostEventLog.Syslog.LogError("Holding attendance update submission because no information was provided.", typeof(AttendanceData), new AttendanceData()
                                    {
                                        Date = attendanceDate.ToShortDateString(),
                                        SectionName = string.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                        Marking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId).Name,
                                        Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                        StudentId = studentId
                                    });
                                    InfoRequest.Add(studentId, new AdditionalInfoRequest()
                                    {
                                        markId = existingMark.id,
                                        markingId = StudentAttendanceMarkings[studentId].markId,
                                        reason = String.Format("Why are you changing {0} {1}'s Excused marking?", student.FirstName, student.LastName)
                                    });
                                }
                            }
                            else if(newMarking.Name.Equals("Excused"))
                            {
                                WebhostEventLog.Syslog.LogWarning("{0} {1} is changing {3} to Excused.  Notes:  {2}",
                                        submitter.FirstName, submitter.LastName, StudentAttendanceMarkings[studentId].notes, existingMark.Marking.Name);
                                
                                String notes = StudentAttendanceMarkings[studentId].notes;
                                Regex noInfo = new Regex(@"\s*\[.+\]\s*");
                                foreach (Match match in noInfo.Matches(notes))
                                {
                                    notes = notes.Replace(match.Value, "");
                                }

                                if (notes.Equals(""))
                                {
                                    holdSubmission = true;
                                    WebhostEventLog.Syslog.LogError("Holding attendance update submission because no information was provided.", typeof(AttendanceData), new AttendanceData()
                                    {
                                        Date = attendanceDate.ToShortDateString(),
                                        SectionName = string.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                        Marking = db.GradeTableEntries.Find(StudentAttendanceMarkings[studentId].markId).Name,
                                        Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                        StudentId = studentId
                                    });
                                    InfoRequest.Add(studentId, new AdditionalInfoRequest()
                                    {
                                        markId = existingMark.id,
                                        markingId = StudentAttendanceMarkings[studentId].markId,
                                        reason = String.Format("Why are you changing {0} {1}'s marking to Excused?", student.FirstName, student.LastName)
                                    });
                                }
                            }
                            if (!holdSubmission)
                            {
                                updateInfo.Add(existingMark.id, true);
                                WebhostEventLog.Syslog.LogInformation("Updated attendance.", typeof(AttendanceData), new AttendanceData()
                                {
                                    Date = attendanceDate.ToShortDateString(),
                                    SectionName = string.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                                    Marking = newMarking.Name,
                                    Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                    StudentId = studentId
                                });

                                existingMark.SubmissionTime = DateTime.Now;
                                existingMark.MarkingIndex = StudentAttendanceMarkings[studentId].markId;
                                existingMark.Notes += existingMark.Notes.Contains(teacherName + StudentAttendanceMarkings[studentId].notes) ?
                                    "" : String.Format("{0}{1}{2}", Environment.NewLine, teacherName, StudentAttendanceMarkings[studentId].notes);
                                existingMark.SubmittedBy = submitter_id;
                            }

                        }
                        #endregion
                    }
                }

                try
                {
                    db.SaveChanges();
                    WebhostEventLog.Syslog.LogInformation("Successfully Submitted attendance for [{0}] {1}", section.Block.LongName, section.Course.Name);
                }
                catch(System.Data.Entity.Infrastructure.DbUpdateException ve)
                {
                    WebhostEventLog.Syslog.LogWarning("Could Not Save Database data: {0}", ve.Message);
                    bool error = true;
                    int saveAttemptCount = 1;
                    while(error)
                    {
                        markId = db.AttendanceMarkings.Count() > 0 ? db.AttendanceMarkings.OrderBy(mark => mark.id).ToList().Last().id : 0;
                        if(markId == oldEnd)
                        {
                            WebhostEventLog.Syslog.LogWarning("Couldn't get a new starting index...");
                            MailControler.MailToWebmaster("It didn't work...", String.Format("Could not get next attendance index.... {0} for class [{1}] {2}", 
                                ve.Message, section.Block.LongName, section.Course.Name));
                            return new Dictionary<int, AdditionalInfoRequest>();
                        }
                        else
                        {
                            WebhostEventLog.Syslog.LogInformation("Trying Again to save Data.");
                            foreach(AttendanceMarking mk in newMarkings)
                            {
                                mk.id = ++markId;
                            }

                            saveAttemptCount++;
                            try
                            {
                                db.SaveChanges();
                                error = false;
                                WebhostEventLog.Syslog.LogInformation("Successfully Submitted Attendance for [{0}] {1} after {2} attempts with conflicts.", section.Block.LongName, section.Course.Name, saveAttemptCount);
                            }
                            catch (System.Data.Entity.Infrastructure.DbUpdateException)
                            {
                                error = true;
                            }
                        }
                    }
                }

                AttendanceSubmissionStatus subStat = section.AttendanceSubmissionStatuses.Where(substat => substat.Day.Equals(attendanceDate)).Single();
                subStat.StatusId = db.AttendanceStatuses.Where(stat => stat.Name.Equals("Submitted")).Single().id;
                subStat.TimeStamp = DateTime.Now;
                db.SaveChanges();
            }

            if (EmailEnabled)
            {
                //report += String.Format("Sending {0} emails concerning attendance.{1}", updateInfo.Keys.Count, Environment.NewLine);
                SendAttendanceWarningEmails(updateInfo);
            }
            //report += "Done!";
            return InfoRequest;
        }

        #region Excused Absences

        /// <summary>
        /// This is just a bulk excuse method.
        /// It calls ExcuseStudent for each date in a DateRange.
        /// </summary>
        /// <param name="studentId">student being excused</param>
        /// <param name="sectionIds">list of section ids to excuse from</param>
        /// <param name="Notes">Notes to be added to the field.</param>
        /// <param name="sender">Who is doing the excusing?</param>
        /// <param name="dates">Dates to excuse.</param>
        /// <returns></returns>
        public static String ExcuseStudent(int studentId, List<int> sectionIds, String Notes, ADUser sender, DateRange dates)
        {
            String report = "";
            foreach (DateTime date in dates.ToList())
                report += ExcuseStudent(studentId, sectionIds, Notes, sender, date) + Environment.NewLine;
            return report;
        }

        /// <summary>
        /// Excuse a student from a list of sections for a particular day
        /// </summary>
        /// <param name="studentId">student being excused</param>
        /// <param name="sectionIds">list of section ids to excuse from</param>
        /// <param name="Notes">Notes to be added to the field.</param>
        /// <param name="sender">Who is doing the excusing?</param>
        /// <param name="date">Date for which this is excused.</param>
        /// <returns></returns>
        public static String ExcuseStudent(int studentId, List<int> sectionIds, String Notes, ADUser sender, DateTime date = new DateTime())
        {
            
            Notes = String.Format("[{0}] {1}", sender.Name, Notes);
            String report = "";
            if (date.Equals(new DateTime()))
            {
                report = "Excusing absence for today." + Environment.NewLine;
                date = DateTime.Today;
            }
            else
                report = String.Format("Excusing absence for {0}.{1}", date.ToLongDateString(), Environment.NewLine);
            int excusedIndex = LookupAttendanceIdByName("Excused");

            String subject = String.Format("Student Excused {0}", date.ToLongDateString());
            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Where(s => s.ID == studentId).Single();
                List<String> sectionNames = new List<string>();
                int mkindex = db.AttendanceMarkings.Count() > 0 ? db.AttendanceMarkings.OrderBy(mk => mk.id).ToList().Last().id : 0;
                List<AttendanceMarking> newlyCreatedMarks = new List<AttendanceMarking>();
                foreach(int sectionId in sectionIds)
                {
                    Section section = db.Sections.Where(sec => sec.id == sectionId).Single();
                    report += String.Format("Excusing {0} {1} from {2}.{3}", student.FirstName, student.LastName, section.Course.Name, Environment.NewLine);
                    if(section.Block.Name.Equals("Morning Meeting") || section.Course.Department.Name.Equals("Residential Life"))
                    {
                        sectionNames.Add(section.Course.Name);
                    }
                    else
                    {
                        sectionNames.Add(String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name));
                    }

                    if(section.AttendanceMarkings.Where(mk => mk.AttendanceDate.Equals(date) && mk.StudentID == studentId).Count() > 0)
                    {
                        AttendanceMarking marking = section.AttendanceMarkings.Where(mk => mk.AttendanceDate.Equals(date) && mk.StudentID == studentId).Single();
                        String oldMark = marking.Marking.Name;
                        marking.MarkingIndex = excusedIndex;
                        marking.Notes = Notes;
                        report += "Updated Existing Attendance Marking." + Environment.NewLine;
                        WebhostEventLog.Syslog.LogInformation("Updated " + oldMark + " marking.", typeof(AttendanceData), new AttendanceData()
                            {
                                Date = marking.AttendanceDate.ToShortDateString(),
                                StudentId = studentId,
                                Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                                Marking = "Excused",
                                SectionName = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name)
                            });
                    }
                    else
                    {
                        AttendanceMarking marking = new AttendanceMarking()
                        {
                            id = ++mkindex,
                            MarkingIndex = excusedIndex,
                            Notes = Notes,
                            AttendanceDate = date,
                            StudentID = studentId,
                            SectionIndex = sectionId,
                            SubmissionTime = DateTime.Now,
                            SubmittedBy = sender.ID
                        };
                        report += "Created new Attendance Marking entry." + Environment.NewLine;
                        WebhostEventLog.Syslog.LogInformation("Created Excused marking.", typeof(AttendanceData), new AttendanceData()
                        {
                            Date = marking.AttendanceDate.ToShortDateString(),
                            StudentId = studentId,
                            Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                            Marking = "Excused",
                            SectionName = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name)
                        });
                        newlyCreatedMarks.Add(marking);
                        db.AttendanceMarkings.Add(marking);
                    }

                    foreach (Faculty teacher in section.Teachers)
                    {
                        report += String.Format("Sending update email to {0} {1}.{2}", teacher.FirstName, teacher.LastName, Environment.NewLine);
                        String body = String.Format(
                            "Dear {1},{0}{0}" +
                            "{2} has Excused {3} {4} from your [{5}] {6} on {7}.{0}{0}" +
                            "Reason:{0}{8}{0}{0}" +
                            "--Attendance Bot",
                            Environment.NewLine,
                            teacher.FirstName,
                            sender.Name,
                            student.FirstName,
                            student.LastName,
                            section.Block.LongName,
                            section.Course.Name,
                            date.ToLongDateString(),
                            Notes);
                        MailControler.MailToUser(subject, body, String.Format("{0}@dublinschool.org", teacher.UserName), String.Format("{0} {1}", teacher.FirstName, teacher.UserName));
                    }
                } 
                
                String bulkBody = String.Format("{0} {1} has been excused for {3} from:{2}{2}", student.FirstName, student.LastName, Environment.NewLine, date.ToLongDateString());
                foreach (String name in sectionNames)
                {
                    bulkBody += name + Environment.NewLine;
                }

                bulkBody += String.Format("{0}{0}--Attendance Bot{0}{0}C.C. {1} {2} (Advisor)", Environment.NewLine, student.Advisor.FirstName, student.Advisor.LastName);
                report += String.Format("Sending Bulk Update Email to {0} {1} and {2} {3}.", student.FirstName, student.LastName, student.Advisor.FirstName, student.Advisor.LastName);
                MailControler.MailToUser(subject, bulkBody, String.Format("{0}@dublinschool.org", student.UserName), String.Format("{0} {1}", student.FirstName, student.LastName));
                bulkBody += Environment.NewLine + "Reason:" + Environment.NewLine + Notes;
                MailControler.MailToUser(subject, bulkBody, String.Format("{0}@dublinschool.org", student.Advisor.UserName), String.Format("{0} {1}", student.Advisor.FirstName, student.Advisor.LastName));
                MailControler.MailToUser(subject, bulkBody, MailControler.DeanOfStudents.Email, MailControler.DeanOfStudents.Name);
                try
                {
                    db.SaveChanges();
                }
                catch(System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    bool failed = true;
                    int failCount = 1;
                    while(failed && failCount < 10)
                    {
                        int newId = db.AttendanceMarkings.OrderBy(m => m.id).Select(m => m.id).ToList().Last();
                        foreach (AttendanceMarking mk in newlyCreatedMarks)
                        {
                            mk.id = ++newId;
                        }
                        try
                        {
                            db.SaveChanges();
                            failed = false;
                        }
                        catch(System.Data.Entity.Infrastructure.DbUpdateException)
                        {
                            failed = true;
                        }
                    }

                    if(failCount >= 10)
                    {
                        WebhostEventLog.Syslog.LogError("Failed to Excuse some Absences for {0} {1} on {2}", student.FirstName, student.LastName, date.ToShortDateString());
                    }
                }
            }
            report += "Done!";
            return report;
        }

        /// <summary>
        /// Dump all recorded attendance data for the given student within a date range.
        /// </summary>
        /// <param name="dates"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public static CSV AttendanceDump(DateRange dates, int studentId)
        {
            CSV csv = new CSV();

            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Where(s => s.ID == studentId).Single();
                csv.Heading = String.Format("Attendance records for {0} {1} [{2}] for the dates of {3}", student.FirstName, student.LastName, student.GraduationYear, dates.ToString());
                List<AttendanceMarking> attendances = student.AttendanceMarkings.Where(att => att.AttendanceDate >= dates.Start && att.AttendanceDate <= dates.End).ToList();

                foreach (AttendanceMarking mark in attendances.OrderBy(att => att.AttendanceDate).ThenBy(att => att.Section.Block.Name))
                {
                    csv.Add(new Dictionary<string, string>()
                        {
                            {"Date", mark.AttendanceDate.ToShortDateString()},
                            {"Block", mark.Section.Block.LongName},
                            {"Class", mark.Section.Course.Name},
                            {"Marking", mark.Marking.Name}
                        });
                }
            }

            return csv;
        }


        #endregion // Excused Absences

        #region Weekend Discipline Calculation

        private class AttendanceCount
        {
            public int Lates;
            public int Cuts;

            public AttendanceCount()
            {
                Lates = 0;
                Cuts = 0;
            }
        }

        public static CSV AttendanceDump(DateRange dateRange)
        {
            int yr = DateRange.GetCurrentAcademicYear();
            CSV csv = new CSV();
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Student student in db.Students.Where(s => s.AttendanceMarkings.Where(att => att.AttendanceDate >= dateRange.Start && att.AttendanceDate <= dateRange.End).Count() > 0).ToList())
                {
                    Dictionary<String, String> row = new Dictionary<string,string>();
                    row.Add("First Name", student.FirstName);
                    row.Add("Last Name", student.LastName);
                    row.Add("Grad Year", Convert.ToString(student.GraduationYear));
                    int lates = 0, cuts = 0, excused = 0;
                    foreach(DateTime date in dateRange.ToList())
                    {
                        foreach(AttendanceMarking mark in student.AttendanceMarkings.Where(mk => mk.AttendanceDate == date).ToList())
                        {
                            switch(mark.Marking.Name)
                            {
                                case "Late": lates++; break;
                                case "Cut": cuts++; break;
                                case "Excused": excused++; break;
                                default: break;
                            }
                        }
                    }

                    row.Add("Total Lates", Convert.ToString(lates));
                    row.Add("Total Cuts", Convert.ToString(cuts));
                    row.Add("Total Excused", Convert.ToString(excused));

                    csv.Add(row);
                }
            }

            return csv;
        }

        public static Dictionary<string, List<int>> GetWeekendDisciplineLists(DateRange attendanceWeek, int latesPerCut, int cutsPer1Hr, int cutsPer2Hr, int cutsPerCampus)
        {
            Dictionary<String, List<int>> lists = new Dictionary<string, List<int>>();
            lists.Add("1Hr", new List<int>());
            lists.Add("2Hr", new List<int>());
            lists.Add("Campused", new List<int>());

            Dictionary<int, AttendanceCount> scorebook = new Dictionary<int,AttendanceCount>();  

            using(WebhostEntities db =new WebhostEntities())
            {
                foreach(AttendanceMarking marking in db.AttendanceMarkings.Where(mk => mk.AttendanceDate >= attendanceWeek.Start && mk.AttendanceDate <= attendanceWeek.End).ToList())
                {
                    if(marking.Marking.Name.Equals("Late"))
                    {
                        if(scorebook.ContainsKey(marking.StudentID))
                        {
                            scorebook[marking.StudentID].Lates++;
                        }
                        else
                        {
                            scorebook.Add(marking.StudentID, new AttendanceCount() { Lates = 1 });
                        }
                    }
                    else if (marking.Marking.Name.Equals("Cut"))
                    {
                        if (scorebook.ContainsKey(marking.StudentID))
                        {
                            scorebook[marking.StudentID].Cuts++;
                        }
                        else
                        {
                            scorebook.Add(marking.StudentID, new AttendanceCount() { Cuts = 1 });
                        }
                    }
                }    

                foreach(int sid in scorebook.Keys)
                {
                    float score = ((float)scorebook[sid].Lates / (float)latesPerCut) + scorebook[sid].Cuts;
                    if (score > cutsPerCampus)
                        lists["Campused"].Add(sid);

                    if (score > cutsPer2Hr)
                        lists["2Hr"].Add(sid);
                    else if (score > cutsPer1Hr)
                        lists["1Hr"].Add(sid);
                }
            }

            return lists;
        }

        public static List<int> GetOneHourDetention(int weekendId)
        {
            List<int> list = new List<int>();

            try
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(wk => wk.id == weekendId).Single();
                    foreach (WeekendDiscipline discipline in weekend.WeekendDisciplines.ToList())
                    {
                        if (discipline.DetentionHours == 1)
                        {
                            list.Add(discipline.StudentId);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                State.log.WriteLine("Failed to get Detention List\r\n" + e.Message);
            }
            return list;
        }

        public static List<int> GetTwoHourDetention(int weekendId)
        {
            List<int> list = new List<int>();

            try
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(wk => wk.id == weekendId).Single();
                    foreach (WeekendDiscipline discipline in weekend.WeekendDisciplines.ToList())
                    {
                        if (discipline.DetentionHours > 1)
                        {
                            list.Add(discipline.StudentId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                State.log.WriteLine("Failed to get Detention List\r\n" + e.Message);
            }
            return list;
        }

        public static List<int> GetCampusedList(int weekendId)
        {
            List<int> list = new List<int>();

            try
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(wk => wk.id == weekendId).Single();
                    foreach (WeekendDiscipline discipline in weekend.WeekendDisciplines.ToList())
                    {
                        if (discipline.Campused)
                        {
                            list.Add(discipline.StudentId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                State.log.WriteLine("Failed to get Campused List\r\n" + e.Message);
            }
            return list;
        }



        #endregion

        #region Morning Meeting

        /// <summary>
        /// Short Entry used for Table Creation
        /// </summary>
        public struct MorningMeetingSeat
        {
            public char row;
            public int seat;
            public String StudentName;
        }

        /// <summary>
        /// Mapping of Row Names to Lengths.
        /// The B row has only 13 seats listed because seat 14 is for Brad--don't forget to account of that in Displays!
        /// </summary>
        public static Dictionary<String, int> MorningMeetingRowSizes = new Dictionary<string, int>() { 
            { "A", 10 }, 
            { "B", 10 }, 
            { "C", 13 }, 
            { "D", 14 }, 
            { "E", 14 },
            { "F", 14 }, 
            { "G", 14 }, 
            { "H", 14 }, 
            { "I", 14 }, 
            { "J", 10 }, 
            { "K", 11 }, 
            { "L", 11 },
            { "M", 6 }
        };

        /// <summary>
        /// Get the SectionId for This year's Morning Meeting.
        /// If the section has not been created, this creates the section.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int MorningMeetingSectionId(int year = -1)
        {
            if (year == -1) year = DateRange.GetCurrentAcademicYear();
            using(WebhostEntities db = new WebhostEntities())
            {
                if(db.Sections.Where(sec => sec.Course.Name.Equals("Morning Meeting") && sec.Course.AcademicYearID == year).Count() > 0)
                {
                    return db.Sections.Where(sec => sec.Course.Name.Equals("Morning Meeting") && sec.Course.AcademicYearID == year).Single().id;
                }

                // Create Morning Meeting!

                int cid = -1;

                if(db.Courses.Where(crs => crs.AcademicYearID == year && crs.Name.Equals("Morning Meeting")).Count() <= 0)
                {
                    Course mm = new Course()
                    {
                        id = db.Courses.Count() > 0 ? db.Courses.OrderBy(c => c.id).ToList().Last().id +1 : 0,
                        Name = "Morning Meeting",
                        BlackBaudID = "mm",
                        AcademicYearID = year,
                        DepartmentID = 8, // Res Life!
                        goesToSchoology = false,
                        LengthInTerms = 3
                    };
                    db.Courses.Add(mm);
                    cid = mm.id;
                }
                else
                {
                    cid = db.Courses.Where(crs => crs.AcademicYearID == year && crs.Name.Equals("Morning Meeting")).Single().id;
                }

                Section mms = new Section()
                {
                    id = db.Sections.Count() > 0 ? db.Sections.OrderBy(s => s.id).ToList().Last().id +1 : 0,
                    BlockIndex = db.Blocks.Where(b => b.AcademicYearID == year && b.Name.Equals("Morning Meeting")).Single().id,
                    SectionNumber = 0,
                    CourseIndex = cid,
                    getsComments = false
                };

                db.Sections.Add(mms);
                db.SaveChanges();

                return mms.id;
            }
        }

        /// <summary>
        /// Build the Morning Meeting seating chart for Attendance.
        /// This will initialize all necessary Course and Section for the Given Year.
        /// Students are placed in the Chart if they are flagged as "Active" in the database.
        /// 
        /// Returns an HTML Table that is poorly formatted....
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static String GenerateMorningMeetingSeatingChart(int year = -1)
        {
            if (year == -1) year = DateRange.GetCurrentAcademicYear();


            List<MorningMeetingSeat> seatlist = new List<MorningMeetingSeat>();

            using(WebhostEntities db = new WebhostEntities())
            {
                // Check for exsisting MM Seating Chart.
                SeatingChart mmchart = new SeatingChart();
                bool update = false;
                if(db.SeatingCharts.Where(chart => chart.Section.Course.Name.Equals("Morning Meeting") && chart.Section.Course.AcademicYearID == year).Count() > 0)
                {
                    mmchart = db.SeatingCharts.Where(chart => chart.Section.Course.Name.Equals("Morning Meeting") && chart.Section.Course.AcademicYearID == year).Single();
                    update = true;
                }
                else
                {
                    mmchart.id = db.SeatingCharts.Count() > 0 ? db.SeatingCharts.OrderBy(ch => ch.id).ToList().Last().id + 1 : 0;
                    mmchart.Name = "Morning Meeting";
                    mmchart.SectionId = MorningMeetingSectionId(year);
                }

                List<Student> students = (from student in db.Students
                                          where student.isActive
                                          orderby student.GraduationYear, student.LastName, student.FirstName
                                          select student).ToList();
                int row = 0;
                int seat = 0;
                String rows = "ABCDEFGHIJKLM";

                foreach(Student student in students)
                {
                    // check for existing seat.
                    if(mmchart.SeatingChartSeats.Where(st => st.Row == row && st.Column == seat).Count() > 0)
                    {
                        SeatingChartSeat mmseat = mmchart.SeatingChartSeats.Where(st => st.Row == row && st.Column == seat).Single();
                        mmseat.StudentId = student.ID;
                    }
                    else
                    {
                        SeatingChartSeat mmseat = new SeatingChartSeat()
                        {
                            id = db.SeatingChartSeats.Count() > 0 ? db.SeatingChartSeats.OrderBy(s => s.id).ToList().Last().id : 0,
                            ChartId = mmchart.id,
                            StudentId = student.ID,
                            Row = row,
                            Column = seat
                        };
                        db.SeatingChartSeats.Add(mmseat);
                    }


                    seatlist.Add(new MorningMeetingSeat() { row = rows[row], seat = seat, StudentName = String.Format("{0} {1}", student.FirstName, student.LastName) });

                    seat = ++seat % MorningMeetingRowSizes[Convert.ToString(rows[row])];
                    row += seat == 0 ? 1 : 0;
                }

                for (; row < rows.Length; row++ )
                {
                    for(; seat < MorningMeetingRowSizes[Convert.ToString(rows[row])]; seat++)
                    {
                        if (mmchart.SeatingChartSeats.Where(st => st.Row == row && st.Column == seat).Count() > 0)
                        {
                            SeatingChartSeat mmseat = mmchart.SeatingChartSeats.Where(st => st.Row == row && st.Column == seat).Single();
                            mmseat.StudentId = null;
                        }
                    }
                    seat = 0;
                }

                if (!update)
                    db.SeatingCharts.Add(mmchart);

                db.SaveChanges();
            }

            seatlist.Add(new MorningMeetingSeat() { row = 'C', seat = 13, StudentName = "Brad Bates" });

            // Build HTML

            String chartHtml = "<table>\r\n\t<tr><th colspan='14'><center>Morning Meeting Seating Chart</center></th>";
            
            foreach(MorningMeetingSeat seat in (from seat in seatlist
                                                    orderby seat.row descending, seat.seat
                                                    select seat))
            {
                // new row?
                chartHtml += seat.seat == 0 ? "</tr>\r\n\t<tr>\r\n" : "\r\n";
                chartHtml += String.Format("\t\t<td><strong>{0}{1}</strong><hr/>{2}</td>", seat.row, seat.seat+1, seat.StudentName);
            }

            chartHtml += "\r\n\t</td>\r\n</table>";
            return chartHtml;

        }

        #endregion // Morning Meeting
    }

    [DataContract]
    public class AttendancePageInfo
    {
        [DataMember(Name = "section_id")]
        public int SectionId { get; set; }

        [DataMember(Name = "section_name")]
        public String Name { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "attendances")]
        public IEnumerable<AttendanceData> Attendances { get; set; }

        public static void LogCurrentData(AttendancePageInfo api)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(AttendancePageInfo));
            MemoryStream mstr = new MemoryStream();
            json.WriteObject(mstr, api);
            mstr.Position = 0;
            StreamReader sr = new StreamReader(mstr);
            String info = sr.ReadToEnd();
            sr.Close();
            mstr.Close();
            sr.Dispose();
            mstr.Dispose();
            WebhostEventLog.Syslog.LogInformation(info);
        }
    }

    [DataContract]
    public class AttendanceData
    {
        [DataMember(Name = "student_id")]
        public int StudentId { get; set; }

        [DataMember(Name = "student_name")]
        public String Name { get; set; }

        [DataMember(Name = "marking")]
        public String Marking { get; set; }

        [DataMember(Name="section_name")]
        public String SectionName { get; set; }

        [DataMember(Name="date")]
        public String Date { get; set; }

    }
}
