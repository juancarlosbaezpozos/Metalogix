using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointList")]
	public class CopyListCmdlet : CopyListItemCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteListAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if only newer lists will be copied.")]
		public SwitchParameter CheckModifiedDatesForLists
		{
			get
			{
				return this.PasteListOptions.CheckModifiedDatesForLists;
			}
			set
			{
				this.PasteListOptions.CheckModifiedDatesForLists = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy content type workflow associations.")]
		public SwitchParameter CopyContentTypeOOBWorkflowAssociations
		{
			get
			{
				return this.PasteListOptions.CopyContentTypeOOBWorkflowAssociations;
			}
			set
			{
				this.PasteListOptions.CopyContentTypeOOBWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy SharePoint Designer created content type workflow associations.")]
		public SwitchParameter CopyContentTypeSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.PasteListOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.PasteListOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include customizations made to the list's form pages.")]
		public SwitchParameter CopyCustomizedFormPages
		{
			get
			{
				return this.PasteListOptions.CopyCustomizedFormPages;
			}
			set
			{
				this.PasteListOptions.CopyCustomizedFormPages = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for folders.")]
		public SwitchParameter CopyFolderPermissions
		{
			get
			{
				return this.PasteListOptions.CopyFolderPermissions;
			}
			set
			{
				this.PasteListOptions.CopyFolderPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include web parts on list form pages (Disp, Edit, New).")]
		public SwitchParameter CopyFormWebParts
		{
			get
			{
				return this.PasteListOptions.CopyFormWebParts;
			}
			set
			{
				this.PasteListOptions.CopyFormWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy globally reusable workflow templates.")]
		public SwitchParameter CopyGloballyReusableWorkflowTemplates
		{
			get
			{
				return this.PasteListOptions.CopyGloballyReusableWorkflowTemplates;
			}
			set
			{
				this.PasteListOptions.CopyGloballyReusableWorkflowTemplates = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include list items and documents.")]
		public SwitchParameter CopyListItems
		{
			get
			{
				return this.PasteListOptions.CopyListItems;
			}
			set
			{
				this.PasteListOptions.CopyListItems = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy list workflow associations.")]
		public SwitchParameter CopyListOOBWorkflowAssociations
		{
			get
			{
				return this.PasteListOptions.CopyListOOBWorkflowAssociations;
			}
			set
			{
				this.PasteListOptions.CopyListOOBWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for lists.")]
		public SwitchParameter CopyListPermissions
		{
			get
			{
				return this.PasteListOptions.CopyListPermissions;
			}
			set
			{
				this.PasteListOptions.CopyListPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy SharePoint Designer created list workflow associations.")]
		public SwitchParameter CopyListSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.PasteListOptions.CopyListSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.PasteListOptions.CopyListSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy Nintex workflow database entries.")]
		public SwitchParameter CopyNintexDatabaseEntries
		{
			get
			{
				return this.PasteListOptions.CopyNintexDatabaseEntries;
			}
			set
			{
				this.PasteListOptions.CopyNintexDatabaseEntries = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy previous version of SPD workflow assciations and instances.")]
		public SwitchParameter CopyPreviousVersionOfWorkflowInstances
		{
			get
			{
				return this.PasteListOptions.CopyPreviousVersionOfWorkflowInstances;
			}
			set
			{
				this.PasteListOptions.CopyPreviousVersionOfWorkflowInstances = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include subfolders.")]
		public new SwitchParameter CopySubFolders
		{
			get
			{
				return this.PasteListOptions.CopySubFolders;
			}
			set
			{
				this.PasteListOptions.CopySubFolders = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include web parts on view pages.")]
		public SwitchParameter CopyViewWebParts
		{
			get
			{
				return this.PasteListOptions.CopyViewWebParts;
			}
			set
			{
				this.PasteListOptions.CopyViewWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy web workflow associations.")]
		public SwitchParameter CopyWebOOBWorkflowAssociations
		{
			get
			{
				return this.PasteListOptions.CopyWebOOBWorkflowAssociations;
			}
			set
			{
				this.PasteListOptions.CopyWebOOBWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy SharePoint Designer created web workflow associations.")]
		public SwitchParameter CopyWebSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.PasteListOptions.CopyWebSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.PasteListOptions.CopyWebSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should use the Azure SPO Migration Pipeline with Encryption for documents.")]
		public SwitchParameter EncryptAzureMigrationJobs
		{
			get
			{
				return this.PasteListOptions.EncryptAzureMigrationJobs;
			}
			set
			{
				this.PasteListOptions.EncryptAzureMigrationJobs = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a folder should be copied.")]
		public IFilterExpression FolderFilterExpression
		{
			get
			{
				if (!this.PasteListOptions.FilterFolders)
				{
					return null;
				}
				return this.PasteListOptions.FolderFilterExpression;
			}
			set
			{
				this.PasteListOptions.FilterFolders = true;
				this.PasteListOptions.FolderFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a list should be copied.")]
		public IFilterExpression ListFilterExpression
		{
			get
			{
				if (!this.PasteListOptions.FilterLists)
				{
					return null;
				}
				return this.PasteListOptions.ListFilterExpression;
			}
			set
			{
				this.PasteListOptions.FilterLists = true;
				this.PasteListOptions.ListFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should overwrite existing lists. Overwriting lists takes precedence over the 'UpdateLists' parameter.")]
		public SwitchParameter OverwriteLists
		{
			get
			{
				return this.PasteListOptions.OverwriteLists;
			}
			set
			{
				this.PasteListOptions.OverwriteLists = value;
			}
		}

		protected override PasteListItemOptions PasteItemOptions
		{
			get
			{
				return base.Action.Options as PasteListItemOptions;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteListOptions PasteListOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteListOptions;
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
				this.PasteListOptions.RenameSpecificNodes = true;
				this.PasteListOptions.TaskCollection = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Specifies whether to update lists or not. Provide the value \"All\", or a comma-separated list of some of the following for only partial updating of the list: \"CoreMetadata\", \"Fields\", \"Views\", \"ContentTypes\".")]
		public string[] UpdateLists
		{
			get
			{
				return this.PasteListOptions.UpdateListOptionsForPowershell;
			}
			set
			{
				this.PasteListOptions.UpdateLists = true;
				this.PasteListOptions.UpdateListOptionsBitField = SystemUtils.GetFlagsEnumeratorFromStrings(typeof(UpdateListFlags), value);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should use the Azure SPO Migration Pipeline for documents. Use this only if you are certain about what you are doing.")]
		public new SwitchParameter UseAzureOffice365Upload
		{
			get
			{
				return this.PasteListOptions.UseAzureOffice365Upload;
			}
			set
			{
				this.PasteListOptions.UseAzureOffice365Upload = value;
			}
		}

		public CopyListCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}