using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WebhostMySQLConnection.SchoologyAPI
{
    [DataContract]
    public class SchoologyUser : ISchoologyCreatable
    {

        //private static String usr_parent = "240231";
        private static int usr_admin = 240225;
        private static int usr_student = 240229;
        private static int usr_teacher = 240227;

        public int id { get; set; }

        /// <summary>
        /// User's Webhost ID.
        /// </summary>
        [DataMember(IsRequired = true, Name = "school_uid")]
        public string school_uid { get; protected set; }

        /// <summary>
        /// First Name
        /// </summary>
        [DataMember(IsRequired = true, Name = "name_first")]
        public string name_first { get; protected set; }

        /// <summary>
        /// Last Name
        /// </summary>
        [DataMember(IsRequired = true, Name = "name_last")]
        public string name_last { get; protected set; }

        /// <summary>
        /// Preferred First Name.
        /// </summary>
        [DataMember(Name = "name_first_preferred")]
        public string name_first_preferred { get; protected set; }

        /// <summary>
        /// School Email Address
        /// </summary>
        [DataMember(IsRequired=true, Name="primary_email")]
        public string primary_email { get { return String.Format("{0}@dublinschool.org", username); } }

        /// <summary>
        /// Dublinschool.org Username.
        /// </summary>
        [DataMember(IsRequired=true, Name="username")]
        public string username { get; protected set; }

        /// <summary>
        /// Role ID in Schoology.
        /// </summary>
        [DataMember(IsRequired = true, Name = "role_id")]
        public int role_id { get; protected set; }

        /// <summary>
        /// Graduation Year (Students Only)
        /// </summary>
        [DataMember(Name="grad_year")]
        public int grad_year { get; protected set; }

        [DataMember(IsRequired = true, Name = "synced")]
        public readonly int synced = 1;

        public SchoologyUser(Student student)
        {
            school_uid = student.ID.ToString();
            name_first = student.FirstName;
            name_last = student.LastName;
            name_first_preferred = String.IsNullOrEmpty(student.PreferredName) ? student.FirstName : student.PreferredName;
            grad_year = student.GraduationYear;
            username = student.UserName;
            role_id = usr_student;
        }

        public SchoologyUser(Faculty teacher)
        {
            school_uid = teacher.ID.ToString();
            name_first = teacher.FirstName;
            name_last = teacher.LastName;
            username = teacher.UserName;
            role_id = teacher.SchoologyRoleId;
        }

        string ISchoologyCreatable.Resource
        {
            get
            {
                return "v1/users";
            }
            set
            {
                
            }
        }

        string ISchoologyCreatable.ToCreateJson()
        {
            return "{\n\t\"school_uid\": \"" + school_uid + 
                "\",\n\t\"role_id\": \"" + role_id + 
                "\",\n\t\"name_first\": \"" + name_first + 
                "\",\n\t\"name_first_preferred\": \"" + name_first_preferred + 
                "\",\n\t\"name_last\": \"" + name_last + 
                "\",\n\t\"primary_email\": \"" + primary_email + 
                "\",\n\t\"username\": \"" + username + 
                "\",\n}";
        }
    }
}
