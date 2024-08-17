using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "SharePointListFromDatabase")]
	public class GetSharePointListFromDatabaseAlias : GetSharePointListFromDatabase
	{
		public GetSharePointListFromDatabaseAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}