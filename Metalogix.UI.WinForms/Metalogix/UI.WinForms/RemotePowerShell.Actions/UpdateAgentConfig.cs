using Metalogix.Actions;
using Metalogix.UI.WinForms.RemotePowerShell.Options;
using Metalogix.UI.WinForms.RemotePowerShell.UI;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[ActionConfig(new Type[] { typeof(UpdateAgentAction) })]
	public class UpdateAgentConfig : IActionConfig
	{
		public UpdateAgentConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			UpdateAgent updateAgent = new UpdateAgent()
			{
				UpdateOptions = context.ActionOptions as UpdateAgentOptions
			};
			if (updateAgent.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = updateAgent.UpdateOptions;
			return ConfigurationResult.Run;
		}
	}
}