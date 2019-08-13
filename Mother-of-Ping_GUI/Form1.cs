using Mother_of_Ping_CLI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            disableDgrAutoSize();
            //backgroundWorker1.RunWorkerAsync();
            timer1.Interval = 1000; // in miliseconds
            timer1.Start();
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

            dgvPing.Refresh();


            // from https://10tec.com/articles/why-datagridview-slow.aspx
            dgvPing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dgvPing.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgvPing, true, null);
            }

            workForce = null;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateStats();
        }
    }
}
