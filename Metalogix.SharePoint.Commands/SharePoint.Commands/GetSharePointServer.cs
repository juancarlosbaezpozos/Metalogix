using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "MLSharePointServer")]
	public class GetSharePointServer : AssemblyBindingCmdlet
	{
		private string m_sUrl;

		private string m_sUser;

		private string m_sPassword;

		private bool m_bReadOnly;

		private bool _savePassword;

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

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates that the connection made to the SharePoint site should be read only.")]
		public SwitchParameter ReadOnly
		{
			get
			{
				return this.m_bReadOnly;
			}
			set
			{
				this.m_bReadOnly = value;
			}
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates whether the password should be remembered.")]
		public SwitchParameter SavePassword
		{
			get
			{
				return this._savePassword;
			}
			set
			{
				this._savePassword = value;
			}
		}

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The URL of the SharePoint server to connect to.")]
		public string ServerURL
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

		public GetSharePointServer()
		{
		}

		protected SPServer GetServerFromParameters()
		{
			Credentials credential = (this.User == null ? new Credentials() : new Credentials(this.User, this.Password.ToSecureString(), this.SavePassword));
			SPServer sPServer = new SPServer(CommandLineParsingUtils.GetAdapterFromShortname("OM"));
			sPServer.Adapter.Credentials = credential;
			sPServer.Adapter.Url = this.ServerURL;
			sPServer.Adapter.IsReadOnlyAdapter = this.ReadOnly;
			try
			{
				sPServer.FetchChildren();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.ThrowTerminatingError(new ErrorRecord(exception, "Could not access server at the specified URL", ErrorCategory.InvalidArgument, this.ServerURL));
			}
			return sPServer;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetServerFromParameters());
		}
	}
}