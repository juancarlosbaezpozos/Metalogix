using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	public class SharePointDriveDynamicParameters
	{
		private string m_sUser;

		private string m_sPassword;

		private string m_sType;

		private string m_sAdapterType;

		[Parameter(Mandatory=false, Position=4, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The short name of the adapter type you want to use for the connection. Use OM for a local SharePoint, WS for a remote SharePoint where our extension service is installed, or NWS to use the native SharePoint web services.")]
		public string AdapterType
		{
			get
			{
				return this.m_sAdapterType;
			}
			set
			{
				this.m_sAdapterType = value;
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

		[Parameter(Mandatory=false, Position=3, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The type of SharePoint connection to make (Site or Server).")]
		public string Type
		{
			get
			{
				return this.m_sType;
			}
			set
			{
				this.m_sType = value;
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

		public SharePointDriveDynamicParameters()
		{
		}
	}
}