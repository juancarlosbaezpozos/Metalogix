using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Attributes;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Metabase;
using Metalogix.UI.WinForms.Properties;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    public class AgentWizard : XtraForm
    {
        private delegate void EnableBackButtonCallback(bool buttonState);

        private delegate void EnableNextButtonCallback(bool buttonState);

        private TCGettingStarted _tcGettingStarted = new TCGettingStarted();

        private TCProvisionAgentDB _tcProvisionAgentDB = new TCProvisionAgentDB();

        private TCConfigureMetabase _tcConfigureMetabase = new TCConfigureMetabase();

        private TCApplicationMappings _tcApplicationMappings = new TCApplicationMappings();

        private TCCreateCertificate _tcCreateCertificate = new TCCreateCertificate();

        private TCDownloadInstaller _tcDownloadInstaller = new TCDownloadInstaller();

        private TCConfigureAgents _tcConfigureAgents = new TCConfigureAgents();

        private TCPrerequisites _tcPrerequisites = new TCPrerequisites();

        private TCDeployCertificate _tcDeployCertificate = new TCDeployCertificate();

        private TCInstallAgent _tcInstallAgent = new TCInstallAgent();

        private TCSummary _tcSummary = new TCSummary();

        private bool _showCommitChangesPopUp;

        private int _tabsCount;

        private IContainer components;

        private TableLayoutPanel agentTableLayoutPanel;

        private XtraWizardControl agentWizardControl;

        private PanelControl pnlBottom;

        private SimpleButton btnDeployNew;

        private SimpleButton btnNext;

        private SimpleButton btnBack;

        private SimpleButton btnFinish;

        public AgentWizard(ActionConfigContext context)
	{
		InitializeComponent();
		LoadWizardTabs();
		if (context.ActionContext.Sources != null && context.ActionContext.Sources[0] is JobListFullControl)
		{
			_tcProvisionAgentDB.JobListFullControl = (JobListFullControl)context.ActionContext.Sources[0];
			_tcApplicationMappings.JobListFullControl = (JobListFullControl)context.ActionContext.Sources[0];
		}
	}

        private void AgentWizard_FormClosed(object sender, FormClosedEventArgs e)
	{
		if (_showCommitChangesPopUp && !_tcProvisionAgentDB.PreviousAgentDbConnectionString.Equals(JobsSettings.AdapterContext.ToInsecureString(), StringComparison.Ordinal))
		{
			FlatXtraMessageBox.Show(Resources.RestartApplicationWarningForFileSystemToDB, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

        private void AgentWizard_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (_showCommitChangesPopUp && agentWizardControl.GetSelectedTabIndex() != _tabsCount - 1)
		{
			if (FlatXtraMessageBox.Show(Resources.CancelAgentWizardMessage, Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
			{
				agentWizardControl.GoToSpecificTab(_tcSummary.Name);
				_tcSummary.LoadUI();
				btnFinish.Visible = true;
				btnBack.Visible = false;
				btnNext.Enabled = false;
				btnNext.Enabled = false;
			}
			e.Cancel = true;
		}
	}

        private void btnBack_Click(object sender, EventArgs e)
	{
		agentWizardControl.GoToPreviousTab();
		btnFinish.Visible = false;
		btnNext.Visible = true;
		btnNext.Enabled = true;
		if (agentWizardControl.GetSelectedTabIndex() == 0)
		{
			btnBack.Visible = false;
		}
	}

        private void btnDeployNew_Click(object sender, EventArgs e)
	{
		agentWizardControl.GoToSpecificTab(_tcConfigureAgents.Name);
		_tcConfigureAgents.Initialize();
		btnBack.Visible = true;
		btnBack.Enabled = true;
		btnDeployNew.Visible = false;
		btnNext.Visible = true;
		btnFinish.Visible = false;
	}

        private void btnFinish_Click(object sender, EventArgs e)
	{
		Close();
	}

        private void btnNext_Click(object sender, EventArgs e)
	{
		try
		{
			agentWizardControl.GoToNextTab();
			btnBack.Visible = true;
			int selectedTabIndex = agentWizardControl.GetSelectedTabIndex();
			if (selectedTabIndex == 2)
			{
				_showCommitChangesPopUp = true;
			}
			if (selectedTabIndex == _tabsCount - 1)
			{
				btnFinish.Visible = true;
				btnNext.Visible = false;
				btnDeployNew.Visible = true;
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			GlobalServices.ErrorHandler.HandleException("Error", $"Error: {exception.Message}", exception, ErrorIcon.Error);
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.AgentWizard));
		this.agentTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
		this.btnFinish = new DevExpress.XtraEditors.SimpleButton();
		this.btnDeployNew = new DevExpress.XtraEditors.SimpleButton();
		this.btnNext = new DevExpress.XtraEditors.SimpleButton();
		this.btnBack = new DevExpress.XtraEditors.SimpleButton();
		this.agentWizardControl = new Metalogix.UI.WinForms.Components.XtraWizardControl();
		this.agentTableLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pnlBottom).BeginInit();
		this.pnlBottom.SuspendLayout();
		base.SuspendLayout();
		this.agentTableLayoutPanel.ColumnCount = 1;
		this.agentTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.agentTableLayoutPanel.Controls.Add(this.pnlBottom, 0, 1);
		this.agentTableLayoutPanel.Controls.Add(this.agentWizardControl, 0, 0);
		this.agentTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.agentTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
		this.agentTableLayoutPanel.Name = "agentTableLayoutPanel";
		this.agentTableLayoutPanel.RowCount = 2;
		this.agentTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90f));
		this.agentTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.agentTableLayoutPanel.Size = new System.Drawing.Size(704, 485);
		this.agentTableLayoutPanel.TabIndex = 0;
		this.pnlBottom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.pnlBottom.Appearance.BackColor = System.Drawing.Color.White;
		this.pnlBottom.Appearance.Options.UseBackColor = true;
		this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pnlBottom.Controls.Add(this.btnFinish);
		this.pnlBottom.Controls.Add(this.btnDeployNew);
		this.pnlBottom.Controls.Add(this.btnNext);
		this.pnlBottom.Controls.Add(this.btnBack);
		this.pnlBottom.Location = new System.Drawing.Point(3, 439);
		this.pnlBottom.Name = "pnlBottom";
		this.pnlBottom.Size = new System.Drawing.Size(698, 43);
		this.pnlBottom.TabIndex = 3;
		this.btnFinish.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnFinish.Location = new System.Drawing.Point(615, 9);
		this.btnFinish.Name = "btnFinish";
		this.btnFinish.Size = new System.Drawing.Size(75, 23);
		this.btnFinish.TabIndex = 3;
		this.btnFinish.Text = "Finish";
		this.btnFinish.Visible = false;
		this.btnFinish.Click += new System.EventHandler(btnFinish_Click);
		this.btnDeployNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnDeployNew.Location = new System.Drawing.Point(528, 9);
		this.btnDeployNew.Name = "btnDeployNew";
		this.btnDeployNew.Size = new System.Drawing.Size(75, 23);
		this.btnDeployNew.TabIndex = 2;
		this.btnDeployNew.Text = "Deploy New";
		this.btnDeployNew.Visible = false;
		this.btnDeployNew.Click += new System.EventHandler(btnDeployNew_Click);
		this.btnNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnNext.Location = new System.Drawing.Point(615, 9);
		this.btnNext.Name = "btnNext";
		this.btnNext.Size = new System.Drawing.Size(75, 23);
		this.btnNext.TabIndex = 0;
		this.btnNext.Text = "Next >";
		this.btnNext.Click += new System.EventHandler(btnNext_Click);
		this.btnBack.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnBack.Location = new System.Drawing.Point(528, 9);
		this.btnBack.Name = "btnBack";
		this.btnBack.Size = new System.Drawing.Size(75, 23);
		this.btnBack.TabIndex = 1;
		this.btnBack.Text = "< Back";
		this.btnBack.Visible = false;
		this.btnBack.Click += new System.EventHandler(btnBack_Click);
		this.agentWizardControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.agentWizardControl.Location = new System.Drawing.Point(3, 3);
		this.agentWizardControl.Name = "agentWizardControl";
		this.agentWizardControl.Size = new System.Drawing.Size(698, 430);
		this.agentWizardControl.TabIndex = 1;
		base.AcceptButton = this.btnNext;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(704, 485);
		base.Controls.Add(this.agentTableLayoutPanel);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(720, 524);
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(720, 524);
		base.Name = "AgentWizard";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Configure Distributed Migration";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AgentWizard_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(AgentWizard_FormClosed);
		this.agentTableLayoutPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pnlBottom).EndInit();
		this.pnlBottom.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        public bool IsMetabaseTabSupported()
	{
		return (from attribute in ReflectionUtils.GetApplicationAttributesMultiple<ApplicationSettingAttribute>()
			where attribute.SettingType == typeof(MetabaseSettingsEditorSetting)
			select attribute).Any();
	}

        private void LoadWizardTabs()
	{
		int num = 1;
		int num1 = num++;
		agentWizardControl.AddTab(_tcGettingStarted, num1, isFirstChildNode: true);
		int num2 = num++;
		agentWizardControl.AddTab(_tcProvisionAgentDB, num2, isFirstChildNode: true);
		if (IsMetabaseTabSupported())
		{
			int num3 = num++;
			agentWizardControl.AddTab(_tcConfigureMetabase, num3, isFirstChildNode: true);
		}
		int num4 = num++;
		agentWizardControl.AddTab(_tcApplicationMappings, num4, isFirstChildNode: true);
		int num5 = num++;
		agentWizardControl.AddTab(_tcCreateCertificate, num5, isFirstChildNode: true);
		int num6 = num++;
		agentWizardControl.AddTab(_tcDownloadInstaller, num6, isFirstChildNode: true);
		int num7 = num++;
		agentWizardControl.AddTab(_tcConfigureAgents, num7, isFirstChildNode: true);
		agentWizardControl.AddTab(_tcPrerequisites, -1, isFirstChildNode: false);
		agentWizardControl.AddTab(_tcDeployCertificate, -1, isFirstChildNode: false);
		agentWizardControl.AddTab(_tcInstallAgent, -1, isFirstChildNode: false);
		agentWizardControl.AddTab(_tcSummary, num, isFirstChildNode: true);
		_tabsCount = agentWizardControl.GetTabsCount();
	}

        public void SetBackButtonState(bool isEnabled)
	{
		if (!btnBack.InvokeRequired)
		{
			btnBack.Enabled = isEnabled;
			return;
		}
		EnableBackButtonCallback enableBackButtonCallback = SetBackButtonState;
		object[] objArray = new object[1] { isEnabled };
		Invoke(enableBackButtonCallback, objArray);
	}

        public void SetNextButtonState(bool isEnabled)
	{
		if (!btnNext.InvokeRequired)
		{
			btnNext.Enabled = isEnabled;
			return;
		}
		EnableNextButtonCallback enableNextButtonCallback = SetNextButtonState;
		object[] objArray = new object[1] { isEnabled };
		Invoke(enableNextButtonCallback, objArray);
	}
    }
}
