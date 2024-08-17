using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.Utilities;
using System;
using System.Globalization;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteListOptions : PasteFolderOptions
	{
		private bool m_bCopyListPermissions = true;

		private bool m_bFilterLists;

		private IFilterExpression m_listFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bUpdateLists;

		private int m_iUpdateListOptions;

		private bool m_bCheckModifiedDatesForLists = true;

		private bool _copyCustomizedFormPages;

		private bool m_bCopyListOOBWorkflowAssociations = true;

		private bool m_bCopyWebOOBWorkflowAssociations;

		private bool m_bCopyContentTypeOOBWorkflowAssociations = true;

		private bool m_bCopyListSharePointDesignerNintexWorkflowAssociations = true;

		private bool m_bCopyWebSharePointDesignerNintexWorkflowAssociations;

		private bool m_bCopyContentTypeSharePointDesignerNintexWorkflowAssociations = true;

		private bool m_bCopyNintexDatabaseEntries;

		private bool _copyPreviousVersionOfWorkflowInstances;

		private bool _copyGloballyReusableWorkflowTemplates;

		private bool m_bCopyViewWebParts;

		private bool m_bCopyFormWebParts;

		public bool CheckModifiedDatesForLists
		{
			get
			{
				return this.m_bCheckModifiedDatesForLists;
			}
			set
			{
				this.m_bCheckModifiedDatesForLists = value;
			}
		}

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

		public bool CopyCustomizedFormPages
		{
			get
			{
				return this._copyCustomizedFormPages;
			}
			set
			{
				this._copyCustomizedFormPages = value;
			}
		}

		public bool CopyFormWebParts
		{
			get
			{
				return this.m_bCopyFormWebParts;
			}
			set
			{
				this.m_bCopyFormWebParts = value;
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

		public bool CopyListPermissions
		{
			get
			{
				return this.m_bCopyListPermissions;
			}
			set
			{
				this.m_bCopyListPermissions = value;
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
				return this._copyPreviousVersionOfWorkflowInstances;
			}
			set
			{
				this._copyPreviousVersionOfWorkflowInstances = value;
			}
		}

		public bool CopyViewWebParts
		{
			get
			{
				return this.m_bCopyViewWebParts;
			}
			set
			{
				this.m_bCopyViewWebParts = value;
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

		[CmdletEnabledParameter(false)]
		public bool FilterLists
		{
			get
			{
				return this.m_bFilterLists;
			}
			set
			{
				this.m_bFilterLists = value;
			}
		}

		[CmdletEnabledParameter("FilterLists", true)]
		public IFilterExpression ListFilterExpression
		{
			get
			{
				return this.m_listFilterExpression;
			}
			set
			{
				this.m_listFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public new bool OverwriteFolders
		{
			get
			{
				return base.OverwriteFolders;
			}
			set
			{
				base.OverwriteFolders = value;
			}
		}

		public override bool OverwriteLists
		{
			get
			{
				return base.OverwriteLists;
			}
			set
			{
				base.OverwriteLists = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public int UpdateListOptionsBitField
		{
			get
			{
				return this.m_iUpdateListOptions;
			}
			set
			{
				this.m_iUpdateListOptions = value;
			}
		}

		[CmdletParameterAlias("UpdateLists")]
		public string[] UpdateListOptionsForPowershell
		{
			get
			{
				if (!this.UpdateLists)
				{
					return new string[0];
				}
				return SystemUtils.GetFlagsEnumNamesAsArray(typeof(UpdateListFlags), this.UpdateListOptionsBitField);
			}
		}

		[CmdletEnabledParameter(false)]
		public bool UpdateLists
		{
			get
			{
				return this.m_bUpdateLists;
			}
			set
			{
				this.m_bUpdateLists = value;
			}
		}

		public PasteListOptions()
		{
		}

		public override void MakeOptionsIncremental(DateTime? incrementFromTime)
		{
			base.MakeOptionsIncremental(incrementFromTime);
			this.OverwriteLists = false;
			this.UpdateLists = true;
			this.UpdateListOptionsBitField = 23;
			if (incrementFromTime.HasValue)
			{
				DateTime value = incrementFromTime.Value;
				FilterExpression filterExpression = new FilterExpression(FilterOperand.GreaterThanOrEqualTo, typeof(SPList), "Modified", value.ToString(CultureInfo.CurrentCulture), false, false, CultureInfo.CurrentCulture);
				FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.And);
				if (this.FilterLists && this.ListFilterExpression != null)
				{
					filterExpressionList.Add(this.ListFilterExpression);
				}
				filterExpressionList.Add(filterExpression);
				this.ListFilterExpression = filterExpressionList;
				this.FilterLists = true;
			}
		}
	}
}