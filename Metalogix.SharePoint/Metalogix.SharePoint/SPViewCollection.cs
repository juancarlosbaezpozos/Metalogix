using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPViewCollection
	{
		private List<SPView> m_data;

		private SPList m_parentList;

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public SPView DefaultView
		{
			get
			{
				SPView item;
				foreach (SPView sPView in this)
				{
					if (sPView.IsDefault)
					{
						item = sPView;
						return item;
					}
				}
				if (this.Count <= 0)
				{
					item = null;
				}
				else
				{
					item = this[0];
				}
				return item;
			}
		}

		public SPView this[int index]
		{
			get
			{
				return this.m_data[index];
			}
		}

		public SPView this[string sName]
		{
			get
			{
				SPView sPView;
				foreach (SPView sPView1 in this)
				{
					if (string.Equals(sPView1.Name, sName, StringComparison.OrdinalIgnoreCase))
					{
						sPView = sPView1;
						return sPView;
					}
				}
				sPView = null;
				return sPView;
			}
		}

		public SPViewCollection(SPList parentList, XmlNode viewCollection)
		{
			this.m_parentList = parentList;
			this.m_data = new List<SPView>();
			foreach (XmlNode xmlNodes in viewCollection.SelectNodes(".//View"))
			{
				this.m_data.Add(new SPView(this.m_parentList, xmlNodes));
			}
		}

		public SPView AddOrUpdateView(string sXmlView)
		{
			SPView sPView = null;
			if (!this.m_parentList.WriteVirtually)
			{
				if (this.m_parentList.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				string str = this.m_parentList.Adapter.Writer.AddView(this.m_parentList.ID, sXmlView);
				XmlNode xmlNode = XmlUtility.StringToXmlNode(str);
				sPView = new SPView(this.m_parentList, xmlNode);
				SPView viewByDisplayName = this.GetViewByDisplayName(sPView.DisplayName);
				if (viewByDisplayName != null)
				{
					this.m_data.Remove(viewByDisplayName);
				}
				this.m_data.Add(sPView);
			}
			else
			{
				XmlNode xmlNodes = XmlUtility.StringToXmlNode(sXmlView).SelectSingleNode("//View");
				XmlNode xmlNode1 = XmlUtility.StringToXmlNode(this.m_parentList.XML);
				XmlNode xmlNodes1 = xmlNode1.SelectSingleNode("./Views");
				string str1 = "./View[@DisplayName='{0}']";
				XmlNode xmlNodes2 = XmlUtility.CloneXMLNodeForTarget(xmlNodes, xmlNodes1, true);
				XmlNode xmlNodes3 = xmlNodes1.SelectSingleNode(string.Format(str1, xmlNodes.Attributes["DisplayName"]));
				if (xmlNodes3 == null)
				{
					xmlNodes1.AppendChild(xmlNodes2);
				}
				else
				{
					xmlNodes1.InsertAfter(xmlNodes2, xmlNodes3);
					xmlNodes1.RemoveChild(xmlNodes3);
				}
				this.m_parentList.UpdateList(xmlNode1.OuterXml, true, true);
			}
			return sPView;
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}

		public SPView GetViewByDisplayName(string sDisplayName)
		{
			SPView sPView;
			foreach (SPView sPView1 in this)
			{
				if (sPView1.DisplayName.Equals(sDisplayName, StringComparison.OrdinalIgnoreCase))
				{
					sPView = sPView1;
					return sPView;
				}
			}
			sPView = null;
			return sPView;
		}

		internal void ResetViews(XmlNode viewCollection)
		{
			this.m_data.Clear();
			foreach (XmlNode xmlNodes in viewCollection.SelectNodes(".//View"))
			{
				this.m_data.Add(new SPView(this.m_parentList, xmlNodes));
			}
		}
	}
}