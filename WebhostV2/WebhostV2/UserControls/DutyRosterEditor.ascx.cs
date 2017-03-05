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
    public partial class DutyRosterEditor : LoggingUserControl
    {
        public int DutyTeamId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(DutyTeamIDField.Value);
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
                    if (db.DutyTeams.Where(t => t.id == value).Count() > 0)
                    {
                        DutyTeamIDField.Value = Convert.ToString(value);
                        DutyTeam team = db.DutyTeams.Where(t => t.id == value).Single();
                        DutyTeamLabel.Text = team.Name;

                        FacultyGroupSelector1.Clear();
                        FacultyGroupSelector1.AddFaculty(team.Members.Select(f => f.ID).ToList());

                        DTLSelector.ClearSelection();
                        DTLSelector.SelectedValue = Convert.ToString(team.DTL);

                        AODSelector.ClearSelection();
                        AODSelector.SelectedValue = Convert.ToString(team.AOD);

                        SaveBtn.Visible = true;
                    }
                    else
                    {
                        SaveBtn.Visible = false;
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    DTLSelector.DataSource = (from faculty in db.Faculties
                                              orderby faculty.LastName, faculty.FirstName
                                              select new
                                              {
                                                  Name = faculty.FirstName + " " + faculty.LastName,
                                                  id = faculty.ID
                                              }).ToList();
                    DTLSelector.DataTextField = "Name";
                    DTLSelector.DataValueField = "id";
                    DTLSelector.DataBind();


                    AODSelector.DataSource = (from faculty in db.Faculties
                                              orderby faculty.LastName, faculty.FirstName
                                              select new
                                              {
                                                  Name = faculty.FirstName + " " + faculty.LastName,
                                                  id = faculty.ID
                                              }).ToList();
                    AODSelector.DataTextField = "Name";
                    AODSelector.DataValueField = "id";
                    AODSelector.DataBind();
                }

                SaveBtn.Visible = false;
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            if (DutyTeamId == -1) return;
            using(WebhostEntities db = new WebhostEntities())
            {
                DutyTeam team = db.DutyTeams.Where(t => t.id == DutyTeamId).Single();

                State.log.WriteLine("Updating Roster for Team {0}", team.Name);

                try
                {
                    team.DTL = Convert.ToInt32(DTLSelector.SelectedValue);
                }
                catch
                {
                    State.log.WriteLine("Invalid DTL Selection.");
                }
                try
                {
                    team.AOD = Convert.ToInt32(AODSelector.SelectedValue);
                }
                catch
                {
                    State.log.WriteLine("Invalid AOD Selection.");
                }

                team.Members.Clear();
                foreach(int id in FacultyGroupSelector1.GroupIds)
                {
                    Faculty faculty = db.Faculties.Where(f => f.ID == id).Single();
                    team.Members.Add(faculty);
                }

                if(!team.Members.Contains(team.Leader))
                {
                    State.log.WriteLine("Forgot to select the DTL!");
                    team.Members.Add(team.Leader);
                }

                if(!team.Members.Contains(team.AdministratorOnDuty))
                {
                    State.log.WriteLine("Forgot to select the AOD!");
                    team.Members.Add(team.AdministratorOnDuty);
                }

                db.SaveChanges();
                State.log.WriteLine("Changes are saved to the database!");
            }
        }
    }
}