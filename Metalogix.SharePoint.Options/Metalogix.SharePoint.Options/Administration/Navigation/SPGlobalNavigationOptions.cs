using Metalogix.Actions;
using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class SPGlobalNavigationOptions : ActionOptions
	{
		private Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion = new Metalogix.SharePoint.Adapters.SharePointVersion();

		private ChangeNavigationSettingsOptions.SharePointAdapterType m_AdapterType;

		private ChangeNavigationSettingsOptions.GlobalNavigationDisplayType m_GlobalNavigationType = ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite;

		private ChangeNavigationSettingsOptions.NavigationZone m_GlobalNavigationZone = new ChangeNavigationSettingsOptions.NavigationZone();

		private ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags m_GlobalNavigationOptionsModified = new ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags();

		public ChangeNavigationSettingsOptions.SharePointAdapterType AdapterType
		{
			get
			{
				return this.m_AdapterType;
			}
			set
			{
				this.m_AdapterType = value;
			}
		}

		public ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags GlobalNavigationOptionsModified
		{
			get
			{
				return this.m_GlobalNavigationOptionsModified;
			}
			set
			{
				this.m_GlobalNavigationOptionsModified = value;
			}
		}

		public ChangeNavigationSettingsOptions.GlobalNavigationDisplayType GlobalNavigationType
		{
			get
			{
				return this.m_GlobalNavigationType;
			}
			set
			{
				this.m_GlobalNavigationType = value;
			}
		}

		public ChangeNavigationSettingsOptions.NavigationZone GlobalNavigationZone
		{
			get
			{
				return this.m_GlobalNavigationZone;
			}
			set
			{
				this.m_GlobalNavigationZone = value;
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

		public SPGlobalNavigationOptions()
		{
		}
	}
}