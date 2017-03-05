using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using System.Text.RegularExpressions;

namespace WebhostV2.UserControls
{
    public partial class WeekendDiscipline : LoggingUserControl
    {
        
        protected int WeekendId
        {
            get
            {
                return DateRange.GetCurrentWeekendId();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void LoadBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.Weekends.Where(w => w.id == WeekendId).Count() > 0)
                {
                    IDField.Value = Convert.ToString(WeekendId);
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                    DetentionListSelector.AddStudent(AttendanceControl.GetOneHourDetention(weekend.id));
                    DetentionListSelector.AddStudent(AttendanceControl.GetTwoHourDetention(weekend.id));
                    CampusedListSelector.AddStudent(AttendanceControl.GetCampusedList(weekend.id));

                    LoadBtn.Visible = false;
                    SaveBtn.Visible = true;
                    DetentionListSelector.Visible = true;
                    CampusedListSelector.Visible = true;

                    return;
                }
            }

            DetentionListSelector.Visible = false;
            CampusedListSelector.Visible = false;
            LoadBtn.Visible = true;
            SaveBtn.Visible = false;
            LoadBtn.Text = "That Weekend Schedule hasn't been started yet.";
        }
        
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();

                //weekend.DetentionList.Clear();

                foreach(int id in DetentionListSelector.GroupIds)
                {
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    //weekend.DetentionList.Add(student);
                }

                //weekend.CampusedStudents.Clear();

                foreach(int id in CampusedListSelector.GroupIds)
                {
                    Student student = db.Students.Where(s => s.ID == id).Single();
                    //weekend.CampusedStudents.Add(student);
                }

                db.SaveChanges();
            }
        }
    }
}