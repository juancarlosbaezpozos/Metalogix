using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPContentType : Metalogix.DataStructures.IComparable, IXmlable
	{
		private SPContentTypeCollection m_collection = null;

		private XmlNode m_XML = null;

		private SPWorkflowAssociationCollection m_WorkflowAssociations = null;

		private bool _includePreviousWorkflowVersions = false;

		public string ContentTypeID
		{
			get
			{
				return this.m_XML.Attributes["ID"].Value;
			}
		}

		public XmlNode ContentTypeXML
		{
			get
			{
				return this.m_XML;
			}
		}

		public string DocumentTemplateServerRelativeUrl
		{
			get
			{
				string serverRelative;
				string[] serverRelativeUrl;
				XmlAttribute xmlAttribute = (XmlAttribute)this.ContentTypeXML.SelectSingleNode(".//DocumentTemplate/@TargetName");
				if ((xmlAttribute == null ? false : !string.IsNullOrEmpty(xmlAttribute.Value)))
				{
					UrlType type = UrlUtils.GetType(xmlAttribute.Value);
					if (type == UrlType.WebRelative)
					{
						if (this.ParentCollection.ParentList == null)
						{
							serverRelativeUrl = new string[] { this.ParentCollection.ParentWeb.ServerRelativeUrl, xmlAttribute.Value };
							serverRelative = UrlUtils.EnsureLeadingSlash(UrlUtils.ConcatUrls(serverRelativeUrl));
						}
						else
						{
							serverRelativeUrl = new string[] { this.ParentCollection.ParentList.ServerRelativeUrl, xmlAttribute.Value };
							serverRelative = UrlUtils.EnsureLeadingSlash(UrlUtils.ConcatUrls(serverRelativeUrl));
						}
					}
					else if (type != UrlType.ServerRelative)
					{
						StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this.ParentCollection.ParentWeb.Adapter, xmlAttribute.Value);
						serverRelative = standardizedUrl.ServerRelative;
					}
					else
					{
						serverRelative = xmlAttribute.Value;
					}
				}
				else
				{
					serverRelative = null;
				}
				return serverRelative;
			}
		}

		public bool HasDocumentTemplateInsideResourceFolder
		{
			get
			{
				bool flag;
				string documentTemplateServerRelativeUrl = this.DocumentTemplateServerRelativeUrl;
				SPFolderBasic resourceFolder = this.GetResourceFolder();
				flag = ((resourceFolder == null ? false : !string.IsNullOrEmpty(documentTemplateServerRelativeUrl)) ? UrlUtils.StartsWith(documentTemplateServerRelativeUrl, resourceFolder.ServerRelativeUrl) : false);
				return flag;
			}
		}

		private bool HasSubContentTypeID
		{
			get
			{
				return Utils.GetContentTypeIDIsSubContentType(this.ContentTypeID);
			}
		}

		public bool IncludePreviousWorkflowVersions
		{
			get
			{
				return this._includePreviousWorkflowVersions;
			}
			set
			{
				this._includePreviousWorkflowVersions = value;
			}
		}

		public bool IsFromFeature
		{
			get
			{
				return (this.m_XML.Attributes["IsFromFeature"] != null ? bool.Parse(this.m_XML.Attributes["IsFromFeature"].Value) : false);
			}
		}

		public string Name
		{
			get
			{
				string value;
				if (this.m_XML.Attributes["Name"] != null)
				{
					value = this.m_XML.Attributes["Name"].Value;
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public SPContentTypeCollection ParentCollection
		{
			get
			{
				return this.m_collection;
			}
		}

		public SPContentType ParentContentType
		{
			get
			{
				SPContentType item;
				SPContentTypeCollection sPContentTypeCollections = (this.ParentCollection.ParentList != null ? this.ParentCollection.ParentWeb.ContentTypes : this.ParentCollection);
				if (this.HasSubContentTypeID)
				{
					string str = this.ContentTypeID.Substring(0, this.ContentTypeID.Length - 34);
					if (sPContentTypeCollections[str] != null)
					{
						item = sPContentTypeCollections[str];
						return item;
					}
				}
				SPContentType closestParent = null;
				if (this.m_collection.ParentList == null)
				{
					closestParent = this.GetClosestParent(sPContentTypeCollections);
				}
				item = closestParent;
				return item;
			}
		}

		public string ParentContentTypeID
		{
			get
			{
				string contentTypeID;
				if (!this.HasSubContentTypeID)
				{
					SPContentType parentContentType = this.ParentContentType;
					if (parentContentType == null)
					{
						contentTypeID = null;
					}
					else
					{
						contentTypeID = parentContentType.ContentTypeID;
					}
				}
				else
				{
					contentTypeID = this.ContentTypeID.Substring(0, this.ContentTypeID.Length - 34);
				}
				return contentTypeID;
			}
		}

		public string ResourceFolderServerRelativeUrl
		{
			get
			{
				string str;
				if (!string.IsNullOrEmpty(this.ResourceFolderWebRelativeUrl))
				{
					string[] serverRelativeUrl = new string[] { this.ParentCollection.ParentWeb.ServerRelativeUrl, this.ResourceFolderWebRelativeUrl };
					str = UrlUtils.ConcatUrls(serverRelativeUrl);
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public string ResourceFolderWebRelativeUrl
		{
			get
			{
				string value;
				XmlAttribute xmlAttribute = (XmlAttribute)this.ContentTypeXML.SelectSingleNode(".//Folder/@TargetName");
				if ((xmlAttribute == null ? false : !string.IsNullOrEmpty(xmlAttribute.Value)))
				{
					UrlType type = UrlUtils.GetType(xmlAttribute.Value);
					if (type == UrlType.WebRelative)
					{
						if (this.ParentCollection.ParentList == null)
						{
							value = xmlAttribute.Value;
						}
						else
						{
							string[] webRelativeUrl = new string[] { this.ParentCollection.ParentList.WebRelativeUrl, xmlAttribute.Value };
							value = UrlUtils.ConcatUrls(webRelativeUrl);
						}
					}
					else if (type != UrlType.ServerRelative)
					{
						value = null;
					}
					else
					{
						StandardizedUrl standardizedUrl = StandardizedUrl.StandardizeUrl(this.ParentCollection.ParentWeb.Adapter, xmlAttribute.Value);
						if (!string.IsNullOrEmpty(standardizedUrl.WebRelative))
						{
							value = standardizedUrl.WebRelative;
						}
						else
						{
							value = null;
						}
					}
				}
				else
				{
					value = null;
				}
				return value;
			}
		}

		public SPWorkflowAssociationCollection WorkflowAssociations
		{
			get
			{
				if (this.m_WorkflowAssociations == null)
				{
					this.m_WorkflowAssociations = this.GetWorkflowAssociations(false);
				}
				return this.m_WorkflowAssociations;
			}
		}

		public string XML
		{
			get
			{
				return this.m_XML.OuterXml;
			}
		}

		public SPContentType(SPContentTypeCollection parentCollection, XmlNode node)
		{
			this.m_collection = parentCollection;
			this.m_XML = node;
		}

		public SPContentType(XmlNode xmlNode)
		{
			this.FromXML(xmlNode);
		}

		public void AddDocumentTemplate(byte[] fileContent, string fileName)
		{
			if (this.ParentCollection.ParentWeb.Adapter.Writer == null)
			{
				throw new Exception("The underlying SharePoint adapter does not support write operations");
			}
			ISharePointWriter writer = this.ParentCollection.ParentWeb.Adapter.Writer;
			string xML = this.XML;
			string[] resourceFolderWebRelativeUrl = new string[] { this.ResourceFolderWebRelativeUrl, fileName };
			writer.AddDocumentTemplatetoContentType(fileContent, xML, UrlUtils.ConcatUrls(resourceFolderWebRelativeUrl));
		}

		public OperationReportingResult AddFormTemplate(byte[] docTemplate, string changedLookupFields)
		{
			if (this.ParentCollection.ParentWeb.Adapter.Writer == null)
			{
				throw new Exception("The underlying SharePoint adapter does not support write operations");
			}
			string contentType = this.ParentCollection.ParentWeb.Adapter.Writer.AddFormTemplateToContentType(this.ParentCollection.ParentList.ID, docTemplate, this.XML, changedLookupFields);
			return new OperationReportingResult(contentType);
		}

		public void FromXML(XmlNode xmlNode)
		{
			this.m_XML = xmlNode;
		}

		public SPContentType GetClosestParent()
		{
			return this.GetClosestParent((this.ParentCollection.ParentList != null ? this.ParentCollection.ParentWeb.ContentTypes : this.ParentCollection));
		}

		private SPContentType GetClosestParent(SPContentTypeCollection collection)
		{
			SPContentType sPContentType = null;
			int length = 0;
			foreach (SPContentType sPContentType1 in collection)
			{
				if (sPContentType1 != this)
				{
					if ((!this.ContentTypeID.StartsWith(sPContentType1.ContentTypeID, StringComparison.OrdinalIgnoreCase) ? false : sPContentType1.ContentTypeID.Length > length))
					{
						sPContentType = sPContentType1;
						length = sPContentType1.ContentTypeID.Length;
					}
				}
			}
			return sPContentType;
		}

		public SPFieldCollection GetDefinedFields()
		{
			return this.ParentCollection.FieldsAvailable.GetFieldsByIdOrName(this.GetFieldReferences());
		}

		public List<SPField> GetFieldReferences()
		{
			List<SPField> sPFields = new List<SPField>();
			foreach (XmlNode xmlNodes in this.m_XML.SelectNodes("./FieldRefs/FieldRef"))
			{
				sPFields.Add(new SPField(xmlNodes));
			}
			return sPFields;
		}

		public SPFolderBasic GetResourceFolder()
		{
			SPFolderBasic sPFolderBasic;
			string resourceFolderWebRelativeUrl = this.ResourceFolderWebRelativeUrl;
			if (!string.IsNullOrEmpty(resourceFolderWebRelativeUrl))
			{
				sPFolderBasic = new SPFolderBasic(this.ParentCollection.ParentWeb, resourceFolderWebRelativeUrl);
			}
			else
			{
				sPFolderBasic = null;
			}
			return sPFolderBasic;
		}

		internal SPWorkflowAssociationCollection GetWorkflowAssociations(bool bAlwaysRefetch)
		{
			SPWorkflowAssociationCollection mWorkflowAssociations;
			if ((this.m_WorkflowAssociations == null ? true : bAlwaysRefetch))
			{
				SPWorkflowAssociationCollection sPWorkflowAssociationCollection = new SPWorkflowAssociationCollection(this);
				sPWorkflowAssociationCollection.FetchData(this._includePreviousWorkflowVersions);
				mWorkflowAssociations = sPWorkflowAssociationCollection;
			}
			else
			{
				mWorkflowAssociations = this.m_WorkflowAssociations;
			}
			return mWorkflowAssociations;
		}

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			bool flag1;
			if (targetComparable == null)
			{
				throw new Exception("Cannot compare an SPContentType to a null value");
			}
			if (!(targetComparable is SPContentType))
			{
				throw new Exception("SPContentType can only be compared to another SPContentType");
			}
			SPContentType sPContentType = (SPContentType)targetComparable;
			bool flag2 = (sPContentType.ParentCollection.ParentList != null ? true : this.ParentCollection.ParentList != null);
			foreach (XmlAttribute attribute in this.m_XML.Attributes)
			{
				XmlAttribute itemOf = sPContentType.ContentTypeXML.Attributes[attribute.Name];
				if (!(attribute.Name == "IsFromFeature"))
				{
					if (!flag2)
					{
						flag1 = true;
					}
					else
					{
						flag1 = (attribute.Name == "Version" || attribute.Name == "FeatureId" || attribute.Name == "Description" ? false : !(attribute.Name == "Group"));
					}
					if (flag1)
					{
						if (itemOf == null)
						{
							differencesOutput.Write(string.Concat("The attribute ", attribute.Name, " is missing. "), attribute.Name, DifferenceStatus.Missing);
							flag = false;
							return flag;
						}
						else if (!(attribute.Name == "ID"))
						{
							if (itemOf.Value.ToLower() != attribute.Value.ToLower())
							{
								differencesOutput.Write(string.Concat("The attribute ", attribute.Name, " is different. "), attribute.Name, DifferenceStatus.Missing);
								flag = false;
								return flag;
							}
						}
					}
				}
			}
			foreach (XmlNode xmlNodes in this.m_XML.SelectNodes("./FieldRefs/FieldRef"))
			{
				string value = xmlNodes.Attributes["Name"].Value;
				if (sPContentType.ContentTypeXML.SelectSingleNode(string.Concat("./FieldRefs/FieldRef[@Name=\"", value, "\"]")) == null)
				{
					differencesOutput.Write(string.Concat("The field ", value, " is missing. "), value, DifferenceStatus.Missing);
					flag = false;
					return flag;
				}
			}
			if (this.ParentContentType == null)
			{
				if (sPContentType.ParentContentType != null)
				{
					differencesOutput.Write("The target inherits from a different content type", "Content type inheritance");
					flag = false;
					return flag;
				}
			}
			else if (sPContentType.ParentContentType == null)
			{
				differencesOutput.Write("The target inherits from a different content type", "Content type inheritance");
				flag = false;
				return flag;
			}
			else if (this.ParentContentType.Name != sPContentType.ParentContentType.Name)
			{
				differencesOutput.Write("The target inherits from a different content type", "Content type inheritance");
				flag = false;
				return flag;
			}
			flag = true;
			return flag;
		}

		public override string ToString()
		{
			string str = string.Concat(this.Name, " (", this.ContentTypeID, ")");
			return str;
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
			this.m_XML.WriteTo(xmlWriter);
		}
	}
}