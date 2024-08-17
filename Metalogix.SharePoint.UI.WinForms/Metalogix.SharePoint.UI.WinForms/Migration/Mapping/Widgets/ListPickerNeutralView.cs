using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Ignore]
	public class ListPickerNeutralView : ListView<ListPickerItem>
	{
		public override string Name
		{
			get
			{
				return "List Picker Item";
			}
		}

		public ListPickerNeutralView()
		{
		}

		public override string Render(ListPickerItem item)
		{
			return item.Target;
		}

		public override string RenderColumn(ListPickerItem item, string propertyName)
		{
			if (!item.CustomColumns.ContainsKey(propertyName))
			{
				return null;
			}
			object obj = item.CustomColumns[propertyName];
			if (obj == null)
			{
				return null;
			}
			return obj.ToString();
		}

		public override string RenderGroup(ListPickerItem item)
		{
			return item.Group;
		}

		public override string RenderType(ListPickerItem item)
		{
			return item.TargetType;
		}
	}
}