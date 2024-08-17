using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "SharePointSiteWithConfigFile")]
	public class CopySiteWithConfigFileCmdletAlias : CopySiteWithConfigFileCmdlet
	{
		public CopySiteWithConfigFileCmdletAlias()
		{
		}
	}
}