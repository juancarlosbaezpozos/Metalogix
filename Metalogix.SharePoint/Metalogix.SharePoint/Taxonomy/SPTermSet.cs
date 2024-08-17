using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
	public class SPTermSet : SPTermSetItem
	{
		private XmlNode _xml;

		private Guid _id;

		private string _name;

		private bool _bIsAvailableForTagging;

		private string _contact;

		private string _description;

		private bool _isOpenForTermCreation;

		private string _owner;

		private SPTermCollection m_allRecursiveTerms = null;

		private SPConnection _connection;

		private SPTermGroup _termGroup;

		internal override SPConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		public string Contact
		{
			get
			{
				return this._contact;
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

		public override bool IsAvailableForTagging
		{
			get
			{
				return this._bIsAvailableForTagging;
			}
		}

		public bool IsOpenForTermCreation
		{
			get
			{
				return this._isOpenForTermCreation;
			}
		}

		public override string Name
		{
			get
			{
				return this._name;
			}
		}

		public override string Owner
		{
			get
			{
				return this._owner;
			}
		}

		public SPTermGroup TermGroup
		{
			get
			{
				return this._termGroup;
			}
		}

		public override SPTermStore TermStore
		{
			get
			{
				return this._termGroup.TermStore;
			}
		}

		internal SPTermSet(XmlNode termSetNode, SPTermGroup termGroup)
		{
			if (termSetNode == null)
			{
				throw new ArgumentNullException("termSetNode");
			}
			if (termGroup == null)
			{
				throw new ArgumentNullException("termGroup");
			}
			this._xml = termSetNode;
			this._termGroup = termGroup;
			this._connection = termGroup.Connection;
			this.ParseTermSetXml(termSetNode);
		}

		public override void FetchData(bool bRefetchTerms)
		{
			this.m_allRecursiveTerms = null;
			base.FetchData(bRefetchTerms);
		}

		public SPTermCollection GetAllTerms()
		{
			if (this.m_allRecursiveTerms == null)
			{
				this.m_allRecursiveTerms = SPTermCollection.GetAllRecursiveTerms(this);
			}
			return this.m_allRecursiveTerms;
		}

		internal override string GetTermCollectionXml()
		{
			ISharePointReader reader = this._connection.Adapter.Reader;
			Guid id = this.TermStore.Id;
			string str = id.ToString();
			id = this.TermGroup.Id;
			string str1 = id.ToString();
			id = this.Id;
			return reader.GetTermCollectionFromTermSet(str, str1, id.ToString());
		}

		private void ParseTermSetXml(XmlNode termSetNode)
		{
			if (termSetNode == null)
			{
				throw new ArgumentNullException("termSetNode");
			}
			this._id = termSetNode.GetAttributeValueAsGuid(TaxonomyFields.Id.ToString());
			this._name = termSetNode.GetAttributeValueAsString(TaxonomyFields.Name.ToString());
			this._owner = termSetNode.GetAttributeValueAsString(TaxonomyFields.Owner.ToString());
			this._bIsAvailableForTagging = termSetNode.GetAttributeValueAsBoolean(TaxonomyFields.IsAvailableForTagging.ToString());
			this._contact = termSetNode.GetAttributeValueAsString(TaxonomyFields.Contact.ToString());
			this._description = termSetNode.GetAttributeValueAsString(TaxonomyFields.Description.ToString());
			this._isOpenForTermCreation = termSetNode.GetAttributeValueAsBoolean(TaxonomyFields.IsOpenForTermCreation.ToString());
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
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermSet.ToString());
			foreach (XmlAttribute attribute in this._xml.Attributes)
			{
				xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
			}
			if (!string.IsNullOrEmpty(this._xml.InnerXml))
			{
				xmlWriter.WriteRaw(this._xml.InnerXml);
			}
			this.Terms.ToXML(xmlWriter);
			xmlWriter.WriteEndElement();
		}

		internal void WriteReusableTermSetXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTermSet.ToString());
			foreach (XmlAttribute attribute in this._xml.Attributes)
			{
				xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
			}
			XmlNode xmlNodes = this._xml.SelectSingleNode(TaxonomyClassType.SPStakeholderCollection.ToString());
			if (xmlNodes != null)
			{
				xmlWriter.WriteRaw(xmlNodes.OuterXml);
			}
			xmlWriter.WriteEndElement();
		}
	}
}