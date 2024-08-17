using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Commands;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointSiteWithConfigFile")]
	public class CopySiteWithConfigFileCmdlet : SharePointActionWithConfigFileCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(PasteSiteAction);
			}
		}

		public CopySiteWithConfigFileCmdlet()
		{
		}
	}
}