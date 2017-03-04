using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.EVOPublishing;
using WebhostMySQLConnection.GoogleAPI;
using WebhostMySQLConnection.SchoologyAPI;
using WebhostMySQLConnection;
using System.IO;
using System.Timers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using EvoPdf;
using System.Threading;
using WebhostMySQLConnection.AccountManagement;

namespace CLICommentLetterPublisher
{
    // c# class for processed clients  

    class Program
    {
        static System.Timers.Timer timer = new System.Timers.Timer() { AutoReset = true, Interval = TimeSpan.FromMinutes(3).TotalMilliseconds };

        static void Main()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Student student in db.Students.Where(s => s.isActive).ToList())
                {
                    AccountManagement.VerifyADUserAccountDirectories(student.UserName);
                }
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void PullSchoologyUserIds()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                using(SchoologyAPICall call = new SchoologyAPICall())
                {
                    XMLTree userList = call.ListUsers();

                    foreach(XMLTree userTree in userList.ChildTrees.Where(u => u.TagName.Equals("user")).ToList())
                    {
                        
                        int webhost_id;
                        try
                        {
                            webhost_id = Convert.ToInt32(userTree.ChildNodes.Where(t => t.TagName.Equals("school_uid")).Single().Value);
                        }
                        catch
                        {
                            WebhostEventLog.SchoologyLog.LogWarning("Skipping user with no school_uid. schoology id = {0}", userTree.ChildNodes.Where(t => t.TagName.Equals("id")).Single().Value);
                            Console.WriteLine("Skipping user with no school_uid. email: {0}", userTree.ChildNodes.Where(t => t.TagName.Equals("primary_email")).Single().Value);
                            continue;
                        }

                        int schoology_id = Convert.ToInt32(userTree.ChildNodes.Where(t => t.TagName.Equals("id")).Single().Value);
                        Faculty faculty = db.Faculties.Find(webhost_id);
                        if(faculty != null)
                        {
                            if(faculty.SchoologyId != schoology_id)
                            {
                                WebhostEventLog.SchoologyLog.LogInformation("Updating Schoology User Id for {0} {1} [{2}, {3}]", faculty.FirstName, faculty.LastName, faculty.ID, schoology_id);
                                Console.WriteLine("Updating Schoology User Id for {0} {1} [{2}, {3}]", faculty.FirstName, faculty.LastName, faculty.ID, schoology_id); 
                                faculty.SchoologyId = schoology_id;
                            }
                            else
                            {
                                Console.WriteLine("{0} {1} has their correct Schoology Id already.  schoology id:  {2}", faculty.FirstName, faculty.LastName, schoology_id);
                            }
                        }
                        else
                        {
                            Student student = db.Students.Find(webhost_id);
                            if (student == null) // unknown webhost_id?
                            {
                                WebhostEventLog.SchoologyLog.LogWarning("No Webhost user corresponds to email: {0}", userTree.ChildNodes.Where(t => t.TagName.Equals("primary_email")).Single().Value);
                                Console.WriteLine("No Webhost user corresponds to email: {0}", userTree.ChildNodes.Where(t => t.TagName.Equals("primary_email")).Single().Value);
                                continue;
                            }

                            if (student.SchoologyId != schoology_id)
                            {
                                WebhostEventLog.SchoologyLog.LogInformation("Updating Schoology User Id for {0} {1} [{2}, {3}]", student.FirstName, student.LastName, student.ID, schoology_id);
                                student.SchoologyId = schoology_id;
                            }
                            else
                            {
                                Console.WriteLine("{0} {1} already has their correct schoology id:  {2}", student.FirstName, student.LastName, schoology_id);
                            }
                        }

                        //Thread.Sleep(250);
                    }

                    db.SaveChanges();
                }
            }
        }

        static void CheckSectionRosters(int ay = 2017)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Section section in db.Sections.Where(s => s.Course.AcademicYearID == ay && s.Course.goesToSchoology).ToList())
                {
                    
                    Console.WriteLine("[{0}] {1}:", section.Block.LongName, section.Course.Name);

                    if(section.Course.SchoologyId <= 0)
                    {
                        // create course
                        using(SchoologyAPICall call = new SchoologyAPICall())
                        {
                            XMLTree response;
                            try
                            {
                                response = call.CreateItem(new SchoologyCourse(section.Course.id));
                            }
                            catch(XMLException e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                            section.Course.SchoologyId = Convert.ToInt32(response.ChildNodes.Where(n => n.TagName.Equals("id")).Single().Value);
                            WebhostEventLog.SchoologyLog.LogInformation("Created new Schoology Course {0} : {1}", section.Course.Name, section.Course.SchoologyId);
                            Console.WriteLine("Created new Schoology Course {0} : {1}", section.Course.Name, section.Course.SchoologyId);
                            db.SaveChanges();
                        }
                    }

                    if(section.SchoologyId <= 0)
                    {
                        if(section.Terms.Count == 0)
                        {
                            Console.WriteLine("This section has no Terms assigned.... I'm deleting it.");
                            WebhostEventLog.Syslog.LogInformation("Deleting section with no terms assigned [{0}] {1}", section.Block.LongName, section.Course.Name);
                            section.Students.Clear();
                            section.Teachers.Clear();
                            section.AttendanceSubmissionStatuses.Clear();
                            db.Sections.Remove(section);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch(Exception e)
                            {
                                String message = e.Message;
                                while(e.InnerException != null)
                                {
                                    e = e.InnerException;
                                    message = e.Message;
                                }
                                Console.WriteLine("Failed to save database changes. {0}", message);
                            }
                            continue;
                        }

                        using(SchoologyAPICall call = new SchoologyAPICall())
                        {
                            XMLTree response;
                            try
                            {
                                response = call.CreateItem(new SchoologySection(section.id));
                            }
                            catch(XMLException e)
                            {
                                Console.WriteLine(e.Message);
                                continue;
                            }
                            section.SchoologyId = Convert.ToInt32(response.ChildNodes.Where(n => n.TagName.Equals("id")).Single().Value);
                            WebhostEventLog.SchoologyLog.LogInformation("Created new Schoology Section [{0}] {1} : {2}", section.Block.LongName, section.Course.Name, section.SchoologyId);
                            Console.WriteLine("Created new Schoology Section [{0}] {1} : {2}", section.Block.LongName, section.Course.Name, section.SchoologyId);
                            db.SaveChanges();
                        }
                    }

                    if (section.Students.Count <= 0 || section.Teachers.Count <= 0)
                    {
                        Console.WriteLine("This section has no students or no teachers...");
                        section.Students.Clear();
                        section.Teachers.Clear();
                        section.Terms.Clear();
                        using (SchoologyAPICall call = new SchoologyAPICall())
                        {
                            call.DeleteSection(section.SchoologyId);
                            WebhostEventLog.SchoologyLog.LogInformation("Deleted superfluous section: [{0}] {1}", section.Block.LongName, section.Course.Name);
                            Console.WriteLine("Deleted Section from Schoology.");
                        }

                        section.SchoologyId = -1;

                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            Console.WriteLine("Failed to save changes to database.");
                        }
                        continue;
                    }

                    List<int> schoologyIds;
                    List<SchoologyEnrollment> enrollments;
                    try
                    {
                        using (SchoologyAPICall call = new SchoologyAPICall())
                            enrollments = call.GetSectionEnrollments(section.SchoologyId).ToList();

                        schoologyIds = enrollments.Select(u => u.user_id).ToList();
                    }
                    catch (SchoologyAPICall.SchoologyAPIException ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    List<int> webhostIds = section.Students.Select(s => s.ID).ToList();
                    webhostIds.AddRange(section.Teachers.Select(t => t.ID).ToList());

                    foreach (int id in schoologyIds)
                    {
                        if (!webhostIds.Contains(id))
                        {
                            String name;
                            Student student = db.Students.Find(id);
                            if (student == null)
                            {
                                Faculty teacher = db.Faculties.Find(id);
                                name = String.Format("{0} {1}", teacher.FirstName, teacher.LastName);
                            }
                            else
                            {
                                name = String.Format("{0} {1} [2]", student.FirstName, student.LastName);
                            }

                            Console.WriteLine("Schoology Section has extra enrollment for {0}.", name);
                            using (SchoologyAPICall call = new SchoologyAPICall())
                            {
                                call.DeleteEnrollment(section.SchoologyId, enrollments.Where(enr => enr.user_id == id).Select(enr => enr.enrollment_id).Single());
                            }
                        }
                    }

                    foreach (int id in webhostIds)
                    {
                        if (!schoologyIds.Contains(id))
                        {
                            String name;
                            Student student = db.Students.Find(id);
                            int schoologyId = 0;
                            if (student == null)
                            {
                                Faculty teacher = db.Faculties.Find(id);
                                schoologyId = teacher.SchoologyId;
                                name = String.Format("{0} {1}", teacher.FirstName, teacher.LastName);
                            }
                            else
                            {
                                name = String.Format("{0} {1} [2]", student.FirstName, student.LastName, student.GraduationYear);
                                schoologyId = student.SchoologyId;
                            }
                            Console.WriteLine("Schoology Section is missing enrollment for {0}.", name);
                            using (SchoologyAPICall call = new SchoologyAPICall())
                            {
                                try
                                {
                                    XMLTree tree = call.CreateEnrollment(section.SchoologyId, schoologyId, student == null);
                                }
                                catch(XMLException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                }
            }
        }

        static void PasswordLetter(String firstName, String lastName, String userName, String pwd)
        {
            NewFacultyAccountLetter letter = new NewFacultyAccountLetter()
            {
                FirstName = firstName,
                Email = String.Format("{0}@dublinschool.org", userName),
                Password = pwd,
                Title = string.Format("Welcome, {0} {1}", firstName, lastName)
            };

            Document doc = letter.Publish();
            String path = String.Format("C:\\Temp\\{0}.pdf", userName);
            
            doc.Save(path);
        }

        static void GetPreRegisteredStudentList()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<Student> students = new List<Student>();
                foreach(RegistrationRequest req in db.RegistrationRequests.ToList())
                {
                    if(!students.Contains(req.Student) && req.RequestCompleted)
                    {
                        students.Add(req.Student);
                    }
                }

                CSV csv = new CSV();
                foreach (Student student in students)
                {
                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"Last Name", student.LastName},
                        {"First Name", student.FirstName},
                        {"Class of", student.GraduationYear.ToString()}
                    };
                    csv.Add(row);
                }

                csv.Save(new FileStream("C:\\Temp\\preregistrations.csv", FileMode.CreateNew));
            }
        }

        static void PrintQuickReport(int studentId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Find(studentId);
                List<DateTime> classDays = GetClassDays(2016);
                Console.WriteLine("There were {0} class days recorded", classDays.Count);

                double classCount = 0;
                List<AttendanceMarking> attendances = new List<AttendanceMarking>();

                foreach(Term term in db.Terms.Where(t => t.AcademicYearID == 2016).ToList())
                {
                    classCount += student.Sections.Where(s => s.Terms.Contains(term) && s.Course.goesToSchoology).Count(); 
                    attendances.AddRange(student.AttendanceMarkings.Where(m => m.AttendanceDate >= term.StartDate && m.AttendanceDate <= term.EndDate).ToList());
                }

                classCount /= 3.0;

                Console.WriteLine("The student had {0} classes per term, for three terms.", classCount);

                Dictionary<DateTime, List<AttendanceMarking>> attendances_by_date = new Dictionary<DateTime, List<AttendanceMarking>>();
                foreach (AttendanceMarking mark in attendances)
                {
                    if (attendances_by_date.ContainsKey(mark.AttendanceDate))
                    {
                        attendances_by_date[mark.AttendanceDate].Add(mark);
                    }
                    else
                    {
                        attendances_by_date.Add(mark.AttendanceDate, new List<AttendanceMarking>() { mark });
                    }
                }

                int totalCuts = 0, totalPresent = 0, totalLates = 0, totalExcused = 0, totalOther = 0;

                foreach (DateTime date in attendances_by_date.Keys.OrderBy(d => d))
                {
                    int cuts = 0, present = 0, lates = 0, excused = 0, other = 0;

                    foreach (AttendanceMarking mark in attendances_by_date[date])
                    {
                        switch (mark.Marking.Name)
                        {
                            case "Cut": cuts++; break;
                            case "Late": lates++; break;
                            case "Present": present++; break;
                            case "Excused": excused++; break;
                            default: other++; break;
                        }
                    }


                    totalCuts += cuts;
                    totalPresent += present;
                    totalLates += lates;
                    totalExcused += excused;
                    totalOther += other;

                }

                Console.WriteLine("{0} was marked Cut {1} times and Excused {2} times out of a total of {3} classes.", student.FirstName, totalCuts, totalExcused, classCount * classDays.Count);
            }
        }

        static List<DateTime> GetClassDays(int academicYear)
        {
            List<DateTime> days = new List<DateTime>();
            using(WebhostEntities db = new WebhostEntities())
            {
                AcademicYear year = db.AcademicYears.Find(academicYear);
                DateTime start = year.Terms.OrderBy(t => t.StartDate).First().StartDate;
                DateTime end = year.Terms.OrderBy(t => t.EndDate).Last().EndDate;

                for (DateTime dt = start; dt <= end; dt = dt.AddDays(1))
                {
                    if(dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday) continue;

                    if(db.AttendanceMarkings.Where(at => at.AttendanceDate.Equals(dt)).Count() > 0)
                    {
                        days.Add(dt);
                    }
                }
            }
            return days;
        }

        static void GetRequestableCourses(int termId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                StreamWriter writer = new StreamWriter(new FileStream("C:\\Temp\\Courses.csv", FileMode.CreateNew));
                writer.WriteLine("CourseID,CourseName");
                foreach(RequestableCourse rc in db.RequestableCourses.Where(c => c.TermId == termId).ToList())
                {
                    writer.WriteLine("\"{0}\",\"{1}\"", rc.Course.BlackBaudID, rc.Course.Name);
                }
                writer.Flush();
                writer.Close();
            }
        }

        /*static void GeneratePerformanceReports(int academicYear)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<Faculty> teachers = db.Faculties.Where(f => 
                    f.Sections.Where(sec => 
                        sec.Course.AcademicYearID == academicYear
                    ).Count() > 0).OrderBy(f => f.LastName).ThenBy(f => f.FirstName).ToList();

                if(!Directory.Exists(@"C:\Temp\TeacherPerformanceReports"))
                {
                    Directory.CreateDirectory(@"C:\Temp\TeacherPerformanceReports");
                }

                List<TeacherPerformanceReport.ClassData> seniorElectiveData = new List<TeacherPerformanceReport.ClassData>();
                List<TeacherPerformanceReport.ClassData> nonSeniorElective = new List<TeacherPerformanceReport.ClassData>();

                foreach(Faculty teacher in teachers)
                {
                    Console.WriteLine("Generating Report for {0} {1}", teacher.FirstName, teacher.LastName);
                    String fileName = String.Format("C:\\Temp\\TeacherPerformanceReports\\{0}_{1}.pdf", teacher.LastName.ToLower(), teacher.FirstName.ToLower());
                    TeacherPerformanceReport report = new TeacherPerformanceReport(teacher.ID, academicYear, fileName);

                    foreach(TeacherPerformanceReport.ClassData cd in report.Classes)
                    {
                        if (cd.IsSeniorElective && cd.IsOneTerm)
                            seniorElectiveData.Add(cd);
                        else if (cd.IsOneTerm)
                            nonSeniorElective.Add(cd);
                    }

                    report.Publish();
                }

                List<TeacherPerformanceReport.Grade> seniorElectiveGrades = new List<TeacherPerformanceReport.Grade>();
                List<TeacherPerformanceReport.Grade> nonSeniorGrades = new List<TeacherPerformanceReport.Grade>();

                foreach(TeacherPerformanceReport.ClassData cd in seniorElectiveData)
                {
                    TeacherPerformanceReport.Grade grade = cd.AggregateGrades.ElementAt(cd.AggregateGrades.Count / 2);
                    if (!grade.Name.Equals("Pass") && !grade.Name.Equals("Fail") && !grade.Name.Equals("Incomplete") && !grade.Name.Equals("Not Applicable"))
                        seniorElectiveGrades.Add(grade);
                }
                foreach (TeacherPerformanceReport.ClassData cd in nonSeniorElective)
                {
                    nonSeniorGrades.Add(cd.AggregateGrades.ElementAt(cd.AggregateGrades.Count / 2));
                }

                seniorElectiveGrades = seniorElectiveGrades.OrderByDescending(g => g.Value).ToList();
                Console.WriteLine("Senior Elective Grade Median:  {0}", seniorElectiveGrades[seniorElectiveGrades.Count / 2].Name);
            }
        }*/

        static void FindWeirdComments(int termId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<CommentHeader> CommentsByClass = db.CommentHeaders.Where(h => h.TermIndex == termId).ToList();
                foreach(CommentHeader header in CommentsByClass)
                {
                    if(header.HTML.Equals("") || header.HTML.Equals("<p></p>"))
                    {
                        Console.WriteLine("[{0}] {1} has a blank header paragraph. HeaderId = {2}.", header.Section.Block.LongName, header.Section.Course.Name, header.id);
                    }

                    foreach(StudentComment comment in header.StudentComments.ToList())
                    {
                        if(comment.HTML.Equals("") || comment.HTML.Equals("<p></p>"))
                        {
                            Console.WriteLine("{0} {1} has a blank comment in [{2}] {3}.  comment_id={4}.", 
                                comment.Student.FirstName, comment.Student.LastName, header.Section.Block.LongName, header.Section.Course.Name, comment.id);
                        }
                    }
                }
            }
        }

        static void CleanUpComments(int termId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<CommentHeader> CommentsByClass = db.CommentHeaders.Where(h => h.TermIndex == termId).ToList();
                foreach(CommentHeader header in CommentsByClass)
                {
                    if(!header.Section.getsComments)
                    {
                        Console.WriteLine("[{0}] {1} has a header but should not.", header.Section.Block.LongName, header.Section.Course.Name);
                        Console.WriteLine("Not sure why this section has a comment letter setup.... Delete it? [y/N]  ");
                        if(Console.ReadKey().KeyChar.Equals('y'))
                        {
                            List<StudentComment> comms = header.StudentComments.ToList();
                            header.StudentComments.Clear();
                            foreach(StudentComment com in comms)
                            {
                                Console.WriteLine("Deleted comment for {0} {1} in {2}", com.Student.FirstName, com.Student.LastName, header.Section.Course.Name);
                                db.StudentComments.Remove(com);
                            }

                            db.CommentHeaders.Remove(header);
                            Console.WriteLine("Deleted Header.");

                            db.SaveChanges();
                        }
                    }

                    List<StudentComment> comments = header.StudentComments.ToList();
                    foreach (StudentComment comment in comments)
                    {
                        if(header.Section.Students.Where(s => s.ID == comment.StudentID).Count() <= 0)
                        {
                            Console.WriteLine("{0} {1} is not enrolled in [{2}] {3}.  Remove their comment? [y/N]  ",
                                comment.Student.FirstName, comment.Student.LastName, header.Section.Block.LongName, header.Section.Course.Name);
                            if(Console.ReadKey().KeyChar.Equals('y'))
                            {
                                Console.WriteLine("Deleted Comment.");
                                db.StudentComments.Remove(comment);
                            }
                        }
                    }

                    db.SaveChanges();
                }
            }
        }

        static void CleanUpCourseRequests(int studentId, int termId)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Find(studentId);
                List<int> reqCourseIds = new List<int>();
                bool cleanedUp = false;
                foreach (WebhostMySQLConnection.CourseRequest cr in student.CourseRequests.Where(c => c.TermId == termId).ToList())
                {
                    if (reqCourseIds.Contains(cr.CourseId))
                    {
                        cleanedUp = true;
                        Console.WriteLine("Deleting Duplicate Course Request for {0} {1} - {2}.", student.FirstName, student.LastName, cr.RequestableCourse.Course.Name);
                        db.CourseRequests.Remove(cr);
                    }
                    else
                    {
                        reqCourseIds.Add(cr.CourseId);
                    }
                }

                if (cleanedUp)
                {
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("No Changes to Save.");
                }
            }
        }

        static void FixLengthInTerms()
        {
            Console.WriteLine("Checking Course Length In Terms...");
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Section section in db.Sections.ToList())
                {
                    if(section.Terms.Count != section.Course.LengthInTerms)
                    {
                        Console.WriteLine();
                        Console.WriteLine("[{0}] {1} ({2}) is fucked up.  It has {3} terms, but the Course should only have {4}.",
                            section.Block.Name, section.Course.Name, section.Course.AcademicYearID, section.Terms.Count, section.Course.LengthInTerms);
                        Console.Write("Fix it or Ignore? [y/N]  ");
                        if(Console.ReadKey().KeyChar.Equals('y'))
                        {
                            Console.WriteLine();
                            section.Course.LengthInTerms = section.Terms.Count;
                            Console.WriteLine("Set {0} ({1}) length in terms to {2}.", section.Course.Name, section.Course.AcademicYearID, section.Course.LengthInTerms);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        static void GenerateStudentAttendanceReport()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                foreach(Student student in db.Students.Where(s => s.isActive).ToList())
                {
                    Console.WriteLine("Processing {0} {1}.", student.FirstName, student.LastName);
                    CSV csv = AttendanceYearReport(student.ID);

                    if (!Directory.Exists("C:\\Temp\\Student Attendance"))
                        Directory.CreateDirectory("C:\\Temp\\Student Attendance");

                    csv.Save(String.Format("C:\\Temp\\Student Attendance\\[{2}] {1}, {0}.csv", student.FirstName, student.LastName, student.GraduationYear));
                }

                Console.WriteLine("Done!!");
            }

            Console.ReadKey();
        }

        static void GenerateAllAttendanceDigest()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int tid = DateRange.GetCurrentOrLastTerm();
                Term term = db.Terms.Find(tid);
                foreach (Section section in term.Sections.ToList())
                {
                    CSV csv = ClassAttendanceStatisticsDigest(section.id);
                    String dept = section.Course.Department.Name;
                    if (!Directory.Exists(String.Format("C:\\Temp\\Attendance Stats\\{0}", dept)))
                        Directory.CreateDirectory(String.Format("C:\\Temp\\Attendance Stats\\{0}", dept));

                    String fileName = String.Format("{0}_{1}_[{2}].csv", section.Block.LongName.ToLower(), section.Course.Name.ToLower(), section.id);
                    Regex invalid = new Regex("[\\\\/:\\*\\?\"<>\\|]");
                    foreach (Match match in invalid.Matches(fileName))
                    {
                        fileName = fileName.Replace(match.Value, "");
                    }
                    csv.Save(String.Format("C:\\Temp\\Attendance Stats\\{0}\\{1}", dept, fileName));
                    Console.WriteLine("Done with {0}.", csv.Heading);
                }

                Console.WriteLine("Done with everything!");
                Console.ReadKey();
            }
        }
        
        static CSV AttendanceYearReport(int student_id)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int year = DateRange.GetCurrentAcademicYear();
                AcademicYear ay = db.AcademicYears.Find(year);
                Student student = db.Students.Find(student_id);
                CSV csv = new CSV(String.Format("{0} {1} Attendance Report", student.FirstName, student.LastName));

                List<AttendanceMarking> attendances = new List<AttendanceMarking>();
                foreach(Term term in ay.Terms.ToList())
                {
                    attendances.AddRange(student.AttendanceMarkings.Where(m => m.AttendanceDate >= term.StartDate && m.AttendanceDate <= term.EndDate).ToList());
                }

                Dictionary<DateTime, List<AttendanceMarking>> attendances_by_date = new Dictionary<DateTime, List<AttendanceMarking>>();
                foreach(AttendanceMarking mark in attendances)
                {
                    if(attendances_by_date.ContainsKey(mark.AttendanceDate))
                    {
                        attendances_by_date[mark.AttendanceDate].Add(mark);
                    }
                    else
                    {
                        attendances_by_date.Add(mark.AttendanceDate, new List<AttendanceMarking>() { mark });
                    }
                }

                int totalCuts = 0, totalPresent = 0, totalLates = 0, totalExcused = 0, totalOther = 0;

                foreach (DateTime date in attendances_by_date.Keys.OrderBy(d => d))
                {
                    int cuts = 0, present = 0, lates = 0, excused = 0, other = 0;

                    foreach(AttendanceMarking mark in attendances_by_date[date])
                    {
                        switch(mark.Marking.Name)
                        {
                            case "Cut": cuts++; break;
                            case "Late": lates++; break;
                            case "Present": present++; break;
                            case "Excused": excused++; break;
                            default: other++; break;
                        }
                    }


                    totalCuts += cuts;
                    totalPresent += present;
                    totalLates += lates;
                    totalExcused += excused;
                    totalOther += other;

                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"Date", date.ToString("ddd dd MMM yyyy")},
                        {"Present", present.ToString()},
                        {"Lates", lates.ToString()},
                        {"Cuts", cuts.ToString()},
                        {"Excused", excused.ToString()},
                        {"Total", (cuts + lates + present + excused).ToString()}
                    };

                    if(other > 0)
                    {
                        row.Add("Ambiguous Marking", other.ToString());
                    }

                    csv.Add(row);
                }

                Dictionary<String, String> totalRow = new Dictionary<string, string>()
                {
                    {"Date", "Totals"},
                    {"Present", totalPresent.ToString()},
                    {"Lates", totalLates.ToString()},
                    {"Cuts", totalCuts.ToString()},
                    {"Excused", totalExcused.ToString()},
                    {"Total", (totalPresent + totalLates + totalCuts + totalExcused).ToString()}
                };

                if(totalOther > 0)
                {
                    totalRow.Add("Ambiguous Marking", totalOther.ToString());
                }

                csv.Add(totalRow);

                return csv;
            }
        }

        static CSV ClassAttendanceStatisticsDigest(int section_id)
        {
            CSV csv = new CSV();
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Find(section_id);
                csv.Heading = String.Format("[{0}] {1}", section.Block.Name, section.Course.Name);

                foreach(Student student in section.Students.ToList())
                {
                    List<AttendanceMarking> attendances = section.AttendanceMarkings.Where(s => s.StudentID == student.ID).ToList();

                    int cuts = 0, lates = 0, present = 0, excused = 0, unknown = 0;
                    foreach(AttendanceMarking mark in attendances)
                    {
                        switch(mark.Marking.Name)
                        {
                            case "Cut": cuts++; break;
                            case "Late": lates++; break;
                            case "Present": present++; break;
                            case "Excused": excused++; break;
                            default: unknown++; break;
                        }
                    }

                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"First Name", student.FirstName},
                        {"Last Name", student.LastName},
                        {"Class of", student.GraduationYear.ToString()},
                        {"Present", present.ToString()},
                        {"Late", lates.ToString()},
                        {"Cut", cuts.ToString()},
                        {"Excused", excused.ToString()}
                    };
                    csv.Add(row);
                }
            }

            return csv;
        }

        static void ExcuseSeniors(DateRange seniorTrip)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Find(DateRange.GetCurrentOrLastTerm());
                List<Student> Juniors = db.Students.Where(s => s.isActive && s.GraduationYear == 2016).ToList();
                foreach (Student student in Juniors)
                {
                    List<int> classes = student.Sections.Where(sec => sec.Terms.Contains(term)).Select(sec => sec.id).ToList();
                    AttendanceControl.ExcuseStudent(student.ID, classes, "On the Senior Trip", ADUser.AttendanceBot, seniorTrip);
                    Console.WriteLine("Excused {0} {1} from {2} classes.", student.FirstName, student.LastName, classes.Count);
                }
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void ExcuseJuniors()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Find(DateRange.GetCurrentOrLastTerm());
                List<Student> Juniors = db.Students.Where(s => s.isActive && s.GraduationYear == 2017).ToList();
                foreach(Student student in Juniors)
                {
                    List<int> classes = student.Sections.Where(sec => sec.Terms.Contains(term)).Select(sec => sec.id).ToList();
                    AttendanceControl.ExcuseStudent(student.ID, classes, "On the Junior Trip", ADUser.AttendanceBot, DateTime.Today);
                    Console.WriteLine("Excused {0} {1} from {2} classes.", student.FirstName, student.LastName, classes.Count);
                }
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void ComplianceReport()
        {
            AttendanceControl.ComplianceReport().Save("C:\\Temp\\AttedanceCompliance.csv");
            using (WebhostEntities db = new WebhostEntities())
            {
                Faculty me = db.Faculties.Find(67);
                int studentCount = 0;
                foreach (Section section in me.Sections.Where(s => s.Course.AcademicYearID == 2016 && s.getsComments))
                {
                    Console.WriteLine("[{0}] {1} - {2}", section.Block.LongName, section.Course.Name, section.Students.Count);
                    studentCount += section.Students.Count;
                }

                Console.WriteLine("Total Students:  {0}", studentCount);
                Console.ReadKey();
            }
        }

        enum TermName
        {
            Fall,
            Winter,
            Spring
        }

        static CSV GetClassSizeDistribution(int academicYear, TermName term)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                String termName = term.Equals(TermName.Fall) ? "Fall" : term.Equals(TermName.Winter) ? "Winter" : "Spring";
                AcademicYear ay = db.AcademicYears.Where(a => a.id == academicYear).Single();
                if (ay.Terms.Count <= 0)
                    throw new ArgumentException(String.Format("No Terms for {0} {1}", termName, academicYear));
                Term theTerm = ay.Terms.Where(t => t.Name.Contains(termName)).Single();

                CSV output = new CSV();
                Dictionary<Faculty, List<Section>> FallTermData = new Dictionary<Faculty,List<Section>>();
                foreach(Section section in theTerm.Sections.ToList())
                {
                    foreach(Faculty teacher in section.Teachers.ToList())
                    {
                        if(FallTermData.ContainsKey(teacher))
                        {
                            FallTermData[teacher].Add(section);
                        }
                        else
                        {
                            FallTermData.Add(teacher, new List<Section>() { section });
                        }
                    }
                }

                List<Faculty> activeTeachers = FallTermData.Keys.ToList();
                foreach(Faculty faculty in activeTeachers)
                {
                    int studentCount = 0;
                    int smallClasses = 0;
                    int largeClasses = 0;
                    int largestClass = 0;
                    foreach(Section section in FallTermData[faculty])
                    {
                        int count = section.Students.Count / section.Teachers.Count;

                        if (count < 6) smallClasses++;
                        if (count > 12) largeClasses++;
                        if (count > largeClasses) largestClass = section.Students.Count;
                        studentCount += count;
                    }

                    int totalSections = FallTermData[faculty].Count;
                    int averageClassSize = studentCount/totalSections;

                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"First Name", faculty.FirstName},
                        {"Last Name", faculty.LastName},
                        {"Advisees", faculty.Students.Where(s => s.GraduationYear >= academicYear && s.isActive).Count().ToString()},
                        {"# Fall Sections", totalSections.ToString()},
                        {"Total Students", studentCount.ToString()},
                        {"Average Class Size", averageClassSize.ToString()},
                        {"Classes Less than 6 students", smallClasses.ToString()},
                        {"Classes Greater than 12 students", largeClasses.ToString()},
                        {"largest class", largestClass.ToString()}
                    };

                    output.Add(row);
                }
                return output;
            }
        }

        static void ForceGoogleCalendars()
        {
            using (GoogleCalendarCall call = new GoogleCalendarCall())
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    foreach (Faculty faculty in db.Faculties.Where(f => f.isActive).ToList())
                    {
                        if (faculty.UserName.Equals("jason")) continue;
                        Console.WriteLine("Checking {0} {1}", faculty.FirstName, faculty.LastName);
                        call.UpdateCalendarsForUser(faculty.ID, true);
                    }
                    foreach (Student student in db.Students.Where(s => s.isActive).ToList())
                    {
                        Console.WriteLine("Checking {0} {1}", student.FirstName, student.LastName);
                        call.UpdateCalendarsForUser(student.ID, false);
                    }
                }

            }
        }

        static void RandomlyAssignStudents(int activityId, int numberOfStudents)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<int> studentIds = db.Students.Where(s => s.GraduationYear > DateTime.Now.Year).Select(s => s.ID).ToList();
                Random rand = new Random();
                for(int i = 0; i< numberOfStudents; i++)
                {
                    int sid = studentIds[rand.Next(studentIds.Count)];
                    studentIds.Remove(sid);

                    StudentSignup newSignup = new StudentSignup()
                    {
                        ActivityId = activityId,
                        StudentId = sid,
                        IsBanned = false,
                        IsRescended = false,
                        TimeStamp = DateTime.Now.AddMinutes(rand.Next(30))
                    };
                    Console.WriteLine("Added {0} to {1}", sid, activityId);
                    db.StudentSignups.Add(newSignup);
                }

                db.SaveChanges();
            }
        }
       
    }
}