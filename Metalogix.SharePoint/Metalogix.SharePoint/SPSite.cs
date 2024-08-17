using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Explorer.Attributes;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Image("Metalogix.SharePoint.Icons.SPSite-Valid.ico")]
	[Name("Top Level Site")]
	[PluralName("Top Level Sites")]
	[UserFriendlyNodeName("SharePoint Site")]
	public class SPSite : SPWeb
	{
		private long? m_iDiskUsed;

		private string m_sHostHeader;

		private string m_sWebApplication = null;

		private string m_sManagedPath = null;

		private bool _isHostHeader = false;

		public long? DiskUsed
		{
			get
			{
				return this.m_iDiskUsed;
			}
		}

		public override string DisplayName
		{
			get
			{
				string str;
				StringWriter stringWriter = new StringWriter();
				try
				{
					if (this.HostHeader == null)
					{
						stringWriter.Write(base.Adapter.Server);
					}
					else
					{
						stringWriter.Write(string.Concat((base.Adapter.Server.IndexOf("http://", StringComparison.OrdinalIgnoreCase) >= 0 ? "http://" : string.Empty), this.HostHeader));
					}
					stringWriter.Write(base.Adapter.ServerRelativeUrl);
					stringWriter.Write((!string.IsNullOrEmpty(base.Title) ? string.Concat(" - ", base.Title) : string.Empty));
					if (base.Parent is SPTenant)
					{
						if (this.DiskUsed.HasValue)
						{
							stringWriter.Write(" ({0})", Format.FormatSize(this.DiskUsed));
						}
					}
					else if (this.DiskUsed.HasValue)
					{
						string str1 = (base.GetWebTemplate(false) != null ? base.GetWebTemplate(false).Title : "Unknown Template");
						string str2 = Format.FormatSize(this.DiskUsed);
						stringWriter.Write(" ({0} - {1})", str1, str2);
					}
					str = stringWriter.ToString();
				}
				finally
				{
					if (stringWriter != null)
					{
						((IDisposable)stringWriter).Dispose();
					}
				}
				return str;
			}
		}

		public string HostHeader
		{
			get
			{
				return this.m_sHostHeader;
			}
		}

		public bool IsHostHeader
		{
			get
			{
				return this._isHostHeader;
			}
		}

		public override bool LoadAutomatically
		{
			get
			{
				bool flag;
				if ((base.Parent == null ? false : !SharePointConfigurationVariables.SiteCollectionAutoloadAll))
				{
					flag = (base.Parent.Children.Count > SharePointConfigurationVariables.SiteCollectionAutoloadThreshold ? false : true);
				}
				else
				{
					flag = true;
				}
				return flag;
			}
		}

		public string ManagedPath
		{
			get
			{
				this.EnsureQuickProperties();
				return this.m_sManagedPath;
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat(base.Adapter.ServerDisplayName.Replace("/", "%2F"), (this.HostHeader != null ? this.HostHeader : string.Empty), (this.ServerRelativeUrl.Equals("/") ? string.Empty : this.ServerRelativeUrl.Replace("/", "%2F")));
			}
		}

		public int TotalWebsInSiteCollection
		{
			get
			{
				int num;
				int num1;
				num1 = (!int.TryParse(base.GetAttributeValueFromFullXml("TotalWebsInSiteCollection"), out num) ? -1 : num);
				return num1;
			}
		}

		public string WebApplication
		{
			get
			{
				this.EnsureQuickProperties();
				return this.m_sWebApplication;
			}
		}

		public override string WebName
		{
			get
			{
				return base.Name;
			}
		}

		public SPSite(XmlNode connectionNode) : base(connectionNode)
		{
		}

		public SPSite(SharePointAdapter adapter, SPBaseServer parent) : base(adapter, parent)
		{
		}

		public SPSite(SharePointAdapter adapter, SPBaseServer parent, XmlNode webXML) : base(adapter, parent)
		{
			this.StoreQuickProperties(webXML);
		}

		protected override bool EnsureQuickProperties()
		{
			bool flag;
			if (!base.EnsureQuickProperties())
			{
				flag = false;
			}
			else
			{
				base.FireDisplayNameChanged();
				flag = true;
			}
			return flag;
		}

		protected override string FetchData()
		{
			return base.Adapter.Reader.GetSite(true);
		}

		public ServerHealthInformation GetServerHealth()
		{
			ServerHealthInformation serverHealthInformation;
			IServerHealthMonitor adapter = base.Adapter as IServerHealthMonitor;
			if (adapter != null)
			{
				string serverHealth = adapter.GetServerHealth();
				if (!string.IsNullOrEmpty(serverHealth))
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(serverHealth);
					ServerHealthInformation serverHealthInformation1 = new ServerHealthInformation();
					serverHealthInformation1.FromXml(xmlNode);
					serverHealthInformation = serverHealthInformation1;
				}
				else
				{
					serverHealthInformation = null;
				}
			}
			else
			{
				serverHealthInformation = null;
			}
			return serverHealthInformation;
		}

		protected override XmlNode GetTerseData()
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(base.Adapter.Reader.GetSite(false));
			return xmlNode;
		}

		protected override void StoreQuickProperties(XmlNode quickXML)
		{
			base.StoreQuickProperties(quickXML);
			if (quickXML.Attributes["DiskUsed"] != null)
			{
				this.m_iDiskUsed = new long?(Convert.ToInt64(quickXML.Attributes["DiskUsed"].Value));
			}
			if ((quickXML.Attributes["HostHeader"] == null ? false : quickXML.Attributes["HostHeader"].Value.Length > 0))
			{
				this.m_sHostHeader = quickXML.Attributes["HostHeader"].Value;
			}
			if ((quickXML.Attributes["WebApplication"] == null ? false : quickXML.Attributes["WebApplication"].Value.Length > 0))
			{
				this.m_sWebApplication = quickXML.Attributes["WebApplication"].Value;
			}
			if ((quickXML.Attributes["ManagedPath"] == null ? false : quickXML.Attributes["ManagedPath"].Value.Length > 0))
			{
				this.m_sManagedPath = quickXML.Attributes["ManagedPath"].Value;
			}
			this._isHostHeader = quickXML.GetAttributeValueAsBoolean("IsHostHeader");
		}

		public override string ToString()
		{
			string str;
			str = (string.IsNullOrEmpty(this.DisplayUrl) ? this.Name : this.DisplayUrl);
			return str;
		}

		public override void UpdateCurrentNode()
		{
			this.ClearExcessNodeData();
			if (base.Status == ConnectionStatus.Valid)
			{
				this.StoreQuickProperties(this.GetTerseData());
			}
			base.FireDisplayNameChanged();
		}
	}
}