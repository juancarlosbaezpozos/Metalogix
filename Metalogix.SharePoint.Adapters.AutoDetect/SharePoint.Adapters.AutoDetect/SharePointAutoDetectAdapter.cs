using Metalogix.DataStructures;
using Metalogix.Licensing;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.CSOM2013Client;
using Metalogix.SharePoint.Adapters.MLWS;
using Metalogix.SharePoint.Adapters.NWS;
using Metalogix.SharePoint.Adapters.NWS.SiteData;
using Metalogix.SharePoint.Adapters.OM;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.Utilities;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.AutoDetect
{
    [AdapterDisplayName("Auto Detect")]
    [AdapterShortName("SP")]
    [AdapterSupports(AdapterSupportedFlags.SiteScope | AdapterSupportedFlags.WebAppScope |
                     AdapterSupportedFlags.FarmScope | AdapterSupportedFlags.LegacyLicense |
                     AdapterSupportedFlags.CurrentLicense)]
    [MenuOrder(1)]
    [ShowInMenu(true)]
    public class SharePointAutoDetectAdapter : SharePointAdapter, IAutoDetectAdapter, IServerHealthMonitor,
        ISharePointAdapterCommand
    {
        private Metalogix.Permissions.Credentials m_credentials;

        private string m_sUrl;

        private SharePointAdapter m_internalAdapter;

        private bool m_bDisposed;

        public override WebProxy AdapterProxy
        {
            get { return base.AdapterProxy; }
            set
            {
                base.AdapterProxy = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.AdapterProxy = value;
                }
            }
        }

        public override Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer AuthenticationInitializer
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return base.AuthenticationInitializer;
                }

                return this.m_internalAdapter.AuthenticationInitializer;
            }
            set
            {
                base.AuthenticationInitializer = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.AuthenticationInitializer = value;
                }
            }
        }

        public override string ConnectionTypeDisplayString
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return base.ConnectionTypeDisplayString;
                }

                return this.InternalAdapter.ConnectionTypeDisplayString;
            }
        }

        public override Metalogix.SharePoint.Adapters.Authentication.CookieManager CookieManager
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return base.CookieManager;
                }

                return this.m_internalAdapter.CookieManager;
            }
            set
            {
                base.CookieManager = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.CookieManager = value;
                }
            }
        }

        public override Metalogix.Permissions.Credentials Credentials
        {
            get { return this.m_credentials; }
            set
            {
                this.m_credentials = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.Credentials = value;
                }
            }
        }

        public override string DisplayedShortName
        {
            get
            {
                if (this.m_internalAdapter != null)
                {
                    return this.InternalAdapterShortName;
                }

                return this.AdapterShortName;
            }
        }

        public override Metalogix.SharePoint.Adapters.ExternalizationSupport ExternalizationSupport
        {
            get { return this.InternalAdapter.ExternalizationSupport; }
        }

        public override X509CertificateWrapperCollection IncludedCertificates
        {
            get { return base.IncludedCertificates; }
            set
            {
                base.IncludedCertificates = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.IncludedCertificates = value;
                }
            }
        }

        private SharePointAdapter InternalAdapter
        {
            get
            {
                string str;
                string str1;
                SharePointAdapter mInternalAdapter;
                if (this.m_internalAdapter != null)
                {
                    return this.m_internalAdapter;
                }

                if (!base.CredentialsAreDefault && this.Credentials.Password.IsNullOrEmpty())
                {
                    throw new UnauthorizedAccessException("A password is required");
                }

                if (this.AuthenticationInitializer == null ||
                    this.AuthenticationInitializer.CompatibleWithAdapter("OM"))
                {
                    try
                    {
                        if ((AdapterSupportsAttribute.GetAdapterSupportedFlags(typeof(OMAdapter)) &
                             AdapterSupportedFlags.LegacyLicense) == AdapterSupportedFlags.LegacyLicense ||
                            LicensingUtils.GetLevel() == CompatibilityLevel.Current)
                        {
                            SharePointAdapter localAdapter = this.GetLocalAdapter(this.Url, this.Credentials);
                            localAdapter.CheckConnection();
                            localAdapter.IsReadOnlyAdapter = this.IsReadOnlyAdapter;
                            this.InternalAdapter = localAdapter;
                            this.Url = this.m_internalAdapter.Url;
                            mInternalAdapter = this.m_internalAdapter;
                            return mInternalAdapter;
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                }

                try
                {
                    this.TestRemoteSharepoint(out str, out str1);
                }
                catch (Exception exception2)
                {
                    Exception exception1 = exception2;
                    if (exception1.Message.Contains("timed out") || exception1.Message.Contains("resolved"))
                    {
                        throw new ServerProblem(exception1.Message);
                    }

                    throw new NoSharePoint(string.Concat("Could not connect to remote SharePoint server: '", this.Url,
                        "'. ", exception1.Message));
                }

                if (this.AuthenticationInitializer == null ||
                    this.AuthenticationInitializer.CompatibleWithAdapter("WS"))
                {
                    try
                    {
                        SharePointAdapter remoteAdapter = this.GetRemoteAdapter(str1, this.Credentials);
                        if (this.AdapterProxy != null)
                        {
                            remoteAdapter.AdapterProxy = this.AdapterProxy;
                            remoteAdapter.ProxyCredentials = this.ProxyCredentials;
                        }

                        remoteAdapter.CheckConnection();
                        remoteAdapter.IsReadOnlyAdapter = this.IsReadOnlyAdapter;
                        this.InternalAdapter = remoteAdapter;
                        this.Url = this.m_internalAdapter.Url;
                        mInternalAdapter = this.m_internalAdapter;
                        return mInternalAdapter;
                    }
                    catch (Exception exception4)
                    {
                        Exception exception3 = exception4;
                        if (exception3.Message.Contains("401:"))
                        {
                            throw new UnauthorizedAccessException(exception3.Message);
                        }
                    }
                }

                if (this.ConnectionScope != Metalogix.SharePoint.Adapters.ConnectionScope.Site)
                {
                    throw new NoSharePoint(
                        "Cannot find the Metalogix Web Extensions on the target. Please ensure that the correct version of the Web Extensions is installed on the target, or try connecting at the Site Level. ");
                }

                try
                {
                    SharePointAdapter sPWSAdapter = this.GetSPWSAdapter(this.Url, this.Credentials);
                    if (this.AdapterProxy != null)
                    {
                        sPWSAdapter.AdapterProxy = this.AdapterProxy;
                        sPWSAdapter.ProxyCredentials = this.ProxyCredentials;
                    }

                    if (sPWSAdapter.SharePointVersion.IsSharePoint2013OrLater)
                    {
                        if (LicensingUtils.GetLevel() == CompatibilityLevel.Legacy)
                        {
                            throw new MLLicenseException(Resources.SharePoint_Not_Supported_By_License);
                        }

                        sPWSAdapter = this.GetCSOMAdapter(this.Url, this.Credentials);
                    }

                    sPWSAdapter.CheckConnection();
                    sPWSAdapter.IsReadOnlyAdapter = this.IsReadOnlyAdapter;
                    this.InternalAdapter = sPWSAdapter;
                    this.Url = this.m_internalAdapter.Url;
                }
                catch (SoapException soapException)
                {
                    string soapError = RPCUtil.GetSoapError(soapException.Detail);
                    throw new NoSharePoint(string.Concat("Could not connect to remote Native SharePoint WebService: '",
                        this.Url, "'. ", soapError));
                }

                return this.m_internalAdapter;
            }
            set { this.m_internalAdapter = value; }
        }

        public string InternalAdapterShortName
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return this.AdapterShortName;
                }

                return this.m_internalAdapter.AdapterShortName;
            }
        }

        public override bool IsClientOM
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return base.IsClientOM;
                }

                return this.m_internalAdapter.IsClientOM;
            }
        }

        public override bool IsDB
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return false;
                }

                return this.InternalAdapter.IsDB;
            }
        }

        public override bool IsNws
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return false;
                }

                return this.InternalAdapter.IsNws;
            }
        }

        public override bool IsPortal2003Connection
        {
            get { return this.InternalAdapter.IsPortal2003Connection; }
        }

        public override bool IsReadOnlyAdapter
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return base.IsReadOnlyAdapter;
                }

                return this.m_internalAdapter.IsReadOnlyAdapter;
            }
            set
            {
                base.IsReadOnlyAdapter = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.IsReadOnlyAdapter = value;
                }
            }
        }

        public override bool IsReadOnlyLicense
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return base.IsReadOnlyLicense;
                }

                return this.m_internalAdapter.IsReadOnlyLicense;
            }
        }

        public override Metalogix.Permissions.Credentials ProxyCredentials
        {
            get { return base.ProxyCredentials; }
            set
            {
                base.ProxyCredentials = value;
                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.ProxyCredentials = value;
                }
            }
        }

        public override ISharePointReader Reader
        {
            get { return this.InternalAdapter.Reader; }
        }

        public override string Server
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return Utils.GetServerPart(this.Url);
                }

                return this.InternalAdapter.Server;
            }
        }

        public override SharePointServerAdapterConfig ServerAdapterConfiguration
        {
            get
            {
                if (base.ServerAdapterConfigIsNull)
                {
                    base.ServerAdapterConfiguration = this.InternalAdapter.ServerAdapterConfiguration;
                }

                return base.ServerAdapterConfiguration;
            }
        }

        public override string ServerRelativeUrl
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return Utils.GetServerRelativeUrlPart(this.Url);
                }

                return this.InternalAdapter.ServerRelativeUrl;
            }
        }

        public override string ServerType
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return "Auto Detect";
                }

                return this.InternalAdapter.ServerType;
            }
        }

        public override string ServerUrl
        {
            get
            {
                if (this.m_internalAdapter == null)
                {
                    return Utils.GetServerPart(this.Url);
                }

                return this.InternalAdapter.Server;
            }
        }

        public override AdapterSupportedFlags SupportedFlags
        {
            get { return this.InternalAdapter.SupportedFlags; }
        }

        public override bool SupportsIDPreservation
        {
            get { return this.InternalAdapter.SupportsIDPreservation; }
        }

        public override bool SupportsTaxonomy
        {
            get { return this.InternalAdapter.SupportsTaxonomy; }
        }

        public override bool SupportsWorkflows
        {
            get { return this.InternalAdapter.SupportsWorkflows; }
        }

        public override string Url
        {
            get { return this.m_sUrl; }
            set
            {
                this.m_sUrl = value;
                if (this.m_internalAdapter != null && this.m_internalAdapter.Url != this.m_sUrl)
                {
                    this.m_internalAdapter.Url = value;
                }
            }
        }

        public override string WebID
        {
            get { return this.InternalAdapter.WebID; }
            set { this.InternalAdapter.WebID = value; }
        }

        public override ISharePointWriter Writer
        {
            get
            {
                if (base.IsReadOnly())
                {
                    return null;
                }

                return this.InternalAdapter.Writer;
            }
        }

        public SharePointAutoDetectAdapter()
        {
        }

        public override void CheckConnection()
        {
            this.InternalAdapter.CheckConnection();
        }

        public override SharePointAdapter Clone()
        {
            SharePointAutoDetectAdapter sharePointAutoDetectAdapter = new SharePointAutoDetectAdapter();
            sharePointAutoDetectAdapter.CloneFrom(this, true);
            return sharePointAutoDetectAdapter;
        }

        public override SharePointAdapter CloneForNewSiteCollection()
        {
            SharePointAutoDetectAdapter sharePointAutoDetectAdapter = new SharePointAutoDetectAdapter();
            sharePointAutoDetectAdapter.CloneFrom(this, false);
            return sharePointAutoDetectAdapter;
        }

        private void CloneFrom(SharePointAutoDetectAdapter adapter, bool bIncludeSiteCollectionSpecificProperties)
        {
            SharePointAdapter sharePointAdapter;
            sharePointAdapter = (!bIncludeSiteCollectionSpecificProperties
                ? adapter.InternalAdapter.CloneForNewSiteCollection()
                : adapter.InternalAdapter.Clone());
            this.InternalAdapter = sharePointAdapter;
            this.m_credentials = adapter.m_credentials;
            base.AzureAdGraphCredentials = adapter.AzureAdGraphCredentials;
            this.m_sUrl = adapter.m_sUrl;
            this.IsReadOnlyAdapter = adapter.IsReadOnlyAdapter;
            base.IsDataLimitExceededForContentUnderMgmt = adapter.IsDataLimitExceededForContentUnderMgmt;
            this.ServerAdapterConfiguration = adapter.ServerAdapterConfiguration;
        }

        protected override void Dispose(bool bDisposing)
        {
            base.Dispose(bDisposing);
            if (!this.m_bDisposed && bDisposing)
            {
                if (this.m_credentials != null)
                {
                    this.m_credentials.Dispose();
                    this.m_credentials = null;
                }

                if (this.m_internalAdapter != null)
                {
                    this.m_internalAdapter.Dispose();
                    this.m_internalAdapter = null;
                }

                this.m_bDisposed = true;
            }
        }

        public string ExecuteCommand(string commandName, string commandConfigurationXml)
        {
            return this.InternalAdapter.Command.ExecuteCommand(commandName, commandConfigurationXml);
        }

        protected virtual SharePointAdapter GetCSOMAdapter(string url, Metalogix.Permissions.Credentials credentials)
        {
            CSOMClientAdapter cSOMClientAdapter = new CSOMClientAdapter(url, credentials)
            {
                CookieManager = this.CookieManager,
                IncludedCertificates = this.IncludedCertificates,
                AuthenticationInitializer = this.AuthenticationInitializer
            };
            if (this.AdapterProxy != null)
            {
                cSOMClientAdapter.AdapterProxy = this.AdapterProxy;
                cSOMClientAdapter.ProxyCredentials = this.ProxyCredentials;
            }

            return cSOMClientAdapter;
        }

        protected virtual SharePointAdapter GetLocalAdapter(string url, Metalogix.Permissions.Credentials credentials)
        {
            return new OMAdapter(url, credentials);
        }

        protected virtual SharePointAdapter GetRemoteAdapter(string url, Metalogix.Permissions.Credentials credentials)
        {
            MLWSAdapter mLWSAdapter = new MLWSAdapter(url, credentials)
            {
                CookieManager = this.CookieManager,
                IncludedCertificates = this.IncludedCertificates,
                AuthenticationInitializer = this.AuthenticationInitializer
            };
            return mLWSAdapter;
        }

        public string GetServerHealth()
        {
            IServerHealthMonitor internalAdapter = this.InternalAdapter as IServerHealthMonitor;
            if (internalAdapter == null)
            {
                return null;
            }

            return internalAdapter.GetServerHealth();
        }

        public override string GetServerVersion()
        {
            return this.InternalAdapter.GetServerVersion();
        }

        protected virtual SharePointAdapter GetSPWSAdapter(string url, Metalogix.Permissions.Credentials credentials)
        {
            NWSAdapter nWSAdapter = new NWSAdapter(url, credentials)
            {
                CookieManager = this.CookieManager,
                IncludedCertificates = this.IncludedCertificates,
                AuthenticationInitializer = this.AuthenticationInitializer
            };
            return nWSAdapter;
        }

        public override void Refresh()
        {
            this.InternalAdapter.Refresh();
        }

        protected virtual void TestRemoteSharepoint(out string sSite, out string sWeb)
        {
            Regex regex = new Regex("http://[^/]*/");
            if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
            {
                this.CookieManager.AquireCookieLock();
            }

            try
            {
                Metalogix.SharePoint.Adapters.NWS.SiteData.SiteData siteDatum =
                    new Metalogix.SharePoint.Adapters.NWS.SiteData.SiteData();
                if (this.AdapterProxy != null)
                {
                    siteDatum.Proxy = this.AdapterProxy;
                }

                this.IncludedCertificates.CopyCertificatesToCollection(siteDatum.ClientCertificates);
                string url = siteDatum.Url;
                string str = string.Concat(this.Url, "/");
                str = regex.Replace(url, str);
                siteDatum.Url = str;
                siteDatum.Credentials = this.Credentials.NetworkCredentials;
                siteDatum.Timeout = 10000;
                if (base.HasActiveCookieManager)
                {
                    siteDatum.CookieContainer = new CookieContainer();
                    this.CookieManager.AddCookiesTo(siteDatum.CookieContainer);
                }

                string str1 = null;
                string str2 = str.Substring(this.Url.Length);
                try
                {
                    siteDatum.GetSiteAndWeb(this.Url, out sSite, out sWeb);
                }
                catch (WebException webException)
                {
                    HttpWebResponse response = webException.Response as HttpWebResponse;
                    if (!Utils.ResponseIsRedirect(response))
                    {
                        throw;
                    }
                    else
                    {
                        string item = response.Headers["Location"];
                        if (item == null)
                        {
                            throw;
                        }

                        if (!item.EndsWith(str2))
                        {
                            throw;
                        }

                        str1 = item;
                        sSite = null;
                        sWeb = null;
                    }
                }

                if (str1 != null)
                {
                    string str3 = str1.Substring(0, str1.Length - str2.Length);
                    siteDatum.Url = str1;
                    siteDatum.GetSiteAndWeb(str3, out sSite, out sWeb);
                    base.SetUrlForRedirect(str3);
                }
            }
            finally
            {
                if (base.HasActiveCookieManager && this.CookieManager.LockCookie)
                {
                    this.CookieManager.ReleaseCookieLock();
                }
            }
        }
    }
}