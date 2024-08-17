using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class CookieRepository
    {
        protected static ReaderWriterLockSlim s_readWriteLock;

        protected static Dictionary<string, IList<Cookie>> s_office365Cookies;

        protected static Dictionary<string, object> s_cookieLocks;

        private readonly static object s_o365SingleAccessLock;

        public static object O365SingleAccessLock
        {
            get { return CookieRepository.s_o365SingleAccessLock; }
        }

        static CookieRepository()
        {
            CookieRepository.s_readWriteLock = new ReaderWriterLockSlim();
            CookieRepository.s_office365Cookies = new Dictionary<string, IList<Cookie>>();
            CookieRepository.s_cookieLocks = new Dictionary<string, object>();
            CookieRepository.s_o365SingleAccessLock = new object();
        }

        public CookieRepository()
        {
        }

        public static object GetCookieLock(string key)
        {
            object obj;
            object obj1;
            CookieRepository.s_readWriteLock.EnterReadLock();
            try
            {
                obj1 = (!CookieRepository.s_cookieLocks.TryGetValue(key, out obj) ? null : obj);
            }
            finally
            {
                CookieRepository.s_readWriteLock.ExitReadLock();
            }

            return obj1;
        }

        public static IList<Cookie> GetCookies(string key)
        {
            IList<Cookie> cookies;
            IList<Cookie> cookies1;
            CookieRepository.s_readWriteLock.EnterReadLock();
            try
            {
                if (!CookieRepository.s_office365Cookies.TryGetValue(key, out cookies))
                {
                    cookies1 = null;
                }
                else
                {
                    cookies1 = cookies;
                }
            }
            finally
            {
                CookieRepository.s_readWriteLock.ExitReadLock();
            }

            return cookies1;
        }

        public static string GetKey(SharePointAdapter adapter)
        {
            string str;
            str = (!adapter.CredentialsAreDefault
                ? string.Concat(adapter.Credentials.UserName.ToLowerInvariant(), adapter.Credentials.Password)
                : "<Default Credentials>");
            return string.Concat(str.ToLowerInvariant(), adapter.Server.ToLowerInvariant());
        }

        public static void MapCookieKey(string oldUrl, string newUrl)
        {
            CookieRepository.s_readWriteLock.EnterWriteLock();
            try
            {
                if (CookieRepository.s_office365Cookies.ContainsKey(oldUrl) &&
                    !CookieRepository.s_office365Cookies.ContainsKey(newUrl))
                {
                    IList<Cookie> item = CookieRepository.s_office365Cookies[oldUrl];
                    object obj = CookieRepository.s_cookieLocks[oldUrl];
                    CookieRepository.s_office365Cookies.Add(newUrl, item);
                    CookieRepository.s_cookieLocks.Add(newUrl, obj);
                }
            }
            finally
            {
                CookieRepository.s_readWriteLock.ExitWriteLock();
            }
        }

        public static void StoreCookies(string key, IList<Cookie> value)
        {
            CookieRepository.s_readWriteLock.EnterWriteLock();
            try
            {
                if (!CookieRepository.s_office365Cookies.ContainsKey(key))
                {
                    CookieRepository.s_office365Cookies.Add(key, value);
                    CookieRepository.s_cookieLocks.Add(key, new object());
                }
                else
                {
                    CookieRepository.s_office365Cookies[key] = value;
                }
            }
            finally
            {
                CookieRepository.s_readWriteLock.ExitWriteLock();
            }
        }
    }
}