using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopySiteContentTypesAction) })]
	public class CopySiteContentTypesConfig : IActionConfig
	{
		public CopySiteContentTypesConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			PasteContentTypesOptions actionOptions = context.GetActionOptions<PasteContentTypesOptions>();
			CopyContentTypesDialog copyContentTypesDialog = new CopyContentTypesDialog()
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Context = context.ActionContext,
				Action = context.Action,
				Options = actionOptions
			};
			copyContentTypesDialog.ShowDialog();
			return copyContentTypesDialog.ConfigurationResult;
		}
	}
}