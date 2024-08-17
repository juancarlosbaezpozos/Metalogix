using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste Item Objects {0-Paste} > Permissions")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPListItem))]
	public class CopyPermissionsFromItem : CopyPermissionsAction
	{
		public CopyPermissionsFromItem()
		{
		}
	}
}