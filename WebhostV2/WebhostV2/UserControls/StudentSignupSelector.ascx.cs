using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public partial class StudentSignupSelector : LoggingUserControl
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
                        Weekend weekend = db.Weekends.Where(w => w.id == value).Single();

                        WeekendLabel.Text = String.Format("{0} Weekend {1} ~ {2}", weekend.DutyTeam.Name, weekend.StartDate, weekend.EndDate);

                        List<WeekendActivity> activities = weekend.WeekendActivities.Where(act => !act.IsDeleted && act.showStudents).OrderBy(act => act.DateAndTime).ToList();

                        WeekendIdField.Value = Convert.ToString(value);
                        ActivityList.DataSource = activities;
                        ActivityList.DataTextField = "Name";
                        ActivityList.DataValueField = "id";
                        ActivityList.DataBind();

                        if(SignupCtrl.ActivityId == -1)
                        {
                            SignupCtrl.ActivityId = Convert.ToInt32(ActivityList.SelectedValue);
                        }

                        TableHeaderRow headerRow = new TableHeaderRow();
                        List<String> titles = new List<string>() { "Day", "Time", "Activity" };
                        foreach(String hdr in titles)
                        {
                            TableHeaderCell hc = new TableHeaderCell()
                            {
                                Text = hdr
                            };
                            headerRow.Cells.Add(hc);
                        }
                        ActivitiesTable.Rows.Add(headerRow);

                        foreach(WeekendActivity activity in activities)
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
                                       (activity.Duration == 0?"":activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString())
                            };
                            row.Cells.Add(timeCell);
                            TableCell actCell = new TableCell()
                            {
                                Text = activity.Name,
                                ToolTip = activity.Description
                            };
                            row.Cells.Add(actCell);
                            ActivitiesTable.Rows.Add(row);
                        }
                    }
                }
            }
        }

        public void SetActiity(int id)
        {
            SignupCtrl.ActivityId = id;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ActivityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int activityId = Convert.ToInt32(ActivityList.SelectedValue);
            SignupCtrl.ActivityId = activityId;
        }
    }
}