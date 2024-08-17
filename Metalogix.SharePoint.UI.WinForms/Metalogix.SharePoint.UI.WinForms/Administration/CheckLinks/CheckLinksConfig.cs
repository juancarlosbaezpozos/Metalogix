using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration.CheckLinks;
using System;
using System.Collections;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration.CheckLinks;

namespace Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks
{
	[ActionConfig(new Type[] { typeof(CheckLinksAction) })]
	public class CheckLinksConfig : IActionConfig
	{
		public CheckLinksConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SharePointObjectScope sharePointObjectScope = SharePointObjectScope.List;
			foreach (object target in context.ActionContext.Targets)
			{
				if (!(target is SPWeb))
				{
					continue;
				}
				sharePointObjectScope = SharePointObjectScope.Site;
				break;
			}
			CheckLinksConfigDialog checkLinksConfigDialog = new CheckLinksConfigDialog(sharePointObjectScope)
			{
				Options = context.GetActionOptions<CheckLinksOptions>()
			};
			checkLinksConfigDialog.ShowDialog();
			if (checkLinksConfigDialog.DialogResult != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = checkLinksConfigDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}