using Metalogix.Actions;
using Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.Actions
{
	[ActionConfig(new Type[] { typeof(AgentWizardAction) })]
	public class AgentWizardConfig : IActionConfig
	{
		public AgentWizardConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			(new AgentWizard(context)).ShowDialog();
			return ConfigurationResult.Run;
		}
	}
}