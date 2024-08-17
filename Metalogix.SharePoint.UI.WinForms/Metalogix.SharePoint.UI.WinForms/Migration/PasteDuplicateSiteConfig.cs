using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteDuplicateSiteAction) })]
	public class PasteDuplicateSiteConfig : IActionConfig
	{
		public PasteDuplicateSiteConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPWeb item = context.ActionContext.Sources[0] as SPWeb;
			SPWeb sPWeb = context.ActionContext.Targets[0] as SPWeb;
			PasteSiteOptions actionOptions = context.GetActionOptions<PasteSiteOptions>();
			if (!this.IsValidRenameConfiguration(this.GetSiteRenameTask(item, actionOptions), item))
			{
				RenameDialog renameDialog = new RenameDialog(item)
				{
					ForceRename = true
				};
				TransformationTask task = actionOptions.TaskCollection.GetTask(item, new CompareDatesInUtc());
				renameDialog.TargetName = (task == null || task.ChangeOperations["Name"] == null ? item.Name : task.ChangeOperations["Name"]);
				renameDialog.TargetTitle = (task == null || task.ChangeOperations["Title"] == null ? item.Title : task.ChangeOperations["Title"]);
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
				actionOptions.TaskCollection.TransformationTasks.Add(renameDialog.Task);
			}
			actionOptions.RenameSpecificNodes = true;
			if (actionOptions.WebTemplateName == null)
			{
				PasteSiteAction.InitializeMappings(actionOptions, item, sPWeb.Templates);
			}
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			bool isClientOM = false;
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			CopySiteActionDialog copySiteActionDialog = new CopySiteActionDialog(isClientOM)
			{
				RequireRootRename = true,
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = targetsAsNodeCollection,
				Options = actionOptions
			};
			copySiteActionDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copySiteActionDialog.ShowDialog();
			return copySiteActionDialog.ConfigurationResult;
		}

		private TransformationTask GetSiteRenameTask(SPWeb sourceWeb, PasteSiteOptions options)
		{
			if (options.TaskCollection == null)
			{
				return null;
			}
			return options.TaskCollection.GetTask(sourceWeb, new CompareDatesInUtc());
		}

		private bool IsValidRenameConfiguration(TransformationTask renameTask, SPWeb sourceWeb)
		{
			if (renameTask == null || !renameTask.ChangeOperations.ContainsKey("Name") || string.IsNullOrEmpty(renameTask.ChangeOperations["Name"]))
			{
				return false;
			}
			return renameTask.ChangeOperations["Name"].ToLower() != sourceWeb.Name.ToLower();
		}
	}
}