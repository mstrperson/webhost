using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class StudyHallCheckIn : BasePage
    {
        protected int SelectedStudyHall
        {
            get
            {
                return (Session["ssh"] == null) ? -1 : (int)Session["ssh"];
            }
            set
            {
                Session["ssh"] = value;
            }
        }

        protected DateTime SelectedDate
        {
            get
            {
                return (Session["shdt"] == null) ? DateTime.Today : (DateTime)Session["shdt"];
            }
            set
            {
                Session["shdt"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            using(WebhostEntities db = new WebhostEntities())
            {
                int ay = DateRange.GetCurrentAcademicYear();
                StudyHallLocationSelector.DataSource = (from section in db.Sections
                                                        where section.Course.AcademicYearID == ay && section.Block.Name.Equals("Study Hall")
                                                        select new
                                                        {
                                                            Text = section.Course.Name,
                                                            id = section.id
                                                        }).ToList();
                StudyHallLocationSelector.DataTextField = "Text";
                StudyHallLocationSelector.DataValueField = "id";
                StudyHallLocationSelector.DataBind();

                if(SelectedStudyHall != -1)
                {
                    Section studyHall = db.Sections.Where(sec => sec.id == SelectedStudyHall).Single();
                    foreach(Student student in studyHall.Students.OrderByDescending(s => s.AcademicLevel).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList())
                    {
                        TableRow row = new TableRow();
                        TableCell cell = new TableCell();
                        StudyHallCheckInRow shcir = (StudyHallCheckInRow)LoadControl("~/UserControls/StudyHallCheckInRow.ascx");
                        shcir.StudentId = student.ID;
                        shcir.Date = SelectedDate;
                        shcir.LoadRow();
                        cell.Controls.Add(shcir);
                        row.Cells.Add(cell);
                        CheckInTable.Rows.Add(row);
                    }
                }
            }
        }

        protected void LoadStudyHallBtn_Click(object sender, EventArgs e)
        {
            SelectedStudyHall = Convert.ToInt32(StudyHallLocationSelector.SelectedValue);
            log.WriteLine("{0}Loading Study Hall:  {1}", Log.TimeStamp(), StudyHallLocationSelector.SelectedItem.Text);
            this.Redirect("~/StudyHallCheckIn.aspx");
        }
    }
}