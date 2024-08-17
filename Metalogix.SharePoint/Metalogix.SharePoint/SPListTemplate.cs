using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPListTemplate
	{
		private readonly List<ListTemplateType> _NOT_CREATABLE_LIST_TYPES = new List<ListTemplateType>(new ListTemplateType[] { ListTemplateType.AccessServerApp });

		private XmlNode m_templateXml;

		public ListType BaseType
		{
			get
			{
				ListType listType;
				string attribute = this.GetAttribute("BaseType");
				listType = (attribute == null ? ListType.CustomList : (ListType)Enum.Parse(typeof(ListType), attribute));
				return listType;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.GetAttribute("DisplayName");
			}
		}

		public string FeatureId
		{
			get
			{
				return this.GetAttribute("FeatureId");
			}
		}

		public bool IsCreatableTemplateType
		{
			get
			{
				return !this._NOT_CREATABLE_LIST_TYPES.Contains(this.TemplateType);
			}
		}

		public bool IsHidden
		{
			get
			{
				bool flag;
				bool flag1 = false;
				flag = (!bool.TryParse(this.GetAttribute("Hidden"), out flag1) ? false : flag1);
				return flag;
			}
		}

		public string Name
		{
			get
			{
				return this.GetAttribute("Name");
			}
		}

		public ListTemplateType TemplateType
		{
			get
			{
				ListTemplateType listTemplateType;
				string attribute = this.GetAttribute("Type");
				listTemplateType = (attribute == null ? ListTemplateType.CustomList : (ListTemplateType)Enum.Parse(typeof(ListTemplateType), attribute));
				return listTemplateType;
			}
		}

		public SPListTemplate(XmlNode templateXml)
		{
			this.m_templateXml = templateXml.CloneNode(false);
		}

		public string GetAttribute(string sAttributeName)
		{
			string value;
			XmlAttribute itemOf = this.m_templateXml.Attributes[sAttributeName];
			if (itemOf == null)
			{
				value = null;
			}
			else
			{
				value = itemOf.Value;
			}
			return value;
		}

		public override string ToString()
		{
			return this.DisplayName;
		}
	}
}