using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "SharePointSite")]
	public class RemoveSiteCmdletAlias : RemoveSiteCmdlet
	{
		public RemoveSiteCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}