using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare | ProductFlags.CMWebComponents)]
	[MenuText("Copy Folder {1-Copy}")]
	[MenuTextPlural("Copy Folders {1-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy Folder")]
	[Shortcut(ShortcutAction.Copy)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder), false)]
	public class CopyFolderAction : CopyAction
	{
		public CopyFolderAction()
		{
		}
	}
}