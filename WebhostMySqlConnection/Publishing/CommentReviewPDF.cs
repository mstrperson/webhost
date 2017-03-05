using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Text.RegularExpressions;
using System.IO;

namespace WebhostMySQLConnection.Publishing
{
    public class CommentReviewPDF : PDFPublisher
    {
        protected struct SmallComment
        {
            public String html;
            public String finalGrade;
            public String examGrade;
            public String effortGrade;
            public String termGrade;
            public String studentName;
        }

        protected List<SmallComment> studentParagraphs;
        protected String headerHTML;
        protected String className;

        protected List<Paragraph> HeaderParagraphs
        {
            get
            {
                String[] p = { "</p>" };
                String[] HTMLBlocks = headerHTML.Split(p, StringSplitOptions.RemoveEmptyEntries);

                List<Paragraph> Paragraphs = new List<Paragraph>();
                Regex emptyParagraph = new Regex(@"^(<p>)?\s*</p>$");
                foreach (String paragraphHTML in HTMLBlocks)
                {
                    string html = Clean(paragraphHTML) + "</p>";
                    if (emptyParagraph.IsMatch(html))
                        continue;
                    StringReader reader = new StringReader(html);
                    List<IElement> elements = HTMLWorker.ParseToList(reader, null);
                    Paragraph HeaderParagraph = new Paragraph("", new Font(Font.FontFamily.TIMES_ROMAN, 12f));

                    for (int i = 0; i < elements.Count; i++)
                    {
                        HeaderParagraph.Add(elements[i]);
                    }

                    Paragraphs.Add(HeaderParagraph);
                    Paragraphs.Add(new Paragraph(" ", new Font(Font.FontFamily.TIMES_ROMAN, 12f)));
                }



                return Paragraphs;
            }
        }

        private List<Paragraph> CommentParagraphs(SmallComment smc)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();

            String[] p = { "</p>" };
            String[] HTMLBlocks = smc.html.Split(p, StringSplitOptions.RemoveEmptyEntries);

            Paragraph bar = new Paragraph("__________________________________________________________", new Font(Font.FontFamily.TIMES_ROMAN, 12f));
            paragraphs.Add(bar);

            Paragraph namep = new Paragraph(smc.studentName, new Font(Font.FontFamily.TIMES_ROMAN, 16f));
            paragraphs.Add(namep);

            paragraphs.Add(new Paragraph(""));

            PdfPTable table = new PdfPTable(4);
            table.AddCell(new PdfPCell(new Phrase("Exam Grade")));
            table.AddCell(new PdfPCell(new Phrase("Trimester Grade")));
            table.AddCell(new PdfPCell(new Phrase("Engagement")));
            table.AddCell(new PdfPCell(new Phrase("Final Grade")));

            table.AddCell(new PdfPCell(new Phrase(smc.examGrade)));
            table.AddCell(new PdfPCell(new Phrase(smc.termGrade)));
            table.AddCell(new PdfPCell(new Phrase(smc.effortGrade)));
            table.AddCell(new PdfPCell(new Phrase(smc.finalGrade)));

            Paragraph tblp = new Paragraph("", new Font(Font.FontFamily.TIMES_ROMAN, 12f));
            tblp.Add(table);

            paragraphs.Add(tblp);

            Regex emptyParagraph = new Regex(@"^(<p>)?\s*</p>");
            foreach (String paragraphHTML in HTMLBlocks)
            {
                String html = Clean(paragraphHTML) + "</p>";
                if (emptyParagraph.IsMatch(html))
                    continue;
                StringReader reader = new StringReader(html);
                List<IElement> elements = HTMLWorker.ParseToList(reader, null);

                Paragraph HeaderParagraph = new Paragraph("", new Font(Font.FontFamily.TIMES_ROMAN, 12f));
                for (int i = 0; i < elements.Count; i++)
                {
                    HeaderParagraph.Add(elements[i]);
                }

                paragraphs.Add(HeaderParagraph);
                paragraphs.Add(new Paragraph(" ", new Font(Font.FontFamily.TIMES_ROMAN, 12f)));
            }

            return paragraphs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="termId"></param>
        public CommentReviewPDF(int sectionId, int termId) : base()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                studentParagraphs = new List<SmallComment>();
                Section section = db.Sections.Where(sec => sec.id == sectionId).Single();
                className = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name);
                if(section.CommentHeaders.Where(h => h.TermIndex == termId).Count() > 0)
                {
                    CommentHeader header = section.CommentHeaders.Where(h => h.TermIndex == termId).Single();
                    headerHTML = CommentLetter.CleanTags(header.HTML);
                    foreach(StudentComment comment in header.StudentComments)
                    {
                        SmallComment smc = new SmallComment()
                        {
                            studentName = String.Format("{0} {1}", comment.Student.FirstName, comment.Student.LastName),
                            html = CommentLetter.CleanTags(comment.HTML),
                            finalGrade = comment.FinalGrade.Name,
                            effortGrade = comment.EffortGrade.Name,
                            termGrade = comment.TermGrade.Name,
                            examGrade = comment.ExamGrade.Name
                        };

                        studentParagraphs.Add(smc);
                    }
                }
                else
                {
                    throw new CommentException(String.Format("No Header Paragraph for {0}", className));
                }
            }
        }

        new public String Publish()
        {
            Init("~/Temp/proofs_" + DateTime.Now.ToFileTimeUtc() + ".pdf");

            foreach(Paragraph p in HeaderParagraphs)
            {
                document.Add(p);
            }

            foreach(SmallComment smc in studentParagraphs)
            {
                List<Paragraph> ppgs = CommentParagraphs(smc);
                foreach(Paragraph p in ppgs)
                {
                    document.Add(p);
                }
            }

            return base.Publish();
        }
    }
}
