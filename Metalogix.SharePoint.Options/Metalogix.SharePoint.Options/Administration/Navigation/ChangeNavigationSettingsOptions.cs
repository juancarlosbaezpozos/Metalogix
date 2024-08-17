using Metalogix;
using Metalogix.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class ChangeNavigationSettingsOptions : ActionOptions
	{
		private const int INCLUDE_SUBSITES = 1;

		private const int INCLUDE_PAGES = 2;

		private const string SHAREPOINT_VERSION = "SharePointVersion";

		private const string WEB_NAME_ATTRIBUTE = "Name";

		private const string QUICK_LAUNCH_ENABLED = "QuickLaunchEnabled";

		private const string TREE_VIEW_ENABLED = "TreeViewEnabled";

		private const string NAVIGATION_SORT_ASCENDING = "NavigationSortAscending";

		private const string INHERIT_GLOBAL_NAVIGATION = "InheritGlobalNavigation";

		private const string INHERIT_CURRENT_NAVIGATION = "InheritCurrentNavigation";

		private const string NAVIGATION_SHOW_SIBLINGS = "NavigationShowSiblings";

		private const string NAVIGATION_ORDERING_METHOD = "NavigationOrderingMethod";

		private const string NAVIGATION_AUTOMATIC_SORTING_METHOD = "NavigationAutomaticSortingMethod";

		private const string SHOW_RIBBON_OPTION = "DisplayShowHideRibbonActionId";

		private const string GLOBAL_DYNAMIC_CHILD_LIMIT = "GlobalDynamicChildLimit";

		private const string CURRENT_DYNAMIC_CHILD_LIMIT = "CurrentDynamicChildLimit";

		private const string GLOBAL_NAVIGATION_SHOW_PAGES = "IncludePagesInGlobalNavigation";

		private const string GLOBAL_NAVIGATION_SHOW_SUBSITES = "IncludeSubSitesInGlobalNavigation";

		private const string CURRENT_NAVIGATION_SHOW_PAGES = "IncludePagesInCurrentNavigation";

		private const string CURRENT_NAVIGATION_SHOW_SUBSITES = "IncludeSubSitesInCurrentNavigation";

		private Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion = new Metalogix.SharePoint.Adapters.SharePointVersion();

		private ChangeNavigationSettingsOptions.SharePointAdapterType m_AdapterType;

		private bool m_bQuickLaunchEnabled;

		private bool m_bTreeViewEnabled;

		private bool m_bApplyChangesToParentSites;

		private bool m_bApplyChangesToSubSites;

		private ChangeNavigationSettingsOptions.RibbonOptions m_bRibbonOptions;

		private bool m_SortAscending = true;

		private ChangeNavigationSettingsOptions.SortingType m_OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;

		private ChangeNavigationSettingsOptions.SortingType m_PublishingPageSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;

		private ChangeNavigationSettingsOptions.SortByColumn m_SortingColumn = ChangeNavigationSettingsOptions.SortByColumn.Title;

		private ChangeNavigationSettingsOptions.CurrentNavigationDisplayType m_CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite;

		private ChangeNavigationSettingsOptions.GlobalNavigationDisplayType m_GlobalNavigationType = ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite;

		private ChangeNavigationSettingsOptions.NavigationZone m_CurrentNavigationZone = new ChangeNavigationSettingsOptions.NavigationZone();

		private ChangeNavigationSettingsOptions.NavigationZone m_GlobalNavigationZone = new ChangeNavigationSettingsOptions.NavigationZone();

		private ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags m_GlobalNavigationDirty = new ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags();

		private ChangeNavigationSettingsOptions.QuickLaunchModifiedFlags m_QuickLaunchDirty = new ChangeNavigationSettingsOptions.QuickLaunchModifiedFlags();

		private ChangeNavigationSettingsOptions.SortingModifiedFlags m_SortingDirty = new ChangeNavigationSettingsOptions.SortingModifiedFlags();

		private ChangeNavigationSettingsOptions.RibbonModifiedFlags m_RibbonDirty = new ChangeNavigationSettingsOptions.RibbonModifiedFlags();

		private string sTrue = "True";

		private string sFalse = "False";

		public string AdapterShortName
		{
			set
			{
				this.m_AdapterType = this.GetAdapterTypeFromShortName(value);
			}
		}

		public ChangeNavigationSettingsOptions.SharePointAdapterType AdapterType
		{
			get
			{
				return this.m_AdapterType;
			}
		}

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

		public ChangeNavigationSettingsOptions.SortByColumn ColumnToSortBy
		{
			get
			{
				return this.m_SortingColumn;
			}
			set
			{
				this.m_SortingColumn = value;
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

		public ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags GlobalNavigationOptionsModified
		{
			get
			{
				return this.m_GlobalNavigationDirty;
			}
			set
			{
				this.m_GlobalNavigationDirty = value;
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

		public ChangeNavigationSettingsOptions.SortingType OverallSortingType
		{
			get
			{
				return this.m_OverallSortingType;
			}
			set
			{
				this.m_OverallSortingType = value;
			}
		}

		public ChangeNavigationSettingsOptions.SortingType PublishingPageSortingType
		{
			get
			{
				return this.m_PublishingPageSortingType;
			}
			set
			{
				this.m_PublishingPageSortingType = value;
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
				return this.m_QuickLaunchDirty;
			}
			set
			{
				this.m_QuickLaunchDirty = value;
			}
		}

		public ChangeNavigationSettingsOptions.RibbonModifiedFlags RibbonOptionsModified
		{
			get
			{
				return this.m_RibbonDirty;
			}
			set
			{
				this.m_RibbonDirty = value;
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

		public bool SortAscending
		{
			get
			{
				return this.m_SortAscending;
			}
			set
			{
				this.m_SortAscending = value;
			}
		}

		public ChangeNavigationSettingsOptions.SortingModifiedFlags SortingOptionsModified
		{
			get
			{
				return this.m_SortingDirty;
			}
			set
			{
				this.m_SortingDirty = value;
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

		public ChangeNavigationSettingsOptions()
		{
		}

		public ChangeNavigationSettingsOptions(string sNavigationSettingsXml, Metalogix.SharePoint.Adapters.SharePointVersion version, string sAdapterShortName)
		{
			base.FromXML(sNavigationSettingsXml);
			this.SharePointVersion = version;
			this.AdapterShortName = sAdapterShortName;
		}

		public override void FromXML(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				return;
			}
			if (xmlNode.Attributes["SharePointVersion"] != null)
			{
				this.SharePointVersion = new Metalogix.SharePoint.Adapters.SharePointVersion(xmlNode.Attributes["SharePointVersion"].Value);
			}
			bool flag = false;
			int num = -1;
			this.QuickLaunchEnabled = (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "QuickLaunchEnabled", out flag) ? flag : this.QuickLaunchEnabled);
			this.TreeViewEnabled = (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "TreeViewEnabled", out flag) ? flag : this.TreeViewEnabled);
			this.SortAscending = (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "NavigationSortAscending", out flag) ? flag : this.SortAscending);
			if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "InheritGlobalNavigation", out flag))
			{
				this.GlobalNavigationType = (flag ? ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite : ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.OnlyBelowCurrentSite);
			}
			if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "InheritCurrentNavigation", out flag))
			{
				if (flag)
				{
					this.CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite;
				}
				else if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "NavigationShowSiblings", out flag))
				{
					this.CurrentNavigationType = (flag ? ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.CurrentSiteAndBelowPlusSiblings : ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.OnlyBelowCurrentSite);
				}
			}
			if (XmlUtility.GetIntegerAttributeFromXml(xmlNode, "NavigationOrderingMethod", out num))
			{
				if (num != 0)
				{
					this.OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Manual;
					if (num == 1)
					{
						this.PublishingPageSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;
					}
					else if (num == 2)
					{
						this.PublishingPageSortingType = ChangeNavigationSettingsOptions.SortingType.Manual;
					}
				}
				else
				{
					this.OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;
					this.PublishingPageSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;
				}
			}
			if (XmlUtility.GetIntegerAttributeFromXml(xmlNode, "NavigationAutomaticSortingMethod", out num))
			{
				if (num == 0)
				{
					this.ColumnToSortBy = ChangeNavigationSettingsOptions.SortByColumn.Title;
				}
				else if (num == 1)
				{
					this.ColumnToSortBy = ChangeNavigationSettingsOptions.SortByColumn.CreatedDate;
				}
				else if (num == 2)
				{
					this.ColumnToSortBy = ChangeNavigationSettingsOptions.SortByColumn.LastModifiedDate;
				}
			}
			this.GlobalNavigationZone.MaxDynamicItems = (XmlUtility.GetIntegerAttributeFromXml(xmlNode, "GlobalDynamicChildLimit", out num) ? num : this.GlobalNavigationZone.MaxDynamicItems);
			this.CurrentNavigationZone.MaxDynamicItems = (XmlUtility.GetIntegerAttributeFromXml(xmlNode, "CurrentDynamicChildLimit", out num) ? num : this.CurrentNavigationZone.MaxDynamicItems);
			this.ShowRibbon = ChangeNavigationSettingsOptions.RibbonOptions.LeaveUnchanged;
			if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "IncludePagesInCurrentNavigation", out flag))
			{
				this.CurrentNavigationZone.ShowPublishingPages = flag;
			}
			if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "IncludeSubSitesInCurrentNavigation", out flag))
			{
				this.CurrentNavigationZone.ShowSubSites = flag;
			}
			if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "IncludePagesInGlobalNavigation", out flag))
			{
				this.GlobalNavigationZone.ShowPublishingPages = flag;
			}
			if (XmlUtility.GetBooleanAttributeFromXml(xmlNode, "IncludeSubSitesInGlobalNavigation", out flag))
			{
				this.GlobalNavigationZone.ShowSubSites = flag;
			}
		}

		public ChangeNavigationSettingsOptions.SharePointAdapterType GetAdapterTypeFromShortName(string sAdapterShortName)
		{
			ChangeNavigationSettingsOptions.SharePointAdapterType sharePointAdapterType = ChangeNavigationSettingsOptions.SharePointAdapterType.Unspecified;
			if (sAdapterShortName == "OM")
			{
				sharePointAdapterType = ChangeNavigationSettingsOptions.SharePointAdapterType.OM;
			}
			else if (sAdapterShortName == "WS")
			{
				sharePointAdapterType = ChangeNavigationSettingsOptions.SharePointAdapterType.ExtensionsService;
			}
			else if (sAdapterShortName != "NW")
			{
				sharePointAdapterType = (sAdapterShortName != "DB" ? ChangeNavigationSettingsOptions.SharePointAdapterType.Unspecified : ChangeNavigationSettingsOptions.SharePointAdapterType.DB);
			}
			else
			{
				sharePointAdapterType = ChangeNavigationSettingsOptions.SharePointAdapterType.NWS;
			}
			return sharePointAdapterType;
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			this.ToXML(xmlWriter, ChangeNavigationSettingsOptions.NavigationXmlOutput.AllNavigationSettings);
		}

		public void ToXML(XmlWriter xmlWriter, ChangeNavigationSettingsOptions.NavigationXmlOutput settingsOutputFilter)
		{
			if (xmlWriter == null)
			{
				return;
			}
			xmlWriter.WriteStartElement("Web");
			xmlWriter.WriteAttributeString("SharePointVersion", this.SharePointVersion.VersionNumberString);
			if (this.SharePointVersion.IsSharePoint2010OrLater && this.ShowRibbon != ChangeNavigationSettingsOptions.RibbonOptions.LeaveUnchanged && this.RibbonOptionsModified.ShowRibbon)
			{
				xmlWriter.WriteAttributeString("DisplayShowHideRibbonActionId", (this.ShowRibbon == ChangeNavigationSettingsOptions.RibbonOptions.ShowRibbon ? this.sTrue : this.sFalse));
			}
			if (settingsOutputFilter == ChangeNavigationSettingsOptions.NavigationXmlOutput.QuickLaunchSettingsOnly || settingsOutputFilter == ChangeNavigationSettingsOptions.NavigationXmlOutput.AllNavigationSettings)
			{
				if (this.QuickLaunchOptionsModified.CurrentNavigationType && this.CurrentNavigationType != ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.Unspecified)
				{
					bool currentNavigationType = this.CurrentNavigationType == ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite;
					xmlWriter.WriteAttributeString("InheritCurrentNavigation", (currentNavigationType ? this.sTrue : this.sFalse));
					bool flag = this.CurrentNavigationType == ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.CurrentSiteAndBelowPlusSiblings;
					xmlWriter.WriteAttributeString("NavigationShowSiblings", (flag ? this.sTrue : this.sFalse));
				}
				if (this.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.OM || this.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.ExtensionsService)
				{
					if (this.QuickLaunchOptionsModified.QuickLaunchEnabled)
					{
						xmlWriter.WriteAttributeString("QuickLaunchEnabled", (this.QuickLaunchEnabled ? this.sTrue : this.sFalse));
					}
					if (this.QuickLaunchOptionsModified.TreeViewEnabled)
					{
						xmlWriter.WriteAttributeString("TreeViewEnabled", (this.TreeViewEnabled ? this.sTrue : this.sFalse));
					}
				}
				if (this.SharePointVersion.IsSharePoint2010OrLater && this.QuickLaunchOptionsModified.DynamicItems)
				{
					xmlWriter.WriteAttributeString("CurrentDynamicChildLimit", this.CurrentNavigationZone.MaxDynamicItems.ToString());
				}
				if (this.QuickLaunchOptionsModified.ShowSubsites)
				{
					xmlWriter.WriteAttributeString("IncludeSubSitesInCurrentNavigation", (this.CurrentNavigationZone.ShowSubSites ? this.sTrue : this.sFalse));
				}
				if (this.QuickLaunchOptionsModified.ShowPublishingPages)
				{
					xmlWriter.WriteAttributeString("IncludePagesInCurrentNavigation", (this.CurrentNavigationZone.ShowPublishingPages ? this.sTrue : this.sFalse));
				}
			}
			if (settingsOutputFilter == ChangeNavigationSettingsOptions.NavigationXmlOutput.GlobalNavigationSettingsOnly || settingsOutputFilter == ChangeNavigationSettingsOptions.NavigationXmlOutput.AllNavigationSettings)
			{
				if ((this.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.OM || this.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.ExtensionsService) && this.GlobalNavigationOptionsModified.GlobalNavigationType && this.GlobalNavigationType != ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.Unspecified)
				{
					xmlWriter.WriteAttributeString("InheritGlobalNavigation", (this.GlobalNavigationType == ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite ? this.sTrue : this.sFalse));
				}
				if (this.SharePointVersion.IsSharePoint2010OrLater && this.GlobalNavigationOptionsModified.DynamicItems)
				{
					xmlWriter.WriteAttributeString("GlobalDynamicChildLimit", this.GlobalNavigationZone.MaxDynamicItems.ToString());
				}
				if (this.GlobalNavigationOptionsModified.ShowSubsites)
				{
					xmlWriter.WriteAttributeString("IncludeSubSitesInGlobalNavigation", (this.GlobalNavigationZone.ShowSubSites ? this.sTrue : this.sFalse));
				}
				if (this.GlobalNavigationOptionsModified.ShowPublishingPages)
				{
					xmlWriter.WriteAttributeString("IncludePagesInGlobalNavigation", (this.GlobalNavigationZone.ShowPublishingPages ? this.sTrue : this.sFalse));
				}
			}
			if (this.SortingOptionsModified.OverallSorting && this.OverallSortingType != ChangeNavigationSettingsOptions.SortingType.Unspecified)
			{
				int num = 0;
				if (this.OverallSortingType != ChangeNavigationSettingsOptions.SortingType.Automatic)
				{
					num = (this.PublishingPageSortingType != ChangeNavigationSettingsOptions.SortingType.Automatic ? 2 : 1);
				}
				xmlWriter.WriteAttributeString("NavigationOrderingMethod", num.ToString());
			}
			if ((this.OverallSortingType == ChangeNavigationSettingsOptions.SortingType.Automatic ? true : this.PublishingPageSortingType == ChangeNavigationSettingsOptions.SortingType.Automatic))
			{
				if (this.SortingOptionsModified.SortDirection)
				{
					xmlWriter.WriteAttributeString("NavigationSortAscending", (this.SortAscending ? this.sTrue : this.sFalse));
				}
				if (this.SortingOptionsModified.SortingColumn && this.ColumnToSortBy != ChangeNavigationSettingsOptions.SortByColumn.Unspecified)
				{
					int num1 = 0;
					if (this.ColumnToSortBy == ChangeNavigationSettingsOptions.SortByColumn.CreatedDate)
					{
						num1 = 1;
					}
					else if (this.ColumnToSortBy == ChangeNavigationSettingsOptions.SortByColumn.LastModifiedDate)
					{
						num1 = 2;
					}
					xmlWriter.WriteAttributeString("NavigationAutomaticSortingMethod", num1.ToString());
				}
			}
			xmlWriter.WriteEndElement();
		}

		public enum CurrentNavigationDisplayType
		{
			Unspecified,
			SameAsParentSite,
			CurrentSiteAndBelowPlusSiblings,
			OnlyBelowCurrentSite
		}

		public enum GlobalNavigationDisplayType
		{
			Unspecified,
			SameAsParentSite,
			OnlyBelowCurrentSite
		}

		public class GlobalNavigationModifiedFlags
		{
			private bool m_bGlobalNavigationType;

			private bool m_bShowSubsites;

			private bool m_bShowPublishingPages;

			private bool m_bDynamicItems;

			public bool DynamicItems
			{
				get
				{
					return this.m_bDynamicItems;
				}
				set
				{
					this.m_bDynamicItems = value;
				}
			}

			public bool GlobalNavigationType
			{
				get
				{
					return this.m_bGlobalNavigationType;
				}
				set
				{
					this.m_bGlobalNavigationType = value;
				}
			}

			public bool ShowPublishingPages
			{
				get
				{
					return this.m_bShowPublishingPages;
				}
				set
				{
					this.m_bShowPublishingPages = value;
				}
			}

			public bool ShowSubsites
			{
				get
				{
					return this.m_bShowSubsites;
				}
				set
				{
					this.m_bShowSubsites = value;
				}
			}

			public GlobalNavigationModifiedFlags()
			{
			}
		}

		public enum NavigationXmlOutput
		{
			AllNavigationSettings,
			QuickLaunchSettingsOnly,
			GlobalNavigationSettingsOnly
		}

		public class NavigationZone : ActionOptions
		{
			private bool m_bShowSubSites;

			private bool m_bShowPublishingPages;

			private int m_iMaxDynamicItems;

			public int MaxDynamicItems
			{
				get
				{
					return this.m_iMaxDynamicItems;
				}
				set
				{
					this.m_iMaxDynamicItems = value;
				}
			}

			public bool ShowPublishingPages
			{
				get
				{
					return this.m_bShowPublishingPages;
				}
				set
				{
					this.m_bShowPublishingPages = value;
				}
			}

			public bool ShowSubSites
			{
				get
				{
					return this.m_bShowSubSites;
				}
				set
				{
					this.m_bShowSubSites = value;
				}
			}

			public NavigationZone()
			{
			}
		}

		public class QuickLaunchModifiedFlags
		{
			private bool m_bQuickLaunchEnabled;

			private bool m_bTreeViewEnabled;

			private bool m_bCurrentNavigationType;

			private bool m_bShowSubsites;

			private bool m_bShowPublishingPages;

			private bool m_bDynamicItems;

			public bool CurrentNavigationType
			{
				get
				{
					return this.m_bCurrentNavigationType;
				}
				set
				{
					this.m_bCurrentNavigationType = value;
				}
			}

			public bool DynamicItems
			{
				get
				{
					return this.m_bDynamicItems;
				}
				set
				{
					this.m_bDynamicItems = value;
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

			public bool ShowPublishingPages
			{
				get
				{
					return this.m_bShowPublishingPages;
				}
				set
				{
					this.m_bShowPublishingPages = value;
				}
			}

			public bool ShowSubsites
			{
				get
				{
					return this.m_bShowSubsites;
				}
				set
				{
					this.m_bShowSubsites = value;
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

			public QuickLaunchModifiedFlags()
			{
			}
		}

		public class RibbonModifiedFlags
		{
			private bool m_bShowRibbon;

			public bool ShowRibbon
			{
				get
				{
					return this.m_bShowRibbon;
				}
				set
				{
					this.m_bShowRibbon = value;
				}
			}

			public RibbonModifiedFlags()
			{
			}
		}

		public enum RibbonOptions
		{
			LeaveUnchanged,
			ShowRibbon,
			HideRibbon
		}

		public enum SharePointAdapterType
		{
			Unspecified,
			DB,
			OM,
			NWS,
			ExtensionsService
		}

		public enum SortByColumn
		{
			Unspecified,
			Title,
			CreatedDate,
			LastModifiedDate
		}

		public class SortingModifiedFlags
		{
			private bool m_bSortDirection;

			private bool m_bOverallSorting;

			private bool m_bPublishingPageSorting;

			private bool m_bSortingColumn;

			public bool OverallSorting
			{
				get
				{
					return this.m_bOverallSorting;
				}
				set
				{
					this.m_bOverallSorting = value;
				}
			}

			public bool PublishingPageSorting
			{
				get
				{
					return this.m_bPublishingPageSorting;
				}
				set
				{
					this.m_bPublishingPageSorting = value;
				}
			}

			public bool SortDirection
			{
				get
				{
					return this.m_bSortDirection;
				}
				set
				{
					this.m_bSortDirection = value;
				}
			}

			public bool SortingColumn
			{
				get
				{
					return this.m_bSortingColumn;
				}
				set
				{
					this.m_bSortingColumn = value;
				}
			}

			public SortingModifiedFlags()
			{
			}
		}

		public enum SortingType
		{
			Unspecified,
			Automatic,
			Manual
		}
	}
}