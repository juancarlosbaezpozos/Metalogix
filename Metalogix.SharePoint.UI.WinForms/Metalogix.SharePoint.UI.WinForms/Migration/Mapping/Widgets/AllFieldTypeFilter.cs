using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Ignore]
	public class AllFieldTypeFilter : IListFilter
	{
		public string Name
		{
			get
			{
				return "All Columns Filter";
			}
		}

		public AllFieldTypeFilter()
		{
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
			return item is SPField;
		}
	}
}