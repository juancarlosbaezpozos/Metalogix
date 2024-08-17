using Metalogix.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Reporting
{
	public class CompareItemOptions : SharePointActionOptions
	{
		private bool m_bConfigured;

		private bool m_bCompareVersions = true;

		private bool m_bHaltIfDifferent;

		private bool m_bFilterItems;

		private string m_sItemFilterExpresssion = "<And />";

		[CmdletParameterAlias("CompareMetadata")]
		public override bool CheckResults
		{
			get
			{
				return base.CheckResults;
			}
			set
			{
				base.CheckResults = value;
			}
		}

		public bool CompareVersions
		{
			get
			{
				return this.m_bCompareVersions;
			}
			set
			{
				this.m_bCompareVersions = value;
			}
		}

		[UsesStickySettings(false)]
		public bool Configured
		{
			get
			{
				return this.m_bConfigured;
			}
			set
			{
				this.m_bConfigured = value;
			}
		}

		[CmdletParameterAlias("FilterItemsAndVersions")]
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

		public bool HaltIfDifferent
		{
			get
			{
				return this.m_bHaltIfDifferent;
			}
			set
			{
				this.m_bHaltIfDifferent = value;
			}
		}

		[CmdletParameterAlias("ItemAndVersionFilterExpression")]
		public string ItemFilterExpression
		{
			get
			{
				return this.m_sItemFilterExpresssion;
			}
			set
			{
				this.m_sItemFilterExpresssion = value;
			}
		}

		public CompareItemOptions()
		{
		}
	}
}