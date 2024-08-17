using Metalogix.Actions;
using Metalogix.Licensing;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.ConnectToSharePointReadOnly16.png")]
	[LargeImage("Metalogix.SharePoint.Actions.Icons.Administration.ConnectToSharePointReadOnly32.png")]
	[LicensedProducts(ProductFlags.CMCSharePoint)]
	[MenuText("Add Connection {3-Connect} > Connect to SharePoint - Read Only {1}")]
	[Name("Connect to SharePoint - Read Only")]
	[ShowInMenus(true)]
	public class ConnectToSharePointReadOnly : ConnectToSharePoint
	{
		public ConnectToSharePointReadOnly()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}