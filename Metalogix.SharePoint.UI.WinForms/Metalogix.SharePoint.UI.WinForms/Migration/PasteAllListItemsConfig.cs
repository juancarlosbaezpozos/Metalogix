using DevExpress.XtraEditors;
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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteAllListItemsAction) })]
	public class PasteAllListItemsConfig : IActionConfig
	{
		public PasteAllListItemsConfig()
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
					SPFolder current = (SPFolder)enumerator.Current;
					IEnumerator enumerator1 = context.ActionContext.Targets.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							SPFolder sPFolder = (SPFolder)enumerator1.Current;
							string empty = string.Empty;
							if (current.IsOneNoteFolder && !sPFolder.IsOneNoteFolder)
							{
								empty = "Cannot Copy: OneNote folder content could only be copied into another OneNote folder.";
							}
							if (!current.IsOneNoteFolder && sPFolder.IsOneNoteFolder)
							{
								empty = "Cannot Copy: Only OneNote folder content could be copied into another OneNote folder.";
							}
							if (empty == string.Empty)
							{
								continue;
							}
							FlatXtraMessageBox.Show(empty, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
				PasteFolderOptions options = context.Action.Options as PasteFolderOptions;
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
			ScopableLeftNavigableTabsForm copyFolderActionDialogBasicView;
			NodeCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			if (PasteActionUtils.CollectionContainsMultipleLists(targetsAsNodeCollection))
			{
				context.GetActionOptions<PasteFolderOptions>().ToggleOptionsForMultiSelectCase();
			}
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			bool flag = SPUIUtils.ShouldShowAdvancedMode(context.ActionOptions);
			if (!flag)
			{
				copyFolderActionDialogBasicView = new CopyFolderActionDialogBasicView();
			}
			else
			{
				copyFolderActionDialogBasicView = new CopyFolderActionDialog(isClientOM);
			}
			copyFolderActionDialogBasicView.SourceNodes = sourcesAsNodeCollection;
			copyFolderActionDialogBasicView.TargetNodes = targetsAsNodeCollection;
			copyFolderActionDialogBasicView.Context = context.ActionContext;
			if (context.ActionContext.Targets.Count > 1)
			{
				copyFolderActionDialogBasicView.MultiSelectUI = true;
			}
			if (!flag)
			{
				copyFolderActionDialogBasicView.Action.JobID = context.Action.JobID;
				((CopyFolderActionDialogBasicView)copyFolderActionDialogBasicView).Options = context.GetActionOptions<PasteFolderOptions>();
			}
			else
			{
				((CopyFolderActionDialog)copyFolderActionDialogBasicView).Options = context.GetActionOptions<PasteFolderOptions>();
				copyFolderActionDialogBasicView.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
				copyFolderActionDialogBasicView.MinimumSize = new Size(792, copyFolderActionDialogBasicView.Height);
			}
			copyFolderActionDialogBasicView.ShowDialog();
			return copyFolderActionDialogBasicView.ConfigurationResult;
		}
	}
}