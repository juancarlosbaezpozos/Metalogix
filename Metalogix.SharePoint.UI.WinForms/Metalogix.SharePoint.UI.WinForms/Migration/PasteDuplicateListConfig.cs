using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteDuplicateListAction) })]
	public class PasteDuplicateListConfig : IActionConfig
	{
		public PasteDuplicateListConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPList item = context.ActionContext.Sources[0] as SPList;
			object obj = context.ActionContext.Targets[0];
			PasteListOptions actionOptions = context.GetActionOptions<PasteListOptions>();
			RenameDialog renameDialog = new RenameDialog(item)
			{
				ForceRename = true
			};
			TransformationTask task = actionOptions.TaskCollection.GetTask(item, new CompareDatesInUtc());
			renameDialog.TargetName = (task == null || task.ChangeOperations["Name"] == null ? item.Name : task.ChangeOperations["Name"]);
			renameDialog.TargetTitle = (task == null || task.ChangeOperations["Title"] == null ? item.Title : task.ChangeOperations["Title"]);
			if (task != null)
			{
				actionOptions.TaskCollection.TransformationTasks.Remove(task);
			}
			renameDialog.PopulateDialog();
			renameDialog.ShowDialog();
			if (renameDialog.Task == null)
			{
				return ConfigurationResult.Cancel;
			}
			if (renameDialog.Task.ChangeOperations["Name"].ToLower() == item.Name.ToLower())
			{
				return ConfigurationResult.Cancel;
			}
			NodeCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				SPNode sPNode = targetsAsNodeCollection[0] as SPNode;
				if (sPNode != null)
				{
					isClientOM = sPNode.Adapter.IsClientOM;
				}
			}
			CopyListActionDialog copyListActionDialog = new CopyListActionDialog(BCSHelper.HasExternalListsOnly(sourcesAsNodeCollection), isClientOM)
			{
				MultiSelectUI = true,
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = targetsAsNodeCollection,
				Options = context.GetActionOptions<PasteListOptions>()
			};
			copyListActionDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyListActionDialog.ShowDialog();
			if (copyListActionDialog.DialogResult != DialogResult.OK)
			{
				actionOptions.TaskCollection.TransformationTasks.Add(task);
			}
			else
			{
				actionOptions.TaskCollection.TransformationTasks.Add(renameDialog.Task);
				actionOptions.RenameSpecificNodes = true;
			}
			return copyListActionDialog.ConfigurationResult;
		}
	}
}