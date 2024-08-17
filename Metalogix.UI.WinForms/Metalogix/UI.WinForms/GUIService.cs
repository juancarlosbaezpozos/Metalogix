using Metalogix.Explorer;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public static class GUIService
	{
		public static bool FireBrowserNavigate(WebBrowser browser, Node node, string url)
		{
			if (GUIService.BrowserNavigate == null)
			{
				return false;
			}
			BrowserNavigateEventArgs browserNavigateEventArg = new BrowserNavigateEventArgs(node, url);
			GUIService.BrowserNavigate(browser, browserNavigateEventArg);
			return browserNavigateEventArg.IsCanceled;
		}

		public static event BrowserNavigateEventHandler BrowserNavigate;
	}
}