﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class CreditViewer : BasePage
    {
        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            using (WebhostEntities db = new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Find(user.ID);
                if (faculty == null)
                {
                    LogError("Could not locate faculty id {0}", user.ID);
                    Response.Redirect("~/Home.aspx");
                }

                StudentComboBox.DataSource = StudentListItem.GetDataSource(faculty.Students.Where(s => s.isActive).Select(s => s.ID).ToList());

                StudentComboBox.DataTextField = "Text";
                StudentComboBox.DataValueField = "ID";
                StudentComboBox.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SelectBtn_Click(object sender, EventArgs e)
        {
            StudentCreditReport1.SelectedStudentId = Convert.ToInt32(StudentComboBox.SelectedValue);
        }
    }
}