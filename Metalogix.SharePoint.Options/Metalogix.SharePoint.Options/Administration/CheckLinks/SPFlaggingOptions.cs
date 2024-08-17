using Metalogix.Actions;
using Metalogix.Data.Filters;
using System;

namespace Metalogix.SharePoint.Options.Administration.CheckLinks
{
	public class SPFlaggingOptions : ActionOptions
	{
		private IFilterExpression m_bFlagFilters = new FilterExpressionList();

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

		public SPFlaggingOptions()
		{
		}
	}
}