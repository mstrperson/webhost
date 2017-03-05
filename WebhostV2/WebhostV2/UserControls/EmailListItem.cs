using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class EmailListItem
    {
        public String Text { get; protected set; }
        public String Value { get; protected set; }

        public EmailListItem(String DisplayName, String Email)
        {
            Text = DisplayName;
            Value = Email;
        }

        public static List<EmailListItem> GetDataSource(bool includeStudents, bool includeFaculty, bool activeOnly)
        {
            List<EmailListItem> items = new List<EmailListItem>();
            using(WebhostEntities db = new WebhostEntities())
            {
                if(includeFaculty)
                {
                    List<Faculty> allFaculty = db.Faculties.Where(f => !activeOnly || f.isActive).OrderBy(f => f.LastName).ThenBy(f => f.FirstName).ToList();
                    foreach(Faculty faculty in allFaculty)
                    {
                        items.Add(new EmailListItem(
                            String.Format("[Faculty] {0} {1}{2}", faculty.FirstName, faculty.LastName, faculty.isActive ? "" : " (inactive)"),
                            faculty.UserName
                            ));
                    }
                }

                if (includeStudents)
                {
                    List<Student> allStudents = db.Students.Where(f => !activeOnly || f.isActive).OrderBy(f => f.LastName).ThenBy(f => f.FirstName).ToList();
                    foreach (Student student in allStudents)
                    {
                        items.Add(new EmailListItem(
                            String.Format("[Student] {0} {1} ({2}){3}", student.FirstName, student.LastName, student.GraduationYear, student.isActive ? "" : " (inactive)"),
                            student.UserName
                            ));
                    }
                }
            }
            return items;
        }
    }
}