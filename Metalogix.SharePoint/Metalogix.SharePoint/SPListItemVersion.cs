using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("Version")]
	[PluralName("Versions")]
	public class SPListItemVersion : SPListItem, ListItemVersion, ListItem, Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
	{
		private SPListItem m_parentItem = null;

		public override bool IsCurrentVersion
		{
			get
			{
				string attributeValueAsString = base.ItemXML.GetAttributeValueAsString("_IsCurrentVersion");
				if (string.IsNullOrEmpty(attributeValueAsString))
				{
					attributeValueAsString = base.ItemXML.GetAttributeValueAsString("_VersionIsCurrent");
				}
				return (string.Equals(attributeValueAsString, "1") ? true : string.Equals(attributeValueAsString, "True", StringComparison.OrdinalIgnoreCase));
			}
		}

		public bool IsMinorVersion
		{
			get
			{
				return (this.Version != null ? this.Version.Minor != 0 : false);
			}
		}

		public bool IsTempVersion
		{
			get
			{
				return this.VersionComments == "Temporary Version - To be deleted";
			}
		}

		public override bool IsWebPartPage
		{
			get
			{
				return this.m_parentItem.IsWebPartPage;
			}
		}

		public override string LinkableUrl
		{
			get
			{
				string str;
				if ((base.Adapter.ServerLinkName == null ? false : !(base.Adapter.ServerLinkName.Trim() == "")))
				{
					string serverLinkName = base.Adapter.ServerLinkName;
					StringBuilder stringBuilder = new StringBuilder(base.ParentList.ServerRelativeUrl);
					stringBuilder.Append((base.ParentList.IsDocumentLibrary ? "/Forms" : ""));
					stringBuilder.Append(this.GetListASPXPageString(base.ParentList.BaseTemplate));
					stringBuilder.Append(base.ID.ToString());
					stringBuilder.Append("&VersionNo=");
					stringBuilder.Append(this.VersionNumber);
					string str1 = stringBuilder.ToString();
					str = (!str1.StartsWith("/") ? string.Concat(serverLinkName, "/", str1) : string.Concat(serverLinkName, str1));
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public override SPListItemCollection ParentCollection
		{
			get
			{
				return this.m_parentItem.ParentCollection;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				string serverRelativeUrl;
				object[] str;
				if (this.IsCurrentVersion)
				{
					serverRelativeUrl = base.ServerRelativeUrl;
				}
				else if (!base.ParentList.IsDocumentLibrary)
				{
					str = new object[] { base.ParentList.ServerRelativeUrl, this.GetListASPXPageString(base.ParentList.BaseTemplate), null, null, null };
					str[2] = base.ID.ToString();
					str[3] = "&VersionNo=";
					str[4] = this.VersionNumber;
					serverRelativeUrl = string.Concat(str);
				}
				else if (!base.Adapter.SharePointVersion.IsSharePoint2007OrEarlier)
				{
					str = new object[5];
					string fileDirRef = base.FileDirRef;
					char[] chrArray = new char[] { '/' };
					str[0] = fileDirRef.TrimEnd(chrArray);
					str[1] = "/";
					string fileLeafRef = base.FileLeafRef;
					chrArray = new char[] { '/' };
					str[2] = fileLeafRef.TrimStart(chrArray);
					str[3] = "?PageVersion=";
					str[4] = this.VersionNumber;
					serverRelativeUrl = string.Concat(str);
				}
				else
				{
					serverRelativeUrl = string.Concat(base.FileDirRef.Insert(base.ParentList.ParentWeb.ServerRelativeUrl.Length - 1, string.Concat("/_vti_history/", this.VersionNumber)), "/", base.FileLeafRef);
				}
				return serverRelativeUrl;
			}
		}

		public System.Version Version
		{
			get
			{
				System.Version version;
				if (!string.IsNullOrEmpty(this.VersionString))
				{
					version = new System.Version(this.VersionString);
				}
				else
				{
					version = null;
				}
				return version;
			}
		}

		public override string VersionComments
		{
			get
			{
				return (base.ItemXML.Attributes["_CheckinComment"] != null ? base.ItemXML.Attributes["_CheckinComment"].Value : "");
			}
		}

		public int VersionNumber
		{
			get
			{
				return (base.ItemXML.Attributes["_VersionNumber"] != null ? Convert.ToInt32(base.ItemXML.Attributes["_VersionNumber"].Value) : 0);
			}
		}

		public override string VersionString
		{
			get
			{
				return (base.ItemXML.Attributes["_VersionString"] != null ? base.ItemXML.Attributes["_VersionString"].Value : "");
			}
		}

		public override SPWebPartPage WebPartPage
		{
			get
			{
				SPWebPartPage webPartPage;
				if (!this.IsWebPartPage)
				{
					webPartPage = null;
				}
				else
				{
					webPartPage = this.m_parentItem.WebPartPage;
				}
				return webPartPage;
			}
		}

		public override string XML
		{
			get
			{
				StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement("ListItem");
				foreach (XmlAttribute attribute in base.ItemXML.Attributes)
				{
					xmlTextWriter.WriteAttributeString(attribute.Name, attribute.Value);
				}
				if ((!base.HasAttachments ? false : this.IsCurrentVersion))
				{
					xmlTextWriter.WriteStartElement("Attachments");
					foreach (SPAttachment attachment in base.Attachments)
					{
						xmlTextWriter.WriteRaw(attachment.XML);
					}
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
		}

	    public SPListItemVersion(SPListItem parentItem, SPList parentList, SPFolder parentFolder, int iID) : base(parentList, parentFolder, null, iID, null, null, null, null, null, SPListItemType.Item)
	    {
	        this.m_parentItem = parentItem;
	    }

        public SPListItemVersion(SPListItem parentItem, SPList parentList, SPFolder parentFolder, XmlNode listItemXML) : base(parentList, parentFolder, null, listItemXML)
		{
			this.m_parentItem = parentItem;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			flag = (base.GetType() == obj.GetType() ? this == obj as SPListItemVersion : false);
			return flag;
		}

		public override int GetHashCode()
		{
			int hashCode = base.ParentList.GetHashCode();
			int d = base.ID;
			int num = hashCode + d.GetHashCode();
			d = this.VersionNumber;
			return num + d.GetHashCode();
		}

		private string GetListASPXPageString(ListTemplateType templateType)
		{
			string str;
			if (templateType == ListTemplateType.BlogPosts)
			{
				str = "/Post.aspx?ID=";
			}
			else if (templateType != ListTemplateType.BlogCategories)
			{
				str = (templateType != ListTemplateType.BlogComments ? "/DispForm.aspx?ID=" : "/ViewComment.aspx?ID=");
			}
			else
			{
				str = "/ViewCategory.aspx?ID=";
			}
			return str;
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable comparableNode, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			if (!(comparableNode is SPListItemVersion))
			{
				throw new Exception("SPListItemVersion can only be compared to another SPListItemVersion");
			}
			return base.IsEqual(comparableNode, differencesOutput, options);
		}

		public static bool operator ==(SPListItemVersion item1, SPListItemVersion item2)
		{
			bool flag;
			if (object.ReferenceEquals(item1, item2))
			{
				flag = true;
			}
			else if ((object.ReferenceEquals(item1, null) ? false : !object.ReferenceEquals(item2, null)))
			{
				flag = (item1.ParentList != item2.ParentList || item1.ID != item2.ID ? false : item1.VersionNumber == item2.VersionNumber);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool operator !=(SPListItemVersion item1, SPListItemVersion item2)
		{
			bool flag;
			if (object.ReferenceEquals(item1, item2))
			{
				flag = false;
			}
			else if ((object.ReferenceEquals(item1, null) ? false : !object.ReferenceEquals(item2, null)))
			{
				flag = (item1.ParentList != item2.ParentList || item1.ID != item2.ID ? true : item1.VersionNumber != item2.VersionNumber);
			}
			else
			{
				flag = true;
			}
			return flag;
		}
	}
}