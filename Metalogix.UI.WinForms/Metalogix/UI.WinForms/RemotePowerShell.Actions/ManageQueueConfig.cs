using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.RemotePowerShell.UI;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[ActionConfig(new Type[] { typeof(ManageQueueAction) })]
	public class ManageQueueConfig : IActionConfig
	{
		public ManageQueueConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ManageQueue manageQueue;
			Form form = null;
			Form item = Application.OpenForms["ManageQueue"];
			try
			{
				IXMLAbleList sources = context.ActionContext.Sources;
				RemoteJobScheduler.Instance.RefreshJobs(true);
				if (item != null)
				{
					manageQueue = (ManageQueue)item;
					manageQueue.BringToFront();
				}
				else
				{
					manageQueue = new ManageQueue();
				}
				if (sources != null && sources.Count > 0)
				{
					form = (!(sources[0] is JobListControl) ? sources[0] as Form : ((JobListControl)sources[0]).TopLevelControl as Form);
				}
				if (form != null)
				{
					manageQueue.Owner = form;
					manageQueue.StartPosition = FormStartPosition.CenterParent;
				}
				manageQueue.Show();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = "An error occurred while viewing queued jobs.";
				GlobalServices.ErrorHandler.HandleException("Manage Queue", str, exception, ErrorIcon.Error);
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, str, true);
			}
			return ConfigurationResult.Run;
		}
	}
}