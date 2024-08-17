using Metalogix.SharePoint.Adapters;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public interface IWebServiceWrapper
    {
        X509CertificateCollection ClientCertificates { get; }

        System.Net.CookieContainer CookieContainer { get; set; }

        ICredentials Credentials { get; set; }

        SharePointAdapter ParentAdapter { get; }

        IWebProxy Proxy { get; set; }

        int Timeout { get; set; }

        string Url { get; set; }

        SoapHttpClientProtocol WrappedService { get; }
    }
}