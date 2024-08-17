using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[ShowInMenus(true)]
	[TargetType(typeof(SPList))]
	public class AnalyzeListChurnAction : AnalyzeChurnAction
	{
		public AnalyzeListChurnAction()
		{
		}
	}
}