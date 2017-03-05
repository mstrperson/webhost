using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class LivingLevelsBuilder : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    List<int> indep = new List<int>();
                    List<int> lib = new List<int>();
                    List<int> super = new List<int>();

                    foreach (Student student in db.Students.Where(s => s.isActive))
                    {
                        switch (student.AcademicLevel)
                        {
                            case 3: indep.Add(student.ID); break;
                            case 2: lib.Add(student.ID); break;
                            default: super.Add(student.ID); break;
                        }

                        IndependentSelector.AddStudent(indep);
                        LibrarySelector.AddStudent(lib);
                        SupervisedSelector.AddStudent(super);
                    }
                }
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<int> done = new List<int>();
                foreach(int id in IndependentSelector.GroupIds)
                {
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    student.AcademicLevel = 3;
                    done.Add(id);
                }

                foreach (int id in LibrarySelector.GroupIds)
                {
                    if (done.Contains(id)) continue;
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    student.AcademicLevel = 2;
                    done.Add(id);
                }

                foreach (int id in SupervisedSelector.GroupIds)
                {
                    if (done.Contains(id)) continue;
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    student.AcademicLevel = 1;
                }

                db.SaveChanges();
            }
        }
    }
}