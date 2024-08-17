using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWorkflowAssociationCollection : SerializableList<SPWorkflowAssociation>
	{
		private SPWeb m_ParentWeb;

		private SPList m_ParentList;

		private SPContentType m_ParentContentType;

		public bool IsContentTypeScoped
		{
			get
			{
				return this.ParentContentType != null;
			}
		}

		public bool IsListScoped
		{
			get
			{
				return this.ParentList != null;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsSet
		{
			get
			{
				return false;
			}
		}

		public bool IsWebScoped
		{
			get
			{
				return this.ParentWeb != null;
			}
		}

		public override SPWorkflowAssociation this[SPWorkflowAssociation key]
		{
			get
			{
				return this[key];
			}
		}

		public SPContentType ParentContentType
		{
			get
			{
				return this.m_ParentContentType;
			}
		}

		public SPList ParentList
		{
			get
			{
				return this.m_ParentList;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_ParentWeb;
			}
		}

		public string Scope
		{
			get
			{
				string str;
				if (this.IsListScoped)
				{
					str = "List";
				}
				else if (this.IsWebScoped)
				{
					str = "Web";
				}
				else if (!this.IsContentTypeScoped)
				{
					str = null;
				}
				else
				{
					str = "ContentType";
				}
				return str;
			}
		}

		public SPWorkflowAssociationCollection()
		{
		}

		public SPWorkflowAssociationCollection(SPList parentList)
		{
			this.m_ParentList = parentList;
		}

		public SPWorkflowAssociationCollection(SPWeb parentWeb)
		{
			this.m_ParentWeb = parentWeb;
		}

		public SPWorkflowAssociationCollection(SPContentType parentContentType)
		{
			this.m_ParentContentType = parentContentType;
		}

		public SPWorkflowAssociationCollection(XmlNode ndWorkflowXml)
		{
			this.FromXML(ndWorkflowXml);
		}

		public SPWorkflowAssociationCollection(SPWorkflowAssociation[] workflows)
		{
			if (workflows != null)
			{
				base.AddRangeToCollection(workflows);
			}
		}

		public void FetchData(bool includePreviousWorkflowVersions)
		{
			string workflowAssociations = null;
			string objectScope = null;
			if (this.IsWebScoped)
			{
				if (this.ParentWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater)
				{
					objectScope = this.GetObjectScope(includePreviousWorkflowVersions, "web", string.Empty, this.ParentWeb.Adapter.IsDB);
					workflowAssociations = this.ParentWeb.Adapter.Reader.GetWorkflowAssociations(this.ParentWeb.ID.ToString(), objectScope);
				}
			}
			else if (this.IsListScoped)
			{
				if (this.ParentList.ParentWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater)
				{
					objectScope = this.GetObjectScope(includePreviousWorkflowVersions, "list", string.Empty, this.ParentList.Adapter.IsDB);
					workflowAssociations = this.ParentList.Adapter.Reader.GetWorkflowAssociations(this.ParentList.ID.ToString(), objectScope);
				}
			}
			else if (this.IsContentTypeScoped)
			{
				if (this.ParentContentType.ParentCollection.ParentList == null)
				{
					if (this.ParentContentType.ParentCollection.ParentWeb.Adapter.SharePointVersion.IsSharePoint2007OrLater)
					{
						objectScope = this.GetObjectScope(includePreviousWorkflowVersions, "contenttype", string.Empty, this.ParentContentType.ParentCollection.ParentWeb.Adapter.IsDB);
						workflowAssociations = this.ParentContentType.ParentCollection.ParentWeb.Adapter.Reader.GetWorkflowAssociations(this.ParentContentType.ContentTypeID.ToString(), objectScope);
					}
				}
				else if (this.ParentContentType.ParentCollection.ParentList.Adapter.SharePointVersion.IsSharePoint2007OrLater)
				{
					objectScope = this.GetObjectScope(includePreviousWorkflowVersions, "ListContentType", string.Concat("ListID='", this.ParentContentType.ParentCollection.ParentList.ID.ToString(), "'"), this.ParentContentType.ParentCollection.ParentList.Adapter.IsDB);
					workflowAssociations = this.ParentContentType.ParentCollection.ParentList.Adapter.Reader.GetWorkflowAssociations(this.ParentContentType.ContentTypeID.ToString(), objectScope);
				}
			}
			if (!string.IsNullOrEmpty(workflowAssociations))
			{
				base.FromXML(workflowAssociations);
			}
		}

		public override void FromXML(XmlNode xmlNode)
		{
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//WorkflowAssociation"))
			{
				this.Add(new SPWorkflowAssociation(xmlNodes, this, this.Scope));
			}
		}

		private string GetObjectScope(bool includePreviousWorkflowVersions, string scope, string parameter, bool isDB)
		{
			string str = string.Concat("<Config Scope='{0}' IncludePreviousVersions = '", includePreviousWorkflowVersions, "' {1}></Config>");
			string str1 = null;
			if ((includePreviousWorkflowVersions ? false : !isDB))
			{
				str1 = (scope.Equals("ListContentType", StringComparison.InvariantCultureIgnoreCase) ? this.ParentContentType.ParentCollection.ParentList.ID.ToString() : scope);
			}
			else
			{
				str1 = string.Format(str, scope, parameter);
			}
			return str1;
		}

		public override void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("WorkflowAssociations");
			foreach (SPWorkflowAssociation sPWorkflowAssociation in this)
			{
				xmlWriter.WriteRaw(sPWorkflowAssociation.XML.OuterXml);
			}
			xmlWriter.WriteEndElement();
		}
	}
}