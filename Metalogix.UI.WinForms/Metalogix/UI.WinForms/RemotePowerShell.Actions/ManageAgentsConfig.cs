using Metalogix.Actions;
using Metalogix.UI.WinForms.RemotePowerShell.UI;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[ActionConfig(new Type[] { typeof(ManageAgentsAction) })]
	public class ManageAgentsConfig : IActionConfig
	{
		public ManageAgentsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if ((new ManageAgents()).ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			return ConfigurationResult.Run;
		}
	}
}