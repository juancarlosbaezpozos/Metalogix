using Metalogix.Actions;
using Metalogix.SharePoint;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Administration.Icons.Delete.ico")]
	[LaunchAsJob(false)]
	[MenuText("Delete Selected Item {2-Delete}")]
	[MenuTextPlural("Delete Selected Items {2-Delete}", PluralCondition.MultipleTargets)]
	[Name("Delete Selected Items")]
	[RequiresWriteAccess(true)]
	[Shortcut(ShortcutAction.Delete)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPDiscussionItem), false)]
	public class DeleteDiscussionItem : DeleteBase
	{
		public DeleteDiscussionItem()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target.Count == 0)
			{
				return;
			}
			foreach (SPDiscussionItem sPDiscussionItem in target)
			{
				string item = sPDiscussionItem["ContentType"];
				if (item != "Message")
				{
					if (item != "Discussion")
					{
						continue;
					}
					sPDiscussionItem.ParentCollection.DeleteDiscussionFolder(sPDiscussionItem);
				}
				else
				{
					sPDiscussionItem.ParentCollection.DeleteItem(sPDiscussionItem);
				}
			}
		}
	}
}