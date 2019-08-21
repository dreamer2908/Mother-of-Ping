using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mother_of_Ping_GUI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public int pingPref_period = 1000;
        public int pingPref_timeout = 1000;
        public int pingPref_bufferSize = 32;
        public int pingPref_ttl = 128;

        public bool appPref_autoStart = true;
        public bool appPref_saveHostList = true;
        public bool appPref_autoLoadList = false;
        public string appPref_autoLoadListFilename = "";

        public bool appPref_markHostConsFail = true;
        public int appPref_markHostConsFailThreshold = 300;
        public bool appPref_sendTaskbarNotifications = true;
        public bool appPref_sendLineNotifications = true;

        public bool appPref_saveGlobalLog = true;
        public bool appPref_saveIndividualLog = true;
        public string appPref_globalLogFilename = "";
        public string appPref_logFolder = "";
        public bool appPref_useTodayFolder = true;
        public int appPref_flushLogPeriod = 600;

        public bool appPref_generateReportAtExit = true;

        public bool appPref_showLowerPanel = true;
        public bool appPref_showLowerPanel_onlyFailed = false;
        public int appPref_showLowerPanel_limit = 50;

        public bool appPref_schedulerEnable = false;
        public bool appPref_schedulerEnable_start = true;
        public bool appPref_schedulerEnable_stop = true;
        public bool appPref_schedulerEnable_report = true;
        public bool appPref_schedulerEnable_reset = true;

        public string appPref_schedulerTime_start = "7:29";
        public string appPref_schedulerTime_stop = "16:29";
        public string appPref_schedulerTime_report = "16:30";
        public string appPref_schedulerTime_reset = "7:28";

        private void btnOK_Click(object sender, EventArgs e)
        {
            storeSettings();

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            loadSettings();
        }

        private void storeSettings()
        {
            int.TryParse(txtTimeout.Text, out pingPref_timeout);
            int.TryParse(txtPeriod.Text, out pingPref_period);
            int.TryParse(txtSize.Text, out pingPref_bufferSize);
            int.TryParse(txtTTL.Text, out pingPref_ttl);

            appPref_autoStart = chbAutoStart.Checked;

            appPref_saveHostList = radbtnSaveList.Checked;
            appPref_autoLoadList = radbtnLoadFile.Checked;

            appPref_autoLoadListFilename = openFileDialog1.FileName;

            appPref_markHostConsFail = chbMarkHostConsFail.Checked;
            int.TryParse(txtFailToMark.Text, out appPref_markHostConsFailThreshold);

            appPref_sendTaskbarNotifications = chbSendNotifications.Checked;
            appPref_sendLineNotifications = chbSendNotificationsLine.Checked;

            appPref_saveGlobalLog = chbSaveGlobalLog.Checked;
            appPref_saveIndividualLog = chbSaveIndividualLog.Checked;

            appPref_globalLogFilename = txtGlobalLogPath.Text;
            appPref_logFolder = txtLogFolder.Text;
            appPref_useTodayFolder = chbUseTodayFolder.Checked;

            int.TryParse(txtFlushLogPeriod.Text, out appPref_flushLogPeriod);

            appPref_generateReportAtExit = chbGenerateReportAtExit.Checked;

            appPref_showLowerPanel = chbShowLowerPanel.Checked;
            appPref_showLowerPanel_onlyFailed = radbtnchbShowLowerPanel_failed.Checked;
            int.TryParse(txtShowLowerPanel_limit.Text, out appPref_showLowerPanel_limit);

            appPref_schedulerEnable = chbScheduleStartStopReport.Checked;
            appPref_schedulerEnable_start = chbSchedulerEnable_start.Checked;
            appPref_schedulerEnable_stop = chbSchedulerEnable_stop.Checked;
            appPref_schedulerEnable_report = chbSchedulerEnable_report.Checked;
            appPref_schedulerEnable_reset = chbSchedulerEnable_reset.Checked;

            appPref_schedulerTime_start = txtSchedulerTime_start.Text;
            appPref_schedulerTime_stop = txtSchedulerTime_stop.Text;
            appPref_schedulerTime_report = txtSchedulerTime_report.Text;
            appPref_schedulerTime_reset = txtSchedulerTime_reset.Text;
        }

        private void loadSettings()
        {
            txtTimeout.Text = pingPref_timeout.ToString();
            txtPeriod.Text = pingPref_period.ToString();
            txtSize.Text = pingPref_bufferSize.ToString();
            txtTTL.Text = pingPref_ttl.ToString();

            chbAutoStart.Checked = appPref_autoStart;

            appPref_autoLoadList = !appPref_saveHostList;
            radbtnSaveList.Checked = appPref_saveHostList;
            radbtnLoadFile.Checked = appPref_autoLoadList;

            openFileDialog1.FileName = appPref_autoLoadListFilename;

            chbMarkHostConsFail.Checked = appPref_markHostConsFail;
            txtFailToMark.Text = appPref_markHostConsFailThreshold.ToString();

            chbSendNotifications.Checked = appPref_sendTaskbarNotifications;
            chbSendNotificationsLine.Checked = appPref_sendLineNotifications;

            chbSaveGlobalLog.Checked = appPref_saveGlobalLog;
            chbSaveIndividualLog.Checked = appPref_saveIndividualLog;

            txtGlobalLogPath.Text = appPref_globalLogFilename;
            txtLogFolder.Text = appPref_logFolder;
            chbUseTodayFolder.Checked = appPref_useTodayFolder;

            txtFlushLogPeriod.Text = appPref_flushLogPeriod.ToString();

            chbGenerateReportAtExit.Checked = appPref_generateReportAtExit;

            chbShowLowerPanel.Checked = appPref_showLowerPanel;
            radbtnchbShowLowerPanel_failed.Checked = appPref_showLowerPanel_onlyFailed;
            radbtnchbShowLowerPanel_all.Checked = !radbtnchbShowLowerPanel_failed.Checked;
            txtShowLowerPanel_limit.Text = appPref_showLowerPanel_limit.ToString();

            chbScheduleStartStopReport.Checked = appPref_schedulerEnable;
            chbSchedulerEnable_start.Checked = appPref_schedulerEnable_start;
            chbSchedulerEnable_stop.Checked = appPref_schedulerEnable_stop;
            chbSchedulerEnable_report.Checked = appPref_schedulerEnable_report;
            chbSchedulerEnable_reset.Checked = appPref_schedulerEnable_reset;

            txtSchedulerTime_start.Text = appPref_schedulerTime_start;
            txtSchedulerTime_stop.Text = appPref_schedulerTime_stop;
            txtSchedulerTime_report.Text = appPref_schedulerTime_report;
            txtSchedulerTime_reset.Text = appPref_schedulerTime_reset;

            lockUnlockSchedulerControls();
        }

        private void lockUnlockSchedulerControls()
        {
            if (!chbScheduleStartStopReport.Checked)
            {
                chbSchedulerEnable_start.Enabled = false;
                chbSchedulerEnable_stop.Enabled = false;
                chbSchedulerEnable_reset.Enabled = false;
                chbSchedulerEnable_report.Enabled = false;

                txtSchedulerTime_start.Enabled = false;
                txtSchedulerTime_stop.Enabled = false;
                txtSchedulerTime_report.Enabled = false;
                txtSchedulerTime_reset.Enabled = false;
            }
            else
            {
                chbSchedulerEnable_start.Enabled = true;
                chbSchedulerEnable_stop.Enabled = true;
                chbSchedulerEnable_reset.Enabled = true;
                chbSchedulerEnable_report.Enabled = true;

                txtSchedulerTime_start.Enabled = true;
                txtSchedulerTime_stop.Enabled = true;
                txtSchedulerTime_report.Enabled = true;
                txtSchedulerTime_reset.Enabled = true;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnBrowseDefaultList_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void btnBrowseGlobalLogOutput_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtGlobalLogPath.Text = saveFileDialog1.FileName;
            }
        }

        private void btnBrowseLogFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtLogFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void chbScheduleStartStopReport_CheckedChanged(object sender, EventArgs e)
        {
            lockUnlockSchedulerControls();
        }
    }
}
