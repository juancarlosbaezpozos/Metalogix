using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.ExportToFileOrange.ico")]
	[LaunchAsJob(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Save Metadata To Disk... {1-Save}")]
	[Name("Save Metadata To Disk")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPListItem))]
	public class ExportItemsWithMetadataAction : SharePointAction<SharePointActionOptions>
	{
		public ExportItemsWithMetadataAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (base.AppliesTo(sourceSelections, targetSelections) && !((SPListItem)targetSelections[0]).ParentList.IsDocumentLibrary)
			{
				return true;
			}
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}