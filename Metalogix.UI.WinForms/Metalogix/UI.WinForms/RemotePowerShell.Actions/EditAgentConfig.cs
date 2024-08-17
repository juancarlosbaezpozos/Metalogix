using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.RemotePowerShell.UI;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[ActionConfig(new Type[] { typeof(EditAgentAction) })]
	public class EditAgentConfig : IActionConfig
	{
		public EditAgentConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			Agent item = (Agent)context.ActionContext.Targets[0];
			if (item == null)
			{
				return ConfigurationResult.Cancel;
			}
			AddEditAgent addEditAgent = new AddEditAgent(true)
			{
				Agent = item
			};
			addEditAgent.LoadUI();
			if (addEditAgent.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = addEditAgent.Agent;
			return ConfigurationResult.Run;
		}
	}
}