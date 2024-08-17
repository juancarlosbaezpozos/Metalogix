using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class DocumentSetsApplicator : Transformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection, ApplyDocumentSetsOptions>
	{
		private const string DOCUMENT_SET_ID = "0x0120D520";

		private List<DocumentSetApplicationOptionsCollection> m_collectionsToApply;

		public override string Name
		{
			get
			{
				return "Apply Document Sets";
			}
		}

		public DocumentSetsApplicator()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			SPList parentSPList = targets.ParentSPList;
			SPList sPList = sources.ParentSPList;
			this.m_collectionsToApply = new List<DocumentSetApplicationOptionsCollection>();
			if (parentSPList.IsDocumentLibrary && action.SharePointOptions.ApplyNewDocumentSets && action.SharePointOptions.DocumentSetApplicationObjects != null && action.SharePointOptions.DocumentSetApplicationObjects.Count > 0)
			{
				LogItem logItem = null;
				foreach (DocumentSetApplicationOptionsCollection documentSetApplicationObject in action.SharePointOptions.DocumentSetApplicationObjects)
				{
					if (!documentSetApplicationObject.AppliesTo(sPList) || documentSetApplicationObject.Data.Count <= 0)
					{
						continue;
					}
					this.m_collectionsToApply.Add(documentSetApplicationObject);
					foreach (DocumentSetApplicationOptions datum in documentSetApplicationObject.Data)
					{
						try
						{
							try
							{
								logItem = new LogItem("Creating Document Set", datum.DocSetName, "", parentSPList.DisplayUrl, ActionOperationStatus.Running);
								string docSetName = datum.DocSetName;
								string serverRelativeUrl = parentSPList.ServerRelativeUrl;
								base.FireOperationStarted(logItem);
								SPContentType contentTypeByName = parentSPList.ContentTypes.GetContentTypeByName(datum.ContentTypeName);
								if (contentTypeByName == null)
								{
									contentTypeByName = parentSPList.ParentWeb.ContentTypes.GetContentTypeByName(datum.ContentTypeName);
									if (contentTypeByName != null)
									{
										contentTypeByName = (!contentTypeByName.ContentTypeID.Equals("0x0120D520") ? parentSPList.ContentTypes.AddOrUpdateContentType(contentTypeByName.Name, contentTypeByName.XML, contentTypeByName.ParentContentType.Name) : parentSPList.ContentTypes.AddOrUpdateContentType(contentTypeByName.Name, contentTypeByName.XML, contentTypeByName.Name));
									}
								}
								if (contentTypeByName != null)
								{
									string[] contentTypeID = new string[] { "<ListItem ContentTypeId=\"", contentTypeByName.ContentTypeID, "\" Title=\"", docSetName, "\" ContentType=\"", contentTypeByName.Name, "\" FSObjType=\"1\" />" };
									string str = string.Concat(contentTypeID);
									AddFolderOptions addFolderOption = new AddFolderOptions()
									{
										Overwrite = false
									};
									parentSPList.SubFolders.AddFolder(str, addFolderOption, AddFolderMode.Comprehensive);
									logItem.Status = ActionOperationStatus.Completed;
									if (action.SharePointOptions.Verbose)
									{
										logItem.TargetContent = parentSPList.XML;
									}
								}
							}
							catch (Exception exception)
							{
								logItem.Exception = exception;
							}
						}
						finally
						{
							base.FireOperationFinished(logItem);
						}
					}
				}
			}
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			this.m_collectionsToApply = null;
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			SPList parentSPList = targets.ParentSPList;
			SPFolder item = null;
			if (dataObject.ItemType != SPListItemType.Folder || action.SharePointOptions.FolderToDocumentSetApplicationObjects == null)
			{
				List<DocumentSetApplicationOptionsCollection>.Enumerator enumerator = this.m_collectionsToApply.GetEnumerator();
				try
				{
					do
					{
						if (!enumerator.MoveNext())
						{
							break;
						}
						using (IEnumerator<DocumentSetApplicationOptions> enumerator1 = enumerator.Current.Data.GetEnumerator())
						{
							do
							{
								if (!enumerator1.MoveNext())
								{
									break;
								}
								DocumentSetApplicationOptions current = enumerator1.Current;
								if (current.MapItemsFilter == null || !current.MapItemsFilter.Evaluate(dataObject, new CompareDatesInUtc()))
								{
									continue;
								}
								item = parentSPList.SubFolders[current.DocSetName] as SPFolder;
							}
							while (item == null);
						}
					}
					while (item == null);
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				if (item != null)
				{
					string dirName = dataObject.ParentFolder.DirName;
					string str = string.Concat(dirName, "/", item.Name);
					dataObject.FileDirRef = str;
				}
				return dataObject;
			}
			foreach (DocumentSetFolderOptions folderToDocumentSetApplicationObject in action.SharePointOptions.FolderToDocumentSetApplicationObjects)
			{
				if (folderToDocumentSetApplicationObject.FolderFilter == null || !folderToDocumentSetApplicationObject.FolderFilter.Evaluate(dataObject, new CompareDatesInUtc()))
				{
					continue;
				}
				SPContentType contentTypeByName = parentSPList.ContentTypes.GetContentTypeByName(folderToDocumentSetApplicationObject.ContentTypeName);
				if (contentTypeByName == null)
				{
					contentTypeByName = parentSPList.ParentWeb.ContentTypes.GetContentTypeByName(folderToDocumentSetApplicationObject.ContentTypeName);
					if (contentTypeByName != null)
					{
						contentTypeByName = (!contentTypeByName.ContentTypeID.Equals("0x0120D520") ? parentSPList.ContentTypes.AddOrUpdateContentType(contentTypeByName.Name, contentTypeByName.XML, contentTypeByName.ParentContentType.Name) : parentSPList.ContentTypes.AddOrUpdateContentType(contentTypeByName.Name, contentTypeByName.XML, contentTypeByName.Name));
					}
				}
				if (contentTypeByName == null)
				{
					continue;
				}
				XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
				if (xmlNode.Attributes["ContentType"] == null || xmlNode.Attributes["ContentTypeId"] == null)
				{
					continue;
				}
				xmlNode.Attributes["ContentType"].Value = contentTypeByName.Name;
				xmlNode.Attributes["ContentTypeId"].Value = contentTypeByName.ContentTypeID.ToString();
				dataObject.SetFullXML(xmlNode);
			}
			return dataObject;
		}
	}
}