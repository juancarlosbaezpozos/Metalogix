using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.AutoDetect;
using Metalogix.SharePoint.Adapters.CSOM2013Client;
using Metalogix.SharePoint.Adapters.MLWS;
using Metalogix.SharePoint.Adapters.NWS;
using Metalogix.SharePoint.Adapters.OM;
using System;

namespace Metalogix.SharePoint.Commands
{
	public class CommandLineParsingUtils
	{
		public CommandLineParsingUtils()
		{
		}

		public static SharePointAdapter GetAdapterFromShortname(string sShortName)
		{
			SharePointAdapter sharePointAutoDetectAdapter;
			if (sShortName == null)
			{
				return new SharePointAutoDetectAdapter();
			}
			string upper = sShortName.ToUpper();
			string str = upper;
			if (upper == null)
			{
				sharePointAutoDetectAdapter = new SharePointAutoDetectAdapter();
				return sharePointAutoDetectAdapter;
			}
			else if (str == "OM")
			{
				sharePointAutoDetectAdapter = new OMAdapter();
			}
			else if (str == "WS")
			{
				sharePointAutoDetectAdapter = new MLWSAdapter();
			}
			else if (str == "NWS")
			{
				sharePointAutoDetectAdapter = new NWSAdapter();
			}
			else
			{
				if (str != "CSOM")
				{
					sharePointAutoDetectAdapter = new SharePointAutoDetectAdapter();
					return sharePointAutoDetectAdapter;
				}
				sharePointAutoDetectAdapter = new CSOMClientAdapter();
			}
			return sharePointAutoDetectAdapter;
		}
	}
}