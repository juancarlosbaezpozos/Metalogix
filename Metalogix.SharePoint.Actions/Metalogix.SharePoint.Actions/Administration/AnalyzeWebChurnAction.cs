using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[ShowInMenus(true)]
	[TargetType(typeof(SPWeb))]
	public class AnalyzeWebChurnAction : AnalyzeChurnAction
	{
		public AnalyzeWebChurnAction()
		{
		}
	}
}