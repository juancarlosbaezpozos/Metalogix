using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(ExportDocItemsWithMetadataAction) })]
	public class ExportDocItemsWithMetadataConfig : IActionConfig
	{
		public ExportDocItemsWithMetadataConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			(new ExportItems(true)).RunAction(context.ActionContext.Targets);
			return ConfigurationResult.Run;
		}
	}
}