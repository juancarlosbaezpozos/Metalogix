using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPPageLayout : SPListItem
	{
		private const string SystemPageLayoutContentTypeId = "0x01010007FF3E057FA8AB4AA42FCB67B453FFC1";

		private const string MasterpageContentTypeId = "0x010105";

		private readonly static IList<string> ValidPageLayoutBaseContentTypeIDs;

		public SPContentType ContentType
		{
			get;
			private set;
		}

		public new string Title
		{
			get;
			private set;
		}

		static SPPageLayout()
		{
			SPPageLayout.ValidPageLayoutBaseContentTypeIDs = new ReadOnlyCollection<string>(new List<string>()
			{
				"0x01010007FF3E057FA8AB4AA42FCB67B453FFC1",
				"0x010105"
			});
		}

		internal SPPageLayout(SPList parentList, SPFolder parentFolder, SPListItemCollection parentCollection, XmlNode listItemXML) : base(parentList, parentFolder, parentCollection, listItemXML)
		{
			this.ContentType = null;
			this.Title = null;
			this.ParseContentTypeInfo();
		}

		public static bool IsPageLayout(XmlNode itemXml)
		{
			bool flag;
			if (itemXml != null)
			{
				string str = (itemXml.Attributes["FileLeafRef"] != null ? itemXml.Attributes["FileLeafRef"].Value : string.Empty);
				if ((string.IsNullOrEmpty(str) ? false : str.Contains(".aspx")))
				{
					string str1 = (itemXml.Attributes["ContentTypeId"] != null ? itemXml.Attributes["ContentTypeId"].Value : string.Empty);
					flag = (!string.IsNullOrEmpty(str1) ? SPPageLayout.ValidPageLayoutBaseContentTypeIDs.Any<string>(new Func<string, bool>(str1.StartsWith)) : false);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private void ParseContentTypeInfo()
		{
			XmlNode nodeXML = this.GetNodeXML();
			XmlAttribute itemOf = nodeXML.Attributes["FileLeafRef"];
			if ((itemOf == null ? true : !itemOf.Value.Contains(".aspx")))
			{
				throw new FormatException("Content type is not page layout");
			}
			XmlAttribute xmlAttribute = nodeXML.Attributes["Title"];
			XmlAttribute itemOf1 = nodeXML.Attributes["PublishingAssociatedContentType"];
			if ((xmlAttribute == null ? true : itemOf1 == null))
			{
				XmlNode xmlNodes = nodeXML.SelectSingleNode("./Versions/ListItem");
				if (xmlNodes != null)
				{
					if (xmlAttribute == null)
					{
						xmlAttribute = xmlNodes.Attributes["Title"];
					}
					if (itemOf1 == null)
					{
						itemOf1 = xmlNodes.Attributes["PublishingAssociatedContentType"];
					}
				}
			}
			this.Title = (xmlAttribute != null ? xmlAttribute.Value : "");
			if (itemOf1 != null)
			{
				string value = itemOf1.Value;
				char[] chrArray = new char[] { '#', ';' };
				value = value.Trim(chrArray);
				if (value.Length > 1)
				{
					int num = value.IndexOf(";#");
					if (num >= 0)
					{
						value = value.Substring(num + 2);
					}
					num = value.IndexOf(";#");
					if (num >= 0)
					{
						value = value.Substring(num + 2);
					}
					num = value.IndexOf(";#");
					if (num >= 0)
					{
						value = value.Substring(0, num);
					}
				}
				this.ContentType = base.ParentList.ParentWeb.RootWeb.ContentTypes[value];
			}
		}

		public override string ToString()
		{
			return (string.IsNullOrEmpty(this.Title) ? this.Name : this.Title);
		}

		public class XmlNames
		{
			public const string CT_ASSOCIATED = "PublishingAssociatedContentType";

			public const string FILE_LEAF_REF = "FileLeafRef";

			public const string TITLE = "Title";

			public XmlNames()
			{
			}
		}
	}
}