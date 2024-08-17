using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.CopyList.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare | ProductFlags.CMWebComponents)]
	[MenuText("Copy List {1-Copy}")]
	[MenuTextPlural("Copy Lists {1-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy List")]
	[Shortcut(ShortcutAction.Copy)]
	[TargetType(typeof(SPList))]
	public class CopyListAction : CopyAction
	{
		public CopyListAction()
		{
		}
	}
}