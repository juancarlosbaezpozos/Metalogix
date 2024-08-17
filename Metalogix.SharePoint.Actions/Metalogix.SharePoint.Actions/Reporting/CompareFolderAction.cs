using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Reporting
{
	[CmdletEnabled(true, "Compare-MLSharePointFolder", new string[] { "Metalogix.SharePoint.Commands" })]
	[MenuText("Compare Folder... {1-copy-compare}")]
	[Name("Compare Folder")]
	[RequiresWriteAccess(false)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.ZeroOrMore)]
	[TargetType(typeof(SPFolder), false)]
	public class CompareFolderAction : CompareListAction
	{
		public CompareFolderAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPFolder item = (SPFolder)source[0];
			SPFolder sPFolder = (SPFolder)target[0];
			base.CompareFolders(item, sPFolder);
		}
	}
}