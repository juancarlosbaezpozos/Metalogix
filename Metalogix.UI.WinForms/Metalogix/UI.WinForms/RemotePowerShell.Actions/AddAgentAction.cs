using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.AddAgent16.png")]
	[Name("Add Agent")]
	[ShowInMenus(false)]
	[TargetCardinality(Cardinality.Zero)]
	public class AddAgentAction : AddUpdateAgentBaseAction<Agent>
	{
		public AddAgentAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				if (source != null && source.Count > 0 && source[0] is AgentCollection)
				{
					AgentCollection item = (AgentCollection)source[0];
					Agent agent = item.Add(this.ActionOptions);
					if (agent != null)
					{
						(new Thread(() => base.ConfigureAgent(agent, true, false))).Start();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Format("An error occurred while adding agent '{0}'", this.ActionOptions.MachineIP);
				GlobalServices.ErrorHandler.HandleException("Add Agent", str, exception, ErrorIcon.Error);
			}
		}
	}
}