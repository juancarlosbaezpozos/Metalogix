using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Migration;
using System;

namespace Metalogix.SharePoint.Options
{
	public class PasteSiteColumnsOptions : PasteTaxonomyOptions
	{
		private bool m_bFilterSiteFields;

		private bool _resolveManagedMetadataByName = true;

		private bool m_bCopyReferencedManagedMetadata;

		private IFilterExpression m_siteFieldFilterExpression = new FilterExpressionList(ExpressionLogic.And);

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

		[CmdletEnabledParameter(true)]
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

		[CmdletEnabledParameter("ResolveManagedMetadataByName", true)]
		[CmdletParameterEnumerate(true)]
		public new bool ResolveManagedMetadataByName
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

		public PasteSiteColumnsOptions()
		{
		}
	}
}