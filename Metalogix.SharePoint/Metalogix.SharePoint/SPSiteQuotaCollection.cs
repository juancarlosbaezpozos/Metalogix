using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPSiteQuotaCollection : IEnumerable
	{
		private SPBaseServer m_server = null;

		private List<SPSiteQuota> m_data = null;

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public SPBaseServer Parent
		{
			get
			{
				return this.m_server;
			}
		}

		public SPSiteQuotaCollection(SPBaseServer parent)
		{
			this.m_server = parent;
			this.m_data = new List<SPSiteQuota>();
			this.FetchData();
		}

		public SPSiteQuotaCollection(SPBaseServer parent, XmlNode xmlNode)
		{
			this.m_server = parent;
			this.m_data = new List<SPSiteQuota>();
			this.FromXml(xmlNode);
		}

		public void FetchData()
		{
			string siteQuotaTemplates = this.m_server.Adapter.Reader.GetSiteQuotaTemplates();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(siteQuotaTemplates);
			this.FromXml(xmlDocument);
		}

		private void FromXml(XmlNode xmlNode)
		{
			this.m_data.Clear();
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//SiteQuotaTemplate"))
			{
				this.m_data.Add(new SPSiteQuota(this.m_server, xmlNodes));
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}
	}
}