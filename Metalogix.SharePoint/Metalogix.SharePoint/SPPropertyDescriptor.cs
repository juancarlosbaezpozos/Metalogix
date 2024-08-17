using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPPropertyDescriptor : PropertyDescriptor
	{
		private static CultureInfo s_CultureInfo;

		private SPField m_field = null;

		private static Int32Converter IntConverter;

		private static System.ComponentModel.DoubleConverter DoubleConverter;

		private static System.ComponentModel.DateTimeConverter DateTimeConverter;

		private static System.ComponentModel.StringConverter StringConverter;

		public override AttributeCollection Attributes
		{
			get
			{
				Attribute[] categoryAttribute = new Attribute[] { new CategoryAttribute(this.m_field.Type), new DescriptionAttribute(this.Description), null, null };
				bool flag = false;
				if (this.m_field.FieldXML.Attributes["Hidden"] != null)
				{
					string value = this.m_field.FieldXML.Attributes["Hidden"].Value;
					if (!bool.TryParse(value, out flag))
					{
						if (value.Contains("True_Unless_Jpn"))
						{
							flag = true;
						}
					}
				}
				categoryAttribute[2] = new IsSystemAttribute(flag);
				categoryAttribute[3] = new NameAttribute(this.m_field.Name);
				return new AttributeCollection(categoryAttribute);
			}
		}

		public override string Category
		{
			get
			{
				return this.m_field.Group;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(SPListItem);
			}
		}

		public override TypeConverter Converter
		{
			get
			{
				return SPPropertyDescriptor.GetConverterFromType(this.PropertyType);
			}
		}

		public override string Description
		{
			get
			{
				return (this.m_field.FieldXML.Attributes["Description"] != null ? this.m_field.FieldXML.Attributes["Description"].Value : "");
			}
		}

		public override string DisplayName
		{
			get
			{
				string str;
				string displayName = this.m_field.DisplayName;
				str = (!string.IsNullOrEmpty(displayName) ? displayName : this.Name);
				return str;
			}
		}

		public string Group
		{
			get
			{
				return this.m_field.Group;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				bool flag;
				flag = (this.m_field.FieldXML.Attributes["ReadOnly"] == null ? false : bool.Parse(this.m_field.FieldXML.Attributes["ReadOnly"].Value));
				return flag;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_field.Name;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return SPPropertyDescriptor.TypeFromString(this.m_field.Type);
			}
		}

		static SPPropertyDescriptor()
		{
			SPPropertyDescriptor.s_CultureInfo = new CultureInfo("en-US");
			SPPropertyDescriptor.IntConverter = new Int32Converter();
			SPPropertyDescriptor.DoubleConverter = new System.ComponentModel.DoubleConverter();
			SPPropertyDescriptor.DateTimeConverter = new System.ComponentModel.DateTimeConverter();
			SPPropertyDescriptor.StringConverter = new System.ComponentModel.StringConverter();
		}

		public SPPropertyDescriptor(SPField field) : base(field.Name, null)
		{
			this.m_field = field;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		private static TypeConverter GetConverterFromType(Type propType)
		{
			TypeConverter intConverter;
			if (propType == typeof(int))
			{
				intConverter = SPPropertyDescriptor.IntConverter;
			}
			else if (propType == typeof(float))
			{
				intConverter = SPPropertyDescriptor.DoubleConverter;
			}
			else if (propType != typeof(DateTime))
			{
				intConverter = SPPropertyDescriptor.StringConverter;
			}
			else
			{
				intConverter = SPPropertyDescriptor.DateTimeConverter;
			}
			return intConverter;
		}

		public override object GetValue(object component)
		{
			float single;
			DateTime dateTime;
			object str;
			if (!(component is ListItem))
			{
				str = null;
			}
			else
			{
				string item = ((ListItem)component)[this.Name];
				if (string.IsNullOrEmpty(item))
				{
					str = null;
				}
				else if (!(this.m_field.Type.ToUpper() == "CHOICE" ? false : !(this.m_field.Type.ToUpper() == "MULTICHOICE")))
				{
					string[] strArrays = new string[] { ";#" };
					string[] strArrays1 = item.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
					StringBuilder stringBuilder = new StringBuilder();
					string[] strArrays2 = strArrays1;
					for (int i = 0; i < (int)strArrays2.Length; i++)
					{
						string str1 = strArrays2[i];
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(str1);
					}
					str = stringBuilder.ToString();
				}
				else if (this.m_field.Type.ToUpper() == "CALCULATED")
				{
					Type type = SPPropertyDescriptor.TypeFromString((this.m_field.FieldXML.Attributes["ResultType"] != null ? this.m_field.FieldXML.Attributes["ResultType"].Value : ""));
					if (type != typeof(string))
					{
						str = (type != typeof(DateTime) ? SPPropertyDescriptor.GetConverterFromType(type).ConvertFromString(null, SPPropertyDescriptor.s_CultureInfo, item) : Utils.ParseDateAsUtc(item));
					}
					else
					{
						if (float.TryParse(item, out single))
						{
							str = single;
						}
						else if (!Utils.TryParseDateAsUtc(item, out dateTime))
						{
							str = item;
						}
						else
						{
							str = dateTime;
						}
					}
				}
				else if ((this.m_field.Type.Equals("DATETIME", StringComparison.OrdinalIgnoreCase) || this.m_field.Type.Equals("PUBLISHINGSCHEDULESTARTDATEFIELDTYPE", StringComparison.OrdinalIgnoreCase) ? false : !this.m_field.Type.Equals("PUBLISHINGSCHEDULEENDDATEFIELDTYPE", StringComparison.OrdinalIgnoreCase)))
				{
					if ((this.m_field.Type.ToUpper() == "ALLDAYEVENT" || this.m_field.Type.ToUpper() == "RECURRENCE" ? true : this.m_field.Type.ToUpper() == "CROSSPROJECTLINK"))
					{
						if (!(item == "0" ? false : !(item.ToUpper() == "FALSE")))
						{
							str = false;
							return str;
						}
						else if ((item == "1" ? true : item.ToUpper() == "TRUE"))
						{
							str = true;
							return str;
						}
					}
					str = this.Converter.ConvertFromString(null, SPPropertyDescriptor.s_CultureInfo, item);
				}
				else
				{
					str = Utils.ParseDateAsUtc(item);
				}
			}
			return str;
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			if (component is SPListItem)
			{
				SPListItem str = (SPListItem)component;
				if (!this.IsReadOnly)
				{
					str[this.Name] = this.Converter.ConvertToString(value);
				}
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public static Type TypeFromString(string sType)
		{
			Type type;
			string upper = sType.ToUpper();
			if (upper != null)
			{
				switch (upper)
				{
					case "TEXT":
					{
						type = typeof(string);
						break;
					}
					case "NOTE":
					{
						type = typeof(string);
						break;
					}
					case "DATETIME":
					case "PUBLISHINGSCHEDULESTARTDATEFIELDTYPE":
					case "PUBLISHINGSCHEDULEENDDATEFIELDTYPE":
					{
						type = typeof(DateTime);
						break;
					}
					case "FILE":
					{
						type = typeof(string);
						break;
					}
					case "NUMBER":
					{
						type = typeof(float);
						break;
					}
					case "INTEGER":
					{
						type = typeof(int);
						break;
					}
					case "URL":
					{
						type = typeof(string);
						break;
					}
					case "ATTACHMENTS":
					{
						type = typeof(string);
						break;
					}
					case "CHOICE":
					{
						type = typeof(string);
						break;
					}
					case "LOOKUP":
					{
						type = typeof(string);
						break;
					}
					case "COMPUTED":
					{
						type = typeof(string);
						break;
					}
					case "IMAGE":
					{
						type = typeof(string);
						break;
					}
					case "CURRENCY":
					{
						type = typeof(float);
						break;
					}
					default:
					{
						type = typeof(string);
						return type;
					}
				}
			}
			else
			{
				type = typeof(string);
				return type;
			}
			return type;
		}
	}
}