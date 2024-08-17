using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(AnalyzeChurnAction) })]
	public class AnalyzeChurnConfig : IActionConfig
	{
		public AnalyzeChurnConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			AnalyzeChurnConfigurationDialog analyzeChurnConfigurationDialog = new AnalyzeChurnConfigurationDialog()
			{
				Options = context.GetActionOptions<AnalyzeChurnOptions>()
			};
			analyzeChurnConfigurationDialog.ShowDialog();
			if (analyzeChurnConfigurationDialog.DialogResult != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = analyzeChurnConfigurationDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}