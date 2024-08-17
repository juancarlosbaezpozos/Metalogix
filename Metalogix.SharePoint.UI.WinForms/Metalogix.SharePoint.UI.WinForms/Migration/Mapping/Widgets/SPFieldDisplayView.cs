using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	[Default]
	public class SPFieldDisplayView : ListView<SPField>
	{
		public override string Name
		{
			get
			{
				return "Name";
			}
		}

		public SPFieldDisplayView()
		{
		}

		public override string Render(SPField item)
		{
			if (item.DisplayName != null)
			{
				return item.DisplayName;
			}
			if (item.Name == null)
			{
				return "";
			}
			return item.Name;
		}

		public override string RenderColumn(SPField item, string propertyName)
		{
			if (!propertyName.Equals("FieldType"))
			{
				return base.RenderColumn(item, propertyName);
			}
			if (item.FieldXML.Attributes["Group"] != null)
			{
				return "Site Column";
			}
			return "List Column";
		}

		public override string RenderType(SPField item)
		{
			return item.Type;
		}
	}
}