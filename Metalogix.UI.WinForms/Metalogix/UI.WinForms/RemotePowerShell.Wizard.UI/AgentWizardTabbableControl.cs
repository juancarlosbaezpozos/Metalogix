using Metalogix.Actions.Remoting;
using Metalogix.Actions.Remoting.Database;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Components;
using Metalogix.Utilities;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
	public class AgentWizardTabbableControl : XtraWizardTabbableControl
	{
		private static Metalogix.Actions.Remoting.AgentDetails _instance;

		private static AgentCollection _agents;

		private IContainer components;

		protected static Metalogix.Actions.Remoting.AgentDetails AgentDetails
		{
			get
			{
				if (AgentWizardTabbableControl._instance == null)
				{
					AgentWizardTabbableControl._instance = new Metalogix.Actions.Remoting.AgentDetails();
				}
				return AgentWizardTabbableControl._instance;
			}
			private set
			{
				AgentWizardTabbableControl._instance = value;
			}
		}

		protected static AgentCollection Agents
		{
			get
			{
				if (AgentWizardTabbableControl._agents == null)
				{
					IAgentDb agentDb = new AgentDb(JobsSettings.AdapterContext.ToInsecureString());
					AgentWizardTabbableControl._agents = new AgentCollection(agentDb);
					AgentWizardTabbableControl._agents.FetchData();
				}
				return AgentWizardTabbableControl._agents;
			}
		}

		public AgentWizardTabbableControl()
		{
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

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		public void Reset()
		{
			AgentWizardTabbableControl._agents = null;
			AgentWizardTabbableControl.AgentDetails = null;
		}

		public void SetBackButtonState(bool isEnabled)
		{
			if (base.TopLevelControl != null && base.TopLevelControl is AgentWizard)
			{
				((AgentWizard)base.TopLevelControl).SetBackButtonState(isEnabled);
			}
		}

		public void SetNextButtonState(bool isEnabled)
		{
			if (base.TopLevelControl != null && base.TopLevelControl is AgentWizard)
			{
				((AgentWizard)base.TopLevelControl).SetNextButtonState(isEnabled);
			}
		}
	}
}