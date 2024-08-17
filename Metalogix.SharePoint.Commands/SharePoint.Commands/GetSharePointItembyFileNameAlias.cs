using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "SharePointItembyFileName")]
	public class GetSharePointItembyFileNameAlias : GetSharePointItembyFileName
	{
		public GetSharePointItembyFileNameAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}