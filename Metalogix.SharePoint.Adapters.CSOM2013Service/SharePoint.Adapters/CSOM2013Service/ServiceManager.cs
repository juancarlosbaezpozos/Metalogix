using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.CSOM2013.Authentication;
using System;
using System.ServiceModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.CSOM2013Service
{
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	public class ServiceManager : IServiceManager
	{
		private static bool s_bRequireManualShutdown;

		internal static bool RequireManualShutdown
		{
			get
			{
				return ServiceManager.s_bRequireManualShutdown;
			}
		}

		static ServiceManager()
		{
			ServiceManager.s_bRequireManualShutdown = true;
		}

		public ServiceManager()
		{
		}

		public void EndService()
		{
			Application.Exit();
		}

		public string GetSharePointOnlineCookie(string url, string userName, string password)
		{
			return O365CSOMAuthenticationManager.GetCookieForUser(url, userName, password);
		}

		public void InitializeService(string sXML, bool bRequireManualShutdown)
		{
			ServiceManager.s_bRequireManualShutdown = bRequireManualShutdown;
			if (string.IsNullOrEmpty(sXML))
			{
				return;
			}
			AdapterConfigurationVariables.LoadConfigurationVariables(sXML);
		}
	}
}