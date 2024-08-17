using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Download Installer")]
    public class TCDownloadInstaller : AgentWizardTabbableControl
    {
        private string _installerSiteUrl;

        private string _installerLocalPath;

        private BackgroundWorker _downloadInstallerWorker;

        private IContainer components;

        private MarqueeBar progressBar;

        private SimpleButton btnDownloadInstaller;

        private RichTextBox tbxMessage;

        public TCDownloadInstaller()
	{
		InitializeComponent();
		InitializeMessage();
	}

        private void _downloadInstallerWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			AgentHelper.DownloadInstaller(_installerSiteUrl, _installerLocalPath);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while downloading installer.", exception, ErrorIcon.Error);
		}
	}

        private void _downloadInstallerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		UpdateUI(isEnabled: true);
	}

        private void btnDownloadInstaller_Click(object sender, EventArgs e)
	{
		bool flag = false;
		if (File.Exists(_installerLocalPath))
		{
			flag = true;
			if (FlatXtraMessageBox.Show($"Installer already exists at '{_installerLocalPath}'.\nWould you like to overwrite it?", "Installer Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				File.Delete(_installerLocalPath);
				flag = false;
			}
		}
		if (!flag)
		{
			UpdateUI(isEnabled: false);
			DownloadInstaller();
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

        private void DownloadInstaller()
	{
		if (_downloadInstallerWorker != null)
		{
			_downloadInstallerWorker.CancelAsync();
		}
		_downloadInstallerWorker = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		_downloadInstallerWorker.DoWork += _downloadInstallerWorker_DoWork;
		_downloadInstallerWorker.RunWorkerCompleted += _downloadInstallerWorker_RunWorkerCompleted;
		_downloadInstallerWorker.RunWorkerAsync();
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCDownloadInstaller));
		this.progressBar = new Metalogix.UI.WinForms.Components.MarqueeBar();
		this.btnDownloadInstaller = new DevExpress.XtraEditors.SimpleButton();
		this.tbxMessage = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressBar.Location = new System.Drawing.Point(6, 308);
		this.progressBar.Name = "progressBar";
		this.progressBar.Size = new System.Drawing.Size(75, 23);
		this.progressBar.TabIndex = 5;
		this.progressBar.Text = "progressBar";
		this.progressBar.Visible = false;
		this.btnDownloadInstaller.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btnDownloadInstaller.Location = new System.Drawing.Point(191, 248);
		this.btnDownloadInstaller.Name = "btnDownloadInstaller";
		this.btnDownloadInstaller.Size = new System.Drawing.Size(160, 30);
		this.btnDownloadInstaller.TabIndex = 3;
		this.btnDownloadInstaller.Text = "Download Installer";
		this.btnDownloadInstaller.Click += new System.EventHandler(btnDownloadInstaller_Click);
		this.tbxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxMessage.BackColor = System.Drawing.Color.White;
		this.tbxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxMessage.DetectUrls = false;
		this.tbxMessage.Location = new System.Drawing.Point(10, 10);
		this.tbxMessage.Name = "tbxMessage";
		this.tbxMessage.ReadOnly = true;
		this.tbxMessage.Size = new System.Drawing.Size(500, 212);
		this.tbxMessage.TabIndex = 4;
		this.tbxMessage.Text = componentResourceManager.GetString("tbxMessage.Text");
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.progressBar);
		base.Controls.Add(this.btnDownloadInstaller);
		base.Controls.Add(this.tbxMessage);
		base.Name = "TCDownloadInstaller";
		base.Size = new System.Drawing.Size(525, 335);
		base.ResumeLayout(false);
	}

        private void InitializeMessage()
	{
		_installerSiteUrl = AgentHelper.GetInstallerUrl();
		_installerLocalPath = Path.Combine(ApplicationData.CommonDataPath, Path.GetFileName(_installerSiteUrl));
		tbxMessage.Text = string.Format(tbxMessage.Text, ApplicationData.CommonDataPath, _installerSiteUrl);
	}

        private void UpdateUI(bool isEnabled)
	{
		btnDownloadInstaller.Enabled = isEnabled;
		SetNextButtonState(isEnabled);
		SetBackButtonState(isEnabled);
		progressBar.Visible = !isEnabled;
	}

        public override bool ValidatePage()
	{
		if (File.Exists(_installerLocalPath))
		{
			return true;
		}
		string str = $"Installer not found at '{_installerLocalPath}'.\nPlease click the ‘Download Installer’ button or manually download the executable and place it at the listed path.";
		FlatXtraMessageBox.Show(str, "Installer Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		return false;
	}
    }
}
