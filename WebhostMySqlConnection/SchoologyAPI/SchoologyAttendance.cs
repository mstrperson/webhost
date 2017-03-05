using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.SchoologyAPI
{
    [DataContract]
    public struct AttendanceXMLData
    {
        /// <summary>
        /// Schoology Enrollment Id
        /// (Somehow related correlates to a user?
        /// </summary>
        [DataMember(IsRequired=true,Name="enrollment_id")]
        public String enrollment_id;

        /// <summary>
        /// YYYY-MM-DD format.
        /// </summary>
        [DataMember(IsRequired=true,Name="date")]
        public String date;

        [DataMember(IsRequired=true,Name="status")]
        public String status;

        [DataMember(Name="comment")]
        public String comment;
    };

    [DataContract]
    public class SchoologyAttendance
    {
        /// <summary>
        /// Determined by the SchoologyAPI.
        /// </summary>
        protected static Dictionary<String, String> StatusCodes = new Dictionary<string, string>() 
        { 
            { "1", "Present" }, 
            { "2", "Cut" }, 
            { "3", "Late" }, 
            { "4", "Excused" } 
        };

        [DataMember(IsRequired=true,Name="schoology_section_id")]
        public int schoology_section_id
        {
            get;
            protected set;
        }

        [DataMember(Name="enrollment", IsRequired=true)]
        public SchoologyEnrollment enrollment
        {
            get;
            protected set;
        }

        [DataMember(Name="date",IsRequired=true)]
        public DateTime date
        {
            get;
            protected set;
        }

        [DataMember(Name="marking",IsRequired=true)]
        public String AttendanceMarking
        {
            get;
            protected set;
        }

        [DataMember(Name="notes")]
        public String Notes
        {
            get;
            protected set;
        }

        new public String ToString()
        {
            return String.Format("Schoology Attendance: student_id={0} {1} on {2}", enrollment.user_id, AttendanceMarking, date.ToLongDateString());
        }

        /// <summary>
        /// Parse Data from the <attendance>...</attendance> tag of Attendance Data.
        /// </summary>
        public SchoologyAttendance(int section_id, XMLTree tree)
        {
            XMLTree AttendanceTree = tree;
            schoology_section_id = section_id;
            AttendanceXMLData rawData = new AttendanceXMLData()
            {
                enrollment_id = AttendanceTree.ChildNodes.Where(node => node.TagName.Equals("enrollment_id")).Single().Value,
                date = AttendanceTree.ChildNodes.Where(node => node.TagName.Equals("date")).Single().Value,
                status = AttendanceTree.ChildNodes.Where(node => node.TagName.Equals("status")).Single().Value,
                comment = AttendanceTree.ChildNodes.Where(node => node.TagName.Equals("comment")).Single().Value
            };

            enrollment = new SchoologyEnrollment(section_id, Convert.ToInt32(rawData.enrollment_id));
            String[] date_parts = rawData.date.Split('-');
            date = new DateTime(Convert.ToInt32(date_parts[0]), Convert.ToInt32(date_parts[1]), Convert.ToInt32(date_parts[2]));
            AttendanceMarking = StatusCodes[rawData.status];
            Notes = rawData.comment;
        }

        /// <summary>
        /// Download Attendance Data for a given section from schoology.
        /// </summary>
        /// <param name="sectionId">Schoology Section Id</param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static List<SchoologyAttendance> Download(int sectionId, DateRange period = null)
        {
            XMLTree tree;
            using (SchoologyAPICall call = new SchoologyAPICall())
            {
                tree = period == null?call.GetAttendances(sectionId):call.GetAttendances(sectionId, period);
            }
            List<SchoologyAttendance> list = new List<SchoologyAttendance>();
            tree = tree.Search("attendance");
            foreach(XMLTree attendanceTag in tree.ChildTrees)
            {
                list.Add(new SchoologyAttendance(sectionId, attendanceTag));
            }

            return list;
        }
    }
}
