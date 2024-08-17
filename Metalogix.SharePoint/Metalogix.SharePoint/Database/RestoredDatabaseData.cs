using Metalogix.Permissions;
using System;

namespace Metalogix.SharePoint.Database
{
	public class RestoredDatabaseData
	{
		public string ServerName;

		public string DatabaseName;

		public Metalogix.Permissions.Credentials Credentials;

		public string SourceServerName;

		public string SourceDatabaseName;

		public RestoredBackupType BackupType;

		public RestoredDatabaseData()
		{
		}
	}
}