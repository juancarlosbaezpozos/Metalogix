using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.NWS.Properties;
using Metalogix.Utilities;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

namespace Metalogix.SharePoint.Adapters.CSOM2013.Authentication
{
	public class O365CSOMAuthenticationManager : CookieManager
	{
		private static Dictionary<string, SharePointOnlineCredentials> s_credentialCache;

		private static bool? bLockCookie;

		public override bool LockCookie
		{
			get
			{
				if (!O365CSOMAuthenticationManager.bLockCookie.HasValue)
				{
					O365CSOMAuthenticationManager.bLockCookie = new bool?(!AdapterConfigurationVariables.EnableConcurrentNWSOffice365Connections);
				}
				return O365CSOMAuthenticationManager.bLockCookie.Value;
			}
		}

		public SharePointOnlineCredentials O365Credentials
		{
			get;
			private set;
		}

		static O365CSOMAuthenticationManager()
		{
			O365CSOMAuthenticationManager.s_credentialCache = new Dictionary<string, SharePointOnlineCredentials>();
			O365CSOMAuthenticationManager.bLockCookie = null;
			Type type = typeof(ClientUtility);
			Assembly assembly = type.Assembly;
			MethodInfo method = type.GetMethod("GetSetupDirectory", BindingFlags.Static | BindingFlags.NonPublic);
			string str = (string)method.Invoke(null, null);
			if (!string.IsNullOrEmpty(str))
			{
				System.IO.File.Exists(Path.Combine(str, "msoidcliL.dll"));
			}
		}

		public O365CSOMAuthenticationManager(SharePointAdapter adapter) : base(adapter)
		{
			SharePointOnlineCredentials sharePointOnlineCredential;
			lock (O365CSOMAuthenticationManager.s_credentialCache)
			{
				if (!O365CSOMAuthenticationManager.s_credentialCache.TryGetValue(this.GetKey(), out sharePointOnlineCredential))
				{
					this.O365Credentials = new SharePointOnlineCredentials(adapter.Credentials.UserName, adapter.Credentials.Password);
					O365CSOMAuthenticationManager.s_credentialCache.Add(this.GetKey(), this.O365Credentials);
				}
				else
				{
					this.O365Credentials = sharePointOnlineCredential;
				}
			}
		}

		protected override IList<Cookie> GetCookie()
		{
			if (string.IsNullOrEmpty(this.O365Credentials.GetAuthenticationCookie(new Uri(base.Adapter.Url))))
			{
				throw new Exception(Metalogix.SharePoint.Adapters.NWS.Properties.Resources.SharePoint_Online_Empty_Cookie);
			}
			Cookie[] cookieArray = new Cookie[] { AuthenticationUtilities.MakeCookie(this.O365Credentials.GetAuthenticationCookie(new Uri(base.Adapter.Url)), base.Adapter.Url) };
			return cookieArray;
		}

		public static string GetCookieForUser(string url, string userName, string password)
		{
			return O365CSOMAuthenticationManager.GetCredentialsForUser(userName, password).GetAuthenticationCookie(new Uri(url));
		}

		private static SharePointOnlineCredentials GetCredentialsForUser(string userName, string password)
		{
			SharePointOnlineCredentials sharePointOnlineCredential;
			string key = O365CSOMAuthenticationManager.GetKey(userName, password);
			lock (O365CSOMAuthenticationManager.s_credentialCache)
			{
				if (!O365CSOMAuthenticationManager.s_credentialCache.TryGetValue(key, out sharePointOnlineCredential))
				{
					SecureString secureString = new SecureString();
					string str = password;
					for (int i = 0; i < str.Length; i++)
					{
						secureString.AppendChar(str[i]);
					}
					sharePointOnlineCredential = new SharePointOnlineCredentials(userName, secureString);
					O365CSOMAuthenticationManager.s_credentialCache.Add(key, sharePointOnlineCredential);
				}
			}
			return sharePointOnlineCredential;
		}

		private static string GetKey(string userName, string password)
		{
			return string.Concat(userName.ToLower(), password);
		}

		private string GetKey()
		{
			return O365CSOMAuthenticationManager.GetKey(base.Adapter.Credentials.UserName, base.Adapter.Credentials.Password.ToInsecureString());
		}
	}
}