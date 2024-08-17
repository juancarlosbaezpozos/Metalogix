using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.Jobs.Actions;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs
{
	public static class JobUIHelper
	{
		private static void FireJobLaunching(Job[] jobs)
		{
			if (JobUIHelper.JobsLaunching != null)
			{
				JobUIHelper.JobsLaunching(jobs);
			}
		}

		internal static Job[] GetJobArray(IXMLAbleList jobs)
		{
			Job[] item = new Job[jobs.Count];
			for (int i = 0; i < (int)item.Length; i++)
			{
				item[i] = (Job)jobs[i];
			}
			return item;
		}

		internal static JobListControl GetJobListControl(IXMLAbleList list)
		{
			JobListControl i;
			if (list == null || list.Count == 0)
			{
				return null;
			}
			Control item = list[0] as Control;
			for (i = item as JobListControl; item != null && i == null; i = item as JobListControl)
			{
				item = item.Parent;
			}
			return i;
		}

		private static JobStatusDialog GetStatusDialog(Job job, Form owner)
		{
			JobStatusDialog jobStatusDialog = new JobStatusDialog(job)
			{
				StartPosition = FormStartPosition.Manual
			};
			if (owner != null)
			{
				jobStatusDialog.Owner = owner;
				Point location = jobStatusDialog.Owner.Location;
				int x = location.X + (jobStatusDialog.Owner.Width - jobStatusDialog.Width) / 2;
				Point point = jobStatusDialog.Owner.Location;
				jobStatusDialog.Location = new Point(x, point.Y + (jobStatusDialog.Owner.Height - jobStatusDialog.Height) / 2);
			}
			return jobStatusDialog;
		}

		private static void LaunchMultipleJobs(IEnumerable<Job> jobs, Form owner)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				foreach (Job job in jobs)
				{
					if (job.Action != null)
					{
						continue;
					}
					throw new ConditionalDetailException("No action was associated with the given job");
				}
				foreach (Job job1 in jobs)
				{
					job1.Clear();
				}
				Job[] array = jobs.ToArray<Job>();
				JobRunner jobRunner = new JobRunner(array);
				Job job2 = new Job(jobRunner, null, null);
				Cursor.Current = Cursors.Default;
				JobUIHelper.FireJobLaunching(array);
				Cursor.Current = Cursors.WaitCursor;
				JobUIHelper.GetStatusDialog(job2, owner).Show();
				jobRunner.RunAsync(job2.SourceList, job2.TargetList);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private static void LaunchSingleJob(Job job, IXMLAbleList source, IXMLAbleList target, Form owner)
		{
			JobUIHelper.FireJobLaunching(new Job[] { job });
			if (job.Action.ShowStatusDialog)
			{
				JobUIHelper.GetStatusDialog(job, owner).Show();
			}
			job.Action.RunAsync(source, target);
		}

		public static void RunJob(Job job, Form owner = null, IXMLAbleList source = null, IXMLAbleList target = null)
		{
			Job[] jobArray = new Job[] { job };
			JobUIHelper.RunJobs(jobArray, owner, source, target);
		}

		public static void RunJobs(IEnumerable<Job> jobs, Form owner = null, IXMLAbleList source = null, IXMLAbleList target = null)
		{
			if (JobHelper.ContainsRunningActions(jobs))
			{
				FlatXtraMessageBox.Show("Cannot run an action that is already running.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (jobs.Count<Job>() != 1 || source == null || target == null)
			{
				JobUIHelper.LaunchMultipleJobs(jobs, owner);
				return;
			}
			JobUIHelper.LaunchSingleJob(jobs.First<Job>(), source, target, owner);
		}

		public static void SaveJob(Job job)
		{
			JobUIHelper.FireJobLaunching(new Job[] { job });
		}

		public static event JobUIHelper.JobsLaunchingHandler JobsLaunching;

		public delegate void JobsLaunchingHandler(Job[] jobs);
	}
}