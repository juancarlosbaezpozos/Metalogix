using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "SharePointList")]
	public class RemoveListCmdletAlias : RemoveListCmdlet
	{
		public RemoveListCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}