using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Text.RegularExpressions;
using System.IO;
using WebhostMySQLConnection.Web;
using System.Web;

namespace WebhostMySQLConnection.Publishing
{
    public class CommentLetter:PDFPublisher
    {
        #region Comment Fields
        public string StudentHTML
        {
            get;
            protected set;
        }

        public string HeaderHTML
        {
            get;
            protected set;
        }

        public string EffortGrade
        {
            get;
            protected set;
        }

        public bool showFinal
        {
            get;
            protected set;
        }

        public bool showExam
        {
            get;
            protected set;
        }

        public string FinalGrade
        {
            get;
            protected set;
        }

        public string TermGrade
        {
            get;
            protected set;
        }

        public string ExamGrade
        {
            get;
            protected set;
        }

        public string StudentName
        {
            get;
            protected set;
        }

        public string CourseName
        {
            get;
            protected set;
        }

        public string AdvisorName
        {
            get;
            protected set;
        }

        public Dictionary<String, Signature> Authors
        {
            get;
            protected set;
        }

        public String TermName
        {
            get;
            protected set;
        }

        public DateTime PublishDate
        {
            get;
            protected set;
        }

        public String flagFileName
        {
            get;
            protected set;
        }

        private HttpServerUtility Server
        {
            get
            {
                try
                {
                    return HttpContext.Current.Server;
                }
                catch
                {
                    return null;
                }
            }
        }

        public String SaveDirectory { get; set; }

        #endregion

        public static String CleanTags(String HTML, bool forPublish = false)
        {
            if(forPublish) HTML = HTML.Replace("{plus-sign}", "+");
            if(!HTML.StartsWith("<p"))
            {
                HTML = "<p>" + HTML;
            }
            Regex pTagPlus = new Regex("<p [^>]+>");
            Regex spanTag = new Regex("<span[^>]*>");
            String boldSpan = "font-weight: bold;";
            String italSpan = "font-style: italic;";
            String ulnSpan = "text-decoration: underline;";
            Regex brTag = new Regex("<br ?/?>");
            foreach(Match match in pTagPlus.Matches(HTML))
            {
                HTML = HTML.Replace(match.Value, "<p>");
            }
            foreach(Match match in spanTag.Matches(HTML))
            {
                bool bold = match.Value.Contains(boldSpan), ital = match.Value.Contains(italSpan), uln = match.Value.Contains(ulnSpan);

                String replace = "";
                if (bold)
                    replace += "<b>";
                if (ital)
                    replace += "<i>";
                if (uln)
                    replace += "<u>";

                int indexOfMatch = HTML.IndexOf(match.Value);

                HTML = HTML.Substring(0, indexOfMatch) + replace + HTML.Substring(indexOfMatch + match.Value.Length);

                int indexOfEnd = HTML.IndexOf("</span>", indexOfMatch);
                if (indexOfEnd == -1) indexOfEnd = HTML.Length;
                HTML = HTML.Substring(0, indexOfEnd) + (uln ? "</u>" : "") + (ital ? "</i>" : "") + (bold ? "</b>" : "") + (indexOfEnd + 7 <= HTML.Length ? HTML.Substring(indexOfEnd + 7) : "");
            }
            foreach(Match match in brTag.Matches(HTML))
            {
                HTML = HTML.Replace(match.Value, "</p><p>");
            }

            if (HTML.EndsWith("</p><p>"))
            {
                HTML = HTML.Substring(0, HTML.Length - 3);
            }
            else if (!HTML.EndsWith("</p>"))
            {
                HTML += "</p>";
            }

            return HTML;
        }

