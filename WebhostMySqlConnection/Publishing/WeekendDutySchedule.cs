using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebhostMySQLConnection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using WebhostMySQLConnection.Web;

namespace WebhostMySQLConnection.Publishing
{
    /// <summary>
    /// Use as Disposable for memory conservation!  File is written on Dispose.
    /// </summary>
    public class WeekendDutySchedule : PDFPublisher
    {       

        public int WeekendId
        {
            get;
            private set;
        }

        public String DutyTeam
        {
            get;
            private set;
        }

        public DateRange WeekendDates
        {
            get;
            private set;
        }

        protected static readonly float FontSize = 10f;
        protected static readonly float HeaderFontSize = 12f;
        protected static readonly Font HeaderFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, HeaderFontSize);
        protected static readonly Font PageFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, FontSize);
        protected static readonly Font PageBold = FontFactory.GetFont(FontFactory.TIMES_BOLD, FontSize);

        
        /// <summary>
        /// Use as Disposable for memory conservation!  File is writtin to the disk on Dispose.
        /// 
        /// i.e. form of:
        /// 
        /// <code>
        /// string filename;
        /// using (WeekendDutySchedule ds = new WeekendDutySchedule(weekendId))
        /// {
        ///     filename = ds.Publish();
        /// }
        /// 
        /// Response.Redirect(filename);
        /// </code>
        /// </summary>
        /// <param name="weekendId"></param>
        public WeekendDutySchedule(int weekendId) : base("~/Temp/WeekendSchedule.pdf", "Weekend Duty Schedule")
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                if (db.Weekends.Where(w => w.id == weekendId).Count() <= 0) throw new WebhostException(String.Format("No weekend with id={0}", weekendId));
                WeekendId = weekendId;
                Weekend weekend = db.Weekends.Where(w => w.id == weekendId).Single();
                DutyTeam = weekend.DutyTeam.Name;
                WeekendDates = new DateRange(weekend.StartDate, weekend.EndDate);
            }
        }

        #region parts

        protected PdfPTable Header
        {
            get
            {
                PdfPTable table = new PdfPTable(1);
                PdfPCell c1 = new PdfPCell() { Border=0, Padding=1f };;
                c1.AddElement(new Paragraph(String.Format("TEAM {0}", DutyTeam.ToUpper()), HeaderFont));
                c1.HorizontalAlignment = 1;
                table.AddCell(c1);
                PdfPCell c2 = new PdfPCell() { Border=0, Padding=1f };;
                c2.AddElement(new Paragraph(String.Format("WEEKEND SCHEDULE:  {0}", WeekendDates), HeaderFont));
                c1.HorizontalAlignment = 1;
                table.AddCell(c2);
                table.SpacingAfter = 0.125f;
                return table;
            }
        }

        protected PdfPTable Members
        {
            get
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    PdfPTable table = new PdfPTable(3);
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                    int year = DateRange.GetCurrentAcademicYear();
                    List<Dorm> Dorms = db.Dorms.Where(d => d.AcademicYearId == year).ToList();
                    List<Faculty> DutyTeamMembers = weekend.DutyTeam.Members.ToList();
                    foreach(Dorm dorm in Dorms.OrderBy(d => d.Name))
                    {
                        PdfPCell dormCell = new PdfPCell() { Border=0, Padding=1f };;
                        Paragraph dp = new Paragraph(dorm.Name, PageBold);
                        dormCell.AddElement(dp);

                        PdfPCell nameCell = new PdfPCell() { Border=0, Padding=1f };;
                        PdfPCell phoneCell = new PdfPCell() { Border=0, Padding=1f };;
                        foreach(Faculty parent in dorm.DormParents)
                        {
                            if(DutyTeamMembers.Contains(parent))
                            {
                                Paragraph parp = new Paragraph(String.Format("{0} {1}{2}{3}", parent.FirstName, parent.LastName,
                                                                               weekend.DutyTeam.Leader.Equals(parent) ? " (DTL)" : "",
                                                                               weekend.DutyTeam.AdministratorOnDuty.Equals(parent) ? " (AOD)" : ""), PageBold);
                                Paragraph php = new Paragraph(parent.PhoneNumber, PageBold);
                                nameCell.AddElement(parp);
                                phoneCell.AddElement(php);
                                DutyTeamMembers.Remove(parent);
                            }
                        }
                        
                        table.AddCell(dormCell);
                        table.AddCell(nameCell);
                        table.AddCell(phoneCell);
                    }

                    foreach(Faculty notParent in DutyTeamMembers)
                    {
                        table.AddCell(new PdfPCell() { Border = 0 });
                        PdfPCell nameCell = new PdfPCell() { Border=0, Padding=1f };;
                        PdfPCell phoneCell = new PdfPCell() { Border=0, Padding=1f };;
                        Paragraph parp = new Paragraph(String.Format("{0} {1}{2}{3}", notParent.FirstName, notParent.LastName,
                                                                               weekend.DutyTeam.Leader.Equals(notParent) ? " (DTL)" : "",
                                                                               weekend.DutyTeam.AdministratorOnDuty.Equals(notParent) ? " (AOD)" : ""), PageBold);
                        Paragraph php = new Paragraph(notParent.PhoneNumber, PageBold);
                        nameCell.AddElement(parp);
                        phoneCell.AddElement(php);
                        table.AddCell(nameCell);
                        table.AddCell(phoneCell);
                    }

                    table.SpacingAfter = .25f;
                    return table;
                }
            }
        }

        public class WeekendTableItem
        {
            public DateTime DateAndTime;
            public List<Faculty> Adults;
            public int Duration;
            public String Name;
            public String Notes;

            public WeekendTableItem(WeekendActivity activity)
            {
                DateAndTime = activity.DateAndTime;
                Adults = activity.Adults.ToList();
                Duration = activity.Duration;
                Name = activity.Name;
                Notes = activity.Description;
            }

            public WeekendTableItem(WeekendDuty activity)
            {
                DateAndTime = activity.DateAndTime;
                Adults = activity.DutyTeamMembers.ToList();
                Duration = activity.Duration;
                Name = activity.Name;
                Notes = activity.Description;
            }
        }

        protected PdfPTable GetDailyEventsFor(DayOfWeek dayOfWeek)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                PdfPTable table = new PdfPTable(3);

                PdfPHeaderCell headerCell = new PdfPHeaderCell()
                {
                    Colspan = 3,
                    Border = 0,
                    Padding=1f
                };
                Paragraph hp = new Paragraph(WeekendDates.GetDayOfWeek(dayOfWeek).ToLongDateString(), HeaderFont);
                headerCell.AddElement(hp);
                table.AddCell(headerCell);

                Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();

                List<WeekendTableItem> things = new List<WeekendTableItem>();
                foreach (WeekendActivity activity in weekend.WeekendActivities.Where(act => !act.IsDeleted && act.DateAndTime.DayOfWeek == dayOfWeek).OrderBy(act => act.DateAndTime).ToList())
                {
                    things.Add(new WeekendTableItem(activity));
                }

                foreach(WeekendDuty duty in weekend.WeekendDuties.Where(d => !d.IsDeleted && d.DateAndTime.DayOfWeek == dayOfWeek).OrderBy(d => d.DateAndTime).ToList())
                {
                    things.Add(new WeekendTableItem(duty));
                }


                foreach(WeekendTableItem activity in things.OrderBy(act => act.DateAndTime))
                {
                    PdfPCell timeCell = new PdfPCell() { Border=0, Padding=1f };;
                    PdfPCell actCell = new PdfPCell() { Border=0, Padding=1f };;
                    PdfPCell adsCell = new PdfPCell() { Border=0, Padding=1f };;

                    Paragraph time = new Paragraph(String.Format("{0}{1}", activity.DateAndTime.Hour == 0 ? "All Day" : activity.DateAndTime.ToShortTimeString(),
                                                                           activity.Duration == 0 ? "" : " ~ " + activity.DateAndTime.AddMinutes(activity.Duration).ToShortTimeString()), PageFont);
                    timeCell.AddElement(time);
                    table.AddCell(timeCell);

                    Paragraph actP = new Paragraph(String.Format("{0}{1}", activity.Name,
                        activity.Notes.Equals("") ? "" : String.Format(" ({0})", activity.Notes)), PageFont);
                    actCell.AddElement(actP);
                    table.AddCell(actCell);

                    if (activity.Adults.Count < 4)
                    {
                        foreach (Faculty adult in activity.Adults)
                        {
                            Paragraph adp = new Paragraph(String.Format("{0} {1}", adult.FirstName, adult.LastName), PageFont);
                            adsCell.AddElement(adp);
                        }
                    }

                    else
                    {
                        Paragraph adp = new Paragraph("All Team", PageFont);
                        adsCell.AddElement(adp);
                    }
                    table.AddCell(adsCell);
                }

                return table;
            }
        }

        protected PdfPTable Notes
        {
            get
            {
                using(WebhostEntities db = new WebhostEntities())
                {
                    Weekend weekend = db.Weekends.Where(w => w.id == WeekendId).Single();
                    PdfPTable table = new PdfPTable(1);
                    PdfPCell hdrc = new PdfPCell()
                    {
                        Border = 0
                    };
                    hdrc.AddElement(new Paragraph("Weekend Notes:", HeaderFont));
                    table.AddCell(hdrc);
                    PdfPCell nc = new PdfPCell()
                    {
                        Border = 0
                    };
                    nc.AddElement(new Paragraph(weekend.Notes, PageFont));
                    return table;
                }
            }
        }

        #endregion

        new public string Publish()
        {
            this.document.Add(Header);
            this.document.Add(Members);
            this.document.Add(GetDailyEventsFor(DayOfWeek.Friday));
            this.document.Add(GetDailyEventsFor(DayOfWeek.Saturday));
            this.document.Add(GetDailyEventsFor(DayOfWeek.Sunday));
            this.document.Add(Notes);
            return base.Publish();
        }
    }
}