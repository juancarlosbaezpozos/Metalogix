using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPField : TypedField, Field, IXmlable
	{
		private XmlNode m_fieldNode;

		private Guid? m_id;

		public bool AllowsGroups
		{
			get
			{
				bool flag;
				bool flag1 = false;
				if ((this.Type == "User" ? true : this.Type == "UserMulti"))
				{
					XmlAttribute itemOf = this.FieldXML.Attributes["UserSelectionMode"];
					if (itemOf == null || !(itemOf.Value == "PeopleAndGroups"))
					{
						flag = (itemOf != null || this.FieldXML.Attributes["ColName"] == null ? true : this.FieldXML.Attributes["ColName"].Value.StartsWith("tp_"));
					}
					else
					{
						flag = false;
					}
					if (!flag)
					{
						flag1 = true;
					}
				}
				return flag1;
			}
		}

		public Guid AnchorId
		{
			get
			{
				Guid empty;
				if (this.IsTaxonomyField)
				{
					XmlNode xmlNodes = this.FieldXML.SelectSingleNode(".//ArrayOfProperty/Property[Name='AnchorId']/Value");
					empty = (XmlExtensions.ContainsInnerText(xmlNodes) ? new Guid(xmlNodes.InnerText) : Guid.Empty);
				}
				else
				{
					empty = Guid.Empty;
				}
				return empty;
			}
		}

		public string ColName
		{
			get
			{
				string value;
				if (this.m_fieldNode.Attributes["ColName"] != null)
				{
					value = this.m_fieldNode.Attributes["ColName"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public string DisplayName
		{
			get
			{
				string value;
				if (this.m_fieldNode.Attributes["DisplayName"] != null)
				{
					value = this.m_fieldNode.Attributes["DisplayName"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public XmlNode FieldXML
		{
			get
			{
				return this.m_fieldNode;
			}
		}

		public bool FromBaseType
		{
			get
			{
				return (this.m_fieldNode.Attributes["FromBaseType"] != null ? this.m_fieldNode.Attributes["FromBaseType"].Value.ToLower() == "true" : false);
			}
		}

		public string Group
		{
			get
			{
				string value;
				if (this.m_fieldNode.Attributes["Group"] != null)
				{
					value = this.m_fieldNode.Attributes["Group"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public bool Hidden
		{
			get
			{
				return (this.m_fieldNode.Attributes["Hidden"] != null ? this.m_fieldNode.Attributes["Hidden"].Value.ToLower() == "true" : false);
			}
		}

		public Guid ID
		{
			get
			{
				if (!this.m_id.HasValue)
				{
					this.m_id = new Guid?((this.m_fieldNode.Attributes["ID"] == null ? Guid.Empty : new Guid(this.m_fieldNode.Attributes["ID"].Value)));
				}
				return this.m_id.Value;
			}
		}

		public bool IsChoiceField
		{
			get
			{
				return (this.Type == "Choice" ? true : this.Type == "MultiChoice");
			}
		}

		public bool IsLookupField
		{
			get
			{
				return (this.Type == "Lookup" ? true : this.Type == "LookupMulti");
			}
		}

		public bool IsLookupMultiField
		{
			get
			{
				return this.Type == "LookupMulti";
			}
		}

		public bool IsMultiChoiceField
		{
			get
			{
				return this.Type == "MultiChoice";
			}
		}

		public bool IsReadOnly
		{
			get
			{
				bool flag;
				bool flag1 = (this.m_fieldNode.Attributes["ReadOnly"] == null ? false : this.m_fieldNode.Attributes["ReadOnly"].Value.ToLower() == "true");
				if ((flag1 ? false : this.Hidden))
				{
					if (this.FromBaseType)
					{
						string lower = this.Type.ToLower();
						if ((lower == "guid" ? true : lower == "threading"))
						{
							flag = false;
							return flag;
						}
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				flag = (flag1 || this.Hidden || this.Type == "Computed" ? true : this.Type == "File");
				return flag;
			}
		}

		public bool IsRichText
		{
			get
			{
				return (this.m_fieldNode.Attributes["RichText"] == null ? false : string.Equals(this.m_fieldNode.Attributes["RichText"].Value, "true", StringComparison.InvariantCultureIgnoreCase));
			}
		}

		public bool IsTaxonomyField
		{
			get
			{
				return (this.Type == "TaxonomyFieldType" ? true : this.Type == "TaxonomyFieldTypeMulti");
			}
		}

		public bool IsUrlField
		{
			get
			{
				return this.Type == "URL";
			}
		}

		public bool IsUserField
		{
			get
			{
				return (this.Type == "User" ? true : this.Type == "UserMulti");
			}
		}

		public string Name
		{
			get
			{
				return this.m_fieldNode.Attributes["Name"].Value;
			}
		}

		public bool Required
		{
			get
			{
				bool flag = false;
				XmlAttribute itemOf = this.FieldXML.Attributes["Required"];
				return ((itemOf == null ? true : !bool.TryParse(itemOf.Value, out flag)) ? false : flag);
			}
		}

		public Guid TaxonomyHiddenTextField
		{
			get
			{
				Guid empty;
				if (this.IsTaxonomyField)
				{
					XmlNode xmlNodes = this.FieldXML.SelectSingleNode(".//ArrayOfProperty/Property[Name='TextField']/Value");
					empty = (XmlExtensions.ContainsInnerText(xmlNodes) ? new Guid(xmlNodes.InnerText) : Guid.Empty);
				}
				else
				{
					empty = Guid.Empty;
				}
				return empty;
			}
		}

		public Guid TermSetId
		{
			get
			{
				Guid empty;
				if (this.IsTaxonomyField)
				{
					XmlNode xmlNodes = this.FieldXML.SelectSingleNode(".//ArrayOfProperty/Property[Name='TermSetId']/Value");
					empty = (XmlExtensions.ContainsInnerText(xmlNodes) ? new Guid(xmlNodes.InnerText) : Guid.Empty);
				}
				else
				{
					empty = Guid.Empty;
				}
				return empty;
			}
		}

		public Guid TermstoreId
		{
			get
			{
				Guid empty;
				if (this.IsTaxonomyField)
				{
					XmlNode xmlNodes = this.FieldXML.SelectSingleNode(".//ArrayOfProperty/Property[Name='SspId']/Value");
					empty = (XmlExtensions.ContainsInnerText(xmlNodes) ? new Guid(xmlNodes.InnerText) : Guid.Empty);
				}
				else
				{
					empty = Guid.Empty;
				}
				return empty;
			}
		}

		public string Type
		{
			get
			{
				return this.m_fieldNode.Attributes["Type"].Value;
			}
		}

		public System.Type UnderlyingType
		{
			get
			{
				System.Type type;
				System.Type type1 = typeof(string);
				string str = this.Type;
				if (str != null)
				{
					switch (str)
					{
						case "Counter":
						case "Integer":
						{
							type1 = typeof(int);
							type = type1;
							return type;
						}
						case "Number":
						{
							type1 = typeof(double);
							type = type1;
							return type;
						}
						case "DateTime":
						case "PublishingScheduleStartDateFieldType":
						case "PublishingScheduleEndDateFieldType":
						{
							type1 = typeof(DateTime);
							type = type1;
							return type;
						}
					}
				}
				type1 = typeof(string);
				type = type1;
				return type;
			}
		}

		public SPField(XmlNode fieldNode)
		{
			this.m_fieldNode = fieldNode;
		}

		private static string EncodeDisplayNameCharacter(char c)
		{
			string str = string.Concat("_x", string.Format("{0:x4}", (int)c), "_");
			return str;
		}

		public static string GetInternalNameFromDisplayName(string sFieldName)
		{
			string str;
			if (!string.IsNullOrEmpty(sFieldName))
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				int num = 0;
				int num1 = -1;
				if (int.TryParse(sFieldName[0].ToString(), out num1))
				{
					flag = true;
				}
				bool flag1 = false;
				int num2 = 0;
				for (int i = 0; i < sFieldName.Length; i++)
				{
					if (flag1)
					{
						if ((!SPField.IsNumber(sFieldName[i]) ? false : num2 != 5))
						{
							num2++;
							if (i + 1 == sFieldName.Length)
							{
								flag = true;
								break;
							}
						}
						else
						{
							break;
						}
					}
					else if (!SPField.IsLetter(sFieldName[i]))
					{
						if (!SPField.IsNumber(sFieldName[i]))
						{
							break;
						}
						else
						{
							flag1 = true;
							num2 = 1;
							if (i + 1 == sFieldName.Length)
							{
								flag = true;
							}
						}
					}
					else if (i == 3)
					{
						break;
					}
				}
				if (flag)
				{
					stringBuilder.Append(SPField.EncodeDisplayNameCharacter(sFieldName[0]));
					num = 1;
				}
				while (num < sFieldName.Length)
				{
					if ((SPField.IsLetter(sFieldName[num]) || SPField.IsNumber(sFieldName[num]) ? false : sFieldName[num] != '\u005F'))
					{
						stringBuilder.Append(SPField.EncodeDisplayNameCharacter(sFieldName[num]));
					}
					else
					{
						char chr = sFieldName[num];
						stringBuilder.Append(chr.ToString());
					}
					num++;
				}
				string str1 = stringBuilder.ToString();
				if (str1.Length > 32)
				{
					str1 = str1.Substring(0, 32);
				}
				str = str1;
			}
			else
			{
				str = sFieldName;
			}
			return str;
		}

		public string[] GetSecondaryFieldNames()
		{
			return this.ParseBcsAttributeString("SecondaryFieldBdcNames");
		}

		public string[] GetSecondaryFieldWssNames()
		{
			return this.ParseBcsAttributeString("SecondaryFieldWssNames");
		}

		private static bool IsLetter(char c)
		{
			bool flag;
			if (c < 'a' || c > 'z')
			{
				flag = (c < 'A' ? false : c <= 'Z');
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private static bool IsNumber(char c)
		{
			return (c < '0' ? false : c <= '9');
		}

		protected string[] ParseBcsAttributeString(string bcsAttributeName)
		{
			int num;
			string[] array;
			if (!(this.Type != "BusinessData"))
			{
				XmlAttribute itemOf = this.FieldXML.Attributes[bcsAttributeName];
				if ((itemOf == null ? false : !string.IsNullOrEmpty(itemOf.Value)))
				{
					string str = Uri.UnescapeDataString(itemOf.Value);
					char[] chrArray = new char[] { ' ' };
					string[] strArrays = str.Split(chrArray);
					string str1 = strArrays[(int)strArrays.Length - 1];
					if (str1 == "0")
					{
						array = new string[0];
					}
					else if (int.TryParse(str1, out num))
					{
						string str2 = str.Substring(num);
						chrArray = new char[] { ' ' };
						List<string> strs = new List<string>(str2.Split(chrArray));
						if (strs.Count >= 2)
						{
							strs.RemoveAt(strs.Count - 1);
							array = strs.ToArray();
						}
						else
						{
							array = null;
						}
					}
					else
					{
						array = null;
					}
				}
				else
				{
					array = null;
				}
			}
			else
			{
				array = null;
			}
			return array;
		}

		internal void SetReferencedManagedMetadata(XmlWriter xWriter, string val)
		{
			xWriter.WriteStartElement("TaxonomyColumn");
			xWriter.WriteAttributeString("Name", this.Name);
			xWriter.WriteAttributeString("Value", val);
			xWriter.WriteAttributeString("TermStoreId", this.FieldXML.SelectSingleNode(".//Property[Name='SspId']/Value").InnerText);
			xWriter.WriteAttributeString("AnchorId", this.FieldXML.SelectSingleNode(".//Property[Name='AnchorId']/Value").InnerText);
			xWriter.WriteAttributeString("TermSetId", this.FieldXML.SelectSingleNode(".//Property[Name='TermSetId']/Value").InnerText);
			xWriter.WriteEndElement();
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", this.DisplayName, this.Name);
		}

		public string ToXML()
		{
			string str;
			StringWriter stringWriter = new StringWriter();
			try
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				try
				{
					this.ToXML(xmlTextWriter);
				}
				finally
				{
					if (xmlTextWriter != null)
					{
						((IDisposable)xmlTextWriter).Dispose();
					}
				}
				str = stringWriter.ToString();
			}
			finally
			{
				if (stringWriter != null)
				{
					((IDisposable)stringWriter).Dispose();
				}
			}
			return str;
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			this.m_fieldNode.WriteTo(xmlWriter);
		}

		public class XMLNames
		{
			public const string Name = "Name";

			public const string Type = "Type";

			public const string Group = "Group";

			public const string DisplayName = "DisplayName";

			public const string FromBaseType = "FromBaseType";

			public const string Hidden = "Hidden";

			public const string ReadOnly = "ReadOnly";

			public const string RichText = "RichText";

			public const string SourceID = "SourceID";

			public const string ID = "ID";

			public const string ColName = "ColName";

			public const string Required = "Required";

			public const string FieldRef = "FieldRef";

			public const string CustomColumnXPATH = "./Field[@SourceID and not(starts-with(@SourceID, 'http://'))]";

			public const string Exclude = "Exclude";

			public XMLNames()
			{
			}
		}
	}
}