        /// <summary>
        /// Publisher for Individual Comment Letter.
        /// </summary>
        /// <param name="studentCommentId"></param>
        public CommentLetter(int studentCommentId, bool commandLine = false, String saveDir = "")
        {
            SaveDirectory = saveDir;
            using(WebhostEntities db = new WebhostEntities())
            {
                if (db.StudentComments.Where(com => com.id == studentCommentId).Count() <= 0)
                    throw new CommentException(String.Format("No comment letter with id={0} exists.", studentCommentId));

                StudentComment studentParagraph = db.StudentComments.Where(com => com.id == studentCommentId).Single();

                flagFileName = String.Format("{1}{0}_flag.txt", studentCommentId, commandLine?"":saveDir.Equals("")?"~/Temp/":saveDir+"\\");

                StudentHTML = CleanTags(studentParagraph.HTML, true);
                HeaderHTML = CleanTags(studentParagraph.CommentHeader.HTML, true);
                StudentName = studentParagraph.Student.FirstName + " " + studentParagraph.Student.LastName;
                AdvisorName = studentParagraph.Student.Advisor.FirstName + " " + studentParagraph.Student.Advisor.LastName;
                CourseName = studentParagraph.CommentHeader.Section.Course.Name;
                Authors = new Dictionary<string, Signature>();
                foreach(Faculty teacher in studentParagraph.CommentHeader.Section.Teachers)
                {
                    Authors.Add(String.Format("{0} {1}", teacher.FirstName, teacher.LastName), new Signature(teacher.ID));
                }

                showExam = !studentParagraph.CommentHeader.Section.Block.LongName.Contains("Sports");
                showFinal = studentParagraph.CommentHeader.TermIndex == studentParagraph.CommentHeader.Section.Terms.OrderBy(term => term.StartDate).ToList().Last().id;

                FinalGrade = studentParagraph.FinalGrade.Name;
                ExamGrade = studentParagraph.ExamGrade.Name;
                TermGrade = studentParagraph.TermGrade.Name;
                EffortGrade = studentParagraph.EffortGrade.Name;

                TermName = studentParagraph.CommentHeader.Term.Name;
                PublishDate = studentParagraph.CommentHeader.Term.CommentsDate;
                String fileName = String.Format("{0} - {1} {2}", StudentName, studentParagraph.CommentHeader.Section.Block.Name, CourseName.Replace("+", "p")).ToLower();

                fileName = fileName.Replace(" ", "_");
                fileName = fileName.Replace("\"", "");
                Regex disalowedChars = new Regex(@"(\.|:|&|#|@|\*|~|\?|<|>|\||\^|( ( )+)|/)");
                foreach (Match match in disalowedChars.Matches(fileName))
                {
                    fileName = fileName.Replace(match.Value, "");
                }

                fileName = String.Format("{0}{1}.pdf", commandLine?"C:\\Temp\\":saveDir.Equals("")?"~/Temp/":saveDir+"\\", fileName);

                Init(fileName, String.Format("{0} - {1} {2} {3} Comment Letter", StudentName, studentParagraph.CommentHeader.Section.Block.Name, CourseName, TermName), Authors.Keys.First());
            }
        }

        #region Letter Parts

        /// <summary>
        /// Put Class Header Paragraph into a Paragraph Element.
        /// </summary>
        /// <param name="Letter">Comment Letter from the specified class</param>
        /// <returns></returns>
        private List<Paragraph> ClassHeaderParagraphs
        {
            get
            {
                String[] p = { "</p>" };
                String[] HTMLBlocks = HeaderHTML.Split(p, StringSplitOptions.RemoveEmptyEntries);

                List<Paragraph> Paragraphs = new List<Paragraph>();
                Regex emptyParagraph = new Regex(@"^(<p>)?\s*</p>$");
                foreach (String paragraphHTML in HTMLBlocks)
                {
                    string html = Clean(paragraphHTML) + "</p>";
                    if (emptyParagraph.IsMatch(html))
                        continue;
                    StringReader reader = new StringReader(html);
                    List<IElement> elements = HTMLWorker.ParseToList(reader, null);
                    Paragraph HeaderParagraph = new Paragraph("", NormalFont(12f));

                    for (int i = 0; i < elements.Count; i++)
                    {
                        foreach (Chunk chunk in elements[i].Chunks)
                        {
                            int style = chunk.Font.Style;
                            chunk.Font = NormalFont(12f, style);
                        }
                        HeaderParagraph.Add(elements[i]);
                    }

                    Paragraphs.Add(HeaderParagraph);
                    Paragraphs.Add(new Paragraph(" ", NormalFont(12f)));
                }



                return Paragraphs;
            }
        }

