using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLAllListItems", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[MenuText("2:Paste List Content {0-Paste} > All List Items and Folders...")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPList), true)]
	public class PasteAllListItemsForListActions : PasteAllListItemsAction
	{
		public PasteAllListItemsForListActions()
		{
		}
	}
}