using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPAlert
	{
		private readonly XmlNode _alertXML;

		private IDictionary<string, string> _properties;

		public string AlertFrequency
		{
			get
			{
				return this._alertXML.Attributes["AlertFrequency"].Value;
			}
		}

		public string AlertTemplate
		{
			get
			{
				return this._alertXML.Attributes["AlertTemplate"].Value;
			}
		}

		public string AlertTime
		{
			get
			{
				string value;
				if (this._alertXML.Attributes["AlertTime"] != null)
				{
					value = this._alertXML.Attributes["AlertTime"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public SPAlertType AlertType
		{
			get
			{
				SPAlertType sPAlertType = SPAlertType.List;
				if ((this._alertXML.Attributes["AlertType"] == null ? false : this._alertXML.Attributes["AlertType"].Value.Equals("Item", StringComparison.OrdinalIgnoreCase)))
				{
					sPAlertType = SPAlertType.Item;
				}
				return sPAlertType;
			}
		}

		public string DynamicRecipient
		{
			get
			{
				return this._alertXML.Attributes["DynamicRecipient"].Value;
			}
		}

		public string EventType
		{
			get
			{
				return this._alertXML.Attributes["EventType"].Value;
			}
		}

		public string Filter
		{
			get
			{
				string value;
				if (this._alertXML.Attributes["Filter"] != null)
				{
					value = this._alertXML.Attributes["Filter"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public string ItemGUID
		{
			get
			{
				string value;
				if (this._alertXML.Attributes["ItemGUID"] != null)
				{
					value = this._alertXML.Attributes["ItemGUID"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
			set
			{
				this._alertXML.Attributes["ItemGUID"].Value = value;
			}
		}

		public string ItemID
		{
			get
			{
				string value;
				if (this._alertXML.Attributes["ItemID"] != null)
				{
					value = this._alertXML.Attributes["ItemID"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
			set
			{
				this._alertXML.Attributes["ItemID"].Value = value;
			}
		}

		public string ListID
		{
			get
			{
				return this._alertXML.Attributes["ListID"].Value;
			}
			set
			{
				this._alertXML.Attributes["ListID"].Value = value;
			}
		}

		public IDictionary<string, string> Properties
		{
			get
			{
				IDictionary<string, string> strs;
				if (this._properties == null)
				{
					Dictionary<string, string> strs1 = new Dictionary<string, string>();
					XmlNode xmlNodes = this._alertXML.SelectSingleNode(".//PropertyBag");
					if (xmlNodes == null)
					{
						this._properties = strs1;
						strs = this._properties;
						return strs;
					}
					foreach (XmlAttribute attribute in xmlNodes.Attributes)
					{
						strs1.Add(attribute.Name, attribute.Value);
					}
					this._properties = strs1;
				}
				strs = this._properties;
				return strs;
			}
		}

		public string Status
		{
			get
			{
				return this._alertXML.Attributes["Status"].Value;
			}
		}

		public string Title
		{
			get
			{
				return this._alertXML.Attributes["Title"].Value;
			}
		}

		public string User
		{
			get
			{
				return this._alertXML.Attributes["User"].Value;
			}
		}

		public string UserDetails
		{
			get
			{
				string outerXml;
				XmlNode xmlNodes = this._alertXML.SelectSingleNode("./UserDetails");
				if (xmlNodes != null)
				{
					outerXml = xmlNodes.OuterXml;
				}
				else
				{
					outerXml = null;
				}
				return outerXml;
			}
		}

		public string XML
		{
			get
			{
				return this._alertXML.OuterXml;
			}
		}

		public SPAlert(XmlNode alertXML)
		{
			this._alertXML = alertXML;
		}

		public bool AlertEqualsXML(XmlNode node)
		{
			bool flag;
			string[] strArrays = new string[] { "User", "AlertType", "Title", "Filter", "Status", "DynamicRecipient", "AlertFrequency" };
			string[] strArrays1 = strArrays;
			int num = 0;
			while (true)
			{
				if (num < (int)strArrays1.Length)
				{
					string str = strArrays1[num];
					if (!(node.Attributes[str].Value != this._alertXML.Attributes[str].Value))
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}
	}
}