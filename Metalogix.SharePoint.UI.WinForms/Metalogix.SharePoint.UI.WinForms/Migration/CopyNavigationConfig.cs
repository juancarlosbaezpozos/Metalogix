using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteNavigationAction) })]
	public class CopyNavigationConfig : IActionConfig
	{
		public CopyNavigationConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			CopyNavigationActionDialog copyNavigationActionDialog = new CopyNavigationActionDialog()
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Options = context.GetActionOptions<PasteNavigationOptions>()
			};
			copyNavigationActionDialog.ShowDialog();
			return copyNavigationActionDialog.ConfigurationResult;
		}
	}
}