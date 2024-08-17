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
	public class FolderColumnMapper : BaseColumnMapper<SPFolder, PasteFolderAction, SPFolderCollection, SPFolderCollection>
	{
		private ColumnMappings columnMappings;

		public FolderColumnMapper()
		{
		}

		public override void BeginTransformation(PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
			this.columnMappings = base.GetColumnMappings(sources.ParentList, action.SharePointOptions);
		}

		public override void EndTransformation(PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
			this.columnMappings = null;
		}

		public override SPFolder Transform(SPFolder dataObject, PasteFolderAction action, SPFolderCollection sources, SPFolderCollection targets)
		{
			if (this.columnMappings != null && !(dataObject is SPList))
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(dataObject.XML);
				SPFolder parentFolder = targets.ParentFolder;
				this.columnMappings.ModifyListItemXML(xmlNode, dataObject.ParentFolder, parentFolder, action.SharePointOptions.FilterFields);
				dataObject.SetFullXML(xmlNode);
			}
			return dataObject;
		}
	}
}