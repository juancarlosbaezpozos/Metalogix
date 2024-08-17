using Metalogix.Actions;
using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class SPNavigationRibbonOptions : ActionOptions
	{
		private Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion = new Metalogix.SharePoint.Adapters.SharePointVersion();

		private ChangeNavigationSettingsOptions.RibbonOptions m_bRibbonOptions;

		private ChangeNavigationSettingsOptions.RibbonModifiedFlags m_RibbonOptionsModified = new ChangeNavigationSettingsOptions.RibbonModifiedFlags();

		public ChangeNavigationSettingsOptions.RibbonModifiedFlags RibbonOptionsModified
		{
			get
			{
				return this.m_RibbonOptionsModified;
			}
			set
			{
				this.m_RibbonOptionsModified = value;
			}
		}

		public Metalogix.SharePoint.Adapters.SharePointVersion SharePointVersion
		{
			get
			{
				return this.m_SharePointVersion;
			}
			set
			{
				this.m_SharePointVersion = value;
			}
		}

		public ChangeNavigationSettingsOptions.RibbonOptions ShowRibbon
		{
			get
			{
				return this.m_bRibbonOptions;
			}
			set
			{
				this.m_bRibbonOptions = value;
			}
		}

		public SPNavigationRibbonOptions()
		{
		}
	}
}