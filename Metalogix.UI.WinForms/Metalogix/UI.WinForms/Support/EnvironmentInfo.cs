using Metalogix.Core.Support;
using System;
using System.IO;

namespace Metalogix.UI.WinForms.Support
{
	public class EnvironmentInfo : IHasSupportInfo
	{
		public EnvironmentInfo()
		{
		}

		public void WriteSupportInfo(TextWriter output)
		{
			output.WriteLine(string.Concat("Client OS Version: ", Environment.OSVersion));
		}
	}
}