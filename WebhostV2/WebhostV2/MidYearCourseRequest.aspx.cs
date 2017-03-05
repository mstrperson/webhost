using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class MidYearCourseRequest : BasePage
    {
        protected int SelectedStudentId
        {
            get
            {
                if (Session["CR_SID"] != null)
                    return (int)Session["CR_SID"];
                else
                    return -1;
            }
            set
            {
                Session["CR_SID"] = value;
            }
        }

        protected int termId = DateRange.GetNextTerm();

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e); 
            // Note to self--FIX THIS LATER!
            //CourseRequestAdmin1.Visible = this.user.Permissions.Contains(10) || this.user.Permissions.Contains(13);

            int tid = this.AdminMasqueradeTeacherId > -1 ? this.AdminMasqueradeTeacherId : this.user.ID;
            using (WebhostEntities db = new WebhostEntities())
            {
                Term theTerm = db.Terms.Where(t => t.id == termId).Single();
                Faculty faculty = db.Faculties.Where(f => f.ID == tid).Single();

                if (faculty.Permissions.Where(p => p.AcademicYear.Equals(theTerm.AcademicYearID) && (p.Name.Equals("Administrator") || p.Name.Equals("Comment Admins"))).Count() > 0)
                    CourseRequestAdmin1.Visible = true;
                else
                    CourseRequestAdmin1.Visible = false;

                StudentSelectDDL.DataSource = StudentListItem.GetDataSource(faculty.Students.Where(s => s.isActive && s.GraduationYear >= theTerm.AcademicYearID).Select(s => s.ID).ToList());
                StudentSelectDDL.DataTextField = "Text";
                StudentSelectDDL.DataValueField = "ID";
                StudentSelectDDL.DataBind();

                if(SelectedStudentId != -1)
                {
                    StudentSelectDDL.ClearSelection();
                    StudentSelectDDL.SelectedValue = SelectedStudentId.ToString();
                }
            }

            LoadRequests();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.RequestableCourses.Where(rc => rc.TermId == termId).Count() > 0)
                    {
                        StudentSelectDDL.Enabled = true;
                        SelectStudentbtn.Enabled = true;
                        StudentSelectDDL.ToolTip = "";
                    }
                    else
                    {
                        StudentSelectDDL.Enabled = false;
                        SelectStudentbtn.Enabled = false;
                        SelectStudentbtn.ToolTip = "No Course Requests are available at this time.";
                    }
                }
            }
        }

        protected void LoadRequests()
        {
            if(SelectedStudentId == -1)
            {
                SignUpPanel.Visible = false;
                return;
            }

            SignUpPanel.Visible = true;
            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Where(s => s.ID == SelectedStudentId).Single();

                bool cleanedUp = false;
                List<int> reqCourseIds = new List<int>();
                // Clean up bad requests...
                foreach(WebhostMySQLConnection.CourseRequest cr in student.CourseRequests.Where(c => c.TermId == termId).ToList())
                {
                    if(reqCourseIds.Contains(cr.CourseId))
                    {
                        cleanedUp = true;
                        LogWarning("Deleting Duplicate Course Request for {0} {1} - {2}.", student.FirstName, student.LastName, cr.RequestableCourse.Course.Name);
                        db.CourseRequests.Remove(cr);
                    }
                    else
                    {
                        reqCourseIds.Add(cr.CourseId);
                    }
                }

                if(cleanedUp)
                {
                    db.SaveChanges();
                }

                List<int> requests = student.CourseRequests.Where(cr => cr.TermId == termId).Select(cr => cr.id).ToList();
                foreach(int reqId in requests)
                {
                    RequestCourseSelector selector = (RequestCourseSelector)LoadControl("~/UserControls/RequestCourseSelector.ascx");
                    selector.Initialize();
                    selector.CourseRequestId = reqId;
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    cell.Controls.Add(selector);
                    row.Cells.Add(cell);
                    SavedRequestsTable.Rows.Add(row);
                }

                if(student.CourseRequestComments.Where(com => com.TermId == termId).Count() > 0)
                {
                    CourseRequestComment comment = student.CourseRequestComments.Where(com => com.TermId == termId).ToList()[0];
                    NotesEntry.Text = comment.Notes;
                    NotesStatus.Text = "Notes have been saved.";
                    NotesStatus.CssClass = "success_highlight";
                }
                else
                {
                    NotesStatus.Text = "No Notes have been saved.";
                    NotesStatus.CssClass = "";
                }
            }
        }

        protected void AddRequestBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                if(db.CourseRequests.Where(cr => cr.TermId == termId && cr.StudentId == SelectedStudentId && cr.CourseId == NewCourseRequestSelector.RequestedCourseId).Count() > 0)
                {
                    LogError("Multiple Course Requests for the Same class detected.");
                    ErrorLabel.Text = "You have already submitted a course request for this student for this class.";
                    ErrorPanel.Visible = true;
                    return;
                }

                int crn = db.CourseRequests.Count() > 0 ? db.CourseRequests.OrderBy(cr => cr.id).ToList().Last().id + 1 : 0;

                WebhostMySQLConnection.CourseRequest request = new WebhostMySQLConnection.CourseRequest()
                {
                    CourseId = NewCourseRequestSelector.RequestedCourseId,
                    id = crn,
                    IsGlobalAlternate = NewCourseRequestSelector.Priority == 2,
                    IsSecondary = NewCourseRequestSelector.Priority == 1,
                    StudentId = SelectedStudentId,
                    TermId = termId
                };

                db.CourseRequests.Add(request);
                db.SaveChanges();

                State.log.WriteLine("Added new Course Request:");
                State.log.WriteLine("{0} - {1}", SelectedStudentId, request.CourseId);
            }

            Response.Redirect(Request.RawUrl);
        }

        protected void SelectStudentbtn_Click(object sender, EventArgs e)
        {
            SelectedStudentId = Convert.ToInt32(StudentSelectDDL.SelectedValue);
            Response.Redirect(Request.RawUrl);
        }

        protected void CourseRequestAdmin_Masquerade(object sender, EventArgs e)
        {
            this.AdminMasqueradeTeacherId = CourseRequestAdmin1.MasqeradeId;
            Response.Redirect(Request.RawUrl);
        }

        protected void NoteSubmit_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db =  new WebhostEntities())
            {
                Student student = db.Students.Find(SelectedStudentId);
                if(student.CourseRequestComments.Where(com => com.TermId == termId).Count() > 0)
                {
                    CourseRequestComment comment = student.CourseRequestComments.Where(com => com.TermId == termId).ToList()[0];
                    comment.Notes = NotesEntry.Text;
                }
                else
                {
                    CourseRequestComment comment = new CourseRequestComment()
                    {
                        id = db.CourseRequestComments.OrderBy(com => com.id).ToList().Last().id + 1,
                        Notes = NotesEntry.Text,
                        TermId = termId,
                        StudentId = SelectedStudentId
                    };

                    db.CourseRequestComments.Add(comment);
                }

                NotesStatus.Text = "Saved Notes!";
                NotesStatus.CssClass = "success_highlight";

                db.SaveChanges();
            }
        }

        protected void DismissBtn_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = false;
        }
    }
}