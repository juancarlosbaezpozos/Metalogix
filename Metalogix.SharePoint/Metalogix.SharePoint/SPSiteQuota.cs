using Metalogix.Utilities;
using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPSiteQuota
	{
		private XmlNode m_siteQuotaXml = null;

		private SPBaseServer m_parent = null;

		public string Name
		{
			get
			{
				return this.m_siteQuotaXml.Attributes["QuotaName"].Value;
			}
		}

		public string QuotaID
		{
			get
			{
				return this.m_siteQuotaXml.Attributes["QuotaID"].Value;
			}
		}

		public long StorageLimit
		{
			get
			{
				string value = this.m_siteQuotaXml.Attributes["QuotaStorageLimit"].Value;
				return long.Parse(value);
			}
		}

		public string StorageLimitDisplay
		{
			get
			{
				return Format.FormatSize(new long?(this.StorageLimit));
			}
		}

		public long StorageWarning
		{
			get
			{
				string value = this.m_siteQuotaXml.Attributes["QuotaStorageWarning"].Value;
				return long.Parse(value);
			}
		}

		public string StorageWarningDisplay
		{
			get
			{
				return Format.FormatSize(new long?(this.StorageWarning));
			}
		}

		public string XML
		{
			get
			{
				return this.m_siteQuotaXml.OuterXml;
			}
		}

		public SPSiteQuota(SPBaseServer parent, XmlNode node)
		{
			this.m_parent = parent;
			this.m_siteQuotaXml = node;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}