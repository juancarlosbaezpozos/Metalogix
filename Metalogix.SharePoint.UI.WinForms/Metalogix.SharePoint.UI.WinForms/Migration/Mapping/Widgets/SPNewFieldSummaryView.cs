using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class SPNewFieldSummaryView : ListSummaryView<SPField, ListPickerItem>
	{
		public override string Name
		{
			get
			{
				return "Internal Name";
			}
		}

		public override ListView<SPField> SourceView
		{
			get
			{
				return new SPFieldView();
			}
		}

		public override ListView<ListPickerItem> TargetView
		{
			get
			{
				return new ListPickerNeutralView();
			}
		}

		public SPNewFieldSummaryView()
		{
		}
	}
}