using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Actions
{
	[MenuText("Copy Log Item to Clipboard {3-Copy} > Without Details")]
	[MenuTextPlural("Copy Log Items to Clipboard {3-Copy} > Without Details", PluralCondition.MultipleTargets)]
	[Name("Copy Log Items Without Details")]
	[Shortcut(ShortcutAction.Copy)]
	[TargetType(typeof(LogItem))]
	public class CopyLogItemWithoutDetailsAction : CopyLogItemAction
	{
		public CopyLogItemWithoutDetailsAction()
		{
		}

		public override bool Configure(IXMLAbleList source, IXMLAbleList target)
		{
			base.IncludeDetails = false;
			return true;
		}
	}
}