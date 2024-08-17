using Metalogix.ExternalConnections;
using Metalogix.SharePoint.Options;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.ExternalConnections
{
	public class AttachNintexConnectionsOptions : SharePointActionOptions
	{
		public Dictionary<int, ExternalConnection> Connections
		{
			get;
			set;
		}

		public AttachNintexConnectionsOptions()
		{
		}
	}
}