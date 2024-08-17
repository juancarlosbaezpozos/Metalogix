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
	[Cmdlet("Get", "MLSharePointSite")]
	public class GetSharePointSite : AssemblyBindingCmdlet
	{
		private string m_sUrl;

		private string m_sUser;

		private string m_sPassword;

		private string m_sAuthenticationType;

		private string m_sAdapterType;

		private bool m_bReadOnly;

		private bool _savePassword;

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The short name of the adapter type you want to use for the connection. Use OM for a local SharePoint, WS for a remote SharePoint where our extension service is installed, or NWS to use the native SharePoint web services.")]
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

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The user authentication type.")]
		public string AuthenticationType
		{
			get
			{
				return this.m_sAuthenticationType;
			}
			set
			{
				this.m_sAuthenticationType = value;
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

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The URL of the SharePoint site to connect to.")]
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

		public GetSharePointSite()
		{
		}

		protected SPWeb GetWebFromParameters()
		{
			Credentials credential = (this.User == null ? new Credentials() : new Credentials(this.User, this.Password.ToSecureString(), this.SavePassword));
			SPWeb sPWeb = new SPWeb(CommandLineParsingUtils.GetAdapterFromShortname(this.AdapterType));
			sPWeb.Adapter.Credentials = credential;
			sPWeb.Adapter.Url = this.SiteURL;
			sPWeb.Adapter.IsReadOnlyAdapter = this.ReadOnly;
			sPWeb.Adapter.AuthenticationType = this.AuthenticationType;
			try
			{
				sPWeb.FetchChildren();
				if (!UrlUtils.Equal(this.SiteURL, sPWeb.DisplayUrl))
				{
					base.ThrowTerminatingError(new ErrorRecord(new ArgumentException("Could not access web at the specified URL."), "ArgumentError", ErrorCategory.InvalidArgument, this.SiteURL));
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.ThrowTerminatingError(new ErrorRecord(exception, "Could not access web at the specified URL", ErrorCategory.InvalidArgument, this.SiteURL));
			}
			return sPWeb;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetWebFromParameters());
		}
	}
}