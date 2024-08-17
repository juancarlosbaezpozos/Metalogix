using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteSiteCollectionAction) })]
	public class CopySiteCollectionConfig : IActionConfig
	{
		public CopySiteCollectionConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			SPBaseServer item = context.ActionContext.Targets[0] as SPBaseServer;
			SPWeb sPWeb = context.ActionContext.Sources[0] as SPWeb;
			context.GetAction<PasteSiteCollectionAction>().InitializeActionConfiguration(sPWeb, item);
			ActionOptions actionOption = context.Action.Options.Clone();
			do
			{
				configurationResult = this.OpenDialog(context);
			}
			while (configurationResult == ConfigurationResult.Switch);
			if (configurationResult == ConfigurationResult.Cancel)
			{
				context.Action.Options = actionOption;
			}
			PasteSiteCollectionOptions options = context.Action.Options as PasteSiteCollectionOptions;
			if (options != null)
			{
				options.IsFromAdvancedMode = null;
			}
			return configurationResult;
		}

		private ConfigurationResult OpenDialog(ActionConfigContext context)
		{
			ScopableLeftNavigableTabsForm copySiteCollectionActionDialogBasicView;
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			bool flag = SPUIUtils.ShouldShowAdvancedMode(context.ActionOptions);
			if (!flag)
			{
				copySiteCollectionActionDialogBasicView = new CopySiteCollectionActionDialogBasicView();
			}
			else
			{
				copySiteCollectionActionDialogBasicView = new CopySiteCollectionActionDialog(isClientOM);
			}
			copySiteCollectionActionDialogBasicView.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
			copySiteCollectionActionDialogBasicView.TargetNodes = targetsAsNodeCollection;
			copySiteCollectionActionDialogBasicView.Context = context.ActionContext;
			if (!flag)
			{
				copySiteCollectionActionDialogBasicView.Action.JobID = context.Action.JobID;
				((CopySiteCollectionActionDialogBasicView)copySiteCollectionActionDialogBasicView).Options = context.GetActionOptions<PasteSiteCollectionOptions>();
			}
			else
			{
				((CopySiteCollectionActionDialog)copySiteCollectionActionDialogBasicView).Options = context.GetActionOptions<PasteSiteCollectionOptions>();
				copySiteCollectionActionDialogBasicView.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			}
			copySiteCollectionActionDialogBasicView.ShowDialog();
			context.GetActionOptions<PasteSiteCollectionOptions>().ChangeWebTemplate = true;
			return copySiteCollectionActionDialogBasicView.ConfigurationResult;
		}
	}
}