using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPFilterOptions : OptionsBase
	{
		private bool m_bFilterItems;

		private bool m_bFilterFolders;

		private bool m_bFilterLists;

		private bool m_bFilterSites;

		private bool m_bFilterFields;

		private bool m_bFilterSiteFields;

		private bool m_bFilterCustomFolders;

		private bool m_bFilterCustomFiles;

		private IFilterExpression m_itemFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_folderFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_listFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_listFieldFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_siteFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_siteFieldFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_customFolderFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private IFilterExpression m_customFileFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		[CmdletEnabledParameter(false)]
		public IFilterExpression CustomFileFilterExpression
		{
			get
			{
				return this.m_customFileFilterExpression;
			}
			set
			{
				this.m_customFileFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public IFilterExpression CustomFolderFilterExpression
		{
			get
			{
				return this.m_customFolderFilterExpression;
			}
			set
			{
				this.m_customFolderFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterCustomFiles
		{
			get
			{
				return this.m_bFilterCustomFiles;
			}
			set
			{
				this.m_bFilterCustomFiles = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterCustomFolders
		{
			get
			{
				return this.m_bFilterCustomFolders;
			}
			set
			{
				this.m_bFilterCustomFolders = value;
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
		public bool FilterFolders
		{
			get
			{
				return this.m_bFilterFolders;
			}
			set
			{
				this.m_bFilterFolders = value;
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
		public bool FilterSiteFields
		{
			get
			{
				return this.m_bFilterSiteFields;
			}
			set
			{
				this.m_bFilterSiteFields = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterSites
		{
			get
			{
				return this.m_bFilterSites;
			}
			set
			{
				this.m_bFilterSites = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public IFilterExpression FolderFilterExpression
		{
			get
			{
				return this.m_folderFilterExpression;
			}
			set
			{
				this.m_folderFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
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

		[CmdletEnabledParameter(false)]
		public IFilterExpression ListFieldsFilterExpression
		{
			get
			{
				return this.m_listFieldFilterExpression;
			}
			set
			{
				this.m_listFieldFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
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
		public IFilterExpression SiteFieldsFilterExpression
		{
			get
			{
				return this.m_siteFieldFilterExpression;
			}
			set
			{
				this.m_siteFieldFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public IFilterExpression SiteFilterExpression
		{
			get
			{
				return this.m_siteFilterExpression;
			}
			set
			{
				this.m_siteFilterExpression = value;
			}
		}

		public SPFilterOptions()
		{
		}
	}
}