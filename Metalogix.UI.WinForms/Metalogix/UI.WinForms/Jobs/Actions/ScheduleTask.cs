using Metalogix;
using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Properties;
using Microsoft.Win32.TaskScheduler;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	public abstract class ScheduleTask : GeneratePowerShellScript
	{
		private const string MetalogiTaskFolder = "Metalogix";

		protected ScheduleTask()
		{
		}

		private bool CommitTask(TaskService ts, TaskDefinition td)
		{
			Cursor.Current = Cursors.WaitCursor;
			TaskSchedulerWizard taskSchedulerWizard = new TaskSchedulerWizard(ts, td, true)
			{
				AvailablePages = TaskSchedulerWizard.AvailableWizardPages.IntroPage | TaskSchedulerWizard.AvailableWizardPages.TriggerSelectPage | TaskSchedulerWizard.AvailableWizardPages.SummaryPage,
				AllowEditorOnFinish = false,
				TaskFolder = "Metalogix"
			};
			TaskSchedulerWizard taskSchedulerWizard1 = taskSchedulerWizard;
			Cursor.Current = Cursors.Default;
			if (taskSchedulerWizard1.ShowDialog() == DialogResult.OK)
			{
				return true;
			}
			ExecAction item = td.Actions[0] as ExecAction;
			if (item != null && File.Exists(item.Arguments))
			{
				File.Delete(item.Arguments);
			}
			return false;
		}

		private TaskDefinition CreateTaskDefinition(IXMLAbleList source, IXMLAbleList target, TaskService ts, Cryptography.ProtectionScope protectionScope)
		{
			string str;
			string fileName;
			string str1;
			Job[] jobArray = JobUIHelper.GetJobArray(target);
			if (!base.VerifySQLCECheckPassed())
			{
				return null;
			}
			if (protectionScope != Cryptography.ProtectionScope.CurrentUser)
			{
				str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Metalogix\\PowerShell");
				if (!Directory.Exists(str))
				{
					Directory.CreateDirectory(str);
				}
				if (protectionScope == Cryptography.ProtectionScope.LocalMachine)
				{
					string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					Guid guid = Guid.NewGuid();
					str1 = Path.Combine(folderPath, string.Concat("Metalogix\\PowerShell\\CurrentMachine", guid.ToString(), ".ps1"));
				}
				else
				{
					string folderPath1 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					Guid guid1 = Guid.NewGuid();
					str1 = Path.Combine(folderPath1, string.Concat("Metalogix\\PowerShell\\Certificate", guid1.ToString(), ".ps1"));
				}
				fileName = str1;
			}
			else
			{
				str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Metalogix\\PowerShell");
				if (!Directory.Exists(str))
				{
					Directory.CreateDirectory(str);
				}
				string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				Guid guid2 = Guid.NewGuid();
				fileName = Path.Combine(folderPath2, string.Concat("Metalogix\\PowerShell\\CurrentUser", guid2.ToString(), ".ps1"));
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				InitialDirectory = str,
				Filter = "PowerShell Scripts (*.ps1)|*.ps1|All files (*.*)|*.*",
				FilterIndex = 1,
				FileName = Path.GetFileName(fileName)
			};
			SaveFileDialog saveFileDialog1 = saveFileDialog;
			if (saveFileDialog1.ShowDialog() != DialogResult.OK)
			{
				return null;
			}
			fileName = saveFileDialog1.FileName;
			base.CreatePowerShellScript(source, target, protectionScope, fileName, false, true, null, false);
			TaskDefinition name = ts.NewTask();
			StringBuilder stringBuilder = new StringBuilder(512);
			if ((int)jobArray.Length > 0)
			{
				int num = 0;
				Job[] jobArray1 = jobArray;
				for (int i = 0; i < (int)jobArray1.Length; i++)
				{
					Job job = jobArray1[i];
					if (num > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append("From Job Name: ");
					stringBuilder.Append(job.Title);
					stringBuilder.AppendLine(" ");
					stringBuilder.Append("From JobID: ");
					stringBuilder.Append(job.JobID);
					stringBuilder.AppendLine(" ");
					stringBuilder.Append("Source: ");
					stringBuilder.Append(job.Source);
					stringBuilder.AppendLine(" ");
					stringBuilder.Append("Target: ");
					stringBuilder.Append(job.Target);
					stringBuilder.AppendLine(" ");
					num++;
				}
			}
			name.RegistrationInfo.Description = stringBuilder.ToString();
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			if (current != null)
			{
				name.RegistrationInfo.Author = current.Name;
			}
			name.RegistrationInfo.Date = DateTime.Now;
			name.Actions.Add(new ExecAction("PowerShell.exe", fileName, null));
			return name;
		}

		protected void ProcessTask(IXMLAbleList source, IXMLAbleList target, Cryptography.ProtectionScope protectionScope)
		{
			using (TaskService taskService = new TaskService())
			{
				if (!taskService.RootFolder.SubFolders.Any<TaskFolder>((TaskFolder tf) => tf.Name == "Metalogix"))
				{
					if (taskService.FindTask("Metalogix", true) == null)
					{
						taskService.RootFolder.CreateFolder("Metalogix", (string)null);
					}
					else
					{
						FlatXtraMessageBox.Show(Resources.JobListControl_ProcessTask_An_error_has_occurred_creating_Metalogix_folder_because_a_task_with_that_name_already_exists__Please_rename_or_remove_that_task_before_trying_again_, Resources.JobListControl_ProcessTask_Task_Schedule_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
				}
				TaskDefinition taskDefinition = this.CreateTaskDefinition(source, target, taskService, protectionScope);
				if (taskDefinition != null)
				{
					this.CommitTask(taskService, taskDefinition);
				}
			}
		}
	}
}