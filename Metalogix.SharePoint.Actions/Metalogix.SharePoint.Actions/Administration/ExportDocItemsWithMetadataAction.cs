using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.ExportToFileOrange.ico")]
	[LaunchAsJob(true)]
	[MenuText("Save File and Metadata To Disk... {1-Save}")]
	[MenuTextPlural("Save Files and Metadata To Disk... {1-Save}", PluralCondition.MultipleTargets)]
	[Name("Save File and Metadata To Disk")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPListItem))]
	public class ExportDocItemsWithMetadataAction : ExportDocItemsAction
	{
		public ExportDocItemsWithMetadataAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}