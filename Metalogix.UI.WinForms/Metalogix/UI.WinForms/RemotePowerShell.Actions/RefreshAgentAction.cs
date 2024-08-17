using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.DataStructures.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.RefreshAgent16.png")]
	[MenuText("Refresh Agent {3-Refresh Agent}")]
	[Name("Refresh Agent")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.Zero)]
	public class RefreshAgentAction : AgentAction
	{
		public RefreshAgentAction()
		{
		}

		public override bool EnabledOn(IXMLAbleList source, IXMLAbleList target)
		{
			if (target == null)
			{
				return false;
			}
			return target.Cast<Agent>().Any<Agent>((Agent agent) => agent.Status != AgentStatus.Configuring);
		}

		private void RefreshAgent(IXMLAbleList source, IXMLAbleList target)
		{
			if (target.Count > 0 && target[0] is Agent)
			{
				Agent item = (Agent)target[0];
				RemoteJobScheduler.Instance.RefreshAgent(item);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.RefreshAgent(source, target);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams[0] != null)
			{
				Agent[] agentArray = new Agent[] { oParams[0] as Agent };
				this.RefreshAgent(null, new CommonSerializableList<Agent>(agentArray));
			}
		}
	}
}