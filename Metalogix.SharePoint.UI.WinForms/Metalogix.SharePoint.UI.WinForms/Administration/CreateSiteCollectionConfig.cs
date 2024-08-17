using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(CreateSiteCollection) })]
	public class CreateSiteCollectionConfig : IActionConfig
	{
		public CreateSiteCollectionConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPBaseServer item = context.ActionContext.Targets[0] as SPBaseServer;
			if (item == null)
			{
				return ConfigurationResult.Cancel;
			}
			CreateSiteCollectionOptions actionOptions = context.GetActionOptions<CreateSiteCollectionOptions>();
			actionOptions.WebApplication = item.WebApplication;
			actionOptions.SelfServiceCreateMode = context.Action is CreateSiteCollectionSelfServiceMode;
			CreateSiteCollectionDialog createSiteCollectionDialog = new CreateSiteCollectionDialog()
			{
				Text = context.Action.Name,
				Target = item,
				Options = actionOptions
			};
			if (createSiteCollectionDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			return ConfigurationResult.Run;
		}
	}
}