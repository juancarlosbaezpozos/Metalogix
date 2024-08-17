using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.UI.WinForms.RemotePowerShell.UI;

namespace Metalogix.UI.WinForms.Components
{
    public class AddEditAgentControl : XtraUserControl
    {
        private delegate void SetTextCallback(string text);

        private IContainer components;

        private TextEdit tbxAgentIP;

        private LabelControl lbIPAddress;

        private GroupControl groupConctAs;

        private TextEdit tbxUserName;

        private LabelControl lblPassword;

        private TextEdit tbxPassword;

        private SimpleButton btnBrowse;

        private LabelControl lblAgentName;

        private LabelControl lblUserName;

        private TextEdit tbxAgentName;

        private LabelControl labelControl1;

        public string MachineIP { get; private set; }

        public string MachineName { get; set; }

        public string Password { get; private set; }

        public string UserName { get; private set; }

        public AddEditAgentControl()
        {
            InitializeComponent();
            tbxUserName.Text = GetCurrentWindowsUser();
        }

        private void AddEditAgentControl_Load(object sender, EventArgs e)
        {
            tbxAgentName.Focus();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                AgentsBrowserDialog agentsBrowserDialog = new AgentsBrowserDialog();
                if (agentsBrowserDialog.ShowDialog() != DialogResult.Cancel)
                {
                    tbxAgentName.Text = agentsBrowserDialog.SelectedAgent;
                    new Thread(SetIPAddress).Start();
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(new ConditionalDetailException(exception));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetCurrentWindowsUser()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            if (current == null)
            {
                return string.Empty;
            }
            return current.Name;
        }

        public void Initialize()
        {
            tbxAgentName.Text = string.Empty;
            tbxAgentIP.Text = string.Empty;
            tbxUserName.Text = GetCurrentWindowsUser();
            tbxPassword.Text = string.Empty;
        }

        private void InitializeComponent()
        {
            this.tbxAgentIP = new DevExpress.XtraEditors.TextEdit();
            this.lbIPAddress = new DevExpress.XtraEditors.LabelControl();
            this.groupConctAs = new DevExpress.XtraEditors.GroupControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblUserName = new DevExpress.XtraEditors.LabelControl();
            this.tbxUserName = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.tbxPassword = new DevExpress.XtraEditors.TextEdit();
            this.btnBrowse = new DevExpress.XtraEditors.SimpleButton();
            this.lblAgentName = new DevExpress.XtraEditors.LabelControl();
            this.tbxAgentName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)this.tbxAgentIP.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.groupConctAs).BeginInit();
            this.groupConctAs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.tbxUserName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxAgentName.Properties).BeginInit();
            base.SuspendLayout();
            this.tbxAgentIP.Location = new System.Drawing.Point(97, 40);
            this.tbxAgentIP.Name = "tbxAgentIP";
            this.tbxAgentIP.Size = new System.Drawing.Size(333, 20);
            this.tbxAgentIP.TabIndex = 2;
            this.lbIPAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbIPAddress.Location = new System.Drawing.Point(20, 40);
            this.lbIPAddress.Name = "lbIPAddress";
            this.lbIPAddress.Size = new System.Drawing.Size(56, 13);
            this.lbIPAddress.TabIndex = 69;
            this.lbIPAddress.Text = "IP Address:";
            this.groupConctAs.Controls.Add(this.labelControl1);
            this.groupConctAs.Controls.Add(this.lblUserName);
            this.groupConctAs.Controls.Add(this.tbxUserName);
            this.groupConctAs.Controls.Add(this.lblPassword);
            this.groupConctAs.Controls.Add(this.tbxPassword);
            this.groupConctAs.Location = new System.Drawing.Point(20, 71);
            this.groupConctAs.Name = "groupConctAs";
            this.groupConctAs.Size = new System.Drawing.Size(410, 120);
            this.groupConctAs.TabIndex = 65;
            this.groupConctAs.Text = "Connect As";
            this.labelControl1.Location = new System.Drawing.Point(95, 96);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(242, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Note: Password will be saved in encrypted format.";
            this.lblUserName.Location = new System.Drawing.Point(20, 43);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(56, 13);
            this.lblUserName.TabIndex = 6;
            this.lblUserName.Text = "User Name:";
            this.tbxUserName.Location = new System.Drawing.Point(95, 40);
            this.tbxUserName.Name = "tbxUserName";
            this.tbxUserName.Size = new System.Drawing.Size(301, 20);
            this.tbxUserName.TabIndex = 1;
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
            this.lblPassword.Location = new System.Drawing.Point(20, 73);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(49, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            this.tbxPassword.Location = new System.Drawing.Point(95, 70);
            this.tbxPassword.Name = "tbxPassword";
            this.tbxPassword.Properties.PasswordChar = '*';
            this.tbxPassword.Size = new System.Drawing.Size(301, 20);
            this.tbxPassword.TabIndex = 2;
            this.btnBrowse.Location = new System.Drawing.Point(365, 8);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(65, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new System.EventHandler(btnBrowse_Click);
            this.lblAgentName.Location = new System.Drawing.Point(20, 10);
            this.lblAgentName.Name = "lblAgentName";
            this.lblAgentName.Size = new System.Drawing.Size(63, 13);
            this.lblAgentName.TabIndex = 68;
            this.lblAgentName.Text = "Agent Name:";
            this.tbxAgentName.Location = new System.Drawing.Point(97, 10);
            this.tbxAgentName.Name = "tbxAgentName";
            this.tbxAgentName.Size = new System.Drawing.Size(262, 20);
            this.tbxAgentName.TabIndex = 0;
            this.tbxAgentName.Leave += new System.EventHandler(tbxAgentName_Leave);
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.tbxAgentName);
            base.Controls.Add(this.tbxAgentIP);
            base.Controls.Add(this.lbIPAddress);
            base.Controls.Add(this.groupConctAs);
            base.Controls.Add(this.btnBrowse);
            base.Controls.Add(this.lblAgentName);
            base.Name = "AddEditAgentControl";
            base.Size = new System.Drawing.Size(450, 212);
            base.Load += new System.EventHandler(AddEditAgentControl_Load);
            ((System.ComponentModel.ISupportInitialize)this.tbxAgentIP.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.groupConctAs).EndInit();
            this.groupConctAs.ResumeLayout(false);
            this.groupConctAs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.tbxUserName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tbxAgentName.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public void LoadUI(Agent Agent)
        {
            tbxAgentName.Text = Agent.MachineName;
            tbxAgentIP.Text = Agent.MachineIP;
            tbxUserName.Text = Agent.UserName;
            tbxPassword.Text = Agent.Password;
            tbxAgentName.Enabled = false;
            tbxAgentIP.Enabled = false;
            btnBrowse.Enabled = false;
        }

        private void SetIPAddress()
        {
            string str = tbxAgentName.Text.Trim();
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }
                IPAddress[] hostAddresses = Dns.GetHostAddresses(str);
                if (hostAddresses != null)
                {
                    IEnumerable<IPAddress> addressFamily = hostAddresses.Where((IPAddress v4Address) => v4Address.AddressFamily == AddressFamily.InterNetwork);
                    if (addressFamily.Any())
                    {
                        SetText(addressFamily.FirstOrDefault().ToString());
                        return;
                    }
                }
                SetText(string.Empty);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                SetText(string.Empty);
                Logging.LogExceptionToTextFileWithEventLogBackup(exception, $"An error occurred while fetching IP address of agent '{str}'.");
            }
        }

        private void SetText(string text)
        {
            if (!tbxAgentIP.InvokeRequired)
            {
                tbxAgentIP.Text = text;
                return;
            }
            SetTextCallback setTextCallback = SetText;
            object[] objArray = new object[1] { text };
            Invoke(setTextCallback, objArray);
        }

        private void tbxAgentName_Leave(object sender, EventArgs e)
        {
            if (tbxAgentName.IsModified)
            {
                new Thread(SetIPAddress).Start();
            }
        }

        public bool ValidateControls(string caption)
        {
            if (string.IsNullOrEmpty(tbxAgentName.Text.Trim()) && string.IsNullOrEmpty(tbxAgentIP.Text.Trim()))
            {
                FlatXtraMessageBox.Show("Agent name cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if (string.IsNullOrEmpty(tbxUserName.Text.Trim()))
            {
                FlatXtraMessageBox.Show("Username cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            if (string.IsNullOrEmpty(tbxPassword.Text))
            {
                FlatXtraMessageBox.Show("Password cannot be blank.", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            MachineName = tbxAgentName.Text.Trim();
            MachineIP = tbxAgentIP.Text.Trim();
            UserName = tbxUserName.Text.Trim();
            Password = tbxPassword.Text;
            return true;
        }
    }
}
