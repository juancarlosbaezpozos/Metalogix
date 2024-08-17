using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyWebAlertsAction) })]
	public class CopyWebAlertsConfig : IActionConfig
	{
		public CopyWebAlertsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (FlatXtraMessageBox.Show("Warning:\nIf the target lists have been modified in the last 5 minutes, copying alerts may cause users to receive email notifications about those recent modifications. \n\nWarning:\nCopying alerts will send email notifications to affected users notifying them alerts have been created on their behalf. \n\nDo you wish to proceed with copying alerts?", "Copy Alerts Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				return ConfigurationResult.Cancel;
			}
			bool isSharePointOnline = (context.ActionContext.Targets[0] as SPWeb).Adapter.SharePointVersion.IsSharePointOnline;
			CopyAlertsDialog copyAlertsDialog = new CopyAlertsDialog(false, isSharePointOnline)
			{
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection(),
				TargetNodes = context.ActionContext.GetTargetsAsNodeCollection(),
				Options = context.GetActionOptions<AlertOptions>()
			};
			copyAlertsDialog.ShowDialog();
			return copyAlertsDialog.ConfigurationResult;
		}
	}
}