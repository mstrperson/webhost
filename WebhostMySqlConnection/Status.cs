using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection
{
    /// <summary>
    /// Check the status of various Webhost things.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Is a student Active?
        /// Checks the graduation year and the isActive flag.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public static bool IsActiveStudent(int studentId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if (db.Students.Where(s => s.ID == studentId).Count() <= 0) return false;

                Student student = db.Students.Where(s => s.ID == studentId).Single();
                return student.GraduationYear <= DateRange.GetCurrentAcademicYear() && student.isActive;
            }
        }

        public static bool IsActiveFaculty(int facultyId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if (db.Faculties.Where(f => f.ID == facultyId).Count() <= 0) return false;

                Faculty faculty = db.Faculties.Where(f => f.ID == facultyId).Single();
                return faculty.isActive;
            }
        }
    }
}
