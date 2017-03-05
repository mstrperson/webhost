using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection
{
    public class SchoologyExport
    {
        #region Static Variable for Import types.
        //private static String usr_parent = "240231";
        private static String usr_admin = "240225";
        private static String usr_student = "240229";
        private static String usr_teacher = "240227";

        private static String enrl_student = "2";
        private static String enrl_teacher = "1";
        #endregion

        /// <summary>
        /// userColumns = { "First Name", "Last Name", "Username", "User Unique ID", "Grad Year", "Role" };
        /// </summary>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        public static CSV Users(int academicYear)
        {
            CSV csv = new CSV();
            using(WebhostEntities db = new WebhostEntities())
            {
                List<Student> activeStudents = db.Students.Where(st => st.Sections.Where(sec => sec.Course.AcademicYearID == academicYear).Count() > 0).ToList();
                List<Faculty> activeFaculty = db.Faculties.Where(fac => fac.Sections.Where(sec => sec.Course.AcademicYearID == academicYear).Count() > 0).ToList();

                foreach(Faculty faculty in activeFaculty)
                {
                    Dictionary<String, String> tdata = new Dictionary<string, string>();
                    tdata.Add("First Name", faculty.FirstName);
                    tdata.Add("Last Name", faculty.LastName);
                    tdata.Add("User name", faculty.UserName);
                    tdata.Add("E-Mail", string.Format("{0}@dublinschool.org", faculty.UserName));
                    tdata.Add("User Unique ID", Convert.ToString(faculty.ID));
                    tdata.Add("Role", Convert.ToString(faculty.SchoologyRoleId));
                    /*if (faculty.ID != 67)
                        tdata.Add("Role", usr_teacher);
                    else
                        tdata.Add("Role", usr_admin);
                    */
                    csv.Add(tdata);
                }

                foreach (Student student in activeStudents)
                {
                    Dictionary<String, String> tdata = new Dictionary<string, string>();
                    tdata.Add("First Name", student.FirstName);
                    tdata.Add("Last Name", student.LastName);
                    tdata.Add("User name", student.UserName);
                    tdata.Add("E-Mail", string.Format("{0}@dublinschool.org", student.UserName));
                    tdata.Add("User Unique ID", Convert.ToString(student.ID));
                    tdata.Add("Grad Year", Convert.ToString(student.GraduationYear));
                    tdata.Add("Role", usr_student);

                    csv.Add(tdata);
                }
            }

            return csv;
        }

        /// <summary>
        /// courseHeaders = { "Course Name", "Department Name", "Course Code", "Section Name", "Section Code" }
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public static CSV FullYearCourses(int termId)
        {
            CSV csv = new CSV();
            using(WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termId).Single();
                List<Section> currentSections = term.Sections.Where(sec => sec.Course.LengthInTerms == 3 && sec.Course.goesToSchoology).ToList();

                foreach(Section sec in currentSections)
                {
                    String TeachersNames = "";
                    bool first = true;
                    foreach (Faculty faculty in sec.Teachers.ToList())
                    {
                        TeachersNames += String.Format("{0}{1} {2}", first ? "" : ", ", faculty.FirstName, faculty.LastName);
                        first = false;
                    }
                    Dictionary<string, string> secdata = new Dictionary<string, string>();
                    secdata.Add("Course Name", sec.Course.Name);
                    secdata.Add("Department Name", sec.Course.Department.Name);
                    secdata.Add("Course Code", Convert.ToString(sec.CourseIndex));
                    secdata.Add("Section Name", String.Format("[{0}] {1}", sec.Block.LongName, TeachersNames));
                    secdata.Add("Section Code", Convert.ToString(sec.id));

                    csv.Add(secdata);
                }
            }
            return csv;
        }

        /// <summary>
        /// courseHeaders = { "Course Name", "Department Name", "Course Code", "Section Name", "Section Code" }
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public static CSV StartingTwoTermCourses(int termId)
        {
            CSV csv = new CSV();
            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termId).Single();
                List<Section> currentSections = term.Sections.Where(sec => sec.Course.LengthInTerms == 2 && sec.Terms.OrderBy(t => t.StartDate).First().id == termId && sec.Course.goesToSchoology).ToList();

                foreach (Section sec in currentSections)
                {
                    String TeachersNames = "";
                    bool first = true;
                    foreach (Faculty faculty in sec.Teachers.ToList())
                    {
                        TeachersNames += String.Format("{0}{1} {2}", first ? "" : ", ", faculty.FirstName, faculty.LastName);
                        first = false;
                    }
                    Dictionary<string, string> secdata = new Dictionary<string, string>();
                    secdata.Add("Course Name", sec.Course.Name);
                    secdata.Add("Department Name", sec.Course.Department.Name);
                    secdata.Add("Course Code", Convert.ToString(sec.CourseIndex));
                    secdata.Add("Section Name", String.Format("[{0}] {1}", sec.Block.LongName, TeachersNames));
                    secdata.Add("Section Code", Convert.ToString(sec.id));

                    csv.Add(secdata);
                }
            }
            return csv;
        }

        /// <summary>
        /// courseHeaders = { "Course Name", "Department Name", "Course Code", "Section Name", "Section Code" }
        /// </summary>
        /// <param name="termId"></param>
        /// <returns></returns>
        public static CSV TrimesterElectives(int termId)
        {
            CSV csv = new CSV();
            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termId).Single();
                List<Section> currentSections = term.Sections.Where(sec => sec.Course.LengthInTerms == 1 && sec.Course.goesToSchoology).ToList();

                foreach (Section sec in currentSections)
                {
                    String TeachersNames = "";
                    bool first = true;
                    foreach (Faculty faculty in sec.Teachers.ToList())
                    {
                        TeachersNames += String.Format("{0}{1} {2}", first ? "" : ", ", faculty.FirstName, faculty.LastName);
                        first = false;
                    }
                    Dictionary<string, string> secdata = new Dictionary<string, string>();
                    secdata.Add("Course Name", sec.Course.Name);
                    secdata.Add("Department Name", sec.Course.Department.Name);
                    secdata.Add("Course Code", Convert.ToString(sec.CourseIndex));
                    secdata.Add("Section Name", String.Format("[{0}] {1}", sec.Block.LongName, TeachersNames));
                    secdata.Add("Section Code", Convert.ToString(sec.id));

                    csv.Add(secdata);
                }
            }
            return csv;
        }

        public static CSV Enrollment(int termId)
        {
            CSV csv = new CSV();
            using (WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termId).Single();
                List<Section> currentSections = term.Sections.Where(sec => sec.Course.goesToSchoology).ToList();

                foreach (Section section in currentSections)
                {
                    //Department Head
                    /*Dictionary<String, String> dept = new Dictionary<string, string>()
                    {
                        {"Course Code", Convert.ToString(section.CourseIndex)},
                        {"Section Code", Convert.ToString(section.id)},
                        {"User Unique ID", Convert.ToString(section.Course.Department.DeptHeadId)},
                        {"Enrollment Type", enrl_teacher}
                    };

                    csv.Add(dept);*/

                    foreach(Faculty teacher in section.Teachers)
                    {
                        Dictionary<String, String> enr = new Dictionary<string, string>();
                        enr.Add("Course Code", Convert.ToString(section.CourseIndex));
                        enr.Add("Section Code", Convert.ToString(section.id));
                        enr.Add("User Unique ID", Convert.ToString(teacher.ID));
                        enr.Add("Enrollment Type", enrl_teacher);
                        enr.Add("Class Name", String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name));
                        enr.Add("Name", String.Format("{0} {1}", teacher.FirstName, teacher.LastName));
                        csv.Add(enr);
                    }
                    foreach(Student student in section.Students)
                    {
                        Dictionary<String, String> enr = new Dictionary<string, string>();
                        enr.Add("Course Code", Convert.ToString(section.CourseIndex));
                        enr.Add("Section Code", Convert.ToString(section.id));
                        enr.Add("User Unique ID", Convert.ToString(student.ID));
                        enr.Add("Enrollment Type", enrl_student);
                        enr.Add("Class Name", String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name));
                        enr.Add("Name", String.Format("{0} {1}", student.FirstName, student.LastName));

                        csv.Add(enr);
                    }
                }
            }
            return csv;
        }

        public static CSV AdvisorList(int termId)
        {
            /*
             * Brad
             * Erika
             * Holly
             * Sarah
             * Eric
             * Anne
             * Jill
             * emily cornell
             */

            List<String> adv_admins = new List<String>(new string[] { "bbates", "erogers", "hmacy", "sdoenmez", "smcfall", "amackey", "jhutchins", "ecornell", "rbeauzay" });

            CSV csv = new CSV();

            using (WebhostEntities db = new WebhostEntities())
            {
                foreach (Student student in db.Students.Where(st => st.isActive).ToList())
                {
                    Dictionary<String, String> row = new Dictionary<string, string>();
                    row.Add("StudentID", Convert.ToString(student.ID));
                    row.Add("AdvisorID", Convert.ToString(student.AdvisorID));
                    csv.Add(row);

                    foreach (String username in adv_admins)
                    {
                        Faculty teacher = db.Faculties.Where(t => t.UserName.Equals(username)).ToList().Single();
                        Dictionary<String, String> adm = new Dictionary<string, string>();
                        adm.Add("StudentID", Convert.ToString(student.ID));
                        adm.Add("AdvisorID", Convert.ToString(teacher.ID));
                        csv.Add(adm);
                    }

                    foreach (Section sec in student.Sections.Where(sec => sec.Terms.Where(t => t.id == termId).Count() > 0).ToList())
                    {
                        if (sec.Teachers.Count > 0 && (sec.Course.Name.Contains("Tutorial") || sec.Course.Name.Contains("Evening Assisted")))
                        {
                            Dictionary<String, String> tut = new Dictionary<string, string>();
                            tut.Add("StudentID", Convert.ToString(student.ID));
                            tut.Add("AdvisorID", Convert.ToString(sec.Teachers.FirstOrDefault().ID));
                            csv.Add(tut);
                        }
                    }
                }
                return csv;
            }
        }
    }
}
