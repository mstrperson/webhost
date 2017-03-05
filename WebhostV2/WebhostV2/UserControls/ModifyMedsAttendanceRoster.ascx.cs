using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class ModifyMedsAttendanceRoster : LoggingUserControl
    {
        protected Dictionary<String,int> MedsSections
        {
            get
            {
                if(Session["med_sections"] == null)
                {
                    return new Dictionary<string, int>();
                }
                else
                {
                    return (Dictionary<string, int>)Session["med_sections"];
                }
            }
        }

        protected struct MedTimes
        {
            public bool morning;
            public bool lunch;
            public bool dinner;
            public bool bedtiem;
        }

        protected MedTimes merge(MedTimes a, MedTimes b)
        {
            return new MedTimes() { morning = a.morning || b.morning, lunch = a.lunch || b.lunch, dinner = a.dinner || b.dinner, bedtiem = a.bedtiem || b.bedtiem };
        }

        protected void LoadTable()
        {
            int year = DateRange.GetCurrentAcademicYear();
            using (WebhostEntities db = new WebhostEntities())
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();

                List<Section> sections = db.Courses.Where(c => c.Name.Equals("Meds Attendance") && c.AcademicYearID == year).Single().Sections.ToList();
                Dictionary<int, MedTimes> studentMeds = new Dictionary<int, MedTimes>();
                foreach (Section sec in sections)
                {
                    if (!dict.Keys.Contains(sec.Block.Name.Split(' ')[0]))
                        dict.Add(sec.Block.Name.Split(' ')[0], sec.id);

                    MedTimes mt = new MedTimes() { morning = false, lunch = false, dinner = false, bedtiem = false };
                    switch (sec.Block.Name.Split(' ')[0])
                    {
                        case "Morning": mt.morning = true; break;
                        case "Lunch": mt.lunch = true; break;
                        case "Dinner": mt.dinner = true; break;
                        case "Bedtime": mt.bedtiem = true; break;
                        default: break;
                    }

                    foreach (Student student in sec.Students.ToList())
                    {
                        if (studentMeds.ContainsKey(student.ID))
                        {
                            studentMeds[student.ID] = merge(studentMeds[student.ID], mt);
                        }
                        else
                        {
                            studentMeds.Add(student.ID, mt);
                        }
                    }
                    MedsTable.Rows.Clear();
                    MedsTable.Rows.Add(MedsAttendanceViewRow.Header);
                    foreach (int id in studentMeds.Keys)
                    {
                        StudentListItem item = new StudentListItem(id);
                        MedsAttendanceViewRow row = new MedsAttendanceViewRow(item.Text, studentMeds[id].morning, studentMeds[id].lunch, studentMeds[id].dinner, studentMeds[id].bedtiem);
                        MedsTable.Rows.Add(row);
                    }
                }

                Session["med_sections"] = dict;

                StudentSelectComboBox.DataSource = StudentListItem.GetDataSource(db.Students.Where(s => s.isActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).Select(s => s.ID).ToList());
                StudentSelectComboBox.DataTextField = "Text";
                StudentSelectComboBox.DataValueField = "ID";
                StudentSelectComboBox.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                LoadTable();
            }
        }

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int studentId = Convert.ToInt32(StudentSelectComboBox.SelectedValue);
                MedTimes mt = new MedTimes()
                {
                    morning = MorningCB.Checked,
                    lunch = LunchCB.Checked,
                    dinner = DinnerCB.Checked,
                    bedtiem = BedtimeCB.Checked
                };

                Student student = db.Students.Where(s => s.ID == studentId).Single();

                int mid = MedsSections["Morning"];
                Section morning = db.Sections.Where(sec => sec.id == mid).Single();
                int lid = MedsSections["Lunch"];
                Section lunch = db.Sections.Where(sec => sec.id == lid).Single();
                int did = MedsSections["Dinner"];
                Section dinner = db.Sections.Where(sec => sec.id == did).Single();
                int bid = MedsSections["Bedtime"];
                Section bedtime = db.Sections.Where(sec => sec.id == bid).Single();
                if (mt.morning)
                {
                    morning.Students.Add(student);
                }
                else if(morning.Students.Contains(student))
                {
                    morning.Students.Remove(student);
                }

                if (mt.lunch)
                {
                    lunch.Students.Add(student);
                }
                else if (lunch.Students.Contains(student))
                {
                    lunch.Students.Remove(student);
                }

                if (mt.dinner)
                {
                    dinner.Students.Add(student);
                }
                else if (dinner.Students.Contains(student))
                {
                    dinner.Students.Remove(student);
                }

                if (mt.bedtiem)
                {
                    bedtime.Students.Add(student);
                }
                else if (bedtime.Students.Contains(student))
                {
                    bedtime.Students.Remove(student);
                }

                db.SaveChanges();
            }

            LoadTable();
        }
    }
}