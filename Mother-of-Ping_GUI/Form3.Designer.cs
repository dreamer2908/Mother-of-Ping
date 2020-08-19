namespace Mother_of_Ping_GUI
{
    partial class Form3
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chbEmailSsl = new System.Windows.Forms.CheckBox();
            this.chbEmailLogin = new System.Windows.Forms.CheckBox();
            this.txtEmailPassword = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtEmailUser = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtEmailHost = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtEmailFrom = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numEmailPort = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtEmailSubject = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnReceiverTest = new System.Windows.Forms.Button();
            this.btnReceiverDelete = new System.Windows.Forms.Button();
            this.btnReceiverAdd = new System.Windows.Forms.Button();
            this.lsvReceiver = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEmailPort)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chbEmailSsl);
            this.groupBox2.Controls.Add(this.chbEmailLogin);
            this.groupBox2.Controls.Add(this.txtEmailPassword);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtEmailUser);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtEmailHost);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtEmailFrom);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.numEmailPort);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(339, 157);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mail Server";
            // 
            // chbEmailSsl
            // 
            this.chbEmailSsl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbEmailSsl.AutoSize = true;
            this.chbEmailSsl.Location = new System.Drawing.Point(287, 64);
            this.chbEmailSsl.Name = "chbEmailSsl";
            this.chbEmailSsl.Size = new System.Drawing.Size(46, 17);
            this.chbEmailSsl.TabIndex = 7;
            this.chbEmailSsl.Text = "SSL";
            this.chbEmailSsl.UseVisualStyleBackColor = true;
            // 
            // chbEmailLogin
            // 
            this.chbEmailLogin.AutoSize = true;
            this.chbEmailLogin.Checked = true;
            this.chbEmailLogin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbEmailLogin.Location = new System.Drawing.Point(10, 86);
            this.chbEmailLogin.Name = "chbEmailLogin";
            this.chbEmailLogin.Size = new System.Drawing.Size(188, 17);
            this.chbEmailLogin.TabIndex = 8;
            this.chbEmailLogin.Text = "This server requires authentication";
            this.chbEmailLogin.UseVisualStyleBackColor = true;
            // 
            // txtEmailPassword
            // 
            this.txtEmailPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailPassword.Location = new System.Drawing.Point(90, 127);
            this.txtEmailPassword.Name = "txtEmailPassword";
            this.txtEmailPassword.PasswordChar = '*';
            this.txtEmailPassword.Size = new System.Drawing.Size(243, 20);
            this.txtEmailPassword.TabIndex = 10;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 130);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Password:";
            // 
            // txtEmailUser
            // 
            this.txtEmailUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailUser.Location = new System.Drawing.Point(90, 105);
            this.txtEmailUser.Name = "txtEmailUser";
            this.txtEmailUser.Size = new System.Drawing.Size(243, 20);
            this.txtEmailUser.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(28, 108);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Username:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Port:";
            // 
            // txtEmailHost
            // 
            this.txtEmailHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailHost.Location = new System.Drawing.Point(90, 39);
            this.txtEmailHost.Name = "txtEmailHost";
            this.txtEmailHost.Size = new System.Drawing.Size(243, 20);
            this.txtEmailHost.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "SMTP Server:";
            // 
            // txtEmailFrom
            // 
            this.txtEmailFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailFrom.Location = new System.Drawing.Point(90, 17);
            this.txtEmailFrom.Name = "txtEmailFrom";
            this.txtEmailFrom.Size = new System.Drawing.Size(243, 20);
            this.txtEmailFrom.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 20);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Sender";
            // 
            // numEmailPort
            // 
            this.numEmailPort.Location = new System.Drawing.Point(90, 63);
            this.numEmailPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numEmailPort.Name = "numEmailPort";
            this.numEmailPort.Size = new System.Drawing.Size(59, 20);
            this.numEmailPort.TabIndex = 6;
            this.numEmailPort.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtEmailSubject);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.btnReceiverTest);
            this.groupBox3.Controls.Add(this.btnReceiverDelete);
            this.groupBox3.Controls.Add(this.btnReceiverAdd);
            this.groupBox3.Controls.Add(this.lsvReceiver);
            this.groupBox3.Location = new System.Drawing.Point(12, 175);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(339, 155);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Target";
            // 
            // txtEmailSubject
            // 
            this.txtEmailSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmailSubject.Location = new System.Drawing.Point(90, 129);
            this.txtEmailSubject.Name = "txtEmailSubject";
            this.txtEmailSubject.Size = new System.Drawing.Size(243, 20);
            this.txtEmailSubject.TabIndex = 18;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 132);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(74, 13);
            this.label19.TabIndex = 6;
            this.label19.Text = "Email Subject:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(7, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(137, 13);
            this.label18.TabIndex = 5;
            this.label18.Text = "Receipent email addresses:";
            // 
            // btnReceiverTest
            // 
            this.btnReceiverTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReceiverTest.Location = new System.Drawing.Point(258, 101);
            this.btnReceiverTest.Name = "btnReceiverTest";
            this.btnReceiverTest.Size = new System.Drawing.Size(75, 23);
            this.btnReceiverTest.TabIndex = 15;
            this.btnReceiverTest.Text = "Test";
            this.btnReceiverTest.UseVisualStyleBackColor = true;
            this.btnReceiverTest.Click += new System.EventHandler(this.btnReceiverTest_Click);
            // 
            // btnReceiverDelete
            // 
            this.btnReceiverDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReceiverDelete.Location = new System.Drawing.Point(258, 72);
            this.btnReceiverDelete.Name = "btnReceiverDelete";
            this.btnReceiverDelete.Size = new System.Drawing.Size(75, 23);
            this.btnReceiverDelete.TabIndex = 14;
            this.btnReceiverDelete.Text = "Delete";
            this.btnReceiverDelete.UseVisualStyleBackColor = true;
            this.btnReceiverDelete.Click += new System.EventHandler(this.btnReceiverDelete_Click);
            // 
            // btnReceiverAdd
            // 
            this.btnReceiverAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReceiverAdd.Location = new System.Drawing.Point(258, 43);
            this.btnReceiverAdd.Name = "btnReceiverAdd";
            this.btnReceiverAdd.Size = new System.Drawing.Size(75, 23);
            this.btnReceiverAdd.TabIndex = 13;
            this.btnReceiverAdd.Text = "Add...";
            this.btnReceiverAdd.UseVisualStyleBackColor = true;
            this.btnReceiverAdd.Click += new System.EventHandler(this.btnReceiverAdd_Click);
            // 
            // lsvReceiver
            // 
            this.lsvReceiver.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvReceiver.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lsvReceiver.FullRowSelect = true;
            this.lsvReceiver.GridLines = true;
            this.lsvReceiver.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lsvReceiver.HideSelection = false;
            this.lsvReceiver.Location = new System.Drawing.Point(7, 43);
            this.lsvReceiver.Name = "lsvReceiver";
            this.lsvReceiver.Size = new System.Drawing.Size(245, 80);
            this.lsvReceiver.TabIndex = 12;
            this.lsvReceiver.UseCompatibleStateImageBehavior = false;
            this.lsvReceiver.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 200;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(276, 339);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(195, 339);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 374);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form3";
            this.Text = "Email Setup";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEmailPort)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chbEmailSsl;
        private System.Windows.Forms.CheckBox chbEmailLogin;
        private System.Windows.Forms.TextBox txtEmailPassword;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtEmailUser;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtEmailHost;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtEmailFrom;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numEmailPort;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtEmailSubject;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnReceiverTest;
        private System.Windows.Forms.Button btnReceiverDelete;
        private System.Windows.Forms.Button btnReceiverAdd;
        private System.Windows.Forms.ListView lsvReceiver;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}