using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPAudienceCollection : IXMLAbleList, IXmlable, IEnumerable
	{
		private SPNode m_parent;

		private List<SPAudience> m_list;

		public Type CollectionType
		{
			get
			{
				return typeof(SPAudience);
			}
		}

		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		public SPAudience this[string sAudienceName]
		{
			get
			{
				SPAudience sPAudience;
				foreach (SPAudience sPAudience1 in this)
				{
					if (sPAudience1.Name == sAudienceName)
					{
						sPAudience = sPAudience1;
						return sPAudience;
					}
				}
				sPAudience = null;
				return sPAudience;
			}
		}

		public SPAudience this[Guid audienceID]
		{
			get
			{
				SPAudience sPAudience;
				foreach (SPAudience sPAudience1 in this)
				{
					if (sPAudience1.ID == audienceID)
					{
						sPAudience = sPAudience1;
						return sPAudience;
					}
				}
				sPAudience = null;
				return sPAudience;
			}
		}

		public object this[int index]
		{
			get
			{
				return this.m_list[index];
			}
		}

		public SPNode Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		public SPAudienceCollection(SPNode parent)
		{
			this.m_parent = parent;
			this.FetchData();
		}

		public SPAudienceCollection(SPNode parent, string sAudienceCollectionXml)
		{
			this.m_parent = parent;
			this.FromXML(sAudienceCollectionXml);
		}

		public SPAudienceCollection(SPNode parent, XmlNode audienceCollectionXml)
		{
			this.m_parent = parent;
			this.FromXML(audienceCollectionXml);
		}

		public SPAudience AddOrUpdateAudience(string sAudienceXml, AddAudienceOptions options)
		{
			if (string.IsNullOrEmpty(this.m_parent.Adapter.Writer.AddOrUpdateAudience(sAudienceXml, options)))
			{
				throw new Exception("Failed to add audience: The target server does not support audiences.");
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sAudienceXml);
			XmlAttribute itemOf = xmlNode.Attributes["Name"];
			if (itemOf == null)
			{
				throw new Exception("Failed to add or update audience: Audience not returned in a recognized format.");
			}
			SPAudience item = this[itemOf.Value];
			if (item != null)
			{
				this.m_list.Remove(item);
			}
			item = new SPAudience(this, xmlNode);
			this.m_list.Add(item);
			return item;
		}

		public void DeleteAllAudiences()
		{
			this.m_parent.Adapter.Writer.DeleteAllAudiences(string.Empty);
			SPAudience sPAudience = null;
			foreach (SPAudience sPAudience1 in this)
			{
				if (sPAudience1.ID == Guid.Empty)
				{
					sPAudience = sPAudience1;
					break;
				}
			}
			this.m_list.Clear();
			if (sPAudience != null)
			{
				this.m_list.Add(sPAudience);
			}
		}

		public void DeleteAudience(string sAudienceName)
		{
			this.m_parent.Adapter.Writer.DeleteAudience(sAudienceName);
			SPAudience item = this[sAudienceName];
			if (item != null)
			{
				this.m_list.Remove(item);
			}
		}

		public void FetchData()
		{
			string audiences = this.m_parent.Adapter.Reader.GetAudiences();
			if (string.IsNullOrEmpty(audiences))
			{
				this.m_list = new List<SPAudience>();
			}
			else
			{
				this.FromXML(audiences);
			}
		}

		private void FromXML(string sAudienceCollectionXml)
		{
			this.FromXML(XmlUtility.StringToXmlNode(sAudienceCollectionXml));
		}

		public void FromXML(XmlNode xmlNode)
		{
			XmlNodeList xmlNodeLists = xmlNode.SelectNodes("//AudienceCollection/Audience");
			this.m_list = new List<SPAudience>(xmlNodeLists.Count);
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				SPAudience sPAudience = new SPAudience(this, xmlNodes);
				this.m_list.Add(sPAudience);
			}
		}

		public static SPAudienceCollection GetAudienceCollection(SPNode node)
		{
			SPAudienceCollection audiences;
			if (node == null)
			{
				audiences = null;
			}
			else if (node is SPServer)
			{
				audiences = ((SPServer)node).Audiences;
			}
			else if (node is SPWeb)
			{
				audiences = ((SPWeb)node).Audiences;
			}
			else if (node.Parent is SPNode)
			{
				audiences = SPAudienceCollection.GetAudienceCollection((SPNode)node.Parent);
			}
			else
			{
				audiences = null;
			}
			return audiences;
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_list.GetEnumerator();
		}

		public string ToXML()
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			this.ToXML(xmlTextWriter);
			xmlTextWriter.Flush();
			return stringBuilder.ToString();
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("AudienceCollection");
			foreach (SPAudience sPAudience in this)
			{
				xmlWriter.WriteRaw(sPAudience.XML);
			}
			xmlWriter.WriteEndElement();
		}
	}
}