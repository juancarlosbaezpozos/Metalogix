using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyPermissionsAction) })]
	public class CopyPermissionsConfig : IActionConfig
	{
		public CopyPermissionsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			CopyPermissionsDialog copyPermissionsDialog = new CopyPermissionsDialog(SharePointObjectScope.Permissions);
			if (context.ActionContext.Sources.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copyPermissionsDialog.MultiSelectUI = true;
			}
			copyPermissionsDialog.Options = context.GetActionOptions<CopyPermissionsOptions>();
			copyPermissionsDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyPermissionsDialog.ShowDialog();
			return copyPermissionsDialog.ConfigurationResult;
		}
	}
}