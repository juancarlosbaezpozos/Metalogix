using Metalogix.Data.Filters;
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
	public class DocumentSetsFolderApplicator : Transformer<SPFolder, PasteFolderAction, SPFolderCollection, SPFolderCollection, ApplyDocumentSetsOptions>
	{
		private const string DOCUMENT_SET_ID = "0x0120D520";

		public override string Name
		{
			get
			{
				return "Apply Document Sets";
			}
		}

		public DocumentSetsFolderApplicator()
		{
		}

		public override void BeginTransformation(PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
		}

		public override void EndTransformation(PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
		}

		public override SPFolder Transform(SPFolder dataObject, PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
			SPList parentList = targets.ParentList;
			if (action.SharePointOptions.ApplyNewDocumentSets && action.SharePointOptions.FolderToDocumentSetApplicationObjects != null)
			{
				foreach (DocumentSetFolderOptions folderToDocumentSetApplicationObject in action.SharePointOptions.FolderToDocumentSetApplicationObjects)
				{
					if (folderToDocumentSetApplicationObject.FolderFilter == null || !folderToDocumentSetApplicationObject.FolderFilter.Evaluate(dataObject, new CompareDatesInUtc()))
					{
						continue;
					}
					SPContentType contentTypeByName = parentList.ContentTypes.GetContentTypeByName(folderToDocumentSetApplicationObject.ContentTypeName);
					if (contentTypeByName == null)
					{
						contentTypeByName = parentList.ParentWeb.ContentTypes.GetContentTypeByName(folderToDocumentSetApplicationObject.ContentTypeName);
						if (contentTypeByName != null)
						{
							contentTypeByName = (!contentTypeByName.ContentTypeID.Equals("0x0120D520") ? parentList.ContentTypes.AddOrUpdateContentType(contentTypeByName.Name, contentTypeByName.XML, contentTypeByName.ParentContentType.Name) : parentList.ContentTypes.AddOrUpdateContentType(contentTypeByName.Name, contentTypeByName.XML, contentTypeByName.Name));
						}
					}
					if (contentTypeByName == null)
					{
						continue;
					}
					XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
					if (xmlNode.Attributes["ContentType"] == null)
					{
						xmlNode.Attributes.Append(xmlNode.OwnerDocument.CreateAttribute("ContentType"));
					}
					if (xmlNode.Attributes["ContentTypeId"] == null)
					{
						xmlNode.Attributes.Append(xmlNode.OwnerDocument.CreateAttribute("ContentTypeId"));
					}
					if (xmlNode.Attributes["ContentType"] == null || xmlNode.Attributes["ContentTypeId"] == null)
					{
						continue;
					}
					xmlNode.Attributes["ContentType"].Value = contentTypeByName.Name;
					xmlNode.Attributes["ContentTypeId"].Value = contentTypeByName.ContentTypeID.ToString();
					dataObject.UpdateSettings(xmlNode.OuterXml);
				}
			}
			return dataObject;
		}
	}
}