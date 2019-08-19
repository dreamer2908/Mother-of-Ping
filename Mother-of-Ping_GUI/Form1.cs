﻿using Mother_of_Ping_CLI;
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

        DataTable smallData;
        SyncBindingSource smallBind;

        Icon icon_ok = Mother_of_Ping_GUI.Properties.Resources.icon_ok;
        Icon icon_warning = Mother_of_Ping_GUI.Properties.Resources.icon_warning;
        Icon icon_blank = Mother_of_Ping_GUI.Properties.Resources.icon_blank;

        List<string[]> hostList = new List<string[]>();
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

        bool appPref_showLowerPanel = true;
        bool appPref_showLowerPanel_onlyFailed = false;
        int appPref_showLowerPanel_limit = 50;

        Mutex mutexLogFlush = new Mutex();
        Mutex mutexStopPing = new Mutex();

        bool pingStarted = false;

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
            if (hostList.Count > 0)
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
            stopPing(false);
            stopLogFlushing();
            stopNotifyOfflineHost();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            resetStats();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopPing(false);
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
            resetPingPanel();
            resetLowerPanel();

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
            updateLowerPanel();
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

                if (File.Exists(saveFileDialog1.FileName))
                {
                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
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
                appPref_generateReportAtExit = appPref_generateReportAtExit,
                appPref_showLowerPanel = appPref_showLowerPanel,
                appPref_showLowerPanel_onlyFailed = appPref_showLowerPanel_onlyFailed,
                appPref_showLowerPanel_limit = appPref_showLowerPanel_limit,
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

                appPref_showLowerPanel = options.appPref_showLowerPanel;
                appPref_showLowerPanel_onlyFailed = options.appPref_showLowerPanel_onlyFailed;

                appPref_showLowerPanel_limit = options.appPref_showLowerPanel_limit;
                updateLatestLogSizeLimit();

                showHideLowerPanel();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }
        #endregion

        private void startPingAio()
        {
            if (hostList.Count > 0)
            {
                startPing();
                startGridUpdate();
                startLogFlushing();
            }
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
            clearPingPanel();
            loadHostListToTable(hostList);
            clearLowerPanel();
        }

        private void cleanUpOldThreads()
        {
            if (workForce != null)
            {
                stopPing(false);
                workForce = null;
            }
        }

        private void resetPingPanel()
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
            bigData.Columns.Add("Max Consecutive Failed On", typeof(string)); // 10
            bigData.Columns.Add("Max Consecutive Failed Duration", typeof(string)); // 11
            bigData.Columns.Add("% Failed", typeof(string)); // 12
            bigData.Columns.Add("Last Ping Status", typeof(string)); // 13
            bigData.Columns.Add("Last Ping Time", typeof(string)); // 14
            bigData.Columns.Add("Average Ping Time", typeof(string)); // 15
            bigData.Columns.Add("Last Succeed On", typeof(string)); // 16
            bigData.Columns.Add("Last Failed On", typeof(string)); // 17
            bigData.Columns.Add("Last Failed Duration", typeof(string)); // 18
            bigData.Columns.Add("Minimum Ping Time", typeof(string)); // 19
            bigData.Columns.Add("Maximum Ping Time", typeof(string)); // 20

            dgvPing.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            dgvPing.Refresh();


            // from https://10tec.com/articles/why-datagridview-slow.aspx
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvPing.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvPing.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dgvPing.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            //dgvPing.Columns[0].Width = 32;
            dgvPing.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
            dgvPing.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;

            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dgvPing.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgvPing, true, null);
            }
        }

        private void clearPingPanel()
        {
            bigData.Rows.Clear();
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
            stopPing(false); // stop all ongoing workers

            pingStarted = true;

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
                work.latestLogSizeLimit = appPref_showLowerPanel_limit;

                work.startPing();
            });
        }

        private void stopPing(bool wait)
        {
            mutexStopPing.WaitOne();

            if (workForce != null)
            {
                Parallel.ForEach(workForce, (worker) =>
                {
                    if (wait)
                    {
                        worker.stopPingWait();
                    }
                    else
                    {
                        worker.stopPing();
                    }
                });
            }

            pingStarted = false;

            mutexStopPing.ReleaseMutex();
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

                    row[0] = pingStatusToIcon(lastReply_result);

                    row[5] = worker.lastReply_address;
                    row[6] = worker.upCount;
                    row[7] = worker.downCount;
                    row[8] = worker.consecutiveDownCount;
                    row[9] = worker.maxConsecutiveDownCount;
                    row[10] = tools.formatDateTimeForGridView(worker.maxConsecutiveDownTimestampEnd);
                    row[11] = tools.formatTimeSpanForGridView(worker.maxConsecutiveDownDuration);
                    row[12] = worker.percentDown;
                    row[13] = pingWork.pingStatusToText[lastReply_result];
                    row[14] = tools.formatPingTimeForGridView(worker.lastReply_time, worker.upCount);
                    row[15] = tools.formatPingTimeForGridView(worker.avgPingTime, worker.upCount);
                    row[16] = tools.formatDateTimeForGridView(worker.lastUpTimestamp);
                    row[17] = tools.formatDateTimeForGridView(worker.lastDownTimestamp);
                    row[18] = tools.formatTimeSpanForGridView(worker.lastDownDuration);
                    row[19] = tools.formatPingTimeForGridView(worker.minPingTime, worker.upCount);
                    row[20] = tools.formatPingTimeForGridView(worker.maxPingTime, worker.upCount);

                    row.EndEdit();
                    row.AcceptChanges();
                }
            }
        }

        private Icon pingStatusToIcon(pingWork.pingStatus lastReply_result)
        {
            if (lastReply_result == pingWork.pingStatus.online)
            {
                return icon_ok;
            }
            else if (lastReply_result == pingWork.pingStatus.none)
            {
                return icon_blank;
            }
            else
            {
                return icon_warning;
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

        private void stopNotifyOfflineHost()
        {
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

            appPref_showLowerPanel = Settings.Get("appPref_showLowerPanel", true);
            appPref_showLowerPanel_onlyFailed = Settings.Get("appPref_showLowerPanel_onlyFailed", true);
            appPref_showLowerPanel_limit = Settings.Get("appPref_showLowerPanel_limit", 50);

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

            showHideLowerPanel();
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

            Settings.Set("appPref_showLowerPanel", appPref_showLowerPanel);
            Settings.Set("appPref_showLowerPanel_onlyFailed", appPref_showLowerPanel_onlyFailed);
            Settings.Set("appPref_showLowerPanel_limit", appPref_showLowerPanel_limit);

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
                if ((row.Cells[13].Value.ToString() != pingWork.pingStatusToText[pingWork.pingStatus.online]) && (row.Cells[18].Value.ToString().Length > 0) && (TimeSpan.Parse(row.Cells[18].Value.ToString()).TotalSeconds >= appPref_markHostConsFailThreshold))
                {
                    if (appPref_markHostConsFail)
                    {
                        row.DefaultCellStyle.BackColor = Color.Orange;
                    }
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
                if (row.DefaultCellStyle.BackColor == Color.Orange)
                {
                    string message = string.Format("{0} is offline for {1}", row.Cells[3].Value, row.Cells[18].Value.ToString());
                    //sendTrayNotification(message);
                    mess.Add(message);
                }
            }

            if (mess.Count > 0 && appPref_sendTaskbarNotifications)
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

        private void updateLatestLogSizeLimit()
        {
            if (workForce != null)
            {
                foreach (var worker in workForce)
                {
                    worker.latestLogSizeLimit = appPref_showLowerPanel_limit;
                }
            }
        }

        private void showHideLowerPanel()
        {
            showHideLowerPanel(appPref_showLowerPanel);
        }

        private void showHideLowerPanel(bool show)
        {
            if (!show)
            {
                splitContainer1.Panel2Collapsed = true;
                splitContainer1.Panel2.Hide();
            }
            else
            {
                splitContainer1.Panel2Collapsed = false;
                splitContainer1.Panel2.Show();
            }
        }

        private void updateLowerPanel()
        {
            // update the lower pannel even if it's hidden
            if (dgvPing.SelectedRows.Count > 0)
            {
                var selectedRow = dgvPing.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells[2].Value);
                var worker = workForce[id];

                clearLowerPanel();

                smallData.BeginLoadData();

                string[][] data;

                if (appPref_showLowerPanel_onlyFailed)
                {
                    data = worker.latestLog_down.ToArray();
                } 
                else
                {
                    data = worker.latestLog_all.ToArray();
                }

                for (int i = 0; i < data.Length; i++)
                {
                    string[] line = data[data.Length - i - 1];
                    smallData.Rows.Add();
                    var row = smallData.Rows[i];

                    row[0] = pingStatusToIcon(pingWork.textToPingStatus[line[2]]);
                    row[1] = line[0];
                    row[2] = line[1];
                    row[3] = line[2];
                    row[4] = line[3];
                    row[5] = line[4];
                    row[6] = line[5];
                    row[7] = line[6];
                }

                smallData.EndLoadData();

                showHideLowerPanel();
            } 
            else
            {
                showHideLowerPanel(false);
            }
        }

        private void resetLowerPanel()
        {
            smallData = new DataTable();
            smallBind = new SyncBindingSource();
            smallBind.DataSource = smallData;

            //dgvPing.AutoGenerateColumns = false;
            dgvLowerPanel.DataSource = smallData;
            dgvLowerPanel.DataSource = smallBind;

            smallData.Columns.Add("   ", typeof(Icon)); // 0
            smallData.Columns.Add("Host Name", typeof(string)); // 1
            smallData.Columns.Add("Timestamp", typeof(string)); // 2
            smallData.Columns.Add("Result", typeof(string)); // 3
            smallData.Columns.Add("Round Trip", typeof(string)); // 4
            smallData.Columns.Add("TTL", typeof(string)); // 5
            smallData.Columns.Add("Consecutive Down Count", typeof(string)); // 6
            smallData.Columns.Add("Check Me", typeof(string)); // 7

            dgvLowerPanel.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            dgvLowerPanel.Refresh();

            // from https://10tec.com/articles/why-datagridview-slow.aspx
            dgvLowerPanel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvLowerPanel.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dgvLowerPanel.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgvLowerPanel, true, null);
            }
        }

        private void clearLowerPanel()
        {
            smallData.Rows.Clear();
        }

        private void resetStats()
        {
            if (workForce != null)
            {
                mutexStopPing.WaitOne();
                bool _pingStarted = this.pingStarted;
                mutexStopPing.ReleaseMutex();

                if (_pingStarted)
                {
                    stopPing(true);
                }

                Parallel.ForEach(workForce, (worker) =>
                {
                    worker.resetStat();
                });

                if (_pingStarted)
                {
                    startPingAio();
                }
            }
        }
    }
}
