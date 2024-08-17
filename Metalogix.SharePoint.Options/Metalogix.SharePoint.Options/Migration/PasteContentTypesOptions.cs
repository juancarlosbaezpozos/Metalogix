using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteContentTypesOptions : PasteTaxonomyOptions
	{
		private bool m_bRecursive;

		private bool m_bFilter;

		private bool m_bCopyReferencedManagedMetadata;

		private bool m_bCopyContentTypeOOBWorkflowAssociations;

		private bool m_bCopyListSharePointDesignerNintexWorkflowAssociations;

		private bool m_bCopyWebSharePointDesignerNintexWorkflowAssociations;

		private bool m_bCopyContentTypeSharePointDesignerNintexWorkflowAssociations;

		private CommonSerializableList<string> _filteredCtCollection = new CommonSerializableList<string>();

		public bool CopyContentTypeOOBWorkflowAssociations
		{
			get
			{
				return this.m_bCopyContentTypeOOBWorkflowAssociations;
			}
			set
			{
				this.m_bCopyContentTypeOOBWorkflowAssociations = value;
			}
		}

		public bool CopyContentTypeSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.m_bCopyContentTypeSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.m_bCopyContentTypeSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		public bool CopyFormWebParts
		{
			get;
			set;
		}

		public bool CopyListSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.m_bCopyListSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.m_bCopyListSharePointDesignerNintexWorkflowAssociations = value;
			}
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

		public bool CopyWebSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.m_bCopyWebSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.m_bCopyWebSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		public bool FilterCTs
		{
			get
			{
				return this.m_bFilter;
			}
			set
			{
				this.m_bFilter = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public CommonSerializableList<string> FilteredCTCollection
		{
			get
			{
				return this._filteredCtCollection;
			}
			set
			{
				this._filteredCtCollection = value;
			}
		}

		public bool Recursive
		{
			get
			{
				return this.m_bRecursive;
			}
			set
			{
				this.m_bRecursive = value;
			}
		}

		public PasteContentTypesOptions()
		{
		}
	}
}