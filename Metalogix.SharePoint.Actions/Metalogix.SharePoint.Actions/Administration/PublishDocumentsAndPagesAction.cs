using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Checkin.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMWebComponents)]
	[MenuText("Check In, Publish or Approve Items...{1-Publish}")]
	[Name("Publish Documents and Pages")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPList), true)]
	public class PublishDocumentsAndPagesAction : SharePointAction<PublishDocumentsAndPagesOptions>
	{
		public PublishDocumentsAndPagesAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (current is SPList && ((SPList)current).BaseType == Metalogix.SharePoint.ListType.DocumentLibrary)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		public void PublishListItems(SPList List)
		{
			foreach (SPListItem item in List.GetItems(true, ListItemQueryType.ListItem))
			{
				item.UpdatePublishStatus(base.SharePointOptions.Publish, base.SharePointOptions.Checkin, base.SharePointOptions.Approve, base.SharePointOptions.CheckinComment, base.SharePointOptions.PublishComment, base.SharePointOptions.ApproveComment);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPList sPList in target)
			{
				this.PublishListItems(sPList);
			}
		}
	}
}