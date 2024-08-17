using Metalogix.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class AlertOptions : SharePointActionOptions
	{
		private bool m_bCopyAlertsChildSites;

		private bool m_bCopyAlertsItems;

		private bool _useAzureUpload = true;

		private bool _useEncryptedAzureMigration = true;

		[CmdletEnabledParameter("CopyChildSiteAlerts", true)]
		[CmdletParameterAlias("RecursivelyCopySubsiteAlerts")]
		public bool CopyChildSiteAlerts
		{
			get
			{
				return this.m_bCopyAlertsChildSites;
			}
			set
			{
				this.m_bCopyAlertsChildSites = value;
			}
		}

		[CmdletEnabledParameter("CopyItemAlerts", true)]
		[CmdletParameterAlias("CopyListItemAlerts")]
		public bool CopyItemAlerts
		{
			get
			{
				return this.m_bCopyAlertsItems;
			}
			set
			{
				this.m_bCopyAlertsItems = value;
			}
		}

		[CmdletEnabledParameter("UseAzureUpload", true)]
		[CmdletParameterAlias("AzureMigrationForAlerts")]
		public bool UseAzureUpload
		{
			get
			{
				return this._useAzureUpload;
			}
			set
			{
				this._useAzureUpload = value;
			}
		}

		[CmdletEnabledParameter("UseEncryptedAzureMigration", true)]
		[CmdletParameterAlias("EncryptAzureJobs")]
		public bool UseEncryptedAzureMigration
		{
			get
			{
				return this._useEncryptedAzureMigration;
			}
			set
			{
				this._useEncryptedAzureMigration = value;
			}
		}

		public AlertOptions()
		{
		}
	}
}