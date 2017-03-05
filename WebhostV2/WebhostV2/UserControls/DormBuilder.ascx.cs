using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class DormBuilder : LoggingUserControl
    {
        public int DormId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(DormSelector.SelectedValue);
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    int year = DateRange.GetCurrentAcademicYear();

                    DormSelector.DataSource = db.Dorms.Where(d => d.AcademicYearId == year).ToList();
                    DormSelector.DataTextField = "Name";
                    DormSelector.DataValueField = "id";
                    DormSelector.DataBind();

                    DormHeadSelector.DataSource = (from faculty in db.Faculties
                                                   orderby faculty.LastName, faculty.FirstName
                                                   select new
                                                   {
                                                       Name = faculty.FirstName + " " + faculty.LastName,
                                                       id = faculty.ID
                                                   }).ToList();
                    DormHeadSelector.DataTextField = "Name";
                    DormHeadSelector.DataValueField = "id";
                    DormHeadSelector.DataBind();
                }
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            if (DormId == -1) return;
            using(WebhostEntities db = new WebhostEntities())
            {
                Dorm dorm = db.Dorms.Where(d => d.id == DormId).Single();

                dorm.DormHeadId = Convert.ToInt32(DormHeadSelector.SelectedValue);

                dorm.DormParents.Clear();
                foreach(int id in DormParentSelector.GroupIds)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == id).Single();
                    dorm.DormParents.Add(faculty);
                }

                dorm.Students.Clear();
                foreach(int id in StudentSelector.GroupIds)
                {
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    dorm.Students.Add(student);
                }

                db.SaveChanges();
            }
        }

        protected void DormSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Dorm dorm = db.Dorms.Where(d => d.id == DormId).Single();
                DormHeadSelector.ClearSelection();
                DormHeadSelector.SelectedValue = Convert.ToString(dorm.DormHeadId);

                DormParentSelector.Clear();
                DormParentSelector.AddFaculty(dorm.DormParents.Select(f => f.ID).ToList());

                StudentSelector.Clear();
                StudentSelector.AddStudent(dorm.Students.Select(s => s.ID).ToList());
            }
        }
    }
}