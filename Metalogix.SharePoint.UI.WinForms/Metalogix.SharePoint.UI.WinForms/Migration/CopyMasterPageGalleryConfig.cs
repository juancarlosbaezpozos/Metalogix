using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyMasterPageGalleryAction) })]
	public class CopyMasterPageGalleryConfig : IActionConfig
	{
		public CopyMasterPageGalleryConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (!SPUIUtils.NotifyDisabledSubactions(context.Action, context.ActionContext.Targets))
			{
				return ConfigurationResult.Cancel;
			}
			CopyMasterPageGalleryDialog copyMasterPageGalleryDialog = new CopyMasterPageGalleryDialog()
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Options = context.GetActionOptions<PasteMasterPageGalleryOptions>(),
				MultiSelectUI = true
			};
			copyMasterPageGalleryDialog.ShowDialog();
			return copyMasterPageGalleryDialog.ConfigurationResult;
		}
	}
}