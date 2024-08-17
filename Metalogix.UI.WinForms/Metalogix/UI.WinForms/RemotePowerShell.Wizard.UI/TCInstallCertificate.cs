using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    public class TCInstallCertificate : CollapsableControl
    {
        private IContainer components;

        private GroupControl gbxDeployCertificate;

        private LabelControl lblPassword;

        private LabelControl lblLocation;

        private TextEdit tbxPassword;

        private TextEdit tbxCertificatePath;

        private SimpleButton btnBrowse;

        public string CertificatePath { get; private set; }

        public string Password { get; private set; }

        public TCInstallCertificate()
	{
		InitializeComponent();
	}

        private void btnBrowse_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Title = Resources.TCCreateCertificate_FileDialogTitle,
			Filter = Resources.TCCreateCertificate_FileDialogFilter,
			AddExtension = true,
			CheckFileExists = true
		};
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			tbxCertificatePath.Text = openFileDialog.FileName;
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

        public void HideControls()
	{
		HideControl(gbxDeployCertificate);
	}

        private void InitializeComponent()
	{
		this.gbxDeployCertificate = new DevExpress.XtraEditors.GroupControl();
		this.lblPassword = new DevExpress.XtraEditors.LabelControl();
		this.lblLocation = new DevExpress.XtraEditors.LabelControl();
		this.tbxPassword = new DevExpress.XtraEditors.TextEdit();
		this.tbxCertificatePath = new DevExpress.XtraEditors.TextEdit();
		this.btnBrowse = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.gbxDeployCertificate).BeginInit();
		this.gbxDeployCertificate.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tbxCertificatePath.Properties).BeginInit();
		base.SuspendLayout();
		this.gbxDeployCertificate.Appearance.BackColor = System.Drawing.Color.White;
		this.gbxDeployCertificate.Appearance.Options.UseBackColor = true;
		this.gbxDeployCertificate.Controls.Add(this.lblPassword);
		this.gbxDeployCertificate.Controls.Add(this.lblLocation);
		this.gbxDeployCertificate.Controls.Add(this.tbxPassword);
		this.gbxDeployCertificate.Controls.Add(this.tbxCertificatePath);
		this.gbxDeployCertificate.Controls.Add(this.btnBrowse);
		this.gbxDeployCertificate.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gbxDeployCertificate.Location = new System.Drawing.Point(0, 0);
		this.gbxDeployCertificate.Name = "gbxDeployCertificate";
		this.gbxDeployCertificate.Size = new System.Drawing.Size(442, 97);
		this.gbxDeployCertificate.TabIndex = 67;
		this.gbxDeployCertificate.Text = "Deploy Certificate";
		this.lblPassword.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.lblPassword.Location = new System.Drawing.Point(19, 66);
		this.lblPassword.Name = "lblPassword";
		this.lblPassword.Size = new System.Drawing.Size(49, 13);
		this.lblPassword.TabIndex = 74;
		this.lblPassword.Text = "Password:";
		this.lblLocation.Location = new System.Drawing.Point(19, 37);
		this.lblLocation.Name = "lblLocation";
		this.lblLocation.Size = new System.Drawing.Size(44, 13);
		this.lblLocation.TabIndex = 73;
		this.lblLocation.Text = "Location:";
		this.tbxPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxPassword.Location = new System.Drawing.Point(95, 63);
		this.tbxPassword.Name = "tbxPassword";
		this.tbxPassword.Properties.PasswordChar = '*';
		this.tbxPassword.Size = new System.Drawing.Size(333, 20);
		this.tbxPassword.TabIndex = 72;
		this.tbxCertificatePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxCertificatePath.Enabled = false;
		this.tbxCertificatePath.Location = new System.Drawing.Point(95, 34);
		this.tbxCertificatePath.Name = "tbxCertificatePath";
		this.tbxCertificatePath.Size = new System.Drawing.Size(260, 20);
		this.tbxCertificatePath.TabIndex = 70;
		this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnBrowse.Location = new System.Drawing.Point(361, 33);
		this.btnBrowse.Name = "btnBrowse";
		this.btnBrowse.Size = new System.Drawing.Size(67, 20);
		this.btnBrowse.TabIndex = 71;
		this.btnBrowse.Text = "Browse...";
		this.btnBrowse.Click += new System.EventHandler(btnBrowse_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.gbxDeployCertificate);
		base.Name = "TCInstallCertificate";
		base.Size = new System.Drawing.Size(442, 97);
		((System.ComponentModel.ISupportInitialize)this.gbxDeployCertificate).EndInit();
		this.gbxDeployCertificate.ResumeLayout(false);
		this.gbxDeployCertificate.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.tbxPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tbxCertificatePath.Properties).EndInit();
		base.ResumeLayout(false);
	}

        public bool ValidateControls(string dialogName)
	{
		if (string.IsNullOrEmpty(tbxCertificatePath.Text.Trim()))
		{
			FlatXtraMessageBox.Show("Certificate location cannot be blank.", dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		if (string.IsNullOrEmpty(tbxPassword.Text))
		{
			FlatXtraMessageBox.Show("Password cannot be blank.", dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		if (!AgentHelper.IsPasswordContainsQuotes(tbxPassword.Text, dialogName))
		{
			tbxPassword.Focus();
			return false;
		}
		CertificatePath = tbxCertificatePath.Text.Trim();
		Password = tbxPassword.Text;
		return true;
	}
    }
}
