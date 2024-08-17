using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class CustomFieldFilter : ListFilter<SPField>
	{
		public override string Name
		{
			get
			{
				return "Custom Columns";
			}
		}

		public CustomFieldFilter()
		{
		}

		public override bool Filter(SPField item)
		{
			bool flag;
			if (item.FieldXML != null && item.FieldXML.Attributes["SourceID"] != null)
			{
				try
				{
					Guid guid = new Guid(item.FieldXML.Attributes["SourceID"].Value);
					flag = true;
				}
				catch
				{
					return false;
				}
				return flag;
			}
			return false;
		}
	}
}