using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WebhostMySQLConnection;
using WebhostMySQLConnection.SchoologyAPI;
using WebhostMySQLConnection.Web;
using System.Timers;

namespace SchoologySyncService
{
    public partial class SchoologySyncService : ServiceBase
    {
        protected Timer SyncTimer;
        protected Timer AdjustmentTimer;
        protected Timer CourseAndSectonsTimer;

        public SchoologySyncService()
        {
            InitializeComponent();
            if(!System.Diagnostics.EventLog.SourceExists("SchoologySyncSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource("SchoologySyncSource", "SchoologySyncLog");
            }

            this.ServiceEventLog.Source = "SchoologySyncSource";
            this.ServiceEventLog.Log = "SchoologySyncLog";

            SyncTimer = new Timer()
            {
                AutoReset = true,
                Interval = TimeSpan.FromMinutes(10).TotalMilliseconds
            };

            SyncTimer.Elapsed += SyncTimer_Elapsed;

            AdjustmentTimer = new Timer()
            {
                AutoReset = true,
                Interval = TimeSpan.FromMinutes(30).TotalMilliseconds
            };

            AdjustmentTimer.Elapsed += AdjustmentTimer_Elapsed;

            CourseAndSectonsTimer = new Timer()
            {
                AutoReset = true,
                Interval = TimeSpan.FromHours(6).TotalMilliseconds
            };

            CourseAndSectonsTimer.Elapsed += CourseAndSectonsTimer_Elapsed;
            CourseAndSectonsTimer.Start();

        }

        void CourseAndSectonsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            String Report = "";
            
            ServiceEventLog.WriteEntry("Checking for Changes to Schoology Courses and Sections.", EventLogEntryType.Information);
            Report += SchoologySync.GetCoursesFromSchoology();
            Report += "\n" + SchoologySync.GetSchoologySectionIdsForTerm(DateRange.GetCurrentOrLastTerm());
            ServiceEventLog.WriteEntry(Report, EventLogEntryType.SuccessAudit);
            ServiceEventLog.WriteEntry("Completed Course and Section Sync.", EventLogEntryType.Information);
            MailControler.MailToWebmaster("Schoology Course and Section Sync.", Report);
        }

        void AdjustmentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday) && SyncTimer.Enabled)
            {
                SyncTimer.Stop();
                CourseAndSectonsTimer.Stop();
                ServiceEventLog.WriteEntry("Turned off Attendance Sync for the Weekend.", EventLogEntryType.Information);
                return;
            }
            else if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                return;
            }

            switch (DateTime.Now.Hour)
            {
                case 6: SyncTimer.Start();
                    CourseAndSectonsTimer.Start();
                    ServiceEventLog.WriteEntry("Waking up Sync for the Day.");
                    break;
                case 8: SyncTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
                    ServiceEventLog.WriteEntry("Attendance Sync Interval set to 10 minutes for the Academic Day.", EventLogEntryType.Information);
                    break;
                case 17: SyncTimer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
                    ServiceEventLog.WriteEntry("Attendance Sync Interval set to 1 hour for off hours.", EventLogEntryType.Information);
                    break;
                case 22: SyncTimer.Stop();
                    CourseAndSectonsTimer.Stop();
                    ServiceEventLog.WriteEntry("Stopped Sync for the Night.", EventLogEntryType.Information);
                    break;
                default: break;
            }
        }

        void SyncTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServiceEventLog.WriteEntry("Syncing Schoology Attendances.", EventLogEntryType.Information);
            long ticks = DateTime.Now.Ticks;
            String report = "";
            //this.CanStop = false;
            try
            {
                report = AttendanceControl.PullFromSchoology();
            }
            catch (SchoologyAPICall.SchoologyAPIException apiex)
            {
                String Message = apiex.Message;
                Exception inner = apiex.InnerException;
                while(inner != null)
                {
                    Message += Environment.NewLine + "________________________________________" + Environment.NewLine + inner.Message;
                }
                ServiceEventLog.WriteEntry(String.Format("API Call failed:{0}{1}", Environment.NewLine, Message), EventLogEntryType.Error);
                MailControler.MailToWebmaster("Schoology Sync Service Error", String.Format("Schoology Sync Service Failed with the following error:{0}{0}{1}", Environment.NewLine, Message));
            }
            catch (Exception ex)
            {
                String Message = ex.Message;
                while(ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Message += Environment.NewLine + "________________________________________" + Environment.NewLine + ex.Message;
                }
                MailControler.MailToWebmaster("Schoology Sync Service Error", String.Format("Schoology Sync Service Failed with the following error:{0}{0}{1}", Environment.NewLine, Message));
                ServiceEventLog.WriteEntry(String.Format(Message), EventLogEntryType.Error);
            }
            //this.CanStop = true;
            ServiceEventLog.WriteEntry(report, EventLogEntryType.Information);
            ticks = DateTime.Now.Ticks - ticks;
            TimeSpan ts = new TimeSpan(ticks);
            ServiceEventLog.WriteEntry(String.Format("Syncronization Complete.  This Sync took {0}.", ts), EventLogEntryType.Information);
        }

        protected override void OnStart(string[] args)
        {
            ServiceEventLog.WriteEntry("Schoology Sync Sevice Starting.", EventLogEntryType.Information);
            SyncTimer.Start();
            AdjustmentTimer.Start();
            MailControler.MailToWebmaster("Schoology Sync Service Started.", "Sync Service is Started!");
        }

        protected override void OnStop()
        {
            SyncTimer.Stop();
            AdjustmentTimer.Stop();
            SyncTimer.Dispose();
            AdjustmentTimer.Dispose();
            ServiceEventLog.WriteEntry("Schoology Sync Service Is Stopped.", EventLogEntryType.Information);
            MailControler.MailToWebmaster("Schoology Sync Service Stopped.", "The Schoology Sync Service has been Stopped...");
        }
    }
}
