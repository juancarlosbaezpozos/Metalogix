using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Jobs;
using Metalogix.Jobs.Reporting.Actions;
using Metalogix.Jobs.Reporting.Options;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[ActionConfig(new Type[] { typeof(ExportJobsToExcel) })]
	public class ExportJobsToExcelConfig : IActionConfig
	{
		public ExportJobsToExcelConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			string initialCatalog;
			bool flag;
			string str;
			IXMLAbleList targets = context.ActionContext.Targets;
			Job[] jobArray = JobUIHelper.GetJobArray(targets);
			JobCollection parent = jobArray[0].Parent;
			JobHistoryDb jobHistoryDb = parent.JobHistoryDb;
			string adapterContext = jobHistoryDb.AdapterContext;
			string adapterType = jobHistoryDb.AdapterType;
			string str1 = adapterType;
			if (adapterType != null)
			{
				if (str1 == "SqlServer" || str1 == "Agent")
				{
					initialCatalog = (new SqlConnectionStringBuilder(adapterContext)).InitialCatalog;
					flag = true;
				}
				else
				{
					if (str1 != "SqlCe")
					{
						throw new ArgumentOutOfRangeException(string.Concat("'", jobHistoryDb.AdapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
					}
					initialCatalog = Path.GetFileNameWithoutExtension(adapterContext);
					flag = false;
				}
				string str2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Metalogix\\Reports");
				if (!Directory.Exists(str2))
				{
					Directory.CreateDirectory(str2);
				}
				if (targets.Count != 1)
				{
					str = ((int)jobArray.Length != parent.Count ? string.Concat(initialCatalog, "Multiple.xlsx") : string.Concat(initialCatalog, ".xlsx"));
				}
				else
				{
					str = string.Concat(initialCatalog, jobArray[0].JobID, ".xlsx");
				}
				SaveFileDialog saveFileDialog = new SaveFileDialog()
				{
					InitialDirectory = str2,
					Filter = "Excel Workbook (*.xlsx)|*.xlsx|All files (*.*)|*.*",
					FilterIndex = 1,
					FileName = str
				};
				SaveFileDialog saveFileDialog1 = saveFileDialog;
				if (saveFileDialog1.ShowDialog() != DialogResult.OK)
				{
					return ConfigurationResult.Cancel;
				}
				ExportJobsToExcelOption actionOptions = context.GetActionOptions<ExportJobsToExcelOption>();
				actionOptions.Directory = Path.GetDirectoryName(saveFileDialog1.FileName);
				actionOptions.FileName = Path.GetFileName(saveFileDialog1.FileName);
				actionOptions.IsSQLServer = flag;
				actionOptions.ConnectionString = adapterContext;
				CommonSerializableList<string> commonSerializableList = new CommonSerializableList<string>((
					from job in (IEnumerable<Job>)jobArray
					select job.JobID).ToArray<string>());
				actionOptions.JobIds = commonSerializableList;
				return ConfigurationResult.Run;
			}
			throw new ArgumentOutOfRangeException(string.Concat("'", jobHistoryDb.AdapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
		}
	}
}