using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteAllSubSitesAction) })]
	public class CopySubSitesConfig : IActionConfig
	{
		public CopySubSitesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			if (!SPUIUtils.ExecutePreSiteCopyConfigurationChecks(context.ActionContext.Sources, context.ActionContext.Targets, context.GetActionOptions<PasteSiteOptions>()))
			{
				return ConfigurationResult.Cancel;
			}
			SPWeb item = context.ActionContext.Sources[0] as SPWeb;
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			CopySiteActionDialog copySiteActionDialog = new CopySiteActionDialog(isClientOM)
			{
				MultiSelectUI = true
			};
			if (item.SubWebs.Count <= 0)
			{
				copySiteActionDialog.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
			}
			else
			{
				copySiteActionDialog.SourceNodes = item.SubWebs;
			}
			copySiteActionDialog.TargetNodes = targetsAsNodeCollection;
			copySiteActionDialog.Options = context.GetActionOptions<PasteSiteOptions>();
			copySiteActionDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copySiteActionDialog.ShowDialog();
			return copySiteActionDialog.ConfigurationResult;
		}
	}
}