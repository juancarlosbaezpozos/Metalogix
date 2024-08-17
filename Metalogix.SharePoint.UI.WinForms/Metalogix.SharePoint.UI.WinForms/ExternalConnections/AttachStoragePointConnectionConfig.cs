using Metalogix.Actions;
using Metalogix.SharePoint.Actions.ExternalConnections;
using Metalogix.SharePoint.Options.ExternalConnections;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	[ActionConfig(new Type[] { typeof(AttachStoragePointConnection) })]
	public class AttachStoragePointConnectionConfig : IActionConfig
	{
		public AttachStoragePointConnectionConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			AttachStoragePointConnectionDialog attachStoragePointConnectionDialog = new AttachStoragePointConnectionDialog()
			{
				Context = context.ActionContext
			};
			if (attachStoragePointConnectionDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.GetActionOptions<AttachStoragePointConnectionsOptions>().Connections = attachStoragePointConnectionDialog.GetSelectedConnections();
			return ConfigurationResult.Run;
		}
	}
}