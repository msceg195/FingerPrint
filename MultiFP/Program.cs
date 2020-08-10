using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MultiFP
{
    static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException; ;
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            Application.Run(new LogForm());
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            using (EventLog log = new EventLog())
            {
                log.WriteEntry(((Exception)e.ExceptionObject).Message, EventLogEntryType.Error);
                Environment.Exit(1);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            using (EventLog log = new EventLog())
            {
                log.WriteEntry(e.Exception.Message, EventLogEntryType.Error);
            }
        }

        public static void Exit()
        {


        }
    }
}
