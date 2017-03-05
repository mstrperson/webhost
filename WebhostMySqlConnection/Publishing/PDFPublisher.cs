using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection.Web;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Text.RegularExpressions;
using System.IO;

namespace WebhostMySQLConnection.Publishing
{
    public abstract class PDFPublisher
    {
        public static Font NormalFont(float fontSize, int style = Font.NORMAL)
        {
            String fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Font(bf, fontSize, style);
        }

        #region Global Settings
        /// <summary>
        /// 3/4" Top Margin for Comment Letter
        /// Float Value measured in pixels @ 72px/inch
        /// </summary>
        public static float TopMargin
        {
            get
            {
                return 0.5f * 72f;
            }
        }

        /// <summary>
        /// 1.25" Bottom Margin for Comment Letter
        /// Float Value measured in pixels @ 72px/inch
        /// </summary>
        public static float BottomMargin
        {
            get
            {
                return 72f;
            }
        }

        /// <summary>
        /// 1/2" Side Margins for Comment Letter
        /// Float Value measured in pixels @ 72px/inch
        /// </summary>
        public static float SideMargin
        {
            get
            {
                return 0.5f * 72f;
            }
        }

        /// <summary>
        /// Remove non-printable unicode characters.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String StripNonPrintable(String str)
        {
            return Regex.Replace(str, @"[^\u0000-\u007F]", string.Empty);
        }

        public static String FixSafeHTMLCodes(String HTML)
        {
            HTML = HTML.Replace("&nbsp;", " ");
            HTML = HTML.Replace("&amp;", "&");
            HTML = HTML.Replace("&lt;", "<");
            HTML = HTML.Replace("&gt;", ">");
            HTML = HTML.Replace("&rsquo;", "'");
            HTML = HTML.Replace("&lsquo;", "'");
            HTML = HTML.Replace("<p><p>", "<p>");
            HTML = HTML.Replace("</p></p>", "</p>");
            HTML = HTML.Replace("\r", "");
            HTML = HTML.Replace("\n", "");
            HTML = HTML.Replace("\t", "");
            return HTML;
        }

        /// <summary>
        /// Remove elements that can mess up the PDF Publish from the given HTML
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static String Clean(String HTML)
        {
            State.log.WriteLine("Cleaning HTML...");
            State.log.WriteLine("<html>");
            State.log.WriteLine(HTML);
            State.log.WriteLine("</html>");

            HTML = FixSafeHTMLCodes(HTML);

            Regex WordShit = new Regex("<!--(.|\n|\r)*-->");
            foreach (Match match in WordShit.Matches(HTML))
            {
                State.log.WriteLine("Found Word Shit...");
                HTML = HTML.Replace(match.Value, "");
            }

            Regex MoreShit = new Regex("(\n)?<hr(.)*/>");
            foreach(Match match in MoreShit.Matches(HTML))
            {
                State.log.WriteLine("Found more shit...");
                HTML = HTML.Replace(match.Value, "");
            }

            Regex divOpen = new Regex("<(div|DIV)[^>]*>");
            foreach (Match match in divOpen.Matches(HTML))
            {
                State.log.WriteLine("Fixing div tags.");
                HTML = HTML.Replace(match.Value, "<p>");
            }

            Regex badBold = new Regex("<[bB](>| [^>]*>)");
            foreach (Match match in badBold.Matches(HTML))
            {
                State.log.WriteLine("Fixing bold tags.");
                HTML = HTML.Replace(match.Value, "<strong>");
            }

            Regex cBold = new Regex("</[bB]>");
            foreach (Match match in cBold.Matches(HTML))
            {
                State.log.WriteLine("Fixing bold tags.");
                HTML = HTML.Replace(match.Value, "</strong>");
            }

            Regex font = new Regex("(</?font[^>]*>|</?(small|big)>)");
            foreach (Match match in font.Matches(HTML))
            {
                State.log.WriteLine("removing font tags.");
                HTML = HTML.Replace(match.Value, "");
            }

            Regex divClose = new Regex("</(div|DIV)>");
            foreach (Match match in divClose.Matches(HTML))
            {
                State.log.WriteLine("Fixing /div tags.");
                HTML = HTML.Replace(match.Value, "</p>");
            }

            Regex Br = new Regex("<(br|BR)( )*/>");
            foreach (Match match in Br.Matches(HTML))
            {
                State.log.WriteLine("removing break tags.");
                HTML = HTML.Replace(match.Value, "");
            }

            Regex span = new Regex("</?(span|SPAN)( (style|class|STYLE|CLASS)=\"([^\">]|\n|\r)*\")*>");
            foreach (Match match in span.Matches(HTML))
            {
                State.log.WriteLine("removing span tags.");
                HTML = HTML.Replace(match.Value, "");
            }
            Regex styleTag = new Regex("<[pP] ((style|class|STYLE|CLASS)=\".*\")+>");
            foreach (Match match in styleTag.Matches(HTML))
            {
                State.log.WriteLine("removing paragrpah style tags.");
                HTML = HTML.Replace(match.Value, "<p>");
            }

            Regex blockParagraph = new Regex("<p>( )+");
            foreach (Match match in blockParagraph.Matches(HTML))
            {
                State.log.WriteLine("removing paragrpah block tags.");
                HTML = HTML.Replace(match.Value, "<p>");
            }

            Regex blankParagraph = new Regex("<[pP]>((&(nbsp|NBSP);)| )*</[pP]>");
            foreach (Match match in blankParagraph.Matches(HTML))
            {
                State.log.WriteLine("removing empty paragraph tags.");
                HTML = HTML.Replace(match.Value, "");
            }

            State.log.WriteLine("Post Cleaning HTML...");
            State.log.WriteLine("<html>");
            State.log.WriteLine(HTML);
            State.log.WriteLine("</html>");

            return HTML;
        }

