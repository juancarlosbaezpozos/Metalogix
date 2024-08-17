using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalListItem : SPNode, ListItem, Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
	{
		private readonly SPList m_parentList;

		private readonly SPFolder m_parentFolder;

		private readonly SPExternalListItemCollection m_parentCollection;

		private readonly SPExternalItem m_item;

		public string ID
		{
			get
			{
				return this.m_item.Identity;
			}
		}

		public override string ImageName
		{
			get
			{
				return "Metalogix.SharePoint.Icons.ExternalItem.ico";
			}
		}

		public override string this[string sPropertyName]
		{
			get
			{
				SPExternalItemProperty sPExternalItemProperty;
				string value;
				SPExternalItemProperty[] properties = this.m_item.Properties;
				int num = 0;
				while (true)
				{
					if (num < (int)properties.Length)
					{
						sPExternalItemProperty = properties[num];
						if (!(sPExternalItemProperty.Name == sPropertyName))
						{
							num++;
						}
						else
						{
							value = sPExternalItemProperty.Value;
							break;
						}
					}
					else
					{
						if ((this.m_parentList == null ? false : this.m_parentList.Fields != null))
						{
							foreach (SPField field in this.m_parentList.Fields)
							{
								if (string.Compare(field.Name, sPropertyName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									properties = this.m_item.Properties;
									num = 0;
									while (num < (int)properties.Length)
									{
										sPExternalItemProperty = properties[num];
										if (!(sPExternalItemProperty.Name == field.DisplayName))
										{
											num++;
										}
										else
										{
											value = sPExternalItemProperty.Value;
											return value;
										}
									}
								}
							}
						}
						PropertyInfo property = this.m_item.GetType().GetProperty(sPropertyName);
						if (property == null)
						{
							value = null;
							break;
						}
						else
						{
							value = property.GetValue(this.m_item, null) as string;
							break;
						}
					}
				}
				return value;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_item.BdcIdentity;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				string str = string.Concat(this.m_parentList.ServerRelativeUrl, "/DispForm.aspx?ID=", this.m_item.BdcIdentity);
				return str;
			}
		}

		public ListItemVersionCollection VersionHistory
		{
			get
			{
				return null;
			}
		}

		public override string XML
		{
			get
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement("SPExternalItem");
				xmlTextWriter.WriteAttributeString("BdcIdentity", this.m_item.BdcIdentity);
				xmlTextWriter.WriteAttributeString("Identity", this.m_item.Identity);
				SPExternalItemProperty[] properties = this.m_item.Properties;
				for (int i = 0; i < (int)properties.Length; i++)
				{
					SPExternalItemProperty sPExternalItemProperty = properties[i];
					xmlTextWriter.WriteStartElement("SPExternalItemProperty");
					xmlTextWriter.WriteAttributeString("Name", sPExternalItemProperty.Name);
					xmlTextWriter.WriteAttributeString("Value", sPExternalItemProperty.Value);
					xmlTextWriter.WriteAttributeString("Identifier", sPExternalItemProperty.IsIdentifier.ToString());
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
		}

		protected SPExternalListItem(SPList parentList, SPFolder parentFolder, SPExternalListItemCollection parentCollection, SPExternalItem item) : base(parentList.Adapter, parentFolder)
		{
			this.m_parentFolder = parentFolder;
			this.m_parentList = parentList;
			this.m_parentCollection = parentCollection;
			this.m_item = item;
		}

		protected override void ClearChildNodes()
		{
		}

		public static SPExternalListItem CreateListItem(SPList parentList, SPFolder parentFolder, SPExternalListItemCollection parentCollection, SPExternalItem item)
		{
			return new SPExternalListItem(parentList, parentFolder, parentCollection, item);
		}

		protected override Node[] FetchChildNodes()
		{
			return new Node[0];
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();
			foreach (SPField field in this.m_parentList.Fields)
			{
				SPPropertyDescriptor sPPropertyDescriptor = new SPPropertyDescriptor(field);
				if (sPPropertyDescriptor.Attributes.Contains(attributes))
				{
					propertyDescriptors.Add(sPPropertyDescriptor);
				}
			}
			return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			return false;
		}
	}
}