using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.ExternalConnections
{
	public static class Utils
	{
		public static NintexExternalConnection GetExternalNintexConfigurationDatabase(SPNode targetNode)
		{
			NintexExternalConnection nintexExternalConnection;
			foreach (NintexExternalConnection value in targetNode.GetExternalConnectionsOfType<NintexExternalConnection>(true).Values)
			{
				value.CheckConnection();
				if (value.IsConfigDB)
				{
					nintexExternalConnection = value;
					return nintexExternalConnection;
				}
			}
			nintexExternalConnection = null;
			return nintexExternalConnection;
		}
	}
}