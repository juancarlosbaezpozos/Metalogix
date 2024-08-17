using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "AllListItems")]
	public class CopyAllListItemsCmdletAlias : CopyAllListItemsCmdlet
	{
		public CopyAllListItemsCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}