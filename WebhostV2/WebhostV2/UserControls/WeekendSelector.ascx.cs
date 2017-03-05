using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class WeekendSelector : LoggingUserControl
    {
        public event EventHandler WeekendIdChanged;

        public int WeekendID
        {
            get
            {
                try
                {
                    return (int)Session["WeekendId"];
                }
                catch
                {
                    Session["WeekendId"] = DateRange.GetCurrentWeekendId();
                    return (int)Session["WeekendId"];
                }
            }
            set
            {
                using (WebhostEntities db = new WebhostEntities())
                {
                    int prev = WeekendID;
                    if (db.Weekends.Where(w => w.id == value).Count() > 0)
                    {
                        Session["WeekendId"] = value;
                    }
                    else
                    {
                        Session["WeekendId"] = -1;
                    }

                    if(WeekendID != prev && WeekendIdChanged != null)
                    {
                        WeekendIdChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public DateRange WeekendRange
        {
            get
            {
                Regex dateEx = new Regex("^(0?[1-9]|1[0-2])/(0?[1-9]|[1-2][0-9]|3[0-1])/20[0-9]{2}$");

                if (dateEx.IsMatch(StartDate.Text))
                {
                    DateTime friday = DateRange.GetDateTimeFromString(StartDate.Text);

                    return new DateRange(friday, friday.AddDays(2));
                }

                return new DateRange(DateRange.ThisFriday, DateRange.ThisFriday.AddDays(2));
            }
        }

        protected void UpdateWeekendId()
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                foreach (Weekend weekend in db.Weekends)
                {
                    if (WeekendRange.Intersects(weekend.StartDate, weekend.EndDate))
                    {
                        WeekendID = weekend.id;
                        return;
                    }
                }

                Session["WeekendId"] = -1;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack && WeekendID != -1)
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendID).Single();
                    StartDate.Text = weekend.StartDate.ToShortDateString();
                    StartDate_CalendarExtender.SelectedDate = weekend.StartDate;
                }
            }
        }

        protected void WeekendSelectorUpdate_Load(object sender, EventArgs e)
        {
            DateTime prev = Session["friday"] == null ? new DateTime() : (DateTime)Session["friday"];
            DateTime friday = new DateTime();
            try
            {
                friday = DateRange.FridayOf(DateRange.GetDateTimeFromString(StartDate.Text));
            }
            catch (WebhostException)
            {
                friday = DateRange.ThisFriday;
            }

            if (!prev.Equals(friday))
            {
                StartDate.Text = friday.ToShortDateString();
                StartDate_CalendarExtender.SelectedDate = friday;

                Session["friday"] = friday;

                UpdateWeekendId();
            }
        }
    }
}