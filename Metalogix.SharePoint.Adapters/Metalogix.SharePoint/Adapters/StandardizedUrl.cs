using System;

namespace Metalogix.SharePoint.Adapters
{
    public class StandardizedUrl
    {
        private string m_full;

        private string m_serverRelative;

        private string m_webRelative;

        public string Full
        {
            get { return this.m_full; }
            set { this.m_full = value; }
        }

        public string ServerRelative
        {
            get { return this.m_serverRelative; }
            set { this.m_serverRelative = value; }
        }

        public string WebRelative
        {
            get { return this.m_webRelative; }
            set { this.m_webRelative = value; }
        }

        public StandardizedUrl()
        {
        }

        public static StandardizedUrl StandardizeUrl(SharePointAdapter adapter, string url)
        {
            StandardizedUrl standardizedUrl = new StandardizedUrl();
            if (url == null)
            {
                return standardizedUrl;
            }

            string str = null;
            if (!string.IsNullOrEmpty(adapter.ServerUrl))
            {
                string[] serverUrl = new string[] { adapter.ServerUrl, adapter.ServerRelativeUrl };
                str = UrlUtils.StandardizeFormat(UrlUtils.ConcatUrls(serverUrl));
            }

            if (url.Contains("://"))
            {
                url = UrlUtils.StandardizeFormat(url);
            }
            else if (!url.StartsWith("/", StringComparison.Ordinal))
            {
                standardizedUrl.WebRelative = UrlUtils.StandardizeFormat(url);
                if (!string.IsNullOrEmpty(str))
                {
                    string[] strArrays = new string[] { str, url };
                    url = UrlUtils.ConcatUrls(strArrays);
                    url = UrlUtils.StandardizeFormat(url);
                }
            }
            else
            {
                standardizedUrl.ServerRelative = UrlUtils.StandardizeFormat(url);
                if (!string.IsNullOrEmpty(str))
                {
                    string[] serverUrl1 = new string[] { adapter.ServerUrl, url };
                    url = UrlUtils.ConcatUrls(serverUrl1);
                    url = UrlUtils.StandardizeFormat(url);
                }
            }

            if (url.Contains("://"))
            {
                standardizedUrl.Full = url;
            }

            if (standardizedUrl.ServerRelative == null)
            {
                if (standardizedUrl.Full != null)
                {
                    standardizedUrl.ServerRelative = UrlUtils.ConvertFullUrlToServerRelative(standardizedUrl.Full);
                    standardizedUrl.ServerRelative = UrlUtils.StandardizeFormat(standardizedUrl.ServerRelative);
                }
                else if (standardizedUrl.WebRelative != null)
                {
                    string[] serverRelativeUrl = new string[]
                        { adapter.ServerRelativeUrl, standardizedUrl.WebRelative };
                    standardizedUrl.ServerRelative = UrlUtils.ConcatUrls(serverRelativeUrl);
                    if (!standardizedUrl.ServerRelative.StartsWith("/", StringComparison.Ordinal))
                    {
                        standardizedUrl.ServerRelative = string.Concat("/", standardizedUrl.ServerRelative);
                    }

                    standardizedUrl.ServerRelative = UrlUtils.StandardizeFormat(standardizedUrl.ServerRelative);
                }
            }

            if (standardizedUrl.WebRelative == null)
            {
                if (standardizedUrl.Full != null && str != null &&
                    standardizedUrl.Full.StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
                {
                    standardizedUrl.WebRelative = standardizedUrl.Full.Remove(0, str.Length);
                    string webRelative = standardizedUrl.WebRelative;
                    char[] chrArray = new char[] { '/' };
                    standardizedUrl.WebRelative = webRelative.Trim(chrArray);
                }
                else if (standardizedUrl.ServerRelative != null)
                {
                    string str1 = UrlUtils.StandardizeFormat(adapter.ServerRelativeUrl);
                    if (!str1.StartsWith("/", StringComparison.Ordinal))
                    {
                        str1 = string.Concat(str1, "/", str1);
                    }

                    if (standardizedUrl.ServerRelative.StartsWith(str1, StringComparison.InvariantCultureIgnoreCase))
                    {
                        standardizedUrl.WebRelative = standardizedUrl.ServerRelative.Remove(0, str1.Length);
                        string webRelative1 = standardizedUrl.WebRelative;
                        char[] chrArray1 = new char[] { '/' };
                        standardizedUrl.WebRelative = webRelative1.Trim(chrArray1);
                    }
                }
            }

            return standardizedUrl;
        }
    }
}