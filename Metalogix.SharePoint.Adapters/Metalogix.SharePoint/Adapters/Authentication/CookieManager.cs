using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public abstract class CookieManager
    {
        private SharePointAdapter m_adapter;

        private ReadOnlyCollection<Cookie> m_cookies;

        private bool m_bIsActive = true;

        private readonly object m_cookieLock = new object();

        public SharePointAdapter Adapter
        {
            get { return this.m_adapter; }
        }

        public virtual object CookieLockObject
        {
            get { return this.m_cookieLock; }
        }

        public ReadOnlyCollection<Cookie> Cookies
        {
            get
            {
                if (this.m_cookies == null)
                {
                    this.UpdateCookiesInternal();
                }

                return this.m_cookies;
            }
        }

        public bool HasCookie
        {
            get { return this.m_cookies != null; }
        }

        public virtual bool IsActive
        {
            get
            {
                if (!this.HasCookie)
                {
                    ReadOnlyCollection<Cookie> cookies = this.Cookies;
                }

                return this.m_bIsActive;
            }
            protected set { this.m_bIsActive = value; }
        }

        public abstract bool LockCookie { get; }

        public virtual bool UsesCookieRepository
        {
            get { return false; }
        }

        public CookieManager(SharePointAdapter adapter)
        {
            this.m_adapter = adapter;
        }

        public void AddCookiesTo(CookieContainer container)
        {
            foreach (Cookie cooky in this.Cookies)
            {
                container.Add(cooky);
            }
        }

        public virtual void AquireCookieLock()
        {
            Monitor.Enter(this.CookieLockObject);
        }

        public void ClearCookie()
        {
            this.m_cookies = null;
        }

        public void EnsureCookies()
        {
            if (this.IsActive && !this.HasCookie)
            {
                ReadOnlyCollection<Cookie> cookies = this.Cookies;
            }
        }

        protected abstract IList<Cookie> GetCookie();

        public string GetCookieString()
        {
            StringBuilder stringBuilder = new StringBuilder(1000);
            bool flag = true;
            foreach (Cookie cooky in this.Cookies)
            {
                if (!flag)
                {
                    stringBuilder.Append("; ");
                }

                stringBuilder.Append(cooky.ToString());
                flag = false;
            }

            return stringBuilder.ToString();
        }

        public virtual void ReleaseCookieLock()
        {
            Monitor.Exit(this.CookieLockObject);
        }

        public virtual void SetCookies(IList<Cookie> cookies)
        {
            if (cookies == null)
            {
                this.m_cookies = null;
                return;
            }

            this.m_cookies = new ReadOnlyCollection<Cookie>(cookies);
        }

        public virtual void UpdateCookie()
        {
            this.UpdateCookiesInternal();
        }

        protected void UpdateCookiesInternal()
        {
            IList<Cookie> cookie = this.GetCookie();
            if (cookie != null)
            {
                this.m_cookies = new ReadOnlyCollection<Cookie>(cookie);
                return;
            }

            this.m_cookies = new ReadOnlyCollection<Cookie>(new Cookie[0]);
        }
    }
}