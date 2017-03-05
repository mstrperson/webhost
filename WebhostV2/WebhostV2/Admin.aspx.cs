using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.SchoologyAPI;
using WebhostMySQLConnection.AccountManagement;
using WebhostMySQLConnection.Analytics;

namespace WebhostV2
{
    public partial class Admin : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    TermSelect.DataSource = (from term in db.Terms
                                             orderby term.StartDate descending
                                             select new
                                             {
                                                 Text = term.Name + " " + term.StartDate.Year,
                                                 ID = term.id
                                             }).ToList();
                    TermSelect.DataTextField = "Text";
                    TermSelect.DataValueField = "ID";
                    TermSelect.DataBind();
                }
            }
        }

        protected void SchoologyExport_Click(object sender, EventArgs e)
        {
            int termId = -1;
            try
            {
                termId = Convert.ToInt32(TermSelect.SelectedValue);
            }
            catch
            {
                termId = Import.GetCurrentOrLastTerm();
            }
            using(WebhostEntities db = new WebhostEntities())
            {
                Term term = db.Terms.Where(t => t.id == termId).Single();

                Response.Redirect(Import.SchoologyExportPack(term.AcademicYearID, termId, Server));
            }
        }

        protected void MorningMeetingGenBtn_Click(object sender, EventArgs e)
        {
            TextEditor1.Html = AttendanceControl.GenerateMorningMeetingSeatingChart();
        }

        protected void AttendanceDump_Click(object sender, EventArgs e)
        {
            AttendanceControl.AttendanceDump(new DateRange(DateRange.ThisFriday.AddDays(-7), DateRange.ThisFriday.AddDays(-1))).Save(Server.MapPath("~/Temp/AttendanceDump.csv"));
            Response.Redirect("~/Temp/AttendanceDump.csv");
        }

        protected void GetTutorBtn_Click(object sender, EventArgs e)
        {
            CSV csv = new CSV();
            using (WebhostEntities db = new WebhostEntities())
            {
                int year = DateRange.GetCurrentAcademicYear();
                List<Section> tutorials = db.Sections.Where(sec => sec.Course.AcademicYearID == year && sec.Course.Name.Contains("Tutorial")).ToList();
                foreach (Section tutorial in tutorials)
                {
                    Dictionary<String, String> row = new Dictionary<string, string>();
                    row.Add("SectionId", Convert.ToString(tutorial.id));
                    row.Add("SchoologyId", Convert.ToString(tutorial.SchoologyId));
                    row.Add("BlackBaud ID", String.Format("{0}-{1}",tutorial.Course.BlackBaudID, tutorial.SectionNumber));
                    row.Add("Block", tutorial.Block.Name);
                    row.Add("Two Day?", tutorial.Course.Name.Contains("2") ? "X" : "");
                    if(tutorial.Students.Count <= 0)
                    {
                        row.Add("Students", "* No Students *");
                    }
                    else
                    {
                        String list = "";
                        bool first = true;
                        foreach (Student student in tutorial.Students.ToList())
                        {
                            list += String.Format("{0}{1} {2} [{3}]", first ? "" : " | ", student.FirstName, student.LastName, student.GraduationYear);
                            first = false;
                        }
                        row.Add("Students", list);
                    }
                    if(tutorial.Teachers.Count <= 0)
                    {
                        row.Add("Tutors", "* No Tutor *");
                    }
                    else
                    {
                        String list = "";
                        bool first = true;
                        foreach (Faculty tutor in tutorial.Teachers.ToList())
                        {
                            list += String.Format("{0}{1} {2}", first ? "" : " | ", tutor.FirstName, tutor.LastName);
                            first = false;
                        }
                        row.Add("Tutors", list);
                    }

                    csv.Add(row);
                }

                csv.Save(Server.MapPath("~/Temp/tutorials.csv"));
            }

            Response.Redirect("~/Temp/tutorials.csv");
        }

        protected void SchoologyPull_Click(object sender, EventArgs e)
        {
            log.WriteLine("Pulling Data from Schoology.");
            log.WriteLine(SchoologySync.GetCoursesFromSchoology());
            log.WriteLine("Done!");
            log.WriteLine("Getting Section Ids");
            log.WriteLine(SchoologySync.GetSchoologySectionIdsForTerm(DateRange.GetCurrentOrLastTerm()));
            log.WriteLine("Done!");
        }

        protected void AddPasswordsBtn_Click(object sender, EventArgs e)
        {
            CSV newUsers = new CSV(NewUsersUpload.FileContent);
            AccountManagement.GeneratePasswords(ref newUsers);
            newUsers.Save(Server.MapPath("~/Temp/newUsersPwd.csv"));
            Response.Redirect("~/Temp/newUsersPwd.csv");
        }

        protected void ByClassBtn_Click(object sender, EventArgs e)
        {
            GradeAnalytics.BySection(Convert.ToInt32(TermSelect.SelectedValue)).Save(Server.MapPath("~/Temp/GradesBySection.csv"));
            Response.Redirect("~/Temp/GradesBySection.csv");
        }
        
    }
}