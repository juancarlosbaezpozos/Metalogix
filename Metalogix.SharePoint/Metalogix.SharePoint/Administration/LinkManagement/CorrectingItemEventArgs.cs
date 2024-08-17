using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class CorrectingItemEventArgs : EventArgs
	{
		private readonly string m_sItemName;

		private readonly string m_sItemUrl;

		private readonly object m_oTag;

		public string ItemName
		{
			get
			{
				return this.m_sItemName;
			}
		}

		public string ItemUrl
		{
			get
			{
				return this.m_sItemUrl;
			}
		}

		public object Tag
		{
			get
			{
				return this.m_oTag;
			}
		}

		public CorrectingItemEventArgs(string sItemUrl, string sItemName, object oTag)
		{
			this.m_sItemUrl = sItemUrl;
			this.m_sItemName = sItemName;
			this.m_oTag = oTag;
		}
	}
}