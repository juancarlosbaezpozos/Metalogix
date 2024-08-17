using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Ignore]
	public class SPContentTypeFieldFilter : IListFilter
	{
		private SPContentType m_contentType;

		public string Name
		{
			get
			{
				return this.m_contentType.Name;
			}
		}

		public SPContentTypeFieldFilter(SPContentType contentType)
		{
			this.m_contentType = contentType;
		}

		public bool AppliesTo(object item)
		{
			if (item == null)
			{
				return false;
			}
			if (item is SPField)
			{
				return true;
			}
			return item is ListPickerItem;
		}

		public bool Filter(object item)
		{
			if (!(item is SPField))
			{
				return false;
			}
			return this.m_contentType.ContentTypeXML.SelectSingleNode(string.Concat(".//FieldRef[@Name=\"", ((SPField)item).Name, "\"]")) != null;
		}
	}
}