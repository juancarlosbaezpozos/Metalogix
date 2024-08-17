using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLAllListItems", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[MenuText("2:Paste Folder Content {0-Paste} > All List Items and Folders...")]
	[ShowInMenus(true)]
	[SourceType(typeof(SPFolder), false)]
	public class PasteAllListItemsForFolderActions : PasteAllListItemsAction
	{
		public PasteAllListItemsForFolderActions()
		{
		}
	}
}