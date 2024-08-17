using Metalogix.Commands;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Database;
using Metalogix.Utilities;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace Metalogix.SharePoint.Commands.Database
{
	[CmdletProvider("MetalogixSharePointBackupProvider", ProviderCapabilities.None)]
	public class MetalogixSharePointBackupProvider : MetalogixSharePointProvider
	{
		public MetalogixSharePointBackupProvider()
		{
		}

		protected override System.Management.Automation.PSDriveInfo NewDrive(System.Management.Automation.PSDriveInfo drive)
		{
			string[] root;
			if (drive == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentNullException("drive"), "NullDrive", ErrorCategory.InvalidArgument, null));
				return null;
			}
			if (string.IsNullOrEmpty(drive.Root))
			{
				base.WriteError(new ErrorRecord(new ArgumentException("drive.Root"), "NoRoot", ErrorCategory.InvalidArgument, drive));
				return null;
			}
			SharePointBackupDriveDynamicParameters dynamicParameters = base.DynamicParameters as SharePointBackupDriveDynamicParameters;
			if (dynamicParameters.DatabaseServer == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("DatabaseServer"), "NoServerSpecified", ErrorCategory.InvalidArgument, drive));
				return null;
			}
			Credentials credential = (dynamicParameters.User == null ? new Credentials() : new Credentials(dynamicParameters.User, dynamicParameters.Password.ToSecureString(), false));
			if (dynamicParameters.SupportingBackups != null)
			{
				root = new string[(int)dynamicParameters.SupportingBackups.Length + 1];
				root[0] = drive.Root;
				dynamicParameters.SupportingBackups.CopyTo(root, 1);
			}
			else
			{
				root = new string[] { drive.Root };
			}
			RestoredDatabaseData restoredDatabaseDatum = DatabaseRestorationManager.Open(dynamicParameters.DatabaseServer, root, credential, null);
			object[] serverName = new object[] { restoredDatabaseDatum.ServerName, restoredDatabaseDatum.DatabaseName, credential };
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
			MetalogixBackupDriveInfo metalogixBackupDriveInfo = new MetalogixBackupDriveInfo(drive)
			{
				Connection = sPServer,
				DatabaseServer = restoredDatabaseDatum.SourceServerName,
				DatabaseName = restoredDatabaseDatum.SourceServerName
			};
			return metalogixBackupDriveInfo;
		}

		protected override object NewDriveDynamicParameters()
		{
			return new SharePointBackupDriveDynamicParameters();
		}

		protected override System.Management.Automation.PSDriveInfo RemoveDrive(System.Management.Automation.PSDriveInfo drive)
		{
			SPServer connection = ((MetalogixBackupDriveInfo)drive).Connection as SPServer;
			if (connection != null)
			{
				IDBReader adapter = connection.Adapter as IDBReader;
				if (adapter != null && connection.BackupType == SPConnection.BackupConnectionType.Bak)
				{
					using (SqlConnection sqlConnection = new SqlConnection(adapter.ConnectionString))
					{
						sqlConnection.Open();
						(new SqlCommand("USE Master", sqlConnection)).ExecuteNonQuery();
						string str = string.Format("IF EXISTS (SELECT * FROM sys.databases where name = '{0}') DROP DATABASE [{0}]", adapter.Database);
						(new SqlCommand(str, sqlConnection)).ExecuteNonQuery();
						sqlConnection.Close();
					}
				}
			}
			((MetalogixBackupDriveInfo)drive).Connection = null;
			GC.Collect();
			return drive;
		}
	}
}