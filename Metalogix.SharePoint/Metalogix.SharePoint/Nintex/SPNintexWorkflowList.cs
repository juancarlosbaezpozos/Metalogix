using Metalogix.SharePoint;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Nintex
{
	public class SPNintexWorkflowList : SPList
	{
		public override string ImageName
		{
			get
			{
				return "Metalogix.SharePoint.Icons.Nintex.ico";
			}
		}

		public SPNintexWorkflowList(SPWeb parentWeb, XmlNode listXml) : base(parentWeb, listXml)
		{
		}

		public SPNintexWorkflowList(SPWeb parentWeb, XmlNode listXml, bool isFullXml) : base(parentWeb, listXml, isFullXml)
		{
		}
	}
}