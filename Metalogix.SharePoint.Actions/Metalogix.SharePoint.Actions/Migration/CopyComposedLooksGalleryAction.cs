using Metalogix;
using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(false)]
	[CmdletEnabled(false, "Copy-MLSharePointComposedLooksGallery", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.MasterPageGallery.ico")]
	[LaunchAsJob(true)]
	[MenuText("3:Paste Site Objects {0-Paste} > Composed Looks Gallery...")]
	[Name("Paste Composed Looks Gallery")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb), true)]
	[SubActionTypes(new Type[] { typeof(CopyListItemAction) })]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb), true)]
	[UsesStickySettings(true)]
	public class CopyComposedLooksGalleryAction : PasteListAction
	{
		public CopyComposedLooksGalleryAction()
		{
		}

		private void CopyComposedLooks(SPFolder sourceGallery, SPFolder targetGallery, bool copyAllThemes)
		{
			if (sourceGallery == null)
			{
				throw new Exception("Source master page gallery cannot be null");
			}
			if (targetGallery == null)
			{
				throw new Exception("Target master page gallery cannot be null");
			}
			SPListItemCollection items = sourceGallery.GetItems(false, ListItemQueryType.ListItem, null);
			if (base.SharePointOptions.CorrectingLinks)
			{
				base.LinkCorrector.PopulateForItemCopy(items, targetGallery);
			}
			if (copyAllThemes)
			{
				try
				{
					targetGallery.Items.DeleteAllItems();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogItem logItem = new LogItem(Resources.DeleteComposedLooksFailed, "", "", targetGallery.DisplayUrl, ActionOperationStatus.Skipped)
					{
						Information = exception.Message
					};
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
				PasteAllListItemsAction pasteAllListItemsAction = new PasteAllListItemsAction();
				pasteAllListItemsAction.Options.SetFromOptions(this.Options);
				base.SubActions.Add(pasteAllListItemsAction);
				object[] objArray = new object[] { sourceGallery, targetGallery };
				pasteAllListItemsAction.RunAsSubAction(objArray, new ActionContext(sourceGallery.ParentList, targetGallery.ParentList), null);
				return;
			}
			try
			{
				SPListItem currentItem = ((SPComposedLooksGallery)targetGallery).CurrentItem;
				targetGallery.Items.DeleteItem(currentItem);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				LogItem logItem1 = new LogItem(Resources.DeleteCurrentLookFailed, "", "", targetGallery.DisplayUrl, ActionOperationStatus.Skipped)
				{
					Information = exception2.Message
				};
				base.FireOperationStarted(logItem1);
				base.FireOperationFinished(logItem1);
				throw;
			}
			SPListItem sPListItem = ((SPComposedLooksGallery)sourceGallery).CurrentItem;
			SPList parentList = sourceGallery.ParentList;
			SPListItem[] sPListItemArray = new SPListItem[] { sPListItem };
			SPListItemCollection sPListItemCollection = new SPListItemCollection(parentList, sourceGallery, sPListItemArray);
			PasteListItemAction pasteListItemAction = new PasteListItemAction();
			pasteListItemAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteListItemAction);
			object[] objArray1 = new object[] { sPListItemCollection, targetGallery, null, false };
			pasteListItemAction.RunAsSubAction(objArray1, new ActionContext(sourceGallery.ParentList, targetGallery.ParentList), null);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPList composedLooksGallery = ((SPWeb)source[0]).ComposedLooksGallery;
			SPList sPList = ((SPWeb)target[0]).ComposedLooksGallery;
			this.CopyComposedLooks(composedLooksGallery, sPList, true);
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			SPList composedLooksGallery = ((SPWeb)oParams[0]).ComposedLooksGallery;
			SPList sPList = ((SPWeb)oParams[1]).ComposedLooksGallery;
			bool flag = false;
			if ((int)oParams.Length >= 3 && oParams[2] != null && oParams[2] is bool)
			{
				flag = (bool)oParams[2];
			}
			this.CopyComposedLooks(composedLooksGallery, sPList, flag);
		}
	}
}