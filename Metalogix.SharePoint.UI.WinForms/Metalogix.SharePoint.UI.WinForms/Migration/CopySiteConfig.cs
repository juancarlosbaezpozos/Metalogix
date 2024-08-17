using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteSiteAction) })]
	public class CopySiteConfig : IActionConfig
	{
		public CopySiteConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			PasteSiteOptions actionOptions = context.GetActionOptions<PasteSiteOptions>();
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			if (!SPUIUtils.ExecutePreSiteCopyConfigurationChecks(context.ActionContext.Sources, context.ActionContext.Targets, actionOptions))
			{
				return ConfigurationResult.Cancel;
			}
			if (context.ActionContext.Sources.Count > 1 && (!actionOptions.RenameSpecificNodes || actionOptions.TaskCollection == null || actionOptions.TaskCollection.Count == 0) && PasteActionUtils.CheckNodeNameCollisions(context.ActionContext.Sources))
			{
				FlatXtraMessageBox.Show(Resources.Site_Collision_Warning, Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			ActionOptions actionOption = context.Action.Options.Clone();
			do
			{
				configurationResult = this.OpenDialog(context, actionOptions);
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

		private ConfigurationResult OpenDialog(ActionConfigContext context, PasteSiteOptions options)
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
			copySiteActionDialogBasicView.Context = context.ActionContext;
			if (context.ActionContext.Sources.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copySiteActionDialogBasicView.MultiSelectUI = true;
			}
			if (!flag)
			{
				copySiteActionDialogBasicView.Action.JobID = context.Action.JobID;
				((CopySiteActionDialogBasicView)copySiteActionDialogBasicView).Options = options;
			}
			else
			{
				((CopySiteActionDialog)copySiteActionDialogBasicView).Options = options;
				copySiteActionDialogBasicView.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			}
			copySiteActionDialogBasicView.ShowDialog();
			return copySiteActionDialogBasicView.ConfigurationResult;
		}
	}
}