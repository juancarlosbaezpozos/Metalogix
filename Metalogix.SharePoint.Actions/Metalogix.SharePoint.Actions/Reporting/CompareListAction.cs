using Metalogix.Actions;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Reporting
{
	[CmdletEnabled(true, "Compare-MLSharePointList", new string[] { "Metalogix.SharePoint.Commands" })]
	[MenuText("Compare List... {1-copy-compare}")]
	[Name("Compare List")]
	[RequiresWriteAccess(false)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.ZeroOrMore)]
	[TargetType(typeof(SPList))]
	public class CompareListAction : CompareSiteAction
	{
		public CompareListAction()
		{
		}

		protected override List<string> GetOptionsToIncludeInPowerShell()
		{
			List<string> strs = new List<string>()
			{
				"CompareFolders",
				"CompareItems",
				"CompareVersions",
				"CheckResults",
				"Verbose",
				"HaltIfDifferent",
				"FilterLists",
				"ListFilterExpression",
				"FilterItems",
				"ItemFilterExpression"
			};
			return strs;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPList item = (SPList)source[0];
			SPList sPList = (SPList)target[0];
			base.CompareFolders(item, sPList);
		}
	}
}