using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public class HttpHelper
	{
		public HttpHelper()
		{
		}

		public static List<string> GetParamsAsCollection(string htmlContent, string xPath, string attribute = null)
		{
			List<string> strs = new List<string>();
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(htmlContent);
			HtmlNodeCollection htmlNodeCollections = htmlDocument.DocumentNode.SelectNodes(xPath);
			if (htmlNodeCollections != null)
			{
				foreach (HtmlNode htmlNode in (IEnumerable<HtmlNode>)htmlNodeCollections)
				{
					string innerText = htmlNode.InnerText;
					if (!string.IsNullOrEmpty(attribute))
					{
						innerText = htmlNode.GetAttributeValue(attribute, string.Empty);
					}
					strs.Add(innerText);
				}
			}
			return strs;
		}

		public static Dictionary<string, string> GetParamsAsDictionary(string htmlContent, string keyXPath, string keyAttribute, string valueXPath, string valueAttribute)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(htmlContent);
			HtmlNodeCollection htmlNodeCollections = htmlDocument.DocumentNode.SelectNodes(keyXPath);
			if (htmlNodeCollections != null)
			{
				foreach (HtmlNode htmlNode in (IEnumerable<HtmlNode>)htmlNodeCollections)
				{
					string empty = string.Empty;
					string attributeValue = string.Empty;
					attributeValue = htmlNode.GetAttributeValue(keyAttribute, string.Empty);
					if (string.IsNullOrEmpty(attributeValue))
					{
						continue;
					}
					HtmlNode htmlNode1 = htmlNode.SelectSingleNode(valueXPath);
					if (htmlNode1 == null)
					{
						continue;
					}
					empty = (string.IsNullOrEmpty(valueAttribute) || !htmlNode.Attributes.Contains(valueAttribute) ? htmlNode1.InnerText : htmlNode1.GetAttributeValue(valueAttribute, string.Empty));
					if (strs.ContainsKey(attributeValue))
					{
						continue;
					}
					strs.Add(attributeValue, empty);
				}
			}
			return strs;
		}

		public static string GetPostDataFromForm(Dictionary<string, string> parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				stringBuilder.Append(string.Format("{0}={1}&", Uri.EscapeDataString(parameter.Key), Uri.EscapeDataString(parameter.Value)));
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}

		public static string GetResponseStringFromGet(string pageUrl, string postData, ICredentials credentials, CookieContainer cookieContainer)
		{
			string str = pageUrl;
			if (!string.IsNullOrEmpty(postData))
			{
				str = string.Concat(pageUrl, "?", postData);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(str);
			if (cookieContainer != null)
			{
				httpWebRequest.CookieContainer = cookieContainer;
			}
			httpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
			httpWebRequest.Credentials = credentials;
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.Headers.Add("X-Vermeer-Content-Type", "application/x-www-form-urlencoded");
			httpWebRequest.Headers.Add("Cache-Control", "no-cache");
			httpWebRequest.Accept = "*/*";
			httpWebRequest.Headers.Add("Accept-Encoding: gzip, deflate");
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
			ServicePointManager.ServerCertificateValidationCallback = (object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true;
			string empty = string.Empty;
			using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						empty = streamReader.ReadToEnd();
					}
				}
			}
			return empty;
		}

		public static string GetResponseStringFromPost(string pageUrl, string postData, string digest, ICredentials credentials, CookieContainer cookieContainer)
		{
			HttpWebRequest length = (HttpWebRequest)WebRequest.Create(pageUrl);
			if (cookieContainer != null)
			{
				length.CookieContainer = cookieContainer;
			}
			length.Credentials = credentials;
			length.UnsafeAuthenticatedConnectionSharing = true;
			length.Method = "POST";
			string str = "application/x-www-form-urlencoded";
			length.ContentType = str;
			length.KeepAlive = true;
			length.Headers.Add("X-Vermeer-Content-Type", str);
			length.Headers.Add("Cache-Control", "no-cache");
			length.Headers.Add("Pragma", "no-cache");
			length.AllowAutoRedirect = true;
			if (!string.IsNullOrEmpty(digest))
			{
				length.Headers.Add("X-RequestDigest", digest);
				length.Headers.Add("X-RequestForceAuthentication", "true");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(postData);
			length.ContentLength = (long)((int)bytes.Length);
			ServicePointManager.ServerCertificateValidationCallback = (object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true;
			using (Stream requestStream = length.GetRequestStream())
			{
				requestStream.Write(bytes, 0, (int)bytes.Length);
			}
			string empty = string.Empty;
			using (WebResponse response = length.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						empty = streamReader.ReadToEnd();
					}
				}
			}
			return empty;
		}

		public static Dictionary<string, string> PopulateParametersFromForm(string htmlContent)
		{
			string attributeValue;
			Dictionary<string, string> strs = new Dictionary<string, string>();
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(htmlContent);
			foreach (HtmlNode htmlNode in (IEnumerable<HtmlNode>)htmlDocument.DocumentNode.SelectNodes("//input"))
			{
				string str = htmlNode.GetAttributeValue("name", string.Empty);
				if (string.IsNullOrEmpty(str))
				{
					continue;
				}
				string attributeValue1 = htmlNode.GetAttributeValue("type", string.Empty);
				if (!attributeValue1.Equals("checkbox", StringComparison.InvariantCultureIgnoreCase))
				{
					if (attributeValue1.Equals("button", StringComparison.InvariantCultureIgnoreCase) || attributeValue1.Equals("image", StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					if (!attributeValue1.Equals("radio", StringComparison.InvariantCultureIgnoreCase))
					{
						attributeValue = htmlNode.GetAttributeValue("value", string.Empty);
					}
					else
					{
						attributeValue = htmlNode.GetAttributeValue("checked", string.Empty);
						if (string.IsNullOrEmpty(attributeValue))
						{
							continue;
						}
						attributeValue = htmlNode.GetAttributeValue("value", string.Empty);
					}
				}
				else
				{
					attributeValue = htmlNode.GetAttributeValue("checked", string.Empty);
					if (!string.IsNullOrEmpty(attributeValue))
					{
						attributeValue = "on";
					}
				}
				if (strs.ContainsKey(str))
				{
					continue;
				}
				strs.Add(str, attributeValue);
			}
			return strs;
		}
	}
}