        /// <summary>
        /// Put formatted Student Comment Paragraph into a Paragraph element.
        /// </summary>
        /// <param name="Letter">Student Comment Letter</param>
        /// <returns></returns>
        private List<Paragraph> StudentCommentParagraph
        {
            get
            {
                List<Paragraph> paragraphs = new List<Paragraph>();

                String[] p = { "</p>" };
                String[] HTMLBlocks = this.StudentHTML.Split(p, StringSplitOptions.RemoveEmptyEntries);

                Regex emptyParagraph = new Regex(@"^(<p>)?\s*</p>");
                foreach (String paragraphHTML in HTMLBlocks)
                {
                    String html = Clean(paragraphHTML) + "</p>";
                    if (emptyParagraph.IsMatch(html))
                        continue;
                    StringReader reader = new StringReader(html);
                    List<IElement> elements = HTMLWorker.ParseToList(reader, null);
                    foreach(IElement element in elements)
                    {
                        foreach(Chunk chunk in element.Chunks)
                        {
                            chunk.Font = NormalFont(12f);
                        }
                    }
                    Paragraph HeaderParagraph = new Paragraph("", NormalFont(12f));
                    for (int i = 0; i < elements.Count; i++)
                    {
                        HeaderParagraph.Add(elements[i]);
                    }

                    paragraphs.Add(HeaderParagraph);
                    paragraphs.Add(new Paragraph(" ", NormalFont(12f)));
                }

                return paragraphs;
            }
        }

        /// <summary>
        /// Returns the Header in a PdfPTable with Seal Image for the Final Published Comment Letter
        /// </summary>
        /// <param name="Date">Date that comments will be sent</param>
        /// <param name="Term">Term Name</param>
        /// <returns></returns>
        private PdfPTable HeaderTable
        {
            get
            {
                PdfPTable HeaderTable = new PdfPTable(1);
                HeaderTable.WidthPercentage = 100f;


                iTextSharp.text.Image SealImage = iTextSharp.text.Image.GetInstance(CommentResources.seal, BaseColor.WHITE);
                SealImage.ScaleAbsolute(72f, 72f);
                SealImage.Alignment = 1;
                SealImage.Border = 0;
                SealImage.SpacingBefore = 10f;


                #region Date

                PdfPCell DateCell = new PdfPCell();
                DateCell.Colspan = 1;
                DateCell.HorizontalAlignment = 1;

                Paragraph ds = new Paragraph("Dublin School", NormalFont(14f));
                ds.Alignment = 1;
                ds.SpacingBefore = 0f;
                DateCell.AddElement(ds);

                DateCell.AddElement(SealImage);

                Paragraph tr = new Paragraph(TermName + " Comments", NormalFont(12f));
                tr.Alignment = 1;
                DateCell.AddElement(tr);

                Paragraph dt = new Paragraph(PublishDate.ToLongDateString(), NormalFont(12f));
                dt.Alignment = 1;
                DateCell.AddElement(dt);

                DateCell.Border = 0;
                HeaderTable.AddCell(DateCell);

                HeaderTable.AddCell(HorizontalLine(1));
                #endregion

                return HeaderTable;
            }
        }

        /// <summary>
        /// Returns the Formatted Table for the Student Name, Advisor, and Class Name lines of a letter.
        /// Example:
        /// 
        /// Student:    Thomas Coneys
        /// Advisor:    Brooks Johnson
        /// Class:      Programming I
        /// </summary>
        /// <param name="Letter">Comment Letter instance (pass by Reference)</param>
        /// <returns></returns>
        private PdfPTable StudentAdvisorClassTable
        {
            get
            {
                PdfPTable Table = new PdfPTable(4);
                Table.WidthPercentage = 100f;

                //Student Name Label
                PdfPCell snLabelCell = new PdfPCell();
                snLabelCell.Border = 0;
                Paragraph snLabel = new Paragraph("Student:  " + StudentName, NormalFont(10f));
                snLabel.Alignment = 0;
                snLabelCell.AddElement(snLabel);
                Table.AddCell(snLabelCell);


                //Advisor Name Label
                PdfPCell adLabelCell = new PdfPCell();
                adLabelCell.Border = 0;
                Paragraph adLabel = new Paragraph("Advisor:  " + AdvisorName, NormalFont(10f));
                adLabel.Alignment = 0;
                adLabelCell.AddElement(adLabel);
                Table.AddCell(adLabelCell);



                //Class Name Label
                PdfPCell clLabelCell = new PdfPCell();
                clLabelCell.Border = 0;
                Paragraph clLabel = new Paragraph("Course:  " + CourseName, NormalFont(10f));
                clLabel.Alignment = 0;
                clLabelCell.Colspan = 2;
                clLabelCell.AddElement(clLabel);
                Table.AddCell(clLabelCell);

                return Table;
            }
        }


