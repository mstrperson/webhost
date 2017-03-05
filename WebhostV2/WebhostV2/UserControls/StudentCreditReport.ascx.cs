using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class StudentCreditReport : LoggingUserControl
    {
        public int SelectedStudentId
        {
            get 
            { 
                if (Session["student_credit_report_student_id"] == null) return -1;
                return (int)Session["student_credit_report_student_id"];
            }
            set
            {
                Session["student_credit_report_student_id"] = value;
                Session["student_credit_report_table"] = CreditAudit.GetCreditReportWebTable(value);
                LoadTable();
                using(WebhostEntities db = new WebhostEntities())
                {
                    Student student = db.Students.Where(s => s.ID == value).Single();
                    StudentNameLabel.Text = String.Format("{0} {1} ({2})", student.FirstName, student.LastName, student.GraduationYear);

                    CreditSelectDDL.DataSource = CreditListItem.GetDataSource(student.Credits.Select(c => c.id).ToList());
                    CreditSelectDDL.DataTextField = "Text";
                    CreditSelectDDL.DataValueField = "ID";
                    CreditSelectDDL.DataBind();
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                return !EditPanel.Visible;
            }
            set
            {
                EditPanel.Visible = !value;
            }
        }

        protected void LoadTable()
        {
            if (Session["student_credit_report_table"] != null)
            {
                CreditTable.Rows.Clear();
                foreach (TableRow tr in (List<TableRow>)Session["student_credit_report_table"])
                {
                    CreditTable.Rows.Add(tr);
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CreditEditor1.OnUpdate += CreditEditor1_OnUpdate;
        }

        private void CreditEditor1_OnUpdate(object sender, EventArgs e)
        {
            Session["student_credit_report_table"] = CreditAudit.GetCreditReportWebTable(SelectedStudentId);
            LoadTable();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadTable();
        }

        protected void EditCreditBtn_Click(object sender, EventArgs e)
        {
            CreditEditor1.CreditId = Convert.ToInt32(CreditSelectDDL.SelectedValue);
        }
    }
}