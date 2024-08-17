using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteListAsFolderAction) })]
	[ActionConfig(new Type[] { typeof(PasteFolderAction) })]
	public class CopyFolderConfig : IActionConfig
	{
		public CopyFolderConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
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
							if (sPFolder.DisplayUrl.ToLower().Contains(string.Concat(current.DisplayUrl.ToLower(), "/")) || sPFolder.DisplayUrl == current.DisplayUrl)
							{
								empty = "Cannot Copy: Target is a Child Folder of the Source";
								if (sPFolder.DisplayUrl == current.DisplayUrl)
								{
									empty = "Cannot Copy: Target is the same as the Source";
								}
								FlatXtraMessageBox.Show(empty, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								configurationResult = ConfigurationResult.Cancel;
								return configurationResult;
							}
							else
							{
								if (current.IsOneNoteFolder && sPFolder.IsOneNoteFolder)
								{
									empty = "Cannot Copy: OneNote folder cannot be copied directly into another OneNote folder. Use 'Paste Folder Content > All List Items and Folders...' action instead.";
								}
								else if (sPFolder.IsOneNoteFolder)
								{
									empty = "Cannot Copy: OneNote folder cannot be target for Copy Folder action.";
								}
								if (empty == string.Empty)
								{
									continue;
								}
								FlatXtraMessageBox.Show(empty, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								configurationResult = ConfigurationResult.Cancel;
								return configurationResult;
							}
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
				goto Label0;
			}
			finally
			{
				IDisposable disposable1 = enumerator as IDisposable;
				if (disposable1 != null)
				{
					disposable1.Dispose();
				}
			}
			return configurationResult;
		Label0:
			if (context.ActionContext.Sources.Count > 1 && (!context.GetActionOptions<PasteFolderOptions>().RenameSpecificNodes || context.GetActionOptions<PasteFolderOptions>().TaskCollection == null || context.GetActionOptions<PasteFolderOptions>().TaskCollection.Count == 0) && PasteActionUtils.CheckNodeNameCollisions(context.ActionContext.Sources))
			{
				FlatXtraMessageBox.Show(Resources.Folder_Collision_Warning, Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			NodeCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			if (PasteActionUtils.CollectionContainsMultipleLists(sourcesAsNodeCollection) || PasteActionUtils.CollectionContainsMultipleLists(targetsAsNodeCollection))
			{
				context.GetActionOptions<PasteFolderOptions>().ToggleOptionsForMultiSelectCase();
			}
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			CopyFolderActionDialog copyFolderActionDialog = new CopyFolderActionDialog(isClientOM);
			if (context.ActionContext.Sources.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copyFolderActionDialog.MultiSelectUI = true;
			}
			copyFolderActionDialog.SourceNodes = sourcesAsNodeCollection;
			copyFolderActionDialog.TargetNodes = targetsAsNodeCollection;
			copyFolderActionDialog.Options = context.GetActionOptions<PasteFolderOptions>();
			copyFolderActionDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyFolderActionDialog.ShowDialog();
			return copyFolderActionDialog.ConfigurationResult;
		}
	}
}