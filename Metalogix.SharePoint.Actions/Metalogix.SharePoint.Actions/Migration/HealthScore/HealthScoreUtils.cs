using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration.Exceptions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public static class HealthScoreUtils
	{
		public static HealthScoreBlocker CreateFileHealthScoreBlocker(string serverHealthFile)
		{
			HealthScoreBlockerSettings healthScoreBlockerSetting = new HealthScoreBlockerSettings();
			return new HealthScoreBlocker(new FileHealthScoreProvider(serverHealthFile), new FileHealthScoreProvider(serverHealthFile), healthScoreBlockerSetting);
		}

		public static LogItem CreateHealthScoreCheckSkippedLogItem()
		{
			LogItem logItem = new LogItem("Server Health Information", "Server Health Check Skipped", "", "", ActionOperationStatus.Skipped)
			{
				Information = "Server Health Score check is not supported for this adapter type."
			};
			return logItem;
		}

		public static LogItem CreateServerHealthScoreLogItem(IDictionary<string, ServerHealthInformation> healthInformations)
		{
			if (healthInformations == null)
			{
				throw new ArgumentNullException("healthInformations");
			}
			LogItem logItem = new LogItem("Server Health Information", "Server Health Scores", "", "", ActionOperationStatus.Completed)
			{
				Information = HealthScoreUtils.FormatHealthInformationsAsText(healthInformations)
			};
			return logItem;
		}

		public static HealthScoreBlocker CreateSiteHealthScoreBlocker(ActionContext context)
		{
			HealthScoreBlockerSettings healthScoreBlockerSetting = new HealthScoreBlockerSettings();
			SiteHealthScoreProvider siteHealthScoreProvider = new SiteHealthScoreProvider(context.GetSourcesAsNodeCollection());
			return new HealthScoreBlocker(siteHealthScoreProvider, new SiteHealthScoreProvider(context.GetTargetsAsNodeCollection()), healthScoreBlockerSetting);
		}

		private static string FormatHealthInformationsAsText(IDictionary<string, ServerHealthInformation> healthInformations)
		{
			if (healthInformations == null)
			{
				throw new ArgumentNullException("healthInformations");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, ServerHealthInformation> healthInformation in healthInformations)
			{
				string key = healthInformation.Key;
				ServerHealthInformation value = healthInformation.Value;
				if (value == null)
				{
					stringBuilder.AppendFormat(string.Concat("The server health for '{0}' is not available.", Environment.NewLine), key);
				}
				else if (value.HealthScore.HasValue)
				{
					int num = value.HealthScore.Value;
					string userFriendlyServerHealth = HealthScoreUtils.GetUserFriendlyServerHealth(num);
					stringBuilder.AppendFormat(string.Concat("The server health for '{0}' is {1}. (Health score: {2})", Environment.NewLine), key, userFriendlyServerHealth, num);
				}
				else
				{
					stringBuilder.AppendFormat(string.Concat("The server health for '{0}' is unknown.", Environment.NewLine), key);
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetUserFriendlyServerHealth(int healthScore)
		{
			if (healthScore < 4)
			{
				return "Excellent";
			}
			if (healthScore < 6)
			{
				return "Good";
			}
			if (healthScore < 8)
			{
				return "Fair";
			}
			return "Poor";
		}

		public static bool NeitherSourceNorTargetSupportsHealthMonitor(IXMLAbleList source, IXMLAbleList target)
		{
			if (HealthScoreUtils.SupportsServerHealthMonitor(source))
			{
				return false;
			}
			return !HealthScoreUtils.SupportsServerHealthMonitor(target);
		}

		public static bool SupportsServerHealthMonitor(IXMLAbleList nodes)
		{
			if (nodes == null || nodes.Count == 0)
			{
				return false;
			}
			object item = nodes[0];
			if (item == null)
			{
				throw new MissingSharePointNodeException();
			}
			SPNode sPNode = item as SPNode;
			if (sPNode == null)
			{
				throw new NotASharePointNodeException(item.GetType());
			}
			return sPNode.Adapter is IServerHealthMonitor;
		}
	}
}