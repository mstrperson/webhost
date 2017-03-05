using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection;

namespace WebhostMySQLConnection.Analytics
{
    /// <summary>
    /// Run Analytics on Gradebooks
    /// </summary>
    public class GradeAnalytics
    {
        protected class ClassGradeStats
        {
            protected int commentHeaderId;
            protected List<int> grades;

            public float Median
            {
                get
                {
                    if (grades.Count == 0) return 0f;
                    return ((float)grades[grades.Count / 2] / 100f);
                }
            }

            public float Average
            {
                get
                {
                    float avg = 0f;
                    for(int i = 0; i < grades.Count; i++)
                    {
                        avg += (float)grades[i] / 100f;
                    }

                    avg /= grades.Count;
                    return avg;
                }
            }

            public float Max
            {
                get
                {
                    if (grades.Count == 0) return 0f;
                    return grades.Last();
                }
            }

            public float Min
            {
                get
                {
                    if (grades.Count == 0) return 0f;
                    return grades.First();
                }
            }

            public int Count
            {
                get
                {
                    return grades.Count;
                }
            }
            
            public ClassGradeStats(int chid)
            {
                this.commentHeaderId = chid;
                using(WebhostEntities db = new WebhostEntities())
                {
                    CommentHeader header = db.CommentHeaders.Where(h => h.id == chid).Single();
                    List<int> grlst = new List<int>();
                    foreach(StudentComment comment in header.StudentComments)
                    {
                        grlst.Add(comment.FinalGrade.Value);
                    }

                    this.grades = grlst;
                    grlst.Sort();
                }
            }
        }

        public static CSV BySection(int termid)
        {
            CSV csv = new CSV();
            using(WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termid).Single();
                foreach (Section section in term.Sections.Where(sec => sec.CommentHeaders.Where(h => h.TermIndex == termid).Count() > 0).ToList())
                {
                    CommentHeader header = section.CommentHeaders.Where(c => c.TermIndex == termid).Single();
                    ClassGradeStats stats = new ClassGradeStats(header.id);

                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"Class", String.Format("[{0}-Block] {1}", section.Block.Name, section.Course.Name)},
                        {"Median", Convert.ToString(stats.Median)},
                        {"Average", Convert.ToString(stats.Average)},
                        {"Max", Convert.ToString(stats.Max)},
                        {"Min", Convert.ToString(stats.Min)}, 
                        {"Count", Convert.ToString(stats.Count)}
                    };

                    for(int i = 0; i < section.Teachers.Count; i++)
                    {
                        row.Add(String.Format("Teacher{0}", i == 0 ? "" : (" " + i)), String.Format("{0} {1}", section.Teachers.ToList()[i].FirstName, section.Teachers.ToList()[i].LastName));
                    }

                    csv.Add(row);
                }
            }
            return csv;
        }
    }
}
