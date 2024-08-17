using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.AddSite.ico")]
	[LaunchAsJob(false)]
	[MenuText("Create Site Collection... {2-Create}")]
	[Name("Create Site Collection")]
	[RequiresWriteAccess(true)]
	[ShowStatusDialog(false)]
	[SourceCardinality(Cardinality.ZeroOrMore)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPServer))]
	public class AddSiteCollectionHeader : SharePointActionheader
	{
		public AddSiteCollectionHeader()
		{
		}
	}
}