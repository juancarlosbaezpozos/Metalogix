using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	public enum AzureToO365MigrationJobState
	{
		None,
		UploadedToAzure,
		RequestedMigrationFromAzure,
		MonitoringProgress,
		RetryCountReached,
		Error,
		Complete,
		ReRequestMigrationFromAzure
	}
}