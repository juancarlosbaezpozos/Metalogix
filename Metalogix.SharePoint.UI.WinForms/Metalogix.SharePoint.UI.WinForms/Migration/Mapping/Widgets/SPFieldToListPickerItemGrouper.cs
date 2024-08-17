using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class SPFieldToListPickerItemGrouper : ListGrouper<SPField, ListPickerItem>
	{
		public SPFieldToListPickerItemGrouper()
		{
		}

		public override string Group(SPField source, ListPickerItem target)
		{
			if (!target.CustomColumns.ContainsKey("FieldType"))
			{
				return "default";
			}
			return string.Concat((string)target.CustomColumns["FieldType"], " - New");
		}
	}
}