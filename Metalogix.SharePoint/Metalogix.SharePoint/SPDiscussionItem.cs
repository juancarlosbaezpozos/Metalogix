using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPDiscussionItem : SPListItem
	{
		public SPDiscussionItemCollection DiscussionItems
		{
			get
			{
				SPDiscussionItem[] discussionItems = ((SPDiscussionList)base.ParentList).GetDiscussionItems(base.ID);
				SPDiscussionItemCollection sPDiscussionItemCollection = new SPDiscussionItemCollection(base.ParentList, base.ParentList, discussionItems, base.ID);
				return sPDiscussionItemCollection;
			}
		}

		public string Ordering
		{
			get
			{
				string str;
				string value;
				if (this.m_terseData != null)
				{
					if (this.m_terseData.TryGetValue("Ordering", out str))
					{
						value = str;
						return value;
					}
					else if (this.m_terseData.TryGetValue("ThreadIndex", out str))
					{
						value = str;
						return value;
					}
				}
				if (base.ItemXML.Attributes["Ordering"] == null)
				{
					value = (base.ItemXML.Attributes["ThreadIndex"] == null ? "" : base.ItemXML.Attributes["ThreadIndex"].Value);
				}
				else
				{
					value = base.ItemXML.Attributes["Ordering"].Value;
				}
				return value;
			}
		}

		public int ParentFolderID
		{
			get
			{
				int num;
				int num1;
				num1 = (!int.TryParse(this["ParentFolderId"], out num) ? 0 : num);
				return num1;
			}
		}

		public int ParentID
		{
			get
			{
				int num;
				num = (base.ItemXML.Attributes["_ParentID"] != null ? Convert.ToInt32(base.ItemXML.Attributes["_ParentID"].Value) : 0);
				return num;
			}
			set
			{
				if (base.ItemXML.Attributes["_ParentID"] == null)
				{
					XmlAttribute xmlAttribute = base.ItemXML.OwnerDocument.CreateAttribute("_ParentID");
					base.ItemXML.Attributes.Append(xmlAttribute);
				}
				base.ItemXML.Attributes["_ParentID"].Value = value.ToString();
			}
		}

		public string ThreadIndex
		{
			get
			{
				string str;
				string str1;
				if (this.m_terseData != null)
				{
					if (this.m_terseData.TryGetValue("ThreadIndex", out str))
					{
						str1 = str;
						return str1;
					}
				}
				str1 = (base.ItemXML.Attributes["ThreadIndex"] == null ? "" : base.ItemXML.Attributes["ThreadIndex"].Value);
				return str1;
			}
		}

	    public SPDiscussionItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, int iID) : base(parentList, parentFolder, parentCollection, iID, null, null, null, null, null, SPListItemType.Item)
	    {
	    }

        internal SPDiscussionItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, int iID, Dictionary<string, string> terseProperties) : base(parentList, parentFolder, parentCollection, iID, terseProperties)
		{
		}

		public SPDiscussionItem(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, XmlNode listItemXML) : base(parentList, parentFolder, parentCollection, listItemXML)
		{
		}
	}
}