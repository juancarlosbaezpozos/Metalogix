using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Paste List Objects {0-Paste} > Permissions")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPList))]
	public class CopyPermissionsFromList : CopyPermissionsAction
	{
		public CopyPermissionsFromList()
		{
		}
	}
}