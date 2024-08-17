using Metalogix.Jobs.Reporting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Threading;

namespace Metalogix.Jobs.Reporting.Commands
{
	[Serializable]
	public class ExcelReportProxy : MarshalByRefObject
	{
		private static volatile AppDomain _workerAppDomain;

		private readonly static object SyncRoot;

		public static AppDomain WorkerAppDomain
		{
			get
			{
				return ExcelReportProxy._workerAppDomain;
			}
		}

		static ExcelReportProxy()
		{
			ExcelReportProxy.SyncRoot = new object();
		}

		public ExcelReportProxy()
		{
		}

		public void CreateFromSqlCeFile(string sqlCeDbFilePath, string outputDirectoryPath, string outputFileName, bool overwrite, IEnumerable<string> jobIds)
		{
			if (string.IsNullOrEmpty(outputFileName))
			{
				ExcelReport.CreateFromSqlCeFile(sqlCeDbFilePath, outputDirectoryPath, "JobHistory.xlsx", overwrite, jobIds, false);
				return;
			}
			ExcelReport.CreateFromSqlCeFile(sqlCeDbFilePath, outputDirectoryPath, outputFileName, overwrite, jobIds, false);
		}

		public void CreateFromSqlServer(string serverName, string databaseName, SqlAuthenticationType sqlAuthenticationType, string outputDirectoryPath, string outputFileName, bool overwrite, IEnumerable<string> jobIds, string username, string password)
		{
			if (string.IsNullOrEmpty(outputFileName))
			{
				ExcelReport.CreateFromSqlServer(serverName, databaseName, sqlAuthenticationType, outputDirectoryPath, "JobHistory.xlsx", overwrite, jobIds, username, password, false);
				return;
			}
			ExcelReport.CreateFromSqlServer(serverName, databaseName, sqlAuthenticationType, outputDirectoryPath, outputFileName, overwrite, jobIds, username, password, false);
		}

		public static ExcelReportProxy CreateInWorkerAppDomain(string assemblyPath)
		{
			ExcelReportProxy excelReportProxy;
			if (ExcelReportProxy._workerAppDomain != null)
			{
				return (ExcelReportProxy)ExcelReportProxy.WorkerAppDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ExcelReportProxy).ToString());
			}
			lock (ExcelReportProxy.SyncRoot)
			{
				if (ExcelReportProxy._workerAppDomain == null)
				{
					AppDomainSetup appDomainSetup = new AppDomainSetup()
					{
						ApplicationBase = assemblyPath
					};
					Evidence evidences = new Evidence();
					evidences.AddHost(new Zone(SecurityZone.MyComputer));
					ExcelReportProxy._workerAppDomain = AppDomain.CreateDomain("ExportToExcelDomain", evidences, appDomainSetup);
					return (ExcelReportProxy)ExcelReportProxy.WorkerAppDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ExcelReportProxy).ToString());
				}
				else
				{
					excelReportProxy = (ExcelReportProxy)ExcelReportProxy.WorkerAppDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ExcelReportProxy).ToString());
				}
			}
			return excelReportProxy;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public static void UnloadWorkerAppDomain()
		{
			if (ExcelReportProxy._workerAppDomain == null || ExcelReportProxy._workerAppDomain.IsFinalizingForUnload())
			{
				return;
			}
			lock (ExcelReportProxy.SyncRoot)
			{
				bool flag = false;
				if (ExcelReportProxy._workerAppDomain != null && !ExcelReportProxy._workerAppDomain.IsFinalizingForUnload())
				{
					try
					{
						AppDomain.Unload(ExcelReportProxy._workerAppDomain);
					}
					catch (CannotUnloadAppDomainException cannotUnloadAppDomainException)
					{
						flag = true;
					}
					while (flag)
					{
						Thread.Sleep(1000);
						try
						{
							AppDomain.Unload(ExcelReportProxy._workerAppDomain);
							flag = false;
						}
						catch (CannotUnloadAppDomainException cannotUnloadAppDomainException1)
						{
							Console.WriteLine("Could not unload worker Application Domain. Retrying...");
						}
					}
				}
			}
		}
	}
}