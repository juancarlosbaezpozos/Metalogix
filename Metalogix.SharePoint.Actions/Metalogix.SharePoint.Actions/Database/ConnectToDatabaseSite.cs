using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint.Actions.Administration;
using System;

namespace Metalogix.SharePoint.Actions.Database
{
	[Image("Metalogix.SharePoint.Actions.Icons.Database.ConnectToSharePointDatabase16.png")]
	[LargeImage("Metalogix.SharePoint.Actions.Icons.Database.ConnectToSharePointDatabase32.png")]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Add Connection {3-Connect} > Connect to SharePoint Database - Read only {0}")]
	[Name("Connect to SharePoint Database")]
	[ShowInMenus(true)]
	public class ConnectToDatabaseSite : Metalogix.SharePoint.Actions.Administration.ConnectAction
	{
		public ConnectToDatabaseSite()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}