using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Interfaces;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(JobListControl))]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public abstract class GeneratePowerShellScript : Metalogix.Actions.Action
	{
		private const string FS_SNAPIN_DECLARATION = "if ( (Get-PSSnapin -Name {0} -ErrorAction SilentlyContinue) -eq $null ) {{ add-pssnapin {0} | out-null }}";

		protected GeneratePowerShellScript()
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
					if (((Job)enumerator.Current).Action.CmdletEnabled)
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

		protected void CreatePowerShellScript(IXMLAbleList source, IXMLAbleList target, Cryptography.ProtectionScope protectionScope = 0, string fullFilePath = null, bool showFile = true, bool skipSQLCEVerification = false, X509Certificate2 certificate = null, bool isRemoteJob = false)
		{
			CopyJobToClipboard copyJobToClipboard = new CopyJobToClipboard();
			base.SubActions.Add(copyJobToClipboard);
			object[] objArray = new object[] { source, target };
			copyJobToClipboard.RunAsSubAction(objArray, new ActionContext(source, target), null);
			try
			{
				Job[] jobArray = JobUIHelper.GetJobArray(target);
				JobCollection parent = jobArray[0].Parent;
				JobListControl jobListControl = this.GetJobListControl(source);
				if (skipSQLCEVerification || this.VerifySQLCECheckPassed())
				{
					Cursor.Current = Cursors.WaitCursor;
					string str = (string.IsNullOrEmpty(fullFilePath) ? this.GetTempPSFilePath() : fullFilePath);
					StreamWriter streamWriter = new StreamWriter(str, false, new UTF8Encoding(true));
					streamWriter.WriteLine(Utils.PowerShellVersionCheck);
					List<string> strs = new List<string>(new string[] { "Metalogix.System.Commands" });
					strs.AddRange((
						from j in (IEnumerable<Job>)jobArray
						select j.Action).SelectMany<Metalogix.Actions.Action, string>((Metalogix.Actions.Action a) => a.RequiredSnapins));
					foreach (string str1 in strs.Distinct<string>(StringComparer.CurrentCultureIgnoreCase))
					{
						streamWriter.WriteLine(string.Format("if ( (Get-PSSnapin -Name {0} -ErrorAction SilentlyContinue) -eq $null ) {{ add-pssnapin {0} | out-null }}", str1));
					}
					streamWriter.WriteLine();
					Job[] jobArray1 = jobArray;
					for (int i = 0; i < (int)jobArray1.Length; i++)
					{
						Job job = jobArray1[i];
						string empty = string.Empty;
						string sourceXml = job.SourceXml;
						string targetXml = job.TargetXml;
						switch (protectionScope)
						{
							case Cryptography.ProtectionScope.LocalMachine:
							{
								sourceXml = PowerShellUtils.ReEncryptAsMachineScope(sourceXml);
								targetXml = PowerShellUtils.ReEncryptAsMachineScope(targetXml);
								break;
							}
							case Cryptography.ProtectionScope.Certificate:
							{
								if (certificate == null)
								{
									certificate = this.GetCertificate(jobListControl);
								}
								if (certificate == null)
								{
									break;
								}
								sourceXml = PowerShellUtils.ReEncryptAsCertificateScope(sourceXml, certificate);
								targetXml = PowerShellUtils.ReEncryptAsCertificateScope(targetXml, certificate);
								break;
							}
						}
						if (isRemoteJob)
						{
							empty = job.JobID;
						}
						string powershellCommand = job.Action.GetPowershellCommand(sourceXml, targetXml, this.GetJobAdapterPowerShellString(parent.JobHistoryDb, protectionScope, certificate), empty);
						streamWriter.Write(powershellCommand);
					}
					streamWriter.Close();
					Cursor.Current = Cursors.Default;
					if (showFile)
					{
						Process process = new Process();
						process.StartInfo.FileName = str;
						process.StartInfo.UseShellExecute = true;
						process.Start();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Cursor.Current = Cursors.Default;
				GlobalServices.ErrorHandler.HandleException(exception.Message, exception);
			}
		}

		protected X509Certificate2 GetCertificate(JobListControl jobList = null)
		{
			X509Certificate2 item;
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			try
			{
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection x509Certificate2Collection = null;
				x509Certificate2Collection = (jobList != null ? X509Certificate2UI.SelectFromCollection(x509Store.Certificates, Resources.AvailableCerts, Resources.SelectCert, X509SelectionFlag.SingleSelection, jobList.Handle) : X509Certificate2UI.SelectFromCollection(x509Store.Certificates, Resources.AvailableCerts, Resources.SelectCert, X509SelectionFlag.SingleSelection));
				if (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0)
				{
					FlatXtraMessageBox.Show(Resources.NoValidCertSelected, Resources.InvalidCert, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return null;
				}
				else
				{
					item = x509Certificate2Collection[0];
				}
			}
			finally
			{
				x509Store.Close();
			}
			return item;
		}

		private string GetJobAdapterPowerShellString(IJobHistoryAdapter adapter, Cryptography.ProtectionScope protectionScope, X509Certificate2 certificate)
		{
			string adapterType = adapter.AdapterType;
			string str = adapterType;
			if (adapterType != null)
			{
				if (str == "SqlServer" || str == "Agent")
				{
					return string.Concat(" -jobdatabase \"", PowerShellUtils.GetSqlServerAdapterPowerShellString(adapter.AdapterContext, protectionScope, certificate), "\"");
				}
				if (str == "SqlCe")
				{
					return string.Concat(" -jobfile \"", adapter.AdapterContext, "\"");
				}
			}
			throw new ArgumentOutOfRangeException(string.Concat("'", adapter.AdapterType, "' is not supported. Please use 'SqlServer' or 'SqlCe'."));
		}

		private JobListControl GetJobListControl(IXMLAbleList source)
		{
			JobListControl jobListControl = JobUIHelper.GetJobListControl(source);
			if (jobListControl == null)
			{
				throw new Exception("Failed to locate job list.");
			}
			return jobListControl;
		}

		protected string GetTempPSFilePath()
		{
			string tempPath = Path.GetTempPath();
			if (tempPath[tempPath.Length - 1] != '\\')
			{
				tempPath = string.Concat(tempPath, "\\");
			}
			tempPath = string.Concat(tempPath, Guid.NewGuid().ToString(), (PowerShellUtils.IsPowerShellInstalled ? ".ps1" : ".txt"));
			return tempPath;
		}

		protected bool VerifySQLCECheckPassed()
		{
			string str = "Using Schedule Task or Generate PowerShell Script on a machine with Metalogix Organizer requires a full install SQL Compact Edition 4.0 SP1. You can find a KB article explaining how to do this at the link below. This issue will be resolved in a future release.";
			string str1 = "http://metalogix-kbase.force.com/pkb/articles/Content_Matrix/SQL-CE-4-SP1-is-required-in-the-GAC-for-Content-Matrix-7?q=SQL+CE&l=en_US&fs=Search&pn=1";
			if (!typeof(Metalogix.Actions.Action).Assembly.GlobalAssemblyCache)
			{
				return true;
			}
			return VerifyUserActionDialog.GetUserVerification(UIConfigurationVariables.ShowPowerShellWarning, str, "Information", str1, "Click here for more information", MessageBoxButtons.OKCancel);
		}
	}
}