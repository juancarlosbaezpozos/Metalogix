using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Configure Agents")]
    public class TCConfigureAgents : AgentWizardTabbableControl
    {
        private IContainer components;

        private GroupControl gbxConfigureAgent;

        private RichTextBox tbxMessage;

        private AddEditAgentControl agentControl;

        public TCConfigureAgents()
	{
		InitializeComponent();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        public void Initialize()
	{
		agentControl.Initialize();
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCConfigureAgents));
		this.tbxMessage = new System.Windows.Forms.RichTextBox();
		this.gbxConfigureAgent = new DevExpress.XtraEditors.GroupControl();
		this.agentControl = new Metalogix.UI.WinForms.Components.AddEditAgentControl();
		((System.ComponentModel.ISupportInitialize)this.gbxConfigureAgent).BeginInit();
		this.gbxConfigureAgent.SuspendLayout();
		base.SuspendLayout();
		this.tbxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxMessage.BackColor = System.Drawing.Color.White;
		this.tbxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxMessage.Location = new System.Drawing.Point(10, 10);
		this.tbxMessage.Name = "tbxMessage";
		this.tbxMessage.ReadOnly = true;
		this.tbxMessage.Size = new System.Drawing.Size(500, 130);
		this.tbxMessage.TabIndex = 1;
		this.tbxMessage.Text = componentResourceManager.GetString("tbxMessage.Text");
		this.gbxConfigureAgent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gbxConfigureAgent.Appearance.BackColor = System.Drawing.Color.White;
		this.gbxConfigureAgent.Appearance.Options.UseBackColor = true;
		this.gbxConfigureAgent.Controls.Add(this.agentControl);
		this.gbxConfigureAgent.Location = new System.Drawing.Point(13, 144);
		this.gbxConfigureAgent.Name = "gbxConfigureAgent";
		this.gbxConfigureAgent.Size = new System.Drawing.Size(500, 262);
		this.gbxConfigureAgent.TabIndex = 0;
		this.gbxConfigureAgent.Text = "Configure Agent";
		this.agentControl.Appearance.BackColor = System.Drawing.Color.White;
		this.agentControl.Appearance.Options.UseBackColor = true;
		this.agentControl.Location = new System.Drawing.Point(17, 29);
		this.agentControl.Margin = new System.Windows.Forms.Padding(10);
		this.agentControl.Name = "agentControl";
		this.agentControl.Size = new System.Drawing.Size(448, 219);
		this.agentControl.TabIndex = 3;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tbxMessage);
		base.Controls.Add(this.gbxConfigureAgent);
		base.Name = "TCConfigureAgents";
		base.Size = new System.Drawing.Size(525, 426);
		((System.ComponentModel.ISupportInitialize)this.gbxConfigureAgent).EndInit();
		this.gbxConfigureAgent.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        private bool IsAgentConnectable(IRemoteWorker worker)
	{
		if (worker.RunCommand(string.Format("(Get-Service -ServiceName \"{0}\").Status", "Server")).Equals("Running", StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		FlatXtraMessageBox.Show("Unable to connect to Agent. This may be because of one of the following reasons:\n\n1. Agent name is invalid.\n2. Agent is not up and running.\n3. Invalid credentials.\n4. \"Server\" service is not running.", ControlName);
		return false;
	}

        public override bool ValidatePage()
	{
		bool flag;
		try
		{
			if (agentControl.ValidateControls(ControlName))
			{
				if (string.IsNullOrEmpty(agentControl.MachineName))
				{
					agentControl.MachineName = agentControl.MachineIP;
				}
				if (!AgentHelper.IsExistingAgent(AgentWizardTabbableControl.Agents.GetList(), agentControl.MachineName))
				{
					AgentWizardTabbableControl.AgentDetails.AgentID = Guid.NewGuid();
					AgentWizardTabbableControl.AgentDetails.IsCertificateDeployed = false;
					AgentWizardTabbableControl.AgentDetails.MachineIP = agentControl.MachineIP;
					AgentWizardTabbableControl.AgentDetails.MachineName = agentControl.MachineName;
					AgentWizardTabbableControl.AgentDetails.UserName = agentControl.UserName;
					AgentWizardTabbableControl.AgentDetails.Password = agentControl.Password;
					IRemoteWorker remoteWorker = new RemoteWorker(AgentWizardTabbableControl.AgentDetails);
					remoteWorker.AddRemoveCredentials($"/add:{AgentWizardTabbableControl.AgentDetails.MachineName} /user:{AgentWizardTabbableControl.AgentDetails.UserName} /pass:{AgentWizardTabbableControl.AgentDetails.Password}");
					if (IsAgentConnectable(remoteWorker))
					{
						return true;
					}
					remoteWorker.AddRemoveCredentials($"/delete:{AgentWizardTabbableControl.AgentDetails.MachineName}");
				}
			}
			flag = false;
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while validating agent details..", exception, ErrorIcon.Error);
			flag = false;
		}
		return flag;
	}
    }
}
