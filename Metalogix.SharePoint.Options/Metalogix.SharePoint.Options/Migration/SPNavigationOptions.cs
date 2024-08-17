using Metalogix;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPNavigationOptions : OptionsBase
	{
		private bool m_bCopyGlobalNavigation = true;

		private bool m_bCopyCurrentNavigation = true;

		private bool m_bRecursive;

		private bool m_bUsingComprehensiveLinkCorrection;

		public bool CopyCurrentNavigation
		{
			get
			{
				return this.m_bCopyCurrentNavigation;
			}
			set
			{
				this.m_bCopyCurrentNavigation = value;
			}
		}

		public bool CopyGlobalNavigation
		{
			get
			{
				return this.m_bCopyGlobalNavigation;
			}
			set
			{
				this.m_bCopyGlobalNavigation = value;
			}
		}

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

		public bool UsingComprehensiveLinkCorrection
		{
			get
			{
				return this.m_bUsingComprehensiveLinkCorrection;
			}
			set
			{
				this.m_bUsingComprehensiveLinkCorrection = value;
			}
		}

		public SPNavigationOptions()
		{
		}
	}
}