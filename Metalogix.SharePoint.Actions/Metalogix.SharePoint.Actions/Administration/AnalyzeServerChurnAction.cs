using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[ShowInMenus(true)]
	[TargetType(typeof(SPServer))]
	public class AnalyzeServerChurnAction : AnalyzeChurnAction
	{
		public AnalyzeServerChurnAction()
		{
		}
	}
}