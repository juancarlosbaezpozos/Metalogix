using Ionic.Zip;
using Metalogix.Core.Support;
using System;
using System.Collections.Generic;
using System.IO;

namespace Metalogix.UI.WinForms.Support
{
	public sealed class SupportPackageBuilder
	{
		private readonly List<IHasSupportInfo> _supportInfo = new List<IHasSupportInfo>();

		private readonly Dictionary<string, string> _files = new Dictionary<string, string>();

		public SupportPackageBuilder()
		{
		}

		public SupportPackageBuilder AddFile(string filePath, string folderPath)
		{
			this._files.Add(filePath, folderPath);
			return this;
		}

		public SupportPackageBuilder AddSupportInfo(IHasSupportInfo info)
		{
			this._supportInfo.Add(info);
			return this;
		}

		public string BuildPackage()
		{
			string tempFileName = Path.GetTempFileName();
			using (ZipFile zipFiles = new ZipFile())
			{
				string str = this.CreateTempSupportInfoFile();
				zipFiles.AddFile(str, "");
				foreach (KeyValuePair<string, string> _file in this._files)
				{
					if (!File.Exists(_file.Key))
					{
						continue;
					}
					zipFiles.AddFile(_file.Key, _file.Value);
				}
				try
				{
					zipFiles.Save(tempFileName);
				}
				finally
				{
					if (File.Exists(str))
					{
						File.Delete(str);
					}
				}
			}
			return tempFileName;
		}

		private string CreateTempSupportInfoFile()
		{
			string str = Path.Combine(Path.GetTempPath(), string.Concat(Guid.NewGuid(), ".txt"));
			using (StreamWriter streamWriter = new StreamWriter(str))
			{
				foreach (IHasSupportInfo hasSupportInfo in this._supportInfo)
				{
					hasSupportInfo.WriteSupportInfo(streamWriter);
				}
			}
			return str;
		}
	}
}