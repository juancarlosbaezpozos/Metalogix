using Metalogix;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPAlertOptions : OptionsBase
	{
		private bool _copyAlertsChildSites = true;

		private bool _copyAlertsItems = true;

		private bool _useAzureUpload = true;

		private bool _useEncryptedAzureMigration;

		public bool CopyChildSiteAlerts
		{
			get
			{
				return this._copyAlertsChildSites;
			}
			set
			{
				this._copyAlertsChildSites = value;
			}
		}

		public bool CopyItemAlerts
		{
			get
			{
				return this._copyAlertsItems;
			}
			set
			{
				this._copyAlertsItems = value;
			}
		}

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

		public SPAlertOptions()
		{
		}
	}
}