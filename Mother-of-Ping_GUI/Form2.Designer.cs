namespace Mother_of_Ping_GUI
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTTL = new System.Windows.Forms.TextBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowseDefaultList = new System.Windows.Forms.Button();
            this.radbtnLoadFile = new System.Windows.Forms.RadioButton();
            this.radbtnSaveList = new System.Windows.Forms.RadioButton();
            this.chbAutoStart = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chbIgnoreWriteFailure = new System.Windows.Forms.CheckBox();
            this.chbGenerateReportAtExit = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.chbUseTodayFolder = new System.Windows.Forms.CheckBox();
            this.btnBrowseLogFolder = new System.Windows.Forms.Button();
            this.txtFlushLogPeriod = new System.Windows.Forms.TextBox();
            this.btnBrowseGlobalLogOutput = new System.Windows.Forms.Button();
            this.txtLogFolder = new System.Windows.Forms.TextBox();
            this.txtGlobalLogPath = new System.Windows.Forms.TextBox();
            this.chbSaveIndividualLog = new System.Windows.Forms.CheckBox();
            this.chbSaveGlobalLog = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnEmailSetup = new System.Windows.Forms.Button();
            this.chbSendNotificationsLine = new System.Windows.Forms.CheckBox();
            this.chbEmailNotification = new System.Windows.Forms.CheckBox();
            this.chbSendNotifications = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtEmailDelaySeconds = new System.Windows.Forms.TextBox();
            this.chbMarkHostConsFail = new System.Windows.Forms.CheckBox();
            this.txtFailToMark = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtShowLowerPanel_limit = new System.Windows.Forms.TextBox();
            this.radbtnchbShowLowerPanel_all = new System.Windows.Forms.RadioButton();
            this.radbtnchbShowLowerPanel_failed = new System.Windows.Forms.RadioButton();
            this.chbShowLowerPanel = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chbSchedulerEnable_reset = new System.Windows.Forms.CheckBox();
            this.chbSchedulerEnable_report = new System.Windows.Forms.CheckBox();
            this.chbSchedulerEnable_stop = new System.Windows.Forms.CheckBox();
            this.chbSchedulerEnable_start = new System.Windows.Forms.CheckBox();
            this.txtSchedulerTime_report = new System.Windows.Forms.TextBox();
            this.txtSchedulerTime_reset = new System.Windows.Forms.TextBox();
            this.txtSchedulerTime_stop = new System.Windows.Forms.TextBox();
            this.txtSchedulerTime_start = new System.Windows.Forms.TextBox();
            this.chbScheduleStartStopReport = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnEnableHttpServer = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ping timeout";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(278, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Bufer size";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtTTL);
            this.groupBox1.Controls.Add(this.txtSize);
            this.groupBox1.Controls.Add(this.txtTimeout);
            this.groupBox1.Controls.Add(this.txtPeriod);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 64);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(403, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "hops";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(403, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "bytes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(169, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "milliseconds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "milliseconds";
            // 
            // txtTTL
            // 
            this.txtTTL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTTL.Location = new System.Drawing.Point(333, 36);
            this.txtTTL.Name = "txtTTL";
            this.txtTTL.Size = new System.Drawing.Size(64, 20);
            this.txtTTL.TabIndex = 4;
            this.txtTTL.Text = "128";
            // 
            // txtSize
            // 
            this.txtSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSize.Location = new System.Drawing.Point(333, 13);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(64, 20);
            this.txtSize.TabIndex = 3;
            this.txtSize.Text = "32";
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(99, 13);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(64, 20);
            this.txtTimeout.TabIndex = 1;
            this.txtTimeout.Text = "1000";
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(99, 36);
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(64, 20);
            this.txtPeriod.TabIndex = 2;
            this.txtPeriod.Text = "1000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ping again every";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(278, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "TTL";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnBrowseDefaultList);
            this.groupBox2.Controls.Add(this.radbtnLoadFile);
            this.groupBox2.Controls.Add(this.radbtnSaveList);
            this.groupBox2.Controls.Add(this.chbAutoStart);
            this.groupBox2.Location = new System.Drawing.Point(15, 82);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 48);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // btnBrowseDefaultList
            // 
            this.btnBrowseDefaultList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseDefaultList.Location = new System.Drawing.Point(373, 17);
            this.btnBrowseDefaultList.Name = "btnBrowseDefaultList";
            this.btnBrowseDefaultList.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseDefaultList.TabIndex = 8;
            this.btnBrowseDefaultList.Text = "Browse...";
            this.btnBrowseDefaultList.UseVisualStyleBackColor = true;
            this.btnBrowseDefaultList.Click += new System.EventHandler(this.btnBrowseDefaultList_Click);
            // 
            // radbtnLoadFile
            // 
            this.radbtnLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radbtnLoadFile.AutoSize = true;
            this.radbtnLoadFile.Location = new System.Drawing.Point(284, 20);
            this.radbtnLoadFile.Name = "radbtnLoadFile";
            this.radbtnLoadFile.Size = new System.Drawing.Size(84, 17);
            this.radbtnLoadFile.TabIndex = 7;
            this.radbtnLoadFile.Text = "Load this file";
            this.radbtnLoadFile.UseVisualStyleBackColor = true;
            // 
            // radbtnSaveList
            // 
            this.radbtnSaveList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radbtnSaveList.AutoSize = true;
            this.radbtnSaveList.Checked = true;
            this.radbtnSaveList.Location = new System.Drawing.Point(172, 19);
            this.radbtnSaveList.Name = "radbtnSaveList";
            this.radbtnSaveList.Size = new System.Drawing.Size(105, 17);
            this.radbtnSaveList.TabIndex = 6;
            this.radbtnSaveList.TabStop = true;
            this.radbtnSaveList.Text = "Save address list";
            this.radbtnSaveList.UseVisualStyleBackColor = true;
            // 
            // chbAutoStart
            // 
            this.chbAutoStart.AutoSize = true;
            this.chbAutoStart.Checked = true;
            this.chbAutoStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAutoStart.Location = new System.Drawing.Point(9, 19);
            this.chbAutoStart.Name = "chbAutoStart";
            this.chbAutoStart.Size = new System.Drawing.Size(132, 17);
            this.chbAutoStart.TabIndex = 5;
            this.chbAutoStart.Text = "Start pinging at startup";
            this.chbAutoStart.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chbIgnoreWriteFailure);
            this.groupBox3.Controls.Add(this.chbGenerateReportAtExit);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.chbUseTodayFolder);
            this.groupBox3.Controls.Add(this.btnBrowseLogFolder);
            this.groupBox3.Controls.Add(this.txtFlushLogPeriod);
            this.groupBox3.Controls.Add(this.btnBrowseGlobalLogOutput);
            this.groupBox3.Controls.Add(this.txtLogFolder);
            this.groupBox3.Controls.Add(this.txtGlobalLogPath);
            this.groupBox3.Controls.Add(this.chbSaveIndividualLog);
            this.groupBox3.Controls.Add(this.chbSaveGlobalLog);
            this.groupBox3.Location = new System.Drawing.Point(15, 235);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(456, 113);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            // 
            // chbIgnoreWriteFailure
            // 
            this.chbIgnoreWriteFailure.AutoSize = true;
            this.chbIgnoreWriteFailure.Checked = true;
            this.chbIgnoreWriteFailure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIgnoreWriteFailure.Location = new System.Drawing.Point(252, 91);
            this.chbIgnoreWriteFailure.Name = "chbIgnoreWriteFailure";
            this.chbIgnoreWriteFailure.Size = new System.Drawing.Size(142, 17);
            this.chbIgnoreWriteFailure.TabIndex = 22;
            this.chbIgnoreWriteFailure.Text = "Ignore log writing failures";
            this.chbIgnoreWriteFailure.UseVisualStyleBackColor = true;
            // 
            // chbGenerateReportAtExit
            // 
            this.chbGenerateReportAtExit.AutoSize = true;
            this.chbGenerateReportAtExit.Checked = true;
            this.chbGenerateReportAtExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbGenerateReportAtExit.Location = new System.Drawing.Point(9, 91);
            this.chbGenerateReportAtExit.Name = "chbGenerateReportAtExit";
            this.chbGenerateReportAtExit.Size = new System.Drawing.Size(200, 17);
            this.chbGenerateReportAtExit.TabIndex = 21;
            this.chbGenerateReportAtExit.Text = "Generate reports automatically at exit";
            this.chbGenerateReportAtExit.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(403, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "seconds";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(249, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Flush log every";
            // 
            // chbUseTodayFolder
            // 
            this.chbUseTodayFolder.AutoSize = true;
            this.chbUseTodayFolder.Checked = true;
            this.chbUseTodayFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbUseTodayFolder.Location = new System.Drawing.Point(9, 67);
            this.chbUseTodayFolder.Name = "chbUseTodayFolder";
            this.chbUseTodayFolder.Size = new System.Drawing.Size(165, 17);
            this.chbUseTodayFolder.TabIndex = 19;
            this.chbUseTodayFolder.Text = "Use <yyyy-MM-dd> sub-folder";
            this.chbUseTodayFolder.UseVisualStyleBackColor = true;
            // 
            // btnBrowseLogFolder
            // 
            this.btnBrowseLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseLogFolder.Location = new System.Drawing.Point(373, 37);
            this.btnBrowseLogFolder.Name = "btnBrowseLogFolder";
            this.btnBrowseLogFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLogFolder.TabIndex = 18;
            this.btnBrowseLogFolder.Text = "Browse...";
            this.btnBrowseLogFolder.UseVisualStyleBackColor = true;
            this.btnBrowseLogFolder.Click += new System.EventHandler(this.btnBrowseLogFolder_Click);
            // 
            // txtFlushLogPeriod
            // 
            this.txtFlushLogPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFlushLogPeriod.Location = new System.Drawing.Point(333, 64);
            this.txtFlushLogPeriod.Name = "txtFlushLogPeriod";
            this.txtFlushLogPeriod.Size = new System.Drawing.Size(64, 20);
            this.txtFlushLogPeriod.TabIndex = 20;
            this.txtFlushLogPeriod.Text = "600";
            // 
            // btnBrowseGlobalLogOutput
            // 
            this.btnBrowseGlobalLogOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseGlobalLogOutput.Location = new System.Drawing.Point(373, 13);
            this.btnBrowseGlobalLogOutput.Name = "btnBrowseGlobalLogOutput";
            this.btnBrowseGlobalLogOutput.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseGlobalLogOutput.TabIndex = 15;
            this.btnBrowseGlobalLogOutput.Text = "Browse...";
            this.btnBrowseGlobalLogOutput.UseVisualStyleBackColor = true;
            this.btnBrowseGlobalLogOutput.Click += new System.EventHandler(this.btnBrowseGlobalLogOutput_Click);
            // 
            // txtLogFolder
            // 
            this.txtLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFolder.Location = new System.Drawing.Point(155, 39);
            this.txtLogFolder.Name = "txtLogFolder";
            this.txtLogFolder.Size = new System.Drawing.Size(213, 20);
            this.txtLogFolder.TabIndex = 17;
            // 
            // txtGlobalLogPath
            // 
            this.txtGlobalLogPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGlobalLogPath.Location = new System.Drawing.Point(155, 16);
            this.txtGlobalLogPath.Name = "txtGlobalLogPath";
            this.txtGlobalLogPath.Size = new System.Drawing.Size(213, 20);
            this.txtGlobalLogPath.TabIndex = 14;
            // 
            // chbSaveIndividualLog
            // 
            this.chbSaveIndividualLog.AutoSize = true;
            this.chbSaveIndividualLog.Checked = true;
            this.chbSaveIndividualLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSaveIndividualLog.Location = new System.Drawing.Point(9, 43);
            this.chbSaveIndividualLog.Name = "chbSaveIndividualLog";
            this.chbSaveIndividualLog.Size = new System.Drawing.Size(138, 17);
            this.chbSaveIndividualLog.TabIndex = 16;
            this.chbSaveIndividualLog.Text = "Save logs for each host";
            this.chbSaveIndividualLog.UseVisualStyleBackColor = true;
            // 
            // chbSaveGlobalLog
            // 
            this.chbSaveGlobalLog.AutoSize = true;
            this.chbSaveGlobalLog.Checked = true;
            this.chbSaveGlobalLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSaveGlobalLog.Location = new System.Drawing.Point(9, 19);
            this.chbSaveGlobalLog.Name = "chbSaveGlobalLog";
            this.chbSaveGlobalLog.Size = new System.Drawing.Size(140, 17);
            this.chbSaveGlobalLog.TabIndex = 13;
            this.chbSaveGlobalLog.Text = "Save the global ping log";
            this.chbSaveGlobalLog.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(395, 576);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(314, 576);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 26;
            this.btnOK.Text = "&Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Csv files|*.csv|Text files|*.txt|All files|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Csv files|*.csv|Text files|*.txt|All files|*.*";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnEmailSetup);
            this.groupBox4.Controls.Add(this.chbSendNotificationsLine);
            this.groupBox4.Controls.Add(this.chbEmailNotification);
            this.groupBox4.Controls.Add(this.chbSendNotifications);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.txtEmailDelaySeconds);
            this.groupBox4.Controls.Add(this.chbMarkHostConsFail);
            this.groupBox4.Controls.Add(this.txtFailToMark);
            this.groupBox4.Location = new System.Drawing.Point(15, 136);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(456, 93);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            // 
            // btnEmailSetup
            // 
            this.btnEmailSetup.Location = new System.Drawing.Point(145, 62);
            this.btnEmailSetup.Name = "btnEmailSetup";
            this.btnEmailSetup.Size = new System.Drawing.Size(75, 23);
            this.btnEmailSetup.TabIndex = 13;
            this.btnEmailSetup.Text = "Setup...";
            this.btnEmailSetup.UseVisualStyleBackColor = true;
            this.btnEmailSetup.Click += new System.EventHandler(this.btnEmailSetup_Click);
            // 
            // chbSendNotificationsLine
            // 
            this.chbSendNotificationsLine.AutoSize = true;
            this.chbSendNotificationsLine.Enabled = false;
            this.chbSendNotificationsLine.Location = new System.Drawing.Point(255, 43);
            this.chbSendNotificationsLine.Name = "chbSendNotificationsLine";
            this.chbSendNotificationsLine.Size = new System.Drawing.Size(180, 17);
            this.chbSendNotificationsLine.TabIndex = 12;
            this.chbSendNotificationsLine.Text = "Send notifications via LINE (n/a)";
            this.chbSendNotificationsLine.UseVisualStyleBackColor = true;
            // 
            // chbEmailNotification
            // 
            this.chbEmailNotification.AutoSize = true;
            this.chbEmailNotification.Checked = true;
            this.chbEmailNotification.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbEmailNotification.Location = new System.Drawing.Point(9, 66);
            this.chbEmailNotification.Name = "chbEmailNotification";
            this.chbEmailNotification.Size = new System.Drawing.Size(137, 17);
            this.chbEmailNotification.TabIndex = 11;
            this.chbEmailNotification.Text = "Send email notifications";
            this.chbEmailNotification.UseVisualStyleBackColor = true;
            // 
            // chbSendNotifications
            // 
            this.chbSendNotifications.AutoSize = true;
            this.chbSendNotifications.Checked = true;
            this.chbSendNotifications.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSendNotifications.Location = new System.Drawing.Point(9, 43);
            this.chbSendNotifications.Name = "chbSendNotifications";
            this.chbSendNotifications.Size = new System.Drawing.Size(211, 17);
            this.chbSendNotifications.TabIndex = 11;
            this.chbSendNotifications.Text = "Send taskbar notifications every minute";
            this.chbSendNotifications.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(247, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Delay between emails";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(403, 67);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(47, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "seconds";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(403, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "seconds";
            // 
            // txtEmailDelaySeconds
            // 
            this.txtEmailDelaySeconds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailDelaySeconds.Location = new System.Drawing.Point(363, 64);
            this.txtEmailDelaySeconds.Name = "txtEmailDelaySeconds";
            this.txtEmailDelaySeconds.Size = new System.Drawing.Size(34, 20);
            this.txtEmailDelaySeconds.TabIndex = 10;
            this.txtEmailDelaySeconds.Text = "300";
            // 
            // chbMarkHostConsFail
            // 
            this.chbMarkHostConsFail.AutoSize = true;
            this.chbMarkHostConsFail.Checked = true;
            this.chbMarkHostConsFail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbMarkHostConsFail.Location = new System.Drawing.Point(9, 19);
            this.chbMarkHostConsFail.Name = "chbMarkHostConsFail";
            this.chbMarkHostConsFail.Size = new System.Drawing.Size(236, 17);
            this.chbMarkHostConsFail.TabIndex = 9;
            this.chbMarkHostConsFail.Text = "Mark hosts that fail consecutively more than ";
            this.chbMarkHostConsFail.UseVisualStyleBackColor = true;
            // 
            // txtFailToMark
            // 
            this.txtFailToMark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFailToMark.Location = new System.Drawing.Point(333, 17);
            this.txtFailToMark.Name = "txtFailToMark";
            this.txtFailToMark.Size = new System.Drawing.Size(64, 20);
            this.txtFailToMark.TabIndex = 10;
            this.txtFailToMark.Text = "300";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.txtShowLowerPanel_limit);
            this.groupBox5.Controls.Add(this.radbtnchbShowLowerPanel_all);
            this.groupBox5.Controls.Add(this.radbtnchbShowLowerPanel_failed);
            this.groupBox5.Controls.Add(this.chbShowLowerPanel);
            this.groupBox5.Location = new System.Drawing.Point(15, 354);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(455, 71);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(29, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(132, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Limit the number of lines to";
            // 
            // txtShowLowerPanel_limit
            // 
            this.txtShowLowerPanel_limit.Location = new System.Drawing.Point(172, 43);
            this.txtShowLowerPanel_limit.Name = "txtShowLowerPanel_limit";
            this.txtShowLowerPanel_limit.Size = new System.Drawing.Size(100, 20);
            this.txtShowLowerPanel_limit.TabIndex = 25;
            this.txtShowLowerPanel_limit.Text = "50";
            // 
            // radbtnchbShowLowerPanel_all
            // 
            this.radbtnchbShowLowerPanel_all.AutoSize = true;
            this.radbtnchbShowLowerPanel_all.Location = new System.Drawing.Point(276, 20);
            this.radbtnchbShowLowerPanel_all.Name = "radbtnchbShowLowerPanel_all";
            this.radbtnchbShowLowerPanel_all.Size = new System.Drawing.Size(64, 17);
            this.radbtnchbShowLowerPanel_all.TabIndex = 23;
            this.radbtnchbShowLowerPanel_all.Text = "All pings";
            this.radbtnchbShowLowerPanel_all.UseVisualStyleBackColor = true;
            // 
            // radbtnchbShowLowerPanel_failed
            // 
            this.radbtnchbShowLowerPanel_failed.AutoSize = true;
            this.radbtnchbShowLowerPanel_failed.Checked = true;
            this.radbtnchbShowLowerPanel_failed.Location = new System.Drawing.Point(346, 20);
            this.radbtnchbShowLowerPanel_failed.Name = "radbtnchbShowLowerPanel_failed";
            this.radbtnchbShowLowerPanel_failed.Size = new System.Drawing.Size(102, 17);
            this.radbtnchbShowLowerPanel_failed.TabIndex = 24;
            this.radbtnchbShowLowerPanel_failed.TabStop = true;
            this.radbtnchbShowLowerPanel_failed.Text = "Only failed pings";
            this.radbtnchbShowLowerPanel_failed.UseVisualStyleBackColor = true;
            // 
            // chbShowLowerPanel
            // 
            this.chbShowLowerPanel.AutoSize = true;
            this.chbShowLowerPanel.Checked = true;
            this.chbShowLowerPanel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbShowLowerPanel.Location = new System.Drawing.Point(9, 20);
            this.chbShowLowerPanel.Name = "chbShowLowerPanel";
            this.chbShowLowerPanel.Size = new System.Drawing.Size(194, 17);
            this.chbShowLowerPanel.TabIndex = 22;
            this.chbShowLowerPanel.Text = "Show recent logs in the lower panel";
            this.chbShowLowerPanel.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.chbSchedulerEnable_reset);
            this.groupBox6.Controls.Add(this.chbSchedulerEnable_report);
            this.groupBox6.Controls.Add(this.chbSchedulerEnable_stop);
            this.groupBox6.Controls.Add(this.chbSchedulerEnable_start);
            this.groupBox6.Controls.Add(this.txtSchedulerTime_report);
            this.groupBox6.Controls.Add(this.txtSchedulerTime_reset);
            this.groupBox6.Controls.Add(this.txtSchedulerTime_stop);
            this.groupBox6.Controls.Add(this.txtSchedulerTime_start);
            this.groupBox6.Controls.Add(this.chbScheduleStartStopReport);
            this.groupBox6.Location = new System.Drawing.Point(15, 431);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(455, 70);
            this.groupBox6.TabIndex = 28;
            this.groupBox6.TabStop = false;
            // 
            // chbSchedulerEnable_reset
            // 
            this.chbSchedulerEnable_reset.AutoSize = true;
            this.chbSchedulerEnable_reset.Checked = true;
            this.chbSchedulerEnable_reset.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSchedulerEnable_reset.Location = new System.Drawing.Point(340, 43);
            this.chbSchedulerEnable_reset.Name = "chbSchedulerEnable_reset";
            this.chbSchedulerEnable_reset.Size = new System.Drawing.Size(54, 17);
            this.chbSchedulerEnable_reset.TabIndex = 33;
            this.chbSchedulerEnable_reset.Text = "Reset";
            this.chbSchedulerEnable_reset.UseVisualStyleBackColor = true;
            // 
            // chbSchedulerEnable_report
            // 
            this.chbSchedulerEnable_report.AutoSize = true;
            this.chbSchedulerEnable_report.Checked = true;
            this.chbSchedulerEnable_report.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSchedulerEnable_report.Location = new System.Drawing.Point(223, 43);
            this.chbSchedulerEnable_report.Name = "chbSchedulerEnable_report";
            this.chbSchedulerEnable_report.Size = new System.Drawing.Size(58, 17);
            this.chbSchedulerEnable_report.TabIndex = 31;
            this.chbSchedulerEnable_report.Text = "Report";
            this.chbSchedulerEnable_report.UseVisualStyleBackColor = true;
            // 
            // chbSchedulerEnable_stop
            // 
            this.chbSchedulerEnable_stop.AutoSize = true;
            this.chbSchedulerEnable_stop.Checked = true;
            this.chbSchedulerEnable_stop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSchedulerEnable_stop.Location = new System.Drawing.Point(116, 43);
            this.chbSchedulerEnable_stop.Name = "chbSchedulerEnable_stop";
            this.chbSchedulerEnable_stop.Size = new System.Drawing.Size(48, 17);
            this.chbSchedulerEnable_stop.TabIndex = 29;
            this.chbSchedulerEnable_stop.Text = "Stop";
            this.chbSchedulerEnable_stop.UseVisualStyleBackColor = true;
            // 
            // chbSchedulerEnable_start
            // 
            this.chbSchedulerEnable_start.AutoSize = true;
            this.chbSchedulerEnable_start.Checked = true;
            this.chbSchedulerEnable_start.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSchedulerEnable_start.Location = new System.Drawing.Point(9, 43);
            this.chbSchedulerEnable_start.Name = "chbSchedulerEnable_start";
            this.chbSchedulerEnable_start.Size = new System.Drawing.Size(48, 17);
            this.chbSchedulerEnable_start.TabIndex = 27;
            this.chbSchedulerEnable_start.Text = "Start";
            this.chbSchedulerEnable_start.UseVisualStyleBackColor = true;
            // 
            // txtSchedulerTime_report
            // 
            this.txtSchedulerTime_report.Location = new System.Drawing.Point(287, 41);
            this.txtSchedulerTime_report.Name = "txtSchedulerTime_report";
            this.txtSchedulerTime_report.Size = new System.Drawing.Size(47, 20);
            this.txtSchedulerTime_report.TabIndex = 32;
            this.txtSchedulerTime_report.Text = "16:30";
            // 
            // txtSchedulerTime_reset
            // 
            this.txtSchedulerTime_reset.Location = new System.Drawing.Point(400, 41);
            this.txtSchedulerTime_reset.Name = "txtSchedulerTime_reset";
            this.txtSchedulerTime_reset.Size = new System.Drawing.Size(47, 20);
            this.txtSchedulerTime_reset.TabIndex = 34;
            this.txtSchedulerTime_reset.Text = "07:28";
            // 
            // txtSchedulerTime_stop
            // 
            this.txtSchedulerTime_stop.Location = new System.Drawing.Point(170, 41);
            this.txtSchedulerTime_stop.Name = "txtSchedulerTime_stop";
            this.txtSchedulerTime_stop.Size = new System.Drawing.Size(47, 20);
            this.txtSchedulerTime_stop.TabIndex = 30;
            this.txtSchedulerTime_stop.Text = "16:29";
            // 
            // txtSchedulerTime_start
            // 
            this.txtSchedulerTime_start.Location = new System.Drawing.Point(63, 41);
            this.txtSchedulerTime_start.Name = "txtSchedulerTime_start";
            this.txtSchedulerTime_start.Size = new System.Drawing.Size(47, 20);
            this.txtSchedulerTime_start.TabIndex = 28;
            this.txtSchedulerTime_start.Text = "07:29";
            // 
            // chbScheduleStartStopReport
            // 
            this.chbScheduleStartStopReport.AutoSize = true;
            this.chbScheduleStartStopReport.Checked = true;
            this.chbScheduleStartStopReport.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbScheduleStartStopReport.Location = new System.Drawing.Point(9, 20);
            this.chbScheduleStartStopReport.Name = "chbScheduleStartStopReport";
            this.chbScheduleStartStopReport.Size = new System.Drawing.Size(374, 17);
            this.chbScheduleStartStopReport.TabIndex = 26;
            this.chbScheduleStartStopReport.Text = "Enable schedulers for daily starting, stopping, reporting, and resetting stats";
            this.chbScheduleStartStopReport.UseVisualStyleBackColor = true;
            this.chbScheduleStartStopReport.CheckedChanged += new System.EventHandler(this.chbScheduleStartStopReport_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.linkLabel2);
            this.groupBox7.Controls.Add(this.linkLabel1);
            this.groupBox7.Controls.Add(this.btnEnableHttpServer);
            this.groupBox7.Location = new System.Drawing.Point(15, 507);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(455, 60);
            this.groupBox7.TabIndex = 29;
            this.groupBox7.TabStop = false;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(169, 40);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(129, 13);
            this.linkLabel2.TabIndex = 1;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "http://localhost:3037/csv";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(29, 40);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(107, 13);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://localhost:3037";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // btnEnableHttpServer
            // 
            this.btnEnableHttpServer.AutoSize = true;
            this.btnEnableHttpServer.Location = new System.Drawing.Point(9, 20);
            this.btnEnableHttpServer.Name = "btnEnableHttpServer";
            this.btnEnableHttpServer.Size = new System.Drawing.Size(328, 17);
            this.btnEnableHttpServer.TabIndex = 0;
            this.btnEnableHttpServer.Text = "Enable HTTP server for remote stats access (requires re-launch)";
            this.btnEnableHttpServer.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 611);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form2";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTTL;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowseDefaultList;
        private System.Windows.Forms.RadioButton radbtnLoadFile;
        private System.Windows.Forms.RadioButton radbtnSaveList;
        private System.Windows.Forms.CheckBox chbAutoStart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chbSaveIndividualLog;
        private System.Windows.Forms.CheckBox chbSaveGlobalLog;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chbUseTodayFolder;
        private System.Windows.Forms.Button btnBrowseLogFolder;
        private System.Windows.Forms.TextBox txtFlushLogPeriod;
        private System.Windows.Forms.Button btnBrowseGlobalLogOutput;
        private System.Windows.Forms.TextBox txtLogFolder;
        private System.Windows.Forms.TextBox txtGlobalLogPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chbSendNotificationsLine;
        private System.Windows.Forms.CheckBox chbSendNotifications;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chbMarkHostConsFail;
        private System.Windows.Forms.TextBox txtFailToMark;
        private System.Windows.Forms.CheckBox chbGenerateReportAtExit;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtShowLowerPanel_limit;
        private System.Windows.Forms.RadioButton radbtnchbShowLowerPanel_all;
        private System.Windows.Forms.RadioButton radbtnchbShowLowerPanel_failed;
        private System.Windows.Forms.CheckBox chbShowLowerPanel;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chbScheduleStartStopReport;
        private System.Windows.Forms.TextBox txtSchedulerTime_report;
        private System.Windows.Forms.TextBox txtSchedulerTime_reset;
        private System.Windows.Forms.TextBox txtSchedulerTime_stop;
        private System.Windows.Forms.TextBox txtSchedulerTime_start;
        private System.Windows.Forms.CheckBox chbSchedulerEnable_reset;
        private System.Windows.Forms.CheckBox chbSchedulerEnable_report;
        private System.Windows.Forms.CheckBox chbSchedulerEnable_stop;
        private System.Windows.Forms.CheckBox chbSchedulerEnable_start;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox btnEnableHttpServer;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.CheckBox chbIgnoreWriteFailure;
        private System.Windows.Forms.Button btnEmailSetup;
        private System.Windows.Forms.CheckBox chbEmailNotification;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtEmailDelaySeconds;
    }
}