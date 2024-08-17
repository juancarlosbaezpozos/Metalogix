using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer.Attributes;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.Utilities;
using System;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("SharePoint Tenant")]
	[PluralName("SharePoint Tenants")]
	[UserFriendlyNodeName("SharePoint Tenant")]
	public class SPTenant : SPBaseServer
	{
		public override string DisplayName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(base.Adapter.ServerDisplayName);
				stringBuilder.Append(" (");
				stringBuilder.Append("Office 365 Tenant ");
				stringBuilder.Append((base.Adapter.ServerType == "Auto Detect" ? "" : string.Concat("- ", base.Adapter.ServerType, " ")));
				stringBuilder.Append((base.IsLimitedSiteCollectionConnection ? "- Limited Site Collections " : string.Empty));
				stringBuilder.Append("- ");
				stringBuilder.Append(base.Adapter.LoggedInAs);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
		}

		public override string ImageResourceName
		{
			get
			{
				return "Metalogix.SharePoint.Icons.SPTenant-{0}-{1}{2}.ico";
			}
		}

		public SPTenant(XmlNode connectionNode) : base(connectionNode)
		{
		}

		public SPTenant(SharePointAdapter adapter) : base(adapter)
		{
		}

		private IMySitesConnector GetMySitesConnector()
		{
			IMySitesConnector adapter = base.Adapter as IMySitesConnector;
			if (adapter == null)
			{
				throw new NotSupportedException("Adapter does not implement the IMySitesConnector interface.");
			}
			return adapter;
		}

		public SPSite GetPersonalSite(string email)
		{
			string value;
			XmlNode xmlNode = XmlUtility.StringToXmlNode(this.GetMySitesConnector().GetPersonalSite(email));
			SharePointAdapter sharePointAdapter = base.Adapter.CloneForNewSiteCollection();
			sharePointAdapter.Url = xmlNode.Attributes["Url"].Value;
			if (base.Adapter.AuthenticationInitializer != null)
			{
				base.Adapter.AuthenticationInitializer.InitializeAuthenticationSettings(sharePointAdapter);
			}
			if (xmlNode.Attributes["ID"] != null)
			{
				sharePointAdapter.WebID = xmlNode.Attributes["ID"].Value;
			}
			if (xmlNode.Attributes["HostHeader"] == null)
			{
				value = null;
			}
			else
			{
				value = xmlNode.Attributes["HostHeader"].Value;
			}
			string str = value;
			if ((string.IsNullOrEmpty(str) ? false : sharePointAdapter is IDBReader))
			{
				((IDBReader)sharePointAdapter).HostHeader = str;
			}
			return new SPSite(sharePointAdapter, this, xmlNode);
		}

		public bool HasPersonalSite(string email)
		{
			return this.GetMySitesConnector().HasPersonalSite(email);
		}

		public void ProvisionPersonalSites(string[] emails)
		{
			this.GetMySitesConnector().ProvisionPersonalSites(emails);
		}

		public void RemovePersonalSite(string email)
		{
			this.GetMySitesConnector().RemovePersonalSite(email);
		}
	}
}