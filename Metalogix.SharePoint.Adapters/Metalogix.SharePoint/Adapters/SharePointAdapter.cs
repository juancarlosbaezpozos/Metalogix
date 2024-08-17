using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public abstract class SharePointAdapter : IDisposable
    {
        private static Dictionary<string, Type> s_shortNameToType;

        private readonly static object s_shortNameToTypeLock;

        private WebProxy m_proxy;

        private X509CertificateWrapperCollection m_includedCertificates = new X509CertificateWrapperCollection();

        private Metalogix.Permissions.Credentials m_proxyCredentials;

        private static bool s_bEnableAdapterLogging;

        private static string s_sAdapterLogFileLocation;

        private bool m_bSavePassword;

        private string m_sAuthenticationType;

        private Metalogix.SharePoint.Adapters.SystemInfo m_SystemInfo;

        private Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion;

        private Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer m_authenticationInitializer;

        private Metalogix.SharePoint.Adapters.Authentication.CookieManager m_cookieManager;

        private bool m_bIsReadOnlyAdapter;

        private static object s_oPrimingLock;

        private static bool s_bWebServicesPrimed;

        private Metalogix.SharePoint.Adapters.ConnectionScope _connectionScope;

        private AdapterSupportedFlags? _supportedFlags = null;

        private string _adapterShortName;

        private string _connectionTypeDisplayString;

        private SharePointServerAdapterConfig m_serverAdapterConfig;

        private bool m_bDisposed;

        public static string AdapterLogFileLocation
        {
            get { return SharePointAdapter.s_sAdapterLogFileLocation; }
            set { SharePointAdapter.s_sAdapterLogFileLocation = value; }
        }

        public virtual WebProxy AdapterProxy
        {
            get { return this.m_proxy; }
            set { this.m_proxy = value; }
        }

        public virtual string AdapterShortName
        {
            get
            {
                if (this._adapterShortName == null)
                {
                    this._adapterShortName = AdapterShortNameAttribute.GetAdapterShortName(this.GetType());
                }

                return this._adapterShortName;
            }
        }

        public virtual Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer AuthenticationInitializer
        {
            get { return this.m_authenticationInitializer; }
            set { this.m_authenticationInitializer = value; }
        }

        public Type AuthenticationInitializerType
        {
            get
            {
                if (this.AuthenticationInitializer == null)
                {
                    return null;
                }

                return this.AuthenticationInitializer.GetType();
            }
        }

        public string AuthenticationType
        {
            get { return this.m_sAuthenticationType; }
            set { this.m_sAuthenticationType = value; }
        }

        public static Type[] AvailableAdapterTypes
        {
            get { return TypeCatalog.AvailableAdapterTypes; }
        }

        public Metalogix.Permissions.AzureAdGraphCredentials AzureAdGraphCredentials { get; set; }

        public virtual bool CanIdentifyDifferentWorkflows
        {
            get { return this.SupportsWorkflows; }
        }

        public ISharePointAdapterCommand Command
        {
            get { return (ISharePointAdapterCommand)this; }
        }

        public virtual Metalogix.SharePoint.Adapters.ConnectionScope ConnectionScope
        {
            get { return this._connectionScope; }
            set { this._connectionScope = value; }
        }

        public virtual string ConnectionTypeDisplayString
        {
            get
            {
                if (this._connectionTypeDisplayString == null)
                {
                    this._connectionTypeDisplayString =
                        AdapterDisplayNameAttribute.GetAdapterDisplayName(this.GetType());
                }

                return this._connectionTypeDisplayString;
            }
        }

        public virtual Metalogix.SharePoint.Adapters.Authentication.CookieManager CookieManager
        {
            get { return this.m_cookieManager; }
            set { this.m_cookieManager = value; }
        }

        public abstract Metalogix.Permissions.Credentials Credentials { get; set; }

        public bool CredentialsAreDefault
        {
            get
            {
                if (this.Credentials == null)
                {
                    return true;
                }

                return this.Credentials.IsDefault;
            }
        }

        public virtual string DisplayedShortName
        {
            get { return this.AdapterShortName; }
        }

        public static bool EnableAdapterLogging
        {
            get { return SharePointAdapter.s_bEnableAdapterLogging; }
            set { SharePointAdapter.s_bEnableAdapterLogging = value; }
        }

        public abstract Metalogix.SharePoint.Adapters.ExternalizationSupport ExternalizationSupport { get; }

        public bool HasActiveCookieManager
        {
            get
            {
                if (this.CookieManager == null)
                {
                    return false;
                }

                return this.CookieManager.IsActive;
            }
        }

        public virtual X509CertificateWrapperCollection IncludedCertificates
        {
            get { return this.m_includedCertificates; }
            set
            {
                if (value == null)
                {
                    this.m_includedCertificates = new X509CertificateWrapperCollection();
                    return;
                }

                this.m_includedCertificates = value;
            }
        }

        public bool IsBPOSConnection
        {
            get { return this.Server.IndexOf(".microsoftonline.com", StringComparison.OrdinalIgnoreCase) > -1; }
        }

        public virtual bool IsClientOM
        {
            get { return false; }
        }

        public bool IsDataLimitExceededForContentUnderMgmt { get; set; }

        public virtual bool IsDB
        {
            get { return false; }
        }

        public virtual bool IsMEWS
        {
            get { return false; }
        }

        public virtual bool IsNws
        {
            get { return false; }
        }

        public abstract bool IsPortal2003Connection { get; }

        public virtual bool IsReadOnlyAdapter
        {
            get { return this.m_bIsReadOnlyAdapter; }
            set { this.m_bIsReadOnlyAdapter = value; }
        }

        public virtual bool IsReadOnlyLicense
        {
            get { return !AdapterLicensing.AllowWrite(this); }
        }

        public string LoggedInAs
        {
            get { return this.Credentials.UserName; }
        }

        public virtual Metalogix.Permissions.Credentials ProxyCredentials
        {
            get { return this.m_proxyCredentials; }
            set
            {
                ICredentials networkCredentials;
                this.m_proxyCredentials = value;
                if (this.m_proxy != null)
                {
                    WebProxy mProxy = this.m_proxy;
                    if (value == null)
                    {
                        networkCredentials = null;
                    }
                    else
                    {
                        networkCredentials = value.NetworkCredentials;
                    }

                    mProxy.Credentials = networkCredentials;
                }
            }
        }

        public abstract ISharePointReader Reader { get; }

        public bool SavePassword
        {
            get { return this.m_bSavePassword; }
            set { this.m_bSavePassword = value; }
        }

        public abstract string Server { get; }

        public bool ServerAdapterConfigIsNull
        {
            get { return this.m_serverAdapterConfig == null; }
        }

        public virtual SharePointServerAdapterConfig ServerAdapterConfiguration
        {
            get
            {
                if (this.m_serverAdapterConfig == null)
                {
                    this.m_serverAdapterConfig = new SharePointServerAdapterConfig(this);
                }

                return this.m_serverAdapterConfig;
            }
            set { this.m_serverAdapterConfig = value; }
        }

        public virtual string ServerDisplayName
        {
            get { return this.Server; }
        }

        public virtual string ServerLinkName
        {
            get { return this.Server; }
        }

        public abstract string ServerRelativeUrl { get; }

        public abstract string ServerType { get; }

        public abstract string ServerUrl { get; }

        public Metalogix.SharePoint.Adapters.SharePointVersion SharePointVersion
        {
            get
            {
                if (this.m_SharePointVersion == null)
                {
                    this.m_SharePointVersion =
                        new Metalogix.SharePoint.Adapters.SharePointVersion(this.Reader.GetSharePointVersion());
                }

                return this.m_SharePointVersion;
            }
        }

        private static Dictionary<string, Type> ShortNameToType
        {
            get
            {
                Dictionary<string, Type> sShortNameToType = SharePointAdapter.s_shortNameToType;
                if (sShortNameToType == null)
                {
                    lock (SharePointAdapter.s_shortNameToTypeLock)
                    {
                        sShortNameToType = SharePointAdapter.s_shortNameToType;
                        if (sShortNameToType == null)
                        {
                            sShortNameToType =
                                new Dictionary<string, Type>((int)SharePointAdapter.AvailableAdapterTypes.Length);
                            Type[] availableAdapterTypes = SharePointAdapter.AvailableAdapterTypes;
                            for (int i = 0; i < (int)availableAdapterTypes.Length; i++)
                            {
                                Type type = availableAdapterTypes[i];
                                sShortNameToType.Add(AdapterShortNameAttribute.GetAdapterShortName(type), type);
                            }

                            SharePointAdapter.s_shortNameToType = sShortNameToType;
                        }
                    }
                }

                return sShortNameToType;
            }
        }

        public virtual AdapterSupportedFlags SupportedFlags
        {
            get
            {
                if (!this._supportedFlags.HasValue)
                {
                    this._supportedFlags =
                        new AdapterSupportedFlags?(AdapterSupportsAttribute.GetAdapterSupportedFlags(this.GetType()));
                }

                return this._supportedFlags.Value;
            }
        }

        public virtual bool SupportsIDPreservation
        {
            get { return true; }
        }

        public virtual bool SupportsTaxonomy
        {
            get { return false; }
        }

        public virtual bool SupportsWorkflows
        {
            get { return false; }
        }

        public virtual bool SupportsWritingAuthorshipData
        {
            get
            {
                if (this.Writer == null)
                {
                    return false;
                }

                if (!this.IsNws)
                {
                    return true;
                }

                return this.IsClientOM;
            }
        }

        public Metalogix.SharePoint.Adapters.SystemInfo SystemInfo
        {
            get
            {
                if (this.m_SystemInfo == null)
                {
                    this.m_SystemInfo = new Metalogix.SharePoint.Adapters.SystemInfo(this.Reader.GetSystemInfo());
                }

                return this.m_SystemInfo;
            }
        }

        public abstract string Url { get; set; }

        public abstract string WebID { get; set; }

        public abstract ISharePointWriter Writer { get; }

        static SharePointAdapter()
        {
            SharePointAdapter.s_shortNameToType = null;
            SharePointAdapter.s_shortNameToTypeLock = new object();
            SharePointAdapter.s_bEnableAdapterLogging = false;
            SharePointAdapter.s_sAdapterLogFileLocation = null;
            SharePointAdapter.s_oPrimingLock = new object();
            SharePointAdapter.s_bWebServicesPrimed = false;
        }

        public SharePointAdapter()
        {
            SharePointAdapter.PrimeWebServices();
        }

        public virtual bool AdapterEquals(XmlNode adapterXML)
        {
            if (this.AdapterShortName != adapterXML.Attributes["AdapterType"].Value)
            {
                return false;
            }

            if (this.Url != adapterXML.Attributes["Url"].Value)
            {
                return false;
            }

            XmlAttribute itemOf = adapterXML.Attributes["ReadOnly"];
            if (itemOf != null && itemOf.Value != this.m_bIsReadOnlyAdapter.ToString())
            {
                return false;
            }

            XmlAttribute xmlAttribute = adapterXML.Attributes["UserName"];
            if (xmlAttribute != null)
            {
                if (this.Credentials.IsDefault)
                {
                    return false;
                }

                if (xmlAttribute.Value != this.Credentials.UserName)
                {
                    return false;
                }
            }
            else if (!this.Credentials.IsDefault)
            {
                return false;
            }

            XmlNode xmlNodes = adapterXML.SelectSingleNode("./Proxy");
            if (xmlNodes != null)
            {
                if (this.m_proxy == null || this.m_proxy.Address == null)
                {
                    return false;
                }

                if (this.m_proxy.Address.AbsoluteUri != xmlNodes.Attributes["Url"].Value)
                {
                    return false;
                }

                XmlAttribute itemOf1 = xmlNodes.Attributes["UserName"];
                if (xmlAttribute != null)
                {
                    if (this.ProxyCredentials.IsDefault)
                    {
                        return false;
                    }

                    if (itemOf1.Value != this.ProxyCredentials.UserName)
                    {
                        return false;
                    }
                }
                else if (!this.ProxyCredentials.IsDefault)
                {
                    return false;
                }
            }
            else if (this.m_proxy != null && this.m_proxy.Address != null)
            {
                return false;
            }

            return true;
        }

        protected virtual Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer
            AuthenticationInitFromXml(XmlNode node)
        {
            Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer authenticationInitializer;
            Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer autoDetectInitializer =
                new AutoDetectInitializer();
            XmlAttribute itemOf = node.Attributes["AuthenticationType"];
            if (itemOf == null)
            {
                return autoDetectInitializer;
            }

            try
            {
                Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer authenticationInitializer1 =
                    Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer.Create(itemOf.Value);
                authenticationInitializer = (authenticationInitializer1 == null
                    ? autoDetectInitializer
                    : authenticationInitializer1);
            }
            catch
            {
                authenticationInitializer = autoDetectInitializer;
            }

            return authenticationInitializer;
        }

        public abstract void CheckConnection();

        public abstract SharePointAdapter Clone();

        public abstract SharePointAdapter CloneForNewSiteCollection();

        public static SharePointAdapter Create(string adapterXML)
        {
            return SharePointAdapter.Create(XmlUtility.StringToXmlNode(adapterXML));
        }

        public static SharePointAdapter Create(XmlNode adapterNode)
        {
            if (adapterNode == null)
            {
                throw new ArgumentNullException("adapterNode");
            }

            if (adapterNode.Attributes["AdapterType"] == null)
            {
                throw new Exception("Invalid parameter. The 'AdapterType' attribute was missing");
            }

            string value = adapterNode.Attributes["AdapterType"].Value;
            SharePointAdapter sharePointAdapter = SharePointAdapter.CreateByShortName(value);
            sharePointAdapter.FromXML(adapterNode);
            return sharePointAdapter;
        }

        public static SharePointAdapter Create(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (!type.IsSubclassOf(typeof(SharePointAdapter)))
            {
                throw new Exception(string.Concat("The given type is not an adapter type: ", type.FullName));
            }

            return Activator.CreateInstance(type) as SharePointAdapter;
        }

        public static SharePointAdapter CreateByShortName(string shortName)
        {
            Type type;
            if (!SharePointAdapter.ShortNameToType.TryGetValue(shortName, out type))
            {
                throw new Exception(string.Concat("Invalid adapter short name: '", shortName, "'"));
            }

            return SharePointAdapter.Create(type);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (!this.m_bDisposed && bDisposing)
            {
                this.CookieManager = null;
                if (this.Credentials != null)
                {
                    this.Credentials.Dispose();
                    this.Credentials = null;
                }

                if (this.m_proxyCredentials != null)
                {
                    this.m_proxyCredentials.Dispose();
                    this.m_proxyCredentials = null;
                }

                this.m_bDisposed = true;
            }
        }

        public virtual void FromXML(XmlNode xml)
        {
            try
            {
                if (xml.Attributes["Url"] == null)
                {
                    throw new Exception("The 'Url' attribute was missing");
                }

                this.Url = xml.Attributes["Url"].Value;
                XmlAttribute itemOf = xml.Attributes["ReadOnly"];
                if (itemOf != null)
                {
                    bool flag = false;
                    if (bool.TryParse(itemOf.Value, out flag))
                    {
                        this.IsReadOnlyAdapter = flag;
                    }
                }

                Metalogix.Permissions.Credentials credential = new Metalogix.Permissions.Credentials(xml);
                Metalogix.Permissions.AzureAdGraphCredentials azureAdGraphCredential =
                    new Metalogix.Permissions.AzureAdGraphCredentials(xml);
                X509CertificateWrapperCollection x509CertificateWrapperCollection = null;
                XmlNode xmlNodes = xml.SelectSingleNode("./IncludedCertificates");
                if (xmlNodes != null)
                {
                    x509CertificateWrapperCollection =
                        X509CertificateWrapperCollection.BuildCollectionFromXml(xmlNodes);
                }

                Metalogix.SharePoint.Adapters.Authentication.AuthenticationInitializer authenticationInitializer =
                    this.AuthenticationInitFromXml(xml);
                authenticationInitializer.Credentials = credential;
                authenticationInitializer.Certificates = x509CertificateWrapperCollection;
                authenticationInitializer.InitializeAuthenticationSettings(this);
                this.ProxyFromXml(xml.SelectSingleNode("./Proxy"));
                this.AzureAdGraphCredentials = azureAdGraphCredential;
            }
            catch (ArgumentException argumentException)
            {
                throw;
            }
            catch (Exception exception)
            {
            }
        }

        public static bool GetAdapterSupportsScope(Type type, Metalogix.SharePoint.Adapters.ConnectionScope scope)
        {
            return SharePointAdapter.GetAdapterSupportsScope(AdapterSupportsAttribute.GetAdapterSupportedFlags(type),
                scope);
        }

        public static bool GetAdapterSupportsScope(AdapterSupportedFlags supportedFlags,
            Metalogix.SharePoint.Adapters.ConnectionScope scope)
        {
            switch (scope)
            {
                case Metalogix.SharePoint.Adapters.ConnectionScope.Site:
                {
                    return (supportedFlags & AdapterSupportedFlags.SiteScope) == AdapterSupportedFlags.SiteScope;
                }
                case Metalogix.SharePoint.Adapters.ConnectionScope.WebApp:
                {
                    return (supportedFlags & AdapterSupportedFlags.WebAppScope) == AdapterSupportedFlags.WebAppScope;
                }
                case Metalogix.SharePoint.Adapters.ConnectionScope.Farm:
                {
                    return (supportedFlags & AdapterSupportedFlags.FarmScope) == AdapterSupportedFlags.FarmScope;
                }
                case Metalogix.SharePoint.Adapters.ConnectionScope.Tenant:
                {
                    return (supportedFlags & AdapterSupportedFlags.TenantScope) == AdapterSupportedFlags.TenantScope;
                }
            }

            return false;
        }

        public static SharePointAdapter GetDBAdapter(object[] parameters)
        {
            Type type = Type.GetType(string.Format(
                "Metalogix.SharePoint.Adapters.DB.DBAdapter, Metalogix.SharePoint.Adapters.DB, Version={0}, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4",
                Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            if (type == null)
            {
                return null;
            }

            return (SharePointAdapter)Activator.CreateInstance(type, parameters);
        }

        public abstract string GetServerVersion();

        public bool IsReadOnly()
        {
            if (this.IsReadOnlyAdapter || this.IsReadOnlyLicense)
            {
                return true;
            }

            return this.IsDataLimitExceededForContentUnderMgmt;
        }

        protected static void PrimeWebServices()
        {
            lock (SharePointAdapter.s_oPrimingLock)
            {
                if (!SharePointAdapter.s_bWebServicesPrimed)
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        (RemoteCertificateValidationCallback)Delegate.Combine(
                            ServicePointManager.ServerCertificateValidationCallback,
                            new RemoteCertificateValidationCallback(CertificateValidator.CertificationValidator));
                    SharePointAdapter.s_bWebServicesPrimed = true;
                }
            }
        }

        protected virtual void ProxyFromXml(XmlNode xml)
        {
            if (xml == null || xml.Attributes["Url"] == null)
            {
                this.m_proxy = new WebProxy();
                return;
            }

            string value = xml.Attributes["Url"].Value;
            this.m_proxy = new WebProxy(value);
            this.ProxyCredentials = new Metalogix.Permissions.Credentials(xml);
        }

        public virtual void Refresh()
        {
        }

        protected void SetSharePointVersion(Metalogix.SharePoint.Adapters.SharePointVersion version)
        {
            this.m_SharePointVersion = version;
        }

        protected void SetSystemInfo(Metalogix.SharePoint.Adapters.SystemInfo info)
        {
            this.m_SystemInfo = info;
        }

        public void SetUrlForRedirect(string url)
        {
            if (!this.CookieManager.UsesCookieRepository)
            {
                this.Url = url;
                return;
            }

            string key = CookieRepository.GetKey(this);
            this.Url = url;
            CookieRepository.MapCookieKey(key, CookieRepository.GetKey(this));
        }

        public bool SupportsConnectionScope(Metalogix.SharePoint.Adapters.ConnectionScope scope)
        {
            return SharePointAdapter.GetAdapterSupportsScope(this.SupportedFlags, scope);
        }

        public virtual string ToXML()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("SharePointAdapter");
            this.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public virtual void ToXML(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("AdapterType", this.AdapterShortName);
            xmlWriter.WriteAttributeString("Url", this.Url);
            this.Credentials.ToXML(xmlWriter);
            if (this.AzureAdGraphCredentials != null)
            {
                this.AzureAdGraphCredentials.ToXML(xmlWriter);
            }

            xmlWriter.WriteAttributeString("ReadOnly", this.m_bIsReadOnlyAdapter.ToString());
            if (!string.IsNullOrEmpty(this.AuthenticationType))
            {
                xmlWriter.WriteAttributeString("AuthenticationType", this.AuthenticationType);
            }
            else if (this.AuthenticationInitializer != null)
            {
                xmlWriter.WriteAttributeString("AuthenticationType", this.AuthenticationInitializer.GetType().FullName);
            }

            if (this.m_proxy != null && this.m_proxy.Address != null)
            {
                xmlWriter.WriteStartElement("Proxy");
                xmlWriter.WriteAttributeString("Url", this.m_proxy.Address.AbsoluteUri);
                if (this.ProxyCredentials != null && !this.ProxyCredentials.IsDefault)
                {
                    xmlWriter.WriteAttributeString("UserName", this.ProxyCredentials.UserName);
                    xmlWriter.WriteAttributeString("Password", this.ProxyCredentials.Password.ToInsecureString());
                }

                xmlWriter.WriteEndElement();
            }

            if (this.m_includedCertificates != null && this.m_includedCertificates.Count > 0)
            {
                this.m_includedCertificates.ToXml(xmlWriter);
            }
        }
    }
}