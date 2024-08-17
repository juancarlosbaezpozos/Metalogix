using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Options.Reporting
{
	public class CompareSiteOptions : CompareListOptions
	{
		private bool m_bCompareLists = true;

		private bool m_bRecursive = true;

		private bool m_bFilterSites;

		private string m_sSiteFilterExpresssion = "<And />";

		public bool CompareLists
		{
			get
			{
				return this.m_bCompareLists;
			}
			set
			{
				this.m_bCompareLists = value;
			}
		}

		[CmdletParameterAlias("FilterSubSites")]
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

		[CmdletParameterAlias("CompareSubSites")]
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

		[CmdletParameterAlias("SubSiteFilterExpression")]
		public string SiteFilterExpression
		{
			get
			{
				return this.m_sSiteFilterExpresssion;
			}
			set
			{
				this.m_sSiteFilterExpresssion = value;
			}
		}

		public CompareSiteOptions()
		{
		}
	}
}