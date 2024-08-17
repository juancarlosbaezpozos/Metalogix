using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.SharePoint.Options.Migration.Mapping
{
	public class ColumnMappings : MappingsCollection
	{
		private bool m_bAutoMapCreatedAndModified;

		private List<ListSummaryItem> m_totalMappings;

		private IFilterExpression m_fieldsFilter = new FilterExpressionList(ExpressionLogic.And);

		public bool AutoMapCreatedAndModified
		{
			get
			{
				return this.m_bAutoMapCreatedAndModified;
			}
			set
			{
				this.m_bAutoMapCreatedAndModified = value;
				this.m_totalMappings = null;
			}
		}

		public bool ContainsColumnAdditions
		{
			get
			{
				bool flag;
				List<ListSummaryItem>.Enumerator enumerator = this.TotalMappings.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Target.IsNew)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
		}

		public bool ContainsColumnChanges
		{
			get
			{
				if (this.TotalMappings.Count > 0)
				{
					return true;
				}
				return this.FieldsFilter != null;
			}
		}

		public IFilterExpression FieldsFilter
		{
			get
			{
				return this.m_fieldsFilter;
			}
			set
			{
				this.m_fieldsFilter = value;
			}
		}

		public List<ListSummaryItem> TotalMappings
		{
			get
			{
				if (this.m_totalMappings == null)
				{
					List<ListSummaryItem> privateMappings = this.GetPrivateMappings();
					this.m_totalMappings = new List<ListSummaryItem>(base.Count + privateMappings.Count);
					this.m_totalMappings.AddRange(this);
					foreach (ListSummaryItem privateMapping in privateMappings)
					{
						bool flag = false;
						foreach (ListSummaryItem mTotalMapping in this.m_totalMappings)
						{
							if (mTotalMapping.Target.Target != privateMapping.Target.Target)
							{
								continue;
							}
							flag = true;
							break;
						}
						if (flag)
						{
							continue;
						}
						this.m_totalMappings.Add(privateMapping);
					}
				}
				return this.m_totalMappings;
			}
		}

		public ColumnMappings()
		{
			this.Initialize();
		}

		public ColumnMappings(ListSummaryItem[] items) : base(items)
		{
			this.Initialize();
		}

		public ColumnMappings(XmlNode node) : base(node)
		{
			this.Initialize();
		}

		private bool AddNewFieldsToListXML(XmlNode listXml, SPList sourceList, SPWeb targetWeb)
		{
			XmlNode xmlNodes = listXml.SelectSingleNode("./Fields");
			if (xmlNodes == null)
			{
				return false;
			}
			List<ListSummaryItem> listSummaryItems = new List<ListSummaryItem>();
			bool flag = false;
			foreach (ListSummaryItem totalMapping in this.TotalMappings)
			{
				if (totalMapping.Target.IsNew)
				{
					if (listXml.SelectSingleNode(string.Concat("./Fields/Field[@Name=\"", SPField.GetInternalNameFromDisplayName(totalMapping.Target.Target), "\"]")) != null)
					{
						continue;
					}
					listSummaryItems.Add(totalMapping);
				}
				else
				{
					SPField tag = totalMapping.Target.Tag as SPField;
					if (tag == null)
					{
						continue;
					}
					string name = tag.Name;
					XmlNode xmlNodes1 = listXml.SelectSingleNode(string.Concat("./Fields/Field[@Name=\"", name, "\"]"));
					XmlNode xmlNodes2 = XmlUtility.CloneXMLNodeForTarget(tag.FieldXML, xmlNodes, true);
					if (xmlNodes1 == null)
					{
						xmlNodes.AppendChild(xmlNodes2);
					}
					else
					{
						xmlNodes.ReplaceChild(xmlNodes2, xmlNodes1);
					}
					flag = true;
					SPField sPField = totalMapping.Source.Tag as SPField;
					if (sPField == null || sPField.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					XmlNodeList xmlNodeLists = listXml.SelectNodes(string.Concat(".//Views/View/ViewFields/FieldRef[@Name=\"", sPField.Name, "\"]"));
					foreach (XmlNode xmlNodes3 in xmlNodeLists)
					{
						XmlNode xmlNodes4 = xmlNodes3.OwnerDocument.CreateElement("FieldRef");
						XmlAttribute xmlAttribute = xmlNodes3.OwnerDocument.CreateAttribute("Name");
						xmlAttribute.Value = name;
						xmlNodes4.Attributes.Append(xmlAttribute);
						xmlNodes3.ParentNode.InsertAfter(xmlNodes4, xmlNodes3);
						flag = true;
					}
				}
			}
			foreach (ListSummaryItem listSummaryItem in listSummaryItems)
			{
				string internalNameFromDisplayName = SPField.GetInternalNameFromDisplayName(listSummaryItem.Target.Target);
				string target = listSummaryItem.Source.Target;
				XmlNode str = null;
				foreach (SPField field in (SPFieldCollection)sourceList.Fields)
				{
					if (field.Name != target)
					{
						continue;
					}
					str = field.FieldXML.Clone();
					break;
				}
				if (str == null)
				{
					continue;
				}
				bool flag1 = false;
				if (str.Attributes["DisplayName"] == null)
				{
					XmlAttribute target1 = str.OwnerDocument.CreateAttribute("DisplayName");
					str.Attributes.Append(target1);
					target1.Value = listSummaryItem.Target.Target;
				}
				else if (str.Attributes["DisplayName"].Value != listSummaryItem.Target.Target)
				{
					str.Attributes["DisplayName"].Value = listSummaryItem.Target.Target;
				}
				else
				{
					flag1 = true;
				}
				if (!flag1 || str.Attributes["Name"] == null)
				{
					if (str.Attributes["Name"] == null)
					{
						XmlAttribute xmlAttribute1 = str.OwnerDocument.CreateAttribute("Name");
						str.Attributes.Append(xmlAttribute1);
						xmlAttribute1.Value = internalNameFromDisplayName;
					}
					else
					{
						str.Attributes["Name"].Value = internalNameFromDisplayName;
					}
				}
				if (!flag1 || str.Attributes["StaticName"] == null)
				{
					if (str.Attributes["StaticName"] == null)
					{
						XmlAttribute xmlAttribute2 = str.OwnerDocument.CreateAttribute("StaticName");
						str.Attributes.Append(xmlAttribute2);
						xmlAttribute2.Value = internalNameFromDisplayName;
					}
					else
					{
						str.Attributes["StaticName"].Value = internalNameFromDisplayName;
					}
				}
				if (str.Attributes["ID"] == null)
				{
					XmlAttribute str1 = str.OwnerDocument.CreateAttribute("ID");
					str.Attributes.Append(str1);
					str1.Value = Guid.NewGuid().ToString();
				}
				else
				{
					str.Attributes["ID"].Value = Guid.NewGuid().ToString();
				}
				ColumnMappings.RemoveAttribute(str, "Hidden");
				ColumnMappings.RemoveAttribute(str, "ReadOnly");
				if (listSummaryItem.Target.CustomColumns.ContainsKey("SiteColumnGroup") && !string.IsNullOrEmpty((string)listSummaryItem.Target.CustomColumns["SiteColumnGroup"]))
				{
					XmlAttribute itemOf = str.Attributes["AddToSiteColumnGroup"] ?? str.OwnerDocument.CreateAttribute("AddToSiteColumnGroup");
					itemOf.Value = listSummaryItem.Target.CustomColumns["SiteColumnGroup"].ToString();
					str.Attributes.Append(itemOf);
					if (listSummaryItem.Target.CustomColumns.ContainsKey("ContentType") && !string.IsNullOrEmpty((string)listSummaryItem.Target.CustomColumns["ContentType"]))
					{
						XmlAttribute itemOf1 = str.Attributes["AddToContentType"] ?? str.OwnerDocument.CreateAttribute("AddToContentType");
						itemOf1.Value = listSummaryItem.Target.CustomColumns["ContentType"].ToString();
						str.Attributes.Append(itemOf1);
					}
				}
				if (listSummaryItem.Target.CustomColumns.ContainsKey("ChangeTypeTo") && !string.IsNullOrEmpty((string)listSummaryItem.Target.CustomColumns["ChangeTypeTo"]))
				{
					XmlAttribute item = str.Attributes["Type"];
					if (item != null)
					{
						item.Value = (string)listSummaryItem.Target.CustomColumns["ChangeTypeTo"];
						if (item.Value == "Text" || item.Value == "Note" || item.Value == "Number")
						{
							XmlAttribute itemOf2 = str.Attributes["List"];
							if (itemOf2 != null)
							{
								str.Attributes.Remove(itemOf2);
							}
						}
					}
				}
				ColumnMappings.AppendNodeToTarget(str, xmlNodes);
				flag = true;
			}
			return flag;
		}

		public static void AppendNodeToTarget(XmlNode sourceNode, XmlNode targetNode)
		{
			XmlNode innerXml = targetNode.OwnerDocument.CreateElement(sourceNode.Name);
			foreach (XmlAttribute attribute in sourceNode.Attributes)
			{
				XmlAttribute value = innerXml.OwnerDocument.CreateAttribute(attribute.Name);
				value.Value = attribute.Value;
				innerXml.Attributes.Append(value);
			}
			innerXml.InnerXml = sourceNode.InnerXml;
			targetNode.AppendChild(innerXml);
		}

		public override void FromXML(XmlNode xmlNode)
		{
			XmlNode xmlNodes = (xmlNode.Name == "ColumnMappingsList" ? xmlNode : xmlNode.SelectSingleNode(".//ColumnMappingsList"));
			if (xmlNodes == null)
			{
				return;
			}
			XmlAttribute itemOf = xmlNodes.Attributes["AutoMapCreatedAndModified"];
			if (itemOf != null)
			{
				this.AutoMapCreatedAndModified = bool.Parse(itemOf.Value);
			}
			XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./FieldsFilter");
			if (xmlNodes1 != null)
			{
				this.FieldsFilter = FilterExpression.ParseExpression(xmlNodes1.ChildNodes[0]);
			}
			XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./XmlableSet");
			if (xmlNodes2 != null)
			{
				base.FromXML(xmlNodes2);
			}
		}
        
	    private List<ListSummaryItem> GetPrivateMappings()
	    {
	        List<ListSummaryItem> list = new List<ListSummaryItem>();
	        if (this.AutoMapCreatedAndModified)
	        {
	            ListSummaryItem listSummaryItem = new ListSummaryItem();
	            ListPickerItem listPickerItem = new ListPickerItem();
	            listPickerItem.Target = "Author";
	            listPickerItem.TargetType = "User";
	            ListPickerItem listPickerItem2 = new ListPickerItem();
	            listPickerItem2.Target = "Created By (Migration)";
	            listPickerItem2.TargetType = "User";
	            listPickerItem2.CustomColumns.Add(new KeyValuePair<string, object>("ChangeTypeTo", "Text"));
	            listPickerItem2.Tag = listPickerItem2;
	            listSummaryItem.Source = listPickerItem;
	            listSummaryItem.Target = listPickerItem2;
	            ListSummaryItem listSummaryItem2 = new ListSummaryItem();
	            ListPickerItem listPickerItem3 = new ListPickerItem();
	            listPickerItem3.Target = "Editor";
	            listPickerItem3.TargetType = "User";
	            ListPickerItem listPickerItem4 = new ListPickerItem();
	            listPickerItem4.Target = "Modified By (Migration)";
	            listPickerItem4.TargetType = "User";
	            listPickerItem4.CustomColumns.Add(new KeyValuePair<string, object>("ChangeTypeTo", "Text"));
	            listPickerItem4.Tag = listPickerItem4;
	            listSummaryItem2.Source = listPickerItem3;
	            listSummaryItem2.Target = listPickerItem4;
	            ListSummaryItem listSummaryItem3 = new ListSummaryItem();
	            ListPickerItem listPickerItem5 = new ListPickerItem();
	            listPickerItem5.Target = "Created";
	            listPickerItem5.TargetType = "DateTime";
	            ListPickerItem listPickerItem6 = new ListPickerItem();
	            listPickerItem6.Target = "Created (Migration)";
	            listPickerItem6.TargetType = "DateTime";
	            listPickerItem6.Tag = listPickerItem6;
	            listSummaryItem3.Source = listPickerItem5;
	            listSummaryItem3.Target = listPickerItem6;
	            ListSummaryItem listSummaryItem4 = new ListSummaryItem();
	            ListPickerItem listPickerItem7 = new ListPickerItem();
	            listPickerItem7.Target = "Modified";
	            listPickerItem7.TargetType = "DateTime";
	            ListPickerItem listPickerItem8 = new ListPickerItem();
	            listPickerItem8.Target = "Modified (Migration)";
	            listPickerItem8.TargetType = "DateTime";
	            listPickerItem8.Tag = listPickerItem8;
	            listSummaryItem4.Source = listPickerItem7;
	            listSummaryItem4.Target = listPickerItem8;
	            list.Add(listSummaryItem);
	            list.Add(listSummaryItem2);
	            list.Add(listSummaryItem3);
	            list.Add(listSummaryItem4);
	        }
	        return list;
	    }

        private string GetUserAsString(string sUser, SPFolder sourceFolder)
		{
			if (sourceFolder == null || sourceFolder.ParentList == null || sourceFolder.ParentList.ParentWeb == null || sourceFolder.ParentList.ParentWeb.SiteUsers == null)
			{
				return sUser;
			}
			SPUser byLoginName = sourceFolder.ParentList.ParentWeb.SiteUsers.GetByLoginName(sUser);
			if (byLoginName == null)
			{
				return sUser;
			}
			return string.Concat(byLoginName.LoginName, " (", byLoginName.Name, ")");
		}

		private void HandleCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			this.m_totalMappings = null;
		}

		private void Initialize()
		{
			base.OnCollectionChanged += new CollectionChangeEventHandler(this.HandleCollectionChanged);
		}

		public static ColumnMappings MergeMappings(ColumnMappings mappings1, ColumnMappings mappings2)
		{
			if (mappings1 == null)
			{
				if (mappings2 == null)
				{
					return null;
				}
				return (ColumnMappings)mappings2.Clone();
			}
			if (mappings2 == null)
			{
				return (ColumnMappings)mappings1.Clone();
			}
			ColumnMappings columnMapping = new ColumnMappings();
			columnMapping.AddRange(mappings1.ToArray());
			foreach (ListSummaryItem listSummaryItem in mappings2)
			{
				bool flag = false;
				foreach (ListSummaryItem listSummaryItem1 in mappings1)
				{
					if (listSummaryItem.Target.Target != listSummaryItem1.Target.Target)
					{
						continue;
					}
					flag = true;
					break;
				}
				if (flag)
				{
					continue;
				}
				columnMapping.Add(listSummaryItem);
			}
			columnMapping.AutoMapCreatedAndModified = (mappings1.AutoMapCreatedAndModified ? true : mappings2.AutoMapCreatedAndModified);
			if (mappings1.FieldsFilter != null)
			{
				if (mappings2.FieldsFilter == null)
				{
					columnMapping.FieldsFilter = mappings1.FieldsFilter;
				}
				else
				{
					FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.And)
					{
						mappings1.FieldsFilter,
						mappings2.FieldsFilter
					};
					columnMapping.FieldsFilter = filterExpressionList;
				}
			}
			else if (mappings2.FieldsFilter != null)
			{
				columnMapping.FieldsFilter = mappings2.FieldsFilter;
			}
			return columnMapping;
		}

		public bool ModifyListItemXML(ref string sItemXml, SPFolder sourceFolder, SPFolder targetFolder, bool bFieldFilteringEnabled)
		{
			if (string.IsNullOrEmpty(sItemXml) || !this.ContainsColumnChanges)
			{
				return false;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sItemXml);
			bool flag = this.ModifyListItemXML(xmlDocument.DocumentElement, sourceFolder, targetFolder, bFieldFilteringEnabled);
			sItemXml = xmlDocument.DocumentElement.OuterXml;
			return flag;
		}

		public bool ModifyListItemXML(ref string sItemXml, SPFolder targetFolder, bool bFieldFilteringEnabled)
		{
			return this.ModifyListItemXML(ref sItemXml, null, targetFolder, bFieldFilteringEnabled);
		}

		public bool ModifyListItemXML(XmlNode itemXml, SPFolder sourceFolder, SPFolder targetFolder, bool bFieldFilteringEnabled)
		{
			if (itemXml == null || !this.ContainsColumnChanges)
			{
				return false;
			}
			SPList sPList = (targetFolder is SPList ? (SPList)targetFolder : targetFolder.ParentList);
			List<XmlAttribute> xmlAttributes = new List<XmlAttribute>();
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (ListSummaryItem totalMapping in this.TotalMappings)
			{
				string target = totalMapping.Source.Target;
				string internalNameFromDisplayName = totalMapping.Target.Target;
				if (totalMapping.Target.IsNew)
				{
					internalNameFromDisplayName = SPField.GetInternalNameFromDisplayName(internalNameFromDisplayName);
				}
				bool flag = false;
				foreach (SPField field in (SPFieldCollection)sPList.Fields)
				{
					if (internalNameFromDisplayName != field.Name)
					{
						continue;
					}
					flag = true;
					break;
				}
				if (!flag)
				{
					continue;
				}
				string str = XmlUtility.EncodeNameStartChars(target);
				XmlUtility.EncodeNameStartChars(internalNameFromDisplayName);
				XmlAttribute itemOf = itemXml.Attributes[str];
				if (itemOf == null)
				{
					continue;
				}
				string value = itemOf.Value;
				if (totalMapping.Source.TargetType == "User" && totalMapping.Target.CustomColumns.ContainsKey("ChangeTypeTo") && (string)totalMapping.Target.CustomColumns["ChangeTypeTo"] == "Text")
				{
					value = this.GetUserAsString(value, sourceFolder);
				}
				strs.Add(internalNameFromDisplayName, value);
				if (xmlAttributes.Contains(itemOf))
				{
					continue;
				}
				xmlAttributes.Add(itemOf);
			}
			foreach (XmlAttribute xmlAttribute in xmlAttributes)
			{
				itemXml.Attributes.Remove(xmlAttribute);
			}
			foreach (string key in strs.Keys)
			{
				XmlAttribute item = itemXml.Attributes[key];
				if (item == null)
				{
					item = itemXml.OwnerDocument.CreateAttribute(key);
					itemXml.Attributes.Append(item);
				}
				item.Value = strs[key];
			}
			bool flag1 = false;
			if (bFieldFilteringEnabled)
			{
				flag1 = this.RemoveFilteredFieldsFromListItemXml(itemXml, sourceFolder);
			}
			if (strs.Count <= 0)
			{
				return flag1;
			}
			return true;
		}

		public bool ModifyListXML(ref string sListXml, SPList sourceList, SPWeb targetWeb, bool bFieldFilteringEnabled)
		{
			return this.ModifyListXML(ref sListXml, sourceList, targetWeb, bFieldFilteringEnabled, false);
		}

		public bool ModifyListXML(ref string sListXml, SPList sourceList, SPWeb targetWeb, bool bFieldFilteringEnabled, bool bRemoveMappedFields)
		{
			if (string.IsNullOrEmpty(sListXml) || !this.ContainsColumnChanges)
			{
				return false;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sListXml);
			bool flag = this.ModifyListXML(xmlDocument.DocumentElement, sourceList, targetWeb, bFieldFilteringEnabled, bRemoveMappedFields);
			sListXml = xmlDocument.DocumentElement.OuterXml;
			return flag;
		}

		public bool ModifyListXML(XmlNode listXml, SPList sourceList, SPWeb targetWeb, bool bFieldFilteringEnabled)
		{
			return this.ModifyListXML(listXml, sourceList, targetWeb, bFieldFilteringEnabled, false);
		}

		public bool ModifyListXML(XmlNode listXml, SPList sourceList, SPWeb targetWeb, bool bFieldFilteringEnabled, bool bRemoveMappedFields)
		{
			if (listXml == null || !this.ContainsColumnChanges)
			{
				return false;
			}
			bool listXML = this.AddNewFieldsToListXML(listXml, sourceList, targetWeb);
			if (bFieldFilteringEnabled)
			{
				listXML = (this.RemoveFilteredFieldsFromListXML(listXml, sourceList) ? true : listXML);
			}
			if (bRemoveMappedFields)
			{
				listXML = (this.RemoveMappedFieldsFromListXML(listXml) ? true : listXML);
			}
			return listXML;
		}

		public static void RemoveAttribute(XmlNode node, string sAttributeName)
		{
			XmlAttribute itemOf = node.Attributes[sAttributeName];
			if (itemOf != null)
			{
				node.Attributes.Remove(itemOf);
			}
		}

		private static bool RemoveField(XmlNode listXml, SPField field)
		{
			bool flag = false;
			XmlNode xmlNodes = listXml.SelectSingleNode(string.Concat("./Fields/Field[@Name=\"", field.Name, "\"]"));
			if (xmlNodes != null)
			{
				xmlNodes.ParentNode.RemoveChild(xmlNodes);
				flag = true;
			}
			XmlNodeList xmlNodeLists = listXml.SelectNodes(string.Concat(".//Views/View/ViewFields/FieldRef[@Name=\"", field.Name, "\"]"));
			foreach (XmlNode xmlNodes1 in xmlNodeLists)
			{
				xmlNodes1.ParentNode.RemoveChild(xmlNodes1);
				flag = true;
			}
			return flag;
		}

		private bool RemoveFilteredFieldsFromListItemXml(XmlNode listItemXml, SPFolder sourceFolder)
		{
			if (this.FieldsFilter == null)
			{
				return false;
			}
			bool flag = false;
			foreach (SPField field in sourceFolder.ParentList.Fields)
			{
				if (this.FieldsFilter.Evaluate(field, new CompareDatesInUtc()))
				{
					continue;
				}
				string str = XmlUtility.EncodeNameStartChars(field.Name);
				XmlAttribute itemOf = listItemXml.Attributes[str];
				if (itemOf == null)
				{
					continue;
				}
				listItemXml.Attributes.Remove(itemOf);
				flag = true;
			}
			return flag;
		}

		private bool RemoveFilteredFieldsFromListXML(XmlNode listXml, SPList sourceList)
		{
			if (this.FieldsFilter == null)
			{
				return false;
			}
			bool flag = false;
			foreach (SPField field in sourceList.Fields)
			{
				if (this.FieldsFilter.Evaluate(field, new CompareDatesInUtc()))
				{
					continue;
				}
				flag |= ColumnMappings.RemoveField(listXml, field);
			}
			return flag;
		}

		private bool RemoveMappedFieldsFromListXML(XmlNode listXml)
		{
			if (base.Count == 0)
			{
				return false;
			}
			bool flag = false;
			foreach (XmlNode xmlNodes in listXml.SelectNodes("./Fields/Field"))
			{
				XmlAttribute itemOf = xmlNodes.Attributes["Name"];
				if (xmlNodes.Attributes["Name"] == null)
				{
					continue;
				}
				ListSummaryItem item = base[itemOf.Value];
				if (item == null || item.Source.Target.Equals(item.Target.Target, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				flag |= ColumnMappings.RemoveField(listXml, new SPField(xmlNodes));
			}
			return flag;
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("ColumnMappingsList");
			xmlWriter.WriteAttributeString("AutoMapCreatedAndModified", this.AutoMapCreatedAndModified.ToString());
			if (this.FieldsFilter != null)
			{
				xmlWriter.WriteStartElement("FieldsFilter");
				this.FieldsFilter.ToXML(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			base.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();
		}
	}
}