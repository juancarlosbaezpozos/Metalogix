using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.ExportToFileOrange.ico")]
	[IsAdvanced(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Export List Metadata {1-Save}")]
	[Name("Export List Metadata")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPList))]
	public class ExportListToXMLAction : SharePointAction<SharePointActionOptions>
	{
		public ExportListToXMLAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}