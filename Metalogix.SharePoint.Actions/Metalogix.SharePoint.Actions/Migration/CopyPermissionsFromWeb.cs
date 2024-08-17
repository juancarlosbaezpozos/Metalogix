using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste Site Objects {0-Paste} > Permissions")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPWeb))]
	public class CopyPermissionsFromWeb : CopyPermissionsAction
	{
		public CopyPermissionsFromWeb()
		{
		}
	}
}