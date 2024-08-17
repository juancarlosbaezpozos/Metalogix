using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "SharePointSiteCollection")]
	public class CopySiteCollectionCmdletAlias : CopySiteCollectionCmdlet
	{
		public CopySiteCollectionCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}