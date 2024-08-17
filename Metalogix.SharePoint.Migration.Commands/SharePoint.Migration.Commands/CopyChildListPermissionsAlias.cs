using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "ChildListPermissions")]
	public class CopyChildListPermissionsAlias : CopyChildListPermissions
	{
		public CopyChildListPermissionsAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}