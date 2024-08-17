using Metalogix.Commands;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointItemAlerts")]
	public class CopyItemAlertsCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyItemAlertsAction);
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.AlertOptions AlertOptions
		{
			get
			{
				return ((CopyItemAlertsAction)base.Action).SharePointOptions;
			}
		}

		public CopyItemAlertsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null || !(base.Source is SPListItem))
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null || !(base.Target is SPListItem))
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