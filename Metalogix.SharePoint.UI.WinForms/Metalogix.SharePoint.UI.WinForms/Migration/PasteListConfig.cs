using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
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
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteListAction) })]
	public class PasteListConfig : IActionConfig
	{
		public PasteListConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			ConfigurationResult configurationResult1;
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			IEnumerator enumerator = context.ActionContext.Sources.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPList current = (SPList)enumerator.Current;
					IEnumerator enumerator1 = context.ActionContext.Targets.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							SPWeb sPWeb = (SPWeb)enumerator1.Current;
							if (current.ParentWeb.DisplayUrl != sPWeb.DisplayUrl)
							{
								continue;
							}
							FlatXtraMessageBox.Show(Resources.ListCopyOverwriteWarning, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							configurationResult1 = ConfigurationResult.Cancel;
							return configurationResult1;
						}
					}
					finally
					{
						IDisposable disposable = enumerator1 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				if (context.ActionContext.Sources.Count > 1 && (!context.GetActionOptions<PasteListOptions>().RenameSpecificNodes || context.GetActionOptions<PasteListOptions>().TaskCollection == null || context.GetActionOptions<PasteListOptions>().TaskCollection.Count == 0) && PasteActionUtils.CheckNodeNameCollisions(context.ActionContext.Sources))
				{
					FlatXtraMessageBox.Show(Resources.List_Collision_Warning, Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
				PasteListOptions options = context.Action.Options as PasteListOptions;
				if (options != null)
				{
					options.IsFromAdvancedMode = null;
				}
				return configurationResult;
			}
			finally
			{
				IDisposable disposable1 = enumerator as IDisposable;
				if (disposable1 != null)
				{
					disposable1.Dispose();
				}
			}
			return configurationResult1;
		}

		private ConfigurationResult OpenDialog(ActionConfigContext context)
		{
			ScopableLeftNavigableTabsForm copyListActionDialogBasicView;
			NodeCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool flag = BCSHelper.HasExternalListsOnly(sourcesAsNodeCollection);
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				SPNode item = targetsAsNodeCollection[0] as SPNode;
				if (item != null)
				{
					isClientOM = item.Adapter.IsClientOM;
				}
			}
			bool flag1 = SPUIUtils.ShouldShowAdvancedMode(context.ActionOptions);
			if (!flag1)
			{
				copyListActionDialogBasicView = new CopyListActionDialogBasicView(flag, isClientOM);
			}
			else
			{
				copyListActionDialogBasicView = new CopyListActionDialog(flag, isClientOM);
			}
			if (context.ActionContext.Targets.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copyListActionDialogBasicView.MultiSelectUI = true;
			}
			copyListActionDialogBasicView.SourceNodes = sourcesAsNodeCollection;
			copyListActionDialogBasicView.TargetNodes = targetsAsNodeCollection;
			copyListActionDialogBasicView.Context = context.ActionContext;
			if (!flag1)
			{
				copyListActionDialogBasicView.Action.JobID = context.Action.JobID;
				((CopyListActionDialogBasicView)copyListActionDialogBasicView).Options = context.GetActionOptions<PasteListOptions>();
			}
			else
			{
				((CopyListActionDialog)copyListActionDialogBasicView).Options = context.GetActionOptions<PasteListOptions>();
				copyListActionDialogBasicView.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			}
			copyListActionDialogBasicView.ShowDialog();
			return copyListActionDialogBasicView.ConfigurationResult;
		}
	}
}