using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[MenuText("Copy Selected Item {1-Copy}")]
	[MenuTextPlural("Copy Selected Items {1-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy Selected Items")]
	[Shortcut(ShortcutAction.Copy)]
	[TargetType(typeof(SPListItem), true)]
	public class CopyListItemAction : CopyAction
	{
		public CopyListItemAction()
		{
		}
	}
}