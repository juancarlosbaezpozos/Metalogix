using Metalogix.Commands;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "MLSharePointDatabase")]
	public class GetSharePointDatabase : AssemblyBindingCmdlet
	{
		private string m_sServer;

		private string m_sDatabase;

		private string m_sUser;

		private string m_sPassword;

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

		public GetSharePointDatabase()
		{
		}

		protected SPServer GetServerFromParameters()
		{
			Credentials credential = (this.User == null ? new Credentials() : new Credentials(this.User, this.Password.ToSecureString(), false));
			object[] server = new object[] { this.Server, this.Database, credential };
			SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(server);
			if (dBAdapter == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(null, "Could not get the database reader.", ErrorCategory.InvalidType, string.Concat(this.Server, ".", this.Database)));
			}
			return new SPServer(dBAdapter);
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetServerFromParameters());
		}
	}
}