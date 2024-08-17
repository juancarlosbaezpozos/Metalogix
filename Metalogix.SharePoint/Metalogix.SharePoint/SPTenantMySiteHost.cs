using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.Explorer.Attributes;
using Metalogix.SharePoint.Adapters;
using System;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("SharePoint Tenant My Site Host")]
	[PluralName("SharePoint Tenant My Site Hosts")]
	[UserFriendlyNodeName("SharePoint Tenant My Site Host")]
	public class SPTenantMySiteHost : SPTenant
	{
		private SPSiteCollection m_sites;

		public override string DisplayName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(this.GetMySiteHostUrl(base.Adapter.ServerDisplayName));
				stringBuilder.Append(" (");
				stringBuilder.Append("Office 365 Tenant Personal/OneDrive Sites ");
				stringBuilder.Append((base.Adapter.ServerType == "Auto Detect" ? "" : string.Concat("- ", base.Adapter.ServerType, " ")));
				stringBuilder.Append((base.IsLimitedSiteCollectionConnection ? "- Limited Site Collections " : string.Empty));
				stringBuilder.Append("- ");
				stringBuilder.Append(base.Adapter.LoggedInAs);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
		}

		public override string DisplayUrl
		{
			get
			{
				return this.GetMySiteHostUrl(base.DisplayUrl);
			}
		}

		public override string LinkableUrl
		{
			get
			{
				string str = (new Uri(new Uri(base.DisplayUrl), "_layouts/15/tenantprofileadmin/manageuserprofileserviceapplication.aspx")).ToString();
				return str;
			}
		}

		public override SPSiteCollection Sites
		{
			get
			{
				if (this.m_sites == null)
				{
					SPMySiteCollection sPMySiteCollection = new SPMySiteCollection(this);
					sPMySiteCollection.OnNodeCollectionChanged += new NodeCollectionChangedHandler(this.On_Sites_CollectionChanged);
					sPMySiteCollection.FetchData();
					this.m_sites = sPMySiteCollection;
				}
				return this.m_sites;
			}
		}

		public SPTenantMySiteHost(XmlNode connectionNode) : base(connectionNode)
		{
		}

		public SPTenantMySiteHost(SharePointAdapter adapter) : base(adapter)
		{
		}

		private string GetMySiteHostUrl(string adminURL)
		{
			return TenantSettingManager.GetTenantSetting(adminURL).MySiteHostPath;
		}
	}
}