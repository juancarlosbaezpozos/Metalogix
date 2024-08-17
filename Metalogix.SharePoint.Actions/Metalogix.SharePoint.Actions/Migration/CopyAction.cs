using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint.Actions;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[ActionConfigRequired(false)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Copy.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMWebComponents)]
	[RunAsync(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	public abstract class CopyAction : SharePointAction<Metalogix.Actions.ActionOptions>
	{
		protected CopyAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			ClipBoard.SetDataObject(target);
		}
	}
}