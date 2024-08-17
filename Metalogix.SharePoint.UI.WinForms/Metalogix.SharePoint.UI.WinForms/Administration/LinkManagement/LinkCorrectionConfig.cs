using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration.LinkManagement;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration.LinkManagement;

namespace Metalogix.SharePoint.UI.WinForms.Administration.LinkManagement
{
	[ActionConfig(new Type[] { typeof(LinkCorrectionAction) })]
	public class LinkCorrectionConfig : IActionConfig
	{
		public LinkCorrectionConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			LinkCorrectionDialog linkCorrectionDialog = new LinkCorrectionDialog(context.ActionContext.Targets)
			{
				Options = context.GetActionOptions<LinkCorrectionOptions>()
			};
			if (linkCorrectionDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = linkCorrectionDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}