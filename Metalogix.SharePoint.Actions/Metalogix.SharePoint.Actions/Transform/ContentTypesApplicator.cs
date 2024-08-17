using Metalogix;
using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ContentTypesApplicator : Transformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection, ApplyContentTypesOptions>
	{
		public override string Name
		{
			get
			{
				return "Apply Content Types";
			}
		}

		public ContentTypesApplicator()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			PasteListItemOptions options = action.Options as PasteListItemOptions;
			SPFolder parentFolder = targets.ParentFolder as SPFolder;
			if (options.ApplyNewContentTypes && options.ContentTypeApplicationObjects != null)
			{
				List<ContentTypeApplicationOptionsCollection> contentTypeApplicationOptionsCollections = new List<ContentTypeApplicationOptionsCollection>();
				if (options.ContentTypeApplicationObjects != null)
				{
					foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in options.ContentTypeApplicationObjects)
					{
						if (!contentTypeApplicationObject.AppliesTo(sources.ParentSPList))
						{
							continue;
						}
						contentTypeApplicationOptionsCollections.Add(contentTypeApplicationObject);
						break;
					}
				}
				if (contentTypeApplicationOptionsCollections.Count > 0)
				{
					bool flag = true;
					try
					{
						SPContentTypeCollection contentTypes = parentFolder.ParentList.ContentTypes;
						SPContentTypeCollection sPContentTypeCollections = parentFolder.ParentList.ParentWeb.ContentTypes;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						flag = false;
						LogItem logItem = new LogItem("Initializing Content Types", parentFolder.ParentList.Name, sources.ParentSPList.DisplayUrl, parentFolder.ParentList.ParentWeb.DisplayUrl, ActionOperationStatus.Running);
						base.FireOperationStarted(logItem);
						logItem.Exception = exception;
						logItem.Status = ActionOperationStatus.Failed;
						base.FireOperationFinished(logItem);
					}
					if (flag)
					{
						foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection in contentTypeApplicationOptionsCollections)
						{
							CopyContentTypesAction copyContentTypesAction = new CopyContentTypesAction();
							copyContentTypesAction.Options.SetFromOptions(base.Options);
							action.SubActions.Add(copyContentTypesAction);
							object[] parentList = new object[] { parentFolder.ParentList, contentTypeApplicationOptionsCollection };
							copyContentTypesAction.RunAsSubAction("ApplyNewContentTypes", parentList, new ActionContext(null, parentFolder.ParentList.ParentWeb));
						}
					}
				}
			}
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			if (!action.SharePointOptions.ApplyNewContentTypes || action.SharePointOptions.ContentTypeApplicationObjects == null)
			{
				return dataObject;
			}
			List<ContentTypeApplicationOptionsCollection> contentTypeApplicationOptionsCollections = new List<ContentTypeApplicationOptionsCollection>();
			if (action.SharePointOptions.ContentTypeApplicationObjects != null)
			{
				foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in action.SharePointOptions.ContentTypeApplicationObjects)
				{
					if (!contentTypeApplicationObject.AppliesTo(dataObject.ParentList))
					{
						continue;
					}
					contentTypeApplicationOptionsCollections.Add(contentTypeApplicationObject);
					break;
				}
			}
			if (contentTypeApplicationOptionsCollections.Count == 0)
			{
				return dataObject;
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
			foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection in contentTypeApplicationOptionsCollections)
			{
				contentTypeApplicationOptionsCollection.ApplyContentTypeMappingsToListItem(xmlNode, dataObject, targets.ParentSPList);
			}
			dataObject.SetFullXML(xmlNode);
			return dataObject;
		}
	}
}