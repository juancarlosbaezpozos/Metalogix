using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Licensing;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.One)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Agent))]
	public class AddUpdateAgentBaseAction<T> : Metalogix.Actions.Action<T>
	{
		private static readonly string MetalogixLicenseFileName;

		private static readonly object _downLoadLock;

		protected Agent agentInfo;

		static AddUpdateAgentBaseAction()
		{
			AddUpdateAgentBaseAction<T>.MetalogixLicenseFileName = "ProductNew.lic";
			AddUpdateAgentBaseAction<T>._downLoadLock = new object();
		}

		public AddUpdateAgentBaseAction()
		{
		}

		public void ConfigureAgent(Agent agent, bool copyLicenseFile = false, bool isUpdateAgentAction = false)
		{
			AgentStatus agentStatu = AgentStatus.Error;
			try
			{
				try
				{
					this.agentInfo = agent;
					string installerUrl = AgentHelper.GetInstallerUrl();
					string fileName = Path.GetFileName(installerUrl);
					string str = this.DownloadFile(installerUrl, fileName);
					if (string.IsNullOrEmpty(str))
					{
						this.LogMessage(string.Format("Installer: {0} not found.", fileName));
					}
					else
					{
						IRemoteWorker remoteWorker = new RemoteWorker(this.agentInfo);
						string systemFolderPath = remoteWorker.GetSystemFolderPath(Environment.SpecialFolder.CommonApplicationData);
						if (string.IsNullOrEmpty(systemFolderPath))
						{
							this.LogMessage("Target Program Data folder path not found.");
						}
						else
						{
							bool flag = true;
							if (copyLicenseFile)
							{
								flag = this.CopyLicenseFile(remoteWorker, systemFolderPath);
							}
							if (flag && this.CopySetup(fileName, str, remoteWorker, systemFolderPath))
							{
								if (!this.InstallSetup(remoteWorker, str))
								{
									agentStatu = AgentStatus.Error;
									this.LogMessage("Installation failed.");
								}
								else
								{
									agent.Parent.UpdateCMVersion(agent, Application.ProductVersion);
									if (!isUpdateAgentAction)
									{
										this.UpdatingOSVerion(agent, remoteWorker);
									}
									if (this.CopyMappingFiles(remoteWorker, null))
									{
										agentStatu = AgentStatus.Available;
										this.LogMessage("Configuration completed.");
									}
								}
							}
						}
					}
					this.agentInfo.Parent.UpdateStatus(this.agentInfo, agentStatu);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.LogMessage(exception, string.Format("An error occurred while configuring agent : {0}", this.agentInfo.MachineName));
					this.agentInfo.Parent.UpdateStatus(this.agentInfo, agentStatu);
				}
			}
			finally
			{
				this.agentInfo.Parent.UpdateLatestLogOnUI(this.agentInfo);
			}
		}

		protected bool CopyLicenseFile(IRemoteWorker worker, string targetFolderPath)
		{
			bool flag;
			try
			{
				this.LogMessage("License File copy started.");
				if (!worker.CopyFile(Path.Combine(ApplicationData.CommonDataPath, AddUpdateAgentBaseAction<T>.MetalogixLicenseFileName), Path.Combine(targetFolderPath, AddUpdateAgentBaseAction<T>.MetalogixLicenseFileName), true))
				{
					this.LogMessage("License File not copied.");
					flag = false;
				}
				else
				{
					this.LogMessage("License File copy completed.");
					flag = true;
				}
			}
			catch (Exception exception)
			{
				this.LogMessage(exception, string.Format("An error occurred while copying license file at path : {0}", targetFolderPath));
				flag = false;
			}
			return flag;
		}

		protected bool CopyMappingFiles(IRemoteWorker worker, Agent agent = null)
		{
			if (agent != null)
			{
				this.agentInfo = agent;
			}
			this.LogMessage("Application Mappings files copying started.");
			bool flag = worker.CopyApplicationMappingFiles(worker.GetSystemFolderPath(Environment.SpecialFolder.ApplicationData));
			this.LogMessage((flag ? "Application Mappings files copying completed." : "Application Mappings files did not get copied."));
			return flag;
		}

		private bool CopySetup(string installerName, string filePath, IRemoteWorker worker, string targetFolderPath)
		{
			bool flag = worker.CopyFile(filePath, Path.Combine(targetFolderPath, installerName), false);
			LogHelper.LogMessage(this.agentInfo, (flag ? "Installer copied successfully." : "Installer cannot be copied."));
			return flag;
		}

		private string DownloadFile(string completeUrl, string installerName)
		{
			string str;
			try
			{
				this.LogMessage("Checking for installer.");
				lock (AddUpdateAgentBaseAction<T>._downLoadLock)
				{
					string str1 = Path.Combine(ApplicationData.CommonDataPath, installerName);
					if (!File.Exists(str1))
					{
						this.LogMessage("Installer download started.");
						AgentHelper.DownloadInstaller(completeUrl, str1);
						this.LogMessage("Installer download completed.");
						str = str1;
					}
					else
					{
						this.LogMessage("Installer found.");
						str = str1;
					}
				}
			}
			catch (WebException webException1)
			{
				WebException webException = webException1;
				string str2 = string.Format("An error occurred while downloading installer from '{0}' due to internet connectivity. \r\nPlease download it manually and copy it to '{1}'", completeUrl, ApplicationData.CommonDataPath);
				this.LogMessage(webException, str2);
				return string.Empty;
			}
			catch (Exception exception)
			{
				this.LogMessage(exception, string.Format("An error occurred while downloading installer '{0}' from Metalogix site.", installerName));
				return string.Empty;
			}
			return str;
		}

		private bool InstallSetup(IRemoteWorker worker, string filePath)
		{
			LogHelper.LogMessage(this.agentInfo, "Installation started.");
			if (!Utils.IsDotNetFramework452(worker))
			{
				this.LogMessage("Microsoft .NET Framework 4.5.2 needs to be installed for this installation to continue.");
				return false;
			}
			bool flag = worker.InstallSetup(filePath);
			if (flag)
			{
				this.LogMessage("Installation completed.");
			}
			return flag;
		}

		private void LogMessage(string message)
		{
			LogHelper.LogMessage(this.agentInfo, message);
		}

		private void LogMessage(Exception ex, string message)
		{
			LogHelper.LogMessage(this.agentInfo, ex, message);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}

		private void UpdatingOSVerion(Agent agent, IRemoteWorker worker)
		{
			this.LogMessage("OS Version retrieval started.");
			string oSVersion = worker.GetOSVersion();
			if (!string.IsNullOrEmpty(oSVersion))
			{
				this.LogMessage("OS Version retrieval completed.");
				agent.Parent.UpdateOSVersion(agent, oSVersion);
			}
		}
	}
}