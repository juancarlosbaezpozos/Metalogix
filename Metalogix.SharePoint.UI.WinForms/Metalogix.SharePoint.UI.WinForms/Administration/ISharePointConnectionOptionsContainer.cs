using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public interface ISharePointConnectionOptionsContainer
	{
		Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer AuthenticationInitializer
		{
			get;
			set;
		}

		Metalogix.Permissions.AzureAdGraphCredentials AzureAdGraphCredentials
		{
			get;
			set;
		}

		Metalogix.SharePoint.Adapters.ConnectionScope ConnectionScope
		{
			get;
			set;
		}

		Type ConnectionType
		{
			get;
			set;
		}

		Metalogix.Permissions.Credentials Credentials
		{
			get;
			set;
		}

		string Url
		{
			get;
			set;
		}
	}
}