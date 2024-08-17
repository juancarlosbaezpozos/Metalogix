using Ionic.Zip;
using Metalogix.Core.OperationLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Nintex.Mappings
{
	public class NintexWorkflowMapper
	{
		private readonly string _nwpFile;

		private readonly List<string> _mappingFiles = new List<string>()
		{
			"Lists.xml",
			"Actions.xml"
		};

		private readonly OperationReporting _opReport;

		public IMapper[] Mappers
		{
			get;
			set;
		}

		public NintexWorkflowMapper(string nwpFile)
		{
			this._nwpFile = nwpFile;
			this._opReport = new OperationReporting();
		}

		public string Save(string outFile)
		{
			if (this.Mappers == null)
			{
				return null;
			}
			string fullPath = Path.GetFullPath(string.Format("TempOutput_{0}", Guid.NewGuid()));
			this._opReport.Start();
			try
			{
				NintexWorkflowMapper.UnzipPackage(this._nwpFile, fullPath);
				this.WalkFolder(fullPath);
				NintexWorkflowMapper.ZipPackage(fullPath, outFile);
			}
			finally
			{
				this._opReport.End();
				if (Directory.Exists(fullPath))
				{
					Directory.Delete(fullPath, true);
				}
			}
			return this._opReport.ResultXml;
		}

		private static void UnzipPackage(string zipFile, string outDir)
		{
			using (ZipFile zipFiles = new ZipFile(zipFile))
			{
				zipFiles.ExtractAll(outDir);
			}
		}

		private void WalkFiles(string folder)
		{
			string[] files = Directory.GetFiles(folder, "*.xml");
			for (int i = 0; i < (int)files.Length; i++)
			{
				string str = files[i];
				string str1 = str;
				char[] chrArray = new char[] { '\\' };
				string str2 = str1.Split(chrArray).Last<string>();
				if (this._mappingFiles.Contains(str2))
				{
					Array.ForEach<IMapper>(this.Mappers, (IMapper extractor) => extractor.UpdateFile(str, this._opReport));
				}
			}
		}

		private void WalkFolder(string folder)
		{
			this.WalkFiles(folder);
			Array.ForEach<string>(Directory.GetDirectories(folder), new Action<string>(this.WalkFolder));
		}

		private static void ZipPackage(string outDir, string outFile)
		{
			using (ZipFile zipFiles = new ZipFile())
			{
				zipFiles.AddDirectory(outDir, string.Empty);
				using (FileStream fileStream = File.Create(outFile))
				{
					zipFiles.Save(fileStream);
				}
			}
		}
	}
}