        private PdfPTable GradesBar
        {
            get
            {
                PdfPTable Table = new PdfPTable(4);
                Table.WidthPercentage = 100f;

                //Exam Grade
                if (showExam)
                {
                    PdfPCell exgCell = new PdfPCell();
                    Paragraph exg = new Paragraph("Exam Grade:  " + ExamGrade, NormalFont(10f));
                    exg.Alignment = 0;
                    exgCell.Border = 0;
                    exgCell.AddElement(exg);
                    Table.AddCell(exgCell);
                }
                else
                {
                    Table.AddCell(new PdfPCell() { Border=0 });
                }
                //Trimester Grade
                PdfPCell tgCell = new PdfPCell();
                Paragraph tg = new Paragraph("Trimester Grade:  " + TermGrade, NormalFont(10f));
                tg.Alignment = 0;
                tgCell.Border = 0;
                tgCell.AddElement(tg);
                Table.AddCell(tgCell);

                //Effort Grade
                PdfPCell egCell = new PdfPCell();
                Paragraph eg = new Paragraph("Academic Engagement Grade:  " + EffortGrade, NormalFont(10f));
                eg.Alignment = 0;
                egCell.Border = 0;
                egCell.AddElement(eg);
                Table.AddCell(egCell);

                //Final Grade -- only shown if Final Grade is applicable.
                if (showFinal)
                {
                    PdfPCell fgCell = new PdfPCell();
                    Paragraph fg = new Paragraph("Final Grade:  " + FinalGrade, NormalFont(10f));
                    fg.Alignment = 0;
                    fgCell.Border = 0;
                    fgCell.AddElement(fg);
                    Table.AddCell(fgCell);
                }
                else
                {
                    PdfPCell fgCell = new PdfPCell();
                    fgCell.Border = 0;
                    Table.AddCell(fgCell);
                }

                Table.AddCell(HorizontalLine(4));

                return Table;
            }
        }

        private PdfPTable EffortGradeBar
        {
            get
            {
                PdfPTable Table = new PdfPTable(1);
                Table.WidthPercentage = 100f;

                Table.AddCell(HorizontalLine(1));


                //Effort Grade
                PdfPCell egCell = new PdfPCell();
                Paragraph eg = new Paragraph("Effort Grade:  " + EffortGrade, NormalFont(10f));
                eg.Alignment = 0;
                egCell.Border = 0;
                egCell.AddElement(eg);
                Table.AddCell(egCell);

                Table.AddCell(HorizontalLine(1));

                return Table;
            }
        }

        private PdfPCell HorizontalLine(int Colspan, int Alignment = 1)
        {
            Paragraph line = new Paragraph("______________________________________________________________________________________________________________________________________", NormalFont(8f));
            line.Alignment = Alignment;

            PdfPCell spacer = new PdfPCell(line);
            spacer.Colspan = Colspan;
            spacer.Border = 0;
            spacer.HorizontalAlignment = Alignment;

            return spacer;
        }

        /// <summary>
        /// Create the Signature Line.
        /// If an image is stored in the database for the Author's signature, then that image is placed above the signature line.
        /// </summary>
        /// <param name="Letter">Comment Letter to be Signed</param>
        /// <returns></returns>
        private PdfPTable SignatureLine
        {
            get
            {
                PdfPTable line = new PdfPTable(Authors.Keys.Count);
                line.WidthPercentage = 100f;

                foreach (String author in Authors.Keys)
                {
                    PdfPCell cell1 = new PdfPCell();
                    cell1.Border = 0;
                    if (!Authors[author].Exists)
                    {
                        Paragraph par = new Paragraph(" \n \n \n", NormalFont(12f));
                        cell1.AddElement(par);
                    }
                    else
                    {
                        iTextSharp.text.Image sig = iTextSharp.text.Image.GetInstance(Authors[author].image, BaseColor.WHITE);

                        sig.ScaleToFit(72f * 8f, 56f);

                        cell1.AddElement(sig);
                    }

                    line.AddCell(cell1);
                }
                foreach (String author in Authors.Keys)
                {
                    Paragraph sigline = new Paragraph("___________________________________________________", NormalFont(8f));
                    sigline.Alignment = 0;

                    PdfPCell spacer = new PdfPCell(sigline);
                    spacer.Colspan = 1;
                    spacer.Border = 0;
                    spacer.HorizontalAlignment = 0;

                    line.AddCell(spacer);
                }
                foreach (String author in Authors.Keys)
                {
                    PdfPCell cell2 = new PdfPCell(new Paragraph(author, NormalFont(14f)));
                    cell2.Border = 0;

                    line.AddCell(cell2);
                }
                return line;
            }
        }

