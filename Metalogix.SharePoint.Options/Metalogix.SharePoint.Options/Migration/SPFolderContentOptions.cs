using Metalogix;
using Metalogix.DataStructures.Generic;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPFolderContentOptions : OptionsBase
	{
		private bool _encryptAzureMigrationJobs = true;

		private bool _useAzureOffice365Upload = true;

		private bool m_bCopySubfolders = true;

		private bool m_bCopyListItems = true;

		private bool m_bCopyVersions = true;

		private bool m_bMaxVersions;

		private int m_iMaxVersionCount = 1;

		private bool m_bPreserveItemIDs = true;

		private bool m_bPreserveDocumentIDs;

		private bool m_bRenameList;

		private string m_sNewListName;

		private bool m_bReattachPageLayouts;

		private bool m_bApplyNewContentTypes;

		private CommonSerializableList<ContentTypeApplicationOptionsCollection> m_contentTypeApplicationOptions;

		private bool m_bApplyNewDocumentSets;

		private CommonSerializableList<DocumentSetApplicationOptionsCollection> m_documentSetApplicationOptions;

		private CommonSerializableList<DocumentSetFolderOptions> m_folderToDocumentSetApplicationObjects;

		private bool m_bShallowCopyExternalizedData;

		private bool m_bDisableDocumentParsing;

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

		public bool CopyListItems
		{
			get
			{
				return this.m_bCopyListItems;
			}
			set
			{
				this.m_bCopyListItems = value;
			}
		}

		public bool CopyMaxVersions
		{
			get
			{
				return this.m_bMaxVersions;
			}
			set
			{
				this.m_bMaxVersions = value;
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

		public CommonSerializableList<DocumentSetFolderOptions> FolderToDocumentSetApplicationObjects
		{
			get
			{
				return this.m_folderToDocumentSetApplicationObjects;
			}
			set
			{
				this.m_folderToDocumentSetApplicationObjects = value;
			}
		}

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

		public string NewFolderName
		{
			get
			{
				return this.m_sNewListName;
			}
			set
			{
				this.m_sNewListName = value;
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

		public bool RenameFolder
		{
			get
			{
				return this.m_bRenameList;
			}
			set
			{
				this.m_bRenameList = value;
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

		public SPFolderContentOptions()
		{
		}
	}
}