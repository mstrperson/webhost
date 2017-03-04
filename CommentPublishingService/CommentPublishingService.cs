using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebhostMySQLConnection;
using WebhostMySQLConnection.Web;
using WebhostMySQLConnection.EVOPublishing;
using System.IO;

namespace CommentPublishingService
{
    public partial class CommentPublishingService : ServiceBase
    {
        protected System.Timers.Timer timer;
        protected System.Timers.Timer prepQueueTimer;

        protected List<String> WorkingQueue = new List<string>();
        protected List<String> PendingQueue = new List<string>();

        public CommentPublishingService()
        {
            InitializeComponent();


            if (!System.Diagnostics.EventLog.SourceExists("CommentLetterPublishing"))
            {
                System.Diagnostics.EventLog.CreateEventSource("CommentLetterPublishing", "Application");
            }

            this.PublishingLog.Source = "CommentLetterPublishing";
            this.PublishingLog.Log = "Application";

            timer = new System.Timers.Timer() { AutoReset = true, Interval = TimeSpan.FromMinutes(3).TotalMilliseconds };
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            prepQueueTimer = new System.Timers.Timer() { AutoReset = true, Interval = TimeSpan.FromSeconds(30).TotalMilliseconds };
            prepQueueTimer.Elapsed += prepQueueTimer_Elapsed;
            prepQueueTimer.Start();
        }

        void prepQueueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (String file in Directory.GetFiles(PublishDirectory))
            {
                if (file.EndsWith(".pubreq") && !WorkingQueue.Contains(file) && !PendingQueue.Contains(file))
                {
                    PublishingLog.WriteEntry("Queueing Reqeust " + file, EventLogEntryType.Information);
                    PendingQueue.Add(file);
                }
            }

            if (!Working)
            {
                timer_Elapsed(prepQueueTimer, null);
            }
        }
        
        bool Working = false;
        static String PublishDirectory = @"C:\inetpub\wwwroot\Temp\";

        protected String TimeStamp
        {
            get
            {
                return String.Format("[{0} {1}]:\t", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
        
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Working)
            {
                PublishingLog.WriteEntry("Process is taking a while... Queue is working.", EventLogEntryType.Warning);
                return;
            }


            if (PendingQueue.Count > 0)
            {
                WorkingQueue.AddRange(PendingQueue);
                PendingQueue.Clear();
            }
            else
            {
                return;
            }

            Working = true;

            if (WorkingQueue.Count > 0)
                PublishingLog.WriteEntry(String.Format("{0} files are queued for this round.", WorkingQueue.Count), EventLogEntryType.Information);

            foreach (String fileName in WorkingQueue)
            {
                PublishingLog.WriteEntry("Beginning to publish " + fileName);
                try
                {
                    PublishRequest.ExecuteRequest(fileName);
                    PublishingLog.WriteEntry(String.Format("Completed Publish Request: {0}", fileName));
                }
                catch (Exception xmlex)
                {
                    PublishingLog.WriteEntry(xmlex.Message, EventLogEntryType.Error);
                }
            }

            Working = false;
            WorkingQueue.Clear();
        }

        protected override void OnStart(string[] args)
        {
            timer.Start();
            PublishingLog.WriteEntry("Comment Publishing Service Started.", EventLogEntryType.Information);
            MailControler.MailToWebmaster("Comment Service Started", "Comment Publishing Service has been started.");
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer.Dispose();
            PublishingLog.WriteEntry("Comment Publishing Service Stopped.", EventLogEntryType.Information);
            MailControler.MailToWebmaster("Comment Service Stopped", "Comment Publishing Service has stopped.");
        }
    }
}
