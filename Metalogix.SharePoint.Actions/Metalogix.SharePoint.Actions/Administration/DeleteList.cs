using Metalogix.Actions;
using Metalogix.SharePoint;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[CmdletEnabled(false, "Remove-MLSharePointList", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Delete.ico")]
	[LaunchAsJob(false)]
	[MenuText("Delete List {2-Delete}")]
	[MenuTextPlural("Delete Lists {2-Delete}", PluralCondition.MultipleTargets)]
	[Name("Delete List")]
	[RequiresWriteAccess(true)]
	[Shortcut(ShortcutAction.Delete)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPList), false)]
	public class DeleteList : DeleteBase
	{
		public DeleteList()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPList sPList in target)
			{
				sPList.ParentWeb.Lists.DeleteList(sPList.ID);
			}
		}
	}
}