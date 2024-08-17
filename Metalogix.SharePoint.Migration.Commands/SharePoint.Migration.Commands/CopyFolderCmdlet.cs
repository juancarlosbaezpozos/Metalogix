using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointFolder")]
	public class CopyFolderCmdlet : CopyListItemCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteFolderAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for folders.")]
		public SwitchParameter CopyFolderPermissions
		{
			get
			{
				return this.PasteFolderOptions.CopyFolderPermissions;
			}
			set
			{
				this.PasteFolderOptions.CopyFolderPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include list items and documents.")]
		public SwitchParameter CopyListItems
		{
			get
			{
				return this.PasteFolderOptions.CopyListItems;
			}
			set
			{
				this.PasteFolderOptions.CopyListItems = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should use the Azure SPO Migration Pipeline with Encryption for documents.")]
		public SwitchParameter EncryptAzureMigrationJobs
		{
			get
			{
				return this.PasteFolderOptions.EncryptAzureMigrationJobs;
			}
			set
			{
				this.PasteFolderOptions.EncryptAzureMigrationJobs = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a folder should be copied.")]
		public IFilterExpression FolderFilterExpression
		{
			get
			{
				if (!this.PasteFolderOptions.FilterFolders)
				{
					return null;
				}
				return this.PasteFolderOptions.FolderFilterExpression;
			}
			set
			{
				this.PasteFolderOptions.FilterFolders = true;
				this.PasteFolderOptions.FolderFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should overwrite existing folders.")]
		public SwitchParameter OverwriteFolders
		{
			get
			{
				return this.PasteFolderOptions.OverwriteLists;
			}
			set
			{
				this.PasteFolderOptions.OverwriteLists = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteFolderOptions PasteFolderOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteFolderOptions;
			}
		}

		protected override PasteListItemOptions PasteItemOptions
		{
			get
			{
				return base.Action.Options as PasteListItemOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a collection of transformation tasks, such as renaming, to be applied to the data as it is copied.")]
		public override TransformationTaskCollection TaskCollection
		{
			get
			{
				return this.PasteItemOptions.TaskCollection;
			}
			set
			{
				this.PasteFolderOptions.RenameSpecificNodes = true;
				this.PasteFolderOptions.TaskCollection = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should use the Azure SPO Migration Pipeline for documents. Use this only if you are certain about what you are doing.")]
		public new SwitchParameter UseAzureOffice365Upload
		{
			get
			{
				return this.PasteFolderOptions.UseAzureOffice365Upload;
			}
			set
			{
				this.PasteFolderOptions.UseAzureOffice365Upload = value;
			}
		}

		public CopyFolderCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}