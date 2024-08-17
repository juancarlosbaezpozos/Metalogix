using Metalogix.Data.Mapping;
using Metalogix.ExternalConnections;
using System;

namespace Metalogix.SharePoint.ExternalConnections
{
	[Default]
	public class NintexExternalConnectionView : ListView<NintexExternalConnection>
	{
		public override string Name
		{
			get
			{
				return "Nintex Connections";
			}
		}

		public NintexExternalConnectionView()
		{
		}

		public override string Render(NintexExternalConnection item)
		{
			return string.Concat(item.Server, "\\", item.Database);
		}

		public override string RenderColumn(NintexExternalConnection item, string propertyName)
		{
			string str;
			str = (!string.Equals("status", propertyName, StringComparison.OrdinalIgnoreCase) ? string.Empty : item.Status.ToString());
			return str;
		}

		public override string RenderGroup(NintexExternalConnection item)
		{
			return "Nintex";
		}

		public override string RenderType(NintexExternalConnection item)
		{
			return "Nintex Database";
		}
	}
}