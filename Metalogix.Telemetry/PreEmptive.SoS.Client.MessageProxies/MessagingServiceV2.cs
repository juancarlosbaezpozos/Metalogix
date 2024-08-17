using PreEmptive.SoS.Client.Cache;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [DesignerCategory("Code")]
    [WebServiceBinding(Name = "MessagingServiceV2Soap", Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class MessagingServiceV2 : SoapHttpClientProtocol
    {
        public MessagingServiceV2()
        {
            base.Url = "http://so-s.info/PreEmptive.Web.Services.Messaging/MessagingServiceV2.asmx";
        }

        public IAsyncResult BeginPublish(PreEmptive.SoS.Client.MessageProxies.MessageCache MessageCache,
            AsyncCallback callback, object asyncState)
        {
            object[] messageCache = new object[] { MessageCache };
            return base.BeginInvoke("Publish", messageCache, callback, asyncState);
        }

        public void EndPublish(IAsyncResult asyncResult)
        {
            base.EndInvoke(asyncResult);
        }

        protected override WebRequest GetWebRequest(System.Uri uri_0)
        {
            HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest(uri_0);
            webRequest.KeepAlive = false;
            string[] strArrays = new string[] { "ttl", "large-envelope-splitting" };
            Array.Sort<string>(strArrays);
            webRequest.Headers.Add("X-RI-Capabilities", string.Join(",", strArrays));
            string applicationId = CacheServiceConfiguration.ApplicationId;
            if (applicationId != null)
            {
                webRequest.Headers.Add("X-RI-AID", applicationId);
            }

            string companyId = CacheServiceConfiguration.CompanyId;
            if (companyId != null)
            {
                webRequest.Headers.Add("X-RI-CID", companyId);
            }

            return webRequest;
        }

        [SoapDocumentMethod("http://schemas.preemptive.com/services/messaging/Publish",
            RequestNamespace = "http://schemas.preemptive.com/services/messaging", OneWay = true,
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void Publish(
            [XmlElement(IsNullable = true)] PreEmptive.SoS.Client.MessageProxies.MessageCache MessageCache)
        {
            object[] messageCache = new object[] { MessageCache };
            base.Invoke("Publish", messageCache);
        }
    }
}