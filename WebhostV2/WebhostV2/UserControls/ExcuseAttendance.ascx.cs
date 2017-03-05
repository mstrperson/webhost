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
    public partial class ExcuseAttendance : LoggingUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    StudentNameCBX.DataSource = (from student in db.Students
                                                 where student.isActive
                                                 orderby student.LastName, student.FirstName
                                                 select new
                                                 {
                                                     Text = student.FirstName + " " + student.LastName + " (" + student.GraduationYear + ")",
                                                     id = student.ID
                                                 }).ToList();
                    StudentNameCBX.DataTextField = "Text";
                    StudentNameCBX.DataValueField = "id";
                    StudentNameCBX.DataBind();

                    // Preload Current Block.
                    AllDayCB.Checked = false;
                    MorningOnlyCB.Checked = false;
                    AfternoonOnlyCB.Checked = false;
                    EveningCB.Checked = false;

                    Dictionary<DateRange, int> BlockTimes = DateRange.BlockIdsByTime(DateTime.Today);

                    List<int> blockIds = BlockTimes.Values.ToList();

                    BlocksCBL.DataSource = (from block in db.Blocks
                                            where blockIds.Contains(block.id)
                                            select block).ToList();
                    BlocksCBL.DataTextField = "Name";
                    BlocksCBL.DataValueField = "id";
                    BlocksCBL.DataBind();

                    foreach(DateRange dr in BlockTimes.Keys)
                    {
                        if(dr.Contains(DateTime.Now))
                        {
                            BlocksCBL.ClearSelection();
                            BlocksCBL.SelectedValue = Convert.ToString(BlockTimes[dr]);
                            break;
                        }
                    }
                }
            }
        }

        protected void ShowError(String message)
        {
            ErrorLabel.Text = message;
            LogWarning("Refused Excused Absence.  Reason: {0}", message);
            ErrorPanel.Visible = true;
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            LogInformation("Attempting To Submit an excused absence.");
            int studentId;
            try
            {
                studentId = Convert.ToInt32(StudentNameCBX.SelectedValue);
            }
            catch
            {
                ShowError("You must select a student to excuse.");
                return;
            }

            DateTime date = DateTime.Today;
            if(!TodayCB.Checked)
            {
                try
                {
                    date = DateRange.GetDateTimeFromString(DateInput.Text);
                }
                catch
                {
                    ShowError("Please select a date using the calendar.");
                    return;
                }
            }

            if(NotesInput.Text.Equals(""))
            {
                ShowError("You must enter a reason for the excused absence.");
                return;
            }

            List<int> selectedBlocks = new List<int>();
            foreach(ListItem item in BlocksCBL.Items)
            {
                if (item.Selected)
                    selectedBlocks.Add(Convert.ToInt32(item.Value));
            }

            using(WebhostEntities db = new WebhostEntities())
            {
                List<int> sectionIds = new List<int>();
                Term theTerm = null;

                foreach(Term term in db.Terms)
                {
                    DateRange termrange = new DateRange(term.StartDate, term.EndDate);
                    if (termrange.Contains(date))
                    {
                        theTerm = term;
                        break;
                    }
                }

                if(theTerm == null)
                {
                    ShowError("The selected Date is not in any Term.");
                    return;
                }

                foreach(int blkid in selectedBlocks)
                {
                    Block block = db.Blocks.Where(b => b.id == blkid).Single();
                    foreach(Section section in block.Sections.Where(sec => sec.Terms.Contains(theTerm)).ToList())
                    {
                        if (section.Students.Where(stu => stu.ID == studentId).Count() > 0)
                            sectionIds.Add(section.id);
                    }
                }

                if(MultiDayCB.Checked)
                {
                    DateTime end = new DateTime();
                    try
                    {
                        end = DateRange.GetDateTimeFromString(EndDateInput.Text);
                    }
                    catch
                    {
                        ShowError("Please select the End Date from the calendar.");
                        return;
                    }

                    String report = AttendanceControl.ExcuseStudent(studentId, sectionIds, NotesInput.Text, ((BasePage)Page).user, new DateRange(date, end));
                    State.log.WriteLine(report);
                }
                else
                {
                    String report = AttendanceControl.ExcuseStudent(studentId, sectionIds, NotesInput.Text, ((BasePage)Page).user, date);
                    State.log.WriteLine(report);
                }

                LogInformation("Excused attendance submitted for student id {0}.  See Webhost MySQL Connection log for details.", studentId);
            }
        }

        protected void AllDayCB_CheckedChanged(object sender, EventArgs e)
        {
            if(AllDayCB.Checked)
            {
                MorningOnlyCB.Checked = true;
                AfternoonOnlyCB.Checked = true;
                EveningCB.Checked = true;

                MorningOnlyCB.Visible = false;
                AfternoonOnlyCB.Visible = false;
                EveningCB.Visible = false;

                foreach (ListItem item in BlocksCBL.Items)
                    item.Selected = true;
            }
            else
            {
                MorningOnlyCB.Visible = true;
                AfternoonOnlyCB.Visible = true;

                EveningCB.Visible = true;
            }
        }

        protected void MorningOnlyCB_CheckedChanged(object sender, EventArgs e)
        {
            if(MorningOnlyCB.Checked)
            {
                DateTime date = TodayCB.Checked ? DateTime.Today : DateRange.GetDateTimeFromString(DateInput.Text);
                AfternoonOnlyCB.Checked = false;
                BlocksCBL.ClearSelection();
                Dictionary<DateRange, int> BlockTimes = DateRange.BlockIdsByTime(date);

                List<String> vals = new List<string>();

                foreach(DateRange dr in BlockTimes.Keys)
                {
                    if(dr.Intersects(DateRange.FirstBlock(date)) || dr.Intersects(DateRange.SecondBlock(date)) || dr.Intersects(DateRange.ThirdBlock(date)))
                    {
                        vals.Add(BlockTimes[dr].ToString());
                    }
                }

               /* vals.Add(Convert.ToString(BlockTimes[DateRange.FirstBlock(date)]));
                vals.Add(Convert.ToString(BlockTimes[DateRange.SecondBlock(date)]));
                vals.Add(Convert.ToString(BlockTimes[DateRange.ThirdBlock(date)]));
                */
                foreach(ListItem item in BlocksCBL.Items)
                {
                    if (vals.Contains(item.Value)) item.Selected = true;
                }
            }
        }

        protected void AfternoonOnlyCB_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (AfternoonOnlyCB.Checked)
                {
                    DateTime date = TodayCB.Checked ? DateTime.Today : DateRange.GetDateTimeFromString(DateInput.Text);
                    MorningOnlyCB.Checked = false;
                    BlocksCBL.ClearSelection();
                    Dictionary<DateRange, int> BlockTimes = DateRange.BlockIdsByTime(date);

                    List<String> vals = new List<string>(); 
                    foreach (DateRange dr in BlockTimes.Keys)
                    {
                        if (dr.Intersects(DateRange.FourthBlock(date)) || dr.Intersects(DateRange.FifthBlock(date)) || dr.Intersects(DateRange.SixthBlock(date)))
                        {
                            vals.Add(BlockTimes[dr].ToString());
                        }
                    }
                    /*vals.Add(Convert.ToString(BlockTimes[DateRange.FourthBlock(date)]));
                    vals.Add(Convert.ToString(BlockTimes[DateRange.FifthBlock(date)]));
                    vals.Add(Convert.ToString(BlockTimes[DateRange.SixthBlock(date)]));
                    */
                    foreach (ListItem item in BlocksCBL.Items)
                    {
                        if (vals.Contains(item.Value)) item.Selected = true;
                    }
                }
            }
            catch(Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void TodayCB_CheckedChanged(object sender, EventArgs e)
        {
            DateInput.Visible = !TodayCB.Checked;
            MultiDayCB.Visible = !TodayCB.Checked;
            SelectDateBtn.Visible = !TodayCB.Checked;
            MultiDayCB.Checked = false;
            EndDateInput.Visible = false;
            if(!TodayCB.Checked)
            {
                DateInput.Text = DateTime.Today.ToShortDateString();
            }
        }

        protected void MultiDayCB_CheckedChanged(object sender, EventArgs e)
        {
            EndDateInput.Visible = MultiDayCB.Checked;
            if(MultiDayCB.Checked)
            {
                EndDateInput.Text = DateRange.GetDateTimeFromString(DateInput.Text).AddDays(1).ToShortDateString();
                AllDayCB.Checked = true;
                AllDayCB_CheckedChanged(sender, e);
            }
        }

        protected void DismissBtn_Click(object sender, EventArgs e)
        {
            ErrorPanel.Visible = false;
        }

        protected void EveningCB_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void SelectDateBtn_Click(object sender, EventArgs e)
        {
            DateTime date = DateRange.GetDateTimeFromString(DateInput.Text);
            Dictionary<DateRange, int> BlockTimes = DateRange.BlockIdsByTime(date);
            using (WebhostEntities db = new WebhostEntities())
            {
                List<int> blockIds = BlockTimes.Values.ToList();

                BlocksCBL.DataSource = (from block in db.Blocks
                                        where blockIds.Contains(block.id)
                                        select block).ToList();
                BlocksCBL.DataTextField = "Name";
                BlocksCBL.DataValueField = "id";
                BlocksCBL.DataBind();
            }
        }
    }
}