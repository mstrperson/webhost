using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class PermissionEditor : LoggingUserControl
    {
        public int PermissionId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(PermissionIdField.Value);
                }
                catch
                {
                    return -1;
                }
            }
            set
            {
                if (value == -1)
                {
                    PermissionIdField.Value = "-1";
                    return;
                }
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.Permissions.Where(p => p.id == value).Count() > 0)
                    {
                        Permission permission = db.Permissions.Where(p => p.id == value).Single();
                        FacultyGroupSelector1.Clear();
                        FacultyGroupSelector1.AddFaculty(permission.Faculties.Select(f => f.ID).ToList());
                        StudentGroupSelector1.Clear();
                        StudentGroupSelector1.AddStudent(permission.Students.Select(s => s.ID).ToList());
                        PermissionNameInput.Text = permission.Name;
                        PermissionIdField.Value = Convert.ToString(permission.id);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            Regex nwhtspc = new Regex(@"[^\s]+");
            if(!nwhtspc.IsMatch(PermissionNameInput.Text))
            {
                PermissionNameInput.Text = "You must give the permission a Unique Name";
                return;
            }
            using(WebhostEntities db = new WebhostEntities())
            {
                int id = PermissionId;
                
                if (id == -1) id = db.Permissions.Count() > 0 ? db.Permissions.OrderBy(p => p.id).ToList().Last().id + 1 : 0;

                Permission permission;
                if (id != PermissionId)
                {
                    if(db.Permissions.Where(p => p.Name.Equals(PermissionNameInput.Text)).Count() >0)
                    {
                        PermissionNameInput.Text = "A permission with that name already exists.";
                        return;
                    }
                    
                    permission = new Permission()
                    {
                        id = id,
                        Name = PermissionNameInput.Text,
                        Description = NotesInput.Text,
                        AcademicYear = DateRange.GetCurrentAcademicYear()
                    };
                    db.Permissions.Add(permission);
                }
                else
                {
                    permission = db.Permissions.Where(p => p.id == id).Single();
                    permission.Name = PermissionNameInput.Text;
                    permission.Description = NotesInput.Text;
                    permission.Faculties.Clear();
                    permission.Students.Clear();
                }

                foreach(int fid in FacultyGroupSelector1.GroupIds)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == fid).Single();
                    permission.Faculties.Add(faculty);
                }

                foreach(int sid in StudentGroupSelector1.GroupIds)
                {
                    Student student = db.Students.Where(s => s.ID == sid).Single();
                    permission.Students.Add(student);
                }

                db.SaveChanges();
            }
        }

        protected void CreateBtn_Click(object sender, EventArgs e)
        {
            PermissionId = -1;
            PermissionNameInput.Text = "";
            NotesInput.Text = "";
            FacultyGroupSelector1.Clear();
            StudentGroupSelector1.Clear();
        }
    }
}