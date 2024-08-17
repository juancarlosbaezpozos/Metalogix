using Metalogix.Actions;
using Metalogix.Data.Filters;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteSiteListsOptions : PasteListOptions
	{
		private bool m_bCopySiteColumns;

		private bool m_bCopyContentTypes;

		private bool m_bFilterSiteFields;

		private IFilterExpression m_siteFieldFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		public bool CopyContentTypes
		{
			get
			{
				return this.m_bCopyContentTypes;
			}
			set
			{
				this.m_bCopyContentTypes = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopySiteColumns
		{
			get
			{
				return this.m_bCopySiteColumns;
			}
			set
			{
				this.m_bCopySiteColumns = value;
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

		[CmdletEnabledParameter("FilterSiteFields", true)]
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

		public PasteSiteListsOptions()
		{
		}
	}
}