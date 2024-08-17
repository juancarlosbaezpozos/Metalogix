using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Adapters;
using Metalogix.UI.WinForms.Database;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(ReConnect) })]
	public class ReConnectConfig : IActionConfig
	{
		public ReConnectConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPConnection item = (SPConnection)context.ActionContext.Targets[0];
			if (!(item.Adapter is IDBReader))
			{
				ConnectToSharePointDialog connectToSharePointDialog = new ConnectToSharePointDialog(item)
				{
					EnableLocationEditing = false
				};
				if (connectToSharePointDialog.ShowDialog() != DialogResult.OK)
				{
					return ConfigurationResult.Cancel;
				}
				Metalogix.Explorer.Settings.SaveActiveConnections();
			}
			else
			{
				DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog()
				{
					Credentials = item.Credentials,
					EnableLocationEditing = false,
					SqlServerName = item.Adapter.Server,
					SqlDatabaseName = (item.Adapter as IDBReader).Database
				};
				if (databaseConnectDialog.ShowDialog() != DialogResult.OK)
				{
					return ConfigurationResult.Cancel;
				}
				Credentials credentials = databaseConnectDialog.Credentials;
				Credentials credential = (credentials.IsDefault ? Credentials.DefaultCredentials : new Credentials(credentials.UserName, credentials.Password, credentials.SavePassword));
				object[] sqlServerName = new object[] { databaseConnectDialog.SqlServerName, "master", credential };
				((IDBReader)SharePointAdapter.GetDBAdapter(sqlServerName)).CheckConnection(false);
				item.Adapter.Credentials = credential;
				Metalogix.Explorer.Settings.SaveActiveConnections();
			}
			item.ReconnectAsync();
			return ConfigurationResult.Run;
		}
	}
}