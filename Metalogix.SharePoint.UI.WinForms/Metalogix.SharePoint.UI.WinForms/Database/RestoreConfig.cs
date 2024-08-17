using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Database;
using Metalogix.SharePoint.Options.Database;
using Metalogix.UI.WinForms;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	[ActionConfig(new Type[] { typeof(RestoreAction) })]
	public class RestoreConfig : IActionConfig
	{
		public RestoreConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			Node item;
			Node node;
			NodeCollection targets;
			Node node1;
			RestoreOptions actionOptions = context.GetActionOptions<RestoreOptions>();
			if (actionOptions.AlgorithmMatchedLocation != null)
			{
				node1 = actionOptions.AlgorithmMatchedLocation.GetNode();
			}
			else
			{
				node1 = null;
			}
			Node node2 = node1;
			string webApplicationUrl = actionOptions.WebApplicationUrl;
			if (!actionOptions.Configured)
			{
				targets = (NodeCollection)context.ActionContext.Targets;
				node = (SPNode)context.ActionContext.Targets[0];
				item = RestoreAction.MapByDatabase(node, out node2, out webApplicationUrl);
			}
			else
			{
				targets = (NodeCollection)context.ActionContext.Sources;
				node = (SPNode)context.ActionContext.Sources[0];
				item = (SPNode)context.ActionContext.Targets[0];
			}
			/*if (node is SPListItem && ((MLLicense)ActionLicenseProvider.Instance.GetLicense(context.Action.GetType(), context.Action)).LicenseType == MLLicenseType.Evaluation)
			{
				FlatXtraMessageBox.Show("Item-level Restore is not enabled in evaluation mode.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return ConfigurationResult.Cancel;
			}*/
			RestoreActionDialog restoreActionDialog = new RestoreActionDialog();
			if (item != null)
			{
				restoreActionDialog.RestoreLocation = item.Location;
			}
			restoreActionDialog.MatchedNode = node2;
			restoreActionDialog.SourceNode = node;
			Node parent = node.Parent;
			restoreActionDialog.Options = actionOptions;
			restoreActionDialog.WebApplicationUrl = webApplicationUrl;
			restoreActionDialog.ShowDialog();
			if (restoreActionDialog.DialogResult != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			actionOptions.AlgorithmMatchedLocation = restoreActionDialog.MatchedNode.Location;
			actionOptions.LegalMatchedLocation = restoreActionDialog.LegalMatchNode.Location;
			actionOptions.WebApplicationUrl = restoreActionDialog.WebApplicationUrl;
			if (!actionOptions.Configured)
			{
				context.ActionContext.Sources = targets;
				actionOptions.Configured = true;
			}
			ActionContext actionContext = context.ActionContext;
			Node[] nodeArray = new Node[] { restoreActionDialog.RestoreLocation.GetNode() };
			actionContext.Targets = new NodeCollection(nodeArray);
			return ConfigurationResult.Run;
		}
	}
}