using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public class SiteHealthScoreProvider : IHealthScoreProvider
	{
		private readonly IEnumerable<Node> _nodes;

		public SiteHealthScoreProvider(IEnumerable<Node> nodes)
		{
			if (nodes == null)
			{
				throw new ArgumentNullException("nodes");
			}
			this._nodes = nodes;
		}

		public int GetHealthScore()
		{
			return this.GetMaximumHealthScore(this._nodes);
		}

		private int? GetHealthScore(SPSite site)
		{
			ServerHealthInformation serverHealth = site.GetServerHealth();
			if (serverHealth != null)
			{
				return serverHealth.HealthScore;
			}
			return null;
		}

		private int GetMaximumHealthScore(IEnumerable<Node> nodeCollection)
		{
			int num = 0;
			if (nodeCollection == null)
			{
				return num;
			}
			foreach (Node node in nodeCollection)
			{
				SPNode sPNode = node as SPNode;
				if (sPNode == null)
				{
					continue;
				}
				SPSite siteFromSPNode = this.GetSiteFromSPNode(sPNode);
				if (siteFromSPNode == null)
				{
					continue;
				}
				int? healthScore = this.GetHealthScore(siteFromSPNode);
				if (!healthScore.HasValue)
				{
					continue;
				}
				num = Math.Max(healthScore.Value, num);
			}
			return num;
		}

		private SPSite GetSiteFromSPNode(SPNode node)
		{
			SPSite sPSite = node as SPSite;
			if (sPSite != null)
			{
				return sPSite;
			}
			SPWeb sPWeb = node as SPWeb;
			if (sPWeb != null)
			{
				return sPWeb.RootSite;
			}
			SPList sPList = node as SPList;
			if (sPList != null)
			{
				return sPList.ParentWeb.RootSite;
			}
			SPFolder sPFolder = node as SPFolder;
			if (sPFolder != null)
			{
				return sPFolder.ParentList.ParentWeb.RootSite;
			}
			SPListItem sPListItem = node as SPListItem;
			if (sPListItem == null)
			{
				return null;
			}
			return sPListItem.ParentList.ParentWeb.RootSite;
		}
	}
}