using Metalogix.Actions;
using Metalogix.Licensing;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.ConnectToSharePoint16.png")]
	[LargeImage("Metalogix.SharePoint.Actions.Icons.Administration.ConnectToSharePoint32.png")]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Add Connection {3-Connect} > Connect to SharePoint {1}")]
	[Name("Connect to SharePoint")]
	[ShowInMenus(true)]
	public class ConnectToSharePoint : Metalogix.SharePoint.Actions.Administration.ConnectAction, ISharePointConnectAction
	{
		static ConnectToSharePoint()
		{
			ActionConfigurationVariables.EnsureNonUIEnvironmentVariables();
		}

		public ConnectToSharePoint()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}