using Metalogix;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPListViewsOptions : OptionsBase
	{
		private bool m_bOverwriteExistingViews = true;

		public bool OverwriteExistingViews
		{
			get
			{
				return this.m_bOverwriteExistingViews;
			}
			set
			{
				this.m_bOverwriteExistingViews = value;
			}
		}

		public SPListViewsOptions()
		{
		}
	}
}