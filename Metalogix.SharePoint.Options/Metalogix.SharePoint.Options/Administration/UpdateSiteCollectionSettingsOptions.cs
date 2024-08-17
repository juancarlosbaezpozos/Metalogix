using Metalogix.Actions;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;
using System.Text;

namespace Metalogix.SharePoint.Options.Administration
{
	public class UpdateSiteCollectionSettingsOptions : SharePointActionOptions
	{
		private Metalogix.SharePoint.Migration.LinkCorrectionScope m_bLinkCorrectionScope;

		private bool m_bSetSiteCollectionAdmins;

		private string m_sSiteCollectionAdmins;

		private static char[] s_adminDelimiter;

		private bool m_bSetSiteQuota;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private string m_sQuotaID;

		[CmdletEnabledParameter(false)]
		public new Metalogix.SharePoint.Migration.LinkCorrectionScope LinkCorrectionScope
		{
			get
			{
				return this.m_bLinkCorrectionScope;
			}
			set
			{
				this.m_bLinkCorrectionScope = value;
			}
		}

		[CmdletEnabledParameter("SetSiteQuota", true)]
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

		[CmdletEnabledParameter("SetSiteQuota", true)]
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

		[CmdletEnabledParameter("SetSiteQuota", true)]
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

		[CmdletEnabledParameter(false)]
		public bool SetSiteCollectionAdmins
		{
			get
			{
				return this.m_bSetSiteCollectionAdmins;
			}
			set
			{
				this.m_bSetSiteCollectionAdmins = value;
			}
		}

		[CmdletEnabledParameter(false)]
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

		[CmdletEnabledParameter(false)]
		public string SiteCollectionAdmins
		{
			get
			{
				return this.m_sSiteCollectionAdmins;
			}
			set
			{
				this.m_sSiteCollectionAdmins = value;
			}
		}

		[CmdletEnabledParameter("SetSiteCollectionAdmins", true)]
		[CmdletParameterAlias("SiteCollectionAdmins")]
		public string[] SiteCollectionAdminsList
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_sSiteCollectionAdmins))
				{
					return new string[0];
				}
				return this.m_sSiteCollectionAdmins.Split(UpdateSiteCollectionSettingsOptions.s_adminDelimiter);
			}
		}

		static UpdateSiteCollectionSettingsOptions()
		{
			UpdateSiteCollectionSettingsOptions.s_adminDelimiter = new char[] { ';' };
		}

		public UpdateSiteCollectionSettingsOptions()
		{
		}

		public void SetAdminsFromList(string[] lsAdmins)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] strArrays = lsAdmins;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(str);
			}
			this.SiteCollectionAdmins = stringBuilder.ToString();
		}
	}
}