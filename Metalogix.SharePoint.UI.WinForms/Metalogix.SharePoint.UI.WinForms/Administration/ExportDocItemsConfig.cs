using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(ExportDocItemsAction) })]
	public class ExportDocItemsConfig : IActionConfig
	{
		public ExportDocItemsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			(new ExportItems(false)).RunAction(context.ActionContext.Targets);
			return ConfigurationResult.Run;
		}
	}
}