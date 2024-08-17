using Metalogix.Actions;
using Metalogix.Jobs;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Metalogix.UI.CommandLine
{
	public class JobCommandLineHandler : ICommandLineHandler
	{
		private string _lastInfo = "";

		public string HelpText
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("  /job[:<jobDBFilePath>] - run job(s) from job DB (default or adjusted DB file) using its name, ID or row number");
				stringBuilder.AppendLine("  Parameters:");
				stringBuilder.AppendLine("    /all - run all jobs from given DB file");
				stringBuilder.AppendLine("    /id:\"<jobID>[,<jobID2>,<jobID3>...]\" - job ID(s) to run");
				stringBuilder.AppendLine("    /name:\"<jobName>[,<jobName2>,<jobName3>...]\" - job name(s) to run");
				stringBuilder.AppendLine("    /num:\"<jobNum>[,<jobNum2>,<jobNum3>...]\" - job num(s) to run");
				stringBuilder.AppendLine("  Example Usage:");
				stringBuilder.AppendFormat("    {0}/job:\"C:\\...\\JobHistory.lst\" /num:1", Path.GetFileName(Assembly.GetEntryAssembly().Location));
				stringBuilder.AppendFormat("    {0}/job /name:\"Copy test site\"", Path.GetFileName(Assembly.GetEntryAssembly().Location));
				return stringBuilder.ToString();
			}
		}

		public JobCommandLineHandler()
		{
		}

		public bool CanHandle(CommandLineParamsCollection parameters)
		{
			if (parameters == null)
			{
				return false;
			}
			return parameters.Contains("job");
		}

		public void Handle(CommandLineParamsCollection pars)
		{
			Console.WriteLine("Start running jobs ...");
			string str = (pars.HasValue("job") ? pars["job"] : JobsSettings.CurrentJobHistoryFile);
			if (!File.Exists(str))
			{
				throw new Exception(string.Format("Database file {0} not exists.", str));
			}
			if (pars.Contains("id") && string.IsNullOrEmpty(pars["id"]))
			{
				throw new Exception("Job IDs not set.");
			}
			if (pars.Contains("name") && string.IsNullOrEmpty(pars["name"]))
			{
				throw new Exception("Job Names not set.");
			}
			if (pars.Contains("num") && string.IsNullOrEmpty(pars["num"]))
			{
				throw new Exception("Job Nums not set.");
			}
			if (!pars.Contains("all") && !pars.Contains("id") && !pars.Contains("name") && !pars.Contains("num"))
			{
				throw new Exception("No jobs set to run.");
			}
			JobRunnerSandboxOptions jobRunnerSandboxOption = new JobRunnerSandboxOptions();
			if (pars.Contains("id"))
			{
				string item = pars["id"];
				char[] chrArray = new char[] { ',' };
				jobRunnerSandboxOption.JobIDs = item.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
			}
			if (pars.Contains("name"))
			{
				string item1 = pars["name"];
				char[] chrArray1 = new char[] { ',' };
				jobRunnerSandboxOption.JobNames = item1.Split(chrArray1, StringSplitOptions.RemoveEmptyEntries);
			}
			if (pars.Contains("num"))
			{
				string str1 = pars["num"];
				char[] chrArray2 = new char[] { ',' };
				string[] strArrays = str1.Split(chrArray2, StringSplitOptions.RemoveEmptyEntries);
				int[] num = new int[(int)strArrays.Length];
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					num[i] = Convert.ToInt32(strArrays[i]);
				}
				jobRunnerSandboxOption.JobNums = num;
			}
			if (pars.Contains("all"))
			{
				jobRunnerSandboxOption.RunAllJobs = true;
			}
			JobRunnerSandbox jobRunnerSandbox = new JobRunnerSandbox(str);
			jobRunnerSandbox.OnStateChanged += new JobCollection.ListChangedHandler(this.jr_OnStateChanged);
			jobRunnerSandbox.RunJob(jobRunnerSandboxOption);
		}

		private void jr_OnStateChanged(ChangeType changeType, Job[] itemsChanged)
		{
			Job[] jobArray = itemsChanged;
			for (int i = 0; i < (int)jobArray.Length; i++)
			{
				Job job = jobArray[i];
				if (job.Status != ActionStatus.NotRunning)
				{
					string str = string.Concat(job.Title, ": ", job.Status.ToString());
					if (!string.IsNullOrEmpty(job.StatusMessage))
					{
						str = string.Concat(str, " (", job.StatusMessage, ")");
					}
					if (this._lastInfo != str)
					{
						this._lastInfo = str;
						Console.WriteLine(str);
					}
					else
					{
						break;
					}
				}
			}
		}
	}
}