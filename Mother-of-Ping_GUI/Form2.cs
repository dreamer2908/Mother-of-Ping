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
        public string appPref_globalLogFilename = "0.0.0.0.csv";
        public string appPref_logFolder = "";
        public bool appPref_useTodayFolder = true;
        public int appPref_flushLogPeriod = 600;

        public bool appPref_generateReportAtExit = true;

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
        }
    }
}
