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
    public partial class CommentGradeBar : LoggingUserControl
    {
        public int AFDefault
        {
            get
            {
                return Session["def_af"] == null ? -1 : (int)Session["def_af"];
            }
            protected set
            {
                Session["def_af"] = value;
            }
        }

        public int EffortDefault
        {
            get
            {
                return Session["def_ef"] == null ? -1 : (int)Session["def_ef"];
            }
            protected set
            {
                Session["def_ef"] = value;
            }
        }

        public int FinalGradeID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(FinalDDL.SelectedValue);
                }
                catch
                {
                    State.log.Write("Failed to convert FinalGrade DDL selected value.");
                    return -1;
                }
            }
            set
            {
                try
                {
                    FinalDDL.ClearSelection();
                    FinalDDL.SelectedValue = Convert.ToString(value);
                }
                catch
                {
                    State.log.Write("Could not select FinalGrade with GradeTableEntry.id={0}", value);
                }
            }
        }

        public int TrimesterGradeID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(TrimesterDDL.SelectedValue);
                }
                catch
                {
                    State.log.Write("Failed to convert TrimesterGrade DDL selected value.");
                    return -1;
                }
            }
            set
            {
                try
                {
                    TrimesterDDL.ClearSelection();
                    TrimesterDDL.SelectedValue = Convert.ToString(value);
                }
                catch
                {
                    State.log.Write("Could not select TrimesterGrade with GradeTableEntry.id={0}", value);
                }
            }
        }
        public int EffortGradeID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(EffortDDL.SelectedValue);
                }
                catch
                {
                    State.log.Write("Failed to convert EffortGrade DDL selected value.");
                    return -1;
                }
            }
            set
            {
                try
                {
                    EffortDDL.ClearSelection();
                    EffortDDL.SelectedValue = Convert.ToString(value);
                }
                catch
                {
                    State.log.Write("Could not select EffortGrade with GradeTableEntry.id={0}", value);
                }
            }
        }

        public int ExamGradeID
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ExamDDL.SelectedValue);
                }
                catch
                {
                    State.log.Write("Failed to convert ExamGrade DDL selected value.");
                    return -1;
                }
            }
            set
            {
                try
                {
                    ExamDDL.ClearSelection();
                    ExamDDL.SelectedValue = Convert.ToString(value);
                }
                catch
                {
                    State.log.Write("Could not select ExamGrade with GradeTableEntry.id={0}", value);
                }
            }
        }

        public void ResetBar()
        {
            FinalDDL.ClearSelection();
            FinalDDL.SelectedValue = Convert.ToString(AFDefault);
            ExamDDL.ClearSelection();
            ExamDDL.SelectedValue = Convert.ToString(AFDefault);
            TrimesterDDL.ClearSelection();
            TrimesterDDL.SelectedValue = Convert.ToString(AFDefault);
            EffortDDL.ClearSelection();
            EffortDDL.SelectedValue = Convert.ToString(EffortDefault);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int year = DateRange.GetCurrentAcademicYear();
                using (WebhostEntities db = new WebhostEntities())
                {
                    GradeTable stdaf = db.GradeTables.Where(gt => gt.AcademicYearID == year && gt.Name.Equals("Standard A-F Scale")).Single();

                    AFDefault = stdaf.GradeTableEntries.Where(gte => gte.Value > 500).Single().id;

                    FinalDDL.DataSource = stdaf.GradeTableEntries.ToList();
                    FinalDDL.DataTextField = "Name";
                    FinalDDL.DataValueField = "id";
                    FinalDDL.DataBind();

                    FinalDDL.ClearSelection();
                    FinalDDL.SelectedValue = Convert.ToString(AFDefault);

                    ExamDDL.DataSource = stdaf.GradeTableEntries.ToList();
                    ExamDDL.DataTextField = "Name";
                    ExamDDL.DataValueField = "id";
                    ExamDDL.DataBind();
                    ExamDDL.ClearSelection();
                    ExamDDL.SelectedValue = Convert.ToString(AFDefault);

                    TrimesterDDL.DataSource = stdaf.GradeTableEntries.ToList();
                    TrimesterDDL.DataTextField = "Name";
                    TrimesterDDL.DataValueField = "id";
                    TrimesterDDL.DataBind();
                    TrimesterDDL.ClearSelection();
                    TrimesterDDL.SelectedValue = Convert.ToString(AFDefault);

                    GradeTable effort = db.GradeTables.Where(gt => gt.AcademicYearID == year && gt.Name.Equals("Effort Grades")).Single();

                    EffortDefault = effort.GradeTableEntries.FirstOrDefault().id;

                    EffortDDL.DataSource = effort.GradeTableEntries.ToList();
                    EffortDDL.DataTextField = "Name";
                    EffortDDL.DataValueField = "id";
                    EffortDDL.DataBind();
                    EffortDDL.ClearSelection();
                    EffortDDL.SelectedValue = Convert.ToString(EffortDefault);
                }
            }
        }
    }
}