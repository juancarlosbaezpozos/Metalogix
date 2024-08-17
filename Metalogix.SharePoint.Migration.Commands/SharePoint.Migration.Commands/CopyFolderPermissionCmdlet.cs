using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointFolderPermissions")]
	public class CopyFolderPermissionCmdlet : CopyPermissionsCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(Metalogix.SharePoint.Actions.Migration.CopyFolderPermissions);
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

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should copy permissions for subfolders and items in subfolders.")]
		public SwitchParameter RecursivelyCopySubFolderPermissions
		{
			get
			{
				return this.CopyPermissionsOptions.RecursiveFolders;
			}
			set
			{
				this.CopyPermissionsOptions.RecursiveFolders = value;
			}
		}

		public CopyFolderPermissionCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}