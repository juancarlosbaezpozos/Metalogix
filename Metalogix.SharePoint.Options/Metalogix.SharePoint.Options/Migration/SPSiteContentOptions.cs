using Metalogix;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPSiteContentOptions : OptionsBase
	{
		private bool m_bRecursive;

		private bool m_bCopyContentTypes;

		private bool m_bCopyNavigation;

		private bool m_bCopySiteFeatures;

		private bool m_bMergeSiteFeatures = true;

		private bool m_bRenameSite;

		private string m_sNewSiteName;

		private string m_sNewSiteTitle;

		private bool m_bChangeWebTemplate;

		private string m_sWebTemplateName;

		private bool m_bApplyThemetoWeb;

		private bool copyAllThemes = true;

		private bool m_bPreserveMasterPage;

		private bool m_bPreserveUIVersion;

		private bool m_bRunNavigationStructureCopy;

		private bool m_bCopyGlobalNavigation = true;

		private bool m_bCopyCurrentNavigation = true;

		private bool m_bCopyPortalListings;

		private bool m_bCopyCustomContent;

		private bool _copyUncustomizedFiles;

		public bool ApplyThemeToWeb
		{
			get
			{
				return this.m_bApplyThemetoWeb;
			}
			set
			{
				this.m_bApplyThemetoWeb = value;
			}
		}

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

		public bool CopyAllThemes
		{
			get
			{
				return this.copyAllThemes;
			}
			set
			{
				this.copyAllThemes = value;
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

		public string NewSiteName
		{
			get
			{
				return this.m_sNewSiteName;
			}
			set
			{
				this.m_sNewSiteName = value;
			}
		}

		public string NewSiteTitle
		{
			get
			{
				return this.m_sNewSiteTitle;
			}
			set
			{
				this.m_sNewSiteTitle = value;
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
				return this.m_bRecursive;
			}
			set
			{
				this.m_bRecursive = value;
			}
		}

		public bool RenameSite
		{
			get
			{
				return this.m_bRenameSite;
			}
			set
			{
				this.m_bRenameSite = value;
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

		public SPSiteContentOptions()
		{
		}
	}
}