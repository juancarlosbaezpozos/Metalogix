using Metalogix.UI.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Web
{
	public static class WebBrowserAuthUtils
	{
		private const int InternetCookieHttponly = 8192;

		private static string GetCookies(string url)
		{
			string str;
			int num = 256;
			StringBuilder stringBuilder = new StringBuilder(num);
			try
			{
				if (!WebBrowserAuthUtils.NativeMethods.InternetGetCookieEx(url, null, stringBuilder, ref num, 8192, IntPtr.Zero))
				{
					if (num >= 0)
					{
						stringBuilder = new StringBuilder(num);
						if (!WebBrowserAuthUtils.NativeMethods.InternetGetCookieEx(url, null, stringBuilder, ref num, 8192, IntPtr.Zero))
						{
							str = null;
							return str;
						}
					}
					else
					{
						str = null;
						return str;
					}
				}
				return stringBuilder.ToString();
			}
			catch
			{
				throw new Exception("Failed to get cookies.");
			}
			return str;
		}

		private static string GetDomainFromUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return url;
			}
			string str = url;
			int num = str.IndexOf("://", StringComparison.Ordinal);
			if (num >= 0)
			{
				str = str.Remove(0, num + 3);
			}
			int num1 = str.IndexOf('/');
			if (num1 >= 0)
			{
				str = str.Substring(0, num1);
			}
			int num2 = str.IndexOf(":", StringComparison.Ordinal);
			if (num2 >= 0)
			{
				str = str.Substring(0, num2);
			}
			return str;
		}

		private static bool IsAbsoluteUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}
			return url.Contains("://");
		}

		public static Cookie[] LoginThroughWebBrowser(string sWebUrl)
		{
			if (!WebBrowserAuthUtils.IsAbsoluteUrl(sWebUrl))
			{
				throw new ArgumentException(string.Format(Resources.Not_A_Valid_Absolute_URL, sWebUrl));
			}
			WebBrowserLoginContext webBrowserLoginContext = new WebBrowserLoginContext(sWebUrl);
			Thread thread = new Thread(new ParameterizedThreadStart(WebBrowserAuthUtils.LoginThroughWebBrowserWorker));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start(webBrowserLoginContext);
			thread.Join();
			if (webBrowserLoginContext.Exception != null)
			{
				throw webBrowserLoginContext.Exception;
			}
			return webBrowserLoginContext.Cookies;
		}

		private static void LoginThroughWebBrowserWorker(object context)
		{
			WebBrowserLoginContext exception = context as WebBrowserLoginContext;
			if (exception == null)
			{
				throw new ArgumentException("The parameter is not of the type BrowserLoginContext", "context");
			}
			AuthenticationWebBrowser authenticationWebBrowser = new AuthenticationWebBrowser(exception.WebUrl);
			if (authenticationWebBrowser.ShowDialog() != DialogResult.OK)
			{
				if (authenticationWebBrowser.LastAttemptException == null)
				{
					exception.Exception = new Exception(Resources.No_Connections_Attempted);
				}
				else
				{
					exception.Exception = authenticationWebBrowser.LastAttemptException;
				}
			}
			if (authenticationWebBrowser.Cookies == null)
			{
				exception.Cookies = new Cookie[0];
				return;
			}
			Cookie[] cookieArray = new Cookie[authenticationWebBrowser.Cookies.Count];
			authenticationWebBrowser.Cookies.CopyTo(cookieArray, 0);
			exception.Cookies = cookieArray;
		}

		internal static Cookie[] ReadBrowserCookies(string siteUrl)
		{
			string cookies = WebBrowserAuthUtils.GetCookies(siteUrl);
			if (string.IsNullOrEmpty(cookies))
			{
				return new Cookie[0];
			}
			string domainFromUrl = WebBrowserAuthUtils.GetDomainFromUrl(siteUrl);
			string[] strArrays = new string[] { "; " };
			string[] strArrays1 = cookies.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
			List<Cookie> cookies1 = new List<Cookie>((int)strArrays1.Length);
			string[] strArrays2 = strArrays1;
			for (int i = 0; i < (int)strArrays2.Length; i++)
			{
				string str = strArrays2[i];
				char[] chrArray = new char[] { '=' };
				string[] strArrays3 = str.Split(chrArray, 2, StringSplitOptions.None);
				if ((int)strArrays3.Length == 2)
				{
					string str1 = strArrays3[0];
					string str2 = strArrays3[1];
					cookies1.Add(new Cookie(str1, str2, "/", domainFromUrl));
				}
			}
			return cookies1.ToArray();
		}

		private static class NativeMethods
		{
			[DllImport("wininet.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
			public static extern bool InternetGetCookieEx(string url, string cookieName, StringBuilder cookieData, ref int size, int flags, IntPtr reserved);
		}
	}
}