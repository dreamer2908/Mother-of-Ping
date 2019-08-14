using Mother_of_Ping_CLI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mother_of_Ping_GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataTable bigData;
        SyncBindingSource bind;

        Icon icon_ok = Mother_of_Ping_GUI.Properties.Resources.icon_ok;
        Icon icon_warning = Mother_of_Ping_GUI.Properties.Resources.icon_warning;
        Icon icon_blank = Mother_of_Ping_GUI.Properties.Resources.icon_blank;

        List<string[]> hostList;
        pingWork[] workForce;

        int pingPref_period = 1000;
        int pingPref_timeout = 1000;
        int pingPref_bufferSize = 32;
        int pingPref_ttl = 128;

        bool appPref_autoStart = true;
        bool appPref_saveHostList = true;
        bool appPref_autoLoadList = false;
        string appPref_autoLoadListFilename = "";
        string defaultHostListPath = Path.Combine(Application.StartupPath.ToString(), "hostlist.csv");

        bool appPref_markHostConsFail = true;
        int appPref_markHostConsFailThreshold = 300;
        bool appPref_sendTaskbarNotifications = true;
        bool appPref_sendLineNotifications = true;

        bool appPref_saveGlobalLog = true;
        bool appPref_saveIndividualLog = true;
        string appPref_globalLogFilename = "0.0.0.0.csv";
        string appPref_logFolder = "";
        bool appPref_useTodayFolder = true;
        int appPref_flushLogPeriod = 600;
        string actualLogFolder = "";

        bool appPref_generateReportAtExit = true;

        Mutex mutexLogFlush = new Mutex();

        #region events

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;

                loadNewHostListAio(filename);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (hostList != null)
            {
                startPingAio();
            }
            else
            {
                MessageBox.Show("There's nothing to run.", "Start", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopPing();
            stopLogFlushing();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            updateStats();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //bind.ResumeBinding();
            Thread.Sleep(1000);
            backgroundWorker1.RunWorkerAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopPing();
            saveSettings();
            flushLogToDisk();

            if (appPref_generateReportAtExit && workForce != null)
            {
                string today = getTodayString();
                string filename1 = Path.Combine(actualLogFolder, string.Format("ping_session_report_{0}.csv", today));
                string filename2 = Path.Combine(actualLogFolder, string.Format("ping_session_report_{0}.html", today));
                tools.generateCsvReport(workForce, filename1);
                tools.generateHtmlReport(workForce, filename2);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            resetTable();
            loadSettings();
            if (appPref_autoStart)
            {
                startPingAio();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateStats();
            updateRowColor();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (workForce == null)
            {
                MessageBox.Show("There's nothing to report.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        // selected csv
                        tools.generateCsvReport(workForce, saveFileDialog1.FileName);
                        break;
                    case 2:
                        //selected html
                        tools.generateHtmlReport(workForce, saveFileDialog1.FileName);
                        break;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            var options = new Form2
            {
                pingPref_period = pingPref_period,
                pingPref_timeout = pingPref_timeout,
                pingPref_bufferSize = pingPref_bufferSize,
                pingPref_ttl = pingPref_ttl,

                appPref_autoStart = appPref_autoStart,
                appPref_saveHostList = appPref_saveHostList,
                appPref_autoLoadList = appPref_autoLoadList,
                appPref_autoLoadListFilename = appPref_autoLoadListFilename,

                appPref_markHostConsFail = appPref_markHostConsFail,
                appPref_markHostConsFailThreshold = appPref_markHostConsFailThreshold,
                appPref_sendTaskbarNotifications = appPref_sendTaskbarNotifications,
                appPref_sendLineNotifications = appPref_sendLineNotifications,

                appPref_saveGlobalLog = appPref_saveGlobalLog,
                appPref_saveIndividualLog = appPref_saveIndividualLog,
                appPref_globalLogFilename = appPref_globalLogFilename,
                appPref_logFolder = appPref_logFolder,
                appPref_useTodayFolder = appPref_useTodayFolder,
                appPref_flushLogPeriod = appPref_flushLogPeriod,
                appPref_generateReportAtExit = appPref_generateReportAtExit
            };

            if (options.ShowDialog() == DialogResult.OK)
            {
                pingPref_period = options.pingPref_period;
                pingPref_timeout = options.pingPref_timeout;
                pingPref_bufferSize = options.pingPref_bufferSize;
                pingPref_ttl = options.pingPref_ttl;

                appPref_autoStart = options.appPref_autoStart;
                appPref_saveHostList = options.appPref_saveHostList;
                appPref_autoLoadList = options.appPref_autoLoadList;
                appPref_autoLoadListFilename = options.appPref_autoLoadListFilename;

                appPref_markHostConsFail = options.appPref_markHostConsFail;
                appPref_markHostConsFailThreshold = options.appPref_markHostConsFailThreshold;
                appPref_sendTaskbarNotifications = options.appPref_sendTaskbarNotifications;
                appPref_sendLineNotifications = options.appPref_sendLineNotifications;

                appPref_saveGlobalLog = options.appPref_saveGlobalLog;
                appPref_saveIndividualLog = options.appPref_saveIndividualLog;
                appPref_globalLogFilename = options.appPref_globalLogFilename;
                appPref_logFolder = options.appPref_logFolder;
                appPref_useTodayFolder = options.appPref_useTodayFolder;

                if (appPref_flushLogPeriod != options.appPref_flushLogPeriod)
                {
                    refreshTimer(timer2, appPref_flushLogPeriod);
                }
                appPref_flushLogPeriod = options.appPref_flushLogPeriod;

                appPref_generateReportAtExit = options.appPref_generateReportAtExit;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            flushLogToDisk();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            notifyOfflineHost();
        }
        #endregion

        private void startPingAio()
        {
            startPing();
            startGridUpdate();
            startLogFlushing();
        }

        private void loadNewHostListAio(string filename)
        {
            if (filename.ToLower().EndsWith(".csv"))
            {
                hostList = tools.csvHostListParser(filename, false);
            }
            else
            {
                hostList = tools.txtPinginfoviewParser(filename, false);
            }

            stopGridUpdate();
            stopLogFlushing();
            cleanUpOldThreads();
            resetTable();
            loadHostListToTable(hostList);
        }

        private void cleanUpOldThreads()
        {
            if (workForce != null)
            {
                stopPing();
                workForce = null;
            }
        }

        private void resetTable()
        {
            bigData = new DataTable();
            bind = new SyncBindingSource();
            bind.DataSource = bigData;

            //dgvPing.AutoGenerateColumns = false;
            dgvPing.DataSource = bigData;
            dgvPing.DataSource = bind;

            bigData.Columns.Add("   ", typeof(Icon)); // 0
            bigData.Columns.Add("Notice", typeof(bool)); // 1
            bigData.Columns.Add("No.", typeof(int)); // 2
            bigData.Columns.Add("Host Name", typeof(string)); // 3
            bigData.Columns.Add("Description", typeof(string)); // 4
            bigData.Columns.Add("Reply IP Address", typeof(string)); // 5
            bigData.Columns.Add("Succeed Count", typeof(int)); // 6
            bigData.Columns.Add("Failed Count", typeof(int)); // 7
            bigData.Columns.Add("Consecutive Failed Count", typeof(int)); // 8
            bigData.Columns.Add("Max Consecutive Failed Count", typeof(int)); // 9
            bigData.Columns.Add("Max Consecutive Failed Time", typeof(string)); // 10
            bigData.Columns.Add("% Failed", typeof(string)); // 11
            bigData.Columns.Add("Last Ping Status", typeof(string)); // 12
            bigData.Columns.Add("Last Ping Time", typeof(int)); // 13
            bigData.Columns.Add("Average Ping Time", typeof(string)); // 14
            bigData.Columns.Add("Last Succeed On", typeof(string)); // 15
            bigData.Columns.Add("Last Failed On", typeof(string)); // 16
            bigData.Columns.Add("Minimum Ping Time", typeof(int)); // 17
            bigData.Columns.Add("Maximum Ping Time", typeof(int)); // 18

            dgvPing.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            dgvPing.Refresh();


            // from https://10tec.com/articles/why-datagridview-slow.aspx
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;

            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dgvPing.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgvPing, true, null);
            }

            cleanUpOldThreads();
        }

        private void enableDgrAutoSize()
        {
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void disableDgrAutoSize()
        {
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void loadHostListToTable(List<string[]> hostList)
        {
            bigData.BeginLoadData();
            int numOfHost = hostList.Count;

            for (int i = 0; i < numOfHost; i++)
            {
                bigData.Rows.Add(icon_blank, true, i, hostList[i][0], hostList[i][1]);
            }
            bigData.EndLoadData();
        }

        private void startPing()
        {
            stopPing(); // stop all ongoing workers

            if (workForce == null)
            {
                workForce = new pingWork[hostList.Count];
            }

            int[] index = new int[hostList.Count];
            for (int i = 0; i < index.Length; i++) index[i] = i;

            Parallel.ForEach(index, (i) => 
            {
                pingWork work = (workForce[i] == null) ? new pingWork() : workForce[i];
                workForce[i] = work;

                work.id = i;
                work.hostname = hostList[i][0];
                work.description = hostList[i][1];
                work.period = pingPref_period;
                work.timeout = pingPref_timeout;
                work.bufferSize = pingPref_bufferSize;
                work.ttl = pingPref_ttl;

                work.startPing();
            });
        }

        private void stopPing()
        {
            if (workForce != null)
            {
                foreach (pingWork worker in workForce)
                {
                    worker.stopPing();
                }
            }
        }

        private void resetPing()
        {
            workForce = new pingWork[hostList.Count];
        }

        private void updateStats()
        {
            if (workForce != null)
            {
                foreach (pingWork worker in workForce)
                {
                    int id = worker.id;
                    int rowId = id;

                    //for (int i = 0; i > table.Rows.Count; i++)
                    //{
                    //    if ((int)table.Rows[i][2] == id) rowId = i;
                    //}

                    pingWork.pingStatus lastReply_result = worker.lastReply_result;

                    var row = bigData.Rows[rowId];

                    row.BeginEdit();

                    row[0] = (lastReply_result == pingWork.pingStatus.online) ? icon_ok : icon_warning;

                    row[5] = worker.lastReply_address;
                    row[6] = worker.upCount;
                    row[7] = worker.downCount;
                    row[8] = worker.consecutiveDownCount;
                    row[9] = worker.maxConsecutiveDownCount;
                    row[10] = worker.maxConsecutiveDownTimestamp;
                    row[11] = worker.percentDown;
                    row[12] = pingWork.pingStatusToText[lastReply_result];
                    row[13] = worker.lastReply_time;
                    row[14] = string.Format("{0:0.#}", worker.avgPingTime);
                    row[15] = worker.lastUpTimestamp;
                    row[16] = worker.lastDownTimestamp;
                    row[17] = worker.minPingTime;
                    row[18] = worker.maxPingTime;

                    row.EndEdit();
                    row.AcceptChanges();
                }
            }
        }

        private void startGridUpdate()
        {
            //disableDgrAutoSize();
            //backgroundWorker1.RunWorkerAsync();
            timer1.Interval = 1000; // in miliseconds
            timer1.Start();

            timer3.Interval = 60000;
            timer3.Start();
        }

        private void stopGridUpdate()
        {
            timer1.Stop();
            timer3.Stop();
        }

        private void loadSettings()
        {
            pingPref_period = Settings.Get("pingPref_period", 1000);
            pingPref_timeout = Settings.Get("pingPref_timeout", 1000);
            pingPref_bufferSize = Settings.Get("pingPref_bufferSize", 32);
            pingPref_ttl = Settings.Get("pingPref_ttl", 128);

            appPref_autoStart = Settings.Get("appPref_autoStart", true);
            appPref_saveHostList = Settings.Get("appPref_saveHostList", true);
            appPref_autoLoadList = Settings.Get("appPref_autoLoadList", false);
            appPref_autoLoadListFilename = Settings.Get("appPref_autoLoadListFilename", "hostlist.csv");

            appPref_markHostConsFail = Settings.Get("appPref_markHostConsFail", true);
            appPref_markHostConsFailThreshold = Settings.Get("appPref_markHostConsFailThreshold", 300);
            appPref_sendTaskbarNotifications = Settings.Get("appPref_sendTaskbarNotifications", true);
            appPref_sendLineNotifications = Settings.Get("appPref_sendLineNotifications", false);

            appPref_saveGlobalLog = Settings.Get("appPref_saveGlobalLog", true);
            appPref_saveIndividualLog = Settings.Get("appPref_saveIndividualLog", true);
            appPref_globalLogFilename = Settings.Get("appPref_globalLogFilename", "0.0.0.0.csv");
            appPref_logFolder = Settings.Get("appPref_logFolder", "");
            appPref_useTodayFolder = Settings.Get("appPref_useTodayFolder", true);
            appPref_flushLogPeriod = Settings.Get("appPref_flushLogPeriod", 600);

            appPref_generateReportAtExit = Settings.Get("appPref_generateReportAtExit", true);

            if (appPref_saveHostList)
            {
                if (File.Exists(defaultHostListPath))
                {
                    loadNewHostListAio(defaultHostListPath);
                }
            }
            else if (appPref_autoLoadList)
            {
                if (File.Exists(appPref_autoLoadListFilename))
                {
                    loadNewHostListAio(appPref_autoLoadListFilename);
                }
            }
        }

        private void saveSettings()
        {
            Settings.Set("pingPref_period", pingPref_period.ToString());
            Settings.Set("pingPref_timeout", pingPref_timeout.ToString());
            Settings.Set("pingPref_bufferSize", pingPref_bufferSize.ToString());
            Settings.Set("pingPref_ttl", pingPref_ttl.ToString());

            Settings.Set("appPref_autoStart", appPref_autoStart.ToString());
            Settings.Set("appPref_saveHostList", appPref_saveHostList.ToString());
            Settings.Set("appPref_autoLoadList", appPref_autoLoadList.ToString());
            Settings.Set("appPref_autoLoadListFilename", appPref_autoLoadListFilename);

            Settings.Set("appPref_markHostConsFail", appPref_markHostConsFail.ToString());
            Settings.Set("appPref_markHostConsFailThreshold", appPref_markHostConsFailThreshold.ToString());
            Settings.Set("appPref_sendTaskbarNotifications", appPref_sendTaskbarNotifications.ToString());
            Settings.Set("appPref_sendLineNotifications", appPref_sendLineNotifications.ToString());

            Settings.Set("appPref_saveGlobalLog", appPref_saveGlobalLog.ToString());
            Settings.Set("appPref_saveIndividualLog", appPref_saveIndividualLog.ToString());
            Settings.Set("appPref_globalLogFilename", appPref_globalLogFilename);
            Settings.Set("appPref_logFolder", appPref_logFolder);
            Settings.Set("appPref_useTodayFolder", appPref_useTodayFolder.ToString());
            Settings.Set("appPref_flushLogPeriod", appPref_flushLogPeriod.ToString());
            Settings.Set("appPref_generateReportAtExit", appPref_generateReportAtExit);

            // save host list
            if (appPref_saveHostList)
            {
                // convert to ConcurrentQueue<string[]> then write down
                ConcurrentQueue<string[]> tmp = new ConcurrentQueue<string[]>(hostList);
                tools.writeCsv_ConcurrentQueue(tmp, defaultHostListPath, true);
            }
        }

        private void updateRowColor()
        {
            foreach (DataGridViewRow row in dgvPing.Rows)
            {
                if (appPref_markHostConsFail && Convert.ToInt32(row.Cells[8].Value) > appPref_markHostConsFailThreshold)
                {
                    row.DefaultCellStyle.BackColor = Color.OrangeRed;
                }
                else if (row.Cells[12].Value.ToString() != pingWork.pingStatusToText[pingWork.pingStatus.online])
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void notifyOfflineHost()
        {
            List<string> mess = new List<string>();
            foreach (DataGridViewRow row in dgvPing.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.OrangeRed)
                {
                    string message = string.Format("{0} is offline for {1:0} seconds", row.Cells[3].Value, Convert.ToInt32(row.Cells[8].Value) * pingPref_period / 1000);
                    //sendTrayNotification(message);
                    mess.Add(message);
                }
            }

            if (mess.Count > 0)
            {
                string message = String.Join("\n", mess);
                sendTrayNotification(message, 5000);
            }
        }

        private void sendTrayNotification(string message, int duration)
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = System.Drawing.SystemIcons.Information,
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                BalloonTipText = message,
            };

            // eliminate the notification when clicked or closed
            notification.BalloonTipClosed += (sender, args) => notification.Dispose();
            notification.BalloonTipClicked += (sender, args) => notification.Dispose();

            // display for x miliseconds
            // note that duration > 5s is ignored in vista+
            notification.ShowBalloonTip(duration);
        }

        private void startLogFlushing()
        {
            timer2.Interval = appPref_flushLogPeriod * 1000; // note interval is ms and period is second
            timer2.Start();
        }

        private void stopLogFlushing()
        {
            timer2.Stop();
        }

        private void flushLogToDisk()
        {
            if (workForce == null) return;

            mutexLogFlush.WaitOne();

            if (appPref_useTodayFolder)
            {
                string today = getTodayString();
                actualLogFolder = Path.Combine(appPref_logFolder, today);
                Directory.CreateDirectory(actualLogFolder);
            }

            if (appPref_saveGlobalLog)
            {
                string globalFilePath = appPref_globalLogFilename;

                // if appPref_useTodayFolder, ignore path in appPref_globalLogFilename and put the log in today folder
                if (appPref_useTodayFolder)
                {
                    string filename = Path.GetFileName(appPref_globalLogFilename);
                    globalFilePath = Path.Combine(actualLogFolder, filename);
                }

                tools.writeCsv_ConcurrentQueue(pingWork.globalLog, globalFilePath, false);
            }
            else
            {
                // clear log if not to save
                tools.clearConcurrentQueue(pingWork.globalLog);
            }

            if (appPref_saveIndividualLog)
            {
                foreach(var worker in workForce)
                {
                    string filePath = worker.hostname + ".csv";
                    if (appPref_useTodayFolder)
                    {
                        filePath = Path.Combine(actualLogFolder, filePath);
                    }

                    tools.writeCsv_ConcurrentQueue(worker.log, filePath, false);
                };
            }
            else
            {
                // clear log if not to save
                Parallel.ForEach(workForce, (worker) =>
                {
                    tools.clearConcurrentQueue(worker.log);
                });
            }

            mutexLogFlush.ReleaseMutex();
        }

        private void refreshTimer(System.Windows.Forms.Timer timer, int interval)
        {
            timer.Stop();
            timer.Interval = interval;
            timer.Start();
        }

        private static string getTodayString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
