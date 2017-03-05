using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class CreditEditor : LoggingUserControl
    {
        public event EventHandler OnUpdate;

        public int CreditId
        {
            get
            {
                if(Session["student_credit_report_edit_id"] == null)
                {
                    return -1;
                }
                return (int)Session["student_credit_report_edit_id"];
            }
            set
            {
                Session["student_credit_report_edit_id"] = value;
                LoadEditor();
            }
        }

        private void LoadEditor()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Credit credit = db.Credits.Where(c => c.id == CreditId).Single();

                CreditTypeDDL.DataSource = GradeTableEntryListItem.GetDataSource(credit.CreditType.GradeTable.GradeTableEntries.Select(g => g.id).ToList());
                CreditTypeDDL.DataTextField = "Text";
                CreditTypeDDL.DataValueField = "ID";
                CreditTypeDDL.DataBind();

                CreditTypeDDL.ClearSelection();
                CreditTypeDDL.SelectedValue = Convert.ToString(credit.CreditTypeId);

                CreditValueDDL.DataSource = GradeTableEntryListItem.GetDataSource(credit.CreditValue.GradeTable.GradeTableEntries.Select(g => g.id).ToList());
                CreditValueDDL.DataTextField = "Text";
                CreditValueDDL.DataValueField = "ID";
                CreditValueDDL.DataBind();

                CreditValueDDL.ClearSelection();
                CreditValueDDL.SelectedValue = Convert.ToString(credit.CreditValueId);

                NotesInput.Text = credit.Notes;

                UpdateAll.Checked = credit.Sections.Count > 1;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Credit credit = db.Credits.Find(CreditId);
                if(credit == null)
                {
                    LogError("Could not locate credit id: {0}", CreditId);
                    ErrorMessage.Text = String.Format("Could not locate credit id: {0}", CreditId);
                    ErrorPanel.Visible = true;
                    return;
                }
                if(UpdateAll.Checked && credit.Sections.Count > 0)
                {
                    foreach(Section section in credit.Sections)
                    {
                        foreach(Credit cr in section.Credits)
                        {
                            LogInformation("Updated Credit for {0} {1} in {2}", cr.Student.FirstName, cr.Student.LastName, cr.CreditType.Name);
                            cr.Notes = NotesInput.Text;
                            cr.CreditValueId = Convert.ToInt32(CreditValueDDL.SelectedValue);
                            cr.CreditTypeId = Convert.ToInt32(CreditTypeDDL.SelectedValue);
                        }
                    }
                }
                else
                {
                    LogInformation("Updated Credit for {0} {1} in {2}", credit.Student.FirstName, credit.Student.LastName, credit.CreditType.Name);
                    credit.Notes = NotesInput.Text;
                    credit.CreditValueId = Convert.ToInt32(CreditValueDDL.SelectedValue);
                    credit.CreditTypeId = Convert.ToInt32(CreditTypeDDL.SelectedValue);
                }
                db.SaveChanges();

                SuccessMessage.Text = "Successfully updated credit.";
                SuccessPanel.Visible = true;
            }
        }

        protected void DelBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Credit credit = db.Credits.Find(CreditId);
                if(credit != null)
                {
                    WebhostEventLog.Syslog.LogInformation("Deleting credit {0} for {1} {2}", credit.Notes, credit.Student.FirstName, credit.Student.LastName);
                    credit.Sections.Clear();
                    db.Credits.Remove(credit);
                    db.SaveChanges();
                    SuccessPanel.Visible = true;
                    SuccessMessage.Text = "Successfully Deleted Credit.";
                }
                else
                {
                    LogError("Could not locate credit id: {0}", CreditId);
                    ErrorMessage.Text = String.Format("Could not locate credit id: {0}", CreditId);
                    ErrorPanel.Visible = true;
                }
            }
        }

        protected void SuccessConfirm_Click(object sender, EventArgs e)
        {
            SuccessPanel.Visible = false;
            if (OnUpdate != null)
                OnUpdate(sender, e);
        }

        protected void ErrorConfirm_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = false;
            if (OnUpdate != null)
                OnUpdate(sender, e);
        }
    }
}