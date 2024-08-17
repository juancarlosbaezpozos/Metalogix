using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Ignore]
	public class SPSiteFieldTypeFilter : IListFilter
	{
		private List<SPField> m_fields;

		public List<SPField> Fields
		{
			get
			{
				if (this.m_fields == null)
				{
					this.m_fields = new List<SPField>();
				}
				return this.m_fields;
			}
		}

		public string Name
		{
			get
			{
				return "Site Columns Filter";
			}
		}

		public SPSiteFieldTypeFilter(List<SPField> fields)
		{
			this.m_fields = fields;
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
			return this.m_fields.Contains(item as SPField);
		}
	}
}