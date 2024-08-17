using Metalogix.Commands;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	public class MetalogixBackupDriveInfo : NodeDriveInfo
	{
		private string m_sDatabaseServer;

		private string m_sDatabaseName;

		public string DatabaseName
		{
			get
			{
				return this.m_sDatabaseName;
			}
			set
			{
				this.m_sDatabaseName = value;
			}
		}

		public string DatabaseServer
		{
			get
			{
				return this.m_sDatabaseServer;
			}
			set
			{
				this.m_sDatabaseServer = value;
			}
		}

		public MetalogixBackupDriveInfo(PSDriveInfo info) : base(info)
		{
		}
	}
}