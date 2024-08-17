using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Delete Job {0-Delete Job}")]
	[Name("Delete Job")]
	[RunAsync(false)]
	[ShowInMenus(false)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class DeleteJobAction : Metalogix.Actions.Action<Job>
	{
		public DeleteJobAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target.Count > 0 && target[0] is Job)
			{
				Job item = (Job)target[0];
				try
				{
					RemoteJobScheduler.Instance.RefreshJobs(false);
					item = RemoteJobScheduler.Instance.Jobs.FirstOrDefault<Job>((Job tempJob) => tempJob.JobID.Equals(item.JobID));
					if (item != null && item.Status == ActionStatus.Running)
					{
						FlatXtraMessageBox.Show("Cannot delete jobs while they are running.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else if (FlatXtraMessageBox.Show(Resources.Delete_Job_Message, Resources.Delete_Job, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && item != null)
					{
						RemoteJobScheduler.Instance.RemoveJob(item);
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (item != null)
					{
						string str = string.Format("An error occurred while deleting job '{0}'.", item.JobID);
						GlobalServices.ErrorHandler.HandleException(this.Name, str, exception, ErrorIcon.Error);
						Logging.LogExceptionToTextFileWithEventLogBackup(exception, str, true);
					}
				}
			}
		}
	}
}