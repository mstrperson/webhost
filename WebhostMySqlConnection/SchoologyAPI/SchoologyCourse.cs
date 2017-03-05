using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.SchoologyAPI
{
    [DataContract]
    public class SchoologyCourse : ISchoologyCreatable
    {
        public String Resource
        {
            get
            {
                return "v1/courses";
            }
            set { }
        }

        [DataMember(IsRequired = true, Name = "title")]
        public String title { get; protected set; }

        [DataMember(IsRequired = true, Name = "course_code")]
        public String course_code { get; protected set; }

        [DataMember(IsRequired = true, Name = "department")]
        public String department { get; protected set; }

        [DataMember(Name = "id")]
        public int id { get; protected set; }

        public SchoologyCourse(int webhost_id)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Course course = db.Courses.Find(webhost_id);

                title = course.Name;
                course_code = Convert.ToString(course.id);
                department = course.Department.Name;
                id = course.SchoologyId;
            }
        }

        public String ToCreateJson()
        {
            return "{\n\t\"title\": \"" + title +"\",\n\t\"course_code\": \""+ course_code + "\",\n\t\"department\": \"" + department + "\",\n\t\"synced\": \"1\"\n}";
        }
    }
}
