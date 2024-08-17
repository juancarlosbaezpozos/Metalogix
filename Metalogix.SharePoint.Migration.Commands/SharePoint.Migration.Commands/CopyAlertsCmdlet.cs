using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointSiteAlerts")]
	public class CopyAlertsCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyWebAlertsAction);
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.AlertOptions AlertOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.AlertOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we need to use Azure Migration Pipeline/SPO Container to copy alerts.")]
		public SwitchParameter AzureMigrationForAlerts
		{
			get
			{
				return this.AlertOptions.UseAzureUpload;
			}
			set
			{
				this.AlertOptions.UseAzureUpload = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy the alerts for items in lists.")]
		public SwitchParameter CopyListItemAlerts
		{
			get
			{
				return this.AlertOptions.CopyItemAlerts;
			}
			set
			{
				this.AlertOptions.CopyItemAlerts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if need to encrypt Azure/SPO Container jobs during copy operation.")]
		public SwitchParameter EncryptAzureJobs
		{
			get
			{
				return this.AlertOptions.UseEncryptedAzureMigration;
			}
			set
			{
				this.AlertOptions.UseEncryptedAzureMigration = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy the alerts for child sub sites.")]
		public SwitchParameter RecursivelyCopySubsiteAlerts
		{
			get
			{
				return this.AlertOptions.CopyChildSiteAlerts;
			}
			set
			{
				this.AlertOptions.CopyChildSiteAlerts = value;
			}
		}

		public CopyAlertsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null || !(base.Source is SPWeb))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null || !(base.Target is SPWeb))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The target of the copy is null, please initialize a proper target node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}