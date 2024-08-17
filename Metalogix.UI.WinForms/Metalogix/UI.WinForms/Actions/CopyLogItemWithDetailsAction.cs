using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Actions
{
	[MenuText("Copy Log Item to Clipboard {3-Copy} > With Details")]
	[MenuTextPlural("Copy Log Items to Clipboard {3-Copy} > With Details", PluralCondition.MultipleTargets)]
	[Name("Copy Log Items With Details")]
	[TargetType(typeof(LogItem))]
	public class CopyLogItemWithDetailsAction : CopyLogItemAction
	{
		public CopyLogItemWithDetailsAction()
		{
		}

		public override bool Configure(IXMLAbleList source, IXMLAbleList target)
		{
			base.IncludeDetails = true;
			return true;
		}
	}
}