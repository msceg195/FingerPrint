using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using zkemkeeper;
namespace MultiFP
{
    public partial class LogForm : Form
    {
        #region Members

        private static readonly List<Thread> threads = new List<Thread>();
        private static Dictionary<int, string> lstMachines = null;
        private static readonly Dictionary<string, CZKEM> lstCZKEM = new Dictionary<string, CZKEM>();
        private static readonly Dictionary<int, string> lstDevices = new Dictionary<int, string>();
        private string ConnectionString => ConfigurationManager.AppSettings["ConnectionString"];
        private static List<Machine> machines = null;
        #endregion

        #region Devices Connection
        private Dictionary<int, string> GetMachines()
        {
            string sqlQuery = "SELECT Reader_ID , ReaderName , Remote_IP , F1 , F3 FROM tblREADERS WHERE Notes IS NOT NULL";
            lstMachines = new Dictionary<int, string>();
            machines = new List<Machine>();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed) connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                lstMachines.Add(Convert.ToInt32(reader["Reader_ID"]), Convert.ToString(reader["Remote_IP"]));

                                machines.Add(new Machine()
                                {
                                    Reader_ID = reader["Reader_ID"].ToString(),
                                    Remote_IP = reader["Remote_IP"].ToString(),
                                    ReaderName = reader["ReaderName"].ToString(),
                                    F1 = reader["F1"].ToString(),
                                    F3 = reader["F3"].ToString()
                                });
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Loger(ex.Message + " \t ");
            }
            return lstMachines;
        }

        private void TimerLog_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetAllLogs();
        }

        private void GetAllLogs()
        {
            foreach (KeyValuePair<int, string> machine in lstMachines)
            {
                CZKEM axCZKEM = new CZKEM();
                axCZKEM.Disconnect();

                if (axCZKEM.Connect_Net(machine.Value, 4370))
                {
                    axCZKEM.MachineNumber = machine.Key;

                    GetLogData(axCZKEM);
                    if (lstDevices.ContainsKey(machine.Key))
                    {
                        lstDevices.Remove(machine.Key);
                    }
                }
            }
            MethodInvoker lbl = delegate { lblText.Text = "Last Update " + DateTime.Now.ToString(); };
            Invoke(lbl);

            PingMachines();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                GetAllLogs();
            });
        }

        private void PingMachines()
        {
            MethodInvoker lst = delegate
            {
                lstOnline.Items.Clear();
                lstOffline.Items.Clear();
            };
            Invoke(lst);

            foreach (KeyValuePair<int, string> machine in lstMachines)
            {
                if (IPAddress.TryParse(machine.Value, out var ip))
                {
                    Ping ping = new Ping();
                    PingReply pingReply = ping.Send(ip);
                    if (pingReply != null)
                    {
                        if (pingReply.Status == IPStatus.Success)
                        {
                            MethodInvoker gv = delegate
                            {
                                lstOnline.Items.Add(ip);
                            };
                            Invoke(gv);
                        }
                        else
                        {
                            MethodInvoker gv = delegate
                            {
                                lstOffline.Items.Add(ip);
                            };
                            Invoke(gv);
                        }
                    }
                }
            }
        }

        #endregion

        #region Transaction

        protected void LogTransaction(int dwMachineNumber, int dwTMachineNumber, int dwEnrollNumber, int dwEMachineNumber, int dwVerifyMode, int dwInOutMode, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond)
        {
            try
            {
                if (GetRecordID(dwMachineNumber.ToString(), dwEnrollNumber.ToString(), dwInOutMode, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond) == null)
                {
                    InsertRecord(dwMachineNumber.ToString(), dwEnrollNumber.ToString(), dwInOutMode, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
                }
            }
            catch (Exception ex)
            {
                Loger(ex.Message);
            }
        }

        private void InsertRecord(string MachineNumber, string EnrollNumber, int AttState, int Year, int Month, int Day, int Hour, int Minute, int Second)
        {

            string state = "";
            Machine machine = machines.FirstOrDefault(q => q.Reader_ID == MachineNumber);
            if (machine.F1 == machine.F3)
            {
                state = machine.F1;
            }
            else if (machine.F1 != machine.F3)
            {
                state = (AttState == 0 ? "A" : "D");
            }

            string sqlQuery = "INSERT INTO [dbo].[tblTransactionsLog] ([BoardID], [ReaderIndex], [Date], [Time], [CardID], [Status], [FunctionKey], [CommTools])";
            sqlQuery += " VALUES ('" + MachineNumber + "', 1, '" + Year + "/" + Month + "/" + Day + "', '" + Hour + ":" + Minute + ":" + Second + "', '" +
            EnrollNumber + "', " + AttState + ", '" + state + "', 'RT')";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        int result = command.ExecuteNonQuery();
                        Loger(sqlQuery);
                    }
                    catch (Exception ex)
                    {
                        Loger(ex.Message);
                    }

                    Loger("Enroll Number : " + EnrollNumber + " | Machine Number : " + MachineNumber + " | " + DateTime.Now.ToString());
                }
            }
        }

        private object GetRecordID(string MachineNumber, string EnrollNumber, int AttState, int Year, int Month, int Day, int Hour, int Minute, int Second)
        {
            string sqlQuery = "Select TransactionID From [dbo].[tblTransactionsLog] Where [BoardID] = '" + MachineNumber + "' AND ";
            sqlQuery += "[Date] = N'" + Year + "/" + Month + "/" + Day + "'  AND [Time] =  N'" + Hour + ":" + Minute + ":" + Second + "' AND [CardID] =  N'" + EnrollNumber + "'";

            object result = null;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = CommandType.Text;

                    try
                    {
                        result = command.ExecuteScalar();

                        //Loger(sqlQuery);
                    }
                    catch (SqlException exSql)
                    {
                        //Loger(sqlQuery);
                        Loger(exSql.Message);
                    }
                    catch (Exception ex)
                    {
                        //Loger(sqlQuery);
                        Loger(ex.Message);
                    }
                }
            }

            return result;
        }

        private void UpdateBaudRate(int MachineNumber, string sDate)
        {
            string sqlQuery = "Update tblReaders set baudrate = '" + sDate + "' Where Reader_ID = " + MachineNumber;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = CommandType.Text;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exSql)
                    {
                        Loger(exSql.Message);
                    }
                    catch (Exception ex)
                    {
                        Loger(ex.Message);
                    }
                }
            }
        }

        private object GetBaudRate(int MachineNumber)
        {
            string sqlQuery = "Select baudrate From tblReaders Where Reader_ID = " + MachineNumber;

            object result = null;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.CommandType = CommandType.Text;

                    try
                    {
                        result = command.ExecuteScalar();

                        //Loger(sqlQuery);
                    }
                    catch (SqlException exSql)
                    {
                        //Loger(sqlQuery);
                        Loger(exSql.Message);
                    }
                    catch (Exception ex)
                    {
                        //Loger(sqlQuery);
                        Loger(ex.Message);
                    }
                }
            }

            return result;
        }
        #endregion

        #region Logging
        private void GetLogData(CZKEM axCZKEM)
        {
            try
            {
                int dwTMachineNumber = 0;
                string dwEnrollNumber = "";
                int dwEMachineNumber = 0;
                int dwVerifyMode = 0;
                int dwInOutMode = 0;
                int dwYear = 0;
                int dwMonth = 0;
                int dwDay = 0;
                int dwHour = 0;
                int dwMinute = 0;
                int dwSecond = 0;
                int dwWorkCode = 0;

                object BaudRate = GetBaudRate(axCZKEM.MachineNumber);
                string sTime = BaudRate == DBNull.Value ? "2019-01-01 00:00:00" : BaudRate.ToString().Replace('/', '-');
                string eTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).
                    ToString("yyyy-MM-dd HH:mm:ss");

                axCZKEM.EnableDevice(axCZKEM.MachineNumber, false);
                if (axCZKEM.ReadTimeGLogData(axCZKEM.MachineNumber, sTime, eTime))
                {
                    while (axCZKEM.SSR_GetGeneralLogData(axCZKEM.MachineNumber, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                    {
                        LogTransaction(axCZKEM.MachineNumber, dwTMachineNumber, int.Parse(dwEnrollNumber), dwEMachineNumber, dwVerifyMode, dwInOutMode, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
                    }
                    UpdateBaudRate(axCZKEM.MachineNumber, eTime);
                }
                else
                {
                    if (axCZKEM.ReadNewGLogData(axCZKEM.MachineNumber))
                    {
                        while (axCZKEM.SSR_GetGeneralLogData(axCZKEM.MachineNumber, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                        {
                            LogTransaction(axCZKEM.MachineNumber, dwTMachineNumber, int.Parse(dwEnrollNumber), dwEMachineNumber, dwVerifyMode, dwInOutMode, dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond);
                        }
                        UpdateBaudRate(axCZKEM.MachineNumber, eTime);
                    }
                }
                axCZKEM.EnableDevice(axCZKEM.MachineNumber, true);//enable the device 
            }
            catch (Exception ex)
            {
                Loger(ex.Message);
            }
        }

        private void Loger(string Message)
        {
            try
            {
                //MethodInvoker lbl = delegate { lblText.Text = Message + "\n\r"; };
                //Invoke(lbl);

                //    MethodInvoker gv = delegate
                //    {
                //        dgvLog.DataSource = null;
                //        dgvLog.DataSource = lstMachines.ToList();
                //        dgvLog.AutoGenerateColumns = true;
                //    };
                //    Invoke(gv);
            }
            catch (Exception ex)
            {
                Loger(ex.Message);
            }
        }
        #endregion

        #region Form Events
        private void NIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                WindowState = FormWindowState.Maximized;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Minimized;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        public LogForm()
        {
            InitializeComponent();
            GetMachines();
        }
        #endregion

        private void LogForm_Load(object sender, EventArgs e)
        {
            GetAllLogs();

            Text = "Login Date: " + DateTime.Now.ToShortDateString();
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 900000 //3600000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void NIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Minimized : FormWindowState.Normal;
        }
    }

    public class Machine
    {
        public string Reader_ID { get; set; }
        public string ReaderName { get; set; }
        public string Remote_IP { get; set; }
        public string F1 { get; set; }
        public string F3 { get; set; }
    }
}
