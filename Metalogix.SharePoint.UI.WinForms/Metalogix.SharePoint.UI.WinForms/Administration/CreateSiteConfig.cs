using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Administration;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(CreateSiteAction) })]
	public class CreateSiteConfig : IActionConfig
	{
		public CreateSiteConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPWeb item = (SPWeb)context.ActionContext.Targets[0];
			CreateSiteDialog createSiteDialog = new CreateSiteDialog(item.Templates)
			{
				Options = context.GetActionOptions<CreateSiteOptions>()
			};
			createSiteDialog.ShowDialog();
			if (createSiteDialog.DialogResult != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = createSiteDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}