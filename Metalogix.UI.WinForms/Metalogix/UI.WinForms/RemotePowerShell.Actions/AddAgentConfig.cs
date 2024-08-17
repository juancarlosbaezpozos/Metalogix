using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.RemotePowerShell.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[ActionConfig(new Type[] { typeof(AddAgentAction) })]
	public class AddAgentConfig : IActionConfig
	{
		public AddAgentConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			IXMLAbleList sources = context.ActionContext.Sources;
			List<Agent> list = null;
			if (sources != null && sources.Count > 0 && sources[0] is AgentCollection)
			{
				list = ((AgentCollection)sources[0]).GetList();
			}
			AddEditAgent addEditAgent = new AddEditAgent(false)
			{
				ConfiguredAgents = list
			};
			if (addEditAgent.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = addEditAgent.Agent;
			return ConfigurationResult.Run;
		}
	}
}