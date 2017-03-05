using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class RequestCourseSelector : LoggingUserControl
    {
        public int CourseRequestId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(RequestId.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if (db.CourseRequests.Where(cr => cr.id == value).Count() <= 0)
                    {
                        DeleteRequestBtn.Visible = false;
                        return;
                    }

                    RequestId.Value = Convert.ToString(value);

                    WebhostMySQLConnection.CourseRequest req = db.CourseRequests.Where(cr => cr.id == value).Single();
                    DeptDDL.SelectedValue = Convert.ToString(req.RequestableCourse.Course.DepartmentID);
                    DeptDDL_SelectedIndexChanged(this, EventArgs.Empty);

                    ClassDDL.SelectedValue = Convert.ToString(req.RequestableCourse.id);
                    ClassDDL.Enabled = false;
                    DeptDDL.Enabled = false;
                    PriorityDDL.Enabled = false;
                    DeleteRequestBtn.Visible = true;

                    PriorityDDL.ClearSelection();
                    PriorityDDL.SelectedIndex = req.IsGlobalAlternate ? 2 : req.IsSecondary ? 1 : 0;
                }
            }
        }

        protected int TermId = DateRange.GetNextTerm();

        public int RequestedCourseId
        {
            get
            {
                return Convert.ToInt32(ClassDDL.SelectedValue);
            }
        }

        public int Priority
        {
            get
            {
                return PriorityDDL.SelectedIndex;
            }
        }


        protected int SelectedDepartmentId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(DeptDDL.SelectedValue);
                }
                catch
                {
                    return -1;
                }
            }
        }

        public void Initialize()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<int> depts = new List<int>();
                foreach (RequestableCourse rc in db.RequestableCourses.Where(r => r.TermId == TermId).ToList())
                {
                    if (!depts.Contains(rc.Course.DepartmentID))
                        depts.Add(rc.Course.DepartmentID);
                }

                DeptDDL.DataSource = DepartmentListItem.GetDataSource(depts);
                DeptDDL.DataTextField = "Text";
                DeptDDL.DataValueField = "ID";
                DeptDDL.DataBind();

                //DeptDDL.SelectedIndex = 0;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Initialize();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    List<int> courses = db.RequestableCourses.Where(rc => rc.TermId.Equals(TermId) && rc.Course.DepartmentID == SelectedDepartmentId).Select(rc => rc.id).ToList();
                    ClassDDL.DataSource = RequestableCourseListItem.GetDataSource(courses);
                    ClassDDL.DataTextField = "Text";
                    ClassDDL.DataValueField = "ID";
                    ClassDDL.DataBind();
                }
            }
        }

        protected void DeptDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                List<int> courses = db.RequestableCourses.Where(rc => rc.TermId.Equals(TermId) && rc.Course.DepartmentID == SelectedDepartmentId).Select(rc => rc.id).ToList();
                ClassDDL.DataSource = RequestableCourseListItem.GetDataSource(courses);
                ClassDDL.DataTextField = "Text";
                ClassDDL.DataValueField = "ID";
                ClassDDL.DataBind();
            }
        }

        protected void DeleteRequestBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                WebhostMySQLConnection.CourseRequest request = db.CourseRequests.Where(cr => cr.id == CourseRequestId).Single();
                request.APRequests.Clear();
                db.CourseRequests.Remove(request);
                //LogWarning("Deleted Course Request of {2} for {0} {1}.", request.Student.FirstName, request.Student.LastName, request.RequestableCourse.Course.Name);
                db.SaveChanges();
                Response.Redirect(Request.RawUrl);
            }
        }
    }
}