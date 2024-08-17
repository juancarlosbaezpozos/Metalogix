using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PastePortalListingsAction) })]
	public class CopyPortalListingConfig : IActionConfig
	{
		public CopyPortalListingConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			CopyPortalListingsDialog copyPortalListingsDialog = new CopyPortalListingsDialog()
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Options = context.GetActionOptions<PastePortalListingsOptions>()
			};
			copyPortalListingsDialog.ShowDialog();
			return copyPortalListingsDialog.ConfigurationResult;
		}
	}
}