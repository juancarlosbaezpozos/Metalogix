using Metalogix.Actions;
using Metalogix.Connectivity.Proxy;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public abstract class ConnectToSharePointConfig : IActionConfig
	{
		protected abstract bool ConnectAsReadOnly
		{
			get;
		}

		protected ConnectToSharePointConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConnectToSharePointDialog connectToSharePointDialog = new ConnectToSharePointDialog();
			connectToSharePointDialog.SetVisitedServers(Metalogix.SharePoint.Settings.VisitedSPServers.ToArray());
			connectToSharePointDialog.ConnectAsReadOnly = this.ConnectAsReadOnly;
			if (connectToSharePointDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			SPConnection connectionNode = connectToSharePointDialog.ConnectionNode;
			List<string> strs = new List<string>();
			bool includeTenantMySiteHostConnection = false;
			if (connectionNode.Adapter.ConnectionScope != ConnectionScope.Site)
			{
				bool flag = false;
				if (connectionNode.Adapter.ConnectionScope == ConnectionScope.Tenant)
				{
					flag = true;
				}
				LimitSiteCollectionsDialog limitSiteCollectionsDialog = new LimitSiteCollectionsDialog(flag)
				{
					ConnectionUrl = connectionNode.Url,
					ConnectionScope = Convert.ToString(connectionNode.Adapter.ConnectionScope)
				};
				DialogResult dialogResult = limitSiteCollectionsDialog.Show();
				if (dialogResult == DialogResult.Abort || dialogResult == DialogResult.Cancel)
				{
					return ConfigurationResult.Cancel;
				}
				includeTenantMySiteHostConnection = limitSiteCollectionsDialog.IncludeTenantMySiteHostConnection;
				connectionNode.IsLimitedSiteCollectionConnection = limitSiteCollectionsDialog.IsLimitedConnection;
				if (connectionNode.IsLimitedSiteCollectionConnection && limitSiteCollectionsDialog.LimitedSiteCollections != null)
				{
					if (!flag)
					{
						connectionNode.LimitedSiteCollections = limitSiteCollectionsDialog.LimitedSiteCollections;
					}
					else
					{
						List<string> strs1 = new List<string>();
						foreach (string limitedSiteCollection in limitSiteCollectionsDialog.LimitedSiteCollections)
						{
							TenantSetting tenantSetting = TenantSettingManager.GetTenantSetting(connectionNode.DisplayUrl);
							string[] mySiteHostPath = new string[] { tenantSetting.MySiteHostPath, tenantSetting.MySiteManagedPath };
							if (!limitedSiteCollection.StartsWith(UrlUtils.ConcatUrls(mySiteHostPath), StringComparison.InvariantCultureIgnoreCase))
							{
								strs1.Add(limitedSiteCollection);
							}
							else
							{
								strs.Add(limitedSiteCollection);
							}
						}
						connectionNode.LimitedSiteCollections = strs1;
					}
				}
			}
			Metalogix.SharePoint.Settings.VisitedSPServers.AddToFront(connectToSharePointDialog.Url);
			Metalogix.SharePoint.Settings.SaveVisitedSPServers();
			Metalogix.Explorer.Settings.ActiveConnections.Add(connectionNode);
			SPTenant sPTenant = connectToSharePointDialog.ConnectionNode as SPTenant;
			if (sPTenant != null && includeTenantMySiteHostConnection)
			{
				SharePointAdapter url = sPTenant.Adapter.CloneForNewSiteCollection();
				url.Url = sPTenant.Adapter.Url;
				if (sPTenant.Adapter.AuthenticationInitializer != null)
				{
					sPTenant.Adapter.AuthenticationInitializer.InitializeAuthenticationSettings(url);
				}
				SPTenantMySiteHost sPTenantMySiteHost = new SPTenantMySiteHost(url)
				{
					IsLimitedSiteCollectionConnection = sPTenant.IsLimitedSiteCollectionConnection,
					LimitedSiteCollections = strs
				};
				Metalogix.Explorer.Settings.ActiveConnections.Add(sPTenantMySiteHost);
			}
			if (connectToSharePointDialog.Proxy.Enabled)
			{
				SharePointConfigurationVariables.LastUsedProxyServer = connectToSharePointDialog.Proxy.Server;
				SharePointConfigurationVariables.LastUsedProxyPort = connectToSharePointDialog.Proxy.Port;
			}
			return ConfigurationResult.Save;
		}
	}
}