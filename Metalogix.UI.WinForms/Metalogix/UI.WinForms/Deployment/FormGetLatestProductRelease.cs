using Metalogix;
using Metalogix.Deployment;
using Metalogix.Interfaces;
using Metalogix.Licensing.LicenseServer.Service;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Deployment
{
	public class FormGetLatestProductRelease : Form
	{
		private LatestProductReleaseInfo m_latestProductInfo;

		private System.Version m_actualVersion;

		private AutomaticUpdater m_updater;

		private IContainer components;

		private LinkLabel linkLabelLatestAvailableReleaseNotesUrl1;

		private RadioButton radioButtonUpdate;

		private RadioButton radioButtonRemindMeLater;

		private RadioButton radioButtonTurnOff;

		private Label labelPleaseSelectAction;

		private RadioButton radioButtonSkipThisVersion;

		private Label labelNewerNotAvailable2;

		private LinkLabel linkLabelMetalogixSales2;

		private Label labelYourMaintenaceExpired;

		private PictureBox pictureBox;

		private Label labelMain;

		private Label labelVersionInfo;

		private Button roundedButtonCancel;

		private Button roundedButtonOK;

		private LinkLabel linkLabelMetalogixSales3;

		private Panel panelDefaultReleaseNotes;

		private Panel panelMaintenanceExpiredNewerReleaseExist;

		private Label labelVersion;

		private Panel panelMaintenanceExpiredNoReleaseAvailable;

		private LinkLabel linkLabelReleaseNotesLatest2;

		private LinkLabel linkLabelLatestAvailableReleaseNotesUrl2;

		private LinkLabel linkLabelReleaseNotesLatest3;

		private Panel panel4;

		private Panel panelMainInformation;

		private Label label2;

		private Label label1;

		private LinkLabel linkLabel1;

		private Label label3;

		private Label label4;

		private Label labelVersion2;

		private Label label5;

		private Label label7;

		private Label label8;

		internal LinkLabel linkLabelInstallationFile;

		public FormGetLatestProductRelease(LatestProductReleaseInfo latestProductInfo, AutomaticUpdater updater)
		{
			if (latestProductInfo == null)
			{
				throw new ArgumentNullException("latestProductInfo");
			}
			if (updater == null)
			{
				throw new ArgumentNullException("updater");
			}
			this.m_actualVersion = Assembly.GetEntryAssembly().GetName().Version;
			this.m_latestProductInfo = latestProductInfo;
			this.m_updater = updater;
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FormGetLatestProductRelease_Load(object sender, EventArgs e)
		{
			if (this.m_updater.Settings.AutoUpdateSettings == AutomaticUpdaterSettings.AutoUpdateSettingType.TurnOffCompletely)
			{
				this.radioButtonTurnOff.Checked = true;
			}
			if (this.m_actualVersion != null)
			{
				this.labelMain.Text = Resources.LabelNewVersionDownload;
				if (!string.IsNullOrEmpty(this.m_latestProductInfo.Version))
				{
					if (this.m_updater.Settings.InstalledVersion >= this.m_updater.ServerVersion && this.m_updater.Settings.InstalledVersion < this.m_updater.ServerLatestVersion)
					{
						Label label = this.labelVersionInfo;
						string[] version = new string[] { Resources.Version, " ", this.m_latestProductInfo.LatestVersion, ", ", Resources.Released, " ", null, null };
						DateTime dateTime = DateTime.Parse(this.m_latestProductInfo.LatestReleaseDate);
						version[6] = dateTime.ToString("dd-MMM-yyyy");
						version[7] = " .";
						label.Text = string.Concat(version);
						this.labelMain.Text = Resources.LabelNewVersionDownloadNot;
						this.panelDefaultReleaseNotes.Visible = false;
						this.panelMaintenanceExpiredNewerReleaseExist.Visible = false;
						this.panelMaintenanceExpiredNoReleaseAvailable.Visible = true;
						this.radioButtonUpdate.Checked = false;
						this.radioButtonUpdate.Enabled = false;
						this.radioButtonRemindMeLater.Checked = true;
						return;
					}
					if (this.m_updater.ServerVersion >= new System.Version(this.m_latestProductInfo.LatestVersion))
					{
						Label label1 = this.labelVersionInfo;
						string[] str = new string[] { Resources.Version, " ", this.m_latestProductInfo.Version, ", ", Resources.Released, " ", null, null };
						DateTime dateTime1 = DateTime.Parse(this.m_latestProductInfo.RelaseDate);
						str[6] = dateTime1.ToString("dd-MMM-yyyy");
						str[7] = " .";
						label1.Text = string.Concat(str);
						this.panelDefaultReleaseNotes.Visible = true;
						this.panelMaintenanceExpiredNewerReleaseExist.Visible = false;
						this.panelMaintenanceExpiredNoReleaseAvailable.Visible = false;
						return;
					}
					Label label2 = this.labelVersionInfo;
					string[] strArrays = new string[] { Resources.Version, " ", this.m_latestProductInfo.Version, ", ", Resources.Released, " ", null, null };
					DateTime dateTime2 = DateTime.Parse(this.m_latestProductInfo.RelaseDate);
					strArrays[6] = dateTime2.ToString("dd-MMM-yyyy");
					strArrays[7] = " .";
					label2.Text = string.Concat(strArrays);
					this.panelDefaultReleaseNotes.Visible = false;
					this.panelMaintenanceExpiredNewerReleaseExist.Visible = true;
					this.panelMaintenanceExpiredNoReleaseAvailable.Visible = false;
					Label label3 = this.labelVersion;
					string[] moreRecentVersion = new string[] { Resources.MoreRecentVersion, " ", Resources.VersionSmall, " ", this.m_latestProductInfo.LatestVersion, ", ", Resources.WasReleased, " ", null, null };
					DateTime dateTime3 = DateTime.Parse(this.m_latestProductInfo.LatestReleaseDate);
					moreRecentVersion[8] = dateTime3.ToString("dd-MMM-yyyy");
					moreRecentVersion[9] = " .";
					label3.Text = string.Concat(moreRecentVersion);
					this.labelVersion2.Text = string.Concat(Resources.ForDetailsOnLatest, " ", this.m_latestProductInfo.LatestVersion, ",");
					return;
				}
				if (!string.IsNullOrEmpty(this.m_latestProductInfo.LatestVersion))
				{
					Label label4 = this.labelVersionInfo;
					string[] version1 = new string[] { Resources.Version, " ", this.m_latestProductInfo.LatestVersion, ", ", Resources.Released, " ", null, null };
					DateTime dateTime4 = DateTime.Parse(this.m_latestProductInfo.LatestReleaseDate);
					version1[6] = dateTime4.ToString("dd-MMM-yyyy");
					version1[7] = " .";
					label4.Text = string.Concat(version1);
					if (this.m_updater.Settings.InstalledVersion < new System.Version(this.m_latestProductInfo.LatestVersion))
					{
						this.labelMain.Text = Resources.LabelNewVersionDownloadNot;
						this.panelDefaultReleaseNotes.Visible = false;
						this.panelMaintenanceExpiredNewerReleaseExist.Visible = false;
						this.panelMaintenanceExpiredNoReleaseAvailable.Visible = true;
						this.radioButtonUpdate.Checked = false;
						this.radioButtonUpdate.Enabled = false;
						this.radioButtonRemindMeLater.Checked = true;
						return;
					}
					this.panelDefaultReleaseNotes.Visible = true;
					this.panelMaintenanceExpiredNewerReleaseExist.Visible = false;
					this.panelMaintenanceExpiredNoReleaseAvailable.Visible = false;
				}
			}
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormGetLatestProductRelease));
			this.linkLabelLatestAvailableReleaseNotesUrl1 = new LinkLabel();
			this.linkLabelInstallationFile = new LinkLabel();
			this.radioButtonUpdate = new RadioButton();
			this.radioButtonRemindMeLater = new RadioButton();
			this.radioButtonTurnOff = new RadioButton();
			this.labelPleaseSelectAction = new Label();
			this.radioButtonSkipThisVersion = new RadioButton();
			this.labelNewerNotAvailable2 = new Label();
			this.linkLabelMetalogixSales2 = new LinkLabel();
			this.labelYourMaintenaceExpired = new Label();
			this.pictureBox = new PictureBox();
			this.labelMain = new Label();
			this.labelVersionInfo = new Label();
			this.roundedButtonCancel = new Button();
			this.roundedButtonOK = new Button();
			this.linkLabelMetalogixSales3 = new LinkLabel();
			this.panelDefaultReleaseNotes = new Panel();
			this.label8 = new Label();
			this.label7 = new Label();
			this.panelMaintenanceExpiredNewerReleaseExist = new Panel();
			this.labelVersion2 = new Label();
			this.label5 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.labelVersion = new Label();
			this.linkLabelReleaseNotesLatest2 = new LinkLabel();
			this.linkLabelLatestAvailableReleaseNotesUrl2 = new LinkLabel();
			this.panelMaintenanceExpiredNoReleaseAvailable = new Panel();
			this.linkLabel1 = new LinkLabel();
			this.label2 = new Label();
			this.label1 = new Label();
			this.linkLabelReleaseNotesLatest3 = new LinkLabel();
			this.panel4 = new Panel();
			this.panelMainInformation = new Panel();
			((ISupportInitialize)this.pictureBox).BeginInit();
			this.panelDefaultReleaseNotes.SuspendLayout();
			this.panelMaintenanceExpiredNewerReleaseExist.SuspendLayout();
			this.panelMaintenanceExpiredNoReleaseAvailable.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panelMainInformation.SuspendLayout();
			base.SuspendLayout();
			this.linkLabelLatestAvailableReleaseNotesUrl1.AutoSize = true;
			this.linkLabelLatestAvailableReleaseNotesUrl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelLatestAvailableReleaseNotesUrl1.LinkArea = new LinkArea(15, 18);
			this.linkLabelLatestAvailableReleaseNotesUrl1.Location = new Point(46, 60);
			this.linkLabelLatestAvailableReleaseNotesUrl1.Name = "linkLabelLatestAvailableReleaseNotesUrl1";
			this.linkLabelLatestAvailableReleaseNotesUrl1.Size = new System.Drawing.Size(152, 17);
			this.linkLabelLatestAvailableReleaseNotesUrl1.TabIndex = 70;
			this.linkLabelLatestAvailableReleaseNotesUrl1.TabStop = true;
			this.linkLabelLatestAvailableReleaseNotesUrl1.Text = "please see the release notes.";
			this.linkLabelLatestAvailableReleaseNotesUrl1.UseCompatibleTextRendering = true;
			this.linkLabelLatestAvailableReleaseNotesUrl1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelReleaseNotesUrl_LinkClicked);
			this.linkLabelInstallationFile.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			this.linkLabelInstallationFile.AutoSize = true;
			this.linkLabelInstallationFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelInstallationFile.LinkArea = new LinkArea(21, 17);
			this.linkLabelInstallationFile.Location = new Point(47, 18);
			this.linkLabelInstallationFile.Name = "linkLabelInstallationFile";
			this.linkLabelInstallationFile.Size = new System.Drawing.Size(205, 17);
			this.linkLabelInstallationFile.TabIndex = 120;
			this.linkLabelInstallationFile.TabStop = true;
			this.linkLabelInstallationFile.Tag = "http://w2k3x32mgadev2/Documents/Migration_Manager_for_SharePoint_5_0_0024.zip";
			this.linkLabelInstallationFile.Text = "you can download the installer package.";
			this.linkLabelInstallationFile.UseCompatibleTextRendering = true;
			this.linkLabelInstallationFile.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelInstallationFile_LinkClicked);
			this.radioButtonUpdate.AutoSize = true;
			this.radioButtonUpdate.Checked = true;
			this.radioButtonUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.radioButtonUpdate.Location = new Point(65, 79);
			this.radioButtonUpdate.Name = "radioButtonUpdate";
			this.radioButtonUpdate.Size = new System.Drawing.Size(60, 17);
			this.radioButtonUpdate.TabIndex = 20;
			this.radioButtonUpdate.TabStop = true;
			this.radioButtonUpdate.Text = "Update";
			this.radioButtonUpdate.UseVisualStyleBackColor = true;
			this.radioButtonRemindMeLater.AutoSize = true;
			this.radioButtonRemindMeLater.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.radioButtonRemindMeLater.Location = new Point(65, 102);
			this.radioButtonRemindMeLater.Name = "radioButtonRemindMeLater";
			this.radioButtonRemindMeLater.Size = new System.Drawing.Size(101, 17);
			this.radioButtonRemindMeLater.TabIndex = 30;
			this.radioButtonRemindMeLater.Text = "Remind me later";
			this.radioButtonRemindMeLater.UseVisualStyleBackColor = true;
			this.radioButtonTurnOff.AutoSize = true;
			this.radioButtonTurnOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.radioButtonTurnOff.Location = new Point(65, 148);
			this.radioButtonTurnOff.Name = "radioButtonTurnOff";
			this.radioButtonTurnOff.Size = new System.Drawing.Size(155, 17);
			this.radioButtonTurnOff.TabIndex = 50;
			this.radioButtonTurnOff.Text = "Turn off automatic updating";
			this.radioButtonTurnOff.UseVisualStyleBackColor = true;
			this.labelPleaseSelectAction.AutoSize = true;
			this.labelPleaseSelectAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelPleaseSelectAction.Location = new Point(47, 54);
			this.labelPleaseSelectAction.Name = "labelPleaseSelectAction";
			this.labelPleaseSelectAction.Size = new System.Drawing.Size(160, 13);
			this.labelPleaseSelectAction.TabIndex = 101;
			this.labelPleaseSelectAction.Text = "Please select the desired action:";
			this.radioButtonSkipThisVersion.AutoSize = true;
			this.radioButtonSkipThisVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.radioButtonSkipThisVersion.Location = new Point(65, 125);
			this.radioButtonSkipThisVersion.Name = "radioButtonSkipThisVersion";
			this.radioButtonSkipThisVersion.Size = new System.Drawing.Size(102, 17);
			this.radioButtonSkipThisVersion.TabIndex = 40;
			this.radioButtonSkipThisVersion.Text = "Skip this version";
			this.radioButtonSkipThisVersion.UseVisualStyleBackColor = true;
			this.labelNewerNotAvailable2.AutoSize = true;
			this.labelNewerNotAvailable2.Location = new Point(47, 59);
			this.labelNewerNotAvailable2.Name = "labelNewerNotAvailable2";
			this.labelNewerNotAvailable2.Size = new System.Drawing.Size(351, 13);
			this.labelNewerNotAvailable2.TabIndex = 104;
			this.labelNewerNotAvailable2.Text = "However, the maintenance subscription for your license key has expired, ";
			this.linkLabelMetalogixSales2.AutoSize = true;
			this.linkLabelMetalogixSales2.LinkArea = new LinkArea(15, 18);
			this.linkLabelMetalogixSales2.Location = new Point(48, 114);
			this.linkLabelMetalogixSales2.Name = "linkLabelMetalogixSales2";
			this.linkLabelMetalogixSales2.Size = new System.Drawing.Size(161, 17);
			this.linkLabelMetalogixSales2.TabIndex = 100;
			this.linkLabelMetalogixSales2.TabStop = true;
			this.linkLabelMetalogixSales2.Text = "please contact Metalogix sales.";
			this.linkLabelMetalogixSales2.UseCompatibleTextRendering = true;
			this.linkLabelMetalogixSales2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelMetalogixSales2_LinkClicked);
			this.labelYourMaintenaceExpired.AutoSize = true;
			this.labelYourMaintenaceExpired.Location = new Point(47, 0);
			this.labelYourMaintenaceExpired.Name = "labelYourMaintenaceExpired";
			this.labelYourMaintenaceExpired.Size = new System.Drawing.Size(306, 13);
			this.labelYourMaintenaceExpired.TabIndex = 107;
			this.labelYourMaintenaceExpired.Text = "The maintenance subscription for your license key has expired, ";
			this.pictureBox.Image = (Image)componentResourceManager.GetObject("pictureBox.Image");
			this.pictureBox.Location = new Point(8, 4);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(32, 32);
			this.pictureBox.TabIndex = 109;
			this.pictureBox.TabStop = false;
			this.labelMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.labelMain.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 238);
			this.labelMain.ForeColor = Color.DarkSlateGray;
			this.labelMain.Location = new Point(46, 0);
			this.labelMain.Name = "labelMain";
			this.labelMain.Size = new System.Drawing.Size(437, 32);
			this.labelMain.TabIndex = 108;
			this.labelMain.Text = "A New version is available for download.";
			this.labelMain.TextAlign = ContentAlignment.MiddleLeft;
			this.labelVersionInfo.AutoSize = true;
			this.labelVersionInfo.Location = new Point(46, 23);
			this.labelVersionInfo.Name = "labelVersionInfo";
			this.labelVersionInfo.Size = new System.Drawing.Size(196, 13);
			this.labelVersionInfo.TabIndex = 110;
			this.labelVersionInfo.Text = "Version 5.1.2.3, Released 22/11/2012 .";
			this.roundedButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.roundedButtonCancel.Location = new Point(385, 5);
			this.roundedButtonCancel.Name = "roundedButtonCancel";
			this.roundedButtonCancel.Size = new System.Drawing.Size(75, 23);
			this.roundedButtonCancel.TabIndex = 11;
			this.roundedButtonCancel.Text = "Cancel";
			this.roundedButtonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.roundedButtonOK.Location = new Point(304, 5);
			this.roundedButtonOK.Name = "roundedButtonOK";
			this.roundedButtonOK.Size = new System.Drawing.Size(75, 23);
			this.roundedButtonOK.TabIndex = 10;
			this.roundedButtonOK.Text = "OK";
			this.roundedButtonOK.Click += new EventHandler(this.roundedButtonOK_Click);
			this.linkLabelMetalogixSales3.AutoSize = true;
			this.linkLabelMetalogixSales3.LinkArea = new LinkArea(15, 18);
			this.linkLabelMetalogixSales3.Location = new Point(48, 42);
			this.linkLabelMetalogixSales3.Name = "linkLabelMetalogixSales3";
			this.linkLabelMetalogixSales3.Size = new System.Drawing.Size(161, 17);
			this.linkLabelMetalogixSales3.TabIndex = 80;
			this.linkLabelMetalogixSales3.TabStop = true;
			this.linkLabelMetalogixSales3.Text = "please contact Metalogix sales.";
			this.linkLabelMetalogixSales3.UseCompatibleTextRendering = true;
			this.linkLabelMetalogixSales3.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelMetalogixSales3_LinkClicked);
			this.panelDefaultReleaseNotes.Controls.Add(this.label8);
			this.panelDefaultReleaseNotes.Controls.Add(this.label7);
			this.panelDefaultReleaseNotes.Controls.Add(this.linkLabelInstallationFile);
			this.panelDefaultReleaseNotes.Controls.Add(this.linkLabelLatestAvailableReleaseNotesUrl1);
			this.panelDefaultReleaseNotes.Dock = DockStyle.Top;
			this.panelDefaultReleaseNotes.Location = new Point(0, 477);
			this.panelDefaultReleaseNotes.Name = "panelDefaultReleaseNotes";
			this.panelDefaultReleaseNotes.Size = new System.Drawing.Size(476, 83);
			this.panelDefaultReleaseNotes.TabIndex = 115;
			this.label8.AutoSize = true;
			this.label8.Location = new Point(46, 46);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(303, 13);
			this.label8.TabIndex = 123;
			this.label8.Text = "For details on the latest Features and Fixes in the new version, ";
			this.label7.AutoSize = true;
			this.label7.Location = new Point(47, 3);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(317, 13);
			this.label7.TabIndex = 122;
			this.label7.Text = "If you would like to install and use the new version at a later date, ";
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.labelVersion2);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.label5);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.label4);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.label3);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.labelVersion);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.linkLabelMetalogixSales2);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.linkLabelReleaseNotesLatest2);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.labelNewerNotAvailable2);
			this.panelMaintenanceExpiredNewerReleaseExist.Controls.Add(this.linkLabelLatestAvailableReleaseNotesUrl2);
			this.panelMaintenanceExpiredNewerReleaseExist.Dock = DockStyle.Top;
			this.panelMaintenanceExpiredNewerReleaseExist.Location = new Point(0, 297);
			this.panelMaintenanceExpiredNewerReleaseExist.Name = "panelMaintenanceExpiredNewerReleaseExist";
			this.panelMaintenanceExpiredNewerReleaseExist.Size = new System.Drawing.Size(476, 180);
			this.panelMaintenanceExpiredNewerReleaseExist.TabIndex = 116;
			this.labelVersion2.AutoSize = true;
			this.labelVersion2.Location = new Point(48, 130);
			this.labelVersion2.Name = "labelVersion2";
			this.labelVersion2.Size = new System.Drawing.Size(295, 13);
			this.labelVersion2.TabIndex = 122;
			this.labelVersion2.Text = "For details on the latest Features and Fixes in version 5.1.2.3,";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(47, 100);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(336, 13);
			this.label5.TabIndex = 120;
			this.label5.Text = "To renew your maintenance subscription and install this newer version";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(47, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(223, 13);
			this.label4.TabIndex = 119;
			this.label4.Text = "and this build is not currently available to you. ";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(46, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(303, 13);
			this.label3.TabIndex = 118;
			this.label3.Text = "For details on the latest Features and Fixes in the new version, ";
			this.labelVersion.AutoSize = true;
			this.labelVersion.Location = new Point(47, 44);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(336, 13);
			this.labelVersion.TabIndex = 117;
			this.labelVersion.Text = "A more recent version, version 5.1.2.3, was released on 22/11/2012 .";
			this.linkLabelReleaseNotesLatest2.AutoSize = true;
			this.linkLabelReleaseNotesLatest2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelReleaseNotesLatest2.LinkArea = new LinkArea(15, 18);
			this.linkLabelReleaseNotesLatest2.Location = new Point(48, 144);
			this.linkLabelReleaseNotesLatest2.Name = "linkLabelReleaseNotesLatest2";
			this.linkLabelReleaseNotesLatest2.Size = new System.Drawing.Size(152, 17);
			this.linkLabelReleaseNotesLatest2.TabIndex = 110;
			this.linkLabelReleaseNotesLatest2.TabStop = true;
			this.linkLabelReleaseNotesLatest2.Text = "please see the release notes.";
			this.linkLabelReleaseNotesLatest2.UseCompatibleTextRendering = true;
			this.linkLabelReleaseNotesLatest2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelReleaseNotesLatest2_LinkClicked);
			this.linkLabelLatestAvailableReleaseNotesUrl2.AutoSize = true;
			this.linkLabelLatestAvailableReleaseNotesUrl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelLatestAvailableReleaseNotesUrl2.LinkArea = new LinkArea(15, 18);
			this.linkLabelLatestAvailableReleaseNotesUrl2.Location = new Point(46, 17);
			this.linkLabelLatestAvailableReleaseNotesUrl2.Name = "linkLabelLatestAvailableReleaseNotesUrl2";
			this.linkLabelLatestAvailableReleaseNotesUrl2.Size = new System.Drawing.Size(152, 17);
			this.linkLabelLatestAvailableReleaseNotesUrl2.TabIndex = 60;
			this.linkLabelLatestAvailableReleaseNotesUrl2.TabStop = true;
			this.linkLabelLatestAvailableReleaseNotesUrl2.Text = "please see the release notes.";
			this.linkLabelLatestAvailableReleaseNotesUrl2.UseCompatibleTextRendering = true;
			this.linkLabelLatestAvailableReleaseNotesUrl2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelLatestAvailableReleaseNotesUrl2_LinkClicked);
			this.panelMaintenanceExpiredNoReleaseAvailable.Controls.Add(this.linkLabel1);
			this.panelMaintenanceExpiredNoReleaseAvailable.Controls.Add(this.label2);
			this.panelMaintenanceExpiredNoReleaseAvailable.Controls.Add(this.label1);
			this.panelMaintenanceExpiredNoReleaseAvailable.Controls.Add(this.linkLabelReleaseNotesLatest3);
			this.panelMaintenanceExpiredNoReleaseAvailable.Controls.Add(this.linkLabelMetalogixSales3);
			this.panelMaintenanceExpiredNoReleaseAvailable.Controls.Add(this.labelYourMaintenaceExpired);
			this.panelMaintenanceExpiredNoReleaseAvailable.Dock = DockStyle.Top;
			this.panelMaintenanceExpiredNoReleaseAvailable.Location = new Point(0, 184);
			this.panelMaintenanceExpiredNoReleaseAvailable.Name = "panelMaintenanceExpiredNoReleaseAvailable";
			this.panelMaintenanceExpiredNoReleaseAvailable.Size = new System.Drawing.Size(476, 113);
			this.panelMaintenanceExpiredNoReleaseAvailable.TabIndex = 117;
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabel1.LinkArea = new LinkArea(15, 18);
			this.linkLabel1.Location = new Point(47, 84);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(152, 17);
			this.linkLabel1.TabIndex = 110;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "please see the release notes.";
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(47, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(329, 13);
			this.label2.TabIndex = 109;
			this.label2.Text = "To renew your maintenance subscription and install the new version ";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(47, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(223, 13);
			this.label1.TabIndex = 108;
			this.label1.Text = "and this build is not currently available to you. ";
			this.linkLabelReleaseNotesLatest3.AutoSize = true;
			this.linkLabelReleaseNotesLatest3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelReleaseNotesLatest3.LinkArea = new LinkArea(0, 0);
			this.linkLabelReleaseNotesLatest3.Location = new Point(47, 70);
			this.linkLabelReleaseNotesLatest3.Name = "linkLabelReleaseNotesLatest3";
			this.linkLabelReleaseNotesLatest3.Size = new System.Drawing.Size(303, 13);
			this.linkLabelReleaseNotesLatest3.TabIndex = 90;
			this.linkLabelReleaseNotesLatest3.Text = "For details on the latest Features and Fixes in the new version, ";
			this.linkLabelReleaseNotesLatest3.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelReleaseNotesLatest3_LinkClicked);
			this.panel4.Controls.Add(this.roundedButtonCancel);
			this.panel4.Controls.Add(this.roundedButtonOK);
			this.panel4.Dock = DockStyle.Top;
			this.panel4.Location = new Point(0, 560);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(476, 36);
			this.panel4.TabIndex = 118;
			this.panelMainInformation.Controls.Add(this.pictureBox);
			this.panelMainInformation.Controls.Add(this.radioButtonTurnOff);
			this.panelMainInformation.Controls.Add(this.radioButtonSkipThisVersion);
			this.panelMainInformation.Controls.Add(this.labelVersionInfo);
			this.panelMainInformation.Controls.Add(this.labelPleaseSelectAction);
			this.panelMainInformation.Controls.Add(this.radioButtonRemindMeLater);
			this.panelMainInformation.Controls.Add(this.labelMain);
			this.panelMainInformation.Controls.Add(this.radioButtonUpdate);
			this.panelMainInformation.Dock = DockStyle.Top;
			this.panelMainInformation.Location = new Point(0, 0);
			this.panelMainInformation.Name = "panelMainInformation";
			this.panelMainInformation.Size = new System.Drawing.Size(476, 184);
			this.panelMainInformation.TabIndex = 119;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			base.ClientSize = new System.Drawing.Size(476, 596);
			base.Controls.Add(this.panel4);
			base.Controls.Add(this.panelDefaultReleaseNotes);
			base.Controls.Add(this.panelMaintenanceExpiredNewerReleaseExist);
			base.Controls.Add(this.panelMaintenanceExpiredNoReleaseAvailable);
			base.Controls.Add(this.panelMainInformation);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(480, 100);
			base.Name = "FormGetLatestProductRelease";
			base.ShowIcon = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Product Update Information";
			base.Load += new EventHandler(this.FormGetLatestProductRelease_Load);
			((ISupportInitialize)this.pictureBox).EndInit();
			this.panelDefaultReleaseNotes.ResumeLayout(false);
			this.panelDefaultReleaseNotes.PerformLayout();
			this.panelMaintenanceExpiredNewerReleaseExist.ResumeLayout(false);
			this.panelMaintenanceExpiredNewerReleaseExist.PerformLayout();
			this.panelMaintenanceExpiredNoReleaseAvailable.ResumeLayout(false);
			this.panelMaintenanceExpiredNoReleaseAvailable.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panelMainInformation.ResumeLayout(false);
			this.panelMainInformation.PerformLayout();
			base.ResumeLayout(false);
		}

		private void linkLabelInstallationFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenLatestAvailableInstallationFile();
		}

		private void linkLabelLatestAvailableReleaseNotesUrl2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenLatestAvailableReleaseNotes();
		}

		private void linkLabelMetalogixSales2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenMetalogixSales();
		}

		private void linkLabelMetalogixSales3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenMetalogixSales();
		}

		private void linkLabelReleaseNotesLatest2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenLatestReleaseNotes();
		}

		private void linkLabelReleaseNotesLatest3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenLatestReleaseNotes();
		}

		private void linkLabelReleaseNotesUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OpenLatestAvailableReleaseNotes();
		}

		private void OnOKButtonClick(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			if (this.radioButtonRemindMeLater.Checked)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
			if (this.radioButtonSkipThisVersion.Checked)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			}
			if (this.radioButtonTurnOff.Checked)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.No;
			}
			if (this.radioButtonSkipThisVersion.Checked)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			}
			base.Close();
		}

		private void OpenLatestAvailableInstallationFile()
		{
			if (this.m_latestProductInfo != null && !string.IsNullOrEmpty(this.m_latestProductInfo.InstallationFileUrl))
			{
				this.OpenUrl(this.m_latestProductInfo.InstallationFileUrl);
				return;
			}
			GlobalServices.ErrorHandler.HandleException("Error", "Can't open url for latest available installation file URL !", ErrorIcon.Error);
		}

		private void OpenLatestAvailableReleaseNotes()
		{
			if (this.m_latestProductInfo != null && !string.IsNullOrEmpty(this.m_latestProductInfo.ReleaseNotesUrl))
			{
				this.OpenUrl(this.m_latestProductInfo.ReleaseNotesUrl);
				return;
			}
			GlobalServices.ErrorHandler.HandleException("Error", "Can't open url for latest available release notes !", ErrorIcon.Error);
		}

		private void OpenLatestReleaseNotes()
		{
			if (this.m_latestProductInfo != null && !string.IsNullOrEmpty(this.m_latestProductInfo.LatestVersionReleaseNotesUrl))
			{
				this.OpenUrl(this.m_latestProductInfo.LatestVersionReleaseNotesUrl);
				return;
			}
			GlobalServices.ErrorHandler.HandleException("Error", "Can't open url for latest release notes !", ErrorIcon.Error);
		}

		private void OpenMetalogixSales()
		{
			string metalogixSalesURL = Resources.MetalogixSalesURL;
			if (string.IsNullOrEmpty(metalogixSalesURL))
			{
				return;
			}
			this.OpenUrl(metalogixSalesURL);
		}

		private void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException(string.Concat("Can't open url ", url), exception);
			}
		}

		private void roundedButtonOK_Click(object sender, EventArgs e)
		{
			this.OnOKButtonClick(sender, e);
		}
	}
}