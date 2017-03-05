using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;

namespace WebhostMySQLConnection.Publishing
{
    public class TeacherPerformanceReport : PDFPublisher
    {
        public String DisplayName { get; protected set; }
        public int AcademicYear { get; protected set; }
        public List<ClassData> Classes { get; protected set; }
        public int FullYearClasses { get; protected set; }
        public int TrimesterElectives { get; protected set; }
        public Grade MedianGrade { get; protected set; }
        public Grade MaxGrade { get; private set; }
        public Grade MinGrade { get; private set; }

        public class Grade
        {
            public String Name;
            public int Value;

            public Grade(GradeTableEntry gte)
            {
                Name = gte.Name;
                Value = gte.Value;
            }
        }

        public class ClassData
        {
            public string ClassName { get; protected set; }
            public bool IsOneTerm { get; protected set; }
            public bool IsSeniorElective { get; protected set; }
            public int TotalStudents { get; protected set; }

            public Dictionary<String, String> GradeData = new Dictionary<string, String>();
            public Dictionary<String, double> AttendanceData = new Dictionary<string, double>();
            public LinkedList<Grade> AggregateGrades
            {
                get;
                protected set;
            }

            public ClassData(int sectionId)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Section section = db.Sections.Find(sectionId);

                    ClassName = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name);
                    if (IsOneTerm = section.Terms.Count == 1)
                    {
                        ClassName += String.Format(" ({0})", section.Terms.ToList()[0].Name);
                    }

                    TotalStudents = section.Students.Count;

                    int seniorCount = 0;
                    foreach(Student student in section.Students.ToList())
                    {
                        if(student.GraduationYear == section.Course.AcademicYearID)
                        {
                            seniorCount++;
                        }
                    }

                    IsSeniorElective = seniorCount >= TotalStudents / 2;

                    double averageGrade = 0;
                    double maxGrade = 0;
                    double minGrade = 400;
                    double engagementAverage = 0;
                    int engagementCount = 0;
                    int gradeCount = 0;

                    AggregateGrades = new LinkedList<Grade>();

                    foreach(CommentHeader header in section.CommentHeaders.ToList())
                    {
                        foreach(StudentComment comment in header.StudentComments.ToList())
                        {
                            if(!comment.FinalGrade.Name.Equals("Not Applicable"))
                            {
                                gradeCount++;
                                averageGrade += comment.FinalGrade.Value;
                                if (comment.FinalGrade.Value > maxGrade)
                                {
                                    maxGrade = comment.FinalGrade.Value;
                                    AggregateGrades.AddFirst(new Grade(comment.FinalGrade));
                                }
                                else if (comment.FinalGrade.Value < minGrade)
                                {
                                    minGrade = comment.FinalGrade.Value;
                                    AggregateGrades.AddLast(new Grade(comment.FinalGrade));
                                }
                                else
                                {
                                    AggregateGrades.AddLast(new Grade(comment.FinalGrade));
                                    AggregateGrades = new LinkedList<Grade>(AggregateGrades.OrderByDescending(g => g.Value));
                                }
                            }

                            engagementCount++;
                            engagementAverage += comment.EffortGrade.Value;
                        }
                    }

                    if (gradeCount == 0) throw new InvalidOperationException("Class has no grades.");

                    averageGrade /= gradeCount;
                    engagementAverage /= engagementCount;

                    GradeData.Add("Average Grade", AggregateGrades.ElementAt(gradeCount/2).Name);
                    GradeData.Add("Max Grade", AggregateGrades.First.Value.Name);
                    GradeData.Add("Min Grade", AggregateGrades.Last.Value.Name);
                    GradeData.Add("Engagement Average", String.Format("{0}", engagementAverage));

                    double presentCount = 0;
                    double lateCount = 0;
                    double cutCount = 0;
                    double excusedCount = 0;
                    double totalAttendances = 0;

                    foreach(AttendanceMarking attendance in section.AttendanceMarkings.ToList())
                    {
                        totalAttendances++;
                        switch(attendance.Marking.Name)
                        {
                            case "Present": presentCount++; break;
                            case "Late": lateCount++; break;
                            case "Cut": cutCount++; break;
                            case "Excused": excusedCount++; break;
                        }
                    }

                    presentCount /= TotalStudents;
                    lateCount /= TotalStudents;
                    cutCount /= TotalStudents;
                    excusedCount /= TotalStudents;
                    totalAttendances /= TotalStudents;

