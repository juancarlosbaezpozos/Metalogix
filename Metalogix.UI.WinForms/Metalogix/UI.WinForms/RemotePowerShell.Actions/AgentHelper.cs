using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.RemotePowerShell.Actions
{
	public static class AgentHelper
	{
		private const string MetalogixSiteURl = "http://download.metalogix.net/Builds/";

		private const string CMInstallerName = "Metalogix Content Matrix Console Setup";

		public static string DeployCertificate(IRemoteWorker worker, string hostCertificateFilePath, string certificatePassword)
		{
			string empty = string.Empty;
			Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
			if (regex.IsMatch(certificatePassword))
			{
				empty = certificatePassword;
			}
			else
			{
				for (int i = 0; i < certificatePassword.Length; i++)
				{
					string str = certificatePassword.Substring(i, 1);
					if (!regex.IsMatch(str))
					{
						empty = string.Concat(empty, "^");
					}
					empty = string.Concat(empty, str);
				}
			}
			string fileName = Path.GetFileName(hostCertificateFilePath);
			worker.CopyFile(hostCertificateFilePath, string.Concat(Utils.MetalogixTempPath, fileName), true);
			string str1 = string.Format("certutil -user -p '{0}' -importPFX \"{1}\" NoRoot", empty, string.Concat(Utils.MetalogixTempPath, fileName));
			return worker.RunCommand(str1);
		}

		public static void DownloadInstaller(string completeUrl, string filePath)
		{
			(new WebClient()).DownloadFile(completeUrl, filePath);
		}

		public static string GetAssemblyVersion()
		{
			Version version = new Version(Application.ProductVersion);
			object[] major = new object[] { version.Major, version.Minor, null, null };
			major[2] = version.Build.ToString("D2");
			major[3] = version.Revision.ToString("D2");
			return string.Format("{0}_{1}_{2}{3}", major);
		}

		public static string GetInstallerUrl()
		{
			string assemblyVersion = AgentHelper.GetAssemblyVersion();
			string str = string.Concat("Metalogix Content Matrix Console Setup".Replace(" ", "_"), "_", assemblyVersion, ".exe");
			return Path.Combine("http://download.metalogix.net/Builds/", str);
		}

		public static bool IsExistingAgent(List<Agent> configuredAgents, string machineName)
		{
			if (configuredAgents != null && configuredAgents.Count > 0)
			{
				IEnumerable<Agent> agents = 
					from existingAgent in configuredAgents
					where existingAgent.MachineName.Equals(machineName, StringComparison.OrdinalIgnoreCase)
					select existingAgent;
				if (agents.Any<Agent>())
				{
					FlatXtraMessageBox.Show("Agent with same name already exists.", "Existing Agent", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return true;
				}
			}
			return false;
		}

		public static bool IsPasswordContainsQuotes(string password, string dialogName)
		{
			if (!password.Contains("\"") && !password.Contains("'"))
			{
				return true;
			}
			FlatXtraMessageBox.Show("Quotes are not allowed in the password.", dialogName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
	}
}