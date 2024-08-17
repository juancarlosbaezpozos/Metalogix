using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.RemotePowerShell.Options;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.UpdateAgent16.png")]
	[MenuText("Update Agent {2-Update Agent}")]
	[Name("Update Agent")]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.Zero)]
	public class UpdateAgentAction : AddUpdateAgentBaseAction<UpdateAgentOptions>
	{
		public UpdateAgentAction()
		{
		}

		public override bool EnabledOn(IXMLAbleList source, IXMLAbleList target)
		{
			if (target == null)
			{
				return false;
			}
			return target.Cast<Agent>().Any<Agent>((Agent agent) => agent.Status == AgentStatus.Available);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target != null && target.Count > 0 && target[0] is Agent)
			{
				Agent item = (Agent)target[0];
				item.Parent.UpdateStatus(item, AgentStatus.Configuring);
				if (this.ActionOptions.IsUpdateContentMatrix)
				{
					if (!item.CMVersion.Equals(Application.ProductVersion, StringComparison.OrdinalIgnoreCase))
					{
						(new Thread(() => this.UpdateCM(item))).Start();
						return;
					}
					(new Thread(() => this.UpdateLicenseFile(item))).Start();
					LogHelper.LogMessage(item, "Content Matrix Consoles with same version is already installed.");
					item.Parent.UpdateStatus(item, AgentStatus.Available);
					return;
				}
				(new Thread(() => this.UpdateMappingFiles(item))).Start();
			}
		}

		private void UpdateCM(Agent agent)
		{
			LogHelper.LogMessage(agent, "Content Matrix Consoles update started.");
			base.ConfigureAgent(agent, true, true);
			LogHelper.LogMessage(agent, "Content Matrix Consoles update completed.");
			agent.Parent.UpdateLatestLogOnUI(agent);
		}

		private void UpdateLicenseFile(Agent agent)
		{
			LogHelper.LogMessage(agent, "License File update started.");
			IRemoteWorker remoteWorker = new RemoteWorker(agent);
			this.agentInfo = agent;
			base.CopyLicenseFile(remoteWorker, remoteWorker.GetSystemFolderPath(Environment.SpecialFolder.CommonApplicationData));
			LogHelper.LogMessage(agent, "License File update completed.");
			agent.Parent.UpdateLatestLogOnUI(agent);
		}

		private void UpdateMappingFiles(Agent agent)
		{
			LogHelper.LogMessage(agent, "Application Mappings update started.");
			base.CopyMappingFiles(new RemoteWorker(agent), agent);
			agent.Parent.UpdateStatus(agent, AgentStatus.Available);
			LogHelper.LogMessage(agent, "Application Mappings update completed.");
			agent.Parent.UpdateLatestLogOnUI(agent);
		}
	}
}