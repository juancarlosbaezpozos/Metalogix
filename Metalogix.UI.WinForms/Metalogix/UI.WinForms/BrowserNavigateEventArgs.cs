using Metalogix.Explorer;
using System;

namespace Metalogix.UI.WinForms
{
	public class BrowserNavigateEventArgs : CancelableEventArgs
	{
		private Metalogix.Explorer.Node _node;

		private string _url;

		public Metalogix.Explorer.Node Node
		{
			get
			{
				return this._node;
			}
		}

		public string Url
		{
			get
			{
				return this._url;
			}
		}

		public BrowserNavigateEventArgs(Metalogix.Explorer.Node node, string url)
		{
			this._node = node;
			this._url = url;
		}
	}
}