using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "AllSharePointSiteContent")]
	public class CopySiteContentCmdletAlias : CopySiteContentCmdlet
	{
		public CopySiteContentCmdletAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}