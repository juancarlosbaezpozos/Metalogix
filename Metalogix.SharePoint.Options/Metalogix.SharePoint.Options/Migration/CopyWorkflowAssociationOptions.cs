using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class CopyWorkflowAssociationOptions : SharePointActionOptions
	{
		private bool m_bCopyListOOBWorkflowAssociations = true;

		private bool m_bCopyWebOOBWorkflowAssociations = true;

		private bool m_bCopyContentTypeOOBWorkflowAssociations = true;

		private bool m_bCopyListSharePointDesignerNintexWorkflowAssociations;

		private bool m_bCopyWebSharePointDesignerNintexWorkflowAssociations;

		private bool m_bCopyContentTypeSharePointDesignerNintexWorkflowAssociations;

		private bool m_bCopyWorkflowInstanceData = true;

		private bool m_bCopyInProgressWorkflows = true;

		private bool _copyPreviousVersionWorkflowInstances = true;

		private bool m_bCopyNintexDatabaseEntries;

		private bool _copyGloballyReusableWorkflowTemplates;

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

		public bool CopyGloballyReusableWorkflowTemplates
		{
			get
			{
				return this._copyGloballyReusableWorkflowTemplates;
			}
			set
			{
				this._copyGloballyReusableWorkflowTemplates = value;
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

		public bool CopyListOOBWorkflowAssociations
		{
			get
			{
				return this.m_bCopyListOOBWorkflowAssociations;
			}
			set
			{
				this.m_bCopyListOOBWorkflowAssociations = value;
			}
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

		public bool CopyNintexDatabaseEntries
		{
			get
			{
				return this.m_bCopyNintexDatabaseEntries;
			}
			set
			{
				this.m_bCopyNintexDatabaseEntries = value;
			}
		}

		public bool CopyPreviousVersionOfWorkflowInstances
		{
			get
			{
				return this._copyPreviousVersionWorkflowInstances;
			}
			set
			{
				this._copyPreviousVersionWorkflowInstances = value;
			}
		}

		public bool CopyWebOOBWorkflowAssociations
		{
			get
			{
				return this.m_bCopyWebOOBWorkflowAssociations;
			}
			set
			{
				this.m_bCopyWebOOBWorkflowAssociations = value;
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

		public CopyWorkflowAssociationOptions()
		{
		}
	}
}