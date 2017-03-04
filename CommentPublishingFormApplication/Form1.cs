using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using WebhostMySQLConnection.EVOPublishing;
using WebhostMySQLConnection;

namespace CommentPublishingFormApplication
{
    public partial class Form1 : Form
    {
        protected System.Timers.Timer timer;
        protected System.Timers.Timer messageTimer;
        protected System.Timers.Timer prepQueueTimer;

        protected List<String> WorkingQueue = new List<string>();
        protected List<String> PendingQueue = new List<string>();

        protected int RequestCount = 0;

        public Form1()
        {
            InitializeComponent();
            timer = new System.Timers.Timer() { AutoReset = true, Interval = TimeSpan.FromMinutes(3).TotalMilliseconds };
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            messageTimer = new System.Timers.Timer() { AutoReset = true, Interval = TimeSpan.FromSeconds(5).TotalMilliseconds };
            messageTimer.Elapsed += messageTimer_Elapsed;
            messageTimer.Start();

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
                    QueueMessage("Queueing Reqeust " + file);
                    PendingQueue.Add(file);
                }
            }

            if(!Working)
            {
                timer_Elapsed(prepQueueTimer, null);
            }
        }

        void messageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Background Process Log:" + Environment.NewLine);

            foreach(String line in MessageQueue)
            {
                builder.Append(line + Environment.NewLine);
            }

            UpdateBackgroundLogText(builder.ToString());
        }

        delegate void UpdateBackgroundLogTextCallback(String text);

        private void UpdateBackgroundLogText(String text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.BackgroundProcessLog.InvokeRequired)
            {
                UpdateBackgroundLogTextCallback d = new UpdateBackgroundLogTextCallback(UpdateBackgroundLogText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.BackgroundProcessLog.Text = text;
                this.StateLabel.Text = String.Format("Completed {0} requests.", RequestCount);
                this.WorkingLabel.Text = String.Format("{0} Working Now.", WorkingQueue.Count);
                this.PendingLabel.Text = String.Format("{0} Waiting to Queue.", PendingQueue.Count);

            }
        }

        bool Working = false;
        static String PublishDirectory = "W:\\Temp\\";
        int QueueMaxSize = 50;
        List<String> MessageQueue = new List<string>();



        protected String TimeStamp
        {
            get
            {
                return String.Format("[{0} {1}]:\t", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }

        protected void QueueMessage(String message)
        {
            
            MessageQueue.Add(TimeStamp + message);

            if(MessageQueue.Count > QueueMaxSize + 1)
            {
                MessageQueue.RemoveRange(0, MessageQueue.Count - QueueMaxSize);
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Working)
            {
                QueueMessage("Process is taking a while... Queue is working.");
                return;
            }


            if(PendingQueue.Count > 0)
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
                QueueMessage(String.Format("{0} files are queued for this round.", WorkingQueue.Count));

            foreach (String fileName in WorkingQueue)
            {
                QueueMessage("Beginning to publish " + fileName);
                try
                {
                    PublishRequest.ExecuteRequest(fileName);
                    RequestCount++;
                    QueueMessage(String.Format("Completed Publish Request: {0}", fileName));
                }
                catch (Exception xmlex)
                {
                    QueueMessage(xmlex.Message);
                }
            }

            Working = false;
            WorkingQueue.Clear();
        }

        private void StopServiceBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
