using Metalogix.Actions;
using Metalogix.Licensing;
using System;

namespace Metalogix.UI.WinForms.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.CopySelectedJobsToClipboard16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Copy Log Item to Clipboard {3-Copy}")]
	[MenuTextPlural("Copy Log Items to Clipboard {3-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy Log Items Menu Header")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(LogItem))]
	public class CopyLogItemMenuHeader : ActionHeader, ILogAction
	{
		public CopyLogItemMenuHeader()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}