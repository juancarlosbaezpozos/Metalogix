using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Database;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Database;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Database;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	[ActionConfig(new Type[] { typeof(ConnectToBackup) })]
	public class ConnectToBackupConfig : IActionConfig
	{
		public ConnectToBackupConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog()
				{
					CheckFileExists = true,
					Multiselect = true,
					AddExtension = true,
					DefaultExt = ".bak",
					Filter = "Database Files (*.bak; *.mdf)|*.bak;*.mdf",
					Title = "Select the SQL Server backup file(s)"
				};
				bool flag = true;
				do
				{
					flag = true;
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						if ((int)openFileDialog.FileNames.Length <= 1)
						{
							continue;
						}
						string[] fileNames = openFileDialog.FileNames;
						for (int i = 0; i < (int)fileNames.Length; i++)
						{
							if (!fileNames[i].EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
							{
								FlatXtraMessageBox.Show("When selecting multiple backup files, they must all be '.bak' files.", "Invalid Backup File Selection", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								flag = false;
							}
						}
					}
					else
					{
						configurationResult = ConfigurationResult.Cancel;
						return configurationResult;
					}
				}
				while (!flag);
				DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog()
				{
					AllowBrowsingNetworkServers = true,
					AllowBrowsingDatabases = false
				};
				if (databaseConnectDialog.ShowDialog() == DialogResult.OK)
				{
					Credentials credentials = databaseConnectDialog.Credentials;
					Credentials credential = (credentials.IsDefault ? Credentials.DefaultCredentials : new Credentials(credentials.UserName, credentials.Password, credentials.SavePassword));
					object[] sqlServerName = new object[] { databaseConnectDialog.SqlServerName, "master", credential };
					((IDBReader)SharePointAdapter.GetDBAdapter(sqlServerName)).CheckConnection(false);
					Cursor.Current = Cursors.WaitCursor;
					RestoredDatabaseData restoredDatabaseDatum = DatabaseRestorationManager.Open(databaseConnectDialog.SqlServerName, openFileDialog.FileNames, credential, new DatabaseRestorationDialogs(new ConnectToMdfDatabaseHandler(), new GetDatabaseBackupVersionsHandler(), new InitializeFileLocationsHandler(), new RestoreDatabaseFromBakHandler(), new RestoreDatabaseFromMdfHandler(), new RunRestoreHandler()));
					if (restoredDatabaseDatum != null)
					{
						string str = string.Concat(restoredDatabaseDatum.ServerName.ToUpper(), ".", restoredDatabaseDatum.DatabaseName.ToUpper());
						string str1 = string.Concat(restoredDatabaseDatum.SourceServerName.ToUpper(), ".", restoredDatabaseDatum.SourceDatabaseName.ToUpper());
						if (DatabaseSettings.OpenedBackups.ContainsKey(str))
						{
							DatabaseSettings.OpenedBackups[str] = str1;
							DatabaseSettings.SaveOpenedBackups();
						}
						else
						{
							DatabaseSettings.OpenedBackups.Add(str, str1);
							DatabaseSettings.SaveOpenedBackups();
						}
						Credentials credential1 = (databaseConnectDialog.Credentials.IsDefault ? new Credentials() : new Credentials(databaseConnectDialog.Credentials.UserName, databaseConnectDialog.Credentials.Password, databaseConnectDialog.Credentials.SavePassword));
						object[] serverName = new object[] { restoredDatabaseDatum.ServerName, restoredDatabaseDatum.DatabaseName, credential1 };
						SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(serverName);
						dBAdapter.CheckConnection();
						SPServer sPServer = new SPServer(dBAdapter);
						if (restoredDatabaseDatum.BackupType == RestoredBackupType.Bak)
						{
							sPServer.BackupType = SPConnection.BackupConnectionType.Bak;
						}
						else if (restoredDatabaseDatum.BackupType == RestoredBackupType.Mdf)
						{
							sPServer.BackupType = SPConnection.BackupConnectionType.Mdf;
						}
						Metalogix.Explorer.Settings.ActiveConnections.Add(sPServer);
						Cursor.Current = Cursors.Default;
						configurationResult = ConfigurationResult.Run;
					}
					else
					{
						configurationResult = ConfigurationResult.Cancel;
					}
				}
				else
				{
					configurationResult = ConfigurationResult.Cancel;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Cursor.Current = Cursors.Default;
				string str2 = "";
				str2 = (!exception.Message.Contains("Too many backup devices specified for backup or restore; only 64 are allowed") ? exception.Message : string.Concat("Selective Restore Manager could not open the selected backup using the chosen SQL server. This may be because the backup was created with a newer version of SQL Server. \n\nError: ", exception.Message));
				GlobalServices.ErrorHandler.HandleException(str2, exception);
				configurationResult = ConfigurationResult.Cancel;
			}
			return configurationResult;
		}
	}
}