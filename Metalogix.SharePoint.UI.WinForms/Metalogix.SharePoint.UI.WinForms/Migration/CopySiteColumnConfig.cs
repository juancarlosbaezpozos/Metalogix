using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopySiteColumnsAction) })]
	public class CopySiteColumnConfig : IActionConfig
	{
		public CopySiteColumnConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			PasteSiteColumnsOptions actionOptions = context.GetActionOptions<PasteSiteColumnsOptions>();
			CopySiteColumnsDialog copySiteColumnsDialog = new CopySiteColumnsDialog()
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Context = context.ActionContext,
				Action = context.Action,
				Options = actionOptions
			};
			copySiteColumnsDialog.EnableTransformerConfiguration(context.Action, context.ActionContext, context.Action.Options.Transformers);
			copySiteColumnsDialog.ShowDialog();
			return copySiteColumnsDialog.ConfigurationResult;
		}
	}
}