using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection.Web;

namespace WebhostV2.UserControls
{
    public partial class TimeSelector : LoggingUserControl
    {
        protected Regex timeEx = new Regex("([0-9]|1[0-2]):[0-5][0-9]");

        public event EventHandler TimeUpdated;

        protected virtual void OnTimeUpdated(object sender, EventArgs e)
        {
            if(TimeUpdated != null)
            {
                TimeUpdated(sender, e);
            }
        }

        public int Hour
        {
            get
            {
                if (timeEx.IsMatch(TimeInput.Text))
                {
                    int hr = Convert.ToInt32(TimeInput.Text.Split(':')[0]);
                    if(hr > 12)
                    {
                        hr -= 12;
                    }

                    if(hr == 12)
                    {
                        hr = 12 * AMPM.SelectedIndex;
                    }
                    else
                    {
                        hr += 12 * AMPM.SelectedIndex;
                    }

                    return hr;
                }
                
                throw new WebhostException(String.Format("Invalid Time String: {0}", TimeInput.Text));
            }
        }

        public int Minute
        {
            get
            {
                if (timeEx.IsMatch(TimeInput.Text))
                    return Convert.ToInt32(TimeInput.Text.Split(':')[1]);

                throw new WebhostException(String.Format("Invalid Time String: {0}", TimeInput.Text));
            }
        }

        public void SetTime(DateTime time)
        {
            this.SetTime(time.Hour, time.Minute);
        }

        public void SetTime(int hour, int minute)
        {
            if(hour == 12)
            {
                AMPM.SelectedIndex = 1;
            }
            else if(hour > 12)
            {
                AMPM.SelectedIndex = 1;
                hour %= 12;
            }
            else
            {
                AMPM.SelectedIndex = 0;
            }

            if (hour == 0) hour = 12;

            TimeInput.Text = String.Format("{0}:{1}", hour, minute == 0 ? "00" : (minute < 10 ? String.Format("0{0}", minute) : Convert.ToString(minute)));
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}