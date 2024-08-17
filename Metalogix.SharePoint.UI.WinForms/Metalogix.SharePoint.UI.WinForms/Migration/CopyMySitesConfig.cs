using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteMySitesAction) })]
	public class CopyMySitesConfig : IActionConfig
	{
		public CopyMySitesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			CopyMySitesActionDialog copyMySitesActionDialog = new CopyMySitesActionDialog(isClientOM);
			PasteMySiteOptions actionOptions = context.GetActionOptions<PasteMySiteOptions>();
			SPBaseServer item = context.ActionContext.Targets[0] as SPBaseServer;
			if (actionOptions.SelfServiceCreateMode && item != null && item.WebApplications != null)
			{
				if (!string.IsNullOrEmpty(actionOptions.WebApplicationName))
				{
					bool flag = false;
					foreach (SPWebApplication webApplication in item.WebApplications)
					{
						if (actionOptions.WebApplicationName != webApplication.Name)
						{
							continue;
						}
						flag = true;
						break;
					}
					if (!flag)
					{
						actionOptions.WebApplicationName = item.WebApplication.Name;
					}
				}
				else
				{
					actionOptions.WebApplicationName = item.WebApplication.Name;
				}
			}
			try
			{
				PasteMySitesAction.InitializeMappings(context.GetActionOptions<PasteSiteOptions>(), context.ActionContext.Sources[0] as SPServer, item.Languages[0].Templates);
			}
			catch
			{
			}
			copyMySitesActionDialog.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
			copyMySitesActionDialog.TargetNodes = context.ActionContext.GetTargetsAsNodeCollection();
			copyMySitesActionDialog.Options = actionOptions;
			copyMySitesActionDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyMySitesActionDialog.ShowDialog();
			return copyMySitesActionDialog.ConfigurationResult;
		}
	}
}