using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Actions.Remoting.Database;
using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Jobs.Actions;
using Metalogix.UI.WinForms.Properties;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.RunJobsRemotely16.png")]
	[MenuText("1:Run selected job remotely {0-Jobs}")]
	[MenuTextPlural("1:Run selected jobs remotely {0-Jobs} ", PluralCondition.MultipleTargets)]
	public class RunSelectedJobsRemotely : GeneratePowerShellScript
	{
		public RunSelectedJobsRemotely()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase) && this.AppliesTo(sourceSelections, targetSelections))
			{
				return true;
			}
			return false;
		}

		private string ExportJobToPsScript(IXMLAbleList source, Job job, X509Certificate2 certificate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			try
			{
				if (!Directory.Exists(Utils.MetalogixTempPath))
				{
					Metalogix.Actions.Remoting.FileUtils.CreateDirectory(Utils.MetalogixTempPath, null, null);
				}
				string str = string.Concat(Utils.MetalogixTempPath, "\\", job.JobID, (PowerShellUtils.IsPowerShellInstalled ? ".ps1" : ".txt"));
				if (!string.IsNullOrEmpty(str))
				{
					IXMLAbleList commonSerializableList = new CommonSerializableList<Job>(new Job[] { job });
					Cryptography.ProtectionScope protectionScope = (certificate == null ? Cryptography.ProtectionScope.CurrentUser : Cryptography.ProtectionScope.Certificate);
					base.CreatePowerShellScript(source, commonSerializableList, protectionScope, str, false, true, certificate, true);
					if (File.Exists(str))
					{
						return str;
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException(string.Format("Failed to export the job '{0}'.", job.Title), exception);
			}
			return null;
		}

		private Job[] GetNewJobsList(Job[] selectedJobs)
		{
			string str;
			Job[] array;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder1 = new StringBuilder();
			List<Job> jobs = new List<Job>();
			List<Job> jobs1 = RemoteJobScheduler.Instance.Jobs;
			int num = 0;
			try
			{
				Job[] jobArray = selectedJobs;
				for (int i = 0; i < (int)jobArray.Length; i++)
				{
					Job job = jobArray[i];
					try
					{
						Job job1 = job.Parent.GetJob(job.JobID);
						if (job1.Status == ActionStatus.Running || job1.Status == ActionStatus.Queued)
						{
							if (num <= 10)
							{
								stringBuilder.AppendLine(string.Format("{0} (JobId: {1})", job.Title, job.JobID));
							}
							num++;
						}
						else
						{
							Job job2 = jobs1.Find((Job tempJob) => tempJob.JobID == job.JobID);
							if (job2 != null)
							{
								jobs1.Remove(job2);
							}
							jobs.Add(job);
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						object[] title = new object[] { job.Title, job.JobID, Environment.NewLine, exception.Message };
						stringBuilder1.AppendLine(string.Format("Job: {0} - {1}{2}Exception: {3} ", title));
					}
				}
				if (!string.IsNullOrEmpty(stringBuilder.ToString()))
				{
					str = (num <= 10 ? string.Format("One or more of the currently selected jobs have already been queued. These duplicate queued jobs are listed below: \n\n{0}", stringBuilder) : string.Format("There are '{0}' of the currently select jobs that have already been queued. Some of these include: \n\n{1}", num, stringBuilder));
					FlatXtraMessageBox.Show(str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				array = jobs.ToArray();
			}
			finally
			{
				if (!string.IsNullOrEmpty(stringBuilder1.ToString()))
				{
					string str1 = string.Format("An error occurred while checking existing jobs: {0}{1}", Environment.NewLine, stringBuilder1);
					GlobalServices.ErrorHandler.HandleException("Check existing jobs", str1, ErrorIcon.Error);
				}
			}
			return array;
		}

		private bool IsAgentsFound()
		{
			IAgentDb agentDb = new AgentDb(JobsSettings.AdapterContext.ToInsecureString());
			AgentCollection agentCollection = new AgentCollection(agentDb);
			agentCollection.FetchData();
			return agentCollection.GetList().Count > 0;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (!this.IsAgentsFound())
			{
				ActionPaletteControl.ActionClick(new ManageAgentsAction(), null, null);
				return;
			}
			if (FlatXtraMessageBox.Show(Resources.JobsConfirmRunMessage, Resources.JobsConfirmRunCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			this.RunJobsRemotely(source, target);
		}

		private void RunJobsRemotely(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				Job[] jobArray = JobUIHelper.GetJobArray(target);
				if (jobArray != null && (int)jobArray.Length > 0)
				{
					jobArray = this.GetNewJobsList(jobArray);
					if ((int)jobArray.Length > 0)
					{
						X509Certificate2 certificate = base.GetCertificate(null);
						Job[] jobArray1 = jobArray;
						for (int i = 0; i < (int)jobArray1.Length; i++)
						{
							Job job = jobArray1[i];
							try
							{
								RemoteJobScheduler.Instance.UpdateJobInfo(job.JobID, ActionStatus.Queued, null);
								if (!string.IsNullOrEmpty(this.ExportJobToPsScript(source, job, certificate)) && !RemoteJobScheduler.Instance.IsJobRunning)
								{
									RemoteJobScheduler.Instance.Run();
								}
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								string str = string.Format("An error occurred while running job '{0}' remotely.", job.Title);
								Logging.LogExceptionToTextFileWithEventLogBackup(exception, str, true);
							}
						}
						RemoteJobScheduler.Instance.ProcessJobQueue(this, null);
					}
					ActionPaletteControl.ActionClick(new ManageQueueAction(), source, null);
				}
			}
			catch (Exception exception2)
			{
				Logging.LogExceptionToTextFileWithEventLogBackup(exception2, "An error occurred while running selected jobs remotely.", true);
			}
		}

		protected override void RunOperation(object[] objectParams)
		{
			this.RunAction(objectParams[0] as IXMLAbleList, objectParams[1] as IXMLAbleList);
		}
	}
}