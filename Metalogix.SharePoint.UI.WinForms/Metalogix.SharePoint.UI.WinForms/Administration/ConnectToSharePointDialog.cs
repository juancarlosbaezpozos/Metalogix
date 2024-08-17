using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Xml;
using Metalogix.Connectivity.Proxy;
using Metalogix.DataStructures;
using Metalogix.MLLicensing.Properties;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Proxy;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class ConnectToSharePointDialog : LeftNavigableTabsForm
    {
        private delegate void ConnectionErrorCase();

        private class SharePointConnectionConfiguration : IProxyOptionsContainer, ISharePointConnectionOptionsContainer, ICertificateInclusionOptionsContainer
        {
            public AuthenticationInitializer AuthenticationInitializer { get; set; }

            public AzureAdGraphCredentials AzureAdGraphCredentials { get; set; }

            public IEnumerable<X509CertificateWrapper> Certificates { get; set; }

            public ConnectionScope ConnectionScope { get; set; }

            public Type ConnectionType { get; set; }

            public Credentials Credentials { get; set; }

            public MLProxy Proxy { get; set; }

            public string Url { get; set; }

            public SharePointConnectionConfiguration()
            {
                Proxy = null;
                Url = null;
                Credentials = null;
                AzureAdGraphCredentials = null;
                AuthenticationInitializer = null;
                ConnectionScope = ConnectionScope.Site;
                ConnectionType = null;
            }
        }

        private Container components;

        private SharePointConnectionConfiguration m_configuration;

        private SPConnection m_node;

        private string m_sUrl;

        private MLProxy m_proxy;

        private TCSharePointConnectionConfig m_generalConfigTab = new TCSharePointConnectionConfig();

        private TCProxyConfig m_proxyConfigTab = new TCProxyConfig();

        private TCCertificateInclusionConfig m_certificateConfigTab = new TCCertificateInclusionConfig();

        public bool ConnectAsReadOnly { get; set; }

        public SPConnection ConnectionNode => m_node;

        public bool EnableLocationEditing
        {
            get
            {
                return m_generalConfigTab.EnableLocationEditing;
            }
            set
            {
                m_generalConfigTab.EnableLocationEditing = value;
            }
        }

        public MLProxy Proxy => m_proxy;

        public string Url => m_sUrl;

        public ConnectToSharePointDialog(SPConnection conn)
        {
            InitializeComponent();
            m_node = conn;
            m_configuration = new SharePointConnectionConfiguration();
            if (conn.Adapter.AdapterProxy == null || !(conn.Adapter.AdapterProxy.Address != null))
            {
                m_configuration.Proxy = new MLProxy
                {
                    Enabled = false
                };
            }
            else
            {
                MLProxy mLProxy = new MLProxy
                {
                    Enabled = true
                };
                mLProxy.SetUrl(conn.Adapter.AdapterProxy.Address.ToString());
                mLProxy.Credentials = (conn.Adapter.ProxyCredentials.IsDefault ? new Credentials() : new Credentials(conn.Adapter.ProxyCredentials.UserName, conn.Adapter.ProxyCredentials.Password, bSavePassword: true));
                m_configuration.Proxy = mLProxy;
            }
            if (conn is SPServer)
            {
                m_configuration.ConnectionScope = ((!(conn as SPServer).ShowAllSites) ? ConnectionScope.WebApp : ConnectionScope.Farm);
            }
            else if (!(conn is SPTenant))
            {
                m_configuration.ConnectionScope = ConnectionScope.Site;
            }
            else
            {
                m_configuration.ConnectionScope = ConnectionScope.Tenant;
            }
            m_configuration.ConnectionType = conn.Adapter.GetType();
            m_configuration.AuthenticationInitializer = conn.Adapter.AuthenticationInitializer;
            m_configuration.Credentials = conn.Credentials;
            m_configuration.AzureAdGraphCredentials = conn.AzureAdGraphCredentials;
            m_configuration.Certificates = conn.Adapter.IncludedCertificates;
            ConnectAsReadOnly = conn.Adapter.IsReadOnlyAdapter;
            try
            {
                m_configuration.Url = ((conn.DisplayUrl.Length > conn.Url.Length) ? conn.DisplayUrl : conn.Url);
            }
            catch (Exception)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(conn.Connection.ConnectionString);
                XmlNode firstChild = xmlDocument.FirstChild;
                m_configuration.Url = firstChild.Attributes["Url"].Value;
            }
            Initialize();
        }

        public ConnectToSharePointDialog()
        {
            InitializeComponent();
            m_configuration = new SharePointConnectionConfiguration();
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            List<TabbableControl> tabs = new List<TabbableControl> { m_generalConfigTab, m_proxyConfigTab, m_certificateConfigTab };
            m_generalConfigTab.Options = m_configuration;
            m_proxyConfigTab.Options = m_configuration;
            m_certificateConfigTab.Options = m_configuration;
            base.Tabs = tabs;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.ConnectToSharePointDialog));
            ((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(base.w_btnCancel, "w_btnCancel");
            resources.ApplyResources(base.w_btnOK, "w_btnOK");
            base.tabControl.LookAndFeel.SkinName = "Office 2013";
            base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
            resources.ApplyResources(base.tabControl, "tabControl");
            base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("ConnectToSharePointDialog.Appearance.BackColor");
            base.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this, "$this");
            base.LookAndFeel.SkinName = "Office 2013";
            base.Name = "ConnectToSharePointDialog";
            ((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
            base.ResumeLayout(false);
        }

        protected override bool SaveUI()
        {
            ConnectionErrorCase connectionErrorCase = null;
            if (!base.SaveUI())
            {
                return false;
            }
            bool result = false;
            ConnectionErrorCase connectionErrorCase2 = null;
            SPConnection mNode = null;
            MLProxy proxy = null;
            string url = null;
            if (PleaseWaitDialog.ShowWaitDialog("Connecting to " + m_configuration.Url + " ...", delegate(BackgroundWorker worker)
                {
                    try
                    {
                        mNode = m_node;
                        AuthenticationInitializer authenticationInitializer = m_configuration.AuthenticationInitializer;
                        SharePointAdapter sharePointAdapter = ((mNode != null) ? mNode.Adapter : ((SharePointAdapter)Activator.CreateInstance(m_configuration.ConnectionType)));
                        sharePointAdapter.IsReadOnlyAdapter = ConnectAsReadOnly;
                        if (!worker.CancellationPending)
                        {
                            Credentials credentials = m_configuration.Credentials;
                            proxy = m_configuration.Proxy;
                            url = m_configuration.Url;
                            X509CertificateWrapperCollection certificates = new X509CertificateWrapperCollection(m_configuration.Certificates);
                            if (!worker.CancellationPending)
                            {
                                sharePointAdapter.Url = url;
                                sharePointAdapter.ConnectionScope = m_configuration.ConnectionScope;
                                if (proxy == null || !proxy.Enabled)
                                {
                                    sharePointAdapter.AdapterProxy = new WebProxy();
                                }
                                else
                                {
                                    sharePointAdapter.AdapterProxy = new WebProxy(proxy.Server, int.Parse(proxy.Port));
                                    sharePointAdapter.ProxyCredentials = (proxy.Credentials.IsDefault ? new Credentials() : new Credentials(proxy.Credentials.UserName, proxy.Credentials.Password, bSavePassword: true));
                                }
                                if (!worker.CancellationPending)
                                {
                                    authenticationInitializer.Credentials = (credentials.IsDefault ? Credentials.DefaultCredentials : new Credentials(credentials.UserName, credentials.Password, credentials.SavePassword));
                                    authenticationInitializer.Certificates = certificates;
                                    authenticationInitializer.InitializeAuthenticationSettings(sharePointAdapter);
                                    sharePointAdapter.AzureAdGraphCredentials = m_configuration.AzureAdGraphCredentials;
                                    if (!worker.CancellationPending)
                                    {
                                        if (mNode == null)
                                        {
                                            switch (m_configuration.ConnectionScope)
                                            {
                                                case ConnectionScope.WebApp:
                                                    mNode = new SPServer(sharePointAdapter)
                                                    {
                                                        ShowAllSites = false
                                                    };
                                                    break;
                                                case ConnectionScope.Farm:
                                                    mNode = new SPServer(sharePointAdapter);
                                                    break;
                                                case ConnectionScope.Tenant:
                                                    mNode = new SPTenant(sharePointAdapter);
                                                    break;
                                                default:
                                                    mNode = new SPWeb(sharePointAdapter);
                                                    break;
                                            }
                                        }
                                        mNode.CheckConnection(bBypassManualConnectBlock: true);
                                    }
                                }
                            }
                        }
                    }
                    catch (NoMLSP)
                    {
                        if (connectionErrorCase == null)
                        {
                            connectionErrorCase = delegate
                            {
                                new ConnectExtensionErrorDialog().ShowDialog();
                            };
                        }
                        connectionErrorCase2 = connectionErrorCase;
                    }
                    catch (InsufficentIEVersionException ex)
                    {
                        InsufficentIEVersionException insufficentIEVersionException = ex;
                        connectionErrorCase2 = delegate
                        {
                            string message = string.Format(Metalogix.SharePoint.Properties.Resources.InsufficientIEMessage, insufficentIEVersionException.ExpectedVersion, insufficentIEVersionException.DetectedVersion);
                            GlobalServices.ErrorHandler.HandleException(Metalogix.SharePoint.Properties.Resources.InsufficientIETitle, message, insufficentIEVersionException, ErrorIcon.Warning);
                        };
                    }
                    catch (NoSharePoint noSharePoint2)
                    {
                        NoSharePoint noSharePoint = noSharePoint2;
                        connectionErrorCase2 = delegate
                        {
                            GlobalServices.ErrorHandler.HandleException("Unable to connect to SharePoint", noSharePoint.Message, ErrorIcon.Error);
                        };
                    }
                    catch (Exception ex2)
                    {
                        Exception exception = ex2;
                        connectionErrorCase2 = delegate
                        {
                            GlobalServices.ErrorHandler.HandleException("Unable to connect to SharePoint", exception.Message, exception, ErrorIcon.Error);
                        };
                    }
                }))
            {
                if (connectionErrorCase2 == null)
                {
                    if (!mNode.Adapter.IsReadOnlyAdapter && mNode.Adapter.IsReadOnlyLicense && !VerifyUserActionDialog.GetUserVerification(SharePointConfigurationVariables.ShowReadOnlyConnectionInformation, Metalogix.SharePoint.UI.WinForms.Properties.Resources.ReadOnlyLicensedWarning))
                    {
                        mNode.Dispose();
                        return false;
                    }
                    if (mNode.Adapter.ServerType == "Microsoft SharePoint Native WebService" && !VerifyUserActionDialog.GetUserVerification(SharePointConfigurationVariables.ShowNWSInformationDialog, Metalogix.SharePoint.UI.WinForms.Properties.Resources.NWSWarningInformation))
                    {
                        mNode.Dispose();
                        return false;
                    }
                    if (mNode.Adapter.IsDataLimitExceededForContentUnderMgmt && !VerifyUserActionDialog.GetUserVerification(Metalogix.MLLicensing.Properties.Resources.Licensed_Data_Exceeded, Metalogix.UI.WinForms.Properties.Resources.License_Data_Exceeded_Title))
                    {
                        mNode.Dispose();
                        return false;
                    }
                    m_node = mNode;
                    m_proxy = proxy;
                    m_sUrl = url;
                    result = true;
                }
                else
                {
                    connectionErrorCase2();
                }
            }
            return result;
        }

        public void SetVisitedServers(string[] visitedSPServers)
        {
            m_generalConfigTab.SetVisitedServers(visitedSPServers);
        }
    }
}
