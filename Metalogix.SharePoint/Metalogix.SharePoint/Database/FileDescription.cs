using System;

namespace Metalogix.SharePoint.Database
{
	public struct FileDescription
	{
		public string LogicalFilename;

		public string PhysicalFilename;

		public string FileType;

		public long? PhysicalFileSize;

		public FileDescription(string logicalName, string physicalName, long? iSize, string sFileType)
		{
			this.LogicalFilename = logicalName;
			this.PhysicalFilename = physicalName;
			this.PhysicalFileSize = iSize;
			this.FileType = sFileType;
		}
	}
}