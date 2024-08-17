using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteMySiteOptions : PasteSiteCollectionOptions
	{
		private List<SPSite> m_MySitesToInclude;

		private CommonSerializableList<string> m_MySitesToExclude = new CommonSerializableList<string>();

		private string m_sMySiteData;

		private bool m_bCopyListContent;

		[CmdletEnabledParameter(false)]
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

		[CmdletEnabledParameter(false)]
		public bool ExclusionsExist
		{
			get
			{
				if (this.m_MySitesToExclude == null)
				{
					return false;
				}
				return this.m_MySitesToExclude.Count > 0;
			}
		}

		[CmdletEnabledParameter(false)]
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

		[CmdletEnabledParameter(false)]
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

		[CmdletEnabledParameter(false)]
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

		public PasteMySiteOptions()
		{
		}
	}
}