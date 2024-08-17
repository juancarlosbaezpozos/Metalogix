using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(PasteListEmailNotificationAction), typeof(PasteWebEmailNotificationAction) })]
	public class PasteListEmailNotificationConfig : IActionConfig
	{
		public PasteListEmailNotificationConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (FlatXtraMessageBox.Show("Warning:\nIf the target lists' items e-mail notifcation user column has been modified in the last 5 minutes (including creation of the items through migration), copying e-mail notifications may cause users to receive email notifications about those recent modifications. \n\nDo you wish to proceed with copying the notification settings?", "Copy List E-mail Notification Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				return ConfigurationResult.Cancel;
			}
			if (!(context.ActionContext.Sources[0] is SPWeb) || !(context.ActionContext.Targets[0] is SPWeb))
			{
				return ConfigurationResult.Run;
			}
			CopyListEmailNotificationDialog copyListEmailNotificationDialog = new CopyListEmailNotificationDialog()
			{
				Options = context.GetActionOptions<ListEmailNotificationOptions>(),
				SourceNodes = context.ActionContext.GetSourcesAsNodeCollection()
			};
			copyListEmailNotificationDialog.ShowDialog();
			return copyListEmailNotificationDialog.ConfigurationResult;
		}
	}
}