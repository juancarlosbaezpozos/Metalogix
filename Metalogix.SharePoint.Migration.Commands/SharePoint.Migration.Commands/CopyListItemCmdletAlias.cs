using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "SharePointItem")]
	public class CopyListItemCmdletAlias : CopyListItemCmdlet
	{
		public CopyListItemCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}