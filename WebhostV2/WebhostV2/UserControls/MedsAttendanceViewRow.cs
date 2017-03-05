using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebhostV2.UserControls
{
    public class MedsAttendanceViewRow : TableRow
    {
        protected Label NameBox;
        protected CheckBox MorningCB;
        protected CheckBox LunchCB;
        protected CheckBox DinnerCB;
        protected CheckBox BedtimeCB;

        public String StudentName
        {
            get
            {
                return NameBox.Text;
            }
            set
            {
                NameBox.Text = value;
            }
        }

        public bool MorningMeds
        {
            get
            {
                return MorningCB.Checked;
            }
            set
            {
                MorningCB.Checked = value;
            }
        }

        public bool LunchMeds
        {
            get
            {
                return LunchCB.Checked;
            }
            set
            {
                LunchCB.Checked = value;
            }
        }

        public bool DinnerMeds
        {
            get
            {
                return DinnerCB.Checked;
            }
            set
            {
                DinnerCB.Checked = value;
            }
        }

        public bool BedtimeMeds
        {
            get
            {
                return BedtimeCB.Checked;
            }
            set
            {
                BedtimeCB.Checked = value;
            }
        }

        public static TableHeaderRow Header
        {
            get
            {
                TableHeaderRow hdr = new TableHeaderRow();

                List<String> headers = new List<string>() { "Student Name (Year)", "Morning Meds", "Lunch Meds", "Dinner Meds", "Bedtime Meds" };
                foreach(String text in headers)
                {
                    TableHeaderCell cell = new TableHeaderCell()
                    {
                        Text = text
                    };
                    hdr.Cells.Add(cell);
                }
                return hdr;
            }
        }

        public MedsAttendanceViewRow(String text, bool morning, bool lunch, bool dinner, bool bedtime)
        {
            NameBox = new Label()
            {
                Text = text
            };

            MorningCB = new CheckBox()
            {
                Checked = morning,
                Enabled = false
            };

            LunchCB = new CheckBox()
            {
                Checked = lunch,
                Enabled = false
            };

            DinnerCB = new CheckBox()
            {
                Checked = dinner,
                Enabled = false
            };

            BedtimeCB = new CheckBox()
            {
                Checked = bedtime,
                Enabled = false
            };

            TableCell nc = new TableCell();
            nc.Controls.Add(NameBox);
            this.Cells.Add(nc);
            
            TableCell mc = new TableCell();
            mc.Controls.Add(MorningCB);
            this.Cells.Add(mc);

            TableCell lc = new TableCell();
            lc.Controls.Add(LunchCB);
            this.Cells.Add(lc);

            TableCell dc = new TableCell();
            dc.Controls.Add(DinnerCB);
            this.Cells.Add(dc);

            TableCell bc = new TableCell();
            bc.Controls.Add(BedtimeCB);
            this.Cells.Add(bc);
        }
    }
}