using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Reporting;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Jobs.Actions;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.UI.WinForms.Support
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.contactus_sm.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Create Support Zip File {3-Support}")]
	[Name("CreateSupportPackage")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(JobListControl))]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Metalogix.Jobs.Job))]
	public class CreateSupportPackageAction : Metalogix.Actions.Action
	{
		private string _saveFile;

		public CreateSupportPackageAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (JobHelper.ContainsUnassociatedJobs(targetSelections))
			{
				return false;
			}
			return true;
		}

		public override ConfigurationResult Configure(ref IXMLAbleList source, ref IXMLAbleList target)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				CheckPathExists = true,
				AddExtension = true,
				Filter = "Zip file (*.zip)|*.zip|All files (*.*)|*.*",
				FileName = this.GetSupportPackageFileName(target)
			};
			SaveFileDialog saveFileDialog1 = saveFileDialog;
			if (saveFileDialog1.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			this._saveFile = saveFileDialog1.FileName;
			return ConfigurationResult.Run;
		}

		public string CreateSupportPackage(Metalogix.Jobs.Job job)
		{
			if (string.IsNullOrEmpty(this._saveFile))
			{
				throw new Exception("Action has not been configured.");
			}
			this.CreateSupportPackage(job, this._saveFile);
			return this._saveFile;
		}

		public void CreateSupportPackage(Metalogix.Jobs.Job job, string saveFile)
		{
			string str = "";
			string str1 = "";
			try
			{
				try
				{
					str = this.CreateTempJobXmlFile(job);
					str1 = this.CreateTempJobLogFile(job);
					SupportPackageBuilder supportPackageBuilder = (new SupportPackageBuilder()).AddSupportInfo(new ProductInfo()).AddSupportInfo(new EnvironmentInfo()).AddSupportInfo(new InstalledProductInfo("Metalogix", "Installed Metalogix Product")).AddSupportInfo(new InstalledProductInfo("SharePoint Server", "Installed SharePoint Product")).AddSupportInfo(new InstalledPowerShellInfo());
					JobInfo jobInfo = new JobInfo()
					{
						Job = job
					};
					string str2 = supportPackageBuilder.AddSupportInfo(jobInfo).AddFile(Path.Combine(ApplicationData.ApplicationPath, "ApplicationSettings.xml"), "settings").AddFile(Path.Combine(ApplicationData.CommonDataPath, "EnvironmentSettings.xml"), "settings").AddFile(Path.Combine(ApplicationData.CommonDataPath, "TenantSettings.xml"), "settings").AddFile(str, "job").AddFile(str1, "job").BuildPackage();
					File.Copy(str2, saveFile, true);
					File.Delete(str2);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					GlobalServices.ErrorHandler.HandleException(exception.Message, exception);
				}
			}
			finally
			{
				if (File.Exists(str))
				{
					File.Delete(str);
				}
				if (File.Exists(str1))
				{
					File.Delete(str1);
				}
			}
		}

		private string CreateTempJobLogFile(Metalogix.Jobs.Job job)
		{
			string str = Path.Combine(Path.GetTempPath(), string.Concat(Guid.NewGuid(), ".xlsx"));
			JobHistoryDb jobHistoryDb = job.Parent.JobHistoryDb;
			string adapterType = jobHistoryDb.AdapterType;
			string str1 = adapterType;
			if (adapterType != null)
			{
				if (str1 == "SqlServer" || str1 == "Agent")
				{
					string connectionString = (new SqlConnectionStringBuilder(jobHistoryDb.AdapterContext)).ConnectionString;
					string directoryName = Path.GetDirectoryName(str);
					string fileName = Path.GetFileName(str);
					string[] jobID = new string[] { job.JobID };
					ExcelReport.CreateFromSqlServer(connectionString, directoryName, fileName, true, jobID, false);
				}
				else
				{
					if (str1 != "SqlCe")
					{
						throw new ArgumentOutOfRangeException(string.Concat("'", jobHistoryDb.AdapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
					}
					string adapterContext = jobHistoryDb.AdapterContext;
					string directoryName1 = Path.GetDirectoryName(str);
					string fileName1 = Path.GetFileName(str);
					string[] strArrays = new string[] { job.JobID };
					ExcelReport.CreateFromSqlCeFile(adapterContext, directoryName1, fileName1, true, strArrays, false);
				}
				return str;
			}
			throw new ArgumentOutOfRangeException(string.Concat("'", jobHistoryDb.AdapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
		}

		private string CreateTempJobXmlFile(Metalogix.Jobs.Job job)
		{
			string str = Path.Combine(Path.GetTempPath(), string.Concat(job.JobID, ".xml"));
			using (StreamWriter streamWriter = new StreamWriter(str))
			{
				XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
				{
					Indent = true,
					NewLineHandling = NewLineHandling.Entitize
				};
				using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSetting))
				{
					job.ToXML(xmlWriter, false);
				}
			}
			return str;
		}

		private string GetSupportPackageFileName(IXMLAbleList target)
		{
			Metalogix.Jobs.Job job = JobUIHelper.GetJobArray(target).FirstOrDefault<Metalogix.Jobs.Job>();
			string title = job.Title;
			DateTime now = DateTime.Now;
			return string.Format("{0}_{1}", title, now.ToString("yyyyMMdd_HHmmss"));
		}

		public void PromptToOpen(string saveFile)
		{
			if (FlatXtraMessageBox.Show((new StringBuilder()).AppendLine("Please review the contents of the zip file for sensitive information before sending to support.").AppendLine().AppendLine().AppendLine("Would you like to open the file now?").ToString(), "Open Support Zip File", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
			{
				return;
			}
			Process.Start(saveFile);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			CopyJobToClipboard copyJobToClipboard = new CopyJobToClipboard();
			base.SubActions.Add(copyJobToClipboard);
			object[] objArray = new object[] { source, target };
			copyJobToClipboard.RunAsSubAction(objArray, new ActionContext(source, target), null);
			Metalogix.Jobs.Job job = JobUIHelper.GetJobArray(target).First<Metalogix.Jobs.Job>();
			this.CreateSupportPackage(job, this._saveFile);
			this.PromptToOpen(this._saveFile);
		}
	}
}