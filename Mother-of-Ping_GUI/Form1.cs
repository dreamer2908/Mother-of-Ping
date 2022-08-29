using Mother_of_Ping_CLI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
        string appPref_globalLogFilename = "";
        string appPref_logFolder = "";
        bool appPref_useTodayFolder = true;
        int appPref_flushLogPeriod = 600;
        string actualLogFolder = "";

        bool appPref_generateReportAtExit = true;

        bool appPref_showLowerPanel = true;
        bool appPref_showLowerPanel_onlyFailed = false;
        int appPref_showLowerPanel_limit = 50;
        int lastSelectedHost = 0;

        Mutex mutexLogFlush = new Mutex();
        Mutex mutexStopPing = new Mutex();

        bool pingStarted = false;

        bool appPref_schedulerEnable = false;
        bool appPref_schedulerEnable_start = false;
        bool appPref_schedulerEnable_stop = false;
        bool appPref_schedulerEnable_report = false;
        bool appPref_schedulerEnable_reset = false;

        string appPref_schedulerTime_start = "7:29";
        string appPref_schedulerTime_stop = "16:29";
        string appPref_schedulerTime_report = "16:30";
        string appPref_schedulerTime_reset = "7:28";

        readonly string scheduler_timeFormat = "HH:mm";
        string scheduler_lastTime = string.Empty;

        NotifyIcon lastNotifyIcon;

        HttpServer Server1;
        HttpServer Server2;

        bool appPref_httpServer_enable = false;

        bool appPref_ignoreWriteFailure = true;

        bool email_enable = false;
        string email_host = "";
        int email_port = 25;
        bool email_ssl = false;
        string email_from = "";
        string email_user = "";
        bool email_login = true;
        string email_password = "";
        List<string> email_to = new List<string>();
        string email_subject = "";
        double delayBetweenEmails = 900; // seconds

        DateTime lastEmailTimestamp = DateTime.MinValue;

        #region email

        public static string getNowStringForFilename()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public static string datetimeCommonFormatString = "yyyy-MM-dd HH:mm:ss";
        public static string getNowString()
        {
            return formatDateTime(DateTime.Now);
        }

        public static string formatDateTime(DateTime d)
        {
            return d.ToString(datetimeCommonFormatString);
        }

        public static DateTime parseDateTime(string s)
        {
            try
            {
                return DateTime.ParseExact(s, datetimeCommonFormatString, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime parseDateTime(string s, TimeSpan maxDistanceFromNow)
        {
            var d = parseDateTime(s);
            var now = DateTime.Now;
            if (now - d > maxDistanceFromNow)
            {
                d = now.Subtract(maxDistanceFromNow);
            }
            else if (d - now > maxDistanceFromNow)
            {
                d = now.Add(maxDistanceFromNow);
            }
            return d;
        }

        public static string formatTimeSpan(TimeSpan s)
        {
            if (s.TotalDays >= 1)
            {
                return s.ToString(@"dd\.hh\:mm\:ss");
            }
            else if (s.TotalHours >= 1)
            {
                return s.ToString(@"hh\:mm\:ss");
            }
            else
            {
                return s.ToString(@"hh\:mm\:ss");
            }
        }

        private string convertTextToHtml(string input)
        {
            string[] lines = splitLines(input);

            StringBuilder sb = new StringBuilder();

            foreach (string text in lines)
            {
                string encoded = System.Net.WebUtility.HtmlEncode(text);
                sb.AppendLine("<pre>" + encoded + "</pre>");
            }

            return sb.ToString();
        }

        private string[] splitLines(string text)
        {
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return lines;
        }

        private static string[] customSplitLines(string text)
        {
            List<string> result = new List<string>();

            string empty = " "; // workaround for Outlook ignoring totally empty line

            string thisLine = empty;
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '\n')
                {
                    result.Add(thisLine);
                    thisLine = empty;
                    i++;
                }
                else if (text[i] == '\r')
                {
                    result.Add(thisLine);
                    thisLine = empty;

                    if (text[i + 1] == '\n')
                    {
                        i += 2;
                    }
                    else
                    {
                        i += 1;
                    }
                }
                else
                {
                    thisLine = thisLine + text[i].ToString();
                    i++;
                }
            }

            return result.ToArray();
        }

        // single receipent
        private void sendEmail(string email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, email_to, email_subject, email_body, attachments);
        }

        // multiple receipents
        private void sendEmail(List<string> email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, email_to, email_subject, email_body, attachments);
        }

        // single receipent
        private void sendEmail(string email_host, int email_port, bool email_ssl, string email_from, string email_user, string email_password, string email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            List<string> to = new List<string>();
            to.Add(email_to);
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, to, email_subject, email_body, attachments);
        }

        // multiple receipents
        private void sendEmail(string email_host, int email_port, bool email_ssl, string email_from, string email_user, string email_password, List<string> email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            using (SmtpClient SmtpServer = new SmtpClient(email_host))
            {
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        mail.From = new MailAddress(email_from);
                        foreach (string em in email_to)
                        {
                            mail.To.Add(em);
                        }
                        mail.Subject = email_subject;
                        mail.IsBodyHtml = true;
                        mail.Body = convertTextToHtml(email_body);

                        // attach files
                        if (attachments != null)
                        {
                            foreach (var filename in attachments)
                            {
                                mail.Attachments.Add(new Attachment(filename));
                                // MessageBox.Show("Added attachment to email");
                            }
                        }

                        SmtpServer.Port = email_port;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(email_user, email_password);
                        SmtpServer.EnableSsl = email_ssl;

                        SmtpServer.Send(mail);

                        // MessageBox.Show("mail Send");
                        Console.WriteLine("mail Send");
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.ToString());
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private static string emailHeadline = "*** This is a system generated email, do not reply to this email id ***\n \n";
        private static string eventSeparator = "\n \n####################################################################\n \n";

        public void sendEmailAlert(bool ignoreDelay = false, bool ignoreAlertStatus = false, List<string> custom_to = null)
        {
            // only send email if more than delayBetweenEmails seconds has passed since last email
            // or when delay is asked to be ignored
            double secondSinceLastEmail = (DateTime.Now - lastEmailTimestamp).TotalSeconds;

            // only send email if there're at least one orange row
            // or alert status is asked to be ignored
            int orangeCount = 0;
            foreach (DataGridViewRow row in dgvPing.Rows)
            {
                bool thisRowNoticeEnabled = (bool)row.Cells[1].Value;
                if (row.DefaultCellStyle.BackColor == Color.Orange && thisRowNoticeEnabled)
                {
                    orangeCount++;
                }
            }

            if ((ignoreAlertStatus || orangeCount > 0) && (ignoreDelay || secondSinceLastEmail >= delayBetweenEmails))
            {
                string email_body = emailHeadline + "At system time: " + getNowString() + "\n" + lblStatusBar.Text + "\n ";

                foreach (DataGridViewRow row in dgvPing.Rows)
                {
                    bool thisRowNoticeEnabled = (bool)row.Cells[1].Value;
                    bool online = row.Cells[0].Value == icon_ok || row.Cells[0].Value == icon_blank;
                    if (!online && thisRowNoticeEnabled)
                    {
                        string message = string.Format("{0} {2} is offline for {1}", row.Cells[3].Value, row.Cells[18].Value.ToString(), row.Cells[4].Value);
                        email_body += "\n" + message;
                    }
                }

                lastEmailTimestamp = DateTime.Now;
                if (custom_to != null)
                {
                    sendEmail(custom_to, email_subject, email_body);
                }
                else
                {
                    sendEmail(email_to, email_subject, email_body);
                }
            }
        }
        #endregion

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
                startPingGui();
            }
            else
            {
                MessageBox.Show("There's nothing to run.", "Start", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopPingGui();
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

            stopHttpServer();

            if (appPref_generateReportAtExit && workForce != null)
            {
                generateReportAutoMode();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbbShowHide.SelectedIndex = 0;

            resetPingPanel();
            resetLowerPanel();

            loadSettings();
            if (appPref_autoStart)
            {
                startPingAio();
            }

            if (appPref_httpServer_enable)
            {
                startHttpServer();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateStats();
            updateRowColor();
            showHideRow();
            updateLowerPanel();
            updateStatusBar();

            if (Server1 != null && Server2 != null)
            {
                Server1.responseString = tools.generateCsvReport(workForce);
                Server2.responseString = tools.generateHtmlReport(workForce);
            }
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

                appPref_schedulerEnable = appPref_schedulerEnable,
                appPref_schedulerEnable_start = appPref_schedulerEnable_start,
                appPref_schedulerEnable_stop = appPref_schedulerEnable_stop,
                appPref_schedulerEnable_report = appPref_schedulerEnable_report,
                appPref_schedulerEnable_reset = appPref_schedulerEnable_reset,

                appPref_schedulerTime_start = appPref_schedulerTime_start,
                appPref_schedulerTime_stop = appPref_schedulerTime_stop,
                appPref_schedulerTime_report = appPref_schedulerTime_report,
                appPref_schedulerTime_reset = appPref_schedulerTime_reset,

                appPref_httpServer_enable = appPref_httpServer_enable,

                appPref_ignoreWriteFailure = appPref_ignoreWriteFailure,

                email_host = email_host,
                email_port = email_port,
                email_ssl = email_ssl,
                email_from = email_from,
                email_user = email_user,
                email_login = email_login,
                email_password = email_password,
                email_subject = email_subject,
                delayBetweenEmails = delayBetweenEmails,
                email_to = email_to,
                email_enable = email_enable,
                form1 = this,
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

                appPref_schedulerEnable = options.appPref_schedulerEnable;
                appPref_schedulerEnable_start = options.appPref_schedulerEnable_start;
                appPref_schedulerEnable_stop = options.appPref_schedulerEnable_stop;
                appPref_schedulerEnable_report = options.appPref_schedulerEnable_report;
                appPref_schedulerEnable_reset = options.appPref_schedulerEnable_reset;

                appPref_schedulerTime_start = options.appPref_schedulerTime_start;
                appPref_schedulerTime_stop = options.appPref_schedulerTime_stop;
                appPref_schedulerTime_report = options.appPref_schedulerTime_report;
                appPref_schedulerTime_reset = options.appPref_schedulerTime_reset;

                if (appPref_schedulerEnable)
                {
                    timer4.Start();
                }
                else
                {
                    timer4.Stop();
                }

                appPref_showLowerPanel_limit = options.appPref_showLowerPanel_limit;
                updateLatestLogSizeLimit();

                showHideLowerPanel();

                appPref_httpServer_enable = options.appPref_httpServer_enable;

                appPref_ignoreWriteFailure = options.appPref_ignoreWriteFailure;

                email_host = options.email_host;
                email_port = options.email_port;
                email_ssl = options.email_ssl;
                email_from = options.email_from;
                email_user = options.email_user;
                email_login = options.email_login;
                email_password = options.email_password;
                email_subject = options.email_subject;
                delayBetweenEmails = options.delayBetweenEmails;
                email_to = options.email_to;
                email_enable = options.email_enable;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            flushLogToDisk();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            notifyOfflineHost();
            sendEmailAlert();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            resetStatsGui();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            schedulerLoop();
        }

        private void dgvPing_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvPing.Columns[1].Index && e.RowIndex != -1)
            {
                // Handle checkbox state change here
                // Actually there's no need to do anything here.
                // MessageBox.Show("e.ColumnIndex = " + e.ColumnIndex.ToString() + "\ne.RowIndex = " + e.RowIndex);
            }
        }

        private void dgvPing_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (e.ColumnIndex == dgvPing.Columns[1].Index && e.RowIndex != -1)
            {
                dgvPing.EndEdit();
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

        private void stopPingAio()
        {
            stopPing(false);
            stopLogFlushing();
            stopNotifyOfflineHost();
            flushLogToDisk();
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
            updateStatusBar();
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
            mutexStopPing.WaitOne();

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

            mutexStopPing.ReleaseMutex();
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

        private string[] separatorComma = new string[] { "," };
        private void splitMultivalueSettingStringToList(string source, string[] separator, List<string> target)
        {
            target.Clear();
            string[] array = source.Split(separator, StringSplitOptions.None);
            for (int i = 0; i < array.Length; i++)
            {
                string sub = array[i].Trim();
                if (sub.Length > 0)
                {
                    target.Add(sub);
                }
            }
        }

        private void loadSettings()
        {
            pingPref_period = Settings.Get("pingPref_period", 1000);
            pingPref_timeout = Settings.Get("pingPref_timeout", 5000);
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

            appPref_saveGlobalLog = Settings.Get("appPref_saveGlobalLog", false);
            appPref_saveIndividualLog = Settings.Get("appPref_saveIndividualLog", false);
            appPref_globalLogFilename = Settings.Get("appPref_globalLogFilename", "");
            appPref_logFolder = Settings.Get("appPref_logFolder", "");
            appPref_useTodayFolder = Settings.Get("appPref_useTodayFolder", true);
            appPref_flushLogPeriod = Settings.Get("appPref_flushLogPeriod", 600);

            appPref_showLowerPanel = Settings.Get("appPref_showLowerPanel", true);
            appPref_showLowerPanel_onlyFailed = Settings.Get("appPref_showLowerPanel_onlyFailed", false);
            appPref_showLowerPanel_limit = Settings.Get("appPref_showLowerPanel_limit", 50);

            appPref_generateReportAtExit = Settings.Get("appPref_generateReportAtExit", true);

            appPref_schedulerEnable = Settings.Get("appPref_schedulerEnable", false);
            appPref_schedulerEnable_start = Settings.Get("appPref_schedulerEnable_start", true);
            appPref_schedulerEnable_stop = Settings.Get("appPref_schedulerEnable_stop", true);
            appPref_schedulerEnable_report = Settings.Get("appPref_schedulerEnable_report", true);
            appPref_schedulerEnable_reset = Settings.Get("appPref_schedulerEnable_reset", true);

            appPref_schedulerTime_start = Settings.Get("appPref_schedulerTime_start", "07:29");
            appPref_schedulerTime_stop = Settings.Get("appPref_schedulerTime_stop", "16:29");
            appPref_schedulerTime_report = Settings.Get("appPref_schedulerTime_report", "16:30");
            appPref_schedulerTime_reset = Settings.Get("appPref_schedulerTime_reset", "07:28");

            appPref_httpServer_enable = Settings.Get("appPref_httpServer_enable", false);

            appPref_ignoreWriteFailure = Settings.Get("appPref_ignoreWriteFailure", true);

            email_enable = Settings.Get("email_enable", false);
            email_host = Settings.Get("email_host", "");
            email_port = Settings.Get("email_port", 25);
            email_ssl = Settings.Get("email_ssl", false);
            email_from = Settings.Get("email_from", "");
            email_user = Settings.Get("email_user", "");
            email_login = Settings.Get("email_login", true);
            string encryptedPassword = Settings.Get("email_password", "");
            email_password = tools.decryptPassword(encryptedPassword);
            email_subject = Settings.Get("email_subject", "");
            delayBetweenEmails = Settings.Get("delayBetweenEmails", 900);
            string email_tos = Settings.Get("email_to", "");
            splitMultivalueSettingStringToList(email_tos, separatorComma, email_to);

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

            if (appPref_schedulerEnable)
            {
                timer4.Start();
            }
            else
            {
                timer4.Stop();
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

            Settings.Set("appPref_showLowerPanel", appPref_showLowerPanel);
            Settings.Set("appPref_showLowerPanel_onlyFailed", appPref_showLowerPanel_onlyFailed);
            Settings.Set("appPref_showLowerPanel_limit", appPref_showLowerPanel_limit);

            Settings.Set("appPref_schedulerEnable", appPref_schedulerEnable);
            Settings.Set("appPref_schedulerEnable_start", appPref_schedulerEnable_start);
            Settings.Set("appPref_schedulerEnable_stop", appPref_schedulerEnable_stop);
            Settings.Set("appPref_schedulerEnable_report", appPref_schedulerEnable_report);
            Settings.Set("appPref_schedulerEnable_reset", appPref_schedulerEnable_reset);

            Settings.Set("appPref_schedulerTime_start", appPref_schedulerTime_start);
            Settings.Set("appPref_schedulerTime_stop", appPref_schedulerTime_stop);
            Settings.Set("appPref_schedulerTime_report", appPref_schedulerTime_report);
            Settings.Set("appPref_schedulerTime_reset", appPref_schedulerTime_reset);

            Settings.Set("appPref_httpServer_enable", appPref_httpServer_enable);

            Settings.Set("appPref_ignoreWriteFailure", appPref_ignoreWriteFailure);

            Settings.Set("email_host", email_host);
            Settings.Set("email_enable", email_enable);
            Settings.Set("email_port", email_port);
            Settings.Set("email_ssl", email_ssl);
            Settings.Set("email_from", email_from);
            Settings.Set("email_user", email_user);
            Settings.Set("email_login", email_login);
            string encryptedPassword = tools.encryptPassword(email_password);
            Settings.Set("email_password", encryptedPassword);
            Settings.Set("email_subject", email_subject);
            Settings.Set("delayBetweenEmails", (int)delayBetweenEmails);
            Settings.Set("email_to", string.Join(separatorComma[0], email_to));

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

        private void showHideRow()
        {
            int showHideMode = cbbShowHide.SelectedIndex;
            if (showHideMode < 0) return;

            // you need to suspend data binding before show/hide rows, otherwise it will throw exception
            bind.SuspendBinding();

            foreach (DataGridViewRow row in dgvPing.Rows)
            {
                bool online = row.Cells[0].Value == icon_ok || row.Cells[0].Value == icon_blank;
                bool orange = row.DefaultCellStyle.BackColor == Color.Orange;

                bool show = (showHideMode == 0) || (showHideMode == 1 && online) || (showHideMode == 2 && !online) || (showHideMode == 3 && orange);

                if (show)
                {
                    row.Visible = true;
                }
                else
                {
                    row.Selected = false; // note that selected row can't be hidden
                    row.Visible = false;
                }
            }

            bind.ResumeBinding();
        }

        private void notifyOfflineHost()
        {
            List<string> mess = new List<string>();
            foreach (DataGridViewRow row in dgvPing.Rows)
            {
                bool thisRowNoticeEnabled = (bool)row.Cells[1].Value;
                if (row.DefaultCellStyle.BackColor == Color.Orange && thisRowNoticeEnabled)
                {
                    string message = string.Format("{0} {2} is offline for {1}", row.Cells[3].Value, row.Cells[18].Value.ToString(), row.Cells[4].Value);
                    //sendTrayNotification(message);
                    mess.Add(message);
                }
            }

            if (mess.Count > 0 && appPref_sendTaskbarNotifications)
            {
                // remove the old notification. in some cases, they won't go away automatically and overflow the notification area
                if (lastNotifyIcon != null)
                {
                    lastNotifyIcon.Dispose();
                }

                string message = String.Join("\n", mess);
                lastNotifyIcon = sendTrayNotification(message, 5000);
            }
        }

        private NotifyIcon sendTrayNotification(string message, int duration)
        {
            var notification = new NotifyIcon()
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

            return notification;
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
                string today = tools.getTodayString();
                actualLogFolder = Path.Combine(appPref_logFolder, today);
                Directory.CreateDirectory(actualLogFolder);
            }

            if (appPref_saveGlobalLog)
            {
                string globalFilePath = (appPref_globalLogFilename.Length < 1) ? "0.0.0.0.csv" : appPref_globalLogFilename;

                // put global log in actualLogFolder if globalFilePath contains only filename
                // otherwise keep it in globalFilePath
                string filename = Path.GetFileName(globalFilePath);
                if (filename == globalFilePath)
                {
                    globalFilePath = Path.Combine(actualLogFolder, filename);
                }

                if (!pingWork.globalLog.IsEmpty)
                {
                    if (!appPref_ignoreWriteFailure)
                    {
                        tools.writeCsv_ConcurrentQueue(pingWork.globalLog, globalFilePath, false);
                    }
                    else
                    {
                        tools.writeCsv_ConcurrentQueue(pingWork.globalLog, globalFilePath, false, 10, 1000);
                    }
                }
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

                    if (!worker.log.IsEmpty)
                    {
                        if (!appPref_ignoreWriteFailure)
                        {
                            tools.writeCsv_ConcurrentQueue(worker.log, filePath, false);
                        }
                        else
                        {
                            tools.writeCsv_ConcurrentQueue(worker.log, filePath, false, 5, 500);
                        }
                    }
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

                string[][] data;
                var worker = workForce[id];

                if (appPref_showLowerPanel_onlyFailed)
                {
                    data = worker.latestLog_down.ToArray();
                } 
                else
                {
                    data = worker.latestLog_all.ToArray();
                }

                // only clear the panel if a different host is selected
                // actually, you don't need to clear it even in that case
                // if (lastSelectedHost != id) clearLowerPanel();

                smallData.BeginLoadData();

                // create enough rows to store data
                while (smallData.Rows.Count < data.Length)
                {
                    smallData.Rows.Add();
                }
                // and remove excessive ones
                while (smallData.Rows.Count > data.Length)
                {
                    smallData.Rows.RemoveAt(0);
                }

                for (int i = 0; i < data.Length; i++)
                {
                    string[] line = data[data.Length - i - 1];
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

                lastSelectedHost = id;
            } 
            else
            {
                clearLowerPanel();
                // showHideLowerPanel(false);
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

        private void generateReportAutoMode()
        {
            if (workForce != null)
            {
                string today = tools.getNowStringForFilename();
                string filename1 = Path.Combine(actualLogFolder, string.Format("ping_session_report_{0}.csv", today));
                string filename2 = Path.Combine(actualLogFolder, string.Format("ping_session_report_{0}.html", today));
                tools.generateCsvReport(workForce, filename1);
                tools.generateHtmlReport(workForce, filename2);
            }
        }

        private void schedulerLoop()
        {
            if (!appPref_schedulerEnable) return;

            string now = DateTime.Now.ToString(scheduler_timeFormat);
            if (now == scheduler_lastTime) return; // skip this loop if it's still the same minute
            scheduler_lastTime = now;

            if (appPref_schedulerEnable_start)
            {
                if (now == appPref_schedulerTime_start)
                {
                    //MessageBox.Show("scheduler running start");
                    startPingGui();
                }
            }

            if (appPref_schedulerEnable_stop)
            {
                if (now == appPref_schedulerTime_stop)
                {
                    //MessageBox.Show("scheduler running stop");
                    stopPingGui();
                }
            }

            if (appPref_schedulerEnable_report)
            {
                if (now == appPref_schedulerTime_report)
                {
                    //MessageBox.Show("scheduler running report");
                    generateReportAutoMode();
                }
            }

            if (appPref_schedulerEnable_reset)
            {
                if (now == appPref_schedulerTime_reset)
                {
                    //MessageBox.Show("scheduler running reset");
                    resetStatsGui();
                }
            }
        }

        private void updateStatusBar()
        {
            int status_total = 0;
            int status_online = 0;
            int status_offline = 0;
            int status_orange = 0;

            foreach (DataGridViewRow row in dgvPing.Rows)
            {
                status_total++;

                bool online = row.Cells[0].Value == icon_ok;
                bool offline = row.Cells[0].Value == icon_warning;
                bool orange = row.DefaultCellStyle.BackColor == Color.Orange;

                if (online) status_online++;
                if (offline) status_offline++;
                if (orange) status_orange++;
            }

            lblStatusBar.Text = string.Format("Total: {0}. Online: {1}. Offline {2}. Long-term offline: {3}.", status_total, status_online, status_offline, status_orange);
        }

        private void startHttpServer()
        {
            stopHttpServer();

            string uri1 = "http://+:3037/csv/";
            string uri2 = "http://+:3037/";

            Server1 = new HttpServer();
            Server2 = new HttpServer();

            try
            {
                Server1.Start(uri1);
                Server2.Start(uri2);
            }
            catch (System.Net.HttpListenerException)
            {
                // to bind to all network interfaces, you need use netsh to whitelist address, for a specific user
                // note: netsh requires elevation
                // you may also run the whole app in elevated mode (run as administrator); it won't need urlacl then
                // another choice is getting all interface addresses and bind them; urlacl is also not required in this case

                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                // MessageBox.Show(userName);

                tools.executeAsAdmin("netsh.exe", @"http add urlacl url=" + uri1 + " user=" + userName, true, true);
                tools.executeAsAdmin("netsh.exe", @"http add urlacl url=" + uri2 + " user=" + userName, true, true);
                // Process.Start("netsh.exe", @"http add urlacl url=" + uri1 + " user=" + userName).WaitForExit();
                // Process.Start("netsh.exe", @"http add urlacl url=" + uri2 + " user=" + userName).WaitForExit();

                // Thread.Sleep(1000);

                try
                {
                    stopHttpServer();
                    Server1.Start(uri1);
                    Server2.Start(uri2);
                }
                catch {}
            }
        }

        private void stopHttpServer()
        {
            if (Server1 != null) Server1.Stop();
            if (Server2 != null) Server2.Stop();
        }

        private void enableStartStopResetButton()
        {
            buttonEnableDisable(btnStart, true);
            buttonEnableDisable(btnStop, true);
            buttonEnableDisable(btnReset, true);
        }

        private void disableStartStopResetButton()
        {
            buttonEnableDisable(btnStart, false);
            buttonEnableDisable(btnStop, false);
            buttonEnableDisable(btnReset, false);
        }

        private void buttonEnableDisable(Button b, bool enabled)
        {
            if (b.InvokeRequired)
            {
                // It's on a different thread, so use Invoke.
                b.BeginInvoke(new MethodInvoker(() =>
                {
                    b.Enabled = enabled;
                }));
            }
            else
            {
                // It's on the same thread, no need for Invoke
                b.Enabled = enabled;
            }
        }

        private void buttonEditText(Button b, string text)
        {
            if (b.InvokeRequired)
            {
                // It's on a different thread, so use Invoke.
                b.BeginInvoke(new MethodInvoker(() =>
                {
                    b.Text = text;
                }));
            }
            else
            {
                // It's on the same thread, no need for Invoke
                b.Text = text;
            }
        }

        private void startPingGui()
        {
            disableStartStopResetButton();
            buttonEditText(btnStart, "Starting...");

            startPingAio();

            buttonEditText(btnStart, "Start");
            enableStartStopResetButton();
        }

        private void stopPingGui()
        {
            disableStartStopResetButton();
            buttonEditText(btnStop, "Stopping...");

            stopPingAio();

            buttonEditText(btnStop, "Stop");
            enableStartStopResetButton();
        }

        private void resetStatsGui()
        {
            disableStartStopResetButton();
            buttonEditText(btnReset, "Resetting...");

            resetStats();

            buttonEditText(btnReset, "Reset");
            enableStartStopResetButton();
        }
    }
}
