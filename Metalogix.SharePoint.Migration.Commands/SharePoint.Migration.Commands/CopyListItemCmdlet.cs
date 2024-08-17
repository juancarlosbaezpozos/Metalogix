using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.Utilities;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointItem")]
	public class CopyListItemCmdlet : SharePointActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteListItemAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if only newer items/documents will be copied.")]
		public SwitchParameter CheckModifiedDatesForItemsDocuments
		{
			get
			{
				return this.PasteItemOptions.CheckModifiedDatesForItemsDocuments;
			}
			set
			{
				this.PasteItemOptions.CheckModifiedDatesForItemsDocuments = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operations should clear any existing role assignments prior to copying the source role assignments.")]
		public SwitchParameter ClearRoleAssignments
		{
			get
			{
				return this.PasteItemOptions.ClearRoleAssignments;
			}
			set
			{
				this.PasteItemOptions.ClearRoleAssignments = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a set of column mappings for the item copying operation.")]
		public Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings ColumnMappings
		{
			get
			{
				return this.PasteItemOptions.ColumnMappings;
			}
			set
			{
				this.PasteItemOptions.ColumnMappings = value;
				this.PasteItemOptions.MapColumns = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a set of content type application rules for the item copying operation.")]
		public CommonSerializableList<ContentTypeApplicationOptionsCollection> ContentTypeApplicationObjects
		{
			get
			{
				return this.PasteItemOptions.ContentTypeApplicationObjects;
			}
			set
			{
				this.PasteItemOptions.ContentTypeApplicationObjects = value;
				this.PasteItemOptions.ApplyNewContentTypes = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether closed web parts from the source are copied to the target or not.")]
		public SwitchParameter CopyClosedWebParts
		{
			get
			{
				return this.PasteItemOptions.CopyClosedWebParts;
			}
			set
			{
				this.PasteItemOptions.CopyClosedWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether content zone content should be updated on the target as part of the copy.")]
		public SwitchParameter CopyContentZoneContent
		{
			get
			{
				return this.PasteItemOptions.CopyContentZoneContent;
			}
			set
			{
				this.PasteItemOptions.CopyContentZoneContent = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if web parts on web part pages in document libraries or pages libraries should be copied.")]
		public SwitchParameter CopyDocumentWebParts
		{
			get
			{
				return this.PasteItemOptions.CopyDocumentWebParts;
			}
			set
			{
				this.PasteItemOptions.CopyDocumentWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether or not to copy in progress workflow instances.")]
		public SwitchParameter CopyInProgressWorkflows
		{
			get
			{
				return this.PasteItemOptions.CopyInProgressWorkflows;
			}
			set
			{
				this.PasteItemOptions.CopyInProgressWorkflows = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for list item and documents.")]
		public SwitchParameter CopyItemPermissions
		{
			get
			{
				return this.PasteItemOptions.CopyItemPermissions;
			}
			set
			{
				this.PasteItemOptions.CopyItemPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether or not to copy referenced taxonomy.")]
		public SwitchParameter CopyReferencedManagedMetadata
		{
			get
			{
				return this.PasteItemOptions.CopyReferencedManagedMetadata;
			}
			set
			{
				this.PasteItemOptions.CopyReferencedManagedMetadata = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy the permissions for the highest-level objects copied, regardless of inheritance.")]
		public SwitchParameter CopyRootPermissions
		{
			get
			{
				return this.PasteItemOptions.CopyRootPermissions;
			}
			set
			{
				this.PasteItemOptions.CopyRootPermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include subfolders.")]
		public SwitchParameter CopySubFolders
		{
			get
			{
				return this.PasteItemOptions.CopySubFolders;
			}
			set
			{
				this.PasteItemOptions.CopySubFolders = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include list item and document versions.")]
		public SwitchParameter CopyVersions
		{
			get
			{
				return this.PasteItemOptions.CopyVersions;
			}
			set
			{
				this.PasteItemOptions.CopyVersions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether or not to copy workflow instances.")]
		public SwitchParameter CopyWorkflowInstanceData
		{
			get
			{
				return this.PasteItemOptions.CopyWorkflowInstanceData;
			}
			set
			{
				this.PasteItemOptions.CopyWorkflowInstanceData = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that SharePoint's automatic promotion and demotion of metadata for defined file types should be disabled during migration.")]
		public SwitchParameter DisableDocumentParsing
		{
			get
			{
				return this.PasteItemOptions.DisableDocumentParsing;
			}
			set
			{
				this.PasteItemOptions.DisableDocumentParsing = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a set of document set application rules for the item copying operation.")]
		public CommonSerializableList<DocumentSetApplicationOptionsCollection> DocumentSetApplicationObjects
		{
			get
			{
				return this.PasteItemOptions.DocumentSetApplicationObjects;
			}
			set
			{
				this.PasteItemOptions.DocumentSetApplicationObjects = value;
				this.PasteItemOptions.ApplyNewDocumentSets = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should update items on the target with new item data from the source. The comparison for item matching is done by name for document libraries and by ID for all other lists.")]
		public ExistingWebPartsProtocol ExistingWebPartsAction
		{
			get
			{
				return this.PasteItemOptions.ExistingWebPartsAction;
			}
			set
			{
				this.PasteItemOptions.ExistingWebPartsAction = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a set of document set application rules for the item copying operation.")]
		public CommonSerializableList<DocumentSetFolderOptions> FolderToDocumentSetApplicationObjects
		{
			get
			{
				return this.PasteItemOptions.FolderToDocumentSetApplicationObjects;
			}
			set
			{
				this.PasteItemOptions.FolderToDocumentSetApplicationObjects = value;
				this.PasteItemOptions.ApplyNewDocumentSets = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a document or item should be copied.")]
		public IFilterExpression ItemFilterExpression
		{
			get
			{
				if (!this.PasteItemOptions.FilterItems)
				{
					return null;
				}
				return this.PasteItemOptions.ItemFilterExpression;
			}
			set
			{
				this.PasteItemOptions.FilterItems = true;
				this.PasteItemOptions.ItemFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a field filter expression used to determine if a item field should be copied.")]
		public IFilterExpression ListFieldsFilterExpression
		{
			get
			{
				if (!this.PasteItemOptions.FilterFields)
				{
					return null;
				}
				return this.PasteItemOptions.ListFieldsFilterExpression;
			}
			set
			{
				this.PasteItemOptions.FilterFields = true;
				this.PasteItemOptions.ListFieldsFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operations should map role assignments strictly by name, skipping role assignments without a match.")]
		public SwitchParameter MapRolesByName
		{
			get
			{
				return this.PasteItemOptions.MapRolesByName;
			}
			set
			{
				this.PasteItemOptions.MapRolesByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether or not to map term stores within the copy.")]
		public SwitchParameter MapTermStores
		{
			get
			{
				return this.PasteItemOptions.MapTermStores;
			}
			set
			{
				this.PasteItemOptions.MapTermStores = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a maximum number of versions to copy for each item. Note that this has no effect if version copying is not requested.")]
		public int MaximumVersionCount
		{
			get
			{
				if (!this.PasteItemOptions.CopyMaxVersions)
				{
					return -1;
				}
				return this.PasteItemOptions.MaximumVersionCount;
			}
			set
			{
				this.PasteItemOptions.CopyMaxVersions = true;
				this.PasteItemOptions.MaximumVersionCount = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should overwrite existing items and documents. Overwriting items takes precedence over the 'UpdateItems' parameter.")]
		public SwitchParameter OverwriteItems
		{
			get
			{
				return this.PasteItemOptions.ItemCopyingMode == ListItemCopyMode.Overwrite;
			}
			set
			{
				this.PasteItemOptions.ItemCopyingMode = (value ? ListItemCopyMode.Overwrite : ListItemCopyMode.Preserve);
			}
		}

		protected virtual PasteListItemOptions PasteItemOptions
		{
			get
			{
				return base.Action.Options as PasteListItemOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should preserve IDs for documents in libraries. Note that this requires writing directly to the database.\nPlease contact support@metalogix.com if you have any questions about this process.")]
		public SwitchParameter PreserveDocumentIDs
		{
			get
			{
				return this.PasteItemOptions.PreserveDocumentIDs;
			}
			set
			{
				this.PasteItemOptions.PreserveDocumentIDs = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should preserve IDs for list items in lists.")]
		public SwitchParameter PreserveItemIDs
		{
			get
			{
				return this.PasteItemOptions.PreserveItemIDs;
			}
			set
			{
				this.PasteItemOptions.PreserveItemIDs = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should preserve SharePoint Document IDs(from the SharePoint Document ID Feature) for documents.")]
		public SwitchParameter PreserveSharePointDocumentIDs
		{
			get
			{
				return this.PasteItemOptions.PreserveSharePointDocumentIDs;
			}
			set
			{
				this.PasteItemOptions.PreserveSharePointDocumentIDs = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if items not appearing on the source should be deleted off of the target. The comparison is done by name for document libraries and by ID for all other lists.")]
		public SwitchParameter PropagateItemDeletions
		{
			get
			{
				return this.PasteItemOptions.PropagateItemDeletions;
			}
			set
			{
				this.PasteItemOptions.PropagateItemDeletions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should reattach the page layouts of any publishing pages with detached layouts as it copies them.")]
		public SwitchParameter ReattachPageLayouts
		{
			get
			{
				return this.PasteItemOptions.ReattachPageLayouts;
			}
			set
			{
				this.PasteItemOptions.ReattachPageLayouts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that mapping of terms should be done by name and hierarchy, rather than guid.")]
		public SwitchParameter ResolveManagedMetadataByName
		{
			get
			{
				return this.PasteItemOptions.ResolveManagedMetadataByName;
			}
			set
			{
				this.PasteItemOptions.ResolveManagedMetadataByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a collection of name to name role mappings, as well as conditional logic used to determine which objects to apply them on.")]
		public ConditionalMappingCollection RoleAssignmentMappings
		{
			get
			{
				return this.PasteItemOptions.RoleAssignmentMappings;
			}
			set
			{
				this.PasteItemOptions.RoleAssignmentMappings = value;
				this.PasteItemOptions.OverrideRoleMappings = true;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that only blob references should be copied for any content externalized with EBS or RBS. Note that this setting may be overridden by a mandatory setting depending on adapter.")]
		public SwitchParameter ShallowCopyExternalizedData
		{
			get
			{
				return false;
			}
			set
			{
				this.PasteItemOptions.ShallowCopyExternalizedData = false;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that documents will be added directly to StoragePoint if an endpoint is configured.")]
		public SwitchParameter SideLoadDocuments
		{
			get
			{
				return this.PasteItemOptions.SideLoadDocuments;
			}
			set
			{
				this.PasteItemOptions.SideLoadDocuments = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a collection of transformation tasks, such as renaming, to be applied to the data as it is copied.")]
		public virtual TransformationTaskCollection TaskCollection
		{
			get
			{
				return this.PasteItemOptions.TaskCollection;
			}
			set
			{
				this.PasteItemOptions.TaskCollection = value;
				if (value != null && value.Count > 0)
				{
					this.PasteItemOptions.RenameSpecificNodes = true;
				}
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The term store mapping table to be used in the copy mapping.")]
		public object TermstoreNameMappingTable
		{
			get
			{
				return this.PasteItemOptions.TermstoreNameMappingTable;
			}
			set
			{
				PSObject pSObject = value as PSObject;
				if (pSObject != null)
				{
					this.PasteItemOptions.TermstoreNameMappingTable = (CommonSerializableTable<string, string>)pSObject.ImmediateBaseObject;
					this.PasteItemOptions.MapTermStores = true;
				}
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should update items on the target with new item data from the source. The comparison for item matching is done by name for document libraries and by ID for all other lists.")]
		public string[] UpdateItems
		{
			get
			{
				return this.PasteItemOptions.UpdateItemOptionsForPowerShell;
			}
			set
			{
				this.PasteItemOptions.UpdateItems = true;
				this.PasteItemOptions.UpdateItemOptionsBitField = SystemUtils.GetFlagsEnumeratorFromStrings(typeof(UpdateItemsFlags), value);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should use the Azure SPO Migration Pipeline for documents. Use this only if you are certain about what you are doing.")]
		public SwitchParameter UseAzureOffice365Upload
		{
			get
			{
				return this.PasteItemOptions.UseAzureOffice365Upload;
			}
			set
			{
				this.PasteItemOptions.UseAzureOffice365Upload = value;
			}
		}

		public CopyListItemCmdlet()
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
			if (typeof(SPNode).IsAssignableFrom(base.Source.GetType()) && typeof(SPNode).IsAssignableFrom(base.Target.GetType()))
			{
				ExternalizationSupport externalizationSupport = ((SPNode)base.Source).Adapter.ExternalizationSupport;
				ExternalizationSupport externalizationSupport1 = ((SPNode)base.Target).Adapter.ExternalizationSupport;
				if (externalizationSupport == ExternalizationSupport.NotSupported || externalizationSupport == ExternalizationSupport.Required || externalizationSupport1 == ExternalizationSupport.NotSupported)
				{
					this.ShallowCopyExternalizedData = false;
				}
				else
				{
				}
			}
			return base.ProcessParameters();
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}

		protected override void TurnOffSwitches(object optionsObject)
		{
			base.TurnOffSwitches(optionsObject);
			PasteListItemOptions pasteListItemOption = optionsObject as PasteListItemOptions;
			if (pasteListItemOption != null)
			{
				pasteListItemOption.ItemCopyingMode = ListItemCopyMode.Preserve;
			}
		}
	}
}