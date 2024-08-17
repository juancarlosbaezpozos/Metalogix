using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "SharePointSiteFromDatabase")]
	public class GetSharePointSiteFromDatabaseAlias : GetSharePointSiteFromDatabase
	{
		public GetSharePointSiteFromDatabaseAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}