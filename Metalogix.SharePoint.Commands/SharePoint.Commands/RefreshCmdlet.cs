using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Refresh", "SharePointNode")]
	public class RefreshCmdlet : ActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(RefreshAction);
			}
		}

		public RefreshCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}