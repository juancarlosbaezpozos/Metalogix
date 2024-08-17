using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyPermissionsRecursivelyAction) })]
	public class CopyPermissionsRecursivelyConfig : IActionConfig
	{
		public CopyPermissionsRecursivelyConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			CopyPermissionsDialog copyPermissionsDialog = new CopyPermissionsDialog(context.GetAction<CopyPermissionsRecursivelyAction>().Scope);
			if (context.ActionContext.Sources.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copyPermissionsDialog.MultiSelectUI = true;
			}
			copyPermissionsDialog.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
			copyPermissionsDialog.TargetNodes = context.ActionContext.GetTargetsAsNodeCollection();
			copyPermissionsDialog.Action = context.Action;
			copyPermissionsDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyPermissionsDialog.ShowDialog();
			return copyPermissionsDialog.ConfigurationResult;
		}
	}
}