using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[MenuText("2:Paste Folder Content {0-Paste}")]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPFolder), false)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder))]
	public class PasteFolderContentHeader : SharePointActionheader
	{
		public PasteFolderContentHeader()
		{
		}
	}
}