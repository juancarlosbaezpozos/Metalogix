using Metalogix.Actions;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteSiteCollectionOptions : PasteSiteOptions
	{
		private string m_sLanguageCode;

		private int m_iExperienceVersion = -1;

		private string m_sName;

		private string m_sDescription;

		private string m_sPath;

		private string m_sSecondaryLogin;

		private string m_SiteCollectionAdministrators;

		private bool m_bCopyMasterPageGallery;

		private bool m_bValidateLogins = true;

		private string m_sContentDatabaseName;

		private string m_sSourcePath;

		private bool m_sIsSameServer;

		private string m_sSourceWebApplication;

		private string m_sSourceUrlName;

		private bool m_bSelfServiceMode;

		private bool _isHostHeader;

		private string _hostHeaderURL;

		private bool m_bCopyMasterPages = true;

		private bool m_bCopyPageLayouts = true;

		private bool m_bCopyOtherResources = true;

		private bool m_bCorrectMasterPageLinks;

		private bool m_bCopySiteAdmins = true;

		private bool m_bCopySiteQuota;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private string m_sQuotaID;

		private long _storageQuota = (long)110;

		private double _resourceQuota = 300;

		private bool m_bCopyAuditSettings;

		[UsesStickySettings(false)]
		public string ContentDatabaseName
		{
			get
			{
				return this.m_sContentDatabaseName;
			}
			set
			{
				this.m_sContentDatabaseName = value;
			}
		}

		public bool CopyAuditSettings
		{
			get
			{
				return this.m_bCopyAuditSettings;
			}
			set
			{
				this.m_bCopyAuditSettings = value;
			}
		}

		public bool CopyListTemplateGallery
		{
			get;
			set;
		}

		public bool CopyMasterPageGallery
		{
			get
			{
				return this.m_bCopyMasterPageGallery;
			}
			set
			{
				this.m_bCopyMasterPageGallery = value;
			}
		}

		public bool CopyMasterPages
		{
			get
			{
				return this.m_bCopyMasterPages;
			}
			set
			{
				this.m_bCopyMasterPages = value;
			}
		}

		public bool CopyOtherResources
		{
			get
			{
				return this.m_bCopyOtherResources;
			}
			set
			{
				this.m_bCopyOtherResources = value;
			}
		}

		public bool CopyPageLayouts
		{
			get
			{
				return this.m_bCopyPageLayouts;
			}
			set
			{
				this.m_bCopyPageLayouts = value;
			}
		}

		public bool CopySiteAdmins
		{
			get
			{
				return this.m_bCopySiteAdmins;
			}
			set
			{
				this.m_bCopySiteAdmins = value;
			}
		}

		public bool CopySiteQuota
		{
			get
			{
				return this.m_bCopySiteQuota;
			}
			set
			{
				this.m_bCopySiteQuota = value;
			}
		}

		public bool CorrectMasterPageLinks
		{
			get
			{
				return this.m_bCorrectMasterPageLinks;
			}
			set
			{
				this.m_bCorrectMasterPageLinks = value;
			}
		}

		[UsesStickySettings(false)]
		public string Description
		{
			get
			{
				return this.m_sDescription;
			}
			set
			{
				this.m_sDescription = value;
			}
		}

		[UsesStickySettings(false)]
		public int ExperienceVersion
		{
			get
			{
				return this.m_iExperienceVersion;
			}
			set
			{
				this.m_iExperienceVersion = value;
			}
		}

		public string HostHeaderURL
		{
			get
			{
				return this._hostHeaderURL;
			}
			set
			{
				this._hostHeaderURL = value;
			}
		}

		public bool IsHostHeader
		{
			get
			{
				return this._isHostHeader;
			}
			set
			{
				this._isHostHeader = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public bool IsSameServer
		{
			get
			{
				return this.m_sIsSameServer;
			}
			set
			{
				this.m_sIsSameServer = value;
			}
		}

		[UsesStickySettings(false)]
		public string LanguageCode
		{
			get
			{
				return this.m_sLanguageCode;
			}
			set
			{
				this.m_sLanguageCode = value;
			}
		}

		[UsesStickySettings(false)]
		public string Name
		{
			get
			{
				return this.m_sName;
			}
			set
			{
				this.m_sName = value;
			}
		}

		[UsesStickySettings(false)]
		public string OwnerLogin
		{
			get;
			set;
		}

		[UsesStickySettings(false)]
		public string Path
		{
			get
			{
				return this.m_sPath;
			}
			set
			{
				this.m_sPath = value;
			}
		}

		public string QuotaID
		{
			get
			{
				return this.m_sQuotaID;
			}
			set
			{
				this.m_sQuotaID = value;
			}
		}

		public long QuotaMaximum
		{
			get
			{
				return this.m_lQuotaMax;
			}
			set
			{
				this.m_lQuotaMax = value;
			}
		}

		public long QuotaWarning
		{
			get
			{
				return this.m_lQuotaWarning;
			}
			set
			{
				this.m_lQuotaWarning = value;
			}
		}

		public double ResourceQuota
		{
			get
			{
				return this._resourceQuota;
			}
			set
			{
				this._resourceQuota = value;
			}
		}

		[UsesStickySettings(false)]
		public string SecondaryOwnerLogin
		{
			get
			{
				return this.m_sSecondaryLogin;
			}
			set
			{
				this.m_sSecondaryLogin = value;
			}
		}

		public bool SelfServiceCreateMode
		{
			get
			{
				return this.m_bSelfServiceMode;
			}
			set
			{
				this.m_bSelfServiceMode = value;
			}
		}

		[UsesStickySettings(false)]
		public string SiteCollectionAdministrators
		{
			get
			{
				return this.m_SiteCollectionAdministrators;
			}
			set
			{
				this.m_SiteCollectionAdministrators = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public string SourcePath
		{
			get
			{
				return this.m_sSourcePath;
			}
			set
			{
				this.m_sSourcePath = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public string SourceUrlName
		{
			get
			{
				return this.m_sSourceUrlName;
			}
			set
			{
				this.m_sSourceUrlName = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public string SourceWebApplication
		{
			get
			{
				return this.m_sSourceWebApplication;
			}
			set
			{
				this.m_sSourceWebApplication = value;
			}
		}

		public long StorageQuota
		{
			get
			{
				return this._storageQuota;
			}
			set
			{
				this._storageQuota = value;
			}
		}

		[UsesStickySettings(false)]
		public string URL
		{
			get;
			set;
		}

		[CmdletEnabledParameter(false)]
		public bool ValidateOwnerLogins
		{
			get
			{
				return this.m_bValidateLogins;
			}
			set
			{
				this.m_bValidateLogins = value;
			}
		}

		[UsesStickySettings(false)]
		public string WebApplicationName
		{
			get;
			set;
		}

		public PasteSiteCollectionOptions()
		{
			this.WebApplicationName = null;
			this.OwnerLogin = null;
			this.URL = null;
		}
	}
}