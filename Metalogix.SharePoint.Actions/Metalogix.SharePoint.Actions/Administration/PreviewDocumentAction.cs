using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Run.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Preview Document... {5-Properties}")]
	[Name("Preview Document")]
	[RunAsync(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPListItem))]
	public class PreviewDocumentAction : SharePointAction<SharePointActionOptions>
	{
		public PreviewDocumentAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				SPListItem item = (SPListItem)targetSelections[0];
				flag = (!flag ? false : item.ParentList.IsDocumentLibrary);
				flag = (!flag ? false : item.BinaryAvailable);
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}