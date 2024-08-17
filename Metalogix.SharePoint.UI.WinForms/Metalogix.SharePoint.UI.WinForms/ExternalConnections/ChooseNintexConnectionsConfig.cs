using Metalogix.Actions;
using Metalogix.SharePoint.Actions.ExternalConnections;
using Metalogix.SharePoint.Options.ExternalConnections;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	[ActionConfig(new Type[] { typeof(AttachNintexConnection) })]
	public class ChooseNintexConnectionsConfig : IActionConfig
	{
		public ChooseNintexConnectionsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ChooseNintexConnectionsDialog chooseNintexConnectionsDialog = new ChooseNintexConnectionsDialog()
			{
				Context = context.ActionContext
			};
			if (chooseNintexConnectionsDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.GetActionOptions<AttachNintexConnectionsOptions>().Connections = chooseNintexConnectionsDialog.GetSelectedConnections();
			return ConfigurationResult.Run;
		}
	}
}