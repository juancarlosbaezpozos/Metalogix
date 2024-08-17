using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Default]
	public class SPFieldDisplaySummaryView : ListSummaryView<SPField, SPField>
	{
		public override string Name
		{
			get
			{
				return "Name";
			}
		}

		public override ListView<SPField> SourceView
		{
			get
			{
				return new SPFieldDisplayView();
			}
		}

		public override ListView<SPField> TargetView
		{
			get
			{
				return new SPFieldDisplayView();
			}
		}

		public SPFieldDisplaySummaryView()
		{
		}
	}
}