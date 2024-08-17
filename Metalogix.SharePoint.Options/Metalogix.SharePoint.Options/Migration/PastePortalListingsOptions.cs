using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using Metalogix.Utilities;
using System;
using System.Globalization;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PastePortalListingsOptions : SharePointActionOptions
	{
		private bool m_bFilterLists;

		private IFilterExpression m_listFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private TransformationTaskCollection m_ttcTaskCollection = new TransformationTaskCollection();

		private bool m_bOverwriteLists = true;

		private bool m_bUpdateLists;

		private int m_iUpdateListOptions;

		private bool m_bCheckModifiedDatesForLists = true;

		private ListItemCopyMode m_ItemCopyingMode;

		private bool m_bUpdateItems;

		private bool m_bPropagateItemDeletions;

		[CmdletEnabledParameter(false)]
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
		public virtual bool OverwriteLists
		{
			get
			{
				return this.m_bOverwriteLists;
			}
			set
			{
				this.m_bOverwriteLists = value;
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

		public PastePortalListingsOptions()
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
				this.ListFilterExpression = filterExpression;
				this.FilterLists = true;
			}
		}
	}
}