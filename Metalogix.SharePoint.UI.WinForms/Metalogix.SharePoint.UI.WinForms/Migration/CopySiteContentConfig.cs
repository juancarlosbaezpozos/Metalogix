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
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteSiteContentAction) })]
	public class CopySiteContentConfig : IActionConfig
	{
		public CopySiteContentConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			if (!SPUIUtils.DisplayPublishingInfrastructureWarning(context.ActionContext.Sources, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			SPWeb item = context.ActionContext.Sources[0] as SPWeb;
			SPWeb sPWeb = context.ActionContext.Targets[0] as SPWeb;
			if (sPWeb.DisplayUrl == item.DisplayUrl && item.ID == sPWeb.ID)
			{
				FlatXtraMessageBox.Show("Cannot Copy: Target is the same as the Source", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return ConfigurationResult.Cancel;
			}
			if (sPWeb != null && sPWeb.IsChildOf(item))
			{
				FlatXtraMessageBox.Show("Cannot Copy: Target is a Child Site of the Source", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return ConfigurationResult.Cancel;
			}
			if (context.GetActionOptions<PasteSiteContentOptions>().WebTemplateName == null && sPWeb != null)
			{
				PasteSiteAction.InitializeMappings(context.GetActionOptions<PasteSiteOptions>(), item, sPWeb.Templates);
			}
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
			PasteSiteOptions options = context.Action.Options as PasteSiteOptions;
			if (options != null)
			{
				options.IsFromAdvancedMode = null;
			}
			return configurationResult;
		}

		private ConfigurationResult OpenDialog(ActionConfigContext context)
		{
			ScopableLeftNavigableTabsForm copySiteActionDialogBasicView;
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				SPNode item = targetsAsNodeCollection[0] as SPNode;
				if (item != null)
				{
					isClientOM = item.Adapter.IsClientOM;
				}
			}
			bool flag = SPUIUtils.ShouldShowAdvancedMode(context.ActionOptions);
			if (!flag)
			{
				copySiteActionDialogBasicView = new CopySiteActionDialogBasicView();
			}
			else
			{
				copySiteActionDialogBasicView = new CopySiteActionDialog(isClientOM);
			}
			copySiteActionDialogBasicView.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
			copySiteActionDialogBasicView.TargetNodes = targetsAsNodeCollection;
			copySiteActionDialogBasicView.MultiSelectUI = true;
			if (!flag)
			{
				copySiteActionDialogBasicView.Action.JobID = context.Action.JobID;
				((CopySiteActionDialogBasicView)copySiteActionDialogBasicView).Options = context.GetActionOptions<PasteSiteOptions>();
			}
			else
			{
				((CopySiteActionDialog)copySiteActionDialogBasicView).Options = context.GetActionOptions<PasteSiteOptions>();
				copySiteActionDialogBasicView.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			}
			copySiteActionDialogBasicView.ShowDialog();
			return copySiteActionDialogBasicView.ConfigurationResult;
		}
	}
}