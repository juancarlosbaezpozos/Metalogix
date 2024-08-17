using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "SharePointSite")]
	public class GetSharePointSiteAlias : GetSharePointSite
	{
		public GetSharePointSiteAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}