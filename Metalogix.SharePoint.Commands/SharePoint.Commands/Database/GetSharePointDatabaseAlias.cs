using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "SharePointDatabase")]
	public class GetSharePointDatabaseAlias : GetSharePointDatabase
	{
		public GetSharePointDatabaseAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}