using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteListViewsAction) })]
	public class CopyListViewConfig : IActionConfig
	{
		public CopyListViewConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			CopyListViewsDialog copyListViewsDialog = new CopyListViewsDialog()
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Options = context.GetActionOptions<PasteListViewsOptions>()
			};
			CopyListViewsDialog copyListViewsDialog1 = copyListViewsDialog;
			copyListViewsDialog1.ShowDialog();
			return copyListViewsDialog1.ConfigurationResult;
		}
	}
}