using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointWebPermissions")]
	public class CopyWebPermissionCmdlet : CopyListPermissionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyWebPermissions);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should include the migration of copied or modified permission levels.")]
		public SwitchParameter CopyPermissionLevels
		{
			get
			{
				return this.CopyPermissionsOptions.CopyPermissionLevels;
			}
			set
			{
				this.CopyPermissionsOptions.CopyPermissionLevels = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should include unique permissions for sites.")]
		public SwitchParameter CopySitePermissions
		{
			get
			{
				return this.CopyPermissionsOptions.CopySitePermissions;
			}
			set
			{
				this.CopyPermissionsOptions.CopySitePermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should copy permissions recursively in child sites of the target site.")]
		public SwitchParameter RecursivelyCopySubSitePermissions
		{
			get
			{
				return this.CopyPermissionsOptions.RecursiveSites;
			}
			set
			{
				this.CopyPermissionsOptions.RecursiveSites = value;
			}
		}

		public CopyWebPermissionCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}