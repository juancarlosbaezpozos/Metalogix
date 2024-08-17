using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLAllSharePointSiteContent")]
	public class CopySiteContentCmdlet : CopyListCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteSiteContentAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should attempt to apply the theme used on the source to the target.")]
		public SwitchParameter ApplyThemeToWeb
		{
			get
			{
				return this.PasteSiteOptions.ApplyThemeToWeb;
			}
			set
			{
				this.PasteSiteOptions.ApplyThemeToWeb = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include the request for access settings.")]
		public SwitchParameter CopyAccessRequestSettings
		{
			get
			{
				return this.PasteSiteOptions.CopyAccessRequestSettings;
			}
			set
			{
				this.PasteSiteOptions.CopyAccessRequestSettings = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should copy either all available themes from source web or just the current theme.")]
		public SwitchParameter CopyAllThemes
		{
			get
			{
				return this.PasteSiteOptions.CopyAllThemes;
			}
			set
			{
				this.PasteSiteOptions.CopyAllThemes = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should attempt to preserve the owner, member and visitor group settings by the source site. Groups specified by these settings will not automatically get permission to this site. To give these groups permissions, use the CopySitePermissions option.")]
		public SwitchParameter CopyAssociatedGroups
		{
			get
			{
				return this.PasteSiteOptions.CopyAssociatedGroups;
			}
			set
			{
				this.PasteSiteOptions.CopyAssociatedGroups = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include custom content types.")]
		public SwitchParameter CopyContentTypes
		{
			get
			{
				return this.PasteSiteOptions.CopyContentTypes;
			}
			set
			{
				this.PasteSiteOptions.CopyContentTypes = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="If a navigation structure copy is being run, this indicates whether or not the nodes in the quick launch menu should be copied.")]
		public SwitchParameter CopyCurrentNavigation
		{
			get
			{
				return this.PasteSiteOptions.CopyCurrentNavigation;
			}
			set
			{
				this.PasteSiteOptions.CopyCurrentNavigation = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we should copy the custom folders and files.")]
		public SwitchParameter CopyCustomContent
		{
			get
			{
				return this.PasteSiteOptions.CopyCustomContent;
			}
			set
			{
				this.PasteSiteOptions.CopyCustomContent = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="If a navigation structure copy is being run, this indicates whether or not the nodes in the top navigation bar should be copied.")]
		public SwitchParameter CopyGlobalNavigation
		{
			get
			{
				return this.PasteSiteOptions.CopyGlobalNavigation;
			}
			set
			{
				this.PasteSiteOptions.CopyGlobalNavigation = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include lists and libraries.")]
		public SwitchParameter CopyLists
		{
			get
			{
				return this.PasteSiteOptions.CopyLists;
			}
			set
			{
				this.PasteSiteOptions.CopyLists = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that navigation should be copied.")]
		public SwitchParameter CopyNavigation
		{
			get
			{
				return this.PasteSiteOptions.CopyNavigation;
			}
			set
			{
				this.PasteSiteOptions.CopyNavigation = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that the copy operation should include customized permission levels. Note that this feature is unavailable when migrating from SharePoint 2003.")]
		public SwitchParameter CopyPermissionLevels
		{
			get
			{
				return this.PasteSiteOptions.CopyPermissionLevels;
			}
			set
			{
				this.PasteSiteOptions.CopyPermissionLevels = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we should copy the portal listings defined on the site.")]
		public SwitchParameter CopyPortalListings
		{
			get
			{
				return this.PasteSiteOptions.CopyPortalListings;
			}
			set
			{
				this.PasteSiteOptions.CopyPortalListings = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we should copy the features defined on the site.")]
		public SwitchParameter CopySiteFeatures
		{
			get
			{
				return this.PasteSiteOptions.CopySiteFeatures;
			}
			set
			{
				this.PasteSiteOptions.CopySiteFeatures = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include unique permissions for sites.")]
		public SwitchParameter CopySitePermissions
		{
			get
			{
				return this.PasteSiteOptions.CopySitePermissions;
			}
			set
			{
				this.PasteSiteOptions.CopySitePermissions = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if web parts on site landing pages should be copied.")]
		public SwitchParameter CopySiteWebParts
		{
			get
			{
				return this.PasteSiteOptions.CopySiteWebParts;
			}
			set
			{
				this.PasteSiteOptions.CopySiteWebParts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we should copy files that are not customized outside of lists and libraries.")]
		public SwitchParameter CopyUncustomizedFiles
		{
			get
			{
				return this.PasteSiteOptions.CopyUncustomizedFiles;
			}
			set
			{
				this.PasteSiteOptions.CopyUncustomizedFiles = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a custom file should be copied.")]
		public IFilterExpression CustomFileFilterExpression
		{
			get
			{
				if (!this.PasteSiteOptions.FilterCustomFiles)
				{
					return null;
				}
				return this.PasteSiteOptions.CustomFileFilterExpression;
			}
			set
			{
				this.PasteSiteOptions.FilterCustomFiles = true;
				this.PasteSiteOptions.CustomFileFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a custom folder should be copied.")]
		public IFilterExpression CustomFolderFilterExpression
		{
			get
			{
				if (!this.PasteSiteOptions.FilterCustomFolders)
				{
					return null;
				}
				return this.PasteSiteOptions.CustomFolderFilterExpression;
			}
			set
			{
				this.PasteSiteOptions.FilterCustomFolders = true;
				this.PasteSiteOptions.CustomFolderFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if we should merge source site features into the target's site features set.\nIf this switch is not set, target features which are not present in the source will be removed.")]
		public SwitchParameter MergeSiteFeatures
		{
			get
			{
				return this.PasteSiteOptions.MergeSiteFeatures;
			}
			set
			{
				this.PasteSiteOptions.MergeSiteFeatures = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should overwrite existing sites. Overwriting sites takes precedence over the 'UpdateSites' parameter.")]
		public SwitchParameter OverwriteSites
		{
			get
			{
				return this.PasteSiteOptions.OverwriteSites;
			}
			set
			{
				this.PasteSiteOptions.OverwriteSites = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteSiteOptions PasteSiteOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteSiteOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should attempt to preserve the master page used by the source site.")]
		public SwitchParameter PreserveMasterPage
		{
			get
			{
				return this.PasteSiteOptions.PreserveMasterPage;
			}
			set
			{
				this.PasteSiteOptions.PreserveMasterPage = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation can preserve 2007 look and feel on 2010 target.")]
		public SwitchParameter PreserveUIVersion
		{
			get
			{
				return this.PasteSiteOptions.PreserveUIVersion;
			}
			set
			{
				this.PasteSiteOptions.PreserveUIVersion = value;
			}
		}

		[Parameter(HelpMessage="Indicates if the copy operation should recursively copy subsites.")]
		public SwitchParameter RecursivelyCopySubsites
		{
			get
			{
				return this.PasteSiteOptions.RecursivelyCopySubsites;
			}
			set
			{
				this.PasteSiteOptions.RecursivelyCopySubsites = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should run a navigation structure copy after all sites have been copied.")]
		public SwitchParameter RunNavigationStructureCopy
		{
			get
			{
				return this.PasteSiteOptions.RunNavigationStructureCopy;
			}
			set
			{
				this.PasteSiteOptions.RunNavigationStructureCopy = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if site columns to be filtered.")]
		public IFilterExpression SiteFieldsFilterExpression
		{
			get
			{
				if (!this.PasteSiteOptions.FilterSiteFields)
				{
					return null;
				}
				return this.PasteSiteOptions.SiteFieldsFilterExpression;
			}
			set
			{
				this.PasteSiteOptions.FilterSiteFields = true;
				this.PasteSiteOptions.SiteFieldsFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a filter expression used to determine if a subsite should be copied.")]
		public IFilterExpression SiteFilterExpression
		{
			get
			{
				if (!this.PasteSiteOptions.FilterSites)
				{
					return null;
				}
				return this.PasteSiteOptions.SiteFilterExpression;
			}
			set
			{
				this.PasteSiteOptions.FilterSites = true;
				this.PasteSiteOptions.SiteFilterExpression = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Specifies whether to update sites or not.")]
		public string[] UpdateSites
		{
			get
			{
				return this.PasteSiteOptions.UpdateSiteOptionsForPowershell;
			}
			set
			{
				this.PasteSiteOptions.UpdateSites = true;
				this.PasteSiteOptions.UpdateSiteOptionsBitField = SystemUtils.GetFlagsEnumeratorFromStrings(typeof(UpdateSiteFlags), value);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines a table of web template mappings to use to assign new web templates when copying subsites.")]
		public object WebTemplateMappingTable
		{
			get
			{
				return this.PasteSiteOptions.WebTemplateMappingTable;
			}
			set
			{
				this.PasteSiteOptions.WebTemplateMappingTable = this.GetWebTemplateMappingTable(value);
				if (this.PasteSiteOptions.WebTemplateMappingTable != null)
				{
					this.PasteSiteOptions.MapChildWebTemplates = true;
				}
			}
		}

		public CopySiteContentCmdlet()
		{
		}

		private CommonSerializableTable<string, string> CastAsWebTemplateMappingTable(object value)
		{
			return (CommonSerializableTable<string, string>)value;
		}

		private CommonSerializableTable<string, string> GetWebTemplateMappingTable(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			PSObject pSObject = value as PSObject;
			if (pSObject != null)
			{
				return this.GetWebTemplateMappingTableFromPsObject(pSObject);
			}
			return this.CastAsWebTemplateMappingTable(value);
		}

		private CommonSerializableTable<string, string> GetWebTemplateMappingTableFromPsObject(PSObject powershellObject)
		{
			return this.CastAsWebTemplateMappingTable(powershellObject.ImmediateBaseObject);
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}