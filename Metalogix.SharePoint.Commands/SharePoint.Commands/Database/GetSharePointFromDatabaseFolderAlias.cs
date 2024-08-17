using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "SharePointFolderFromDatabase")]
	public class GetSharePointFromDatabaseFolderAlias : GetSharePointFolderFromDatabase
	{
		public GetSharePointFromDatabaseFolderAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}