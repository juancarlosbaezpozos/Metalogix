using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.DeleteJobs16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("4:Delete selected job {0-Jobs}")]
	[MenuTextPlural("4:Delete selected jobs {0-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.Delete)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class DeleteSelectedJobs : Metalogix.Actions.Action
	{
		public DeleteSelectedJobs()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (!JobHelper.ContainsRunningActions(targetSelections) && !JobHelper.ContainsPausedActions(targetSelections))
			{
				return true;
			}
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				foreach (Job job in target)
				{
					if (job.Status != ActionStatus.Running)
					{
						continue;
					}
					FlatXtraMessageBox.Show("Cannot delete jobs while they are running.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				if (FlatXtraMessageBox.Show("Do you wish to delete selected jobs? Note: any existing logs for these jobs will be cleared.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Cursor.Current = Cursors.WaitCursor;
					try
					{
						Job[] jobArray = JobUIHelper.GetJobArray(target);
						jobArray[0].Parent.DeleteJobs(jobArray);
						if (JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase))
						{
							Job[] jobArray1 = jobArray;
							for (int i = 0; i < (int)jobArray1.Length; i++)
							{
								Job job1 = jobArray1[i];
								RemoteJobScheduler.Instance.RemoveJob(job1);
							}
						}
					}
					finally
					{
						Cursor.Current = Cursors.Default;
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException("Error Deleting Jobs", string.Concat("Error deleting jobs: ", exception.Message), exception, ErrorIcon.Error);
			}
		}
	}
}