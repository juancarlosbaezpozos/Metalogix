using Metalogix;
using Metalogix.Data.Filters;
using Metalogix.Utilities;
using System;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.SharePoint.Options.Transform
{
	public class SiteColumnToManagedMetadataOption : ManagedMetadataOption
	{
		private IFilterExpression m_siteFilterExpression;

		private IFilterExpression m_siteColumnFilterExpression;

		[LocalizedDisplayName("SCMMOSiteColumnFilter")]
		public string FieldFilter
		{
			get
			{
				if (this.m_siteColumnFilterExpression == null)
				{
					return string.Empty;
				}
				return this.m_siteColumnFilterExpression.GetLogicString();
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		public IFilterExpression SiteColumnFilterExpression
		{
			get
			{
				return this.m_siteColumnFilterExpression;
			}
			set
			{
				this.m_siteColumnFilterExpression = value;
			}
		}

		[LocalizedDisplayName("SCMMOSiteFilter")]
		public string SiteFilter
		{
			get
			{
				if (this.m_siteFilterExpression == null)
				{
					return string.Empty;
				}
				return this.m_siteFilterExpression.GetLogicString();
			}
		}

		[Bindable(false)]
		[Browsable(false)]
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

		public SiteColumnToManagedMetadataOption()
		{
			this.m_siteFilterExpression = null;
			this.m_siteColumnFilterExpression = null;
		}

		public SiteColumnToManagedMetadataOption(XmlNode node) : this()
		{
			this.FromXML(node);
		}

		public SiteColumnToManagedMetadataOption Copy()
		{
			return new SiteColumnToManagedMetadataOption(XmlUtility.StringToXmlNode(base.ToXML()));
		}
	}
}