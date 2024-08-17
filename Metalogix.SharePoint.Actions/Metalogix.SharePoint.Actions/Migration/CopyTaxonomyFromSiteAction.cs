using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste Site Objects {0-Paste} > Managed Metadata Term Stores...")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPWeb))]
	[TargetType(typeof(SPWeb))]
	public class CopyTaxonomyFromSiteAction : CopyTaxonomyAction
	{
		public CopyTaxonomyFromSiteAction()
		{
		}
	}
}