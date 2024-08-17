using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Ignore]
	public class NewFieldTypeFilter : ListFilter<SPField>
	{
		public override string Name
		{
			get
			{
				return "New Columns Filter";
			}
		}

		public NewFieldTypeFilter()
		{
		}

		public override bool Filter(SPField item)
		{
			return false;
		}
	}
}