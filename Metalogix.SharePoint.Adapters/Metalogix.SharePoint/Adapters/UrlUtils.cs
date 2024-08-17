using Metalogix.SharePoint.Adapters.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Metalogix.SharePoint.Adapters
{
    public static class UrlUtils
    {
        public static string ConcatUrls(params string[] urls)
        {
            if (urls == null || (int)urls.Length == 0)
            {
                return null;
            }

            if ((int)urls.Length == 1)
            {
                return urls[0];
            }

            string str = urls[0];
            for (int i = 1; i < (int)urls.Length; i++)
            {
                str = UrlUtils.ConcatUrlsHelper(str, urls[i]);
            }

            return str;
        }

        private static string ConcatUrlsHelper(string url1, string url2)
        {
            if (string.IsNullOrEmpty(url2))
            {
                return url1;
            }

            if (string.IsNullOrEmpty(url1))
            {
                return UrlUtils.EnsureSameScope(url1, url2);
            }

            string str = url1.TrimEnd(new char[] { '/' });
            char[] chrArray = new char[] { '/' };
            return UrlUtils.EnsureSameScope(url1, string.Concat(str, "/", url2.TrimStart(chrArray)));
        }

        public static string ConvertFullUrlToServerRelative(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            int num = url.IndexOf("://", StringComparison.Ordinal);
            if (num >= 0)
            {
                num = url.IndexOf("/", num + 3, StringComparison.Ordinal);
                if (num < 0)
                {
                    return "/";
                }
            }
            else
            {
                num = url.IndexOf("/", StringComparison.Ordinal);
            }

            return url.Substring(num);
        }

        public static bool EndsWith(string masterUrl, string endingUrl)
        {
            return UrlUtils.EnsureLeadingSlash(UrlUtils.StandardizeFormat(masterUrl)).EndsWith(
                UrlUtils.EnsureLeadingSlash(UrlUtils.StandardizeFormat(endingUrl)),
                StringComparison.InvariantCultureIgnoreCase);
        }

        private static string EnsureEndingSlash(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "/";
            }

            if (url.EndsWith("/", StringComparison.Ordinal))
            {
                return url;
            }

            return string.Concat(url, "/");
        }

        public static string EnsureLeadingSlash(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "/";
            }

            char[] chrArray = new char[] { '/' };
            return string.Concat("/", url.TrimStart(chrArray));
        }

        private static string EnsureSameScope(string orignalUrl, string returnUrl)
        {
            if (string.IsNullOrEmpty(orignalUrl) || !orignalUrl.StartsWith("/", StringComparison.Ordinal))
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return returnUrl;
                }

                return returnUrl.TrimStart(new char[] { '/' });
            }

            return string.Concat("/", (string.IsNullOrEmpty(returnUrl) ? "" : returnUrl.TrimStart(new char[] { '/' })));
        }

        public static bool Equal(string url1, string url2)
        {
            return string.Equals(UrlUtils.StandardizeFormat(url1), UrlUtils.StandardizeFormat(url2));
        }

        public static string GetAfterLastSlash(string url)
        {
            string str;
            string str1;
            UrlUtils.SplitOnLastSlash(url, out str, out str1);
            return str1;
        }

        public static string GetServerUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            int num = url.IndexOf("://", StringComparison.Ordinal);
            num = (num < 0 ? 0 : num + 3);
            if (num >= url.Length)
            {
                return url;
            }

            num = url.IndexOf('/', num);
            if (num < 0)
            {
                return url;
            }

            return url.Substring(0, num);
        }

        public static UrlType GetType(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return UrlType.WebRelative;
            }

            if (url.Contains("://"))
            {
                return UrlType.Full;
            }

            if (url.StartsWith("/", StringComparison.Ordinal))
            {
                return UrlType.ServerRelative;
            }

            return UrlType.WebRelative;
        }

        public static string GetValidSiteUrl(string siteUrl)
        {
            if (string.IsNullOrEmpty(siteUrl))
            {
                throw new ArgumentNullException("siteUrl");
            }

            string str = siteUrl;
            for (int i = 0; i < 2147483647; i++)
            {
                if (i == 2147483647)
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.UrlUtils_GetValidSiteUrl_MaxTriesReached, 2147483647, siteUrl));
                }

                if (string.IsNullOrEmpty(str) || !UrlUtils.RemoveInvalidConsecutiveCharactersInUrl(ref str) &&
                    !UrlUtils.RemoveInvalidCharactersInUrl(ref str) && !UrlUtils.RemoveInvalidStringsInUrl(ref str) &&
                    !UrlUtils.TrimStartAndEndOfInvalidCharactersInUrl(ref str) &&
                    !UrlUtils.TrimUrlExceeding128Characters(ref str))
                {
                    break;
                }
            }

            return str;
        }

        public static bool MakeRelativeUrl(string masterUrl, string parentUrl, out string result)
        {
            result = null;
            if (!UrlUtils.ReplaceStart(masterUrl, parentUrl, "", out result))
            {
                return false;
            }

            char[] chrArray = new char[] { '/' };
            result = result.TrimStart(chrArray);
            return true;
        }

        public static string RemoveAfterLastSlash(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            string str = url.TrimEnd(new char[] { '/' });
            int num = str.LastIndexOf('/');
            if (num < 0)
            {
                return UrlUtils.EnsureSameScope(url, "");
            }

            return UrlUtils.EnsureSameScope(url, str.Substring(0, num));
        }

        private static bool RemoveInvalidCharactersInUrl(ref string siteUrl)
        {
            bool flag = false;
            foreach (char chr in new List<char>("~\"#%&*:<>?\\{|}".ToCharArray())
                     {
                         '\u007F'
                     })
            {
                if (!siteUrl.Contains<char>(chr))
                {
                    continue;
                }

                siteUrl = siteUrl.Replace(chr.ToString(CultureInfo.InvariantCulture), string.Empty);
                flag = true;
            }

            return flag;
        }

        private static bool RemoveInvalidConsecutiveCharactersInUrl(ref string siteUrl)
        {
            bool flag = false;
            string str = "/.";
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                string str1 = string.Format("{0}{0}", chr);
                while (siteUrl.Contains(str1))
                {
                    siteUrl = siteUrl.Replace(str1, chr.ToString(CultureInfo.InvariantCulture));
                    flag = true;
                }
            }

            return flag;
        }

        private static bool RemoveInvalidStringsInUrl(ref string siteUrl)
        {
            int num = siteUrl.IndexOf("/wpresources", StringComparison.OrdinalIgnoreCase);
            if (num == -1)
            {
                return false;
            }

            siteUrl = siteUrl.Remove(num, "/wpresources".Length);
            return true;
        }

        public static bool ReplaceStart(string masterUrl, string originalStart, string newStart, out string result)
        {
            if (!UrlUtils.StartsWith(masterUrl, originalStart))
            {
                result = masterUrl;
                return false;
            }

            string str = UrlUtils.StandardizeFormat(masterUrl);
            string str1 = UrlUtils.StandardizeFormat(originalStart);
            string str2 = str.Remove(0, str1.Length);
            string[] strArrays = new string[] { UrlUtils.StandardizeFormat(newStart), str2 };
            result = UrlUtils.EnsureSameScope(masterUrl, UrlUtils.ConcatUrls(strArrays));
            return true;
        }

        public static bool SameType(string url1, string url2)
        {
            return UrlUtils.GetType(url1) == UrlUtils.GetType(url2);
        }

        public static void SplitOnLastSlash(string url, out string beforeSlash, out string afterSlash)
        {
            beforeSlash = null;
            afterSlash = url;
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            int num = url.LastIndexOf('/');
            if (num < 0)
            {
                return;
            }

            beforeSlash = url.Substring(0, num);
            num++;
            afterSlash = (num >= url.Length ? "" : url.Substring(num));
        }

        public static StandardizedUrl Standardize(SharePointAdapter adapter, string url)
        {
            return StandardizedUrl.StandardizeUrl(adapter, url);
        }

        public static string StandardizeFormat(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            string str = HttpUtility.UrlPathEncode(url.ToLower());
            char[] chrArray = new char[] { '/' };
            return UrlUtils.EnsureSameScope(url, str.TrimEnd(chrArray));
        }

        public static bool StartsWith(string masterUrl, string leadingUrl)
        {
            return UrlUtils.EnsureEndingSlash(UrlUtils.StandardizeFormat(masterUrl)).StartsWith(
                UrlUtils.EnsureEndingSlash(UrlUtils.StandardizeFormat(leadingUrl)),
                StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool TrimStartAndEndOfInvalidCharactersInUrl(ref string siteUrl)
        {
            string str = siteUrl.Trim(" ./".ToCharArray());
            bool flag = !string.Equals(str, siteUrl);
            siteUrl = str;
            return flag;
        }

        private static bool TrimUrlExceeding128Characters(ref string siteUrl)
        {
            if (siteUrl.Length <= 128)
            {
                return false;
            }

            siteUrl = siteUrl.Substring(0, 128);
            return true;
        }
    }
}