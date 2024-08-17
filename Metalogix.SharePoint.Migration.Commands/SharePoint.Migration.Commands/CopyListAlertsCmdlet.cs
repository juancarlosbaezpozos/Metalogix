using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointListAlerts")]
	public class CopyListAlertsCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyListAlertsAction);
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.AlertOptions AlertOptions
		{
			get
			{
				return ((CopyListAlertsAction)base.Action).SharePointOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we need to use Azure Pipeline/SPO Container to copy alerts.")]
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

		[Parameter(Mandatory=false, HelpMessage="Indicates if need to Encrypt Azure/SPO Container Jobs during copy operation.")]
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

		public CopyListAlertsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null || !(base.Source is SPList))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null || !(base.Target is SPList))
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