        #endregion

        public String Publish(bool commandLine = false)
        {
            #region Publish Flag
            if (File.Exists(commandLine?String.Format("C:\\Temp\\{0}",flagFileName):Server==null?flagFileName:Server.MapPath(flagFileName)))
            {
                State.log.WriteLine("Flag file {0} already exists... checking timestamp", flagFileName);
                String data = (new StreamReader(new FileStream(commandLine ? String.Format("C:\\Temp\\{0}", flagFileName) : Server == null ?  flagFileName : Server.MapPath(flagFileName), FileMode.Open))).ReadLine();
                long ticks = 0;
                try
                {
                    ticks = Convert.ToInt64(data);
                }
                catch
                {
                    // ignore... zero will do!
                    State.log.WriteLine("Flag file is incomplete.  Ignoring.");
                    WebhostEventLog.CommentLog.LogWarning("Flag file {0} is incomplete.  Ignoring.", flagFileName);
                }

                if(DateTime.Now.AddMinutes(2).Ticks < ticks)
                {
                    File.Delete(Server == null ? flagFileName : Server.MapPath(flagFileName));
                    State.log.WriteLine("Deleting Extraneous Old Flag File.");
                    WebhostEventLog.CommentLog.LogWarning("Deleting timed out flag file {0}", flagFileName);
                }
                else
                {
                    State.log.WriteLine("This file is Busy! Be Patient!");
                    throw new CommentException("Comment is already Publishing! Be Patient!");
                }
            }

            StreamWriter writer = new StreamWriter(new FileStream(commandLine ? String.Format("C:\\Temp\\{0}", flagFileName) : Server == null ? flagFileName : Server.MapPath(flagFileName), FileMode.Create));
            writer.WriteLine(DateTime.Now.Ticks);
            writer.Close();
            #endregion  // Publish flag

            document.Add(HeaderTable);
            document.Add(StudentAdvisorClassTable);
            document.Add(GradesBar);
            
            foreach (Paragraph paragraph in ClassHeaderParagraphs)
            {
                // if (!whiteSpace.IsMatch(paragraph.Content))
                document.Add(paragraph);
            }
            foreach (Paragraph paragraph in StudentCommentParagraph)
            {
                //  if (!whiteSpace.IsMatch(paragraph.Content))
                document.Add(paragraph);
            }
            document.Add(SignatureLine);

            File.Delete(commandLine ? String.Format("C:\\Temp\\{0}", flagFileName) : SaveDirectory.Equals("")? Server.MapPath(flagFileName):flagFileName);

            if(commandLine)
            {
                base.Publish();
                return FileName;
            }

            return base.Publish();
        }

        public static String PublishClass(int headerId, String SaveDir = "")
        {
            List<int> commentIds = new List<int>();
            List<String> fileNames = new List<string>();
            String packFileName = "";
            using(WebhostEntities db = new WebhostEntities())
            {
                CommentHeader header = db.CommentHeaders.Find(headerId);
                packFileName = String.Format("{0} {1} comments", header.Section.Block.LongName, header.Section.Course.Name).ToLower();
                WebhostEventLog.CommentLog.LogInformation("Publishing {0}", packFileName);
                packFileName = packFileName.Replace(" ", "_");
                packFileName = packFileName.Replace("\"", "");
                Regex disalowedChars = new Regex(@"(\.|:|&|#|@|\*|~|\?|<|>|\||\^|( ( )+)|/)");
                foreach(Match match in disalowedChars.Matches(packFileName))
                {
                    packFileName = packFileName.Replace(match.Value, "");
                }

                foreach(int id in header.StudentComments.Select(c => c.id))
                {
                    fileNames.Add((new CommentLetter(id, false, SaveDir)).Publish());
                }
            }

            if(SaveDir.Equals(""))
            {
                return MailControler.PackForDownloading(fileNames, packFileName, HttpContext.Current.Server);
            }
            else
            {
                return MailControler.PackForDownloading(fileNames, String.Format("{0}\\{1}.zip", SaveDir, packFileName));
            }
        }



