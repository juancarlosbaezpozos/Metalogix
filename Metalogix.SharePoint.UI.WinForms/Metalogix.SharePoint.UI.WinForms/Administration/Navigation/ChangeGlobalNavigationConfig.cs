using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration.Navigation;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
	[ActionConfig(new Type[] { typeof(ChangeGlobalNavigationAction) })]
	public class ChangeGlobalNavigationConfig : IActionConfig
	{
		public ChangeGlobalNavigationConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPWeb item = context.ActionContext.Targets[0] as SPWeb;
			ChangeGlobalNavigationDialog changeGlobalNavigationDialog = new ChangeGlobalNavigationDialog()
			{
				Options = context.GetAction<ChangeGlobalNavigationAction>().GetWebNavigationSettings(item)
			};
			if (changeGlobalNavigationDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = changeGlobalNavigationDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}