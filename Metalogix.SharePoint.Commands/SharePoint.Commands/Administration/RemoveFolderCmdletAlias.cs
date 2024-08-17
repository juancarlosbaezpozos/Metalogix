using Metalogix.Commands;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Administration
{
	[Cmdlet("Remove", "SharePointFolder")]
	public class RemoveFolderCmdletAlias : RemoveFolderCmdlet
	{
		public RemoveFolderCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}