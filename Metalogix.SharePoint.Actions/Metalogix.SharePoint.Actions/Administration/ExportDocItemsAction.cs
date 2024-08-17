using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.ExportToFileOrange.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Save File To Disk... {1-Save}")]
	[MenuTextPlural("Save Files To Disk... {1-Save}", PluralCondition.MultipleTargets)]
	[Name("Save File To Disk")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPListItem))]
	public class ExportDocItemsAction : SharePointAction<SharePointActionOptions>
	{
		public ExportDocItemsAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				SPListItem item = (SPListItem)targetSelections[0];
				flag = (!flag ? false : item.ParentList.IsDocumentLibrary);
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}