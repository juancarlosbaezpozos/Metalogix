using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "SharePointItembyID")]
	public class GetSharePointItembyIdAlias : GetSharePointItembyId
	{
		public GetSharePointItembyIdAlias()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}