using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public abstract class BaseServiceWrapper : IWebServiceWrapper, IDisposable
    {
        protected SharePointAdapter m_Parent;

        protected SoapHttpClientProtocol m_wrappedService;

        private bool m_bDisposed;

        public X509CertificateCollection ClientCertificates
        {
            get { return this.m_wrappedService.ClientCertificates; }
        }

        public System.Net.CookieContainer CookieContainer
        {
            get { return this.m_wrappedService.CookieContainer; }
            set { this.m_wrappedService.CookieContainer = value; }
        }

        public ICredentials Credentials
        {
            get { return this.m_wrappedService.Credentials; }
            set { this.m_wrappedService.Credentials = value; }
        }

        protected bool Disposed
        {
            get { return this.m_bDisposed; }
            set { this.m_bDisposed = value; }
        }

        public SharePointAdapter ParentAdapter
        {
            get { return this.m_Parent; }
        }

        public IWebProxy Proxy
        {
            get { return this.m_wrappedService.Proxy; }
            set { this.m_wrappedService.Proxy = value; }
        }

        public int Timeout
        {
            get { return this.m_wrappedService.Timeout; }
            set { this.m_wrappedService.Timeout = value; }
        }

        public string Url
        {
            get { return this.m_wrappedService.Url; }
            set { this.m_wrappedService.Url = value; }
        }

        public SoapHttpClientProtocol WrappedService
        {
            get { return this.m_wrappedService; }
        }

        protected BaseServiceWrapper()
        {
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
                this.m_wrappedService.Dispose();
                this.m_wrappedService = null;
                this.m_bDisposed = true;
            }
        }

        protected void InitializeWrappedWebService(string sWebServiceAsmxName)
        {
            string url = this.ParentAdapter.Url;
            char[] chrArray = new char[] { '/' };
            this.Url = string.Concat(url.TrimEnd(chrArray), "/_vti_bin/", sWebServiceAsmxName, ".asmx");
            this.Timeout = WebServiceWrapperUtilities.WebServiceTimeoutTime;
            this.Credentials = this.ParentAdapter.Credentials.NetworkCredentials;
            this.ParentAdapter.IncludedCertificates.CopyCertificatesToCollection(this.ClientCertificates);
            if (this.ParentAdapter.AdapterProxy != null)
            {
                this.Proxy = this.ParentAdapter.AdapterProxy;
            }

            if (this.ParentAdapter.HasActiveCookieManager)
            {
                this.CookieContainer = new System.Net.CookieContainer();
                this.ParentAdapter.CookieManager.AddCookiesTo(this.CookieContainer);
            }
        }
    }
}