using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "SharePointItem")]
	public class RemoveListItemCmdletAlias : RemoveListItemCmdlet
	{
		public RemoveListItemCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}