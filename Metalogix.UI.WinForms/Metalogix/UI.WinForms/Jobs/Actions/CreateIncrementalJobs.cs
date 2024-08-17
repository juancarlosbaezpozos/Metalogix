using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("1:Create incremental job {2-Interact}")]
	[MenuTextPlural("1:Create incremental jobs {2-Interact}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class CreateIncrementalJobs : Metalogix.Actions.Action
	{
		public CreateIncrementalJobs()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (JobHelper.ContainsUnassociatedJobs(targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Job current = (Job)enumerator.Current;
					if (current.Action.Incrementable && current.Started.HasValue)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

	    public Job CreateIncrementalJob(Job sourceJob)
	    {
	        if (sourceJob == null)
	        {
	            return null;
	        }
	        Metalogix.Actions.Action action = sourceJob.Action.GenerateIncrementalAction(sourceJob.Started);
	        Job job = new Job(action, sourceJob.SourceList, sourceJob.TargetList);
	        job.Title = (action.IncrementalName ?? job.Title);
	        return job;
	    }


        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				try
				{
					Cursor.Current = Cursors.WaitCursor;
					JobCollection parent = ((Job)target[0]).Parent;
					List<Job> jobs = new List<Job>();
					foreach (Job job in target)
					{
						jobs.Add(this.CreateIncrementalJob(job));
					}
					foreach (Job job1 in jobs)
					{
						parent.Add(job1);
					}
					JobListControl jobListControl = JobUIHelper.GetJobListControl(source);
					if (jobListControl != null)
					{
						jobListControl.SelectedJobs = jobs.ToArray();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					string str = string.Format("Error Creating Incremental Job{0}: ", (target == null || target.Count <= 1 ? string.Empty : "s"));
					GlobalServices.ErrorHandler.HandleException(str, string.Concat(str, exception.Message), exception, ErrorIcon.Error);
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}