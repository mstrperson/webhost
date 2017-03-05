using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection.SchoologyAPI
{
    [DataContract]
    public class SchoologySection : ISchoologyCreatable
    {
        public String Resource { get; set; }

        [DataMember(IsRequired = true, Name = "title")]
        public String title { get; protected set; }

        [DataMember(IsRequired = true, Name = "section_school_code")]
        public String section_school_code { get; protected set; }

        [DataMember(IsRequired = true, Name = "grading_periods")]
        public String grading_periods { get; protected set; }

        [DataMember(Name = "id")]
        public int id { get; protected set; }

        public SchoologySection(int webhost_id)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Find(webhost_id);

                Resource = String.Format("v1/courses/{0}/sections", section.Course.SchoologyId);

                title = String.Format("[{0}]", section.Block.LongName);
                section_school_code = Convert.ToString(section.id);
                grading_periods = "[";
                bool first = true;
                foreach(Term term in section.Terms.ToList())
                {
                    grading_periods += String.Format("{0}{1}", first ? " " : ", ", term.SchoologyId);
                    first = false;
                }

                grading_periods += " ]";
                
                id = section.SchoologyId;
            }
        }

        public String ToCreateJson()
        {
            return "{\n\t\"title\": \""+title+"\",\n\t\"section_school_code\": \""+ section_school_code +"\",\n\t\"grading_periods\": \""+grading_periods+"\",\n\t\"synced\": \"1\"\n}";
        }
    }
}
