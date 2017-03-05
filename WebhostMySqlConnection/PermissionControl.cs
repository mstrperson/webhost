using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection
{
    public class PermissionControl
    {
        public static Permission GetPermissionByName(String name, int AcademicYear = -1)
        {
            if (AcademicYear == -1) AcademicYear = DateRange.GetCurrentAcademicYear();
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.Permissions.Where(p => p.Name.Equals(name) && p.AcademicYear == AcademicYear).Count() <= 0) throw new InvalidCastException(String.Format("No Permission named {0}", name));

                return db.Permissions.Where(p => p.Name.Equals(name) && p.AcademicYear == AcademicYear).Single();
            }
        }

        public static List<int> GetProctors(int AcademicYear = -1)
        {
            using(WebhostEntities db = new WebhostEntities())
            { 
                int permid = GetPermissionByName("Proctors", AcademicYear).id;
                Permission proctorPermission = db.Permissions.Where(p => p.id == permid).Single();
                return proctorPermission.Students.Select(student => student.ID).ToList();
            }
        }
    }
}
