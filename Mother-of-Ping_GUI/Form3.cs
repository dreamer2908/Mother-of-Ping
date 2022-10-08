using Microsoft.VisualBasic;
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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        public Form2 form2;

        private void btnReceiverAdd_Click(object sender, EventArgs e)
        {
            string newEmail = Interaction.InputBox("Enter a new receipent email address", "Add email", "admin@mail.com");
            lsvReceiver.Items.Add(newEmail);
        }

        private void btnReceiverDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in lsvReceiver.SelectedItems)
            {
                lsvReceiver.Items.Remove(eachItem);
            }
        }

        private void btnReceiverTest_Click(object sender, EventArgs e)
        {
            List<string> custom_to = new List<string>();
            for (int i = 0; i < lsvReceiver.Items.Count; i++)
            {
                custom_to.Add(lsvReceiver.Items[i].Text);
            }

            form2.form1.sendEmailAlert(true, custom_to);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            txtEmailFrom.Text = form2.email_from;
            txtEmailHost.Text = form2.email_host;
            chbEmailSsl.Checked = form2.email_ssl;
            numEmailPort.Value = form2.email_port;
            chbEmailLogin.Checked = form2.email_login;
            txtEmailUser.Text = form2.email_user;
            txtEmailPassword.Text = form2.email_password;
            txtEmailSubject.Text = form2.email_subject;

            lsvReceiver.Items.Clear();
            foreach (var item in form2.email_to)
            {
                lsvReceiver.Items.Add(item);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            form2.email_from = txtEmailFrom.Text;
            form2.email_host = txtEmailHost.Text;
            form2.email_ssl = chbEmailSsl.Checked;
            form2.email_port = (int)numEmailPort.Value;
            form2.email_login = chbEmailLogin.Checked;
            form2.email_user = txtEmailUser.Text;
            form2.email_password = txtEmailPassword.Text;
            form2.email_subject = txtEmailSubject.Text;

            form2.email_to.Clear();
            for (int i = 0; i < lsvReceiver.Items.Count; i++)
            {
                form2.email_to.Add(lsvReceiver.Items[i].Text);
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
