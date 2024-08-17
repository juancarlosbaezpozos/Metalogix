using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "SharePointServer")]
	public class GetSharePointServerAlias : GetSharePointServer
	{
		public GetSharePointServerAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}