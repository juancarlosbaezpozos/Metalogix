using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(Metalogix.SharePoint.Actions.Administration.ExportItemsWithMetadataAction) })]
	public class ExportItemsWithMetadataAction : IActionConfig
	{
		public ExportItemsWithMetadataAction()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			(new ExportItems(true)).RunAction(context.ActionContext.Targets);
			return ConfigurationResult.Run;
		}
	}
}