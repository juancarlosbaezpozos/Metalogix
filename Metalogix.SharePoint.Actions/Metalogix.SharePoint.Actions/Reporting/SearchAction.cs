using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
using System;

namespace Metalogix.SharePoint.Actions.Reporting
{
	[Batchable(false, "")]
	[Image("Metalogix.SharePoint.Actions.Icons.Reporting.MagnifyingGlass.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Search... {4-Refresh}")]
	[Name("Search")]
	[RequiresFullEdition(false)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPNode), true)]
	public class SearchAction : Metalogix.Actions.Action
	{
		public SearchAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				SPConnection item = targetSelections[0] as SPConnection;
				if (item != null && item.Status == ConnectionStatus.Invalid)
				{
					return false;
				}
				Type type = targetSelections[0].GetType();
				if (typeof(SPListItem).IsAssignableFrom(type))
				{
					return false;
				}
				if (typeof(SPNode).IsAssignableFrom(type))
				{
					return true;
				}
			}
			return flag;
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (targetSelections.Count > 0)
			{
				if (!BCSHelper.SharePointActionEnabledOn(this, sourceSelections, targetSelections))
				{
					return false;
				}
				Type type = targetSelections[0].GetType();
				if (typeof(SPServer).IsAssignableFrom(type))
				{
					return true;
				}
				if (typeof(SPWeb).IsAssignableFrom(type))
				{
					return ((SPWeb)targetSelections[0]).IsSearchable;
				}
				if (typeof(SPFolder).IsAssignableFrom(type))
				{
					return ((SPFolder)targetSelections[0]).ParentList.ParentWeb.IsSearchable;
				}
			}
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}