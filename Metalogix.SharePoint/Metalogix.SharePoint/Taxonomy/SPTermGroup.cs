using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
	public class SPTermGroup : SPTaxonomyItem, IComparable<SPTermGroup>, IEquatable<SPTermGroup>, IXmlable
	{
		private XmlNode _xml;

		private Guid _id;

		private string _name;

		private SPConnection _connection;

		private SPTermStore _parentTermStore;

		private SPTermSetCollection _termSets = null;

		private readonly object _termSetLock = new object();

		private bool _isSystemGroup;

		private string _description;

		private bool _isSiteCollectionGroup;

		internal SPConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		public string Description
		{
			get
			{
				return this._description;
			}
		}

		public override Guid Id
		{
			get
			{
				return this._id;
			}
		}

		public bool IsSiteCollectionGroup
		{
			get
			{
				return this._isSiteCollectionGroup;
			}
		}

		public bool IsSystemGroup
		{
			get
			{
				return this._isSystemGroup;
			}
		}

		public override string Name
		{
			get
			{
				return this._name;
			}
		}

		public SPTermSetCollection TermSets
		{
			get
			{
				lock (this._termSetLock)
				{
					if (this._termSets == null)
					{
						SPTermSetCollection sPTermSetCollection = new SPTermSetCollection(this);
						sPTermSetCollection.FetchData();
						this._termSets = sPTermSetCollection;
					}
				}
				return this._termSets;
			}
		}

		public override SPTermStore TermStore
		{
			get
			{
				return this._parentTermStore;
			}
		}

		internal SPTermGroup(XmlNode nodeTermGroup, SPTermStore parentTermStore)
		{
			if (nodeTermGroup == null)
			{
				throw new ArgumentNullException("nodeTermGroup");
			}
			if (parentTermStore == null)
			{
				throw new ArgumentNullException("parentTermStore");
			}
			this._xml = nodeTermGroup;
			this._parentTermStore = parentTermStore;
			this._connection = parentTermStore.Connection;
			this.ParseTermGroupXml(nodeTermGroup);
		}

		public int CompareTo(SPTermGroup other)
		{
			int num;
			num = (other != null ? this._id.CompareTo(other._id) : 1);
			return num;
		}

		public bool ContainsSiteCollectionAccessId(string siteCollectionId)
		{
			bool flag = false;
			if (this._xml != null)
			{
				XmlNodeList xmlNodeLists = this._xml.SelectNodes(string.Format("{0}/{1}", TaxonomyClassType.SPSiteCollectionAccessIds.ToString(), TaxonomyFields.Id.ToString()));
				if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
				{
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						if (string.Equals(xmlNodes.InnerText, siteCollectionId, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
							break;
						}
					}
				}
			}
			return flag;
		}

		public bool Equals(SPTermGroup other)
		{
			return (other == null ? false : this._id.Equals(other._id));
		}

		private void ParseTermGroupXml(XmlNode termGroupNode)
		{
			if (termGroupNode == null)
			{
				throw new ArgumentNullException("termGroupNode");
			}
			this._id = termGroupNode.GetAttributeValueAsGuid(TaxonomyFields.Id.ToString());
			this._name = termGroupNode.GetAttributeValueAsString(TaxonomyFields.Name.ToString());
			this._description = termGroupNode.GetAttributeValueAsString(TaxonomyFields.Description.ToString());
			this._isSystemGroup = termGroupNode.GetAttributeValueAsBoolean(TaxonomyFields.IsSystemGroup.ToString());
			this._isSiteCollectionGroup = termGroupNode.GetAttributeValueAsBoolean(TaxonomyFields.IsSiteCollectionGroup.ToString());
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override string ToXML()
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
			{
				Formatting = Formatting.Indented
			};
			this.ToXML(xmlTextWriter);
			return stringBuilder.ToString();
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermGroup.ToString());
			foreach (XmlAttribute attribute in this._xml.Attributes)
			{
				xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
			}
			if (!string.IsNullOrEmpty(this._xml.InnerXml))
			{
				xmlWriter.WriteRaw(this._xml.InnerXml);
			}
			this.TermSets.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();
		}

		public string ToXMLSpecific(IList<string> termSetNames)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				Indent = true,
				OmitXmlDeclaration = true
			};
			XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting);
			try
			{
				xmlWriter.WriteStartElement(TaxonomyClassType.SPTermGroup.ToString());
				foreach (XmlAttribute attribute in this._xml.Attributes)
				{
					xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
				}
				if (!string.IsNullOrEmpty(this._xml.InnerXml))
				{
					xmlWriter.WriteRaw(this._xml.InnerXml);
				}
				this.TermSets.ToXMLSpecific(xmlWriter, termSetNames);
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			finally
			{
				if (xmlWriter != null)
				{
					((IDisposable)xmlWriter).Dispose();
				}
			}
			return stringBuilder.ToString();
		}

		internal void WriteReusableTermGroupXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermGroup.ToString());
			foreach (XmlAttribute attribute in this._xml.Attributes)
			{
				xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
			}
			XmlNode xmlNodes = this._xml.SelectSingleNode(TaxonomyClassType.SPContributorCollection.ToString());
			if (xmlNodes != null)
			{
				xmlWriter.WriteRaw(xmlNodes.OuterXml);
			}
			XmlNode xmlNodes1 = this._xml.SelectSingleNode(TaxonomyClassType.SPGroupManagerCollection.ToString());
			if (xmlNodes1 != null)
			{
				xmlWriter.WriteRaw(xmlNodes1.OuterXml);
			}
			XmlNode xmlNodes2 = this._xml.SelectSingleNode(TaxonomyClassType.SPSiteCollectionAccessIds.ToString());
			if (xmlNodes2 != null)
			{
				xmlWriter.WriteRaw(xmlNodes2.OuterXml);
			}
			xmlWriter.WriteEndElement();
		}
	}
}