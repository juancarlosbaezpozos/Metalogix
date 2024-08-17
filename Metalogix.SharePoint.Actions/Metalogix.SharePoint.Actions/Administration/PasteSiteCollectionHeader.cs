using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Metalogix.Actions.AllowsSameSourceTarget(false)]
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Paste.ico")]
	[MenuText("1:Paste Site Collection...{0-Paste}")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste Site Collection")]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb), true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPServer))]
	public class PasteSiteCollectionHeader : SharePointActionheader
	{
		public PasteSiteCollectionHeader()
		{
		}
	}
}