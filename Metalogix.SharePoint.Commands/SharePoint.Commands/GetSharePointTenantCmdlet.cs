using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.Utilities;
using System;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "MLSharePointTenant")]
	public class GetSharePointTenantCmdlet : AssemblyBindingCmdlet
	{
		private bool _savePassword;

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="[Optional] The Azure AD Graph Application Client ID.")]
		public string AzureAdGraphAppClientId
		{
			get;
			set;
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="[Optional] The Azure AD Graph Application Secret.")]
		public string AzureAdGraphAppSecret
		{
			get;
			set;
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The password for the user provided.")]
		public string Password
		{
			get;
			set;
		}

		[Parameter(Mandatory=false, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="Indicates that the connection made to the SharePoint tenant should be read only.")]
		public SwitchParameter ReadOnly
		{
			get;
			set;
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

		[Parameter(Mandatory=true, Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The URL of the SharePoint tenant to connect to.")]
		public string TenantURL
		{
			get;
			set;
		}

		[Parameter(Mandatory=true, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="A user to connect as, in user@company.onmicrosoft.com format.")]
		public string User
		{
			get;
			set;
		}

		public GetSharePointTenantCmdlet()
		{
		}

		protected SPTenant GetTenantFromParameters()
		{
			SharePointAdapter adapterFromShortname = CommandLineParsingUtils.GetAdapterFromShortname("CSOM");
			SPTenant sPTenant = new SPTenant(adapterFromShortname);
			Credentials credential = new Credentials(this.User, this.Password.ToSecureString(), this.SavePassword);
			AutoDetectInitializer autoDetectInitializer = new AutoDetectInitializer();
			adapterFromShortname.Url = this.TenantURL;
			adapterFromShortname.IsReadOnlyAdapter = this.ReadOnly;
			adapterFromShortname.ConnectionScope = ConnectionScope.Tenant;
			if (!string.IsNullOrEmpty(this.AzureAdGraphAppClientId) && !string.IsNullOrEmpty(this.AzureAdGraphAppSecret))
			{
				adapterFromShortname.AzureAdGraphCredentials = new AzureAdGraphCredentials(this.AzureAdGraphAppClientId, this.AzureAdGraphAppSecret);
			}
			autoDetectInitializer.Credentials = credential;
			autoDetectInitializer.InitializeAuthenticationSettings(adapterFromShortname);
			try
			{
				sPTenant.CheckConnection(true);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.ThrowTerminatingError(new ErrorRecord(exception, "Could not access tenant with the specified parameters", ErrorCategory.InvalidArgument, this.TenantURL));
			}
			sPTenant.FetchChildren();
			return sPTenant;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetTenantFromParameters());
		}
	}
}