using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Install Agent")]
    public class TCInstallAgent : AgentWizardTabbableControl
    {
        private delegate void SetMemoStatusCallback(object sender, EventArgs e);

        private delegate void SetMemoTextCallback(object sender, EventArgs e);

        private BackgroundWorker _deployCMWorker;

        private Agent _agent;

        private IContainer components;

        private PanelControl pnlInstallAgent;

        private SimpleButton btnDeployCM;

        private MemoEdit memoDownloadStatus;

        private LabelControl lblDeployCMMessage;

        private MarqueeBar progressBar;

        public TCInstallAgent()
	{
		InitializeComponent();
		progressBar.Hide();
	}

        private void _deployCMWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		new AddAgentAction().ConfigureAgent(_agent, copyLicenseFile: true);
	}

        private void _deployCMWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			if (_agent != null)
			{
				Agent remoteContextFromId = AgentWizardTabbableControl.Agents.GetRemoteContextFromId(_agent.AgentID);
				if (remoteContextFromId != null && remoteContextFromId.Status == AgentStatus.Error)
				{
					btnDeployCM.Enabled = true;
				}
			}
		}
		finally
		{
			progressBar.Hide();
			SetNextButtonState(isEnabled: true);
		}
	}

        private void btnDeployCM_Click(object sender, EventArgs e)
	{
		AgentWizardTabbableControl.Agents.AgentListChanged += SetAgentLog;
		SetBackButtonState(isEnabled: false);
		Agent remoteContextFromId = AgentWizardTabbableControl.Agents.GetRemoteContextFromId(AgentWizardTabbableControl.AgentDetails.AgentID);
		if (remoteContextFromId != null)
		{
			_agent = remoteContextFromId;
		}
		else
		{
			GetAgentDetails();
			_agent = AgentWizardTabbableControl.Agents.Add(new Agent(AgentWizardTabbableControl.AgentDetails.AgentID, AgentWizardTabbableControl.AgentDetails.MachineIP, AgentWizardTabbableControl.AgentDetails.MachineName, AgentWizardTabbableControl.AgentDetails.OSVersion, AgentWizardTabbableControl.AgentDetails.CMVersion, AgentWizardTabbableControl.AgentDetails.UserName, AgentWizardTabbableControl.AgentDetails.Password, AgentWizardTabbableControl.AgentDetails.Status, AgentWizardTabbableControl.AgentDetails.Details));
		}
		_agent.Parent = AgentWizardTabbableControl.Agents;
		progressBar.Show();
		btnDeployCM.Enabled = false;
		DeployCM();
	}

        private void DeployCM()
	{
		if (_deployCMWorker != null)
		{
			_deployCMWorker.CancelAsync();
		}
		_deployCMWorker = new BackgroundWorker
		{
			WorkerSupportsCancellation = true
		};
		_deployCMWorker.DoWork += _deployCMWorker_DoWork;
		_deployCMWorker.RunWorkerCompleted += _deployCMWorker_RunWorkerCompleted;
		_deployCMWorker.RunWorkerAsync();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void GetAgentDetails()
	{
		AgentWizardTabbableControl.AgentDetails.AgentID = Guid.NewGuid();
		AgentWizardTabbableControl.AgentDetails.OSVersion = string.Empty;
		AgentWizardTabbableControl.AgentDetails.CMVersion = string.Empty;
		AgentDetails agentDetails = AgentWizardTabbableControl.AgentDetails;
		List<KeyValuePair<DateTime, string>> keyValuePairs = new List<KeyValuePair<DateTime, string>>
		{
			new KeyValuePair<DateTime, string>(DateTime.Now, "Configuration Started.")
		};
		agentDetails.Details = keyValuePairs;
		AgentWizardTabbableControl.AgentDetails.Status = AgentStatus.Configuring;
		AgentWizardTabbableControl.AgentDetails.Parent = AgentWizardTabbableControl.Agents;
	}

        private string GetAgentLogDetails(List<KeyValuePair<DateTime, string>> logDetails)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<DateTime, string> logDetail in logDetails)
		{
			stringBuilder.Append(logDetail.Key).Append(": ").AppendLine(logDetail.Value)
				.AppendLine();
		}
		return stringBuilder.ToString();
	}

        private void InitializeComponent()
	{
		this.pnlInstallAgent = new DevExpress.XtraEditors.PanelControl();
		this.progressBar = new Metalogix.UI.WinForms.Components.MarqueeBar();
		this.memoDownloadStatus = new DevExpress.XtraEditors.MemoEdit();
		this.btnDeployCM = new DevExpress.XtraEditors.SimpleButton();
		this.lblDeployCMMessage = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.pnlInstallAgent).BeginInit();
		this.pnlInstallAgent.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memoDownloadStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.pnlInstallAgent.Appearance.BackColor = System.Drawing.Color.White;
		this.pnlInstallAgent.Appearance.Options.UseBackColor = true;
		this.pnlInstallAgent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pnlInstallAgent.Controls.Add(this.progressBar);
		this.pnlInstallAgent.Controls.Add(this.memoDownloadStatus);
		this.pnlInstallAgent.Controls.Add(this.btnDeployCM);
		this.pnlInstallAgent.Controls.Add(this.lblDeployCMMessage);
		this.pnlInstallAgent.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnlInstallAgent.Location = new System.Drawing.Point(0, 0);
		this.pnlInstallAgent.Name = "pnlInstallAgent";
		this.pnlInstallAgent.Size = new System.Drawing.Size(525, 385);
		this.pnlInstallAgent.TabIndex = 0;
		this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressBar.Location = new System.Drawing.Point(8, 353);
		this.progressBar.Name = "progressBar";
		this.progressBar.Size = new System.Drawing.Size(75, 23);
		this.progressBar.TabIndex = 3;
		this.progressBar.Text = "marqueeBar1";
		this.memoDownloadStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memoDownloadStatus.Location = new System.Drawing.Point(8, 122);
		this.memoDownloadStatus.Name = "memoDownloadStatus";
		this.memoDownloadStatus.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.memoDownloadStatus.Properties.Appearance.Options.UseBackColor = true;
		this.memoDownloadStatus.Properties.ReadOnly = true;
		this.memoDownloadStatus.Size = new System.Drawing.Size(509, 219);
		this.memoDownloadStatus.TabIndex = 2;
		this.memoDownloadStatus.TextChanged += new System.EventHandler(memoDownloadStatus_TextChanged);
		this.btnDeployCM.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btnDeployCM.Location = new System.Drawing.Point(185, 48);
		this.btnDeployCM.Name = "btnDeployCM";
		this.btnDeployCM.Size = new System.Drawing.Size(160, 30);
		this.btnDeployCM.TabIndex = 0;
		this.btnDeployCM.Text = "Deploy Content Matrix";
		this.btnDeployCM.Click += new System.EventHandler(btnDeployCM_Click);
		this.lblDeployCMMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lblDeployCMMessage.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblDeployCMMessage.Location = new System.Drawing.Point(10, 10);
		this.lblDeployCMMessage.Name = "lblDeployCMMessage";
		this.lblDeployCMMessage.Size = new System.Drawing.Size(255, 13);
		this.lblDeployCMMessage.TabIndex = 0;
		this.lblDeployCMMessage.Text = "Click to deploy Content Matrix to the selected Agent.";
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.pnlInstallAgent);
		base.Name = "TCInstallAgent";
		base.Size = new System.Drawing.Size(525, 385);
		((System.ComponentModel.ISupportInitialize)this.pnlInstallAgent).EndInit();
		this.pnlInstallAgent.ResumeLayout(false);
		this.pnlInstallAgent.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.memoDownloadStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}

        public override bool LoadUI()
	{
		btnDeployCM.Enabled = true;
		memoDownloadStatus.Text = string.Empty;
		SetNextButtonState(isEnabled: false);
		return true;
	}

        private void memoDownloadStatus_TextChanged(object sender, EventArgs e)
	{
		if (!memoDownloadStatus.InvokeRequired)
		{
			memoDownloadStatus.SelectionStart = memoDownloadStatus.Text.Length;
			memoDownloadStatus.ScrollToCaret();
			return;
		}
		SetMemoStatusCallback setMemoStatusCallback = memoDownloadStatus_TextChanged;
		object[] objArray = new object[2] { sender, e };
		Invoke(setMemoStatusCallback, objArray);
	}

        private void SetAgentLog(object sender, EventArgs e)
	{
		if (!memoDownloadStatus.InvokeRequired)
		{
			memoDownloadStatus.DeselectAll();
			memoDownloadStatus.Text = GetAgentLogDetails(AgentWizardTabbableControl.Agents.GetAgentLogDetails(AgentWizardTabbableControl.AgentDetails.AgentID, isLatestEntry: false, sortAsc: true));
			return;
		}
		SetMemoTextCallback setMemoTextCallback = SetAgentLog;
		object[] objArray = new object[2] { sender, e };
		Invoke(setMemoTextCallback, objArray);
	}
    }
}
