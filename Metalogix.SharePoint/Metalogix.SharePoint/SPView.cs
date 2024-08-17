using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPView
	{
		private SPList m_parentList;

		private XmlNode m_viewXML;

		private CalendarSettingCollection m_calendarSettings;

		public CalendarSettingCollection CalendarSettings
		{
			get
			{
				if (this.m_calendarSettings == null)
				{
					this.m_calendarSettings = CalendarSettingCollection.Create(this.m_viewXML.SelectSingleNode("./CalendarSettings"));
				}
				return this.m_calendarSettings;
			}
		}

		public string ContentTypeId
		{
			get
			{
				string value;
				if (this.m_viewXML.Attributes["ContentTypeID"] == null)
				{
					value = null;
				}
				else
				{
					value = this.m_viewXML.Attributes["ContentTypeID"].Value;
				}
				return value;
			}
		}

		public string DisplayName
		{
			get
			{
				string value;
				if (this.m_viewXML.Attributes["DisplayName"] == null)
				{
					value = null;
				}
				else
				{
					value = this.m_viewXML.Attributes["DisplayName"].Value;
				}
				return value;
			}
		}

		public SPFieldCollection Fields
		{
			get
			{
				Dictionary<string, SPField> strs = new Dictionary<string, SPField>();
				foreach (XmlNode xmlNodes in this.m_viewXML.SelectNodes(".//FieldRef/@Name"))
				{
					SPField sPField = new SPField(xmlNodes);
					if (!strs.ContainsKey(sPField.Name))
					{
						strs.Add(sPField.Name, sPField);
					}
				}
				return ((SPFieldCollection)this.m_parentList.Fields).GetFieldsByIdOrName(strs.Values);
			}
		}

		public bool IsDefault
		{
			get
			{
				return ((this.m_viewXML.Attributes["Type"] == null ? true : !(this.m_viewXML.Attributes["Type"].Value == "0")) ? false : true);
			}
		}

		public bool IsDefaultView
		{
			get
			{
				bool flag;
				flag = (this.m_viewXML.Attributes["DefaultView"] != null ? bool.Parse(this.m_viewXML.Attributes["DefaultView"].Value) : false);
				return flag;
			}
		}

		public bool IsFromTemplate
		{
			get
			{
				bool flag;
				XmlAttribute itemOf = this.m_viewXML.Attributes["IsFromTemplate"];
				flag = (itemOf == null ? false : bool.Parse(itemOf.Value));
				return flag;
			}
		}

		public bool IsWebPartView
		{
			get
			{
				bool flag;
				if (string.IsNullOrEmpty(this.DisplayName))
				{
					flag = false;
				}
				else
				{
					flag = (this.m_viewXML.Attributes["Hidden"] == null ? true : !string.Equals(this.m_viewXML.Attributes["Hidden"].Value, "true", StringComparison.OrdinalIgnoreCase));
				}
				return (flag ? false : true);
			}
		}

		public string Name
		{
			get
			{
				string value;
				if (this.m_viewXML.Attributes["Name"] == null)
				{
					value = null;
				}
				else
				{
					value = this.m_viewXML.Attributes["Name"].Value;
				}
				return value;
			}
		}

		public SPList ParentList
		{
			get
			{
				return this.m_parentList;
			}
		}

		public ViewType Type
		{
			get
			{
				ViewType viewType;
				if (this.m_viewXML.Attributes["Type"] != null)
				{
					string value = this.m_viewXML.Attributes["Type"].Value;
					if (string.Equals(value, "Calendar", StringComparison.OrdinalIgnoreCase))
					{
						viewType = ViewType.Calendar;
					}
					else if (string.Equals(value, "Chart", StringComparison.OrdinalIgnoreCase))
					{
						viewType = ViewType.Chart;
					}
					else if (string.Equals(value, "Gantt", StringComparison.OrdinalIgnoreCase))
					{
						viewType = ViewType.Gantt;
					}
					else if (!string.Equals(value, "Grid", StringComparison.OrdinalIgnoreCase))
					{
						viewType = (!string.Equals(value, "Html", StringComparison.OrdinalIgnoreCase) ? ViewType.Unknown : ViewType.Html);
					}
					else
					{
						viewType = ViewType.Grid;
					}
				}
				else
				{
					viewType = ViewType.Unknown;
				}
				return viewType;
			}
		}

		public string Url
		{
			get
			{
				string value;
				XmlAttribute itemOf = this.m_viewXML.Attributes["Url"];
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
		}

		public XmlNode XML
		{
			get
			{
				return this.m_viewXML;
			}
		}

		public SPView(SPList parentList, XmlNode viewNode)
		{
			this.m_parentList = parentList;
			this.m_viewXML = viewNode;
		}

		public SPView Clone()
		{
			SPView sPView = new SPView(this.m_parentList, this.m_viewXML.CloneNode(true));
			return sPView;
		}
	}
}