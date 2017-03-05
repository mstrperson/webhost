using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.SchoologyAPI
{
    [DataContract]
    public class SchoologyEnrollment
    {
        /// <summary>
        /// Schoology Enrollment Id.
        /// Each Section/User combination has a unique enrollment_id.
        /// </summary>
        [DataMember(IsRequired=true, Name="enrollment_id")]
        public int enrollment_id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Webhost User.ID (Either Faculty or Student).
        /// </summary>
        [DataMember(IsRequired=true, Name="user_id")]
        public int user_id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Get a Schoology Enrollment and map it to a Webhost User Id.
        /// </summary>
        /// <param name="section_id">Schoology Section Id</param>
        /// <param name="enrollment_id">Schoology Enrollment Id</param>
        public SchoologyEnrollment(int section_id, int enrollment_id)
        {
            XMLTree tree;
            using (SchoologyAPICall call = new SchoologyAPICall())
            {
                try
                {
                    tree = call.GetEnrollmentXML(section_id, enrollment_id);
                }
                catch(XMLException)
                {
                    this.enrollment_id = enrollment_id;
                    user_id = -1;
                    return;
                }
            }
            
            this.enrollment_id = enrollment_id;
            user_id = Convert.ToInt32(tree.ChildNodes.Where(node => node.TagName.Equals("school_uid")).Single().Value);
        }

        [DataContract]
        public class CreateEnrollment: ISchoologyCreatable
        {
            public String Resource { get; set; }
            [DataMember(IsRequired=true, Name="uid")]
            public int uid { get; set; }

            [DataMember(IsRequired=true,Name="admin")]
            public bool admin { get; set; }

            [DataMember(IsRequired=true, Name="status")]
            public int status { get { return 1; } set { } }

            public String ToCreateJson()
            {
                return "{\"uid\":\"" + uid + "\",\"admin\":\"" + (admin ? "1" : "0") + "\",\"status\":\"1\"}";
            }
        }
    }
}
