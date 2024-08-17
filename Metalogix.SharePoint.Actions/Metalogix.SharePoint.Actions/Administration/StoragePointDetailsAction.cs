using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.StoragePoint.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("StoragePoint Details... {5-aProperties}")]
	[Name("StoragePoint Details")]
	[RunAsync(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPListItem))]
	public class StoragePointDetailsAction : SharePointAction<SharePointActionOptions>
	{
		public StoragePointDetailsAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				SPListItem item = (SPListItem)targetSelections[0];
				flag = (!flag ? false : item.ParentList.IsDocumentLibrary);
				flag = (!flag ? false : item.IsExternalized);
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}