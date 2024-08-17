using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste Folder Objects {0-Paste} > Permissions")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPFolder), false)]
	public class CopyPermissionsFromFolder : CopyPermissionsAction
	{
		public CopyPermissionsFromFolder()
		{
		}
	}
}