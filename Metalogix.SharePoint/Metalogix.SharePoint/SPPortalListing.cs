using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPPortalListing
	{
		private Guid m_listingID;

		private string m_sGroup;

		private int m_iOrder;

		private SPPortalListingCollection m_parentCollection;

		private DateTime m_modified;

		private XmlNode m_listingXml;

		private XmlNode FullXml
		{
			get
			{
				if (this.m_listingXml == null)
				{
					this.FetchFullData();
				}
				return this.m_listingXml;
			}
		}

		public string Group
		{
			get
			{
				return this.m_sGroup;
			}
		}

		public int GroupOrder
		{
			get
			{
				int num;
				int order;
				foreach (SPPortalListingGroup group in this.ParentCollection.Groups)
				{
					if (group.Name == this.Group)
					{
						order = group.Order;
						return order;
					}
				}
				order = (!int.TryParse(this.Group, out num) ? -1 : num);
				return order;
			}
		}

		public bool HasFullData
		{
			get
			{
				return this.m_listingXml != null;
			}
		}

		public bool HasOwnContent
		{
			get
			{
				bool flag;
				string item = this["Url"];
				if (!string.IsNullOrEmpty(item))
				{
					Guid listingID = this.ListingID;
					flag = item.EndsWith(string.Concat("/txtlstvw.aspx?LstID=", listingID.ToString()), StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					flag = false;
				}
				return flag;
			}
		}

		public bool IsPersonListing
		{
			get
			{
				return !string.IsNullOrEmpty(this["PersonSID"]);
			}
		}

		public string this[string sPropertyName]
		{
			get
			{
				string value;
				XmlAttribute itemOf = this.FullXml.Attributes[sPropertyName];
				if (itemOf != null)
				{
					value = itemOf.Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public Guid ListingID
		{
			get
			{
				return this.m_listingID;
			}
		}

		public DateTime Modified
		{
			get
			{
				return this.m_modified;
			}
		}

		public int Order
		{
			get
			{
				return this.m_iOrder;
			}
		}

		public SPPortalListingCollection ParentCollection
		{
			get
			{
				return this.m_parentCollection;
			}
		}

		public SPPortalListing(SPPortalListingCollection parentCollection, XmlNode terseDataNode)
		{
			this.m_parentCollection = parentCollection;
			this.m_listingID = new Guid(terseDataNode.Attributes["ListingID"].Value);
			this.m_sGroup = terseDataNode.Attributes["Group"].Value;
			this.m_iOrder = int.Parse(terseDataNode.Attributes["Order"].Value);
			this.m_modified = Utils.ParseDateAsUtc(terseDataNode.Attributes["Modified"].Value);
			this.m_listingXml = null;
		}

		public void FetchFullData()
		{
			this.FetchFullData(false);
		}

		public void FetchFullData(bool bForceRefresh)
		{
			if ((this.m_listingXml == null ? true : bForceRefresh))
			{
				string portalListings = this.ParentCollection.ParentWeb.Adapter.Reader.GetPortalListings(this.ListingID.ToString());
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(portalListings);
				this.m_listingXml = xmlDocument.DocumentElement.SelectSingleNode(".//Listing");
			}
		}

		public string GetAsListItemXml()
		{
			return this.GetAsListItemXml(null);
		}

		public string GetAsListItemXml(int? iItemID)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlTextWriter.WriteStartElement("ListItem");
			if ((!iItemID.HasValue ? false : iItemID.HasValue))
			{
				xmlTextWriter.WriteAttributeString("ID", iItemID.Value.ToString());
			}
			foreach (XmlAttribute attribute in this.FullXml.Attributes)
			{
				if ((attribute.Name == "PersonSID" || attribute.Name == "HtmlBlob" ? false : !(attribute.Name == "Order")))
				{
					string name = attribute.Name;
					string value = attribute.Value;
					if (attribute.Name == "Url")
					{
						string item = this["Title"];
						value = string.Concat(value, (string.IsNullOrEmpty(item) ? "" : string.Concat(", ", item)));
						name = "URL";
					}
					xmlTextWriter.WriteAttributeString(name, value);
				}
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.Flush();
			return stringBuilder.ToString();
		}

		public bool GetHasProperty(string sPropertyName)
		{
			return this.FullXml.Attributes[sPropertyName] != null;
		}

		internal void SetFullData(XmlNode listingDataNode)
		{
			this.m_listingXml = listingDataNode;
		}
	}
}