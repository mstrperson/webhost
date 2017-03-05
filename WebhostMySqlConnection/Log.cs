using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebhostMySQLConnection
{
    public class Log:StreamWriter
    {
        public const String __MAIL = "Mail";
        public const String __SUCCESS = "Success";
        public const String __ERROR = "Error";
        public const String __GENERAL = "General";
        public const String __ATTENDANCE = "Attendance";

        public static Log CommandLineLog = new Log(String.Format("CommandLineLog_{0}", DateTime.Now.Ticks));

        private String logName;

        public String FileName
        {
            get
            {
                return String.Format("~/Temp/{0}_{1}.txt", logName, DateTime.Now.Ticks);
            }
        }

        public override void WriteLine(string value)
        {
            value = String.Format("{0}:  {1}", Log.TimeStamp(), value);
            base.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            format = String.Format("{0}:  {1}", Log.TimeStamp(), format);
            base.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            format = String.Format("{0}:  {1}", Log.TimeStamp(), format);
            base.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            format = String.Format("{0}:  {1}", Log.TimeStamp(), format);
            base.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            format = String.Format("{0}:  {1}", Log.TimeStamp(), format);
            base.WriteLine(format, arg);
        }

        public Log(String LogName) : base(new FileStream(String.Format("C:\\Temp\\{0}.txt", LogName), FileMode.OpenOrCreate))
        {
            logName = LogName;
            this.AutoFlush = true;
        }

        public Log(String LogName, HttpServerUtility Server)
            : base(new FileStream(Server.MapPath(String.Format("~/Temp/{0}_{1}.txt", LogName, DateTime.Now.Ticks)), FileMode.CreateNew))
        {
            logName = LogName;
            this.AutoFlush = true;
        }

        public static String TimeStamp()
        {
            return String.Format("[{0}] ", DateTime.Now.ToLongTimeString());
        }
    }
}
