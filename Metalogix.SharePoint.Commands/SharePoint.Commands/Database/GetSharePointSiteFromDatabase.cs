using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "MLSharePointSiteFromDatabase")]
	public class GetSharePointSiteFromDatabase : AssemblyBindingCmdlet
	{
		private string m_sServer;

		private string m_sDatabase;

		private string m_sUrl;

		private string m_sUser;

		private string m_sPassword;

		private string m_sHostHeader;

		[Parameter(Mandatory=true, Position=1, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the SharePoint content database.")]
		public string Database
		{
			get
			{
				return this.m_sDatabase;
			}
			set
			{
				this.m_sDatabase = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A host header for a specified subsite. Use this if you are connecting to a site or subsite with a host header name.")]
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

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The name of the database server.")]
		public string Server
		{
			get
			{
				return this.m_sServer;
			}
			set
			{
				this.m_sServer = value;
			}
		}

		[Parameter(Mandatory=false, Position=2, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The server-relative URL of the SharePoint site to connect to.")]
		public string SiteURL
		{
			get
			{
				return this.m_sUrl;
			}
			set
			{
				this.m_sUrl = value;
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

		public GetSharePointSiteFromDatabase()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
		}

		protected override void EndProcessing()
		{
			base.EndProcessing();
		}

		protected SPWeb GetWebFromParameters()
		{
			string str;
			Credentials credential = (this.User == null ? new Credentials() : new Credentials(this.User, this.Password.ToSecureString(), false));
			object[] server = new object[] { this.Server, this.Database, credential };
			SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(server);
			if (dBAdapter == null)
			{
				string[] strArrays = new string[] { this.Server, ".", this.Database, "/", this.SiteURL };
				base.ThrowTerminatingError(new ErrorRecord(null, "Could not get the database reader.", ErrorCategory.InvalidType, string.Concat(strArrays)));
			}
			((IDBReader)dBAdapter).HostHeader = this.HostHeader;
			SharePointAdapter sharePointAdapter = dBAdapter;
			if (string.IsNullOrEmpty(this.SiteURL))
			{
				str = "/";
			}
			else
			{
				str = (this.SiteURL.StartsWith("/") ? this.SiteURL : string.Concat("/", this.SiteURL));
			}
			sharePointAdapter.Url = str;
			SPWeb sPWeb = new SPWeb(dBAdapter);
			try
			{
				sPWeb.FetchChildren();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string[] server1 = new string[] { this.Server, ".", this.Database, "/", this.SiteURL };
				base.ThrowTerminatingError(new ErrorRecord(exception, "Could not access web at the specified URL", ErrorCategory.InvalidArgument, string.Concat(server1)));
			}
			return sPWeb;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetWebFromParameters());
		}
	}
}