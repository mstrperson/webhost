using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.EVOPublishing;
using EvoPdf;
using System.IO;

namespace WebhostV2.UserControls
{
    public partial class CommentEditor : LoggingUserControl
    {
        public int DepartmentHeadMode
        {
            get { return Session["dhmode"] != null ? (int)Session["dhmode"] : -1; }
            set
            {
                if(value == -1) 
                {
                    Session["dhmode"] = -1;
                    return;
                }
                using(WebhostEntities db = new WebhostEntities())
                {
                    Faculty fac = db.Faculties.Find(FacultyId);
                    if (fac.Departments.Where(d => d.id == value).Count() > 0)
                    {
                        WebhostEventLog.CommentLog.LogInformation("Loading Department Head mode.");
                        Session["dhmode"] = value;
                    }
                    else
                    {
                        WebhostEventLog.CommentLog.LogWarning("Invalid access for department id {0}", value);
                        Session["dhmode"] = -1;
                    }
                }
            }
        }

        public bool ArchiveMode
        {
            set
            {
                if(value)
                {
                    WebhostEventLog.CommentLog.LogInformation("Loading in Archive Mode.");
                    TermSelBtn.Visible = false;
                    TermDDL.Visible = false;
                    SaveBtn.Enabled = false;
                    PFWInput.Enabled = false;
                    ShowPFWBtn.Visible = false;
                }
            }
        }

        public int SelectedTermId
        {
            get
            {
                return (Session["selterm"] != null ? (int)Session["selterm"] : -1);
            }
            set
            {
                WebhostEventLog.CommentLog.LogInformation("Selected term {0}", value);
                Session["selterm"] = value;
            }
        }

        public bool Saving
        {
            get
            {
                return (Session["saving"] != null && (bool)Session["saving"]);
            }
            protected set
            {
                Session["saving"] = value;
            }
        }

