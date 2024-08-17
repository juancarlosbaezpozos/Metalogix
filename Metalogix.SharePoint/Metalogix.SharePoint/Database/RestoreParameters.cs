using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Database
{
	public struct RestoreParameters
	{
		public string sSqlServer;

		public string sNewDatabaseName;

		public List<DatabaseBackup> databaseBackups;

		public string sUserId;

		public string sPassword;

		public bool bSqlAuthentication;

		public string sDatabaseDataFileLocation;

		public string sDatabaseLogFileLocation;

		public bool UseRedGate;
	}
}