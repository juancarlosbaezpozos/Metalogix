using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	public class SharePointBackupDriveDynamicParameters
	{
		private string m_sDatabaseServer;

		private string m_sUser;

		private string m_sPassword;

		private string[] m_sSupportingBackups;

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the database server to restore the backup into to allow browsing.")]
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

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The password for the user provided.")]
		public string Password
		{
			get
			{
				return this.m_sPassword;
			}
			set
			{
				this.m_sPassword = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="File names for any required supporting backups. For example, if the file specified as the root for the drive contains only a differential backup, this parameter should contain the name of the file containing the previous full backup.")]
		public string[] SupportingBackups
		{
			get
			{
				return this.m_sSupportingBackups;
			}
			set
			{
				this.m_sSupportingBackups = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A user to connect as, in DOMAIN\\Login format.")]
		public string User
		{
			get
			{
				return this.m_sUser;
			}
			set
			{
				this.m_sUser = value;
			}
		}

		public SharePointBackupDriveDynamicParameters()
		{
		}
	}
}