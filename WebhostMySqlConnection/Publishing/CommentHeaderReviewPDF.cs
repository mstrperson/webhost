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
    public class CommentHeaderReviewPDF : PDFPublisher
    {
        protected struct SmallHeader
        {
            public string html;
            public string className;
            public string teachers;
        }

        protected List<SmallHeader> Headers;

        protected String TeacherNames(List<Faculty> teachers)
        {
            String names = "";
            bool first = true;
            foreach(Faculty teacher in teachers)
            {
                if(!first)
                    names += ", ";

                names += String.Format("{0} {1}", teacher.FirstName, teacher.LastName);
                first = false;
            }
            return names;
        }

        protected List<Paragraph> Paragraphs(SmallHeader header)
        {
            String[] p = { "</p>" };
            String[] HTMLBlocks = header.html.Split(p, StringSplitOptions.RemoveEmptyEntries);

            List<Paragraph> Paragraphs = new List<Paragraph>() {
                                                                    new Paragraph(header.className, new Font(Font.FontFamily.TIMES_ROMAN, 16f)),
                                                                    new Paragraph(header.teachers, new Font(Font.FontFamily.TIMES_ROMAN, 12f))
                                                               };



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

            Paragraphs.Add(new Paragraph("_______________________________________________________________", new Font(Font.FontFamily.TIMES_ROMAN, 12f)));

            return Paragraphs;
        }

        public CommentHeaderReviewPDF(int termId = -1)
        {
            if (termId == -1) termId = DateRange.GetCurrentOrLastTerm();

            Headers = new List<SmallHeader>();
            using(WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termId).Single();
                foreach(Section section in term.Sections.Where(sec => sec.getsComments).ToList())
                {
                    string html = "<p>Not done!</p>";
                    if(section.CommentHeaders.Where(hdr => hdr.TermIndex == termId).Count() > 0)
                    {
                        html = section.CommentHeaders.Where(hdr => hdr.TermIndex == termId).First().HTML;
                    }

                    Headers.Add(new SmallHeader()
                        {
                            html = html,
                            className = String.Format("[{0}] {1}", section.Block.LongName, section.Course.Name),
                            teachers = TeacherNames(section.Teachers.ToList())
                        });
                }
            }
        }

        new public String Publish()
        {
            Init("~/Temp/headers_" + DateTime.Now.ToFileTimeUtc() + ".pdf");

            foreach(SmallHeader header in Headers.OrderBy(hdr => hdr.teachers).ThenBy(hdr => hdr.className))
            {
                foreach(Paragraph p in Paragraphs(header))
                {
                    document.Add(p);
                }
            }

            return base.Publish();
        }

    }
}
