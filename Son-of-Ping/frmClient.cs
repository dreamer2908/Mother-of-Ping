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
using System.Threading.Tasks;
using System.Windows.Forms;
using Mother_of_Ping_CLI;
using Mother_of_Ping_GUI;

namespace Son_of_Ping
{
    public partial class frmClient : Form
    {
        public frmClient()
        {
            InitializeComponent();
        }

        string csvUrl = "http://localhost:3037/csv";
        string upstreamCsv = string.Empty;

        DataTable bigData;
        SyncBindingSource bind;

        Icon icon_ok = Son_of_Ping.Properties.Resources.icon_ok;
        Icon icon_warning = Son_of_Ping.Properties.Resources.icon_warning;
        Icon icon_blank = Son_of_Ping.Properties.Resources.icon_blank;

        bool appPref_markHostConsFail = true;
        int appPref_markHostConsFailThreshold = 300;
        bool appPref_sendTaskbarNotifications = true;
        bool appPref_sendLineNotifications = true;

        bool appPref_saveHostList = true;
        bool appPref_autoLoadList = false;
        string appPref_autoLoadListFilename = "";
        string defaultHostListPath = Path.Combine(Application.StartupPath.ToString(), "hostlist.csv");

        List<string[]> hostList = new List<string[]>();
        
        NotifyIcon lastNotifyIcon;

        private void frmClient_Load(object sender, EventArgs e)
        {
            cbbShowHide.SelectedIndex = 0;
            resetPingPanel();
            loadSettings();
        }

        private void frmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateStats();
            updateRowColor();
            showHideRow();
            updateStatusBar();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            notifyOfflineHost();
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

        private void loadSettings()
        {
            appPref_saveHostList = Settings.Get("appPref_saveHostList", true);
            appPref_autoLoadList = Settings.Get("appPref_autoLoadList", false);
            appPref_autoLoadListFilename = Settings.Get("appPref_autoLoadListFilename", "hostlist.csv");

            appPref_markHostConsFail = Settings.Get("appPref_markHostConsFail", true);
            appPref_markHostConsFailThreshold = Settings.Get("appPref_markHostConsFailThreshold", 300);
            appPref_sendTaskbarNotifications = Settings.Get("appPref_sendTaskbarNotifications", true);
            appPref_sendLineNotifications = Settings.Get("appPref_sendLineNotifications", false);

            chbSendNotifications.Checked = appPref_sendTaskbarNotifications;
            txtFailToMark.Text = appPref_markHostConsFailThreshold.ToString();

            txtServer.Text = Settings.Get("txtServer.Text", "localhost");

            if (appPref_saveHostList)
            {
                if (File.Exists(defaultHostListPath))
                {
                    loadHostWhiteList(defaultHostListPath);
                }
            }
            else if (appPref_autoLoadList)
            {
                if (File.Exists(appPref_autoLoadListFilename))
                {
                    loadHostWhiteList(appPref_autoLoadListFilename);
                }
            }
        }

        private void saveSettings()
        {
            Settings.Set("appPref_saveHostList", appPref_saveHostList.ToString());
            Settings.Set("appPref_autoLoadList", appPref_autoLoadList.ToString());
            Settings.Set("appPref_autoLoadListFilename", appPref_autoLoadListFilename);

            Settings.Set("appPref_markHostConsFail", appPref_markHostConsFail.ToString());
            Settings.Set("appPref_markHostConsFailThreshold", appPref_markHostConsFailThreshold.ToString());
            Settings.Set("appPref_sendTaskbarNotifications", appPref_sendTaskbarNotifications.ToString());
            Settings.Set("appPref_sendLineNotifications", appPref_sendLineNotifications.ToString());

            Settings.Set("txtServer.Text", txtServer.Text);

            // save host list
            if (appPref_saveHostList)
            {
                // convert to ConcurrentQueue<string[]> then write down
                ConcurrentQueue<string[]> tmp = new ConcurrentQueue<string[]>(hostList);
                tools.writeCsv_ConcurrentQueue(tmp, defaultHostListPath, true);
            }
        }

        private void loadHostWhiteList(string filename)
        {
            if (filename.ToLower().EndsWith(".csv"))
            {
                hostList = tools.csvHostListParser(filename, false);
            }
            else
            {
                hostList = tools.txtPinginfoviewParser(filename, false);
            }
        }

        private void stopGridUpdate()
        {
            timer1.Stop();
            timer3.Stop();
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

        private bool isHostInWhiteList(string hostname)
        {
            // return true if the host list is empty
            if (hostList.Count == 0)
            {
                return true;
            }

            // otherwise, check if hostname in the host list
            foreach (var white in hostList)
            {
                if (white[0] == hostname)
                {
                    return true;
                }
            }

            return false;
        }

        private void updateStats()
        {
            upstreamCsv = tools.readTextFromUrl(csvUrl);

            List<string[]> csvRows = tools.csvParser(upstreamCsv, true, 18);

            // create enough rows to store data
            while (bigData.Rows.Count < csvRows.Count)
            {
                bigData.Rows.Add();
            }

            bigData.BeginLoadData();

            int rowAdded = 0;

            for (int i = 0; i < csvRows.Count; i++)
            {
                var csvRow = csvRows[i];

                string hostname = csvRow[0];

                if (isHostInWhiteList(hostname))
                {
                    var row = bigData.Rows[rowAdded];

                    row[1] = true;
                    row[2] = i;
                    row[3] = csvRow[0];
                    row[4] = csvRow[1];
                    row[5] = csvRow[2];
                    row[6] = int.Parse(csvRow[3]);
                    row[7] = int.Parse(csvRow[4]);
                    row[8] = int.Parse(csvRow[5]);
                    row[9] = int.Parse(csvRow[6]);
                    row[10] = csvRow[7];
                    row[11] = csvRow[8];
                    row[12] = csvRow[9];
                    row[13] = csvRow[10];
                    row[14] = csvRow[11];
                    row[15] = csvRow[12];
                    row[16] = csvRow[13];
                    row[17] = csvRow[14];
                    row[18] = csvRow[15];
                    row[19] = csvRow[16];
                    row[20] = csvRow[17];
                    row[0] = pingStatusToIcon(pingWork.textToPingStatus[csvRow[10]]);

                    rowAdded++;
                }
            }

            // and remove excessive ones
            while (bigData.Rows.Count > rowAdded)
            {
                bigData.Rows.RemoveAt(bigData.Rows.Count - 1);
            }

            bigData.EndLoadData();
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
                if (row.DefaultCellStyle.BackColor == Color.Orange)
                {
                    string message = string.Format("{0} is offline for {1}", row.Cells[3].Value, row.Cells[18].Value.ToString());
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

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text.StartsWith("Connect"))
            {
                csvUrl = "http://" + txtServer.Text + ":3037/csv";
                timer1.Start();
                timer3.Start();
                btnConnect.Text = "Disconnect";
            }
            else
            {
                timer1.Stop();
                timer3.Stop();
                btnConnect.Text = "Connect";
            }
        }

        private void txtFailToMark_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtFailToMark.Text, out int n))
            {
                appPref_markHostConsFailThreshold = n;
            }
            else
            {
                txtFailToMark.Text = appPref_markHostConsFailThreshold.ToString();
            }
        }

        private void chbSendNotifications_CheckedChanged(object sender, EventArgs e)
        {
            appPref_sendTaskbarNotifications = chbSendNotifications.Checked;
        }

        private void btnLoadList_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;

                loadHostWhiteList(filename);
            }
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            hostList.Clear();
        }
    }
}
