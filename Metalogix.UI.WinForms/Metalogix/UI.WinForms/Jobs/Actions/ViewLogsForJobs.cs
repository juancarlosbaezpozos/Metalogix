using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.ViewLogs16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("1:View log for selected job {3-Jobs}")]
	[MenuTextPlural("1:View aggregated log for selected jobs {3-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(JobListControl))]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class ViewLogsForJobs : Metalogix.Actions.Action
	{
		public ViewLogsForJobs()
		{
		}

		private LogItemViewer GetLogItemViewer(IXMLAbleList source)
		{
			JobListControl jobListControl = JobUIHelper.GetJobListControl(source);
			if (jobListControl == null)
			{
				throw new Exception("Could not display logs. Failed to locate job list.");
			}
			return jobListControl.LogItemViewer;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				LogItemViewer logItemViewer = this.GetLogItemViewer(source);
				Job[] jobArray = JobUIHelper.GetJobArray(target);
				JobCollection parent = jobArray[0].Parent;
				if ((int)jobArray.Length <= 1)
				{
					logItemViewer.DataSource = jobArray[0].Log;
				}
				else
				{
					if (JobHelper.ContainsRunningActions(jobArray))
					{
						throw new ConditionalDetailException("Cannot view aggregate log while actions are being run");
					}
					logItemViewer.DataSource = parent.GetRelatedLogItems(jobArray);
				}
				Cursor.Current = Cursors.Default;
				if (logItemViewer.Visible)
				{
					logItemViewer.Focus();
				}
				else
				{
					logItemViewer.Show();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Cursor.Current = Cursors.Default;
				GlobalServices.ErrorHandler.HandleException(exception.Message, exception);
			}
		}
	}
}