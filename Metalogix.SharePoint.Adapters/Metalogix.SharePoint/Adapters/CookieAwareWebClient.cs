using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Metalogix.SharePoint.Adapters
{
    public class CookieAwareWebClient : WebClient
    {
        private HttpWebRequest myRequest;

        private System.Net.CookieContainer m_container;

        private X509CertificateCollection m_clientCertificates;

        public X509CertificateCollection ClientCertificates
        {
            get { return this.m_clientCertificates; }
            set { this.m_clientCertificates = value; }
        }

        public System.Net.CookieContainer CookieContainer
        {
            get { return this.m_container; }
            set { this.m_container = value; }
        }

        public CookieAwareWebClient()
        {
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            this.myRequest = (HttpWebRequest)base.GetWebRequest(address);
            this.myRequest.UserAgent =
                "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 1.1.4322; .NET CLR 3.5.30729; .NET CLR 3.0.30729; MS-RTC LM 8)";
            if (this.CookieContainer != null)
            {
                this.myRequest.CookieContainer = this.CookieContainer;
            }

            if (this.ClientCertificates != null)
            {
                this.myRequest.ClientCertificates = this.ClientCertificates;
            }

            return this.myRequest;
        }

        [DebuggerStepThrough]
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            return this.myRequest.GetResponse();
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            return this.myRequest.EndGetResponse(result);
        }
    }
}