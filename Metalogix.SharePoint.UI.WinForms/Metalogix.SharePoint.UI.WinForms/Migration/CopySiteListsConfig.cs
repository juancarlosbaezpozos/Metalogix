using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteSiteLists) })]
	public class CopySiteListsConfig : IActionConfig
	{
		public CopySiteListsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			SPWeb item = context.ActionContext.Sources[0] as SPWeb;
			object obj = context.ActionContext.Targets[0];
			NodeCollection sourcesAsNodeCollection = null;
			if (item.Lists.Count <= 0)
			{
				sourcesAsNodeCollection = context.ActionContext.GetSourcesAsNodeCollection();
			}
			else
			{
				sourcesAsNodeCollection = item.Lists;
			}
			bool isClientOM = false;
			NodeCollection targetsAsNodeCollection = context.ActionContext.GetTargetsAsNodeCollection();
			if (targetsAsNodeCollection != null && targetsAsNodeCollection.Count > 0)
			{
				isClientOM = (targetsAsNodeCollection[0] as SPNode).Adapter.IsClientOM;
			}
			CopyListActionDialog copyListActionDialog = new CopyListActionDialog(BCSHelper.HasExternalListsOnly(sourcesAsNodeCollection), isClientOM)
			{
				MultiSelectUI = true,
				SourceNodes = sourcesAsNodeCollection,
				TargetNodes = targetsAsNodeCollection,
				Options = context.GetActionOptions<PasteListOptions>()
			};
			copyListActionDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyListActionDialog.ShowDialog();
			return copyListActionDialog.ConfigurationResult;
		}
	}
}