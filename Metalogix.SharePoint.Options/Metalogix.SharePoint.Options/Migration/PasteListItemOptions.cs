using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.Utilities;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteListItemOptions : SharePointActionOptions
	{
		private bool m_bCopyDocumentWebParts = true;

		private bool m_bCopyClosedWebParts;

		private bool m_bCopyContentZoneContent = true;

		private ExistingWebPartsProtocol m_ExistingWebPartsAction = ExistingWebPartsProtocol.Delete;

		private bool m_bCopyWebPartsAtItemsLevel = true;

		private bool m_bCopyDefaultPageWebPartsAtItemsLevel = true;

		private bool m_bFilterItems;

		private bool m_bFilterFields;

		private IFilterExpression m_itemFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_listFieldsFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bRenameSpecificNodes;

		private TransformationTaskCollection m_ttcTaskCollection = new TransformationTaskCollection();

		private bool m_bMapColumns;

		private Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings m_bColumnMappings = new Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings();

		private bool m_bMapTermStores;

		private bool m_bCopyRootPermissions = true;

		private bool m_bMapRolesByName = true;

		private bool m_bClearRoleAssignments = true;

		private bool m_bOverrideRoleMappings;

		private ConditionalMappingCollection m_roleAssignmentMappings = new ConditionalMappingCollection();

		private bool m_bCopyItemPermissions = true;

		private ListItemCopyMode m_ItemCopyingMode;

		private bool m_bUpdateItems;

		private bool m_bCheckModifiedDatesForItemsDocuments = true;

		private int m_iUpdateItemsOptions;

		private bool m_bPropagateItemDeletions;

		private bool m_bReattachPageLayouts;

		private bool m_bCopyVersions = true;

		private bool m_bCopySubfolders = true;

		private int m_iMaxVersionCount = 1;

		private bool m_bPreserveItemIDs = true;

		private bool m_bPreserveDocumentIDs;

		private bool m_bPreserveSharePointDocumentIDs;

		private bool m_bShallowCopyExternalizedData;

		private bool m_bDisableDocumentParsing;

		private bool m_bApplyNewContentTypes;

		private CommonSerializableList<ContentTypeApplicationOptionsCollection> m_contentTypeApplicationOptions;

		private bool m_bApplyNewDocumentSets;

		private CommonSerializableList<DocumentSetApplicationOptionsCollection> m_documentSetApplicationOptions;

		private CommonSerializableList<DocumentSetFolderOptions> m_folderDocumentSetApplicationOptions;

		private bool m_bCopyWorkflowInstanceData;

		private bool m_bCopyInProgressWorkflows;

		private bool _resolveManagedMetadataByName = true;

		private bool m_bCopyReferencedManagedMetadata;

		private CommonSerializableTable<string, string> m_termstoreNameMappingTable;

		private bool m_bSideLoadDocuments;

		private bool _useAzureOffice365Upload = true;

		private bool _encryptAzureMigrationJobs = true;

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public bool ApplyNewContentTypes
		{
			get
			{
				return this.m_bApplyNewContentTypes;
			}
			set
			{
				this.m_bApplyNewContentTypes = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public bool ApplyNewDocumentSets
		{
			get
			{
				return this.m_bApplyNewDocumentSets;
			}
			set
			{
				this.m_bApplyNewDocumentSets = value;
			}
		}

		public bool CheckModifiedDatesForItemsDocuments
		{
			get
			{
				return this.m_bCheckModifiedDatesForItemsDocuments;
			}
			set
			{
				this.m_bCheckModifiedDatesForItemsDocuments = value;
			}
		}

		public bool ClearRoleAssignments
		{
			get
			{
				return this.m_bClearRoleAssignments;
			}
			set
			{
				this.m_bClearRoleAssignments = value;
			}
		}

		[CmdletEnabledParameter("MapColumns", true)]
		[CmdletParameterEnumerate(true)]
		public Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings ColumnMappings
		{
			get
			{
				return this.m_bColumnMappings;
			}
			set
			{
				this.m_bColumnMappings = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool ContainsTransformationTasks
		{
			get
			{
				return this.m_ttcTaskCollection.Count > 0;
			}
		}

		[CmdletEnabledParameter("ApplyNewContentTypes", true)]
		[CmdletParameterEnumerate(true)]
		[UsesStickySettings(false)]
		public CommonSerializableList<ContentTypeApplicationOptionsCollection> ContentTypeApplicationObjects
		{
			get
			{
				return this.m_contentTypeApplicationOptions;
			}
			set
			{
				this.m_contentTypeApplicationOptions = value;
			}
		}

		public bool CopyClosedWebParts
		{
			get
			{
				return this.m_bCopyClosedWebParts;
			}
			set
			{
				this.m_bCopyClosedWebParts = value;
			}
		}

		public bool CopyContentZoneContent
		{
			get
			{
				return this.m_bCopyContentZoneContent;
			}
			set
			{
				this.m_bCopyContentZoneContent = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopyDefaultPageWebPartsAtItemsLevel
		{
			get
			{
				return this.m_bCopyDefaultPageWebPartsAtItemsLevel;
			}
			set
			{
				this.m_bCopyDefaultPageWebPartsAtItemsLevel = value;
			}
		}

		public bool CopyDocumentWebParts
		{
			get
			{
				return this.m_bCopyDocumentWebParts;
			}
			set
			{
				this.m_bCopyDocumentWebParts = value;
			}
		}

		public bool CopyInProgressWorkflows
		{
			get
			{
				return this.m_bCopyInProgressWorkflows;
			}
			set
			{
				this.m_bCopyInProgressWorkflows = value;
			}
		}

		public bool CopyItemPermissions
		{
			get
			{
				return this.m_bCopyItemPermissions;
			}
			set
			{
				this.m_bCopyItemPermissions = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopyMaxVersions
		{
			get;
			set;
		}

		[CmdletEnabledParameter("CopyReferencedManagedMetadata", true)]
		[CmdletParameterEnumerate(true)]
		public bool CopyReferencedManagedMetadata
		{
			get
			{
				return this.m_bCopyReferencedManagedMetadata;
			}
			set
			{
				this.m_bCopyReferencedManagedMetadata = value;
			}
		}

		public bool CopyRootPermissions
		{
			get
			{
				return this.m_bCopyRootPermissions;
			}
			set
			{
				this.m_bCopyRootPermissions = value;
			}
		}

		public bool CopySubFolders
		{
			get
			{
				return this.m_bCopySubfolders;
			}
			set
			{
				this.m_bCopySubfolders = value;
			}
		}

		public bool CopyVersions
		{
			get
			{
				return this.m_bCopyVersions;
			}
			set
			{
				this.m_bCopyVersions = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopyWebPartsAtItemsLevel
		{
			get
			{
				return this.m_bCopyWebPartsAtItemsLevel;
			}
			set
			{
				this.m_bCopyWebPartsAtItemsLevel = value;
			}
		}

		public bool CopyWorkflowInstanceData
		{
			get
			{
				return this.m_bCopyWorkflowInstanceData;
			}
			set
			{
				this.m_bCopyWorkflowInstanceData = value;
			}
		}

		public bool DisableDocumentParsing
		{
			get
			{
				return this.m_bDisableDocumentParsing;
			}
			set
			{
				this.m_bDisableDocumentParsing = value;
			}
		}

		[CmdletEnabledParameter("ApplyNewDocumentSets", true)]
		[CmdletParameterEnumerate(true)]
		[UsesStickySettings(false)]
		public CommonSerializableList<DocumentSetApplicationOptionsCollection> DocumentSetApplicationObjects
		{
			get
			{
				return this.m_documentSetApplicationOptions;
			}
			set
			{
				this.m_documentSetApplicationOptions = value;
			}
		}

		public bool EncryptAzureMigrationJobs
		{
			get
			{
				return this._encryptAzureMigrationJobs;
			}
			set
			{
				this._encryptAzureMigrationJobs = value;
			}
		}

		[CmdletEnabledParameter("ExistingWebPartsActionAllowed", true)]
		public ExistingWebPartsProtocol ExistingWebPartsAction
		{
			get
			{
				return this.m_ExistingWebPartsAction;
			}
			set
			{
				this.m_ExistingWebPartsAction = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool ExistingWebPartsActionAllowed
		{
			get
			{
				return this.CopyDocumentWebParts;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterFields
		{
			get
			{
				return this.m_bFilterFields;
			}
			set
			{
				this.m_bFilterFields = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterItems
		{
			get
			{
				return this.m_bFilterItems;
			}
			set
			{
				this.m_bFilterItems = value;
			}
		}

		[CmdletEnabledParameter("ApplyNewDocumentSets", true)]
		[CmdletParameterEnumerate(true)]
		[UsesStickySettings(false)]
		public CommonSerializableList<DocumentSetFolderOptions> FolderToDocumentSetApplicationObjects
		{
			get
			{
				return this.m_folderDocumentSetApplicationOptions;
			}
			set
			{
				this.m_folderDocumentSetApplicationOptions = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public ListItemCopyMode ItemCopyingMode
		{
			get
			{
				return this.m_ItemCopyingMode;
			}
			set
			{
				this.m_ItemCopyingMode = value;
			}
		}

		[CmdletEnabledParameter("FilterItems", true)]
		public IFilterExpression ItemFilterExpression
		{
			get
			{
				return this.m_itemFilterExpression;
			}
			set
			{
				this.m_itemFilterExpression = value;
			}
		}

		[CmdletEnabledParameter("FilterFields", true)]
		public IFilterExpression ListFieldsFilterExpression
		{
			get
			{
				return this.m_listFieldsFilterExpression;
			}
			set
			{
				this.m_listFieldsFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool MapColumns
		{
			get
			{
				return this.m_bMapColumns;
			}
			set
			{
				this.m_bMapColumns = value;
			}
		}

		public bool MapRolesByName
		{
			get
			{
				return this.m_bMapRolesByName;
			}
			set
			{
				this.m_bMapRolesByName = value;
			}
		}

		[CmdletEnabledParameter(true)]
		public bool MapTermStores
		{
			get
			{
				return this.m_bMapTermStores;
			}
			set
			{
				this.m_bMapTermStores = value;
			}
		}

		[CmdletEnabledParameter("MaximumVersionsAreBeingCopied", true)]
		public int MaximumVersionCount
		{
			get
			{
				return this.m_iMaxVersionCount;
			}
			set
			{
				this.m_iMaxVersionCount = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool MaximumVersionsAreBeingCopied
		{
			get
			{
				if (!this.CopyVersions)
				{
					return false;
				}
				return this.CopyMaxVersions;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool OverrideRoleMappings
		{
			get
			{
				return this.m_bOverrideRoleMappings;
			}
			set
			{
				this.m_bOverrideRoleMappings = value;
			}
		}

		[CmdletParameterAlias("OverwriteItems")]
		public bool OverwriteItemsForPowerShell
		{
			get
			{
				return this.ItemCopyingMode == ListItemCopyMode.Overwrite;
			}
		}

		public bool PreserveDocumentIDs
		{
			get
			{
				return this.m_bPreserveDocumentIDs;
			}
			set
			{
				this.m_bPreserveDocumentIDs = value;
			}
		}

		public bool PreserveItemIDs
		{
			get
			{
				return this.m_bPreserveItemIDs;
			}
			set
			{
				this.m_bPreserveItemIDs = value;
			}
		}

		public bool PreserveSharePointDocumentIDs
		{
			get
			{
				return this.m_bPreserveSharePointDocumentIDs;
			}
			set
			{
				this.m_bPreserveSharePointDocumentIDs = value;
			}
		}

		public bool PropagateItemDeletions
		{
			get
			{
				return this.m_bPropagateItemDeletions;
			}
			set
			{
				this.m_bPropagateItemDeletions = value;
			}
		}

		public bool ReattachPageLayouts
		{
			get
			{
				return this.m_bReattachPageLayouts;
			}
			set
			{
				this.m_bReattachPageLayouts = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public bool RenameSpecificNodes
		{
			get
			{
				return this.m_bRenameSpecificNodes;
			}
			set
			{
				this.m_bRenameSpecificNodes = value;
			}
		}

		[CmdletEnabledParameter("ResolveManagedMetadataByName", true)]
		[CmdletParameterEnumerate(true)]
		public bool ResolveManagedMetadataByName
		{
			get
			{
				return this._resolveManagedMetadataByName;
			}
			set
			{
				this._resolveManagedMetadataByName = value;
			}
		}

		[CmdletEnabledParameter("OverrideRoleMappings", true)]
		[CmdletParameterEnumerate(true)]
		public ConditionalMappingCollection RoleAssignmentMappings
		{
			get
			{
				return this.m_roleAssignmentMappings;
			}
			set
			{
				this.m_roleAssignmentMappings = value;
			}
		}

		public bool ShallowCopyExternalizedData
		{
			get
			{
				return false;
			}
			set
			{
				this.m_bShallowCopyExternalizedData = false;
			}
		}

		public bool SideLoadDocuments
		{
			get
			{
				return this.m_bSideLoadDocuments;
			}
			set
			{
				this.m_bSideLoadDocuments = value;
			}
		}

		[CmdletEnabledParameter("ContainsTransformationTasks", true)]
		[UsesStickySettings(false)]
		public TransformationTaskCollection TaskCollection
		{
			get
			{
				return this.m_ttcTaskCollection;
			}
			set
			{
				this.m_ttcTaskCollection = value;
			}
		}

		[CmdletEnabledParameter("MapTermStores", true)]
		public CommonSerializableTable<string, string> TermstoreNameMappingTable
		{
			get
			{
				if (this.m_termstoreNameMappingTable == null)
				{
					this.m_termstoreNameMappingTable = new CommonSerializableTable<string, string>();
				}
				return this.m_termstoreNameMappingTable;
			}
			set
			{
				this.m_termstoreNameMappingTable = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public int UpdateItemOptionsBitField
		{
			get
			{
				return this.m_iUpdateItemsOptions;
			}
			set
			{
				this.m_iUpdateItemsOptions = value;
			}
		}

		[CmdletParameterAlias("UpdateItems")]
		public string[] UpdateItemOptionsForPowerShell
		{
			get
			{
				if (!this.UpdateItems)
				{
					return new string[0];
				}
				return SystemUtils.GetFlagsEnumNamesAsArray(typeof(UpdateItemsFlags), this.UpdateItemOptionsBitField);
			}
		}

		[CmdletEnabledParameter(false)]
		public bool UpdateItems
		{
			get
			{
				return this.m_bUpdateItems;
			}
			set
			{
				this.m_bUpdateItems = value;
			}
		}

		public bool UseAzureOffice365Upload
		{
			get
			{
				return this._useAzureOffice365Upload;
			}
			set
			{
				this._useAzureOffice365Upload = value;
			}
		}

		public PasteListItemOptions()
		{
		}

		public override void MakeOptionsIncremental(DateTime? incrementFromTime)
		{
			base.MakeOptionsIncremental(incrementFromTime);
			this.ItemCopyingMode = ListItemCopyMode.Preserve;
			this.UpdateItems = true;
			this.UpdateItemOptionsBitField = 1;
			this.PreserveItemIDs = true;
			this.ClearRoleAssignments = false;
			if (this.CopyDocumentWebParts)
			{
				this.ExistingWebPartsAction = ExistingWebPartsProtocol.Delete;
			}
			if (incrementFromTime.HasValue)
			{
				DateTime value = incrementFromTime.Value;
				FilterExpression filterExpression = new FilterExpression(FilterOperand.GreaterThanOrEqualTo, typeof(SPListItem), "Modified", value.ToString(CultureInfo.CurrentCulture), false, false, CultureInfo.CurrentCulture);
				FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.And);
				if (this.FilterItems && this.ItemFilterExpression != null)
				{
					filterExpressionList.Add(this.ItemFilterExpression);
				}
				filterExpressionList.Add(filterExpression);
				this.ItemFilterExpression = filterExpressionList;
				this.FilterItems = true;
			}
		}

		public virtual void ToggleOptionsForMultiSelectCase()
		{
			this.MapColumns = false;
			this.ColumnMappings = new Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings();
		}
	}
}