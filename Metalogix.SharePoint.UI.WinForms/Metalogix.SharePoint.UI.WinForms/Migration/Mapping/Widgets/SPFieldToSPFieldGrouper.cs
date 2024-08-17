using Metalogix.Data.Mapping;
using Metalogix.SharePoint;
using System;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class SPFieldToSPFieldGrouper : ListGrouper<SPField, SPField>
	{
		public SPFieldToSPFieldGrouper()
		{
		}

		public override string Group(SPField source, SPField target)
		{
			if (target.FieldXML.Attributes["Group"] != null)
			{
				return "Site Column";
			}
			return "List Column";
		}
	}
}