using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace WebhostMySQLConnection
{
    public class CreditAudit
    {

        private static String Credit_AcademicYear = "Academic year";
        private static String Credit_StudentId = "Student ID";
        private static String Credit_FirstName = "First name";
        private static String Credit_LastName = "Last name";
        private static String Credit_CourseName = "Course name";
        private static String Credit_Department = "Department";
        private static String Credit_Term = "Term";

        private static float EnglishCreditsRequired = 4f;
        private static float MathSciCreditsRequired = 7f;
        private static float MathSciCreditsHonors = 8f;
        private static float HistoryWLCreditsRequired = 6f;
        private static float HistoryOnlyCreditsRequired = 3f;
        private static float TechnologyCreditsRequired = (float)Math.Round(1f / 3f, 3);
        private static float ArtsCreditsRequired = 2f;
        private static float HistoryWLCreditsHonors = (float)Math.Round(7f + (1f / 3f));
        private static float HistoryOnlyCreditsHonors = (float)Math.Round(3f + (1f / 3f));

        private static List<String> RequiredScienceCourses { get { return new List<string>() { "Physical Science", "Biology", "Chemistry" }; } }
        private static List<String> RequiredHistoryCources { get { return new List<string>() { "US History" }; } }

        public static Dictionary<String, String> GetGraduationRequirementStatuses(int studentId)
        {
            return new Dictionary<string, string>()
            {
                {"English", EnglishRequirementStatus(studentId)},
                {"History / World Languages", HistoryWorldLanguageRequirementStatus(studentId)},
                {"Math / Science", ScienceMathRequirementStatus(studentId)},
                {"Technology", TechnologyRequirementStatus(studentId)},
                {"Arts", ArtsRequirementStatus(studentId)}
            };
        }

        public static String EnglishRequirementStatus(int studentId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<Credit> credits = db.Credits.Where(c => c.StudentId == studentId && c.CreditType.Name.Equals("English")).ToList();
                float value = 0f;
                foreach(Credit credit in credits)
                {
                    value += credit.CreditValue.Value / 9f;
                }

                return EnglishCreditsRequired <= (float)Math.Round(value, 3) ? "Credit Fulfilled" : String.Format("Incomplete {0} of {1}", value, EnglishCreditsRequired);
            }
        }

        public static String ArtsRequirementStatus(int studentId)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<Credit> credits = db.Credits.Where(c => c.StudentId == studentId && c.CreditType.Name.Equals("Art")).ToList();
                float value = 0f;
                foreach (Credit credit in credits)
                {
                    value += credit.CreditValue.Value / 9f;
                }

                return ArtsCreditsRequired <= (float)Math.Round(value, 3) ? "Credit Fulfilled" : String.Format("Incomplete {0} of {1}", value, ArtsCreditsRequired);
            }
        }

        public static String TechnologyRequirementStatus(int studentId)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<Credit> credits = db.Credits.Where(c => c.StudentId == studentId && c.CreditType.Name.Equals("Technology")).ToList();
                float value = 0f;
                foreach (Credit credit in credits)
                {
                    value += credit.CreditValue.Value / 9f;
                }

                return TechnologyCreditsRequired <= (float)Math.Round(value, 3) ? "Credit Fulfilled" : String.Format("Incomplete {0} of {1}", value, TechnologyCreditsRequired);
            }
        }

        public static String HistoryWorldLanguageRequirementStatus(int studentId)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<Credit> historyCredits = db.Credits.Where(c => c.StudentId == studentId && (c.CreditType.Name.Equals("History"))).ToList();
                List<Credit> worldLanguageCredits = db.Credits.Where(c => c.StudentId == studentId && (c.CreditType.Name.Equals("World Languages"))).ToList();
                float histvalue = 0f;
                foreach (Credit credit in historyCredits)
                {
                    histvalue += credit.CreditValue.Value / 9f;
                }

                bool hasLanguageWaiver = worldLanguageCredits.Where(c => c.Notes.Contains("Waiver")).Count() > 0;

                float wlvalue = 0f;
                Regex languages = new Regex("French|Spanish|Latin|Mandarin");
                Dictionary<String, float> languageCounts = new Dictionary<string, float>();

                foreach(Credit credit in worldLanguageCredits)
                {
                    wlvalue += credit.CreditValue.Value / 9f;

                    String sectionName = credit.Notes;
                    if(credit.Sections.Count > 0)
                    {
                        sectionName += credit.Sections.Single().Course.Name;
                    }

                    if (languages.IsMatch(sectionName))
                    {
                        if (languageCounts.ContainsKey(languages.Match(sectionName).Value))
                        {
                            languageCounts[languages.Match(sectionName).Value] += credit.CreditValue.Value / 9f;
                        }
                        else
                        {
                            languageCounts.Add(languages.Match(sectionName).Value, credit.CreditValue.Value / 9f);
                        }
                    }
                }

                float value = histvalue + wlvalue;

                List<String> Required = new List<string>(RequiredHistoryCources);

                foreach (String course in RequiredHistoryCources)
                {
                    if (historyCredits.Where(c => c.Sections.Where(s => s.Course.Name.Contains(course)).Count() > 0).Count() > 0 || historyCredits.Where(c => c.Notes.Contains(course)).Count() > 0)
                    {
                        Required.Remove(course);
                    }
                }

                if (Required.Count > 0)
                {
                    String missing = "Missing: ";
                    foreach (String course in Required)
                    {
                        missing += String.Format("{0} ", course);
                    }

                    return missing;
                }

                String response = "";

                if (histvalue >= (hasLanguageWaiver?4f:HistoryOnlyCreditsHonors))
                    response = "History Honors Fulfilled, ";
                else if (histvalue >= (hasLanguageWaiver?4f:HistoryOnlyCreditsRequired))
                    response = "History Requirement Fulfilled, ";
                else
                    response = "History Requirement Incomplete, ";

                if (hasLanguageWaiver) response += "Language Waiver";
                else if (worldLanguageCredits.Count < 2) response += "World Languages Requirement Incomplete";
                else
                {
                    if (languageCounts.Keys.Count == 1 &&
                        (float)Math.Round(languageCounts[languageCounts.Keys.Single()], 3) >= 3f)
                    {
                        response += "World Languages Honors Fulfilled.";
                    }
                    else
                    {
                        int atLeastTwo = 0;
                        foreach(String key in languageCounts.Keys)
                        {
                            if (languageCounts[key] >= 2f) atLeastTwo++;
                        }

                        if(atLeastTwo >= 2f)
                        {
                            response += "World Languages Honors Fulfilled.";
                        }
                        else if(atLeastTwo >= 1f)
                        {
                            response += "World Language Requirement Fulfilled.";
                        }
                        else
                        {
                            response += "World Language Requirement Incomplete.";
                        }
                    }
                }

                if((float)Math.Round(value, 3) >= HistoryWLCreditsHonors)
                {
                    response += String.Format("{0}Overall Honors Fulfilled.", Environment.NewLine);
                }
                else if ((float)Math.Round(value, 3) >= HistoryWLCreditsRequired)
                {
                    response += String.Format("{0}Overall Requirement Fulfilled.", Environment.NewLine);
                }
                else
                {
                    response += String.Format("{0}Overall Requirement Incomplete.", Environment.NewLine);
                }

                return response;
            }
        }

        public static String ScienceMathRequirementStatus(int studentId)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<Credit> credits = db.Credits.Where(c => c.StudentId == studentId && (c.CreditType.Name.Equals("Mathematics") || c.CreditType.Name.Equals("Science"))).ToList();
                float value = 0f;
                foreach (Credit credit in credits)
                {
                    value += credit.CreditValue.Value / 9f;
                }

                List<String> Required = new List<string>(RequiredScienceCourses);

                foreach(String course in RequiredScienceCourses)
                {
                    if(credits.Where(c => c.Sections.Where(s => s.Course.Name.Contains(course)).Count() > 0).Count() > 0 || credits.Where(c => c.Notes.Contains(course)).Count() > 0)
                    {
                        Required.Remove(course);
                    }
                }

                if(Required.Count > 0)
                {
                    String missing = "Missing: ";
                    foreach(String course in Required)
                    {
                        missing += String.Format("{0} ", course);
                    }

                    return missing;
                }

                String response = MathSciCreditsHonors <= (float)Math.Round(value, 3) ? "Honors Fulfilled" : (MathSciCreditsRequired <= (float)Math.Round(value, 3) ? "Credit Fulfilled" : String.Format("Incomplete {0} of {1}", value, MathSciCreditsRequired));


                return response;
            }
        }

        public static List<TableRow> GetCreditReportWebTable(int studentId)
        {
            List<TableRow> table = new List<TableRow>();
            Dictionary<String, String> gradstatus = GetGraduationRequirementStatuses(studentId);
            TableHeaderRow gradreqheader = new TableHeaderRow();
            TableRow gradreqrow = new TableRow();
            foreach(String req in gradstatus.Keys)
            {
                gradreqheader.Cells.Add(new TableHeaderCell() { Text = req });
                gradreqrow.Cells.Add(new TableCell() { Text = gradstatus[req] });
            }

            table.Add(gradreqheader);
            table.Add(gradreqrow);

            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Where(s => s.ID == studentId).Single();

                Dictionary<String, float> credits = new Dictionary<string, float>();
                foreach (GradeTableEntry credType in db.GradeTableEntries.Where(t => t.GradeTable.Name.Equals("Credit Types")).ToList())
                {
                    if (!credits.ContainsKey(credType.Name))
                        credits.Add(credType.Name, 0f);
                }

                // Build Header Rows
                TableHeaderRow totalCreditsHeaderRow = new TableHeaderRow();
                totalCreditsHeaderRow.Cells.Add(new TableHeaderCell() { Text = "Total Credits", ColumnSpan = credits.Keys.Count });
                table.Add(totalCreditsHeaderRow);

                TableHeaderRow subheader = new TableHeaderRow();
                foreach(String type in credits.Keys)
                {
                    subheader.Cells.Add(new TableCell() { Text = type });
                }
                table.Add(subheader);

                Dictionary<String, List<TableRow>> creditRows = new Dictionary<string, List<TableRow>>();

                foreach (Credit credit in student.Credits.ToList())
                {
                    credits[credit.CreditType.Name] += credit.CreditValue.Value / 9f;
                    TableRow credRow = new TableRow();
                    credRow.Cells.AddRange(new TableCell[] {
                        new TableCell() { Text = credit.Sections.Count > 0? String.Format("{0} {1} {2}",credit.Sections.ToList().Single().Course.Name, credit.Sections.ToList().Single().Terms.ToList().First().Name, credit.Sections.ToList().Single().Terms.ToList().First().StartDate.Year): ""},
                        new TableCell() {Text = credit.Notes, ColumnSpan = (credits.Keys.Count > 3 ? credits.Keys.Count - 2 : 1)},
                        new TableCell() {Text= String.Format("{0:F3}",credit.CreditValue.Value / 9f)}
                        });

                    if(creditRows.ContainsKey(credit.CreditType.Name))
                        creditRows[credit.CreditType.Name].Add(credRow);
                    else
                    {
                        creditRows.Add(credit.CreditType.Name, new List<TableRow>() {credRow});
                    }
                }

                TableRow row = new TableRow();

                foreach (String credType in credits.Keys)
                {
                    row.Cells.Add(new TableCell() { Text = String.Format("{0:F3}", credits[credType]) });
                }

                table.Add(row);
                table.Add(new TableFooterRow());

                foreach(String type in creditRows.Keys)
                {
                    TableHeaderRow typeHeaderRow = new TableHeaderRow();
                    typeHeaderRow.Cells.Add(new TableHeaderCell() { Text = type, ColumnSpan = credits.Keys.Count });
                    table.Add(typeHeaderRow);
                    foreach(TableRow tr in creditRows[type])
                    {
                        table.Add(tr);
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// Import Credits from Blackbaud.
        /// Use the Query named for this in Blackbaud.
        /// Returns a CSV contaning everything that was ignored or errored.
        /// </summary>
        /// <param name="creditCSV"></param>
        /// <returns></returns>
        public static CSV ImportCredits(CSV creditCSV)
        {
            CSV output = new CSV();
            using (WebhostEntities db = new WebhostEntities())
            {
                int newCreditId = db.Credits.Count() > 0 ? db.Credits.OrderBy(c => c.id).Select(c => c.id).ToList().Last() +1 : 0;
                int year = DateRange.GetCurrentAcademicYear();
                foreach (Dictionary<String, String> row in creditCSV.Data)
                {
                    String note = String.Format("Imported {5} Credit for {0} taken {1} {2} - {3} {4}", row[Credit_CourseName], row[Credit_Term], row[Credit_AcademicYear], row[Credit_FirstName], row[Credit_LastName], row[Credit_Department]);
                    int studentId = Convert.ToInt32(row[Credit_StudentId]);
                    if (db.Students.Where(s => s.ID == studentId).Count() <= 0)
                    {
                        output.Add(new Dictionary<string, string>() { {"Student", String.Format("{0} {1}", row[Credit_FirstName], row[Credit_LastName]) }, {"Error", "Not in Webhost"}});
                        continue;
                    }
                    Student student = db.Students.Where(s => s.ID == studentId).Single();
                    if (student.Credits.Where(c => c.Notes.Equals(note)).Count() > 0)
                    {

                        output.Add(new Dictionary<string, string>() { { "Student", String.Format("{0} {1}", row[Credit_FirstName], row[Credit_LastName]) }, { "Error", String.Format("{3} Credit already exists for {0} taken {1} {2}", row[Credit_CourseName], row[Credit_Term], row[Credit_AcademicYear], row[Credit_Department]) } });
                        continue; // Don't re-import the same credit more than once!
                    }


                    GradeTable CreditTypes;
                    GradeTable CreditValues;
                    try
                    {
                        CreditTypes = db.GradeTables.Where(t => t.Name.Equals("Credit Types") && t.AcademicYearID == year).Single();
                        CreditValues = db.GradeTables.Where(t => t.Name.Equals("Credit Values") && t.AcademicYearID == year).Single();
                    }
                    catch
                    {
                        continue;
                    }

                    String dept = row[Credit_Department];
                    if (CreditTypes.GradeTableEntries.Where(t => dept.Contains(t.Name)).Count() <= 0)
                    {
                        output.Add(new Dictionary<string, string>() { { "Student", String.Format("{0} {1}", row[Credit_FirstName], row[Credit_LastName]) }, { "Error", String.Format("{3} No credit given for {0} taken {1} {2}", row[Credit_CourseName], row[Credit_Term], row[Credit_AcademicYear], row[Credit_Department]) } });
                        continue;
                    }
                    Credit credit = new Credit()
                    {
                        StudentId = studentId,
                        Notes = note,
                        CreditTypeId = CreditTypes.GradeTableEntries.Where(t => dept.Contains(t.Name)).Single().id,
                        CreditValueId = CreditValues.GradeTableEntries.Where(v => v.Value == 3).Single().id,
                        id = newCreditId++
                    };

                    db.Credits.Add(credit);
                }

                db.SaveChanges();
            }

            return output;
        }


        /// <summary>
        /// Assigns Default Credit Values for all classes for all students.  These credits may be modified later and will be passed over by this method if it is run again.
        /// </summary>
        /// <returns>CSV object containing all new credits which have been created.</returns>
        public static CSV AssignDefaultCredits()
        {
            CSV output = new CSV();

            using(WebhostEntities db = new WebhostEntities())
            {
                int newCreditId = db.Credits.Count() > 0? db.Credits.OrderBy(c => c.id).ToList().Last().id + 1 : 0;

                foreach(AcademicYear acadYear in db.AcademicYears.ToList())
                {
                    GradeTable CreditTypes;
                    GradeTable CreditValues;
                    try
                    {
                        CreditTypes = acadYear.GradeTables.Where(t => t.Name.Equals("Credit Types")).Single();
                        CreditValues = acadYear.GradeTables.Where(t => t.Name.Equals("Credit Values")).Single();
                    }
                    catch
                    {
                        continue;
                    }
                    foreach(Course course in acadYear.Courses.Where(c => c.Sections.Count > 0).ToList())
                    {
                        if (CreditTypes.GradeTableEntries.Where(t => course.Department.Name.Contains(t.Name)).Count() <= 0)
                        {
                            WebhostEventLog.Syslog.LogInformation("Skipping nonsense credit for {0}", course.Name);
                            continue;
                        }
                        foreach(Section section in course.Sections.Where(s => s.Students.Count > 0).ToList())
                        {

                            GradeTableEntry creditType = CreditTypes.GradeTableEntries.Where(t => course.Department.Name.Contains(t.Name)).Single();
                            GradeTableEntry creditValue = CreditValues.GradeTableEntries.Where(v => v.Value == course.LengthInTerms * 3).Single();

                            foreach(Student student in section.Students.ToList())
                            {
                                // if this student already has credit for this section, then don't do anything!  it may be that the credit has been altered.
                                if(student.Credits.Where(c => c.Sections.Contains(section)).Count() <= 0)
                                {
                                    WebhostEventLog.Syslog.LogInformation("Giving {0} {1} credit for [{2}] {3}", student.FirstName, student.LastName, section.Block.LongName, section.Course.Name);
                                    Credit credit = new Credit()
                                    {
                                        id = newCreditId++,
                                        StudentId = student.ID,
                                        CreditTypeId = creditType.id,
                                        CreditValueId = creditValue.id,
                                        Student = student,
                                        CreditType = creditType,
                                        CreditValue = creditValue,
                                        Notes = "This is a default credit."
                                    };

                                    credit.Sections.Add(section);

                                    db.Credits.Add(credit);

                                    Dictionary<String, String> row = new Dictionary<string, string>()
                                    {
                                        {"Student", String.Format("{0} {1}", student.FirstName, student.LastName)},
                                        {"Credit Type", creditType.Name},
                                        {"Credit Value", creditValue.Name},
                                        {"Class", section.Course.Name},
                                        {"Year", acadYear.id.ToString()}
                                    };

                                    output.Add(row);
                                }
                                else
                                {
                                    WebhostEventLog.Syslog.LogInformation("{0} {1} already has credit for {2}. Nothing to do.", student.FirstName, student.LastName, section.Course.Name);
                                }
                            }
                        }
                    }
                }

                db.SaveChanges();
            }

            return output;
        }

        /// <summary>
        /// Create a credit waiver for a given student.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="creditTypeId"></param>
        /// <param name="creditValueId"></param>
        /// <param name="notes"></param>
        public static void CreateWaiverCredit(int studentId, int creditTypeId, int creditValueId, String notes = "Waived Credit.")
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int newCreditId = db.Credits.Count() > 0 ? db.Credits.OrderBy(c => c.id).ToList().Last().id + 1 : 0;

                Credit waiver = new Credit()
                {
                    StudentId = studentId,
                    CreditTypeId = creditTypeId,
                    CreditValueId = creditValueId,
                    Notes = notes
                };

                db.Credits.Add(waiver);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// Get a CSV file containing completed credit counts for each student in the given list.
        /// Rows => Students
        /// Columns => Credits
        /// </summary>
        /// <param name="studentIds">Default value is all active students.</param>
        /// <returns></returns>
        public static CSV GetStudentCredits(List<int> studentIds = null)
        {
            CSV output = new CSV();

            using(WebhostEntities db = new WebhostEntities())
            {
                List<Student> students = new List<Student>();
                if (studentIds == null)
                    students = db.Students.Where(s => s.isActive).OrderBy(s => s.GraduationYear).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
                else
                    students = db.Students.Where(s => studentIds.Contains(s.ID)).OrderBy(s => s.GraduationYear).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();

                foreach(Student student in students)
                {
                    Dictionary<String, float> credits = new Dictionary<string,float>();
                    foreach(GradeTableEntry credType in db.GradeTableEntries.Where(t => t.GradeTable.Name.Equals("Credit Types")).ToList())
                    {
                        if (!credits.ContainsKey(credType.Name))
                            credits.Add(credType.Name, 0f);
                    }

                    foreach(Credit credit in student.Credits.ToList())
                    {
                        credits[credit.CreditType.Name] += credit.CreditValue.Value / 9f;
                    }

                    Dictionary<String, String> row = new Dictionary<string, string>()
                    {
                        {"First Name", student.FirstName},
                        {"Last Name", student.LastName} 
                    };

                    foreach(String credType in credits.Keys)
                    {
                        row.Add(credType, credits[credType].ToString());
                    }

                    output.Add(row);
                }
            }

            return output;
        }
    }
}
