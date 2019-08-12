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
            Icon _0_icon;
            bool _1_notice;
            int _2_no;
            string _3_hostname;
            string _4_description;
            string _5_replyIpAddr;
            int _6_upCount;
            int _7_downCount;
            int _8_consecutiveDownCount;
            int _9_maxConsecutiveDownCount;
            string _10_maxConsecutiveDownTimestamp;
            string _11_percentDown;
            string _12_lastReply_result;
            int _13_lastReply_time;
            int _14_avg_PingTime;
            string _15_lastUpTimestamp;
            string _16_lastDownTimestamp;
            int _17_minPingTime;
            int _18_maxPingTime;
        }

        private void resetTable()
        {

            bigData = new DataTable();
            bind = new SyncBindingSource();
            bind.DataSource = bigData;

            //dgvPing.AutoGenerateColumns = false;
            dgvPing.DataSource = bigData;
            dgvPing.DataSource = bind;

            bigData.Columns.Add(" ", typeof(Icon)); // 0
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
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dgvPing.Refresh();
        }

        private void loadHostListToTable(List<string[]> hostList)
        {
            int numOfHost = hostList.Count;

            for (int i = 0; i < numOfHost; i++)
            {
                bigData.Rows.Add(icon_blank, true, i, hostList[i][0], hostList[i][1]);
            }
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

                    bigData.Rows[rowId][0] = (lastReply_result == pingWork.pingStatus.online) ? icon_ok : icon_warning;

                    bigData.Rows[rowId][5] = worker.lastReply_address;
                    bigData.Rows[rowId][6] = worker.upCount;
                    bigData.Rows[rowId][7] = worker.downCount;
                    bigData.Rows[rowId][8] = worker.consecutiveDownCount;
                    bigData.Rows[rowId][9] = worker.maxConsecutiveDownCount;
                    bigData.Rows[rowId][10] = worker.maxConsecutiveDownTimestamp;
                    bigData.Rows[rowId][11] = worker.percentDown;
                    bigData.Rows[rowId][12] = pingWork.pingStatusToText[lastReply_result];
                    bigData.Rows[rowId][13] = worker.lastReply_time;
                    bigData.Rows[rowId][14] = string.Format("{0:0.#}", worker.avgPingTime);
                    bigData.Rows[rowId][15] = worker.lastUpTimestamp;
                    bigData.Rows[rowId][16] = worker.lastDownTimestamp;
                    bigData.Rows[rowId][17] = worker.minPingTime;
                    bigData.Rows[rowId][18] = worker.maxPingTime;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateStats();
        }
    }
}