        /// <summary>
        /// Publish comments via commandline tool.  dumps them in C:\Temp
        /// Defaults to this term, all students.
        /// </summary>
        /// <param name="termId"></param>
        /// <param name="studentIds"></param>
        public static void CommandLinePublish(int termId = -1, List<int> studentIds = null)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.Terms.Where(t => t.id == termId).Count() <= 0) termId = Import.GetCurrentOrLastTerm();

                Term term = db.Terms.Where(t => t.id == termId).Single();
                String packFileName = term.Name.ToLower() + "_term_comments";
                List<String> termCommentFiles = new List<string>();


                if (studentIds == null)
                {
                    studentIds = Import.ActiveStudents(termId);
                }

                foreach (int studentId in studentIds)
                {
                    Student student = db.Students.Where(s => s.ID == studentId).Single();
                    String studentFileName = String.Format("{0}, {1} [{3}] {2} Comments", student.LastName, student.FirstName, term.Name, student.GraduationYear);
                    Console.WriteLine("Publishing {0}, {1} [2]", student.LastName, student.FirstName, student.GraduationYear);
                    List<String> studentFiles = new List<string>();
                    List<StudentComment> commentIds = student.StudentComments.Where(com => com.CommentHeader.TermIndex == termId).ToList();
                    foreach (StudentComment comment in commentIds)
                    {
                        studentFiles.Add((new CommentLetter(comment.id,true)).Publish(true));
                        Console.WriteLine("\t[{0}] {1}", comment.CommentHeader.Section.Block.LongName, comment.CommentHeader.Section.Course.Name);
                    }

                    termCommentFiles.Add(MailControler.PackForDownloading(studentFiles, studentFileName));
                }

                MailControler.PackForDownloading(termCommentFiles, packFileName);
            }
        }

        public static String PublishTermByStudent(int termId = -1, List<int> studentIds = null, String SaveDir = "")
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if (db.Terms.Where(t => t.id == termId).Count() <= 0) termId = Import.GetCurrentOrLastTerm();

                Term term = db.Terms.Where(t => t.id == termId).Single();
                String packFileName = term.Name.ToLower() + "_term_comments.zip";
                List<String> termCommentFiles = new List<string>();

                if(studentIds == null)
                {
                    studentIds = Import.ActiveStudents(termId);
                }

                foreach(int studentId in studentIds)
                {
                    Student student = db.Students.Where(s => s.ID == studentId).Single();
                    String studentFileName = String.Format("[{3}] {0}, {1} {2} Comments.zip", student.LastName, student.FirstName, term.Name, student.GraduationYear);
                    WebhostEventLog.CommentLog.LogInformation("Publishing {0}", studentFileName);
                    List<String> studentFiles = new List<string>();
                    List<int> commentIds = student.StudentComments.Where(com => com.CommentHeader.TermIndex == termId).Select(com => com.id).ToList();
                    foreach(int id in commentIds)
                    {
                        studentFiles.Add((new CommentLetter(id, false, SaveDir)).Publish());
                    }

                    if (SaveDir.Equals(""))
                        termCommentFiles.Add(MailControler.PackForDownloading(studentFiles, studentFileName, HttpContext.Current.Server));
                    else
                        termCommentFiles.Add(MailControler.PackForDownloading(studentFiles, String.Format("{0}\\{1}", SaveDir, studentFileName)));
                }

                if (SaveDir.Equals(""))
                    return MailControler.PackForDownloading(termCommentFiles, packFileName, HttpContext.Current.Server);
                else
                    return MailControler.PackForDownloading(termCommentFiles, String.Format("{0}\\{1}", SaveDir, packFileName));
            }
        }
    }

    public class CommentException : WebhostException
    {
        public CommentException(String message, Exception innerException = null) : base(message, innerException)
        {
            WebhostEventLog.CommentLog.LogError(message);
        }
    }
}
