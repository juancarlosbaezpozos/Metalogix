using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Deploy Certificate")]
    public class TCDeployCertificate : AgentWizardTabbableControl
    {
        private IContainer components;

        private LabelControl lblDeployCertificateMessage;

        private TextEdit tbxCertificatePath;

        private SimpleButton btnDeployCertificate;

        private LabelControl lblErrorMessage;

        public TCDeployCertificate()
	{
		InitializeComponent();
	}

        private void btnDeployCertificate_Click(object sender, EventArgs e)
	{
		try
		{
			if (!File.Exists(tbxCertificatePath.Text))
			{
				string str = $"Certificate not found at given location.\nPlease make sure certificate exists at given location.";
				FlatXtraMessageBox.Show(str, "Certificate Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			IRemoteWorker remoteWorker = new RemoteWorker(AgentWizardTabbableControl.AgentDetails);
			string str1 = AgentHelper.DeployCertificate(remoteWorker, tbxCertificatePath.Text, AgentWizardTabbableControl.AgentDetails.CertificatePassword);
			if (str1.Contains("already exists"))
			{
				FlatXtraMessageBox.Show("Certificate already exists.", ControlName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				SetNextButtonState(isEnabled: true);
				btnDeployCertificate.Enabled = false;
				AgentWizardTabbableControl.AgentDetails.IsCertificateDeployed = true;
			}
			else if (!str1.Contains("added to store"))
			{
				string str2 = $"Error occurred while deploying certificate. Error: {str1}";
				FlatXtraMessageBox.Show(str2, ControlName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				SetNextButtonState(isEnabled: false);
			}
			else
			{
				FlatXtraMessageBox.Show("Certificate deployed successfully.", ControlName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				SetNextButtonState(isEnabled: true);
				btnDeployCertificate.Enabled = false;
				AgentWizardTabbableControl.AgentDetails.IsCertificateDeployed = true;
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while deploying certificate.", exception, ErrorIcon.Error);
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

        private void InitializeComponent()
	{
		this.btnDeployCertificate = new DevExpress.XtraEditors.SimpleButton();
		this.tbxCertificatePath = new DevExpress.XtraEditors.TextEdit();
		this.lblDeployCertificateMessage = new DevExpress.XtraEditors.LabelControl();
		this.lblErrorMessage = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.tbxCertificatePath.Properties).BeginInit();
		base.SuspendLayout();
		this.btnDeployCertificate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btnDeployCertificate.Location = new System.Drawing.Point(188, 84);
		this.btnDeployCertificate.Name = "btnDeployCertificate";
		this.btnDeployCertificate.Size = new System.Drawing.Size(160, 30);
		this.btnDeployCertificate.TabIndex = 2;
		this.btnDeployCertificate.Text = "Deploy Certificate";
		this.btnDeployCertificate.Click += new System.EventHandler(btnDeployCertificate_Click);
		this.tbxCertificatePath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxCertificatePath.Enabled = false;
		this.tbxCertificatePath.Location = new System.Drawing.Point(10, 44);
		this.tbxCertificatePath.Name = "tbxCertificatePath";
		this.tbxCertificatePath.Size = new System.Drawing.Size(503, 20);
		this.tbxCertificatePath.TabIndex = 1;
		this.lblDeployCertificateMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblDeployCertificateMessage.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblDeployCertificateMessage.Location = new System.Drawing.Point(10, 10);
		this.lblDeployCertificateMessage.Name = "lblDeployCertificateMessage";
		this.lblDeployCertificateMessage.Size = new System.Drawing.Size(247, 13);
		this.lblDeployCertificateMessage.TabIndex = 0;
		this.lblDeployCertificateMessage.Text = "Click to deploy the chosen certificate on this Agent.";
		this.lblErrorMessage.Location = new System.Drawing.Point(15, 369);
		this.lblErrorMessage.Name = "lblErrorMessage";
		this.lblErrorMessage.Size = new System.Drawing.Size(0, 13);
		this.lblErrorMessage.TabIndex = 3;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.lblErrorMessage);
		base.Controls.Add(this.btnDeployCertificate);
		base.Controls.Add(this.tbxCertificatePath);
		base.Controls.Add(this.lblDeployCertificateMessage);
		base.Name = "TCDeployCertificate";
		base.Size = new System.Drawing.Size(525, 140);
		((System.ComponentModel.ISupportInitialize)this.tbxCertificatePath.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        public override bool LoadUI()
	{
		tbxCertificatePath.Text = AgentWizardTabbableControl.AgentDetails.CertificateLocation.Trim();
		btnDeployCertificate.Enabled = !AgentWizardTabbableControl.AgentDetails.IsCertificateDeployed;
		SetNextButtonState(!btnDeployCertificate.Enabled);
		return true;
	}
    }
}
