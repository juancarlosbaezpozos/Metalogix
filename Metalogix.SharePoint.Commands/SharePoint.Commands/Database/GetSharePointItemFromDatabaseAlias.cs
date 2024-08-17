using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "SharePointItemFromDatabase")]
	public class GetSharePointItemFromDatabaseAlias : GetSharePointItemFromDatabase
	{
		public GetSharePointItemFromDatabaseAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}