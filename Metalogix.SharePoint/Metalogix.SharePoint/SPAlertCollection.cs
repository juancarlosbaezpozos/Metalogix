using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPAlertCollection
	{
		private SPWeb m_parentWeb;

		private SPList m_parentList;

		private SPListItem m_parentListItem;

		private List<SPAlert> m_Alerts = new List<SPAlert>();

		public int Count
		{
			get
			{
				return this.m_Alerts.Count;
			}
		}

		public SPAlert this[int iIndex]
		{
			get
			{
				return this.m_Alerts[iIndex];
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public SPAlertCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
		}

		public SPAlertCollection(SPList parentList)
		{
			this.m_parentList = parentList;
			this.m_parentWeb = parentList.ParentWeb;
		}

		public SPAlertCollection(SPListItem parentListItem)
		{
			this.m_parentListItem = parentListItem;
			this.m_parentList = parentListItem.ParentList;
			this.m_parentWeb = parentListItem.ParentList.ParentWeb;
		}

		public string AddAlert(string sSiteUrl, string sWebId, XmlNode alertXML)
		{
			SPAlert sPAlert;
			string title;
			SPNode mParentListItem;
			if (this.m_parentListItem != null)
			{
				mParentListItem = this.m_parentListItem;
			}
			else if (this.m_parentList != null)
			{
				mParentListItem = this.m_parentList;
			}
			else
			{
				mParentListItem = this.m_parentWeb;
			}
			SPNode sPNode = mParentListItem;
			if (!sPNode.WriteVirtually)
			{
				if (this.ParentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying adapter does not support write operations");
				}
				string str = this.ParentWeb.Adapter.Writer.AddAlerts(sSiteUrl, sWebId, alertXML.SelectSingleNode("//Alert").OuterXml);
				XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
				XmlNode xmlNodes = xmlNode.SelectSingleNode("//Alert");
				if (xmlNodes == null)
				{
					XmlNode xmlNodes1 = xmlNode.SelectSingleNode("//Error");
					title = (xmlNodes1 == null ? "An unknown error has occurred while adding the alert" : xmlNodes1.InnerText);
				}
				else
				{
					sPAlert = new SPAlert(xmlNodes);
					this.m_Alerts.Add(sPAlert);
					title = sPAlert.Title;
				}
			}
			else
			{
				string xML = this.ToXML();
				sPAlert = new SPAlert(alertXML);
				this.m_Alerts.Add(sPAlert);
				string xML1 = this.ToXML();
				sPNode.SaveVirtualData(XmlUtility.StringToXmlNode(xML), XmlUtility.StringToXmlNode(xML1), "Alerts");
				title = sPAlert.Title;
			}
			return title;
		}

		public bool ContainsAlertWithXML(XmlNode alertNode)
		{
			bool flag;
			foreach (SPAlert sPAlert in this)
			{
				if (sPAlert.AlertEqualsXML(alertNode))
				{
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		public void FetchData()
		{
			string constantID;
			SPNode mParentListItem;
			this.m_Alerts.Clear();
			if (this.m_parentList != null)
			{
				constantID = this.m_parentList.ConstantID;
			}
			else
			{
				constantID = null;
			}
			string str = constantID;
			int num = (this.m_parentListItem != null ? this.m_parentListItem.ConstantID : -1);
			string alerts = this.m_parentWeb.Adapter.Reader.GetAlerts(str, num);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(alerts);
			if (this.m_parentListItem != null)
			{
				mParentListItem = this.m_parentListItem;
			}
			else if (this.m_parentList != null)
			{
				mParentListItem = this.m_parentList;
			}
			else
			{
				mParentListItem = this.m_parentWeb;
			}
			mParentListItem.AttachVirtualData(xmlDocument.DocumentElement, "Alerts");
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//AlertCollection//Alert"))
			{
				this.m_Alerts.Add(new SPAlert(xmlNodes));
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_Alerts.GetEnumerator();
		}

		public string ToXML()
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
			this.ToXML(new XmlTextWriter(stringWriter));
			return stringWriter.ToString();
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("AlertCollection");
			foreach (SPAlert mAlert in this.m_Alerts)
			{
				xmlWriter.WriteRaw(mAlert.XML);
			}
			xmlWriter.WriteEndElement();
		}
	}
}