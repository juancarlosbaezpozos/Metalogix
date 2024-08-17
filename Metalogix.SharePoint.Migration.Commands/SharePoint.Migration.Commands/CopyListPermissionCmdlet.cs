using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointListPermissions")]
	public class CopyListPermissionCmdlet : CopyPermissionsCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(Metalogix.SharePoint.Actions.Migration.CopyListPermissions);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should include unique permissions for folders.")]
		public SwitchParameter CopyFolderPermissions
		{
			get
			{
				return this.CopyPermissionsOptions.CopyFolderPermissions;
			}
			set
			{
				this.CopyPermissionsOptions.CopyFolderPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should include unique permissions for items and documents.")]
		public SwitchParameter CopyItemPermissions
		{
			get
			{
				return this.CopyPermissionsOptions.CopyItemPermissions;
			}
			set
			{
				this.CopyPermissionsOptions.CopyItemPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should include unique permissions for lists.")]
		public SwitchParameter CopyListPermissions
		{
			get
			{
				return this.CopyPermissionsOptions.CopyListPermissions;
			}
			set
			{
				this.CopyPermissionsOptions.CopyListPermissions = value;
			}
		}

		public CopyListPermissionCmdlet()
		{
		}

		protected override bool ProcessParameters()
		{
			bool flag = base.ProcessParameters();
			this.CopyPermissionsOptions.RecursiveFolders = (this.CopyPermissionsOptions.CopyFolderPermissions ? true : this.CopyPermissionsOptions.CopyItemPermissions);
			return flag;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}