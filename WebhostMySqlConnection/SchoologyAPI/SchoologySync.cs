using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.SchoologyAPI
{
    public class SchoologySync
    {
        #region Course Syncing
        protected static String CourseToXML(Course course)
        {
            return String.Format("<body>\r\n\t<title>{0}</title>\r\n\t<course_code>{1}</course_code>\r\n\t<department>{2}</department>\r\n\t<description></description>\r\n</body>",
                                        course.Name, course.id, course.Department.Name);
        }

        /// <summary>
        /// Encapsulate XML response from Schoology querrying courses.
        /// </summary>
        protected class SchoologyCourseResponse
        {
            public XMLTree xml
            {
                get;
                protected set;
            }

            public string CourseName
            {
                get
                {
                    return xml.ChildNodes.Where(n => n.TagName.Equals("title")).Single().Value;
                }
            }

            public string Department
            {
                get
                {
                    return xml.ChildNodes.Where(n => n.TagName.Equals("department")).Single().Value;
                }
            }

            public int SchoologyId
            {
                get
                {
                    return Convert.ToInt32(xml.ChildNodes.Where(n => n.TagName.Equals("id")).Single().Value);
                }
            }

            public int WebhostId
            {
                get
                {
                    try
                    {
                        return Convert.ToInt32(xml.ChildNodes.Where(n => n.TagName.Equals("course_code")).Single().Value);
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }

            public SchoologyCourseResponse(String xmlstr)
            {
                xml = new XMLTree(xmlstr);
            }

            public SchoologyCourseResponse(XMLTree tree)
            {
                if (!tree.TagName.Equals("course")) throw new FormatException("Xml is not a <course> tag.");
                xml = tree;
            }
        }

        /// <summary>
        /// Get Course Data from Schoology to populate the SchoologyId Field.
        /// </summary>
        /// <returns>Report suitable for printing to logs.</returns>
        public static String GetCoursesFromSchoology()
        {
            XMLTree tree;
            String report = "Getting courses from schoology...\r\n";
            WebhostEventLog.SchoologyLog.LogInformation("Getting courses from Schoology...");
            using (SchoologyAPICall call = new SchoologyAPICall())
            {
                tree = call.GetCourses();
            }

            List<SchoologyCourseResponse> schoologyCourses = new List<SchoologyCourseResponse>();

            foreach (XMLTree courseTree in tree.ChildTrees.Where(tr => tr.TagName.Equals("course")))
            {
                SchoologyCourseResponse resp = new SchoologyCourseResponse(courseTree);
                if (resp.WebhostId != -1)
                {
                    schoologyCourses.Add(new SchoologyCourseResponse(courseTree));
                }
                else
                {
                    report += String.Format("Disregarding Schoology Course with no Webhost Course Code:\r\n{0}\r\n", courseTree.ToString());
                    WebhostEventLog.SchoologyLog.LogWarning("Disregarding Schoology Course with no Webhost Course Code:\r\n{0}", courseTree.ToString());
                }
            }

            using (WebhostEntities db = new WebhostEntities())
            {
                foreach (SchoologyCourseResponse sc in schoologyCourses)
                {
                    int cid = sc.WebhostId;
                    if (db.Courses.Where(c => c.id == cid).Count() <= 0)
                    {
                        // put a placeholder in WebhostDB so to prevent conflicts.
                        report += String.Format("Creating Placholder Course to prevent conflicts with \r\n{0}\r\n", sc.xml);
                        WebhostEventLog.SchoologyLog.LogWarning("Creating Placholder Course to prevent conflicts with \r\n{0}", sc.xml);
                        Course newCourse = new Course()
                        {
                            id = cid,
                            BlackBaudID = "DEL",
                            AcademicYearID = 2014,
                            DepartmentID = sc.Department.Equals("") ? 0 : db.Departments.Where(d => d.Name.Equals(sc.Department)).Single().id,
                            goesToSchoology = true,
                            Name = sc.CourseName,
                            SchoologyId = sc.SchoologyId,
                            LengthInTerms = 0
                        };

                        db.Courses.Add(newCourse);
                    }
                    else
                    {
                        Course course = db.Courses.Where(c => c.id == cid).Single();
                        report += String.Format("Updating reference to {0} in Webhost with Schoology UID: {1}\r\n", course.Name, sc.SchoologyId);
                        WebhostEventLog.SchoologyLog.LogInformation("Updating reference to {0} in Webhost with Schoology UID: {1}", course.Name, sc.SchoologyId);
                        course.goesToSchoology = true;
                        course.SchoologyId = sc.SchoologyId;
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    throw e;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException e)
                {
                    throw e;
                }
            }
            report += "Done!";
            return report;
        }

        #endregion Courses...

        #region Sections

        protected class SchoologySectionResponse
        {
            public XMLTree xml
            {
                get;
                protected set;
            }

            public int WebhostId
            {
                get
                {
                    try
                    {
                        return Convert.ToInt32(xml.ChildNodes.Where(n => n.TagName.Equals("section_code")).Single().Value);
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }

            public int SchoologyId
            {
                get
                {
                    try
                    {
                        return Convert.ToInt32(xml.ChildNodes.Where(n => n.TagName.Equals("id")).Single().Value);
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }

            public SchoologySectionResponse(String xmlstr)
            {
                xml = new XMLTree(xmlstr);
            }

            public SchoologySectionResponse(XMLTree tree)
            {
                if (!tree.TagName.Equals("section")) throw new XMLException("XMLTree is not a <section> tag.");
                xml = tree;
            }
        }

        public static String GetSchoologySectionIdsForTerm(int TermId)
        {
            String report = "Getting Section Ides for term " + TermId + "\r\n";
            WebhostEventLog.SchoologyLog.LogInformation("Getting Section Ides for term {0}", TermId);
            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == TermId).Single();

                List<Section> sections = term.Sections.ToList();
                report += String.Format("Found {0} sections for {1} {2}\r\n", sections.Count, term.Name, term.StartDate.Year);
                WebhostEventLog.SchoologyLog.LogInformation("Found {0} sections for {1} {2}", sections.Count, term.Name, term.StartDate.Year);

                List<Course> courses = new List<Course>();
                foreach(Section section in sections)
                {
                    if (section.Course.goesToSchoology && !courses.Contains(section.Course))
                        courses.Add(section.Course);
                }

                report += String.Format("Found {0} courses for {1} {2}\r\n", courses.Count, term.Name, term.StartDate.Year);
                WebhostEventLog.SchoologyLog.LogInformation("Found {0} courses for {1} {2}", courses.Count, term.Name, term.StartDate.Year);
                foreach(Course course in courses)
                {
                    if(course.SchoologyId <= 0)
                    {
                        report += String.Format("Course {0} webhost_id={1} schoology_id={2} cannot be querried.\r\n", course.Name, course.id, course.SchoologyId);
                        WebhostEventLog.SchoologyLog.LogWarning("Course {0} webhost_id={1} schoology_id={2} cannot be querried.", course.Name, course.id, course.SchoologyId);
                        continue;
                    }

                    report += String.Format("Querrying Schoology for course_id={0}\r\n", course.SchoologyId);
                    WebhostEventLog.SchoologyLog.LogInformation("Querrying Schoology for course_id={0}", course.SchoologyId);
                    XMLTree sectionsTree;
                    using(SchoologyAPICall call = new SchoologyAPICall())
                    {
                        sectionsTree = call.GetSectionsOf(course.SchoologyId);
                    }
                    
                    foreach(XMLTree sectionTree in sectionsTree.ChildTrees.Where(t => t.TagName.Equals("section")))
                    {
                        SchoologySectionResponse resp = new SchoologySectionResponse(sectionTree);
                        if(resp.WebhostId == -1)
                        {
                            report += String.Format("Section does not have a Webhost ID:\r\n{0}\r\n", sectionTree.ToString());
                            WebhostEventLog.SchoologyLog.LogError("Section does not have a Webhost ID:\r\n{0}", sectionTree.ToString());
                            continue;
                        }
                        try
                        {
                            Section section = db.Sections.Where(sec => sec.id == resp.WebhostId).Single();
                            section.SchoologyId = resp.SchoologyId;
                            report += String.Format("Updated [{0}] {1} with schoology_id={2}\r\n", section.Block.LongName, section.Course.Name, resp.SchoologyId);
                            WebhostEventLog.SchoologyLog.LogInformation("Updated [{0}] {1} with schoology_id={2}", section.Block.LongName, section.Course.Name, resp.SchoologyId);
                        }
                        catch(Exception e)
                        {
                            report += String.Format("Failed to locate section with Id {0}.  Error:  {1}\r\n", resp.WebhostId, e.Message);
                            WebhostEventLog.SchoologyLog.LogError("Failed to locate section with Id {0}.  Error:  {1}\r\n", resp.WebhostId, e.Message);
                        }

                    }
                }

                db.SaveChanges();
            }
            return report;
        }

        #endregion

        #region Attendance

        public static String SyncAttendanceForSection(int webhost_section_id, DateRange period = null)
        {
            String report = "";

            using (WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Where(sec => sec.id == webhost_section_id).Single();
                int schoology_section_id = section.SchoologyId;
                int attmarkid = db.AttendanceMarkings.Count() > 0 ? db.AttendanceMarkings.OrderBy(att => att.id).ToList().Last().id : 0;
                List<SchoologyAttendance> schoologyAttendances = SchoologyAttendance.Download(schoology_section_id, period);
                report += String.Format("Found {0} Attendance Markings{1}", schoologyAttendances.Count, Environment.NewLine);
                foreach (SchoologyAttendance sch_att in schoologyAttendances)
                {
                    AttendanceMarking marking;
                    if (section.AttendanceMarkings.Where(mk =>
                        mk.AttendanceDate.Date.Equals(sch_att.date) && mk.StudentID == sch_att.enrollment.user_id).Count() <= 0)
                    {
                        marking = new AttendanceMarking()
                        {
                            id = ++attmarkid,
                            AttendanceDate = sch_att.date,
                            StudentID = sch_att.enrollment.user_id,
                            SectionIndex = webhost_section_id,
                            MarkingIndex = AttendanceControl.LookupAttendanceIdByName(sch_att.AttendanceMarking),
                            Notes = sch_att.Notes,
                            SubmissionTime = DateTime.Now,
                            SubmittedBy = 9997
                        };

                        db.AttendanceMarkings.Add(marking);
                        Student student = db.Students.Where(s => s.ID == sch_att.enrollment.user_id).Single();
                        GradeTableEntry entry = db.GradeTableEntries.Where(e => e.id == marking.MarkingIndex).Single();
                        report += String.Format("Created new attendance [{3}] for {0} {1} on {2}{4}", student.FirstName, student.LastName, marking.AttendanceDate.ToShortDateString(), entry.Name, Environment.NewLine);

                    }
                    else
                    {
                        marking = section.AttendanceMarkings.Where(mk =>
                                            mk.AttendanceDate.Date.Equals(sch_att.date) && mk.StudentID == sch_att.enrollment.user_id).Single();
                        if (marking.MarkingIndex == AttendanceControl.LookupAttendanceIdByName("Excused"))
                        {
                            report += String.Format("Skipping previously excused absence for {0} {1} on {3}{2}", marking.Student.FirstName, marking.Student.LastName, Environment.NewLine, marking.AttendanceDate.ToShortDateString());
                            continue; // Do not overwrite an excused absence.
                        }
                        marking.MarkingIndex = AttendanceControl.LookupAttendanceIdByName(sch_att.AttendanceMarking);
                        marking.Notes = sch_att.Notes;
                        marking.SubmissionTime = DateTime.Now;

                        report += String.Format("Updated attendance [{4}] for {0} {1} on {3}{2}", marking.Student.FirstName, marking.Student.LastName, Environment.NewLine, marking.AttendanceDate.ToShortDateString(), marking.Marking.Name);
                    }
                }

                db.SaveChanges();
            }

            return report;
        }

        #endregion

    }
}
