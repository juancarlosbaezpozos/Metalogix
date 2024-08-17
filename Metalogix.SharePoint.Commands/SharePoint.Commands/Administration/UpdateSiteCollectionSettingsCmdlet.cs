using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Update", "SharePointSiteCollectionSettings")]
	public class UpdateSiteCollectionSettingsCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(UpdateSiteCollectionSettingsAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="A quota template ID to use for this site collection.")]
		public string QuotaID
		{
			get
			{
				return this.UpdateSiteCollectionSettingsOptions.QuotaID;
			}
			set
			{
				this.UpdateSiteCollectionSettingsOptions.QuotaID = value;
				this.UpdateSiteCollectionSettingsOptions.SetSiteQuota = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="An independently defined maximum storage size for this site collection.")]
		public long QuotaMaximum
		{
			get
			{
				return this.UpdateSiteCollectionSettingsOptions.QuotaMaximum;
			}
			set
			{
				this.UpdateSiteCollectionSettingsOptions.QuotaMaximum = value;
				this.UpdateSiteCollectionSettingsOptions.SetSiteQuota = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="An independently defined size at which to send a warning email for this site collection.")]
		public long QuotaWarning
		{
			get
			{
				return this.UpdateSiteCollectionSettingsOptions.QuotaWarning;
			}
			set
			{
				this.UpdateSiteCollectionSettingsOptions.QuotaWarning = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="A list of login names to be granted site collection administrator privileges.")]
		public string[] SiteCollectionAdmins
		{
			get
			{
				return this.UpdateSiteCollectionSettingsOptions.SiteCollectionAdminsList;
			}
			set
			{
				this.UpdateSiteCollectionSettingsOptions.SetAdminsFromList(value);
				this.UpdateSiteCollectionSettingsOptions.SetSiteCollectionAdmins = true;
			}
		}

		private Metalogix.SharePoint.Options.Administration.UpdateSiteCollectionSettingsOptions UpdateSiteCollectionSettingsOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Administration.UpdateSiteCollectionSettingsOptions;
			}
		}

		public UpdateSiteCollectionSettingsCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}