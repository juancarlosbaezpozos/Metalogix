using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "SharePointListViews")]
	public class CopyListViewsCmdletAlias : CopyListViewsCmdlet
	{
		public CopyListViewsCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}