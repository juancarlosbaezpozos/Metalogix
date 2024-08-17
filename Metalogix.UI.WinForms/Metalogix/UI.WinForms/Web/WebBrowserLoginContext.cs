using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Web
{
	public class WebBrowserLoginContext
	{
		private readonly string _webUrl;

		public Cookie[] Cookies
		{
			get;
			set;
		}

		public System.Exception Exception
		{
			get;
			set;
		}

		public string WebUrl
		{
			get
			{
				return this._webUrl;
			}
		}

		public WebBrowserLoginContext(string sWebUrl)
		{
			this.Exception = null;
			this.Cookies = null;
			this._webUrl = sWebUrl;
		}
	}
}