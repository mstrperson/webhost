using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using System.IO;

namespace WebhostV2.UserControls
{
    public partial class CourseRequestAdmin : LoggingUserControl
    {
        public event EventHandler Masquerade;

        public int MasqeradeId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(FacultyCmbBx.SelectedValue);
                }
                catch
                {
                    return -1;
                }
            }
        }

        protected int TermId = DateRange.GetNextTerm();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    FacultyCmbBx.DataSource = (from faculty in db.Faculties
                                               orderby faculty.LastName, faculty.FirstName
                                               select new
                                               {
                                                   Name = faculty.FirstName + " " + faculty.LastName,
                                                   ID = faculty.ID
                                               }).ToList();
                    FacultyCmbBx.DataTextField = "Name";
                    FacultyCmbBx.DataValueField = "ID";
                    FacultyCmbBx.DataBind();
                }
            }
        }

        protected void MasqBtn_Click(object sender, EventArgs e)
        {
            if (Masquerade != null)
            {
                Masquerade(sender, e);
            }
        }

        protected void BlackbaudExport_Click(object sender, EventArgs e)
        {
            CourseRequestExport.ForBlackbaud(TermId).Save(Server.MapPath("~/Temp/CourseRequestsBB.csv"));
            Response.Redirect("~/Temp/CourseRequestsBB.csv");
        }

        protected void ExcelExport_Click(object sender, EventArgs e)
        {
            CourseRequestExport.ForExcel(TermId).Save(Server.MapPath("~/Temp/CourseRequests.csv"));
            Response.Redirect("~/Temp/CourseRequests.csv");
        }

        protected void NotesExport_Click(object sender, EventArgs e)
        {
            if (File.Exists(Server.MapPath("~/Temp/CourseRequestNotes.txt"))) File.Delete(Server.MapPath("~/Temp/CourseRequestNotes.txt"));
            StreamWriter outfile = new StreamWriter(new FileStream(Server.MapPath("~/Temp/CourseRequestNotes.txt"), FileMode.OpenOrCreate));

            outfile.WriteLine(CourseRequestExport.GetCourseRequestNotes(TermId));
            outfile.Flush();
            outfile.Close();
            Response.Redirect("~/Temp/CourseRequestNotes.txt");
        }

        protected void GetOverviewBtn_Click(object sender, EventArgs e)
        {

            CourseRequestExport.CourseRequestsCompleted(TermId).Save(Server.MapPath("~/Temp/CourseRequestsStatus.csv"));
            Response.Redirect("~/Temp/CourseRequestsStatus.csv");
        }

        protected void ClassCountBtn_Click(object sender, EventArgs e)
        {
            CourseRequestExport.ForExcelByClass(TermId).Save(Server.MapPath("~/Temp/CourseRequestCounts.csv"));
            Response.Redirect("~/Temp/CourseRequestCounts.csv");
        }

        protected void ExportPack_Click(object sender, EventArgs e)
        {
            String fileName = CourseRequestExport.CourseRequestsByStudent(TermId, Server.MapPath("~/Temp/"));
            Response.Redirect(fileName);
        }
    }
}