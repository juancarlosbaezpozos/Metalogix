using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Reporting;
using Metalogix.Jobs.Reporting.Options;
using Metalogix.Licensing;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Metalogix.Jobs.Reporting.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.Jobs.Reporting.Icons.ExportToExcel16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("6:Export selected job to Excel {0-Jobs}")]
	[MenuTextPlural("6:Export selected jobs to Excel {0-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Metalogix.Jobs.Job))]
	public class ExportJobsToExcel : Metalogix.Actions.Action<ExportJobsToExcelOption>
	{
		public ExportJobsToExcel()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.EnabledOn(sourceSelections, targetSelections))
			{
				return false;
			}
			return !JobHelper.ContainsRunningActions(targetSelections);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				if (!this.ActionOptions.IsSQLServer)
				{
					ExcelReport.CreateFromSqlCeFile(this.ActionOptions.ConnectionString, this.ActionOptions.Directory, this.ActionOptions.FileName, true, this.ActionOptions.JobIds, false);
				}
				else
				{
					ExcelReport.CreateFromSqlServer(this.ActionOptions.ConnectionString, this.ActionOptions.Directory, this.ActionOptions.FileName, true, this.ActionOptions.JobIds, false);
				}
				if (this.SupportsFile(Path.GetExtension(this.ActionOptions.FileName)))
				{
					using (Process process = new Process())
					{
						process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
						process.StartInfo.CreateNoWindow = true;
						process.StartInfo.RedirectStandardError = false;
						process.StartInfo.RedirectStandardOutput = false;
						process.StartInfo.RedirectStandardInput = false;
						process.StartInfo.UseShellExecute = true;
						process.StartInfo.FileName = Path.Combine(this.ActionOptions.Directory, this.ActionOptions.FileName);
						process.Start();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Cursor.Current = Cursors.Default;
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}

		private bool SupportsFile(string extension)
		{
			bool flag = false;
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension))
			{
				if (registryKey != null && !string.IsNullOrEmpty((string)registryKey.GetValue(null, "")))
				{
					flag = true;
				}
			}
			return flag;
		}
	}
}