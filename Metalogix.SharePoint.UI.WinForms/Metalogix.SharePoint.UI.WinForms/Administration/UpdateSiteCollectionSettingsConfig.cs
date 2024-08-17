using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(UpdateSiteCollectionSettingsAction) })]
	public class UpdateSiteCollectionSettingsConfig : IActionConfig
	{
		public UpdateSiteCollectionSettingsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPWeb item = context.ActionContext.Targets[0] as SPWeb;
			UpdateSiteCollectionSettingsOptions actionOptions = context.GetActionOptions<UpdateSiteCollectionSettingsOptions>();
			actionOptions.SetAdminsFromList(item.SiteUsers.GetSiteAdminLoginNames());
			actionOptions.QuotaID = item.QuotaID;
			actionOptions.QuotaMaximum = (long)(item.QuotaStorageLimit / 1048576);
			actionOptions.QuotaWarning = (long)(item.QuotaStorageWarning / 1048576);
			UpdateSiteCollectionSettingsDialog updateSiteCollectionSettingsDialog = new UpdateSiteCollectionSettingsDialog()
			{
				Target = item,
				Options = actionOptions
			};
			if (updateSiteCollectionSettingsDialog.ShowDialog() == DialogResult.OK)
			{
				return ConfigurationResult.Run;
			}
			return ConfigurationResult.Cancel;
		}
	}
}