using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteDefaultWebPartPageAction) })]
	public class PasteDefaultWebPartPageConfig : IActionConfig
	{
		public PasteDefaultWebPartPageConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			CopyDefaultWebPartPageDialog copyDefaultWebPartPageDialog = new CopyDefaultWebPartPageDialog();
			if (context.ActionContext.Sources.Count > 1 || context.ActionContext.Targets.Count > 1)
			{
				copyDefaultWebPartPageDialog.MultiSelectUI = true;
			}
			copyDefaultWebPartPageDialog.SourceNodes = context.ActionContext.GetSourcesAsNodeCollection();
			copyDefaultWebPartPageDialog.TargetNodes = context.ActionContext.GetTargetsAsNodeCollection();
			copyDefaultWebPartPageDialog.Options = context.GetActionOptions<WebPartOptions>();
			copyDefaultWebPartPageDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copyDefaultWebPartPageDialog.ShowDialog();
			return copyDefaultWebPartPageDialog.ConfigurationResult;
		}
	}
}