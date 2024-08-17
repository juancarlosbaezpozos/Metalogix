using Metalogix.SharePoint.Actions.Reporting;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Reporting
{
	[Cmdlet("Compare", "MLSharePointList")]
	public class CompareListCmdlet : CompareFolderCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CompareListAction);
			}
		}

		public CompareListCmdlet()
		{
		}
	}
}