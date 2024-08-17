using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(StoragePointDetailsAction) })]
	public class StoragePointDetailsConfig : IActionConfig
	{
		public StoragePointDetailsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPListItem item = (SPListItem)context.ActionContext.Targets[0];
			(new StoragePointDetailsDialog()
			{
				Item = item
			}).ShowDialog();
			return ConfigurationResult.Run;
		}
	}
}