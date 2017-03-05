using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.EVOPublishing;

namespace WebhostV2
{
    public partial class DTL : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    int year = DateRange.GetCurrentAcademicYear();
                    DutyTeamSelectList.DataSource = (from team in db.DutyTeams.Where(t => t.AcademicYearID == year)
                                                    select new
                                                    {
                                                        Name = team.Name,
                                                        ID = team.id
                                                    }).ToList();
                    DutyTeamSelectList.DataTextField = "Name";
                    DutyTeamSelectList.DataValueField = "ID";
                    DutyTeamSelectList.DataBind();

                    if(db.Faculties.Where(f => f.ID == user.ID).Count() > 0)
                    {
                        Faculty faculty = db.Faculties.Where(f => f.ID == user.ID).Single();
                        if(faculty.DutyTeams.Where(dt => dt.AcademicYearID == year).Count() > 0)
                        {
                            DutyTeamSelectList.ClearSelection();
                            DutyTeamSelectList.SelectedValue = Convert.ToString(faculty.DutyTeams.Where(dt => dt.AcademicYearID == year).ToList().First().id);
                            DutyRosterEditor1.DutyTeamId = Convert.ToInt32(DutyTeamSelectList.SelectedValue);
                            WeekendBuilder1.DutyTeamID = Convert.ToInt32(DutyTeamSelectList.SelectedValue);
                        }
                    }
                }
            }
        }

        protected void LoadDutyTeam_Click(object sender, EventArgs e)
        {
            DutyRosterEditor1.DutyTeamId = Convert.ToInt32(DutyTeamSelectList.SelectedValue);
            WeekendBuilder1.DutyTeamID = Convert.ToInt32(DutyTeamSelectList.SelectedValue);
        }

        protected void OpenJob_Click(object sender, EventArgs e)
        {
            DutyRosterEditor1.Visible = JobSelectList.SelectedIndex == 0;
            WeekendBuilder1.Visible = JobSelectList.SelectedIndex == 1;
            WeekendDiscipline1.Visible = JobSelectList.SelectedIndex == 2;
        }

        protected void DownloadSchedule_Click(object sender, EventArgs e)
        {
            State.log.WriteLine("Publishing Weekend Schedule PDF.");
            if (WeekendBuilder1.WeekendID != -1)
            {
                //WeekendDutySchedule schedule = new WeekendDutySchedule(WeekendBuilder1.WeekendID);
                //Response.Redirect(schedule.Publish());
            }
            else
            {
                State.log.WriteLine("No Schedule is Loaded, cannot publish!");
            }
        }
    }
}