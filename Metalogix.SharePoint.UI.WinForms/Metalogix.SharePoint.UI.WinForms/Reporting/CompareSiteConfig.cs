using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Database;
using Metalogix.SharePoint.Actions.Reporting;
using Metalogix.SharePoint.Options.Reporting;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
    [ActionConfig(new Type[] { typeof(CompareSiteAction) })]
    public class CompareSiteConfig : IActionConfig
    {
        public CompareSiteConfig()
        {
        }

        public ConfigurationResult Configure(ActionConfigContext context)
        {
            CompareDialog compareDialog = new CompareDialog();
            if (!context.GetActionOptions<CompareSiteOptions>().Configured)
            {
                compareDialog.SourceNode = (SPNode)context.ActionContext.Targets[0];
                Node node = null;
                string str = null;
                SPNode sPNode = RestoreAction.MapByDatabase(compareDialog.SourceNode, out node, out str) as SPNode;
                if (sPNode != null)
                {
                    compareDialog.TargetNode = sPNode.Children[compareDialog.SourceNode.Name] as SPNode;
                }
            }
            else
            {
                compareDialog.SourceNode = (SPNode)context.ActionContext.Sources[0];
                compareDialog.TargetNode = (SPNode)context.ActionContext.Targets[0];
            }
            compareDialog.Options = context.GetActionOptions<CompareSiteOptions>();
            compareDialog.ShowDialog();
            if (compareDialog.DialogResult != DialogResult.OK)
            {
                return ConfigurationResult.Cancel;
            }
            context.ActionOptions = compareDialog.Options;
            context.GetActionOptions<CompareSiteOptions>().Configured = true;
            ActionContext actionContext = context.ActionContext;
            SPNode[] sourceNode = new SPNode[] { compareDialog.SourceNode };
            actionContext.Sources = new NodeCollection(sourceNode);
            ActionContext nodeCollection = context.ActionContext;
            SPNode[] targetNode = new SPNode[] { compareDialog.TargetNode };
            nodeCollection.Targets = new NodeCollection(targetNode);
            return ConfigurationResult.Run;
        }
    }
}