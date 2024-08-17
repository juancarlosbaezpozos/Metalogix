using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class SPNavigationPropagationOptions : ActionOptions
	{
		private bool m_bApplyChangesToParentSites;

		private bool m_bApplyChangesToSubSites;

		public bool ApplyChangesToParentSites
		{
			get
			{
				return this.m_bApplyChangesToParentSites;
			}
			set
			{
				this.m_bApplyChangesToParentSites = value;
			}
		}

		public bool ApplyChangesToSubSites
		{
			get
			{
				return this.m_bApplyChangesToSubSites;
			}
			set
			{
				this.m_bApplyChangesToSubSites = value;
			}
		}

		public SPNavigationPropagationOptions()
		{
		}
	}
}