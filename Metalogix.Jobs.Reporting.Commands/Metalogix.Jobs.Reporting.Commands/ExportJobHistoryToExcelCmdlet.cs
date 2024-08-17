using Metalogix.Jobs.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs.Reporting.Commands
{
	[Cmdlet("Export", "JobHistory", DefaultParameterSetName="SQLCE")]
	public class ExportJobHistoryToExcelCmdlet : PSCmdlet
	{
		[Alias(new string[] { "A" })]
		[Parameter(ParameterSetName="SQL", HelpMessage="Indicates whether to use Integrated (Windows) or SQL Authentication. If not specified will Integrated Authentication will be used.")]
		public SqlAuthenticationType AuthType
		{
			get;
			set;
		}

		[Alias(new string[] { "D" })]
		[Parameter(Mandatory=true, ParameterSetName="SQL", HelpMessage="The name of the Job History database.")]
		public string DatabaseName
		{
			get;
			set;
		}

		[Alias(new string[] { "J" })]
		[Parameter(HelpMessage="Optional list of comma-delimiter Job IDs. If not provided all jobs will be exported.")]
		[Parameter(ParameterSetName="SQL")]
		[Parameter(ParameterSetName="SQLCE")]
		public IEnumerable<string> JobIds
		{
			get;
			set;
		}

		[Alias(new string[] { "O" })]
		[Parameter(ParameterSetName="SQL")]
		[Parameter(ParameterSetName="SQLCE")]
		[Parameter(ValueFromPipelineByPropertyName=true, HelpMessage="Directory where Excel Report should be created. If not specified file will be created in the current directory.")]
		public string OutputDirectoryPath
		{
			get;
			set;
		}

		[Alias(new string[] { "F" })]
		[Parameter(ParameterSetName="SQL")]
		[Parameter(ParameterSetName="SQLCE")]
		[Parameter(ValueFromPipelineByPropertyName=true, HelpMessage="Name of new Excel file. The file name must include the \".xlsx\" extension. If no name is provided the file will be named \"JobHistory.xlsx\".")]
		public string OutputFileName
		{
			get;
			set;
		}

		[Alias(new string[] { "W" })]
		[Parameter(ParameterSetName="SQL")]
		[Parameter(ParameterSetName="SQLCE")]
		[Parameter(ValueFromPipelineByPropertyName=true, HelpMessage="Indicates whether to overwrite an existing file with same name. If not specified, it will not overwrite.")]
		public bool Overwrite
		{
			get;
			set;
		}

		[Alias(new string[] { "P" })]
		[Parameter(ParameterSetName="SQL", HelpMessage="Specify the password for SQL Authentication.")]
		public string Password
		{
			get;
			set;
		}

		[Alias(new string[] { "N" })]
		[Parameter(Mandatory=true, ParameterSetName="SQL", HelpMessage="The name of the SQL Server.")]
		public string ServerName
		{
			get;
			set;
		}

		[Alias(new string[] { "I" })]
		[Parameter(Mandatory=true, ParameterSetName="SQLCE", HelpMessage="Specify the path to the SQLCE database file that contains the Job History data.")]
		public string SqlCeDbFilePath
		{
			get;
			set;
		}

		[Alias(new string[] { "U" })]
		[Parameter(ParameterSetName="SQL", HelpMessage="Specify the username for SQL Authentication.")]
		public string Username
		{
			get;
			set;
		}

		public ExportJobHistoryToExcelCmdlet()
		{
		}

	    ~ExportJobHistoryToExcelCmdlet()
	    {
	        ExcelReportProxy.UnloadWorkerAppDomain();
	    }

        protected override void ProcessRecord()
		{
			try
			{
				base.WriteVerbose(string.Concat("Export Started at ", DateTime.Now));
				string location = Assembly.GetAssembly(typeof(ExcelReport)).Location;
				ExcelReportProxy excelReportProxy = ExcelReportProxy.CreateInWorkerAppDomain(Path.GetDirectoryName(location));
				if (!string.IsNullOrEmpty(this.SqlCeDbFilePath))
				{
					excelReportProxy.CreateFromSqlCeFile(this.SqlCeDbFilePath, this.OutputDirectoryPath, this.OutputFileName, this.Overwrite, this.JobIds);
				}
				else if (!string.IsNullOrEmpty(this.ServerName) && !string.IsNullOrEmpty(this.DatabaseName))
				{
					excelReportProxy.CreateFromSqlServer(this.ServerName, this.DatabaseName, this.AuthType, this.OutputDirectoryPath, this.OutputFileName, this.Overwrite, this.JobIds, this.Username, this.Password);
				}
				base.WriteVerbose(string.Concat("Export Completed ", DateTime.Now));
			}
			catch (Exception exception)
			{
				base.ThrowTerminatingError(new ErrorRecord(exception, string.Empty, ErrorCategory.NotSpecified, this));
				throw;
			}
		}
	}
}