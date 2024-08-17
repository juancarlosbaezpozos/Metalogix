using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyGroupsAction) })]
	public class CopyGroupsConfig : IActionConfig
	{
		public CopyGroupsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			CopyGroupsDialog copyGroupsDialog = new CopyGroupsDialog();
			if (context.ActionContext.Sources.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copyGroupsDialog.MultiSelectUI = true;
			}
			copyGroupsDialog.SourceNodes = context.ActionContext.Sources as NodeCollection;
			copyGroupsDialog.TargetNodes = context.ActionContext.Targets as NodeCollection;
			copyGroupsDialog.Options = context.GetActionOptions<CopyGroupsOptions>();
			copyGroupsDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyGroupsDialog.ShowDialog();
			return copyGroupsDialog.ConfigurationResult;
		}
	}
}