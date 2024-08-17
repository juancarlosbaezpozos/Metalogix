using Metalogix;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Metalogix.Licensing.SK
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("wsdl", "2.0.50727.42")]
    [WebServiceBinding(Name = "LicenseServiceSoap", Namespace = "http://license.metalogix.com/webservices/")]
    [XmlInclude(typeof(object[]))]
    public class BPOSLicenseService : SoapHttpClientProtocol
    {
        private AuthenticationHeader authenticationHeaderValueField;

        private SendOrPostCallback SetAssociationsOperationCompleted;

        private SendOrPostCallback BPOSCheckLicenseOperationCompleted;

        private SendOrPostCallback CheckLicenseOperationCompleted;

        private SendOrPostCallback GetPartnerOperationCompleted;

        private SendOrPostCallback DisplayUserOperationCompleted;

        private SendOrPostCallback GenerateLicenseOperationCompleted;

        public AuthenticationHeader AuthenticationHeaderValue
        {
            get { return this.authenticationHeaderValueField; }
            set { this.authenticationHeaderValueField = value; }
        }

        internal BPOSLicenseService(string url)
        {
            base.Url = url;
        }

        public BPOSLicenseService()
        {
            base.Url = "http://valasek-xp2/License/LicenseService.asmx";
        }

        public IAsyncResult BeginBPOSCheckLicense(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("BPOSCheckLicense", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginCheckLicense(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("CheckLicense", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginDisplayUser(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("DisplayUser", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginGenerateLicense(LicenseInfo licenseInfo, AsyncCallback callback, object asyncState)
        {
            object[] objArray = new object[] { licenseInfo };
            return base.BeginInvoke("GenerateLicense", objArray, callback, asyncState);
        }

        public IAsyncResult BeginGetPartner(AsyncCallback callback, object asyncState)
        {
            return base.BeginInvoke("GetPartner", new object[0], callback, asyncState);
        }

        public IAsyncResult BeginSetAssociations(Association[] associations, string[] partnerIds,
            AsyncCallback callback, object asyncState)
        {
            object[] objArray = new object[] { associations, partnerIds };
            return base.BeginInvoke("SetAssociations", objArray, callback, asyncState);
        }

        [SoapDocumentMethod("http://license.metalogix.com/webservices/BPOSCheckLicense",
            RequestNamespace = "http://license.metalogix.com/webservices/",
            ResponseNamespace = "http://license.metalogix.com/webservices/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("AuthenticationHeaderValue")]
        public BPOSLicenseInfo BPOSCheckLicense()
        {
            object[] objArray = base.Invoke("BPOSCheckLicense", new object[0]);
            return (BPOSLicenseInfo)objArray[0];
        }

        public void BPOSCheckLicenseAsync()
        {
            this.BPOSCheckLicenseAsync(null);
        }

        public void BPOSCheckLicenseAsync(object userState)
        {
            if (this.BPOSCheckLicenseOperationCompleted == null)
            {
                this.BPOSCheckLicenseOperationCompleted =
                    new SendOrPostCallback(this.OnBPOSCheckLicenseOperationCompleted);
            }

            base.InvokeAsync("BPOSCheckLicense", new object[0], this.BPOSCheckLicenseOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://license.metalogix.com/webservices/CheckLicense",
            RequestNamespace = "http://license.metalogix.com/webservices/",
            ResponseNamespace = "http://license.metalogix.com/webservices/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("AuthenticationHeaderValue")]
        public Association[] CheckLicense()
        {
            object[] objArray = base.Invoke("CheckLicense", new object[0]);
            return (Association[])objArray[0];
        }

        public void CheckLicenseAsync()
        {
            this.CheckLicenseAsync(null);
        }

        public void CheckLicenseAsync(object userState)
        {
            if (this.CheckLicenseOperationCompleted == null)
            {
                this.CheckLicenseOperationCompleted = new SendOrPostCallback(this.OnCheckLicenseOperationCompleted);
            }

            base.InvokeAsync("CheckLicense", new object[0], this.CheckLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://license.metalogix.com/webservices/DisplayUser",
            RequestNamespace = "http://license.metalogix.com/webservices/",
            ResponseNamespace = "http://license.metalogix.com/webservices/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DisplayUser()
        {
            base.Invoke("DisplayUser", new object[0]);
        }

        public void DisplayUserAsync()
        {
            this.DisplayUserAsync(null);
        }

        public void DisplayUserAsync(object userState)
        {
            if (this.DisplayUserOperationCompleted == null)
            {
                this.DisplayUserOperationCompleted = new SendOrPostCallback(this.OnDisplayUserOperationCompleted);
            }

            base.InvokeAsync("DisplayUser", new object[0], this.DisplayUserOperationCompleted, userState);
        }

        public BPOSLicenseInfo EndBPOSCheckLicense(IAsyncResult asyncResult)
        {
            return (BPOSLicenseInfo)base.EndInvoke(asyncResult)[0];
        }

        public Association[] EndCheckLicense(IAsyncResult asyncResult)
        {
            return (Association[])base.EndInvoke(asyncResult)[0];
        }

        public void EndDisplayUser(IAsyncResult asyncResult)
        {
            base.EndInvoke(asyncResult);
        }

        public LicenseInfo EndGenerateLicense(IAsyncResult asyncResult)
        {
            return (LicenseInfo)base.EndInvoke(asyncResult)[0];
        }

        public PartnerInfo EndGetPartner(IAsyncResult asyncResult)
        {
            return (PartnerInfo)base.EndInvoke(asyncResult)[0];
        }

        public Association[] EndSetAssociations(IAsyncResult asyncResult)
        {
            return (Association[])base.EndInvoke(asyncResult)[0];
        }

        [SoapDocumentMethod("http://license.metalogix.com/webservices/GenerateLicense",
            RequestNamespace = "http://license.metalogix.com/webservices/",
            ResponseNamespace = "http://license.metalogix.com/webservices/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public LicenseInfo GenerateLicense(LicenseInfo licenseInfo)
        {
            object[] objArray = new object[] { licenseInfo };
            return (LicenseInfo)base.Invoke("GenerateLicense", objArray)[0];
        }

        public void GenerateLicenseAsync(LicenseInfo licenseInfo)
        {
            this.GenerateLicenseAsync(licenseInfo, null);
        }

        public void GenerateLicenseAsync(LicenseInfo licenseInfo, object userState)
        {
            if (this.GenerateLicenseOperationCompleted == null)
            {
                this.GenerateLicenseOperationCompleted =
                    new SendOrPostCallback(this.OnGenerateLicenseOperationCompleted);
            }

            object[] objArray = new object[] { licenseInfo };
            base.InvokeAsync("GenerateLicense", objArray, this.GenerateLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://license.metalogix.com/webservices/GetPartner",
            RequestNamespace = "http://license.metalogix.com/webservices/",
            ResponseNamespace = "http://license.metalogix.com/webservices/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("AuthenticationHeaderValue")]
        public PartnerInfo GetPartner()
        {
            object[] objArray = base.Invoke("GetPartner", new object[0]);
            return (PartnerInfo)objArray[0];
        }

        public void GetPartnerAsync()
        {
            this.GetPartnerAsync(null);
        }

        public void GetPartnerAsync(object userState)
        {
            if (this.GetPartnerOperationCompleted == null)
            {
                this.GetPartnerOperationCompleted = new SendOrPostCallback(this.OnGetPartnerOperationCompleted);
            }

            base.InvokeAsync("GetPartner", new object[0], this.GetPartnerOperationCompleted, userState);
        }

        protected override WebRequest GetWebRequest(System.Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest(uri);
            Logger.Debug.Write("GetWebRequest called for BPOSLicenseService");
            ILogMethods debug = Logger.Debug;
            bool keepAlive = webRequest.KeepAlive;
            debug.Write(string.Concat("KeepAlive=", keepAlive.ToString()));
            Logger.Debug.Write(string.Concat("ProtocolVersion=", webRequest.ProtocolVersion.ToString()));
            webRequest.KeepAlive = false;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            return webRequest;
        }

        private void OnBPOSCheckLicenseOperationCompleted(object arg)
        {
            if (this.BPOSCheckLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.BPOSCheckLicenseCompleted(this,
                    new BPOSCheckLicenseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCheckLicenseOperationCompleted(object arg)
        {
            if (this.CheckLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CheckLicenseCompleted(this,
                    new CheckLicenseCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnDisplayUserOperationCompleted(object arg)
        {
            if (this.DisplayUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DisplayUserCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGenerateLicenseOperationCompleted(object arg)
        {
            if (this.GenerateLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GenerateLicenseCompleted(this,
                    new GenerateLicenseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetPartnerOperationCompleted(object arg)
        {
            if (this.GetPartnerCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetPartnerCompleted(this,
                    new GetPartnerCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetAssociationsOperationCompleted(object arg)
        {
            if (this.SetAssociationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetAssociationsCompleted(this,
                    new SetAssociationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://license.metalogix.com/webservices/SetAssociations",
            RequestNamespace = "http://license.metalogix.com/webservices/",
            ResponseNamespace = "http://license.metalogix.com/webservices/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("AuthenticationHeaderValue")]
        public Association[] SetAssociations(Association[] associations, string[] partnerIds)
        {
            object[] objArray = new object[] { associations, partnerIds };
            return (Association[])base.Invoke("SetAssociations", objArray)[0];
        }

        public void SetAssociationsAsync(Association[] associations, string[] partnerIds)
        {
            this.SetAssociationsAsync(associations, partnerIds, null);
        }

        public void SetAssociationsAsync(Association[] associations, string[] partnerIds, object userState)
        {
            if (this.SetAssociationsOperationCompleted == null)
            {
                this.SetAssociationsOperationCompleted =
                    new SendOrPostCallback(this.OnSetAssociationsOperationCompleted);
            }

            object[] objArray = new object[] { associations, partnerIds };
            base.InvokeAsync("SetAssociations", objArray, this.SetAssociationsOperationCompleted, userState);
        }

        public event BPOSCheckLicenseCompletedEventHandler BPOSCheckLicenseCompleted;

        public event CheckLicenseCompletedEventHandler CheckLicenseCompleted;

        public event DisplayUserCompletedEventHandler DisplayUserCompleted;

        public event GenerateLicenseCompletedEventHandler GenerateLicenseCompleted;

        public event GetPartnerCompletedEventHandler GetPartnerCompleted;

        public event SetAssociationsCompletedEventHandler SetAssociationsCompleted;
    }
}