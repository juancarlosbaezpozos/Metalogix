using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.Transformers;
using Metalogix.Utilities;
using System;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class BDCUpdater : PreconfiguredTransformer<SPList, PasteListAction, SPListCollection, SPListCollection>
	{
		public override string Name
		{
			get
			{
				return "BDC to BCS Column Upgrade";
			}
		}

		public BDCUpdater()
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
			SPWeb parentWeb = targets.ParentWeb;
			if (dataObject.Adapter.SharePointVersion.IsSharePoint2007 && parentWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				string xML = dataObject.XML;
				string bCS = BCSHelper.ModifyFieldsXmlFromBDCToBCS(ref xML, dataObject, parentWeb);
				if (string.IsNullOrEmpty(bCS))
				{
					dataObject.SetXML(XmlUtility.StringToXmlNode(xML));
				}
				else
				{
					LogItem logItem = new LogItem("Updating BDC Columns", dataObject.Name, dataObject.DisplayUrl, parentWeb.DisplayUrl, ActionOperationStatus.Warning)
					{
						Information = string.Concat("BDC to BCS columns transformation: ", bCS)
					};
					base.FireOperationStarted(logItem);
					base.FireOperationFinished(logItem);
				}
			}
			return dataObject;
		}
	}
}