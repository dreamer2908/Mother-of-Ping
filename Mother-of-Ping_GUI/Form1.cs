using Mother_of_Ping_CLI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Mother_of_Ping_GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            resetTable();
        }

        List<struct_bigData> bigData;
        BindingList<struct_bigData> bindingList;
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

        #region events

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string filename = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;

                if (filename.ToLower().EndsWith(".csv"))
                {
                    hostList = tools.csvHostListParser(filename, false);
                }
                else
                {
                    hostList = tools.txtPinginfoviewParser(filename, false);
                }

                resetTable();
                loadHostListToTable(hostList);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            startPing();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopPing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //updateStats();
            //bind.SuspendBinding();
            backgroundWorker1.RunWorkerAsync();
            //timer1.Interval = 1000; // in miliseconds
            //timer1.Start();
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
        }
        #endregion

        struct struct_bigData
        {
            public Icon _00_icon { get; set; }
            public bool _01_notice { get; set; }
            public int _02_no { get; set; }
            public string _03_hostname { get; set; }
            public string _04_description { get; set; }
            public string _05_replyIpAddr { get; set; }
            public int _06_upCount { get; set; }
            public int _07_downCount { get; set; }
            public int _08_consecutiveDownCount { get; set; }
            public int _09_maxConsecutiveDownCount { get; set; }
            public string _10_maxConsecutiveDownTimestamp { get; set; }
            public string _11_percentDown { get; set; }
            public string _12_lastReply_result { get; set; }
            public int _13_lastReply_time { get; set; }
            public string _14_avg_PingTime { get; set; }
            public string _15_lastUpTimestamp { get; set; }
            public string _16_lastDownTimestamp { get; set; }
            public int _17_minPingTime { get; set; }
            public int _18_maxPingTime { get; set; }
        }

        private void resetTable()
        {
            bigData = new List<struct_bigData>();
            bindingList = new BindingList<struct_bigData>(bigData);
            bind = new SyncBindingSource();
            bind.DataSource = bindingList;

            // dgvPing.AutoGenerateColumns = false;
            // dgvPing.DataSource = bigData;

            //dgvPing.Columns.Clear();

            //dgvPing.Columns.Add("_00_icon", " ");
            //dgvPing.Columns.Add("_01_notice", "Notice");
            //dgvPing.Columns.Add("_02_no", "No.");
            //dgvPing.Columns.Add("_03_hostname", "Host Name");
            //dgvPing.Columns.Add("_04_description", "Description");
            //dgvPing.Columns.Add("_05_replyIpAddr", "Reply IP Address");
            //dgvPing.Columns.Add("_06_upCount", "Succeed Count");
            //dgvPing.Columns.Add("_07_downCount", "Failed Count");
            //dgvPing.Columns.Add("_08_consecutiveDownCount", "Consecutive Failed Count");
            //dgvPing.Columns.Add("_09_maxConsecutiveDownCount", "Max Consecutive Failed Count");
            //dgvPing.Columns.Add("_10_maxConsecutiveDownTimestamp", "Max Consecutive Failed Time");
            //dgvPing.Columns.Add("_11_percentDown", "% Failed");
            //dgvPing.Columns.Add("_12_lastReply_result", "Last Ping Status");
            //dgvPing.Columns.Add("_13_lastReply_time", "Last Ping Time");
            //dgvPing.Columns.Add("_14_avg_PingTime", "Average Ping Time");
            //dgvPing.Columns.Add("_15_lastUpTimestamp", "Last Succeed On");
            //dgvPing.Columns.Add("_16_lastDownTimestamp", "Last Failed On");
            //dgvPing.Columns.Add("_17_minPingTime", "Minimum Ping Time");
            //dgvPing.Columns.Add("_18_maxPingTime", "Maximum Ping Time");

            dgvPing.DataSource = bind;

            //dgvPing.Columns.Add(" ", typeof(Icon)); // 0
            //dgvPing.Columns.Add("Notice", typeof(bool)); // 1
            //dgvPing.Columns.Add("No.", typeof(int)); // 2
            //dgvPing.Columns.Add("Host Name", typeof(string)); // 3
            //dgvPing.Columns.Add("Description", typeof(string)); // 4
            //dgvPing.Columns.Add("Reply IP Address", typeof(string)); // 5
            //dgvPing.Columns.Add("Succeed Count", typeof(int)); // 6
            //dgvPing.Columns.Add("Failed Count", typeof(int)); // 7
            //dgvPing.Columns.Add("Consecutive Failed Count", typeof(int)); // 8
            //dgvPing.Columns.Add("Max Consecutive Failed Count", typeof(int)); // 9
            //dgvPing.Columns.Add("Max Consecutive Failed Time", typeof(string)); // 10
            //dgvPing.Columns.Add("% Failed", typeof(string)); // 11
            //dgvPing.Columns.Add("Last Ping Status", typeof(string)); // 12
            //dgvPing.Columns.Add("Last Ping Time", typeof(int)); // 13
            //dgvPing.Columns.Add("Average Ping Time", typeof(string)); // 14
            //dgvPing.Columns.Add("Last Succeed On", typeof(string)); // 15
            //dgvPing.Columns.Add("Last Failed On", typeof(string)); // 16
            //dgvPing.Columns.Add("Minimum Ping Time", typeof(int)); // 17
            //dgvPing.Columns.Add("Maximum Ping Time", typeof(int)); // 18

            dgvPing.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dgvPing.Refresh();
        }

        private void loadHostListToTable(List<string[]> hostList)
        {
            int numOfHost = hostList.Count;

            for (int i = 0; i < numOfHost; i++)
            {
                struct_bigData row = new struct_bigData
                {
                    _00_icon = icon_blank,
                    _01_notice = true,
                    _02_no = i,
                    _03_hostname = hostList[i][0],
                    _04_description = hostList[i][1],
                    _05_replyIpAddr = "",
                    _06_upCount = 0,
                    _07_downCount = 0,
                    _08_consecutiveDownCount = 0,
                    _09_maxConsecutiveDownCount = 0,
                    _10_maxConsecutiveDownTimestamp = string.Empty,
                    _11_percentDown = string.Empty,
                    _12_lastReply_result = string.Empty,
                    _13_lastReply_time = 0,
                    _14_avg_PingTime = string.Empty,
                    _15_lastUpTimestamp = string.Empty,
                    _16_lastDownTimestamp = string.Empty,
                    _17_minPingTime = 0,
                    _18_maxPingTime = 0
                };

                bigData.Add(row);
                // bigData.Rows.Add(icon_blank, true, i, hostList[i][0], hostList[i][1]);
            }

            bind.ResetBindings(true);
        }

        private void startPing()
        {
            stopPing(); // stop all ongoing workers

            if (workForce == null)
            {
                workForce = new pingWork[hostList.Count];
            }

            for (int i = 0; i < hostList.Count; i++)
            {
                pingWork work = new pingWork();
                workForce[i] = work;

                work.id = i;
                work.hostname = hostList[i][0];
                work.description = hostList[i][1];
                work.period = pingPref_period;
                work.timeout = pingPref_timeout;
                work.bufferSize = pingPref_bufferSize;
                work.ttl = pingPref_ttl;

                work.startPing();
            }
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

                    struct_bigData row = new struct_bigData
                    {
                        _00_icon = (lastReply_result == pingWork.pingStatus.online) ? icon_ok : icon_warning,
                        _01_notice = bigData[rowId]._01_notice,
                        _02_no = bigData[rowId]._02_no,
                        _03_hostname = bigData[rowId]._03_hostname,
                        _04_description = bigData[rowId]._04_description,
                        _05_replyIpAddr = worker.lastReply_address,
                        _06_upCount = worker.upCount,
                        _07_downCount = worker.downCount,
                        _08_consecutiveDownCount = worker.consecutiveDownCount,
                        _09_maxConsecutiveDownCount = worker.maxConsecutiveDownCount,
                        _10_maxConsecutiveDownTimestamp = worker.maxConsecutiveDownTimestamp,
                        _11_percentDown = worker.percentDown,
                        _12_lastReply_result = pingWork.pingStatusToText[lastReply_result],
                        _13_lastReply_time = (int)worker.lastReply_time,
                        _14_avg_PingTime = string.Format("{0:0.#}", worker.avgPingTime),
                        _15_lastUpTimestamp = worker.lastUpTimestamp,
                        _16_lastDownTimestamp = worker.lastDownTimestamp,
                        _17_minPingTime = (int)worker.minPingTime,
                        _18_maxPingTime = (int)worker.maxPingTime
                    };

                    bigData[rowId] = row;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateStats();
        }
    }
}
