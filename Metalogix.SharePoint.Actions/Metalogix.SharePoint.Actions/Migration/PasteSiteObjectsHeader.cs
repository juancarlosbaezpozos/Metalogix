using Metalogix.Actions;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste Site Objects {0-Paste}")]
	[ShowInMenus(true)]
	[SourceCardinality(Cardinality.OneOrMore)]
	[SourceType(typeof(SPWeb))]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(ISecurableObject))]
	public class PasteSiteObjectsHeader : SharePointActionheader
	{
		public PasteSiteObjectsHeader()
		{
		}
	}
}