                    AttendanceData.Add("Total Attendances", totalAttendances);
                    AttendanceData.Add("Present", presentCount);
                    AttendanceData.Add("Late", lateCount);
                    AttendanceData.Add("Cut", cutCount);
                    AttendanceData.Add("Excused", excusedCount);
                }
            }



            public Paragraph Paragraph
            {
                get
                {
                    String text = String.Format("{0}, {1} student(s).{10}" +
                                                "Grade Data:  {2:P} ({3:P} / {4:P}){10}"+
                                                "Total Attendances: {9} per student.{10}" +
                                                "Attendance: Present ({5}), Late ({6}), Cut ({7}), Excused ({8}){10}",
                                                ClassName, TotalStudents, 
                                                GradeData["Average Grade"], GradeData["Max Grade"], GradeData["Min Grade"],
                                                AttendanceData["Present"], AttendanceData["Late"], AttendanceData["Cut"], AttendanceData["Excused"],
                                                AttendanceData["Total Attendances"], Environment.NewLine);

                    return new Paragraph(text, NormalFont(12f));
                }
            }

        }

        public PdfPTable Header
        {
            get
            {
                String heading = String.Format("Performance Report for {0}", DisplayName);
                PdfPTable table = new PdfPTable(4);
                PdfPCell headingCell = new PdfPCell() { Colspan = 4, HorizontalAlignment = 1 };
                headingCell.AddElement(new Paragraph(heading, NormalFont(16f, Font.BOLD)));
                table.AddCell(headingCell);

                PdfPCell classLabel = new PdfPCell();
                classLabel.AddElement(new Paragraph("Full Year Classes:", NormalFont(12f)));
                table.AddCell(classLabel);

                PdfPCell fyc = new PdfPCell();
                fyc.AddElement(new Paragraph(String.Format("{0}", FullYearClasses), NormalFont(12f)));
                table.AddCell(fyc);

                PdfPCell trimLabel = new PdfPCell();
                trimLabel.AddElement(new Paragraph("Trimester Electives:", NormalFont(12f)));
                table.AddCell(trimLabel);

                PdfPCell tec = new PdfPCell();
                tec.AddElement(new Paragraph(String.Format("{0}", TrimesterElectives), NormalFont(12f)));
                table.AddCell(tec);

                if (MedianGrade != null)
                {
                    PdfPCell avgc = new PdfPCell();
                    avgc.AddElement(new Paragraph("Overall Grading Data", NormalFont(12f)));
                    table.AddCell(avgc);

                    PdfPCell gdc = new PdfPCell() { Colspan = 3 };
                    gdc.AddElement(new Paragraph(String.Format("{0}  ({1} / {2})", MedianGrade.Name, MaxGrade.Name, MinGrade.Name), NormalFont(12f)));
                    table.AddCell(gdc);
                }

                return table;
            }
        }

        public TeacherPerformanceReport(int teacherId, int academicYear, String fileName): base(fileName, "Performance Report")
        {
            List<int> sectionIds = new List<int>();
            using(WebhostEntities db = new WebhostEntities())
            {
                Faculty teacher = db.Faculties.Find(teacherId);
                DisplayName = String.Format("{0} {1}", teacher.FirstName, teacher.LastName);
                AcademicYear = academicYear;

                sectionIds = teacher.Sections.Where(s => s.Course.AcademicYearID == academicYear && s.CommentHeaders.Count > 0).Select(s => s.id).ToList();
            }

            Classes = new List<ClassData>();

            FullYearClasses = 0;
            TrimesterElectives = 0;

            LinkedList<Grade> AllGrades = new LinkedList<Grade>();

            foreach(int sectionId in sectionIds)
            {
                try
                {
                    ClassData theClass = new ClassData(sectionId);
                    if (theClass.IsOneTerm) TrimesterElectives++;
                    else FullYearClasses++;
                    Classes.Add(theClass);
                    foreach(Grade grade in theClass.AggregateGrades)
                    {
                        if(!grade.Name.Equals("Pass") && !grade.Name.Equals("Fail") && !grade.Name.Equals("Incomplete") && !grade.Name.Equals("Not Applicable"))
                            AllGrades.AddLast(grade);
                    }
                }
                catch(InvalidOperationException)
                {
                    // ignore.
                }
            }

            AllGrades = new LinkedList<Grade>(AllGrades.OrderByDescending(g => g.Value));

            if (AllGrades.Count > 0)
            {
                MedianGrade = AllGrades.ElementAt(AllGrades.Count / 2);
                MaxGrade = AllGrades.First.Value;
                MinGrade = AllGrades.Last.Value;
            }
        }

        new public String Publish()
        {
            document.Add(Header);
            foreach(ClassData classInfo in Classes)
            {
                document.Add(classInfo.Paragraph);
                document.Add(new Paragraph("___________________________", NormalFont(12f, Font.BOLD)));
            }

            return base.Publish();
        }
    }
}
