using Metalogix.Actions;
using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class SPQuickLaunchOptions : ActionOptions
	{
		private Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion = new Metalogix.SharePoint.Adapters.SharePointVersion();

		private ChangeNavigationSettingsOptions.SharePointAdapterType m_AdapterType;

		private bool m_bQuickLaunchEnabled = true;

		private bool m_bTreeViewEnabled;

		private ChangeNavigationSettingsOptions.CurrentNavigationDisplayType m_CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite;

		private ChangeNavigationSettingsOptions.NavigationZone m_CurrentNavigationZone = new ChangeNavigationSettingsOptions.NavigationZone();

		private ChangeNavigationSettingsOptions.QuickLaunchModifiedFlags m_QuickLaunchOptionsModified = new ChangeNavigationSettingsOptions.QuickLaunchModifiedFlags();

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

		public ChangeNavigationSettingsOptions.CurrentNavigationDisplayType CurrentNavigationType
		{
			get
			{
				return this.m_CurrentNavigationType;
			}
			set
			{
				this.m_CurrentNavigationType = value;
			}
		}

		public ChangeNavigationSettingsOptions.NavigationZone CurrentNavigationZone
		{
			get
			{
				return this.m_CurrentNavigationZone;
			}
			set
			{
				this.m_CurrentNavigationZone = value;
			}
		}

		public bool QuickLaunchEnabled
		{
			get
			{
				return this.m_bQuickLaunchEnabled;
			}
			set
			{
				this.m_bQuickLaunchEnabled = value;
			}
		}

		public ChangeNavigationSettingsOptions.QuickLaunchModifiedFlags QuickLaunchOptionsModified
		{
			get
			{
				return this.m_QuickLaunchOptionsModified;
			}
			set
			{
				this.m_QuickLaunchOptionsModified = value;
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

		public bool TreeViewEnabled
		{
			get
			{
				return this.m_bTreeViewEnabled;
			}
			set
			{
				this.m_bTreeViewEnabled = value;
			}
		}

		public SPQuickLaunchOptions()
		{
		}
	}
}