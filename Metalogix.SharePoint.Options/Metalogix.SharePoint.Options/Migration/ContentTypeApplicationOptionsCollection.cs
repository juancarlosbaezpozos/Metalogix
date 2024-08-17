using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Options.Migration
{
	public class ContentTypeApplicationOptionsCollection : IXmlable
	{
		private SerializableList<ContentTypeApplicationOptions> m_data;

		private IFilterExpression m_appliesToFilter;

		private Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings m_columnMappings;

		private bool m_bAddUmappedItemsToDefaultContentType;

		private bool m_bRemoveAllOtherContentTypes;

		public bool AddUnmappedItemsToDefaultContentType
		{
			get
			{
				return this.m_bAddUmappedItemsToDefaultContentType;
			}
			set
			{
				this.m_bAddUmappedItemsToDefaultContentType = value;
			}
		}

		public IFilterExpression AppliesToFilter
		{
			get
			{
				return this.m_appliesToFilter;
			}
			set
			{
				this.m_appliesToFilter = value;
			}
		}

		public Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings ColumnMappings
		{
			get
			{
				return this.m_columnMappings;
			}
			set
			{
				this.m_columnMappings = value;
			}
		}

		public SerializableList<ContentTypeApplicationOptions> Data
		{
			get
			{
				return this.m_data;
			}
		}

		public ContentTypeApplicationOptions this[string sContentTypeName]
		{
			get
			{
				ContentTypeApplicationOptions contentTypeApplicationOption;
				using (IEnumerator<ContentTypeApplicationOptions> enumerator = this.m_data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ContentTypeApplicationOptions current = enumerator.Current;
						if (current.ContentTypeName != sContentTypeName)
						{
							continue;
						}
						contentTypeApplicationOption = current;
						return contentTypeApplicationOption;
					}
					return null;
				}
				return contentTypeApplicationOption;
			}
		}

		public ContentTypeApplicationOptions this[ContentTypeApplicationOptions key]
		{
			get
			{
				ContentTypeApplicationOptions contentTypeApplicationOption;
				using (IEnumerator<ContentTypeApplicationOptions> enumerator = this.m_data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ContentTypeApplicationOptions current = enumerator.Current;
						if (current.ContentTypeName != key.ContentTypeName)
						{
							continue;
						}
						contentTypeApplicationOption = current;
						return contentTypeApplicationOption;
					}
					return null;
				}
				return contentTypeApplicationOption;
			}
		}

		public bool RemoveAllOtherContentTypes
		{
			get
			{
				return this.m_bRemoveAllOtherContentTypes;
			}
			set
			{
				this.m_bRemoveAllOtherContentTypes = value;
			}
		}

		public ContentTypeApplicationOptionsCollection()
		{
			this.m_data = new CommonSerializableList<ContentTypeApplicationOptions>();
		}

		public ContentTypeApplicationOptionsCollection(ContentTypeApplicationOptions[] items)
		{
			this.m_data = new CommonSerializableList<ContentTypeApplicationOptions>(items);
		}

		public ContentTypeApplicationOptionsCollection(XmlNode node)
		{
			this.m_data = new CommonSerializableList<ContentTypeApplicationOptions>();
			this.FromXML(node);
		}

		public bool AppliesTo(SPList list)
		{
			if (this.AppliesToFilter == null)
			{
				return true;
			}
			return this.AppliesToFilter.Evaluate(list, new CompareDatesInUtc());
		}

		public void ApplyContentTypeMappingsToListItem(ref string sItemXml, SPListItem sourceItem, SPList targetList)
		{
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sItemXml);
			this.ApplyContentTypeMappingsToListItem(xmlNode, sourceItem, targetList);
			sItemXml = xmlNode.OuterXml;
		}

		public void ApplyContentTypeMappingsToListItem(XmlNode itemXml, SPListItem sourceItem, SPList targetList)
		{
			ContentTypeApplicationOptions contentTypeApplicationOption = null;
			foreach (ContentTypeApplicationOptions mDatum in this.m_data)
			{
				if (!mDatum.ItemIsMapped(sourceItem))
				{
					if (!this.AddUnmappedItemsToDefaultContentType || !mDatum.MakeDefault || sourceItem.ItemType != SPListItemType.Item)
					{
						continue;
					}
					contentTypeApplicationOption = mDatum;
				}
				else
				{
					contentTypeApplicationOption = mDatum;
					break;
				}
			}
			if (contentTypeApplicationOption == null)
			{
				return;
			}
			SPContentType sPContentType = null;
			foreach (SPContentType contentType in targetList.ContentTypes)
			{
				if (contentType.Name != contentTypeApplicationOption.ContentTypeName)
				{
					continue;
				}
				sPContentType = contentType;
			}
			if (sPContentType == null)
			{
				return;
			}
			XmlAttribute itemOf = itemXml.Attributes["ContentType"];
			if (itemOf == null)
			{
				itemOf = itemXml.OwnerDocument.CreateAttribute("ContentType");
				itemXml.Attributes.Append(itemOf);
			}
			itemOf.Value = sPContentType.Name;
			XmlAttribute contentTypeID = itemXml.Attributes["ContentTypeId"];
			if (contentTypeID == null)
			{
				contentTypeID = itemXml.OwnerDocument.CreateAttribute("ContentTypeId");
				itemXml.Attributes.Append(contentTypeID);
			}
			contentTypeID.Value = sPContentType.ContentTypeID;
		}

		public ContentTypeApplicationOptionsCollection Clone()
		{
			return new ContentTypeApplicationOptionsCollection(XmlUtility.StringToXmlNode(this.ToXML()));
		}

		public void FromXML(XmlNode xmlNode)
		{
			XmlNode xmlNodes = xmlNode.SelectSingleNode(".//ContentTypeApplicationOptionsCollection");
			xmlNodes = (xmlNodes == null ? xmlNode : xmlNodes);
			XmlAttribute itemOf = xmlNodes.Attributes["AddUnmappedItemsToDefaultContentType"];
			if (itemOf != null)
			{
				this.AddUnmappedItemsToDefaultContentType = bool.Parse(itemOf.Value);
			}
			itemOf = xmlNodes.Attributes["RemoveAllOtherContentTypes"];
			if (itemOf != null)
			{
				this.RemoveAllOtherContentTypes = bool.Parse(itemOf.Value);
			}
			XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./AppliesToFilter");
			if (xmlNodes1 != null && xmlNodes1.ChildNodes.Count > 0)
			{
				if (xmlNodes1.ChildNodes[0].Name.Equals("And") || xmlNodes1.ChildNodes[0].Name.Equals("Or"))
				{
					this.AppliesToFilter = new FilterExpressionList(xmlNodes1.ChildNodes[0]);
				}
				else
				{
					this.AppliesToFilter = new FilterExpression(xmlNodes1.ChildNodes[0]);
				}
			}
			xmlNodes1 = xmlNodes.SelectSingleNode("./ColumnMappings");
			if (xmlNodes1 != null && xmlNodes1.ChildNodes.Count > 0)
			{
				this.ColumnMappings = new Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings(xmlNodes1.ChildNodes[0]);
			}
			xmlNodes1 = xmlNodes.SelectSingleNode("./InternalData");
			if (xmlNodes1 != null && xmlNodes1.ChildNodes.Count > 0)
			{
				this.m_data.FromXML(xmlNodes1.ChildNodes[0]);
			}
		}

		public string ToXML()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ToXML(new XmlTextWriter(new StringWriter(stringBuilder)));
			return stringBuilder.ToString();
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("ContentTypeApplicationOptionsCollection");
			xmlWriter.WriteAttributeString("Type", this.GetType().AssemblyQualifiedName);
			xmlWriter.WriteAttributeString("AddUnmappedItemsToDefaultContentType", this.AddUnmappedItemsToDefaultContentType.ToString());
			xmlWriter.WriteAttributeString("RemoveAllOtherContentTypes", this.RemoveAllOtherContentTypes.ToString());
			if (this.AppliesToFilter != null)
			{
				xmlWriter.WriteStartElement("AppliesToFilter");
				this.AppliesToFilter.ToXML(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			if (this.ColumnMappings != null)
			{
				xmlWriter.WriteStartElement("ColumnMappings");
				this.ColumnMappings.ToXML(xmlWriter);
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteStartElement("InternalData");
			this.m_data.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}
	}
}