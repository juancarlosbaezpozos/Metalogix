using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Administration
{
	public class CreateSiteCollectionOptions : SharePointActionOptions
	{
		private SPWebTemplate m_template;

		private SPWebApplication m_webApp;

		private SPLanguage m_language;

		private string m_sTitle;

		private string m_sDescription;

		private string m_sPath;

		private string m_sURL;

		private string m_sContentDatabaseName;

		private string m_sOwnerLogin;

		private string m_sSecondaryLogin;

		private bool m_bValidateLogins = true;

		private int m_iExperienceVersion = -1;

		private bool m_bSelfServiceCreateMode;

		private bool _isHostHeader;

		private string _hostHeaderURL;

		private bool m_bSetSiteQuota;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private string m_sQuotaID;

		private long _storageQuota = (long)110;

		private double _resourceQuota = 300;

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

		[CmdletEnabledParameter(false)]
		public SPLanguage Language
		{
			get
			{
				return this.m_language;
			}
			set
			{
				this.m_language = value;
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
				return this.m_bSelfServiceCreateMode;
			}
			set
			{
				this.m_bSelfServiceCreateMode = value;
			}
		}

		public bool SetSiteQuota
		{
			get
			{
				return this.m_bSetSiteQuota;
			}
			set
			{
				this.m_bSetSiteQuota = value;
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

		[CmdletEnabledParameter(false)]
		public SPWebTemplate Template
		{
			get
			{
				return this.m_template;
			}
			set
			{
				this.m_template = value;
			}
		}

		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
			set
			{
				this.m_sTitle = value;
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

		[CmdletEnabledParameter(false)]
		public override bool Verbose
		{
			get
			{
				return base.Verbose;
			}
			set
			{
				base.Verbose = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public SPWebApplication WebApplication
		{
			get
			{
				return this.m_webApp;
			}
			set
			{
				this.m_webApp = value;
			}
		}

		public CreateSiteCollectionOptions()
		{
		}
	}
}