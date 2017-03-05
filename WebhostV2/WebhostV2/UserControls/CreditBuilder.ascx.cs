using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class CreditBuilder : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                int year = DateRange.GetCurrentAcademicYear();
                using(WebhostEntities db = new WebhostEntities())
                {
                    StudentSelect.DataSource = StudentListItem.GetDataSource(db.Students.Where(s => s.isActive).OrderBy(s => s.GraduationYear).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).Select(s => s.ID).ToList());
                    StudentSelect.DataTextField = "Text";
                    StudentSelect.DataValueField = "ID";
                    StudentSelect.DataBind();

                    CreditTypeDDL.DataSource = GradeTableEntryListItem.GetDataSource(db.GradeTables.Where(t => t.Name.Equals("Credit Types") && t.AcademicYearID == year).Single().GradeTableEntries.Select(d => d.id).ToList());
                    CreditTypeDDL.DataTextField = "Text";
                    CreditTypeDDL.DataValueField = "ID";
                    CreditTypeDDL.DataBind();

                    CreditValueDDL.DataSource = GradeTableEntryListItem.GetDataSource(db.GradeTables.Where(t => t.Name.Equals("Credit Values") && t.AcademicYearID == year).Single().GradeTableEntries.Select(d => d.id).ToList());
                    CreditValueDDL.DataTextField = "Text";
                    CreditValueDDL.DataValueField = "ID";
                    CreditValueDDL.DataBind();
                }
            }
        }

        protected void OKBtn_Click(object sender, EventArgs e)
        {
            bPage.log.WriteLine("Added {3} {0} credit for {1} in {2}", 
                TransferWaiverSelect.SelectedValue, StudentSelect.SelectedItem.Text, CreditTypeDDL.SelectedItem.Text, CreditValueDDL.SelectedItem.Text);
            SuccessPanel.Visible = false;
        }

        protected void DismissBtn_Click(object sender, EventArgs e)
        {
            ((BasePage)Page).log.WriteLine(ErrorLabel.Text);
            ErrorPanel.Visible = false;
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                int id = db.Credits.Count() > 0 ? db.Credits.OrderBy(c => c.id).ToList().Last().id + 1 : 0;
                Credit credit = new Credit()
                {
                    id = id,
                    StudentId = Convert.ToInt32(StudentSelect.SelectedValue),
                    CreditTypeId = Convert.ToInt32(CreditTypeDDL.SelectedValue),
                    CreditValueId = Convert.ToInt32(CreditValueDDL.SelectedValue),
                    Notes = String.Format("[{0}] {1}", TransferWaiverSelect.SelectedValue, NotesInput.Text)
                };

                db.Credits.Add(credit);
                try
                {
                    db.SaveChanges();
                    LogInformation("Added {3} {0} credit for {1} in {2}",
                        TransferWaiverSelect.SelectedValue, StudentSelect.SelectedItem.Text, CreditTypeDDL.SelectedItem.Text, CreditValueDDL.SelectedItem.Text);
                    SuccessPanel.Visible = true;
                }
                catch(Exception ex)
                {
                    String message = ex.Message;
                    while(ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        message += String.Format(" -- {0}{1}", Environment.NewLine, ex.Message);
                    }
                    LogError("Faield to add 3} {0} credit for {1} in {2}{4}{4}{5}",
                        TransferWaiverSelect.SelectedValue, StudentSelect.SelectedItem.Text, CreditTypeDDL.SelectedItem.Text, CreditValueDDL.SelectedItem.Text,
                        Environment.NewLine, message);
                    ErrorLabel.Text = message;
                    ErrorPanel.Visible = true;
                }
            }
        }
    }
}