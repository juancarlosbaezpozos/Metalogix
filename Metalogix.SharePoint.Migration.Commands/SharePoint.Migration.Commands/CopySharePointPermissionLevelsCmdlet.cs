using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointPermissionLevels")]
	public class CopySharePointPermissionLevelsCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteRolesAction);
			}
		}

		protected virtual PasteRolesOptions CopyRoleOptions
		{
			get
			{
				return base.Action.Options as PasteRolesOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Recursively Copy Permission Levels.")]
		public SwitchParameter RecursivelyCopyPermissionLevels
		{
			get
			{
				return this.CopyRoleOptions.RecursivelyCopyPermissionLevels;
			}
			set
			{
				this.CopyRoleOptions.RecursivelyCopyPermissionLevels = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Enables verbose logging.")]
		public SwitchParameter VerboseLog
		{
			get
			{
				return this.CopyRoleOptions.Verbose;
			}
			set
			{
				this.CopyRoleOptions.Verbose = value;
			}
		}

		public CopySharePointPermissionLevelsCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			if (base.Source == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("The source of the copy is null, please initialize a proper source node."), "ArgumentError", ErrorCategory.InvalidArgument, base.Target));
			}
			else if (base.Target == null)
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