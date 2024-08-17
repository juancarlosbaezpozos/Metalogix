using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "SharePointFolder")]
	public class GetSharePointFolderAlias : GetSharePointFolder
	{
		public GetSharePointFolderAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}