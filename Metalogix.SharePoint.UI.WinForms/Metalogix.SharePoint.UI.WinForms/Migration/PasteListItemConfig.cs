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
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteListItemAction) })]
	public class PasteListItemConfig : IActionConfig
	{
		public PasteListItemConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			SPListItemCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection() as SPListItemCollection;
			if (sourcesAsNodeCollection != null && sourcesAsNodeCollection.ParentFolder != null)
			{
				SPFolder parentFolder = sourcesAsNodeCollection.ParentFolder as SPFolder;
				if (parentFolder != null && parentFolder.IsOneNoteFolder)
				{
					FlatXtraMessageBox.Show("Cannot Copy: This action is not allowed for OneNote items. Only 'Copy Folder' / 'Paste Folder Content > All List Items and Folders...' action is allowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return ConfigurationResult.Cancel;
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
			PasteListItemOptions options = context.Action.Options as PasteListItemOptions;
			if (options != null)
			{
				options.IsFromAdvancedMode = null;
			}
			return configurationResult;
		}

		private ConfigurationResult OpenDialog(ActionConfigContext context)
		{
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
			NodeCollection sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			ScopableLeftNavigableTabsForm listItemDialog = CopyListItemHelper.GetListItemDialog(sourcesAsNodeCollection, targetsAsNodeCollection, isClientOM, context);
			listItemDialog.ShowDialog();
			return listItemDialog.ConfigurationResult;
		}
	}
}