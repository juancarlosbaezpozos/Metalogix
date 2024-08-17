using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	public class SharePointDatabaseDriveDynamicParameters
	{
		private string m_sUser;

		private string m_sPassword;

		private string m_sHostHeader;

		[Parameter(Mandatory=false, Position=3, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A host header for a specified subsite. Use this if you are connecting to a site or subsite with a host header name.")]
		public string HostHeader
		{
			get
			{
				return this.m_sHostHeader;
			}
			set
			{
				this.m_sHostHeader = value;
			}
		}

		[Parameter(Mandatory=false, Position=2, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The password for the user provided.")]
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

		[Parameter(Mandatory=false, Position=1, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A user to connect as, in DOMAIN\\Login format.")]
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

		public SharePointDatabaseDriveDynamicParameters()
		{
		}
	}
}