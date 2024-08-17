using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class SPFieldToSPFieldComparer : IListPickerComparer, IComparer<ListPickerItem>
	{
		public SPFieldToSPFieldComparer()
		{
		}

		public bool AppliesTo(ListPickerItem source, ListPickerItem target)
		{
			if (source == null || source.Tag == null || !(source.Tag is SPField) || target == null || target.Tag == null)
			{
				return false;
			}
			return target.Tag is SPField;
		}

		public int Compare(ListPickerItem x, ListPickerItem y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			if (x.Target != null && !x.Target.Equals(y.Target))
			{
				return -1;
			}
			if (x.TargetType != null && !x.TargetType.Equals(y.TargetType))
			{
				return -1;
			}
			if (x.CustomColumns.ContainsKey("FieldType") && y.CustomColumns.ContainsKey("FieldType") && !x.CustomColumns["FieldType"].Equals(y.CustomColumns["FieldType"]))
			{
				return -1;
			}
			return 0;
		}
	}
}