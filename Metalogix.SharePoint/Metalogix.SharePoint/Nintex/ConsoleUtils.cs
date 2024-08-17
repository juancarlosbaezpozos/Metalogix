using Metalogix;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Metalogix.SharePoint.Nintex
{
	public static class ConsoleUtils
	{
		private static string _nintexWorkflowsExePath;

		private static string _nintexWorkflowsExeName;

		public static string NintexWorkflowsExePath
		{
			get
			{
				return ConsoleUtils.GetNintexWorkflowsExePath();
			}
		}

		static ConsoleUtils()
		{
			ConsoleUtils._nintexWorkflowsExeName = "NintexWorkflowService\\Metalogix.NintexWorkflowService.exe";
		}

		private static string GetLauncherLocation(string launcherName)
		{
			DirectoryInfo parent = (new DirectoryInfo(ApplicationData.MainAssembly.Location)).Parent;
			return Path.Combine(parent.FullName, launcherName);
		}

		private static string GetNintexWorkflowsExePath()
		{
			if (ConsoleUtils._nintexWorkflowsExePath == null)
			{
				try
				{
					ConsoleUtils._nintexWorkflowsExePath = ConsoleUtils.GetLauncherLocation(ConsoleUtils._nintexWorkflowsExeName);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					throw new FileNotFoundException(string.Format("Couldn't not find '{0}' in installed directory.", ConsoleUtils._nintexWorkflowsExeName), exception.InnerException);
				}
			}
			return ConsoleUtils._nintexWorkflowsExePath;
		}

		public static string LaunchConsole(string launcherPath, string arguments)
		{
			if (string.IsNullOrEmpty(arguments))
			{
				throw new ArgumentNullException("Valid arguments are not provided");
			}
			if (!File.Exists(launcherPath))
			{
				throw new FileNotFoundException(string.Format("File '{0}' does not exists or removed", launcherPath));
			}
			ProcessStartInfo processStartInfo = new ProcessStartInfo(launcherPath, arguments)
			{
				CreateNoWindow = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				WorkingDirectory = Path.GetDirectoryName(launcherPath),
				RedirectStandardOutput = true,
				StandardOutputEncoding = Encoding.UTF8
			};
			Process process = Process.Start(processStartInfo);
			process.WaitForExit();
			string end = process.StandardError.ReadToEnd();
			if (!string.IsNullOrEmpty(end))
			{
				throw new ConsoleUtilsLauncherException(arguments, end);
			}
			return process.StandardOutput.ReadToEnd();
		}

		public static string LaunchNintexWorkflows(string arguments)
		{
			return ConsoleUtils.LaunchConsole(ConsoleUtils.NintexWorkflowsExePath, arguments);
		}
	}
}