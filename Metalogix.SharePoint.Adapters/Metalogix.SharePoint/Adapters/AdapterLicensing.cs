using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.CMWebService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters
{
    public class AdapterLicensing
    {
        private static ReaderWriterLockSlim rwLock;

        private static volatile bool s_CanSkipWebComponentsCheck;

        private static Dictionary<string, AdapterLicensing.WebComponentsValue> s_webComponentsAvailable;

        static AdapterLicensing()
        {
            AdapterLicensing.rwLock = new ReaderWriterLockSlim();
            AdapterLicensing.s_CanSkipWebComponentsCheck = true;
            AdapterLicensing.s_webComponentsAvailable = new Dictionary<string, AdapterLicensing.WebComponentsValue>();
        }

        public AdapterLicensing()
        {
        }

        public static bool AllowWrite(SharePointAdapter adapter)
        {
            if (adapter == null)
            {
                return false;
            }

            if (AdapterLicensing.s_CanSkipWebComponentsCheck)
            {
                return true;
            }

            return AdapterLicensing.CheckWebComponentsAvailable(adapter);
        }

        private static bool CheckWebComponentsAvailable(SharePointAdapter adapter)
        {
            AdapterLicensing.WebComponentsValue item = null;
            string serverUrl = adapter.ServerUrl;
            bool flag = false;
            AdapterLicensing.rwLock.EnterReadLock();
            try
            {
                if (AdapterLicensing.s_webComponentsAvailable.ContainsKey(serverUrl))
                {
                    item = AdapterLicensing.s_webComponentsAvailable[serverUrl];
                    flag = true;
                }
            }
            finally
            {
                AdapterLicensing.rwLock.ExitReadLock();
            }

            if (!flag)
            {
                AdapterLicensing.rwLock.EnterWriteLock();
                try
                {
                    if (!AdapterLicensing.s_webComponentsAvailable.ContainsKey(serverUrl))
                    {
                        item = new AdapterLicensing.WebComponentsValue();
                        AdapterLicensing.s_webComponentsAvailable.Add(serverUrl, item);
                    }
                    else
                    {
                        item = AdapterLicensing.s_webComponentsAvailable[serverUrl];
                    }
                }
                finally
                {
                    AdapterLicensing.rwLock.ExitWriteLock();
                }
            }

            return item.HasWebComponents(adapter);
        }

        public static void UpdateLicenseData(Dictionary<string, object> arguments)
        {
            try
            {
                AdapterLicensing.s_CanSkipWebComponentsCheck = (arguments.ContainsKey("skipWebChecking")
                    ? (bool)arguments["skipWebChecking"]
                    : false);
            }
            catch (InvalidCastException invalidCastException)
            {
                AdapterLicensing.s_CanSkipWebComponentsCheck = false;
            }

            AdapterLicensing.rwLock.EnterWriteLock();
            try
            {
                foreach (AdapterLicensing.WebComponentsValue value in AdapterLicensing.s_webComponentsAvailable.Values)
                {
                    value.Dispose();
                }

                AdapterLicensing.s_webComponentsAvailable.Clear();
            }
            finally
            {
                AdapterLicensing.rwLock.ExitWriteLock();
            }
        }

        private class WebComponentsValue : IDisposable
        {
            private bool? _hasWebComponents = null;

            private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

            private bool _hasDisposed;

            public WebComponentsValue()
            {
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing && !this._hasDisposed && this._lock != null)
                {
                    this._lock.Dispose();
                    this._lock = null;
                }
            }

            private static bool GetWebComponentsAvailable(SharePointAdapter adapter)
            {
                bool flag = false;
                if (adapter.HasActiveCookieManager && adapter.CookieManager.LockCookie)
                {
                    adapter.CookieManager.AquireCookieLock();
                }

                try
                {
                    Uri uri = new Uri(new Uri(adapter.ServerUrl), "/_vti_bin/ContentMatrix/ContentMatrix.asmx");
                    if (Uri.IsWellFormedUriString(uri.AbsoluteUri, UriKind.Absolute))
                    {
                        using (Metalogix.SharePoint.Adapters.CMWebService.CMWebService webServiceClient =
                               AdapterLicensing.WebComponentsValue.GetWebServiceClient(uri, adapter))
                        {
                            try
                            {
                                flag = webServiceClient.IsActivated();
                            }
                            catch (WebException webException)
                            {
                                flag = false;
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                finally
                {
                    if (adapter.HasActiveCookieManager && adapter.CookieManager.LockCookie)
                    {
                        adapter.CookieManager.ReleaseCookieLock();
                    }
                }

                return flag;
            }

            private static Metalogix.SharePoint.Adapters.CMWebService.CMWebService GetWebServiceClient(Uri targetUri,
                SharePointAdapter adapter)
            {
                Metalogix.SharePoint.Adapters.CMWebService.CMWebService cMWebService =
                    new Metalogix.SharePoint.Adapters.CMWebService.CMWebService()
                    {
                        Url = targetUri.AbsoluteUri,
                        UseDefaultCredentials = true,
                        Credentials = adapter.Credentials.NetworkCredentials
                    };
                if (adapter.IncludedCertificates != null)
                {
                    adapter.IncludedCertificates.CopyCertificatesToCollection(cMWebService.ClientCertificates);
                }

                if (adapter.AdapterProxy != null)
                {
                    cMWebService.Proxy = adapter.AdapterProxy;
                    if (adapter.ProxyCredentials != null)
                    {
                        cMWebService.Proxy.Credentials = adapter.ProxyCredentials.NetworkCredentials;
                    }
                }

                if (adapter.HasActiveCookieManager)
                {
                    cMWebService.CookieContainer = new CookieContainer();
                    adapter.CookieManager.AddCookiesTo(cMWebService.CookieContainer);
                }

                return cMWebService;
            }

            public bool HasWebComponents(SharePointAdapter adapter)
            {
                bool value = false;
                bool flag = false;
                this._lock.EnterReadLock();
                try
                {
                    if (this._hasWebComponents.HasValue)
                    {
                        value = this._hasWebComponents.Value;
                        flag = true;
                    }
                }
                finally
                {
                    this._lock.ExitReadLock();
                }

                if (!flag)
                {
                    this._lock.EnterWriteLock();
                    try
                    {
                        if (!this._hasWebComponents.HasValue)
                        {
                            this._hasWebComponents =
                                new bool?(AdapterLicensing.WebComponentsValue.GetWebComponentsAvailable(adapter));
                            if (!this._hasWebComponents.Value)
                            {
                                this._hasWebComponents = new bool?((adapter.SharePointVersion.IsSharePoint2007
                                    ? true
                                    : adapter.SharePointVersion.IsSharePointOnline));
                            }

                            value = this._hasWebComponents.Value;
                        }
                    }
                    finally
                    {
                        this._lock.ExitWriteLock();
                    }
                }

                return value;
            }
        }
    }
}