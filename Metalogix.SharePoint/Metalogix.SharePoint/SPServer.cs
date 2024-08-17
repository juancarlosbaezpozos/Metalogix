using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer.Attributes;
using Metalogix.SharePoint.Adapters;
using System;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	[Name("SharePoint Server")]
	[PluralName("SharePoint Servers")]
	[UserFriendlyNodeName("SharePoint Farm")]
	public class SPServer : SPBaseServer
	{
		public override string DisplayName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append(base.Adapter.ServerDisplayName);
				stringBuilder.Append(" (");
				stringBuilder.Append((base.ShowAllSites ? "Farm " : "Web App "));
				stringBuilder.Append((base.Adapter.ServerType == "Auto Detect" ? "" : string.Concat("- ", base.Adapter.ServerType, " ")));
				stringBuilder.Append((base.IsLimitedSiteCollectionConnection ? "- Limited Site Collections " : string.Empty));
				stringBuilder.Append("- ");
				stringBuilder.Append(base.Adapter.LoggedInAs);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
		}

		public override string ImageResourceName
		{
			get
			{
				return "Metalogix.SharePoint.Icons.SPServer-{0}-{1}{2}.ico";
			}
		}

		public SPServer(XmlNode connectionNode) : base(connectionNode)
		{
		}

		public SPServer(SharePointAdapter adapter) : base(adapter)
		{
		}
	}
}