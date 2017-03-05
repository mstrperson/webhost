using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class DutyScheduleRow : TableRow, IComparable
    {
        public static TableHeaderRow HeaderRow
        {
            get
            {
                TableHeaderRow hr = new TableHeaderRow();
                hr.Cells.AddRange(new TableHeaderCell[] {
                    new TableHeaderCell() { Text = "Day and Time"},
                    new TableHeaderCell() { Text = "Activity"},
                    new TableHeaderCell() { Text = "Number of Signups" },
                    new TableHeaderCell() { Text = "Adults"}
                });

                return hr;
            }
        }

        private int _actId;
        public int ActivityId
        {
            get
            {
                return _actId;
            }
            protected set
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    if(db.WeekendActivities.Where(act => act.id == value).Count() > 0)
                    {
                        this._actId = value;

                        WeekendActivity activity = db.WeekendActivities.Where(act => act.id == value).Single();

                        sortDateTime = activity.DateAndTime;

                        TableCell DayCell = new TableCell();
                        TableCell ActivityCell = new TableCell();
                        TableCell CountCell = new TableCell() { Text = String.Format("{0}", activity.StudentSignups.Count) };
                        TableCell PeopleCell = new TableCell();

                        String DayText = String.Format("{0} {1} {2}",
                            activity.DateAndTime.DayOfWeek == DayOfWeek.Friday? "Friday":activity.DateAndTime.DayOfWeek == DayOfWeek.Saturday?"Saturday":"Sunday",
                            activity.DateAndTime.Hour == 0?"":activity.DateAndTime.ToShortTimeString(),
                            activity.DateAndTime.Hour ==0 || activity.Duration == 0?"":activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString());

                        DayCell.Text = DayText;

                        HyperLink link = new HyperLink() { Text = activity.Name, NavigateUrl = LinkUrl };
                        ActivityCell.Controls.Add(link);

                        String people = "";
                        bool first = true;
                        foreach(Faculty adult in activity.Adults)
                        {
                            people += (first ? "" : ", ") + adult.FirstName + " " + adult.LastName;
                            first = false;
                        }

                        PeopleCell.Text = people;
                        PeopleCell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;

                        if (activity.IsDeleted)
                        {
                            DayCell.BackColor = System.Drawing.Color.DarkRed;
                            ActivityCell.BackColor = System.Drawing.Color.DarkRed;
                            PeopleCell.BackColor = System.Drawing.Color.DarkRed;
                        }
                        this.Cells.Add(DayCell);
                        this.Cells.Add(ActivityCell);
                        this.Cells.Add(CountCell);
                        this.Cells.Add(PeopleCell);
                    }
                }
            }
        }

        private DateTime sortDateTime;

        private int _dutyId;
        public int DutyId
        {
            get { return _dutyId; }
            protected set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    if (db.WeekendDuties.Where(act => act.id == value).Count() > 0)
                    {
                        this._dutyId = value;

                        WeekendDuty activity = db.WeekendDuties.Where(act => act.id == value).Single();

                        sortDateTime = activity.DateAndTime;

                        TableCell DayCell = new TableCell();
                        TableCell ActivityCell = new TableCell() { ColumnSpan = 2 };
                        TableCell PeopleCell = new TableCell();

                        String DayText = String.Format("{0} {1} {2}",
                            activity.DateAndTime.DayOfWeek == DayOfWeek.Friday ? "Friday" : activity.DateAndTime.DayOfWeek == DayOfWeek.Saturday ? "Saturday" : "Sunday",
                            activity.DateAndTime.Hour == 0 ? "" : activity.DateAndTime.ToShortTimeString(),
                            activity.DateAndTime.Hour == 0 || activity.Duration == 0 ? "" : activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString());

                        DayCell.Text = DayText;

                        ActivityCell.Text = activity.Name;
                        String people = "";
                        if (activity.DutyTeamMembers.Count > 5)
                        {
                            people = "All Team";
                        }
                        else
                        {
                            bool first = true;
                            foreach (Faculty adult in activity.DutyTeamMembers.ToList())
                            {
                                people += (first ? "" : ", ") + adult.FirstName + " " + adult.LastName;
                                first = false;
                            }
                        }
                        PeopleCell.Text = people;
                        PeopleCell.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;

                        if (activity.IsDeleted)
                        {
                            DayCell.BackColor = System.Drawing.Color.DarkRed;
                            ActivityCell.BackColor = System.Drawing.Color.DarkRed;
                            PeopleCell.BackColor = System.Drawing.Color.DarkRed;
                        }

                        this.Cells.Add(DayCell);
                        this.Cells.Add(ActivityCell);
                        this.Cells.Add(PeopleCell);
                    }
                }
            }
        }

        public String LinkUrl
        {
            get;
            protected set;
        }

        public DutyScheduleRow(int Id, bool duty = false, bool mobile = false, bool faculty = true)
        {
            if (duty)
                DutyId = Id;
            else
            {
                if (!mobile && faculty)
                {
                    LinkUrl = String.Format("~/WeekendSignupTeacherView.aspx?act_id={0}", Id);
                }
                else if (!mobile)
                {
                    LinkUrl = String.Format("~/WeekendSignup.aspx?act_id={0}", Id);
                }
                else
                {
                    LinkUrl = String.Format("~/Mobile/Weekend.aspx?activity={0}", Id);
                }
                ActivityId = Id;
            }
        }

        public int CompareTo(object obj)
        {
            if(obj is DutyScheduleRow)
            {
                return this.sortDateTime.CompareTo(((DutyScheduleRow)obj).sortDateTime);
            }
            else
            {
                throw new InvalidOperationException("Cannot Compare those two objects.");
            }
        }
    }
}