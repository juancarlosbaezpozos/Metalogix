using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "SharePointList")]
	public class CopyListCmdletAlias : CopyListCmdlet
	{
		public CopyListCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}