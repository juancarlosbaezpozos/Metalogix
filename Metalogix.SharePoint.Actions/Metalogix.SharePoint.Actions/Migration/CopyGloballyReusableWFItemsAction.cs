using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[Name("Paste Globally Reusable Workflow Items")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[SupportsThreeStateConfiguration(true)]
	[UsesStickySettings(true)]
	public class CopyGloballyReusableWFItemsAction : PasteAction<PasteFolderOptions>
	{
		public CopyGloballyReusableWFItemsAction()
		{
		}

		private void CopyGloballyReusableWorkflowList(SPList sourceList, SPWeb targetWeb)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = null;
			try
			{
				try
				{
					logItem = new LogItem("Copying wfpub library items", "Globally Reusable Workflow", sourceList.Url, targetWeb.Url, ActionOperationStatus.Running);
					base.FireOperationStarted(logItem);
					SPList item = targetWeb.Lists[sourceList.Name];
					logItem.Status = ActionOperationStatus.Completed;
					logItem.AddCompletionDetail(Resources.Migration_Detail_ListsCopied, (long)1);
					this.CopyItems(sourceList, item);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (logItem != null)
					{
						logItem.Status = ActionOperationStatus.Failed;
						logItem.Exception = exception;
					}
				}
			}
			finally
			{
				base.FireOperationFinished(logItem);
			}
		}

		private void CopyItems(SPList sourceList, SPList targetList)
		{
			IEnumerable<Node> nodes;
			PasteListItemAction pasteListItemAction = new PasteListItemAction();
			pasteListItemAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteListItemAction);
			LogItem logItem = null;
			if (sourceList != null)
			{
				nodes = new Node[] { sourceList };
			}
			else
			{
				nodes = null;
			}
			IXMLAbleList nodeCollection = new NodeCollection(nodes);
			using (targetList)
			{
				foreach (SPFolder sPFolder in nodeCollection)
				{
					try
					{
						try
						{
							logItem = new LogItem("Fetching source items", "", "", "", ActionOperationStatus.Running)
							{
								WriteToJobDatabase = false
							};
							base.FireOperationStarted(logItem);
							SPListItemCollection terseItemData = PasteActionUtils.GetTerseItemData(sPFolder, base.SharePointOptions);
							if (terseItemData != null && terseItemData.Count > 0)
							{
								terseItemData = this.FilterItems(terseItemData);
							}
							object[] objArray = new object[] { terseItemData, targetList, null, false };
							pasteListItemAction.RunAsSubAction(objArray, new ActionContext(sPFolder.ParentList.ParentWeb, targetList.ParentList.ParentWeb), null);
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							LogItem logItem1 = new LogItem("Fetching List Items", sPFolder.Name, sPFolder.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Failed)
							{
								Exception = exception
							};
							base.FireOperationStarted(logItem1);
							base.FireOperationFinished(logItem1);
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
					sPFolder.Dispose();
				}
			}
		}

		private SPListItemCollection FilterItems(SPListItemCollection itemCollection)
		{
			List<string> strs = new List<string>();
			itemCollection.ToList<Node>().ForEach((Node itemValue) => {
				SPListItem sPListItem = itemValue as SPListItem;
				if (sPListItem.Name.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase))
				{
					string attributeValueAsString = XmlUtility.StringToXmlNode(sPListItem.XML).GetAttributeValueAsString("BaseAssociationGuid");
					if (!string.IsNullOrEmpty(attributeValueAsString) && Utils.IsOOBWorkflowAssociation(new Guid(attributeValueAsString), sPListItem.ParentList.ParentWeb.Language, true))
					{
						strs.Add(sPListItem.ParentRelativePath);
					}
				}
			});
			if (strs.Count <= 0)
			{
				return itemCollection;
			}
			IEnumerable<Node> nodes = 
				from item in itemCollection
				where !strs.Contains(((SPListItem)item).ParentRelativePath)
				select item;
			SPListItem sPListItem1 = (SPListItem)itemCollection[0];
			return new SPListItemCollection(sPListItem1.ParentList, sPListItem1.ParentFolder, nodes.ToArray<Node>());
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPList item = source[0] as SPList;
			foreach (SPWeb sPWeb in target)
			{
				Node[] nodeArray = new Node[] { sPWeb };
				this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
				try
				{
					this.CopyGloballyReusableWorkflowList(item, sPWeb);
				}
				finally
				{
					sPWeb.Dispose();
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			SPList sPList = oParams[0] as SPList;
			this.CopyGloballyReusableWorkflowList(sPList, oParams[1] as SPWeb);
		}
	}
}