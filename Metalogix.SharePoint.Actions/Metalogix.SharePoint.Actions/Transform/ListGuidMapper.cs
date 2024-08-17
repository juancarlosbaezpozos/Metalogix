using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ListGuidMapper : PreconfiguredTransformer<SPList, PasteListAction, SPListCollection, SPListCollection>
	{
		public override string Name
		{
			get
			{
				return "List GUID Mapping";
			}
		}

		public ListGuidMapper()
		{
		}

		public override void BeginTransformation(PasteListAction action, SPListCollection sources, SPListCollection targets)
		{
		}

		public override void EndTransformation(PasteListAction action, SPListCollection sources, SPListCollection targets)
		{
		}

		public override SPList Transform(SPList dataObject, PasteListAction action, SPListCollection sources, SPListCollection targets)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
			Dictionary<Guid, Guid> guids = action.CloneGuidMappings();
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//Field[@Type='Lookup' or @Type='LookupMulti']"))
			{
				XmlAttribute itemOf = xmlNodes.Attributes["List"];
				if (itemOf == null || !itemOf.Value.Contains("-"))
				{
					continue;
				}
				Guid guid = new Guid(itemOf.Value);
				if (guids.ContainsKey(guid))
				{
					continue;
				}
				XmlAttribute xmlAttribute = xmlNodes.Attributes["TargetListName"];
				if (xmlAttribute == null)
				{
					continue;
				}
				SPList item = targets[xmlAttribute.Value];
				if (item == null)
				{
					continue;
				}
				guids.Add(guid, new Guid(item.ID));
				XmlAttribute itemOf1 = xmlNodes.Attributes["WebId"];
				if (itemOf1 == null)
				{
					continue;
				}
				Guid guid1 = new Guid(itemOf1.Value);
				if (guids.ContainsKey(guid1))
				{
					continue;
				}
				guids.Add(guid1, new Guid(targets.ParentWeb.ID));
			}
			SPFieldCollection.MapFieldXmlGuids(xmlNode, guids, targets.ParentWeb.RootWebGUID, targets.ParentWeb.TaxonomyListGUID, action.ActionOptions.TermstoreNameMappingTable, new Guid(dataObject.ID), action.TransformationRepository, action.ActionOptions.ResolveManagedMetadataByName, dataObject.IsDocumentLibrary, sources.ParentWeb.AvailableColumns);
			dataObject.UpdateList(xmlNode.OuterXml, true, true);
			return dataObject;
		}
	}
}