using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebhostMySQLConnection
{
    public class GradeCalculation
    {
        [DataContract(Name ="grade")]
        public class Grade
        {
            [DataMember(Name ="value")]
            public float Value { get; set; }

            [DataMember(Name ="weight")]
            public float CreditWeight { get; set; }

            [DataMember(Name ="display")]
            public String DisplayName { get; set; }

            public void CombineGradePoints(Grade other)
            {
                Value = (this.Value + other.Value) / 2f;
                CreditWeight += other.CreditWeight;
                DisplayName += ", " + other.DisplayName;
            }
        }

        [DataContract(Name ="grade_report")]
        public class GradeReport
        {
            [DataMember(IsRequired = true, Name ="student_id")]
            public int StudentId { get; protected set; }

            [DataMember(Name ="gpa")]
            public float GPA { get; protected set; }

            [DataMember(Name ="credit_data")]
            public Dictionary<int, Grade> Grades { get; protected set; }

            [DataMember(Name ="total_credits")]
            public float TotalCredits { get; protected set; }

            public GradeReport(int studentId)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    Student student = db.Students.Find(studentId);
                    if (student == null) throw new InvalidOperationException("No Student with ID:  " + studentId);

                    StudentId = studentId;
                    Grades = new Dictionary<int, Grade>();

                    foreach (StudentComment comment in 
                        student.StudentComments.Where(com => 
                            !com.FinalGrade.Name.Equals("Not Applicable") && 
                            !com.FinalGrade.Name.Equals("Pass") && 
                            !com.FinalGrade.Name.Equals("Fail")))
                    {
                        Grades.Add(comment.CommentHeader.Section.id, new Grade()
                        {
                            CreditWeight = comment.CommentHeader.Section.Course.LengthInTerms / 3f,
                            DisplayName = comment.FinalGrade.Name,
                            Value = comment.FinalGrade.Value / 100f
                        });
                    }

                    Dictionary<int, Grade> temp = new Dictionary<int, Grade>();

                    foreach(StudentComment comment in student.StudentComments.Where(com =>
                            com.FinalGrade.Name.Equals("Not Applicable") 
                            && !com.TermGrade.Name.Equals("Pass")  
                            && !com.TermGrade.Name.Equals("Fail") 
                            && !com.TermGrade.Name.Equals("Not Applicable")))
                    {
                        if (Grades.ContainsKey(comment.CommentHeader.SectionIndex))
                            continue; // already got the final grade from this section.

                        if(temp.ContainsKey(comment.CommentHeader.SectionIndex))
                        {
                            temp[comment.CommentHeader.SectionIndex].CombineGradePoints(new Grade() { CreditWeight = 1f / 3f, Value = comment.TermGrade.Value/100f, DisplayName = comment.TermGrade.Name });
                        }

                        else
                        {
                            temp.Add(comment.CommentHeader.SectionIndex, new Grade() { CreditWeight = 1f / 3f, Value = comment.TermGrade.Value/100f, DisplayName=comment.TermGrade.Name });
                        }
                    }

                    foreach(int key in temp.Keys)
                    {
                        Grades.Add(key, temp[key]);
                    }

                    // Calculate GPA
                    TotalCredits = 0f;
                    float totalGradePoints = 0f;

                    foreach(int sectionId in Grades.Keys)
                    {
                        TotalCredits += Grades[sectionId].CreditWeight;
                        totalGradePoints += Grades[sectionId].Value * Grades[sectionId].CreditWeight;
                    }

                    GPA = totalGradePoints / TotalCredits;
                }
            }
        }
    }
}
