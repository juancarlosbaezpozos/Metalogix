using Metalogix.Core.Support;
using Microsoft.Win32;
using System;
using System.IO;

namespace Metalogix.UI.WinForms.Support
{
	public class InstalledPowerShellInfo : IHasSupportInfo
	{
		private const string Ps1RegPath = "SOFTWARE\\Microsoft\\PowerShell\\1";

		private const string Ps3RegPath = "SOFTWARE\\Microsoft\\PowerShell\\3";

		public InstalledPowerShellInfo()
		{
		}

		protected void WriteInstalledPowerShell(TextWriter output, string regPath)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regPath))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("Install");
					if (value != null)
					{
						if (value.ToString() == "1")
						{
							using (RegistryKey registryKey1 = registryKey.OpenSubKey("PowerShellEngine"))
							{
								if (registryKey1 != null)
								{
									object obj = registryKey1.GetValue("PowerShellVersion");
									if (obj != null)
									{
										output.WriteLine("Installed PowerShell Version: {0}", obj);
									}
									else
									{
										return;
									}
								}
								else
								{
									return;
								}
							}
						}
					}
				}
			}
		}

		public void WriteSupportInfo(TextWriter output)
		{
			try
			{
				this.WriteInstalledPowerShell(output, "SOFTWARE\\Microsoft\\PowerShell\\1");
				this.WriteInstalledPowerShell(output, "SOFTWARE\\Microsoft\\PowerShell\\3");
			}
			catch (Exception exception)
			{
				output.WriteLine("Unable to retrieve the PowerShell information. (Exception: {0})", exception.Message);
				output.WriteLine("Please make sure you have rights to access the Windows registry.");
			}
		}
	}
}