        #endregion

        #region InstanceVariables

        protected Document document;
        private PdfWriter writer;

        public String FileName
        {
            get;
            protected set;
        }

        public bool Initialized
        {
            get;
            protected set;
        }

        #endregion

        /// <summary>
        /// Uninitialized constructor to allow for dynamic file name setting.
        /// You must call the Init method before publishing!
        /// </summary>
        public PDFPublisher()
        {
            // uninitialized constructor.
            Initialized = false;
        }

        /// <summary>
        /// Initialize a PdfWriter with the default settings.
        /// </summary>
        /// <param name="fileName"></param>
        public PDFPublisher(String fileName, String title = "", String author = "webhost")
        {
            FileName = fileName;

            if (fileName.StartsWith("~/"))
                fileName = HttpContext.Current.Server.MapPath(fileName);

            if (File.Exists(fileName)) File.Delete(fileName);
            document = new Document(PageSize.LETTER, SideMargin, SideMargin, TopMargin, BottomMargin);
            writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.OpenOrCreate));

            document.Open();
            document.AddCreationDate();
            document.AddAuthor(author);
            document.AddCreator("webhost.dublinschool.org");
            document.AddTitle(title);
            Initialized = true;
        }

        protected void Init(String fileName, String title = "", String author = "webhost")
        {
            if (Initialized) throw new WebhostException("This PDFPublisher is already initialized.");
            FileName = fileName;

            if (fileName.StartsWith("~/"))
                fileName = HttpContext.Current.Server.MapPath(fileName);

            if (File.Exists(fileName)) File.Delete(fileName);
            document = new Document(PageSize.LETTER, SideMargin, SideMargin, TopMargin, BottomMargin);
            writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.OpenOrCreate));

            document.Open();
            document.AddCreationDate();
            document.AddAuthor(author);
            document.AddCreator("webhost.dublinschool.org");
            document.AddTitle(title);
            Initialized = true;
        }

        /// <summary>
        /// Publish the pdf.  Return the file name for downloading.
        /// When Implementing--
        /// 
        /// return base.Publish()
        /// 
        /// </summary>
        /// <returns></returns>
        public String Publish()
        {
            if (!Initialized) throw new WebhostException("You cannot publish an UnInitialized Document.");
            document.Close();
            return FileName;
        }
    }
}
