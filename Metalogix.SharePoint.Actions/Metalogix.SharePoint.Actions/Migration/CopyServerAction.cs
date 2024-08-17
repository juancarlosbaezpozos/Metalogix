using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Image("Metalogix.SharePoint.Migration.Icons.Copy.ico")]
	[LicensedProducts(ProductFlags.CMCFileShare)]
	[MenuText("Copy Server {1-Copy}")]
	[MenuTextPlural("Copy Server {1-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy Server")]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.Copy)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPServer))]
	public class CopyServerAction : CopyAction
	{
		public CopyServerAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			ClipBoard.SetDataObject(target);
		}
	}
}