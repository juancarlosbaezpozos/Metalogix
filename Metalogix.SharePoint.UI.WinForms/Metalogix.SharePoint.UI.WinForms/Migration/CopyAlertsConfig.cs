using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.UI.WinForms.Components;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ActionConfig(new Type[] { typeof(CopyAlertsAction) })]
	public class CopyAlertsConfig : IActionConfig
	{
		public CopyAlertsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			return RunSaveCancelForm.ShowDialog("Warning:\nIf the target lists have been modified in the last 5 minutes, copying alerts may cause users to receive email notifications about those recent modifications. \n\nWarning:\nCopying alerts will send email notifications to affected users notifying them alerts have been created on their behalf. \n\nDo you wish to proceed with copying alerts?", "Copy Alerts", context.Action.Image);
		}
	}
}