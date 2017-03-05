using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2
{
    public partial class Rosters : BasePage
    {
        protected int TermId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(TermIdField.Value);
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
                    int term = value;
                    TermIdField.Value = Convert.ToString(value);

                    SectionSelectList.DataSource = (from section in db.Sections
                                                    where (section.Terms.Count == 0 || section.Terms.Where(t => t.id == term).Count() > 0)
                                                    orderby section.Course.Name, section.Block.Name
                                                    select new
                                                    {
                                                        Name = section.Course.Name + " [" + section.Block.LongName + "]",
                                                        ID = section.id
                                                    }).ToList();
                    SectionSelectList.DataTextField = "Name";
                    SectionSelectList.DataValueField = "ID";
                    SectionSelectList.DataBind();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                if (TermId == -1) TermId = DateRange.GetCurrentOrLastTerm();
                using(WebhostEntities db = new WebhostEntities())
                {
                    int year = DateRange.GetCurrentAcademicYear();
                    int termid = Import.GetCurrentOrLastTerm();
                    Term term = db.Terms.Where(t => t.id == termid).Single();
                    TermSelectDDL.ClearSelection();
                    TermSelectDDL.SelectedValue = term.Name;
                    
                }
            }
        }

        protected void LoadSection_Click(object sender, EventArgs e)
        {
            RosterEditor1.SectionID = Convert.ToInt32(SectionSelectList.SelectedValue);
        }

        protected void TermSelectDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year = DateRange.GetCurrentAcademicYear();
            TermId = Import.GetTermByName(TermSelectDDL.SelectedValue, year);
        }

        protected void CleanBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                foreach(Section section in db.Sections.ToList())
                {
                    bool delete = false;
                    if(section.Teachers.Count <= 0 && section.Block.ShowInSchedule)
                    {
                        LogWarning("Section [{0}-{1}] {2} has no teacher assigned.", section.Course.BlackBaudID, section.SectionNumber, section.Course.Name);
                        log.WriteLine("Section [{0}-{1}] {2} has no teacher assigned.", section.Course.BlackBaudID, section.SectionNumber, section.Course.Name);
                        delete = true;
                    }
                    if (section.Students.Count <= 0)
                    {
                        LogWarning("Section [{0}-{1}] {2} has no students assigned.", section.Course.BlackBaudID, section.SectionNumber, section.Course.Name);
                        log.WriteLine("Section [{0}-{1}] {2} has no students assigned.", section.Course.BlackBaudID, section.SectionNumber, section.Course.Name);
                        delete = true;
                    }
                    if (section.Terms.Count <= 0 && section.Block.ShowInSchedule)
                    {
                        LogWarning("Section [{0}-{1}] {2} has no terms assigned.", section.Course.BlackBaudID, section.SectionNumber, section.Course.Name);
                        log.WriteLine("Section [{0}-{1}] {2} has no terms assigned.", section.Course.BlackBaudID, section.SectionNumber, section.Course.Name);
                        delete = true;
                    }

                    if(delete)
                    {
                        LogWarning("Attempting to delete section [{0}-{1}].", section.Course.BlackBaudID, section.SectionNumber);
                        log.WriteLine("Attempting to delete section [{0}-{1}].", section.Course.BlackBaudID, section.SectionNumber);
                        section.Students.Clear();
                        section.Teachers.Clear();
                        section.Terms.Clear();
                        section.Credits.Clear();
                        section.SeatingCharts.Clear();
                        db.Sections.Remove(section);
                        db.SaveChanges();
                    }
                    else
                    {
                        LogInformation("[{0}-Block] {1}, {2}, [{3}-{4}] should be fine.",
                            section.Block.Name, section.Course.Name, section.Terms.Count > 0 ? section.Terms.First().Name : "all terms", section.Course.BlackBaudID, section.SectionNumber);
                        log.WriteLine("[{0}-Block] {1}, {2}, [{3}-{4}] should be fine.",
                            section.Block.Name, section.Course.Name, section.Terms.Count > 0? section.Terms.First().Name:"all terms", section.Course.BlackBaudID, section.SectionNumber);
                    }
                }

            }
        }
    }
}