using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Administration.CheckLinks
{
	public class CheckLinksOptions : SharePointActionOptions
	{
		private bool m_bCheckSubsites = true;

		private bool m_bCheckWebparts;

		private bool m_bCheckTextFields;

		private bool m_bShowSuccesses;

		private int m_iPageResponseTimeout = 5000;

		private IFilterExpression m_bFlagFilters = new FilterExpressionList();

		public bool CheckSubsites
		{
			get
			{
				return this.m_bCheckSubsites;
			}
			set
			{
				this.m_bCheckSubsites = value;
			}
		}

		public bool CheckTextFields
		{
			get
			{
				return this.m_bCheckTextFields;
			}
			set
			{
				this.m_bCheckTextFields = value;
			}
		}

		public bool CheckWebparts
		{
			get
			{
				return this.m_bCheckWebparts;
			}
			set
			{
				this.m_bCheckWebparts = value;
			}
		}

		public IFilterExpression FlagFilterList
		{
			get
			{
				return this.m_bFlagFilters;
			}
			set
			{
				this.m_bFlagFilters = value;
			}
		}

		public int PageResponseTimeout
		{
			get
			{
				return this.m_iPageResponseTimeout;
			}
			set
			{
				this.m_iPageResponseTimeout = value;
			}
		}

		public bool ShowSuccesses
		{
			get
			{
				return this.m_bShowSuccesses;
			}
			set
			{
				this.m_bShowSuccesses = value;
			}
		}

		public CheckLinksOptions()
		{
		}
	}
}