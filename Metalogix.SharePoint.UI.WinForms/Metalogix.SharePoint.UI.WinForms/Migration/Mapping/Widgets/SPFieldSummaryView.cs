using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class SPFieldSummaryView : ListSummaryView<SPField, SPField>
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

		public override ListView<SPField> TargetView
		{
			get
			{
				return new SPFieldView();
			}
		}

		public SPFieldSummaryView()
		{
		}
	}
}