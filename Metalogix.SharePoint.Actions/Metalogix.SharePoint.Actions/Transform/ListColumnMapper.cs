using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.Utilities;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class ListColumnMapper : BaseColumnMapper<SPList, PasteListAction, SPListCollection, SPListCollection>
	{
		public ListColumnMapper()
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
			ColumnMappings columnMappings = base.GetColumnMappings(dataObject, action.SharePointOptions);
			if (columnMappings != null)
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
				columnMappings.ModifyListXML(xmlNode, dataObject, targets.ParentWeb, action.SharePointOptions.FilterFields, SharePointConfigurationVariables.RemoveMappedColumns);
				dataObject.SetXML(xmlNode);
			}
			return dataObject;
		}
	}
}