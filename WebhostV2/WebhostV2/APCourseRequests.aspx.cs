using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class APCourseRequests : BasePage
    {
        /// <summary>
        /// FIX THIS LATER.
        /// </summary>
        protected static int TermId = 4;

        protected int LoadedId
        {
            get
            {
                if (Session["apid"] != null)
                    return (int)Session["apid"];
                else
                    return -1;
            }
            set
            {
                Session["apid"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    List<int> sids = db.Students.Where(s => s.CourseRequests.Where(cr => cr.TermId == TermId && cr.RequestableCourse.Course.Name.Contains("AP")).Count() > 0).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).Select(s => s.ID).ToList();
                    StudentSelectDDL.DataSource = StudentListItem.GetDataSource(sids);
                    StudentSelectDDL.DataTextField = "Text";
                    StudentSelectDDL.DataValueField = "ID";
                    StudentSelectDDL.DataBind();

                    CourseRequestDDL.Enabled = false;
                    SelectCR.Enabled = false;

                    ApprovalPanel.Visible = false;
                }
            }
        }

        protected void SelectStudent_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                CourseRequestDDL.Enabled = true;
                SelectCR.Enabled = true;

                ApprovalPanel.Visible = false;
                int sid = Convert.ToInt32(StudentSelectDDL.SelectedValue);
                Student student = db.Students.Where(s => s.ID == sid).Single();

                List<int> crids = student.CourseRequests.Where(cr => cr.TermId == TermId && cr.RequestableCourse.Course.Name.Contains("AP")).Select(cr => cr.id).ToList();
                CourseRequestDDL.DataSource = CourseRequestListItem.GetDataSource(crids);
                CourseRequestDDL.DataTextField = "Text";
                CourseRequestDDL.DataValueField = "ID";
                CourseRequestDDL.DataBind();
                LoadedId = -1;
            }
        }

        protected void SelectCR_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                LoadedId = -1;
                int crid = Convert.ToInt32(CourseRequestDDL.SelectedValue);
                WebhostMySQLConnection.CourseRequest courseRequest = db.CourseRequests.Where(cr => cr.id == crid).Single();

                RequestedCourseName.Text = courseRequest.RequestableCourse.Course.Name;


                int sid = Convert.ToInt32(StudentSelectDDL.SelectedValue);
                Student student = db.Students.Where(s => s.ID == sid).Single();

                int termId = DateRange.GetCurrentOrLastTerm();
                List<int> currentClasses = student.Sections.Where(sec => sec.Terms.Select(t => t.id).Contains(termId)).Select(sec => sec.id).ToList();
                CurrentCourseDDL.DataSource = SectionListItem.GetDataSource(currentClasses);
                CurrentCourseDDL.DataTextField = "Text";
                CurrentCourseDDL.DataValueField = "ID";
                CurrentCourseDDL.DataBind();


                SecondaryCourseDDL.DataSource = SectionListItem.GetDataSource(currentClasses);
                SecondaryCourseDDL.DataTextField = "Text";
                SecondaryCourseDDL.DataValueField = "ID";
                SecondaryCourseDDL.DataBind();

                int year = DateRange.GetCurrentAcademicYear();

                GradeTable stdaf = db.GradeTables.Where(gt => gt.AcademicYearID == year && gt.Name.Equals("Standard A-F Scale")).Single();

                CurrentCourseGradeDDL.DataSource = stdaf.GradeTableEntries.ToList();
                CurrentCourseGradeDDL.DataTextField = "Name";
                CurrentCourseGradeDDL.DataValueField = "id";
                CurrentCourseGradeDDL.DataBind();

                SecondaryCourseGradeDDL.DataSource = stdaf.GradeTableEntries.ToList();
                SecondaryCourseGradeDDL.DataTextField = "Name";
                SecondaryCourseGradeDDL.DataValueField = "id";
                SecondaryCourseGradeDDL.DataBind();

                List<int> facultyIds = db.Faculties.Select(f => f.ID).ToList();

                CurrentTeacherDDL.DataSource = FacultyListItem.GetDataSource(facultyIds);
                CurrentTeacherDDL.DataTextField = "Text";
                CurrentTeacherDDL.DataValueField = "ID";
                CurrentTeacherDDL.DataBind();

                DeptHeadDDL.DataSource = FacultyListItem.GetDataSource(facultyIds);
                DeptHeadDDL.DataTextField = "Text";
                DeptHeadDDL.DataValueField = "ID";
                DeptHeadDDL.DataBind();

                GradeTable approval = db.GradeTables.Where(gt => gt.AcademicYearID == year && gt.Name.Equals("AP Approval")).Single();

                ApprovalDDL.DataSource = approval.GradeTableEntries.ToList();
                ApprovalDDL.DataTextField = "Name";
                ApprovalDDL.DataValueField = "id";
                ApprovalDDL.DataBind();

                ApprovalPanel.Visible = true;

                if (courseRequest.APRequests.Count > 0)
                {
                    APRequest apcr = courseRequest.APRequests.FirstOrDefault();
                    LoadedId = apcr.id;
                    if (apcr.Sections.Count > 0)
                    {
                        List<Section> secs = apcr.Sections.ToList();
                        CurrentCourseDDL.ClearSelection();
                        CurrentCourseDDL.SelectedValue = Convert.ToString(secs[0].id);
                        if(student.StudentComments.Where(com => com.CommentHeader.SectionIndex == secs[0].id && com.CommentHeader.TermIndex == termId).Count() > 0)
                        {
                            StudentComment comment = student.StudentComments.Where(com => com.CommentHeader.SectionIndex == secs[0].id && com.CommentHeader.TermIndex == termId).Single();
                            CurrentCourseGradeDDL.ClearSelection();
                            CurrentCourseGradeDDL.SelectedValue = Convert.ToString(comment.FinalGradeID);
                        }
                        if (secs.Count > 1)
                        {
                            SecondaryCourseDDL.ClearSelection();
                            SecondaryCourseDDL.SelectedValue = Convert.ToString(secs[1].id);
                            if (student.StudentComments.Where(com => com.CommentHeader.SectionIndex == secs[1].id && com.CommentHeader.TermIndex == termId).Count() > 0)
                            {
                                StudentComment comment = student.StudentComments.Where(com => com.CommentHeader.SectionIndex == secs[1].id && com.CommentHeader.TermIndex == termId).Single();
                                SecondaryCourseGradeDDL.ClearSelection();
                                SecondaryCourseGradeDDL.SelectedValue = Convert.ToString(comment.FinalGradeID);
                            }
                        }
                    }

                    CurrentTeacherDDL.ClearSelection();
                    CurrentTeacherDDL.SelectedValue = Convert.ToString(apcr.TeacherSignedBy);

                    DeptHeadDDL.ClearSelection();
                    DeptHeadDDL.SelectedValue = Convert.ToString(apcr.DeptHeadSignedBy);

                    ApprovalDDL.ClearSelection();
                    ApprovalDDL.SelectedValue = Convert.ToString(apcr.ApprovalId);
                }

            }
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int apid = LoadedId == -1 ? (db.APRequests.Count() > 0 ? db.APRequests.OrderBy(ap => ap.id).ToList().Last().id + 1 : 0) : LoadedId;

                APRequest req = new APRequest();
                if (LoadedId != -1)
                {
                    req = db.APRequests.Where(ap => ap.id == LoadedId).Single();
                }
                else
                {
                    req.id = apid;

                    req.CourseRequestId = Convert.ToInt32(CourseRequestDDL.SelectedValue);
                }

                req.Sections.Clear();
                req.ApprovalId = Convert.ToInt32(ApprovalDDL.SelectedValue);
                req.TeacherSignedBy = Convert.ToInt32(CurrentTeacherDDL.SelectedValue);
                req.DeptHeadSignedBy = Convert.ToInt32(DeptHeadDDL.SelectedValue);
                if (CurrentCourseDDL.SelectedIndex >= 0)
                {
                    int secid = Convert.ToInt32(CurrentCourseDDL.SelectedValue);
                    req.Sections.Add(db.Sections.Where(s => s.id == secid).Single());
                }

                if(SecondaryCourseDDL.SelectedIndex >= 0)
                {
                    int secid = Convert.ToInt32(SecondaryCourseDDL.SelectedValue);
                    req.Sections.Add(db.Sections.Where(s => s.id == secid).Single());
                }

                if(LoadedId == -1)
                {
                    db.APRequests.Add(req);
                }

                db.SaveChanges();

                SelectStudent_Click(sender, e);
            }
        }
    }
}