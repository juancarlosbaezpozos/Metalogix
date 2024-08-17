using Metalogix.Core.Support;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using System;
using System.IO;

namespace Metalogix.UI.WinForms.Support
{
	public class ProductInfo : IHasSupportInfo
	{
		public ProductInfo()
		{
		}

		public void WriteSupportInfo(TextWriter output)
		{
			output.WriteLine("Product Name: {0}", UIApplication.INSTANCE.ProductName);
			output.WriteLine("Product Version: {0}", UIApplication.INSTANCE.ProductVersion);
			output.WriteLine("Product License:");
			string[] licenseInfo = UIApplication.INSTANCE.GetLicense().GetLicenseInfo();
			for (int i = 0; i < (int)licenseInfo.Length; i++)
			{
				output.WriteLine("\t{0}", licenseInfo[i]);
			}
		}
	}
}