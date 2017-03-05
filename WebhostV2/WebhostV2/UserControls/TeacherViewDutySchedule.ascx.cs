using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.EVOPublishing;

namespace WebhostV2.UserControls
{
    public partial class TeacherViewDutySchedule : LoggingUserControl
    {
        public int WeekendId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(WeekendIdField.Value);
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
                    if(db.Weekends.Where(w => w.id == value).Count() > 0)
                    {
                        bool forStudents = ((BasePage)Page).user.IsStudent;

                        DownloadSchedule.Visible = !forStudents;
                        WeekendIdField.Value = Convert.ToString(value);

                        Weekend weekend = db.Weekends.Where(w => w.id == value).Single();

                        WeekendLabel.Text = String.Format("{0} Weekend -- {1} ~ {2}", weekend.DutyTeam.Name, weekend.StartDate.ToShortDateString(), weekend.EndDate.ToShortDateString());

                        int year = DateRange.GetCurrentAcademicYear();

                        #region DutyTeamTable

                        TableHeaderRow headerRow = new TableHeaderRow();
                        List<String> fields = new List<string>() { "Dorm", "Name", "Phone Number" };

                        foreach(string field in fields)
                        {
                            TableHeaderCell hc = new TableHeaderCell()
                            {
                                Text = field
                            };
                            headerRow.Cells.Add(hc);
                        }

                        DutyTeamTable.Rows.Add(headerRow);

                        foreach(Faculty member in weekend.DutyTeam.Members)
                        {
                            TableRow row = new TableRow();
                            TableCell dormCell = new TableCell()
                            {
                                Text = member.DormsParented.Where(dorm => dorm.AcademicYearId == year).Count() > 0 ? member.DormsParented.Where(dorm => dorm.AcademicYearId == year).FirstOrDefault().Name : ""
                            };
                            row.Cells.Add(dormCell);

                            TableCell nameCell = new TableCell()
                            {
                                Text = String.Format("{0} {1}", member.FirstName, member.LastName)
                            };
                            row.Cells.Add(nameCell);
                            if (!forStudents)
                            {
                                TableCell phoneCell = new TableCell()
                                {
                                    Text = member.PhoneNumber
                                };
                                row.Cells.Add(phoneCell);
                            }
                            else
                                row.Cells.Add(new TableCell());

                            DutyTeamTable.Rows.Add(row);
                        }

                        #endregion

                        #region Activities Tables

                        foreach (WeekendActivity activity in weekend.WeekendActivities.Where(act => act.showStudents || !forStudents).OrderBy(a => a.DateAndTime))
                        {
                            TableRow row = new TableRow();

                            TableCell dayCell = new TableCell()
                            {
                                Text = activity.DateAndTime.DayOfWeek.Equals(DayOfWeek.Friday) ? "Friday" :
                                       activity.DateAndTime.DayOfWeek.Equals(DayOfWeek.Saturday) ? "Saturday" : "Sunday",
                                ToolTip = activity.DateAndTime.ToLongDateString()
                            };
                            row.Cells.Add(dayCell);
                            TableCell timeCell = new TableCell()
                            {
                                Text = activity.DateAndTime.ToShortTimeString() +
                                       (activity.Duration == 0 ? "" : activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString())
                            };
                            row.Cells.Add(timeCell);
                            TableCell actCell = new TableCell()
                            {
                                Text = activity.Name,
                                ToolTip = activity.Description
                            };
                            row.Cells.Add(actCell);

                            switch(activity.DateAndTime.DayOfWeek)
                            {
                                case DayOfWeek.Friday: FridayTable.Rows.Add(row); break;
                                case DayOfWeek.Saturday: SaturdayTable.Rows.Add(row); break;
                                case DayOfWeek.Sunday: SundayTable.Rows.Add(row); break;
                                default: FridayTable.Rows.Add(row); break;
                            }
                        }

                        #endregion

                    }
                    else
                    {
                        DownloadSchedule.Visible = false;
                        WeekendLabel.Text = "This Weekend has not been prepared yet.";
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DownloadSchedule_Click(object sender, EventArgs e)
        {
            if (WeekendId == -1) return;
            //WeekendDutySchedule schedule = new WeekendDutySchedule(WeekendId);
            //Response.Redirect(schedule.Publish());
        }
    }
}