using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPMySiteOptions : OptionsBase
	{
		private string m_sPath;

		private string m_sURL;

		private string m_sWebApplicationName;

		private List<SPSite> m_MySitesToInclude;

		private CommonSerializableList<string> m_MySitesToExclude = new CommonSerializableList<string>();

		private string m_sMySiteData;

		private bool m_bCopyListContent;

		private bool m_bSelfServiceMode;

		private string m_sLanguageCode;

		public bool CopySubSiteAndListContent
		{
			get
			{
				return this.m_bCopyListContent;
			}
			set
			{
				this.m_bCopyListContent = value;
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

		public string MySiteData
		{
			get
			{
				return this.m_sMySiteData;
			}
			set
			{
				this.m_sMySiteData = value;
			}
		}

		public CommonSerializableList<string> MySitesToExclude
		{
			get
			{
				return this.m_MySitesToExclude;
			}
			set
			{
				this.m_MySitesToExclude = value;
			}
		}

		public List<SPSite> MySitesToInclude
		{
			get
			{
				return this.m_MySitesToInclude;
			}
			set
			{
				this.m_MySitesToInclude = value;
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

		public SPMySiteOptions()
		{
		}
	}
}