using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "SharePointSiteCollection")]
	public class RemoveSiteCollectionCmdletAlias : RemoveSiteCollectionCmdlet
	{
		public RemoveSiteCollectionCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}