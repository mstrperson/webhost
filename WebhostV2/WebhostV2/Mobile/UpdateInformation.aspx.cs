using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2.Mobile
{
    public partial class UpdateInformation : BasePage
    {
        protected int SectionId
        {
            get
            {
                return Session["att_secid"] == null ? -1 : (int)Session["att_secid"];
            }
            set
            {
                Session["att_secid"] = value;
            }
        }

        protected DateTime AttendanceDate
        {
            get
            {
                return Session["att_date"] == null ? DateTime.Today : (DateTime)Session["att_date"];
            }
            set
            {
                Session["att_date"] = value;
            }
        }

        protected void LoadInfo()
        {
            if (Session["required_update_info"] == null)
            {
                LogError("UpdateInformation page accessed without any required data input.");
                if (this.IsRedirect)
                    Redirect(this.RedirectedUrl);
                else
                    Redirect("~/Mobile/Attendance.aspx");
                return;
            }

            using (WebhostEntities db = new WebhostEntities())
            {
                Dictionary<int, AttendanceControl.AdditionalInfoRequest> request = (Dictionary<int, AttendanceControl.AdditionalInfoRequest>)Session["required_update_info"];
                Section section = db.Sections.Find(SectionId);

                ClassInfoLabel.Text = String.Format("[{0}] {1}, {2}", section.Block.LongName, section.Course.Name, AttendanceDate.ToLongDateString());

                foreach (int studentId in request.Keys)
                {
                    TableRow row = new TableRow();
                    row.Cells.Add(new TableCell()
                        {
                            Text = request[studentId].reason,
                            Width = Unit.Percentage(50)
                        });

                    TableCell noteCell = new TableCell()
                    {
                        Text = String.Format("Marked: {0}",
                        db.GradeTableEntries.Find(request[studentId].markingId).Name)
                    };

                    TextBox noteInput = new TextBox() { TextMode = TextBoxMode.MultiLine, Height = Unit.Pixel(150) };
                    HiddenField idfield = new HiddenField() { Value = Convert.ToString(studentId) };

                    noteCell.Controls.Add(idfield);
                    noteCell.Controls.Add(noteInput);

                    row.Cells.Add(noteCell);

                    UpdateTable.Rows.Add(row);
                }

                LogInformation("Loaded Attendance Update Information Page");
            }
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (!user.IsTeacher)
            {   
                LogError("Invalid access attempt by student {0}", user.Name);
                return;
            }
            LoadInfo();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LogCurrentData(AttendancePageInfo api)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(AttendancePageInfo));
            MemoryStream mstr = new MemoryStream();
            json.WriteObject(mstr, api);
            mstr.Position = 0;
            StreamReader sr = new StreamReader(mstr);
            String info = sr.ReadToEnd();
            sr.Close();
            mstr.Close();
            sr.Dispose();
            mstr.Dispose();
            LogInformation(info);
        }

        protected void SubmitNoteBtn_Click(object sender, EventArgs e)
        {
            Dictionary<int, AttendanceControl.ShortMarking> MarkingData = new Dictionary<int, AttendanceControl.ShortMarking>();
            LogInformation("Submitting Requested Notes.");

            Dictionary<int, AttendanceControl.AdditionalInfoRequest>request = (Dictionary<int, AttendanceControl.AdditionalInfoRequest>)Session["required_update_info"];

            AttendancePageInfo api = new AttendancePageInfo() { SectionId = this.SectionId, Name = ClassInfoLabel.Text, Date = AttendanceDate, Attendances = new List<AttendanceData>() };
            using (WebhostEntities db = new WebhostEntities())
            {
                foreach (TableRow row in UpdateTable.Rows)
                {
                    int studentId = Convert.ToInt32(((HiddenField)row.Cells[1].Controls[0]).Value);
                    Student student = db.Students.Find(studentId);
                    String Notes = ((TextBox)row.Cells[1].Controls[1]).Text;
                    int markId = request[studentId].markingId;
                    MarkingData.Add(studentId, new AttendanceControl.ShortMarking() { markId = markId, notes = Notes });
                    AttendanceData atd = new AttendanceData()
                    {
                        StudentId = studentId,
                        Name = String.Format("{0} {1}", student.FirstName, student.LastName),
                        Marking = db.GradeTableEntries.Find(markId).Name
                    };
                    ((List<AttendanceData>)api.Attendances).Add(atd);
                }
            }
            LogCurrentData(api);

            request.Clear();
            request = AttendanceControl.SubmitAttendance(SectionId, MarkingData, user.ID, AttendanceDate);
            if (request.Count != 0)
            {
                Session["required_update_info"] = request;
                Response.Redirect("~/Mobile/UpdateInformation.aspx");
            }
            else
            {
                Session["required_update_info"] = null;
                SectionId = -1;
                AttendanceDate = DateTime.Today;
                if (this.IsRedirect)
                    Redirect(this.RedirectedUrl);
                else
                    Redirect("~/Mobile/Attendance.aspx");
            }
        }

        protected void CancelNoteBtn_Click(object sender, EventArgs e)
        {
            LogError(String.Format("User has actively aborted entering requested data for {0}", ClassInfoLabel.Text),
                typeof(Dictionary<int, AttendanceControl.AdditionalInfoRequest>),
                (Dictionary<int, AttendanceControl.AdditionalInfoRequest>)Session["required_update_info"]);
            MailControler.MailToUser("User has actively aborted entering requested data.",
                String.Format("{0} actively ignored the request for additional information on attendance markings for {1}.  These markings changes have been rejected.",
                user.Name, ClassInfoLabel.Text), MailControler.DeanOfStudents.Email, MailControler.DeanOfStudents.Name, user);
            MailControler.MailToUser("Aborted entering requested data.",
                String.Format("You actively ignored the request for additional information on attendance markings for {1}.  These markings changes have been rejected.",
                user.Name, ClassInfoLabel.Text), user);
            Session["required_update_info"] = null;
            Redirect("~/Mobile/Attendance.aspx");
        }
    }
}