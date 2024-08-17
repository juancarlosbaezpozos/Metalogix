using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
	public class SPTerm : SPTermSetItem
	{
		private readonly static IList<string> _childTermElements;

		private XmlNode _xml;

		private Guid _id;

		private string _name;

		private bool _isAvailableForTagging;

		private bool _isRoot;

		private int _termsCount;

		private string _owner;

		private bool _isDeprecated;

		private bool _isKeyword;

		private bool _isReused;

		private bool _isSourceTerm;

		private SPTerm _sourceTerm;

		private SPTermSet _parentTermSet;

		private SPTerm _parentTerm;

		private SPConnection _connection;

		private List<string> _labelCollection;

		private Guid _sourceTermGroupGuid;

		private Guid _sourceTermSetGuid;

		private Guid _sourceTermGuid;

		internal override SPConnection Connection
		{
			get
			{
				return this._connection;
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
				return this._isAvailableForTagging;
			}
		}

		public bool IsDeprecated
		{
			get
			{
				return this._isDeprecated;
			}
			set
			{
				this._isDeprecated = value;
			}
		}

		public bool IsKeyword
		{
			get
			{
				return this._isKeyword;
			}
			set
			{
				this._isKeyword = value;
			}
		}

		public bool IsReused
		{
			get
			{
				return this._isReused;
			}
			set
			{
				this._isReused = value;
			}
		}

		public bool IsRoot
		{
			get
			{
				return this._isRoot;
			}
		}

		public bool IsSourceTerm
		{
			get
			{
				return this._isSourceTerm;
			}
			set
			{
				this._isSourceTerm = value;
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

		public SPTerm Parent
		{
			get
			{
				return this._parentTerm;
			}
		}

		public SPTerm SourceTerm
		{
			get
			{
				if ((!this.IsReused ? false : this._sourceTerm == null))
				{
					if (!this.IsSourceTerm)
					{
						this._sourceTerm = this.TermStore.GetTerm(this.SourceTermGroupGuid, this.SourceTermSetGuid, this.SourceTermGuid);
					}
					else
					{
						this._sourceTerm = this;
					}
				}
				return this._sourceTerm;
			}
		}

		public Guid SourceTermGroupGuid
		{
			get
			{
				return this._sourceTermGroupGuid;
			}
		}

		public Guid SourceTermGuid
		{
			get
			{
				return this._sourceTermGuid;
			}
		}

		public Guid SourceTermSetGuid
		{
			get
			{
				return this._sourceTermSetGuid;
			}
		}

		public int TermsCount
		{
			get
			{
				return this._termsCount;
			}
		}

		public SPTermSet TermSet
		{
			get
			{
				return this._parentTermSet;
			}
		}

		public override SPTermStore TermStore
		{
			get
			{
				return this._parentTermSet.TermStore;
			}
		}

		static SPTerm()
		{
			List<string> strs = new List<string>()
			{
				TaxonomyClassType.SPDescriptionCollection.ToString(),
				TaxonomyClassType.SPLabelCollection.ToString(),
				TaxonomyClassType.SPMergedTermIds.ToString(),
				TaxonomyClassType.SPReusedTermCollection.ToString(),
				TaxonomyClassType.SPSourceTerm.ToString(),
				TaxonomyClassType.SPParentTerm.ToString(),
				TaxonomyClassType.SPCustomProperties.ToString()
			};
			SPTerm._childTermElements = strs.AsReadOnly();
		}

		internal SPTerm(XmlNode nodeTerm, SPTermSet parentTermSet, SPTerm parentTerm)
		{
			if (nodeTerm == null)
			{
				throw new ArgumentNullException("nodeTerm");
			}
			if (parentTermSet == null)
			{
				throw new ArgumentNullException("parentTermSet");
			}
			this._xml = nodeTerm;
			this._connection = parentTermSet.Connection;
			this._parentTermSet = parentTermSet;
			this._parentTerm = parentTerm;
			this.ParseTermXml(nodeTerm);
		}

		internal SPTerm(XmlNode nodeTerm, SPTermSetItem parentTermSetItem)
		{
			SPTerm sPTerm;
			if (nodeTerm == null)
			{
				throw new ArgumentNullException("nodeTerm");
			}
			if (parentTermSetItem == null)
			{
				throw new ArgumentNullException("parentTermSetItem");
			}
			this._xml = nodeTerm;
			this._parentTermSet = (parentTermSetItem is SPTermSet ? parentTermSetItem as SPTermSet : (parentTermSetItem as SPTerm).TermSet);
			if (parentTermSetItem is SPTerm)
			{
				sPTerm = parentTermSetItem as SPTerm;
			}
			else
			{
				sPTerm = null;
			}
			this._parentTerm = sPTerm;
			this._connection = this._parentTermSet.Connection;
			this.ParseTermXml(nodeTerm);
		}

		internal bool DoesLabelMatch(string termName)
		{
			bool flag = false;
			if (this._labelCollection != null)
			{
				flag = this._labelCollection.Exists((string e) => e.Equals(termName, StringComparison.OrdinalIgnoreCase));
			}
			return flag;
		}

		internal override string GetTermCollectionXml()
		{
			ISharePointReader reader = this._connection.Adapter.Reader;
			Guid id = this.TermStore.Id;
			string str = id.ToString();
			id = this.TermSet.TermGroup.Id;
			string str1 = id.ToString();
			id = this.TermSet.Id;
			string str2 = id.ToString();
			id = this.Id;
			string termCollectionFromTerm = reader.GetTermCollectionFromTerm(str, str1, str2, id.ToString());
			return termCollectionFromTerm;
		}

		private void ParseTermXml(XmlNode termNode)
		{
			if (termNode == null)
			{
				throw new ArgumentNullException("termNode");
			}
			this._id = termNode.GetAttributeValueAsGuid(TaxonomyFields.Id.ToString());
			this._name = termNode.GetAttributeValueAsString(TaxonomyFields.Name.ToString());
			this._owner = termNode.GetAttributeValueAsString(TaxonomyFields.Owner.ToString());
			this._isAvailableForTagging = termNode.GetAttributeValueAsBoolean(TaxonomyFields.IsAvailableForTagging.ToString());
			this._isRoot = termNode.GetAttributeValueAsBoolean(TaxonomyFields.IsRoot.ToString());
			this._termsCount = termNode.GetAttributeValueAsInt(TaxonomyFields.TermsCount.ToString());
			this._isDeprecated = termNode.GetAttributeValueAsBoolean(TaxonomyFields.IsDeprecated.ToString());
			this._isKeyword = termNode.GetAttributeValueAsBoolean(TaxonomyFields.IsKeyword.ToString());
			this._isReused = termNode.GetAttributeValueAsBoolean(TaxonomyFields.IsReused.ToString());
			this._isSourceTerm = termNode.GetAttributeValueAsBoolean(TaxonomyFields.IsSourceTerm.ToString());
			XmlNode xmlNodes = this._xml.SelectSingleNode(string.Format("{0}/{1}", TaxonomyClassType.SPSourceTerm.ToString(), TaxonomyClassType.SPTerm.ToString()));
			if (xmlNodes != null)
			{
				this._sourceTermGroupGuid = xmlNodes.GetAttributeValueAsGuid(TaxonomyFields.TermGroupId.ToString());
				this._sourceTermSetGuid = xmlNodes.GetAttributeValueAsGuid(TaxonomyFields.TermSetId.ToString());
				this._sourceTermGuid = xmlNodes.GetAttributeValueAsGuid(TaxonomyFields.Id.ToString());
			}
			XmlNodeList xmlNodeLists = this._xml.SelectNodes(string.Format("{0}/{1}", TaxonomyClassType.SPLabelCollection.ToString(), TaxonomyClassType.SPLabel.ToString()));
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				this._labelCollection = new List<string>();
				foreach (XmlNode xmlNodes1 in xmlNodeLists)
				{
					if (!string.Equals(this._name, xmlNodes1.Attributes["Value"].Value, StringComparison.OrdinalIgnoreCase))
					{
						this._labelCollection.Add(xmlNodes1.Attributes["Value"].Value);
					}
				}
			}
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
			xmlWriter.WriteStartElement(TaxonomyClassType.SPTerm.ToString());
			foreach (XmlAttribute attribute in this._xml.Attributes)
			{
				xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
			}
			foreach (string _childTermElement in SPTerm._childTermElements)
			{
				XmlNode xmlNodes = this._xml.SelectSingleNode(_childTermElement);
				if (xmlNodes != null)
				{
					xmlWriter.WriteRaw(xmlNodes.OuterXml);
				}
			}
			if (this.Terms.Count > 0)
			{
				this.Terms.ToXML(xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}

		internal void WriteReusableTermXML(XmlWriter xmlWriter)
		{
			foreach (XmlAttribute attribute in this._xml.Attributes)
			{
				xmlWriter.WriteAttributeString(attribute.Name, attribute.Value);
			}
			XmlNode xmlNodes = this._xml.SelectSingleNode(TaxonomyClassType.SPDescriptionCollection.ToString());
			if (xmlNodes != null)
			{
				xmlWriter.WriteRaw(xmlNodes.OuterXml);
			}
			XmlNode xmlNodes1 = this._xml.SelectSingleNode(TaxonomyClassType.SPLabelCollection.ToString());
			if (xmlNodes1 != null)
			{
				xmlWriter.WriteRaw(xmlNodes1.OuterXml);
			}
			XmlNode xmlNodes2 = this._xml.SelectSingleNode(TaxonomyClassType.SPSourceTerm.ToString());
			if (xmlNodes2 != null)
			{
				xmlWriter.WriteRaw(xmlNodes2.OuterXml);
			}
			XmlNode xmlNodes3 = this._xml.SelectSingleNode(TaxonomyClassType.SPParentTerm.ToString());
			if (xmlNodes3 != null)
			{
				xmlWriter.WriteRaw(xmlNodes3.OuterXml);
			}
			XmlNode xmlNodes4 = this._xml.SelectSingleNode(TaxonomyClassType.SPCustomProperties.ToString());
			if (xmlNodes4 != null)
			{
				xmlWriter.WriteRaw(xmlNodes4.OuterXml);
			}
			this.TermSet.TermGroup.WriteReusableTermGroupXML(xmlWriter);
			this.TermSet.WriteReusableTermSetXML(xmlWriter);
		}
	}
}