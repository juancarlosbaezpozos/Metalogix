using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteSiteOptions : PasteListOptions
	{
		private bool m_bCopyPermissionLevels = true;

		private bool m_bCopySitePermissions = true;

		private bool m_bCopyAccessRequestSettings = true;

		private bool m_bCopyAssociatedGroups = true;

		private bool m_bCopySiteWebParts = true;

		private bool m_bFilterSiteFields;

		private IFilterExpression m_siteFieldFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bFilterSites;

		private IFilterExpression m_siteFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bFilterCustomFolders;

		private IFilterExpression m_customFolderFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bFilterCustomFiles;

		private IFilterExpression m_customFileFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bMapChildSiteTemplates;

		private CommonSerializableTable<string, string> m_stWebTemplateMapping = new CommonSerializableTable<string, string>();

		private bool m_bCopyLists = true;

		private bool m_bOverwriteSites = true;

		private bool m_bUpdateSites;

		private int m_iUpdateOptions;

		private bool m_bRecursivelyCopySubsites = true;

		private bool m_bCopySiteColumns = true;

		private bool m_bCopyContentTypes = true;

		private bool m_bChangeWebTemplate;

		private string m_sWebTemplateName;

		private bool m_bCopyNavigation = true;

		private bool m_bCopySiteFeatures = true;

		private bool m_bMergeSiteFeatures = true;

		private bool m_bApplyThemeToWeb;

		private bool _copyAllThemes;

		private bool m_bPreserveMasterPage;

		private bool m_bPreserveUIVersion;

		private bool m_bCopyPortalListings;

		private bool m_bCopyCustomContent;

		public bool _copyUncustomizedFiles;

		private bool m_bRunNavigationStructureCopy = true;

		private bool m_bCopyGlobalNavigation = true;

		private bool m_bCopyCurrentNavigation = true;

		public bool ApplyThemeToWeb
		{
			get
			{
				return this.m_bApplyThemeToWeb;
			}
			set
			{
				this.m_bApplyThemeToWeb = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public bool ChangeWebTemplate
		{
			get
			{
				return this.m_bChangeWebTemplate;
			}
			set
			{
				this.m_bChangeWebTemplate = value;
			}
		}

		public bool CopyAccessRequestSettings
		{
			get
			{
				return this.m_bCopyAccessRequestSettings;
			}
			set
			{
				this.m_bCopyAccessRequestSettings = value;
			}
		}

		public bool CopyAllThemes
		{
			get
			{
				return this._copyAllThemes;
			}
			set
			{
				this._copyAllThemes = value;
			}
		}

		public bool CopyAssociatedGroups
		{
			get
			{
				return this.m_bCopyAssociatedGroups;
			}
			set
			{
				this.m_bCopyAssociatedGroups = value;
			}
		}

		public bool CopyContentTypes
		{
			get
			{
				return this.m_bCopyContentTypes;
			}
			set
			{
				this.m_bCopyContentTypes = value;
			}
		}

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

		public bool CopyCustomContent
		{
			get
			{
				return this.m_bCopyCustomContent;
			}
			set
			{
				this.m_bCopyCustomContent = value;
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

		public bool CopyLists
		{
			get
			{
				return this.m_bCopyLists;
			}
			set
			{
				this.m_bCopyLists = value;
			}
		}

		public bool CopyNavigation
		{
			get
			{
				return this.m_bCopyNavigation;
			}
			set
			{
				this.m_bCopyNavigation = value;
			}
		}

		public bool CopyPermissionLevels
		{
			get
			{
				return this.m_bCopyPermissionLevels;
			}
			set
			{
				this.m_bCopyPermissionLevels = value;
			}
		}

		public bool CopyPortalListings
		{
			get
			{
				return this.m_bCopyPortalListings;
			}
			set
			{
				this.m_bCopyPortalListings = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopySiteColumns
		{
			get
			{
				return this.m_bCopySiteColumns;
			}
			set
			{
				this.m_bCopySiteColumns = value;
			}
		}

		public bool CopySiteFeatures
		{
			get
			{
				return this.m_bCopySiteFeatures;
			}
			set
			{
				this.m_bCopySiteFeatures = value;
			}
		}

		public bool CopySitePermissions
		{
			get
			{
				return this.m_bCopySitePermissions;
			}
			set
			{
				this.m_bCopySitePermissions = value;
			}
		}

		public bool CopySiteWebParts
		{
			get
			{
				return this.m_bCopySiteWebParts;
			}
			set
			{
				this.m_bCopySiteWebParts = value;
			}
		}

		public bool CopyUncustomizedFiles
		{
			get
			{
				return this._copyUncustomizedFiles;
			}
			set
			{
				this._copyUncustomizedFiles = value;
			}
		}

		[CmdletEnabledParameter("FilterCustomFiles", true)]
		public IFilterExpression CustomFileFilterExpression
		{
			get
			{
				return this.m_customFileFilterExpression;
			}
			set
			{
				this.m_customFileFilterExpression = value;
			}
		}

		[CmdletEnabledParameter("FilterCustomFolders", true)]
		public IFilterExpression CustomFolderFilterExpression
		{
			get
			{
				return this.m_customFolderFilterExpression;
			}
			set
			{
				this.m_customFolderFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public new bool ExistingWebPartsActionAllowed
		{
			get
			{
				if (base.CopyDocumentWebParts)
				{
					return true;
				}
				return this.CopySiteWebParts;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterCustomFiles
		{
			get
			{
				return this.m_bFilterCustomFiles;
			}
			set
			{
				this.m_bFilterCustomFiles = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterCustomFolders
		{
			get
			{
				return this.m_bFilterCustomFolders;
			}
			set
			{
				this.m_bFilterCustomFolders = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterSiteFields
		{
			get
			{
				return this.m_bFilterSiteFields;
			}
			set
			{
				this.m_bFilterSiteFields = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterSites
		{
			get
			{
				return this.m_bFilterSites;
			}
			set
			{
				this.m_bFilterSites = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool MapChildWebTemplates
		{
			get
			{
				return this.m_bMapChildSiteTemplates;
			}
			set
			{
				this.m_bMapChildSiteTemplates = value;
			}
		}

		[CmdletEnabledParameter("CopySiteFeatures", true)]
		public bool MergeSiteFeatures
		{
			get
			{
				return this.m_bMergeSiteFeatures;
			}
			set
			{
				this.m_bMergeSiteFeatures = value;
			}
		}

		public bool OverwriteSites
		{
			get
			{
				return this.m_bOverwriteSites;
			}
			set
			{
				this.m_bOverwriteSites = value;
				if (this.m_bOverwriteSites)
				{
					this.OverwriteLists = true;
				}
			}
		}

		public bool PreserveMasterPage
		{
			get
			{
				return this.m_bPreserveMasterPage;
			}
			set
			{
				this.m_bPreserveMasterPage = value;
			}
		}

		public bool PreserveUIVersion
		{
			get
			{
				return this.m_bPreserveUIVersion;
			}
			set
			{
				this.m_bPreserveUIVersion = value;
			}
		}

		public bool RecursivelyCopySubsites
		{
			get
			{
				return this.m_bRecursivelyCopySubsites;
			}
			set
			{
				this.m_bRecursivelyCopySubsites = value;
			}
		}

		public bool RunNavigationStructureCopy
		{
			get
			{
				return this.m_bRunNavigationStructureCopy;
			}
			set
			{
				this.m_bRunNavigationStructureCopy = value;
			}
		}

		[CmdletEnabledParameter("FilterSiteFields", true)]
		public IFilterExpression SiteFieldsFilterExpression
		{
			get
			{
				return this.m_siteFieldFilterExpression;
			}
			set
			{
				this.m_siteFieldFilterExpression = value;
			}
		}

		[CmdletEnabledParameter("FilterSites", true)]
		public IFilterExpression SiteFilterExpression
		{
			get
			{
				return this.m_siteFilterExpression;
			}
			set
			{
				this.m_siteFilterExpression = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public int UpdateSiteOptionsBitField
		{
			get
			{
				return this.m_iUpdateOptions;
			}
			set
			{
				this.m_iUpdateOptions = value;
			}
		}

		[CmdletParameterAlias("UpdateSites")]
		public string[] UpdateSiteOptionsForPowershell
		{
			get
			{
				if (!this.UpdateSites)
				{
					return new string[0];
				}
				return SystemUtils.GetFlagsEnumNamesAsArray(typeof(UpdateSiteFlags), this.UpdateSiteOptionsBitField);
			}
		}

		[CmdletEnabledParameter(false)]
		public bool UpdateSites
		{
			get
			{
				return this.m_bUpdateSites;
			}
			set
			{
				this.m_bUpdateSites = value;
			}
		}

		[CmdletEnabledParameter("MapChildWebTemplates", true)]
		public CommonSerializableTable<string, string> WebTemplateMappingTable
		{
			get
			{
				return this.m_stWebTemplateMapping;
			}
			set
			{
				this.m_stWebTemplateMapping = value;
			}
		}

		[CmdletEnabledParameter("ChangeWebTemplate", true)]
		[UsesStickySettings(false)]
		public string WebTemplateName
		{
			get
			{
				return this.m_sWebTemplateName;
			}
			set
			{
				this.m_sWebTemplateName = value;
			}
		}

		public PasteSiteOptions()
		{
			base.CopyDefaultPageWebPartsAtItemsLevel = false;
		}

		public override void MakeOptionsIncremental(DateTime? incrementFromTime)
		{
			base.MakeOptionsIncremental(incrementFromTime);
			this.OverwriteSites = false;
			this.UpdateSites = true;
			this.UpdateSiteOptionsBitField = 2163;
			if (this.CopySiteWebParts)
			{
				base.ExistingWebPartsAction = ExistingWebPartsProtocol.Delete;
			}
		}
	}
}