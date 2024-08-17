using Metalogix.Core.Support;
using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Support
{
	public class InstalledProductInfo : IHasSupportInfo
	{
		private const string Win32ProductsRegPath = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		private const string Win64ProductsRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		private string DisplayName
		{
			get;
			set;
		}

		private string Label
		{
			get;
			set;
		}

		public InstalledProductInfo(string displayName, string label)
		{
			this.DisplayName = displayName;
			this.Label = label;
		}

		private void WriteInstalledProduct(TextWriter output, string regPath, string criteria, string label)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regPath))
			{
				if (registryKey != null)
				{
					string[] subKeyNames = registryKey.GetSubKeyNames();
					for (int i = 0; i < (int)subKeyNames.Length; i++)
					{
						using (RegistryKey registryKey1 = registryKey.OpenSubKey(subKeyNames[i]))
						{
							object value = registryKey1.GetValue("DisplayName");
							if (value != null)
							{
								if (value.ToString().IndexOf(criteria, StringComparison.OrdinalIgnoreCase) != -1)
								{
									object obj = registryKey1.GetValue("DisplayVersion");
									output.WriteLine("{2}: {0} {1}", value, (obj != null ? obj.ToString() : ""), label);
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
				this.WriteInstalledProduct(output, "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall", this.DisplayName, this.Label);
				this.WriteInstalledProduct(output, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", this.DisplayName, this.Label);
			}
			catch (Exception exception)
			{
				output.WriteLine("Unable to retrieve the list of installed products for {1}. (Exception: {0})", exception.Message, this.DisplayName);
				output.WriteLine("Please make sure you have rights to access the Windows registry.");
			}
		}
	}
}