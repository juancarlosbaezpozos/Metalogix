using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPSiteCollection : NodeCollection
	{
		private SPBaseServer m_parentServer;

		public SPBaseServer ParentServer
		{
			get
			{
				return this.m_parentServer;
			}
		}

		public SPSiteCollection(SPBaseServer parentWeb)
		{
			this.m_parentServer = parentWeb;
		}

		public override void Add(Node item)
		{
			throw new Exception("Can't add node because WebAppName is not defined in the node XML");
		}

		public SPSite AddSiteCollection(string sWebAppName, string sSiteXml, AddSiteCollectionOptions addSiteCollOptions)
		{
			if (this.ParentServer.Adapter.Writer == null)
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			if (AdapterConfigurationVariables.MigrateLanguageSettings)
			{
				sSiteXml = XmlUtility.AddLanguageSettingsAttribute(sSiteXml, "Site", null);
			}
			string str = this.ParentServer.Adapter.Writer.AddSiteCollection(sWebAppName, sSiteXml, addSiteCollOptions);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			string value = xmlDocument.FirstChild.Attributes["ServerRelativeUrl"].Value;
			string str1 = string.Concat(this.ParentServer.WebApplications[sWebAppName].Url, value).Replace("/", "%2F");
			SPSite item = (SPSite)base[str1];
			if (item != null)
			{
				base.RemoveFromCollection(item);
			}
			item = this.AddSiteToCollection(this.ParentServer.Adapter, xmlDocument.FirstChild);
			if (this.ParentServer.IsLimitedSiteCollectionConnection)
			{
				this.ParentServer.LimitedSiteCollections.Add(item.Url);
				Metalogix.Explorer.Settings.UpgradeActiveConnections();
			}
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, item);
			return item;
		}

		public void AddSiteCollection(XmlDocument xmlDoc, SPSite site)
		{
			if (site != null)
			{
				base.RemoveFromCollection(site);
			}
			site = this.AddSiteToCollection(this.ParentServer.Adapter, xmlDoc.FirstChild);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, site);
			site.Refresh();
		}

		protected SPSite AddSiteToCollection(SharePointAdapter adapter, XmlNode siteXml)
		{
			string value;
			SharePointAdapter sharePointAdapter = adapter.CloneForNewSiteCollection();
			sharePointAdapter.Url = siteXml.Attributes["Url"].Value;
			if (adapter.AuthenticationInitializer != null)
			{
				adapter.AuthenticationInitializer.InitializeAuthenticationSettings(sharePointAdapter);
			}
			if (siteXml.Attributes["ID"] != null)
			{
				sharePointAdapter.WebID = siteXml.Attributes["ID"].Value;
			}
			if (siteXml.Attributes["HostHeader"] == null)
			{
				value = null;
			}
			else
			{
				value = siteXml.Attributes["HostHeader"].Value;
			}
			string str = value;
			if ((str == null || str.Length <= 0 ? false : sharePointAdapter is IDBReader))
			{
				((IDBReader)sharePointAdapter).HostHeader = str;
			}
			SPSite sPSite = new SPSite(sharePointAdapter, this.m_parentServer, siteXml);
			base.AddToCollection(sPSite);
			return sPSite;
		}

		public bool DeleteHostHeaderSiteCollection(SPSite site)
		{
			if ((site == null ? true : site.Parent != this.ParentServer))
			{
				throw new ArgumentException("The site specified does not belong to this server");
			}
			if (this.ParentServer.Adapter.Writer == null)
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			if (this.ParentServer.IsLimitedSiteCollectionConnection)
			{
				this.ParentServer.LimitedSiteCollections.Remove(site.Url);
				Metalogix.Explorer.Settings.UpgradeActiveConnections();
			}
			this.ParentServer.Adapter.Writer.DeleteSiteCollection(site.Url, site.WebApplication);
			bool flag = base.RemoveFromCollection(site);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, site);
			return flag;
		}

		public bool DeleteSiteCollection(string sSiteName)
		{
			return this.DeleteSiteCollection((SPSite)base[sSiteName]);
		}

		public bool DeleteSiteCollection(SPSite site)
		{
			if ((site == null ? true : site.Parent != this.ParentServer))
			{
				throw new ArgumentException("The site specified does not belong to this server");
			}
			if (this.ParentServer.Adapter.Writer == null)
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			if (this.ParentServer.IsLimitedSiteCollectionConnection)
			{
				this.ParentServer.LimitedSiteCollections.Remove(site.Url);
				Metalogix.Explorer.Settings.UpgradeActiveConnections();
			}
			SPWebApplication webApplicationByUrl = this.ParentServer.WebApplications.GetWebApplicationByUrl(site.Adapter.Server);
			string serverRelativeUrl = site.ServerRelativeUrl;
			this.ParentServer.Adapter.Writer.DeleteSiteCollection(serverRelativeUrl, webApplicationByUrl.Name);
			bool flag = base.RemoveFromCollection(site);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, site);
			return flag;
		}

		public void FetchData()
		{
			this.FetchData(false, null);
		}

		public void FetchData(bool bFetchAllSites, string sWebAppName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			string empty = string.Empty;
			if (!this.m_parentServer.IsLimitedSiteCollectionConnection)
			{
				empty = (bFetchAllSites ? this.m_parentServer.Adapter.Reader.GetSiteCollections() : this.m_parentServer.Adapter.Reader.GetSiteCollectionsOnWebApp(sWebAppName));
			}
			else
			{
				empty = this.FetchLimitSiteCollectionXml();
			}
			xmlDocument.LoadXml(empty);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Site"))
			{
				this.AddSiteToCollection(this.m_parentServer.Adapter, xmlNodes);
			}
		}

		protected string FetchLimitSiteCollectionXml()
		{
			return this.GetLimitSiteCollectionXml(this.m_parentServer.LimitedSiteCollections);
		}

		private string GetLimitSiteCollectionXml(List<string> siteCollection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			try
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				try
				{
					xmlTextWriter.WriteStartElement("SiteCollections");
					foreach (string str in siteCollection)
					{
						xmlTextWriter.WriteStartElement("Site");
						xmlTextWriter.WriteAttributeString("Url", str);
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.WriteEndElement();
				}
				finally
				{
					if (xmlTextWriter != null)
					{
						((IDisposable)xmlTextWriter).Dispose();
					}
				}
			}
			finally
			{
				if (stringWriter != null)
				{
					((IDisposable)stringWriter).Dispose();
				}
			}
			return stringBuilder.ToString();
		}

		public override Node GetNodeByUrl(string sURL)
		{
			char chr;
			Node node;
			char chr1;
			Node nodeByUrl;
			int num = 0;
			SPNode sPNode = null;
			foreach (SPSite sPSite in this)
			{
				if (sURL.StartsWith(sPSite.Adapter.ServerDisplayName))
				{
					int num1 = -1;
					string displayUrl = sPSite.DisplayUrl;
					if (!(sURL == displayUrl))
					{
						do
						{
							num1++;
							chr = (sURL.Length > num1 ? sURL[num1] : '\uFFFF');
							chr1 = (displayUrl.Length > num1 ? displayUrl[num1] : '\uFFFF');
						}
						while (chr == chr1);
						if (num1 > num)
						{
							num = num1;
							sPNode = sPSite;
						}
					}
					else
					{
						node = sPSite;
						return node;
					}
				}
			}
			if (sPNode != null)
			{
				nodeByUrl = sPNode.GetNodeByUrl(sURL);
			}
			else
			{
				nodeByUrl = null;
			}
			node = nodeByUrl;
			return node;
		}

		public override bool Remove(Node item)
		{
			if (!(item is SPSite))
			{
				throw new Exception("Can't delete node because it is not a SPSite");
			}
			return this.DeleteSiteCollection((SPSite)item);
		}
	}
}