        public int FacultyId
        {
            get
            {
                if (Session["editing_as"] != null)
                {
                    return (int)Session["editing_as"];
                }
                else
                {
                    return ((BasePage)Page).user.ID;
                }
            }
            set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Faculty faculty = db.Faculties.Find(value);
                    if(faculty != null)
                    {
                        Session["editing_as"] = value;
                        WebhostEventLog.CommentLog.LogInformation("Editing as => {0} {1}", faculty.FirstName, faculty.LastName);
                        ResetPage();
                    }
                }
            }
        }

        public int LoadedHeaderId
        {
            get
            {
                if (Session["loaded_header"] != null)
                {
                    return (int)Session["loaded_header"];
                }
                else return -1;
            }
            protected set
            {
                if (value == -1)
                {
                    int tmp = LoadedHeaderId;
                    Session["loaded_header"] = -1;
                    WebhostEventLog.CommentLog.LogInformation("Deselected Header {0}",tmp);
                    return;
                }

                using(WebhostEntities db = new WebhostEntities())
                {
                    if (db.CommentHeaders.Where(hdr => hdr.id == value).Count() > 0)
                    {
                        WebhostEventLog.CommentLog.LogInformation("Loaded header id {0}", value);
                        Session["loaded_header"] = value;
                    }
                    else
                    {
                        WebhostEventLog.CommentLog.LogWarning("Invalid header id {0}", value);
                        Session["loaded_header"] = -1;
                        ResetPage();
                    }
                }
            }
        }

        public int LoadedCommentId
        {
            get
            {
                if (Session["loaded_comment"] != null)
                {
                    return (int)Session["loaded_comment"];
                }
                else return -1;
            }
            protected set
            {
                if (value == -1)
                {
                    WebhostEventLog.CommentLog.LogInformation("Deselected Student Comment.");
                    Session["loaded_comment"] = -1;
                    return;
                }

                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.StudentComments.Where(hdr => hdr.id == value).Count() > 0)
                    {
                        Session["loaded_comment"] = value;
                        WebhostEventLog.CommentLog.LogInformation("Loaded Student Comment id {0}", value);
                    }
                    else
                    {
                        WebhostEventLog.CommentLog.LogWarning("Invalid Student Comment id {0}", value);
                        Session["loaded_comment"] = -1;
                    }
                }
            }
        }

        public int SelectedSectionId
        {
            get
            {
                if (Session["selected_section"] != null)
                {
                    return (int)Session["selected_section"];
                }
                else
                    return -1;
            }
            protected set
            {
                if (value == -1)
                {
                    WebhostEventLog.CommentLog.LogInformation("Deselected Section.");
                    Session["selected_section"] = -1;
                    return;
                }

                using(WebhostEntities db = new WebhostEntities())
                {
                    if (db.Sections.Where(s => s.id == value).Count() > 0)
                    {
                        WebhostEventLog.CommentLog.LogInformation("Selected section id {0}", value);
                        Session["selected_section"] = value;
                    }
                    else
                    {
                        WebhostEventLog.CommentLog.LogWarning("Invalide Section id {0}", value);
                        Session["selected_section"] = -1;
                    }
                }
            }
        }

        public int SelectedStudentId
        {
            get
            {
                if (Session["selected_student"] != null)
                {
                    return (int)Session["selected_student"];
                }
                else
                    return -1;
            }
            protected set
            {
                if (value == -1)
                {
                    WebhostEventLog.CommentLog.LogInformation("Deselected Student.");
                    Session["selected_student"] = -1;
                    return;
                }
                using(WebhostEntities db = new WebhostEntities())
                {
                    if (db.Students.Where(s => s.ID == value).Count() > 0)
                    {
                        WebhostEventLog.CommentLog.LogInformation("Selected Student id {0}", value);
                        Session["selected_student"] = value;
                    }
                    else
                    {
                        Session["selected_student"] = -1;
                        WebhostEventLog.CommentLog.LogWarning("Invalid Student Id {0}", value);
                    }
                }
            }
        }
        
        public String HeaderHTML
        {
            get
            {
                if (Session["header_html"] != null)
                {
                    return (String)Session["header_html"];
                }
                else return "";
            }
            protected set
            {
                Session["header_html"] = value;
            }
        }

        public String StudentCommentHTML
        {
            get
            {
                if (Session["comment_html"] != null)
                {
                    return (String)Session["comment_html"];
                }
                else return "";
            }
            protected set
            {
                Session["comment_html"] = value;
            }
        }

        public void ResetPage()
        {
            State.log.WriteLine("{1} {0}:  Beginning Comment Letter Page From Scratch.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
            WebhostEventLog.CommentLog.LogInformation("Beginning Comment Letter Page From Scratch.");
            using (WebhostEntities db = new WebhostEntities())
            {
                State.log.WriteLine("{1} {0}:  Database Connection Established.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                Faculty faculty = db.Faculties.Find(FacultyId);
                if(faculty == null)
                {
                    WebhostEventLog.CommentLog.LogError("Faculty ID {0} is invalid.", FacultyId);
                    throw new InvalidOperationException("Faculty Id is Invalid");
                }
                State.log.WriteLine("{1} {0}:  Editing Comments as {2} {3}", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), faculty.FirstName, faculty.LastName);

                EditorLabel.Text = String.Format("Comments for {0} {1}", faculty.FirstName, faculty.LastName);

                int year = DateRange.GetCurrentAcademicYear();
                int term = SelectedTermId == -1? Import.GetCurrentOrLastTerm(): SelectedTermId;

                if(SelectedTermId != -1)
                {
                    Term t = db.Terms.Find(SelectedTermId);
                    year = t.AcademicYearID;
                }

                if(SelectedTermId != -1)
                {
                    EditorLabel.Text = "Select a class to Edit it's header, or load Students.";
                }
                else
                {
                    EditorLabel.Text = "To change terms, select from the dropdown.  Or continue to edit classes for the current term.";
                    TermDDL.ClearSelection();
                    TermDDL.SelectedValue = Convert.ToString(term);
                }
                TermDDL.DataSource = (from t in db.Terms
                                      where t.AcademicYearID == year
                                      select t).ToList();
                TermDDL.DataTextField = "Name";
                TermDDL.DataValueField = "id";
                TermDDL.DataBind();

                List<int> currentSections = new List<int>();

                if (DepartmentHeadMode == -1)
                {
                    currentSections = faculty.Sections.Where(sec => sec.Course.AcademicYearID == year && sec.Terms.Where(t => t.id == term).Count() > 0).Select(sec => sec.id).ToList();
                    State.log.WriteLine("{1} {0}:  Found {2} current sections.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), currentSections.Count);
                    LogInformation("Got {0} sections for this period.", currentSections.Count);
                }
                else
                {
                    State.log.WriteLine("DepartmentHead mode enabled.");
                    currentSections = db.Sections.Where(sec => sec.Course.DepartmentID == DepartmentHeadMode && sec.Course.AcademicYearID == year && sec.Terms.Where(t => t.id == term).Count() > 0).Select(sec=>sec.id).ToList();
                    State.log.WriteLine("{1} {0}:  Found {2} current sections.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), currentSections.Count);
                    LogInformation("Department Head Mode:  found {0} sections this period.", currentSections.Count);
                }

                ClassSelectCmbBx.DataSource = SectionListItem.GetDataSource(currentSections);
                ClassSelectCmbBx.DataTextField = DepartmentHeadMode == -1 ? "Text" : "ExtendedText";
                ClassSelectCmbBx.DataValueField = "ID";
                ClassSelectCmbBx.DataBind();

                StudentSelectCmbBx.Visible = false;
                HeaderPanel.Visible = false;
                StudentPanel.Visible = false;

                HeaderHTML = "";
                StudentCommentHTML = "";

                SelectedSectionId = -1;
                SelectedStudentId = -1;
                LoadedHeaderId = -1;
                LoadedCommentId = -1;
            }
            HeaderBtn.Visible = false;
            PreviewBtn.Visible = false;
            DownloadClassBtn.Visible = false;
            ClassReviewBtn.Visible = false;

            State.log.WriteLine("{1} {0}:  End of Page Reset Method.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
            WebhostEventLog.CommentLog.LogInformation("Page Reset Successful.");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                ResetPage();
            }
        }

        protected void ClassSelectCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Saving)
            {
                State.log.WriteLine("{1} {0}:  Aborted Changing Comment while Saving.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                WebhostEventLog.CommentLog.LogWarning("Aborted changing comment while saving.");
                return;
            }
            State.log.WriteLine("{1} {0}:  Loading Selected Course.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
            try
            {
                SelectedSectionId = Convert.ToInt32(ClassSelectCmbBx.SelectedValue);
            }
            catch(Exception ex)
            {
                State.log.WriteLine("{1} {0}:  Failed:  {2}", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), ex.Message);
                WebhostEventLog.CommentLog.LogError("Failed to select section:  {0}", ex.Message);
                return;
            }

            StudentSelectCmbBx.Visible = true;
            int term = SelectedTermId == -1 ? Import.GetCurrentOrLastTerm() : SelectedTermId;
            using(WebhostEntities db = new WebhostEntities())
            {
                Section section = db.Sections.Find(SelectedSectionId);
                if(section == null)
                {
                    WebhostEventLog.CommentLog.LogError("Unable to locate section id {0}.", SelectedSectionId);
                    throw new InvalidOperationException("Invalid Section Id");
                }
                State.log.WriteLine("{1} {0}:  Loading Section [{2}] {3}", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), section.Block.LongName, section.Course.Name);
                WebhostEventLog.CommentLog.LogInformation("Loading Section [{0}] {1}", section.Block.LongName, section.Course.Name);
                // Load Header Paragraph.
                if(section.CommentHeaders.Where(hdr => hdr.TermIndex == term).Count() > 0)
                {
                    if(section.CommentHeaders.Where(hdr => hdr.TermIndex == term).Count() > 1)  // PROBLEM!
                    {
                        MailControler.MailToWebmaster("Class has Too Many header paragraphs....", String.Format("{0} objects match header paragraph for [{1}] {2} {3} comments.", 
                            section.CommentHeaders.Where(hdr => hdr.TermIndex == term).Count(), section.Block.LongName, section.Course.Name, section.Terms.Where(t => t.id == term).Single().Name), ((BasePage)Page).user);

                        State.log.WriteLine("{1} {0}:  Too many Class header paragraphs are stored in the database!!!", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                        /*********************************************************************
                         * TODO:
                         * Handle This some how!
                         * 
                         * Make a pop-up to choose which to keep.
                         * 
                         *********************************************************************/

                        State.log.WriteLine("{1} {0}:  Aborting the defunct Header Loader.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                        WebhostEventLog.CommentLog.LogError("Too many Class Header paragraphs stored for section {0} in term id {1}", SelectedSectionId, term);
                        return;
                    }


                    State.log.WriteLine("{1} {0}:  Getting Comment Header HTML.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                    CommentHeader header = section.CommentHeaders.Where(hdr => hdr.TermIndex == term).Single();

                    LoadedHeaderId = header.id;

                    HeaderHTML = header.HTML;
                    HeaderEditor.Content = header.HTML;

                    State.log.WriteLine("{1} {0}:  Done.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                    WebhostEventLog.CommentLog.LogInformation("Successfully Loaded header id {0}", LoadedHeaderId);
                }
                else
                {
                    State.log.WriteLine("{0} {1}:  No Header exisits--creating new header.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    WebhostEventLog.CommentLog.LogInformation("Creating a new header paragraph object.");
                    // Create Paragraph!
                    CommentHeader header = new CommentHeader()
                    {
                        id = db.CommentHeaders.Count() > 0 ? db.CommentHeaders.OrderBy(h => h.id).ToList().Last().id + 1 : 0,
                        HTML = "",
                        SectionIndex = SelectedSectionId,
                        TermIndex = term
                    };
                    db.CommentHeaders.Add(header);
                    db.SaveChanges();

                    LoadedHeaderId = header.id;
                    HeaderEditor.Content = "";

                    State.log.WriteLine("{0} {1}:  Saved new blank header to database.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                }

                State.log.WriteLine("{1} {0}:  Loading Student Roster.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());

                StudentPanel.Visible = false;
                StudentSelectCmbBx.DataSource = (from student in section.Students
                                                 orderby student.LastName, student.FirstName
                                                 select new
                                                 {
                                                     Name = student.FirstName + " " + student.LastName,
                                                     ID = student.ID
                                                 }).ToList();
                StudentSelectCmbBx.DataTextField = "Name";
                StudentSelectCmbBx.DataValueField = "ID";
                StudentSelectCmbBx.DataBind();

                EditorLabel.Text = "Editing Comment Header.  Select a Student to load the Individual Comment.";

                PreviewBtn.Visible = false;
                DownloadClassBtn.Visible = true;
                ClassReviewBtn.Visible = true;
                HeaderPanel.Visible = true;
                HeaderBtn.Visible = false;
                State.log.WriteLine("{0} {1}:  Completed Loading Class Header.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
            }
        }

        protected void StudentSelectCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommentGrades.ResetBar();
            State.log.WriteLine("{0} {1}:  Loading new student selection.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
            if (Saving)
            {
                State.log.WriteLine("{1} {0}:  Aborted Changing Comment while Saving.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                WebhostEventLog.CommentLog.LogWarning("Aborted changing comment while saving.");
                return;
            }
            if(LoadedHeaderId == -1)
            {
                State.log.WriteLine("{0} {1}:  No Header is Loaded... There's a problem...", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                StudentSelectCmbBx.DataSource = new List<string>();
                StudentSelectCmbBx.DataBind();
                WebhostEventLog.CommentLog.LogError("Cannot Select a student while no Header is loaded..."); 
                return;
            }

            try
            {
                SelectedStudentId = Convert.ToInt32(StudentSelectCmbBx.SelectedValue);
            }
            catch (Exception ex)
            {
                State.log.WriteLine("{1} {0}:  Failed:  {2}", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(), ex.Message);
                WebhostEventLog.CommentLog.LogError("Failed to select Student: {0}", ex.Message);
                return;
            }

            using(WebhostEntities db = new WebhostEntities())
            {
                CommentHeader header = db.CommentHeaders.Where(hdr => hdr.id == LoadedHeaderId).Single();
                // check for existing comments.
                if(header.StudentComments.Where(com => com.StudentID == SelectedStudentId).Count() > 0)
                {
                    // check for multiples.
                    if(header.StudentComments.Where(com => com.StudentID == SelectedStudentId).Count() > 1)
                    {
                        State.log.WriteLine("{1} {0}:  Something went wrong!  Too many comments for Student.id={2} and CommentHeader.id={3}", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString(),SelectedStudentId, LoadedHeaderId);
                        MailControler.MailToWebmaster("Class has Too Many comment paragraphs....", String.Format("{0} objects match comment paragraph for Student.id={4} in [{1}] {2} {3} comments.",
                           header.StudentComments.Where(com => com.StudentID == SelectedStudentId).Count(), header.Section.Block.LongName, header.Section.Course.Name, header.Term.Name), ((BasePage)Page).user);

                        WebhostEventLog.CommentLog.LogError("Something went wrong!  Too many comments for Student.id={0} and CommentHeader.id={1}", SelectedStudentId, LoadedHeaderId);

                        /*********************************************************************
                         * TODO:
                         * Handle This some how!
                         * 
                         * Make a pop-up to choose which to keep.
                         * 
                         *********************************************************************/

                        State.log.WriteLine("{1} {0}:  Aborting the defunct Header Loader.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                        return;
                    }

                    // Load the unique comment.

                    StudentComment comment = header.StudentComments.Where(com => com.StudentID == SelectedStudentId).Single();
                    LoadedCommentId = comment.id;

                    State.log.WriteLine("{0} {1}:  Loading unique comment id={2}", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString(), LoadedCommentId);
                    WebhostEventLog.CommentLog.LogInformation("Loaded Comment id {0}", LoadedCommentId);
                    StudentCommentHTML = comment.HTML;
                    CommentGrades.EffortGradeID = comment.EffortGradeID;
                    CommentGrades.FinalGradeID = comment.FinalGradeID;
                    CommentGrades.ExamGradeID = comment.ExamGradeID;
                    CommentGrades.TrimesterGradeID = comment.TermGradeID;
                    StudentEditor.Content = comment.HTML;

                    StudentNameLabel.Text = String.Format("Comment for {0} {1}", comment.Student.FirstName, comment.Student.LastName);

                }
                else
                {
                    // Create new blank comment.
                    State.log.WriteLine("{0} {1}:  Creating a new Student Comment paragraph.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    WebhostEventLog.CommentLog.LogInformation("Creating new student comment.");
                    StudentComment comment = new StudentComment()
                    {
                        id = db.StudentComments.Count() > 0 ? db.StudentComments.OrderBy(com => com.id).ToList().Last().id + 1 : 0,
                        StudentID = SelectedStudentId,
                        Student = db.Students.Where(s => s.ID == SelectedStudentId).Single(),
                        HeaderIndex = header.id,
                        HTML = "",
                        TermGradeID = CommentGrades.AFDefault,
                        EffortGradeID = CommentGrades.EffortDefault,
                        FinalGradeID = CommentGrades.AFDefault,
                        ExamGradeID = CommentGrades.AFDefault
                    };

                    
                    db.StudentComments.Add(comment);
                    db.SaveChanges();
                    LoadedCommentId = comment.id;
                    comment = db.StudentComments.Where(c => c.id == LoadedCommentId).Single();
                    StudentEditor.Content = "";
                    StudentNameLabel.Text = String.Format("Comment for {0} {1}", comment.Student.FirstName, comment.Student.LastName);
                    State.log.WriteLine("{0} {1}:  New blank comment is saved to the database.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                }

                EditorLabel.Text = "Editing Student Individual Comment.";
                HeaderBtn.Visible = true;
                HeaderPanel.Visible = false;
                StudentPanel.Visible = true;
                PreviewBtn.Visible = true;
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            if(SelectedSectionId == -1)
            {
                State.log.WriteLine("{0} {1}:  Cannot Save with no section selected.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                LogWarning("Cannot save with no section selected.");
                return;
            }
            if (Saving)
            {
                State.log.WriteLine("{1} {0}:  Aborted multiple Saving attempts.", DateTime.Now.ToLongTimeString(), DateTime.Today.ToShortDateString());
                LogWarning("Aborted multiple Saving Attempts.");
                return;
            }
            using (WebhostEntities db = new WebhostEntities())
            {
                Saving = true;
                if (HeaderPanel.Visible)
                {
                    CommentHeader header = db.CommentHeaders.Where(h => h.id == LoadedHeaderId).Single();
                    State.log.WriteLine("{0} {1}:  Saving Header Paragraph.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    // Save the Header!
                    if(HeaderEditor.Content.Length < header.HTML.Length)
                    {
                        State.log.WriteLine("{0} {1}:  Warning--suspicious overwrite:{2}Original:{2}{3}{2}Replacement:{2}{4}", DateTime.Today.ToShortDateString(),
                                            DateTime.Now.ToLongTimeString(), Environment.NewLine, header.HTML, HeaderEditor.Content);
                        WebhostEventLog.CommentLog.LogWarning("Suspicious Overwrite:{0}Original:{0}{1}{0}========================================================{0}Replacement:{0}{2}",
                            Environment.NewLine, header.HTML, HeaderEditor.Content);
                    }

                    String html = HeaderEditor.Content;
                    String temp = html;
                    html = CommentLetter.CleanTags(html);

                    if(!temp.Equals(html))
                    {
                        WebhostEventLog.CommentLog.LogWarning("CleanTags method changed:{0}{1}{0}============================================================={0}Into:{0}{2}",
                            Environment.NewLine, temp, html);
                    }

                    WebhostEventLog.CommentLog.LogInformation("Saving Header:{0}{1}", Environment.NewLine, html);

                    HeaderEditor.Content = html;

                    header.HTML = HeaderEditor.Content;
                    HeaderHTML = HeaderEditor.Content;
                }

                if(LoadedCommentId != -1)
                {
                    State.log.WriteLine("{0} {1}:  Saving Student Comment.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                    StudentComment comment = db.StudentComments.Where(com => com.id == LoadedCommentId).Single();

                    if(StudentEditor.Content.Length < comment.HTML.Length)
                    {
                        State.log.WriteLine("{0} {1}:  Warning--suspicious overwrite:{2}Original:{2}{3}{2}Replacement:{2}{4}", DateTime.Today.ToShortDateString(),
                                            DateTime.Now.ToLongTimeString(), Environment.NewLine, comment.HTML, StudentEditor.Content);
                        WebhostEventLog.CommentLog.LogWarning("Suspicious Overwrite:{0}Original:{0}{1}{0}========================================================{0}Replacement:{0}{2}",
                            Environment.NewLine, comment.HTML, StudentEditor.Content);
                    }


                    String html = StudentEditor.Content;

                    String temp = html;
                    html = CommentLetter.CleanTags(html);

                    if (!temp.Equals(html))
                    {
                        WebhostEventLog.CommentLog.LogWarning("CleanTags method changed:{0}{1}{0}============================================================={0}Into:{0}{2}",
                            Environment.NewLine, temp, html);
                    }

                    WebhostEventLog.CommentLog.LogInformation("Saving Comment for {2} {3}:{0}{1}", Environment.NewLine, html, comment.Student.FirstName, comment.Student.LastName);

                    StudentEditor.Content = html;

                    comment.HTML = StudentEditor.Content;
                    comment.EffortGradeID = CommentGrades.EffortGradeID;
                    comment.FinalGradeID = CommentGrades.FinalGradeID;
                    comment.TermGradeID = CommentGrades.TrimesterGradeID;
                    comment.ExamGradeID = CommentGrades.ExamGradeID;
                    StudentCommentHTML = StudentEditor.Content;
                }

                db.SaveChanges();
                State.log.WriteLine("{0} {1}:  Changes saved to the Database.", DateTime.Today.ToShortDateString(), DateTime.Now.ToLongTimeString());
                WebhostEventLog.CommentLog.LogInformation("Save Complete.");
            }

            Saving = false;
        }

        protected void PreviewBtn_Click(object sender, EventArgs e)
        {
            SaveBtn_Click(sender, e);
            if(SelectedStudentId != -1)
            {
                //Response.Redirect((new CommentLetter(LoadedCommentId)).Publish());
                WebhostMySQLConnection.EVOPublishing.CommentLetter letter = new WebhostMySQLConnection.EVOPublishing.CommentLetter(LoadedCommentId);
                String path = Server.MapPath(String.Format("~/Temp/{0}/{1}{2}/comments/",
                    user.UserName, TermDDL.SelectedItem.Text, DateRange.GetCurrentAcademicYear()));
                String filename = letter.Publish(path).Split('\\').Last();

                Response.Redirect(String.Format("~/Temp/{0}/{1}{2}/comments/{3}",
                    user.UserName, TermDDL.SelectedItem.Text, DateRange.GetCurrentAcademicYear(), filename));
            }                
        }

        protected void ClassReviewBtn_Click(object sender, EventArgs e)
        {
            SaveBtn_Click(sender, e);
            CommentReviewPDF rpdf = new CommentReviewPDF(SelectedSectionId, SelectedTermId == -1? DateRange.GetCurrentOrLastTerm() : SelectedTermId);
            String fileName = Server.MapPath(String.Format("~/Temp/{0}/{1}{2}/proofs/{3}.pdf", 
                user.UserName, TermDDL.SelectedItem.Text, DateRange.GetCurrentAcademicYear(), LoadedHeaderId));
            rpdf.Publish(fileName);
            Response.Redirect(String.Format("~/Temp/{0}/{1}{2}/proofs/{3}.pdf",
                user.UserName, TermDDL.SelectedItem.Text, DateRange.GetCurrentAcademicYear(), LoadedHeaderId));
            /*FileStream fs = new FileStream(fileName, FileMode.Open);
            Response.ClearContent();
            Response.StatusCode = 200;
            Response.ContentType = "application/pdf";
            byte[] buffer = new byte[fs.Length];
            for(int i = 0; i < fs.Length; i++)
            {
                buffer[i] = (byte)fs.ReadByte();
            }
            Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.pdf; size={1}", rpdf.Title, buffer.Length.ToString()));

            Response.BinaryWrite(buffer);
            Response.End();

            MailControler.MailToUser("Proofs are ready for download!", 
                String.Format("https://webhost.dublinschool.org/Temp/{0}/{1}{2}/proofs/{3}.pdf",
                    user.UserName, TermDDL.SelectedItem.Text, DateRange.GetCurrentAcademicYear(), LoadedHeaderId),
                    user, "noreply@dublinschool.org", "No Reply", fileName);
          */
        }

        protected void DownloadClassBtn_Click(object sender, EventArgs e)
        {
            SaveBtn_Click(sender, e);
            PublishRequest.RequestByClass(((BasePage)Page).user, new List<int>() { SelectedSectionId }, Server.MapPath(String.Format("~/Temp/{0}_{1}", ((BasePage)Page).user.UserName, SelectedSectionId)));
            WebhostEventLog.CommentLog.LogInformation("Requesting Class Publish for section {0}.", SelectedSectionId);
            ShowMessage("Your class has been queued.  You will recieve an email with a download link once the request is completed.");
        }

        protected void DownloadAllBtn_Click(object sender, EventArgs e)
        {
            SaveBtn_Click(sender, e);
            List<int> sectionIds = new List<int>();
            foreach (ListItem item in ClassSelectCmbBx.Items)
            {
                int sectionId = Convert.ToInt32(item.Value);
                sectionIds.Add(sectionId);
            }

            PublishRequest.RequestByClass(((BasePage)Page).user, sectionIds, Server.MapPath(String.Format("~/Temp/{0}_classes", ((BasePage)Page).user.UserName)));
            LogInformation("Requesting All Comments to be published.");
            ShowMessage("Your request has been queued.  You will recieve an email with a download link once the request is completed.  If you have a lot of students, this may take some time.");
        }

        protected void ReloadBtn_Click(object sender, EventArgs e)
        {
            if(HeaderPanel.Visible)
            {
                HeaderEditor.Content = HeaderHTML;
            }

            if(StudentEditor.Visible)
            {
                StudentEditor.Content = StudentCommentHTML;
            }
        }

        protected void TermSelBtn_Click(object sender, EventArgs e)
        {
            SelectedTermId = Convert.ToInt32(TermDDL.SelectedValue);
            EditorLabel.Text = "Select A Class to Load the Header.";
            ResetPage();
        }

        protected void ShowPFWBtn_Click(object sender, EventArgs e)
        {
            LogInformation("Toggling Paste From Word to {0}", PFWPanel.Visible ? "Off" : "On");
            PFWPanel.Visible = !PFWPanel.Visible;
            ShowPFWBtn.Text = PFWPanel.Visible ? "Cancel Paste From Word" : "Paste From Word";
        }

        protected void DoPFWBtn_Click(object sender, EventArgs e)
        {
            Regex lineBreak = new Regex("(\r)?\n");
            String text = PFWInput.Text;
            State.log.WriteLine("Pasting from Word:{0}{1}{0}_______________________________________________{0}", Environment.NewLine, text);
            WebhostEventLog.CommentLog.LogInformation("Pasting from Word:{0}{1}{0}_______________________________________________{0}", Environment.NewLine, text);
            foreach(Match match in lineBreak.Matches(text))
            {
                text = text.Replace(match.Value, "<br/>");
            }

            if(HeaderPanel.Visible)
            {
                HeaderEditor.Content += text;
            }
            else if(StudentEditor.Visible)
            {
                StudentEditor.Content += text;
            }

            PFWInput.Text = "";
            PFWPanel.Visible = false;
            ShowPFWBtn.Text = "Paste From Word";
            SaveBtn_Click(sender, e);
        }

        protected void ShowMessage(String message)
        {
            SuccessMessage.Text = message;
            MessagePanel.Visible = true;
        }

        protected void SuccessOK_Click(object sender, EventArgs e)
        {
            SuccessMessage.Text = "";
            MessagePanel.Visible = false;
        }
    }
}