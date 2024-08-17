using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare | ProductFlags.CMWebComponents)]
	[MenuText("Copy Site {1-Copy}")]
	[MenuTextPlural("Copy Sites {1-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy Site")]
	[Shortcut(ShortcutAction.Copy)]
	[TargetType(typeof(SPWeb))]
	public class CopySiteAction : CopyAction
	{
		public CopySiteAction()
		{
		}
	}
}