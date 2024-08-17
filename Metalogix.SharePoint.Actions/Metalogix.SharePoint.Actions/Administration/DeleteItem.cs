using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Administration
{
	[CmdletEnabled(false, "Remove-MLSharePointItem", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Delete.ico")]
	[LaunchAsJob(false)]
	[MenuText("Delete Selected Item {2-Delete}")]
	[MenuTextPlural("Delete Selected Items {2-Delete}", PluralCondition.MultipleTargets)]
	[Name("Delete Selected Items")]
	[RequiresWriteAccess(true)]
	[Shortcut(ShortcutAction.Delete)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPListItem), false)]
	public class DeleteItem : DeleteBase
	{
		public DeleteItem()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target.Count == 0)
			{
				return;
			}
			ExplorerNode item = (ExplorerNode)target[0];
			SPListItemCollection parentCollection = ((SPListItem)item).ParentCollection;
			List<SPListItem> sPListItems = new List<SPListItem>();
			foreach (SPListItem sPListItem in target)
			{
				sPListItems.Add(sPListItem);
			}
			parentCollection.DeleteItems(sPListItems.ToArray());
			((ExplorerNode)item.Parent).UpdateCurrentNode();
		}
	}
}