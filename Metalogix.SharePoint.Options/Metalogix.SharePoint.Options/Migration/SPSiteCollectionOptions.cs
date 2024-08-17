using Metalogix;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPSiteCollectionOptions : OptionsBase
	{
		private bool m_bIsSameServer;

		private bool m_bOverwriteSites;

		private bool m_bCopyMasterPageGallery;

		private bool m_bCopyMasterPages = true;

		private bool m_bCopyPageLayouts = true;

		private bool m_bCopyOtherResources = true;

		private bool m_bCorrectMasterPageLinks;

		private bool m_bCopyAuditSettings;

		private string m_sWebTemplateName;

		private string m_sLanguageCode;

		private int m_iExperienceVersion = -1;

		private string m_sName;

		private string m_sDescription;

		private string m_sPath;

		private string m_sURL;

		private string m_sOwnerLogin;

		private string m_sSecondaryLogin;

		private string m_SiteCollectionAdministrators;

		private bool m_bValidateLogins = true;

		private string m_sContentDatabaseName;

		private string m_sWebApplicationName;

		private bool m_bCopySiteAdmins;

		private bool m_bCopySiteQuota;

		private string m_sQuotaID;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private long _storageQuota = (long)110;

		private double _resourceQuota = 300;

		private string m_sSourcePath;

		private string m_sSourceUrlName;

		private string m_sSourceWebApplication;

		private bool m_bUpdateSites;

		private int m_iUpdateSiteOptionsBitField;

		private bool m_bSelfServiceMode;

		private bool _isHostHeader;

		private string _hostHeaderURL;

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

		public bool IsSameServer
		{
			get
			{
				return this.m_bIsSameServer;
			}
			set
			{
				this.m_bIsSameServer = value;
			}
		}

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

		public bool OverwriteSites
		{
			get
			{
				return this.m_bOverwriteSites;
			}
			set
			{
				this.m_bOverwriteSites = value;
			}
		}

		public string OwnerLogin
		{
			get
			{
				return this.m_sOwnerLogin;
			}
			set
			{
				this.m_sOwnerLogin = value;
			}
		}

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

		public int UpdateSiteOptionsBitField
		{
			get
			{
				return this.m_iUpdateSiteOptionsBitField;
			}
			set
			{
				this.m_iUpdateSiteOptionsBitField = value;
			}
		}

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

		public string URL
		{
			get
			{
				return this.m_sURL;
			}
			set
			{
				this.m_sURL = value;
			}
		}

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

		public string WebApplicationName
		{
			get
			{
				return this.m_sWebApplicationName;
			}
			set
			{
				this.m_sWebApplicationName = value;
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

		public SPSiteCollectionOptions()
		{
		}
	}
}