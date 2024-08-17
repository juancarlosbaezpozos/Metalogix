using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPListTemplateCollection
	{
		private SPWeb m_parentWeb = null;

		private List<SPListTemplate> m_data;

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public SPListTemplate this[int index]
		{
			get
			{
				return this.m_data[index];
			}
		}

		public SPListTemplateCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
			this.m_data = new List<SPListTemplate>();
		}

		public void FetchData()
		{
			this.m_data.Clear();
			string listTemplates = this.m_parentWeb.Adapter.Reader.GetListTemplates();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(listTemplates);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes(".//ListTemplate"))
			{
				SPListTemplate sPListTemplate = new SPListTemplate(xmlNodes);
				this.m_data.Add(sPListTemplate);
			}
		}

		public SPListTemplate GetByType(string type)
		{
			SPListTemplate byType;
			try
			{
				byType = this.GetByType((ListTemplateType)Enum.Parse(typeof(ListTemplateType), type));
			}
			catch
			{
				byType = null;
			}
			return byType;
		}

		public SPListTemplate GetByType(ListTemplateType type)
		{
			SPListTemplate sPListTemplate;
			foreach (SPListTemplate sPListTemplate1 in this)
			{
				if (sPListTemplate1.TemplateType == type)
				{
					sPListTemplate = sPListTemplate1;
					return sPListTemplate;
				}
			}
			sPListTemplate = null;
			return sPListTemplate;
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}
	}
}