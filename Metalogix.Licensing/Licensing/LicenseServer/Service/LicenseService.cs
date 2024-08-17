using Metalogix.Licensing.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.6.1055.0")]
    [WebServiceBinding(Name = "LicenseServiceSoap", Namespace = "http://www.metalogix.com/")]
    [XmlInclude(typeof(SignedResponse))]
    public class LicenseService : SoapHttpClientProtocol
    {
        private SendOrPostCallback LoginOperationCompleted;

        private TokenAuthenticationHeader tokenAuthenticationHeaderValueField;

        private SendOrPostCallback GetLicenseDataOperationCompleted;

        private ClientVersionHeader clientVersionHeaderValueField;

        private SendOrPostCallback UpdateLicenseOperationCompleted;

        private SendOrPostCallback ValidateInstallVersionOperationCompleted;

        private SendOrPostCallback ConvertOldKeyOperationCompleted;

        private SendOrPostCallback UpdateServerSystemInfoOperationCompleted;

        private SendOrPostCallback UpdateServerProductSystemInfoOperationCompleted;

        private SendOrPostCallback EchoOperationCompleted;

        private SendOrPostCallback CreateLicenseOperationCompleted;

        private SendOrPostCallback CreateTrialLicenseOperationCompleted;

        private SendOrPostCallback ModifyLicenseOperationCompleted;

        private SendOrPostCallback GetCustomersOperationCompleted;

        private SendOrPostCallback GetLicensesOperationCompleted;

        private SendOrPostCallback AddProductReleaseOperationCompleted;

        private SendOrPostCallback ModifyProductReleaseOperationCompleted;

        private SendOrPostCallback GetLatestProductReleaseOperationCompleted;

        private SendOrPostCallback AddAdvancedOptionForLicenseOperationCompleted;

        private SendOrPostCallback GetAdvancedOptionForLicenseOperationCompleted;

        private SendOrPostCallback UpdateAdvancedOptionForLicenseOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        public ClientVersionHeader ClientVersionHeaderValue
        {
            get { return this.clientVersionHeaderValueField; }
            set { this.clientVersionHeaderValueField = value; }
        }

        public TokenAuthenticationHeader TokenAuthenticationHeaderValue
        {
            get { return this.tokenAuthenticationHeaderValueField; }
            set { this.tokenAuthenticationHeaderValueField = value; }
        }

        public new string Url
        {
            get { return base.Url; }
            set
            {
                if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly &&
                    !this.IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }

                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get { return base.UseDefaultCredentials; }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public LicenseService()
        {
            this.Url = Settings.Default.Metalogix_Licensing_LicenseServer_Service_LicenseService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://www.metalogix.com/AddAdvancedOptionForLicense",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public int AddAdvancedOptionForLicense(AdvancedOptionRequest advancedOption)
        {
            object[] objArray = new object[] { advancedOption };
            return (int)base.Invoke("AddAdvancedOptionForLicense", objArray)[0];
        }

        public void AddAdvancedOptionForLicenseAsync(AdvancedOptionRequest advancedOption)
        {
            this.AddAdvancedOptionForLicenseAsync(advancedOption, null);
        }

        public void AddAdvancedOptionForLicenseAsync(AdvancedOptionRequest advancedOption, object userState)
        {
            if (this.AddAdvancedOptionForLicenseOperationCompleted == null)
            {
                this.AddAdvancedOptionForLicenseOperationCompleted =
                    new SendOrPostCallback(this.OnAddAdvancedOptionForLicenseOperationCompleted);
            }

            object[] objArray = new object[] { advancedOption };
            base.InvokeAsync("AddAdvancedOptionForLicense", objArray,
                this.AddAdvancedOptionForLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/AddProductRelease",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public void AddProductRelease(AddProductReleaseRequest info)
        {
            object[] objArray = new object[] { info };
            base.Invoke("AddProductRelease", objArray);
        }

        public void AddProductReleaseAsync(AddProductReleaseRequest info)
        {
            this.AddProductReleaseAsync(info, null);
        }

        public void AddProductReleaseAsync(AddProductReleaseRequest info, object userState)
        {
            if (this.AddProductReleaseOperationCompleted == null)
            {
                this.AddProductReleaseOperationCompleted =
                    new SendOrPostCallback(this.OnAddProductReleaseOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("AddProductRelease", objArray, this.AddProductReleaseOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/ConvertOldKey", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public string ConvertOldKey(string oldKey)
        {
            object[] objArray = new object[] { oldKey };
            return (string)base.Invoke("ConvertOldKey", objArray)[0];
        }

        public void ConvertOldKeyAsync(string oldKey)
        {
            this.ConvertOldKeyAsync(oldKey, null);
        }

        public void ConvertOldKeyAsync(string oldKey, object userState)
        {
            if (this.ConvertOldKeyOperationCompleted == null)
            {
                this.ConvertOldKeyOperationCompleted = new SendOrPostCallback(this.OnConvertOldKeyOperationCompleted);
            }

            object[] objArray = new object[] { oldKey };
            base.InvokeAsync("ConvertOldKey", objArray, this.ConvertOldKeyOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/CreateLicense", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LicenseInfo CreateLicense(CreateLicenseRequest info)
        {
            object[] objArray = new object[] { info };
            return (LicenseInfo)base.Invoke("CreateLicense", objArray)[0];
        }

        public void CreateLicenseAsync(CreateLicenseRequest info)
        {
            this.CreateLicenseAsync(info, null);
        }

        public void CreateLicenseAsync(CreateLicenseRequest info, object userState)
        {
            if (this.CreateLicenseOperationCompleted == null)
            {
                this.CreateLicenseOperationCompleted = new SendOrPostCallback(this.OnCreateLicenseOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("CreateLicense", objArray, this.CreateLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/CreateTrialLicense",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LicenseInfo CreateTrialLicense(CreateTrialLicenseRequest info)
        {
            object[] objArray = new object[] { info };
            return (LicenseInfo)base.Invoke("CreateTrialLicense", objArray)[0];
        }

        public void CreateTrialLicenseAsync(CreateTrialLicenseRequest info)
        {
            this.CreateTrialLicenseAsync(info, null);
        }

        public void CreateTrialLicenseAsync(CreateTrialLicenseRequest info, object userState)
        {
            if (this.CreateTrialLicenseOperationCompleted == null)
            {
                this.CreateTrialLicenseOperationCompleted =
                    new SendOrPostCallback(this.OnCreateTrialLicenseOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("CreateTrialLicense", objArray, this.CreateTrialLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/Echo", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string Echo()
        {
            object[] objArray = base.Invoke("Echo", new object[0]);
            return (string)objArray[0];
        }

        public void EchoAsync()
        {
            this.EchoAsync(null);
        }

        public void EchoAsync(object userState)
        {
            if (this.EchoOperationCompleted == null)
            {
                this.EchoOperationCompleted = new SendOrPostCallback(this.OnEchoOperationCompleted);
            }

            base.InvokeAsync("Echo", new object[0], this.EchoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/GetAdvancedOptionForLicense",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public AdvancedOptionsResponse GetAdvancedOptionForLicense(string licenseKey)
        {
            object[] objArray = new object[] { licenseKey };
            return (AdvancedOptionsResponse)base.Invoke("GetAdvancedOptionForLicense", objArray)[0];
        }

        public void GetAdvancedOptionForLicenseAsync(string licenseKey)
        {
            this.GetAdvancedOptionForLicenseAsync(licenseKey, null);
        }

        public void GetAdvancedOptionForLicenseAsync(string licenseKey, object userState)
        {
            if (this.GetAdvancedOptionForLicenseOperationCompleted == null)
            {
                this.GetAdvancedOptionForLicenseOperationCompleted =
                    new SendOrPostCallback(this.OnGetAdvancedOptionForLicenseOperationCompleted);
            }

            object[] objArray = new object[] { licenseKey };
            base.InvokeAsync("GetAdvancedOptionForLicense", objArray,
                this.GetAdvancedOptionForLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/GetCustomers", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public Customer[] GetCustomers(GetCustomersRequest info)
        {
            object[] objArray = new object[] { info };
            return (Customer[])base.Invoke("GetCustomers", objArray)[0];
        }

        public void GetCustomersAsync(GetCustomersRequest info)
        {
            this.GetCustomersAsync(info, null);
        }

        public void GetCustomersAsync(GetCustomersRequest info, object userState)
        {
            if (this.GetCustomersOperationCompleted == null)
            {
                this.GetCustomersOperationCompleted = new SendOrPostCallback(this.OnGetCustomersOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("GetCustomers", objArray, this.GetCustomersOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/GetLatestProductRelease",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LatestProductReleaseInfo GetLatestProductRelease(GetProductReleaseRequest request)
        {
            object[] objArray = new object[] { request };
            return (LatestProductReleaseInfo)base.Invoke("GetLatestProductRelease", objArray)[0];
        }

        public void GetLatestProductReleaseAsync(GetProductReleaseRequest request)
        {
            this.GetLatestProductReleaseAsync(request, null);
        }

        public void GetLatestProductReleaseAsync(GetProductReleaseRequest request, object userState)
        {
            if (this.GetLatestProductReleaseOperationCompleted == null)
            {
                this.GetLatestProductReleaseOperationCompleted =
                    new SendOrPostCallback(this.OnGetLatestProductReleaseOperationCompleted);
            }

            object[] objArray = new object[] { request };
            base.InvokeAsync("GetLatestProductRelease", objArray, this.GetLatestProductReleaseOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/GetLicenseData", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LicenseDataResponse GetLicenseData(string licenseKey, LicenseDataDetails details)
        {
            object[] objArray = new object[] { licenseKey, details };
            return (LicenseDataResponse)base.Invoke("GetLicenseData", objArray)[0];
        }

        public void GetLicenseDataAsync(string licenseKey, LicenseDataDetails details)
        {
            this.GetLicenseDataAsync(licenseKey, details, null);
        }

        public void GetLicenseDataAsync(string licenseKey, LicenseDataDetails details, object userState)
        {
            if (this.GetLicenseDataOperationCompleted == null)
            {
                this.GetLicenseDataOperationCompleted = new SendOrPostCallback(this.OnGetLicenseDataOperationCompleted);
            }

            object[] objArray = new object[] { licenseKey, details };
            base.InvokeAsync("GetLicenseData", objArray, this.GetLicenseDataOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/GetLicenses", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LicenseInfo[] GetLicenses(GetLicensesRequest info)
        {
            object[] objArray = new object[] { info };
            return (LicenseInfo[])base.Invoke("GetLicenses", objArray)[0];
        }

        public void GetLicensesAsync(GetLicensesRequest info)
        {
            this.GetLicensesAsync(info, null);
        }

        public void GetLicensesAsync(GetLicensesRequest info, object userState)
        {
            if (this.GetLicensesOperationCompleted == null)
            {
                this.GetLicensesOperationCompleted = new SendOrPostCallback(this.OnGetLicensesOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("GetLicenses", objArray, this.GetLicensesOperationCompleted, userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
            {
                return false;
            }

            System.Uri uri = new System.Uri(url);
            if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        [SoapDocumentMethod("http://www.metalogix.com/Login", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public string Login(string userName, string password)
        {
            object[] objArray = new object[] { userName, password };
            return (string)base.Invoke("Login", objArray)[0];
        }

        public void LoginAsync(string userName, string password)
        {
            this.LoginAsync(userName, password, null);
        }

        public void LoginAsync(string userName, string password, object userState)
        {
            if (this.LoginOperationCompleted == null)
            {
                this.LoginOperationCompleted = new SendOrPostCallback(this.OnLoginOperationCompleted);
            }

            object[] objArray = new object[] { userName, password };
            base.InvokeAsync("Login", objArray, this.LoginOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/ModifyLicense", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LicenseInfo ModifyLicense(UpdateLicenseRequest info)
        {
            object[] objArray = new object[] { info };
            return (LicenseInfo)base.Invoke("ModifyLicense", objArray)[0];
        }

        public void ModifyLicenseAsync(UpdateLicenseRequest info)
        {
            this.ModifyLicenseAsync(info, null);
        }

        public void ModifyLicenseAsync(UpdateLicenseRequest info, object userState)
        {
            if (this.ModifyLicenseOperationCompleted == null)
            {
                this.ModifyLicenseOperationCompleted = new SendOrPostCallback(this.OnModifyLicenseOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("ModifyLicense", objArray, this.ModifyLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/ModifyProductRelease",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public void ModifyProductRelease(ModifyProductReleaseRequest request)
        {
            object[] objArray = new object[] { request };
            base.Invoke("ModifyProductRelease", objArray);
        }

        public void ModifyProductReleaseAsync(ModifyProductReleaseRequest request)
        {
            this.ModifyProductReleaseAsync(request, null);
        }

        public void ModifyProductReleaseAsync(ModifyProductReleaseRequest request, object userState)
        {
            if (this.ModifyProductReleaseOperationCompleted == null)
            {
                this.ModifyProductReleaseOperationCompleted =
                    new SendOrPostCallback(this.OnModifyProductReleaseOperationCompleted);
            }

            object[] objArray = new object[] { request };
            base.InvokeAsync("ModifyProductRelease", objArray, this.ModifyProductReleaseOperationCompleted, userState);
        }

        private void OnAddAdvancedOptionForLicenseOperationCompleted(object arg)
        {
            if (this.AddAdvancedOptionForLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddAdvancedOptionForLicenseCompleted(this,
                    new AddAdvancedOptionForLicenseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddProductReleaseOperationCompleted(object arg)
        {
            if (this.AddProductReleaseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddProductReleaseCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnConvertOldKeyOperationCompleted(object arg)
        {
            if (this.ConvertOldKeyCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ConvertOldKeyCompleted(this,
                    new ConvertOldKeyCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateLicenseOperationCompleted(object arg)
        {
            if (this.CreateLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateLicenseCompleted(this,
                    new CreateLicenseCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateTrialLicenseOperationCompleted(object arg)
        {
            if (this.CreateTrialLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateTrialLicenseCompleted(this,
                    new CreateTrialLicenseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnEchoOperationCompleted(object arg)
        {
            if (this.EchoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.EchoCompleted(this,
                    new EchoCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAdvancedOptionForLicenseOperationCompleted(object arg)
        {
            if (this.GetAdvancedOptionForLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAdvancedOptionForLicenseCompleted(this,
                    new GetAdvancedOptionForLicenseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCustomersOperationCompleted(object arg)
        {
            if (this.GetCustomersCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCustomersCompleted(this,
                    new GetCustomersCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetLatestProductReleaseOperationCompleted(object arg)
        {
            if (this.GetLatestProductReleaseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetLatestProductReleaseCompleted(this,
                    new GetLatestProductReleaseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetLicenseDataOperationCompleted(object arg)
        {
            if (this.GetLicenseDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetLicenseDataCompleted(this,
                    new GetLicenseDataCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetLicensesOperationCompleted(object arg)
        {
            if (this.GetLicensesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetLicensesCompleted(this,
                    new GetLicensesCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnLoginOperationCompleted(object arg)
        {
            if (this.LoginCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.LoginCompleted(this,
                    new LoginCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnModifyLicenseOperationCompleted(object arg)
        {
            if (this.ModifyLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ModifyLicenseCompleted(this,
                    new ModifyLicenseCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnModifyProductReleaseOperationCompleted(object arg)
        {
            if (this.ModifyProductReleaseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ModifyProductReleaseCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateAdvancedOptionForLicenseOperationCompleted(object arg)
        {
            if (this.UpdateAdvancedOptionForLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateAdvancedOptionForLicenseCompleted(this,
                    new UpdateAdvancedOptionForLicenseCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateLicenseOperationCompleted(object arg)
        {
            if (this.UpdateLicenseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateLicenseCompleted(this,
                    new UpdateLicenseCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateServerProductSystemInfoOperationCompleted(object arg)
        {
            if (this.UpdateServerProductSystemInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateServerProductSystemInfoCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateServerSystemInfoOperationCompleted(object arg)
        {
            if (this.UpdateServerSystemInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateServerSystemInfoCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnValidateInstallVersionOperationCompleted(object arg)
        {
            if (this.ValidateInstallVersionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ValidateInstallVersionCompleted(this,
                    new ValidateInstallVersionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://www.metalogix.com/UpdateAdvancedOptionForLicense",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public int UpdateAdvancedOptionForLicense(AdvancedOptionRequest advancedOption)
        {
            object[] objArray = new object[] { advancedOption };
            return (int)base.Invoke("UpdateAdvancedOptionForLicense", objArray)[0];
        }

        public void UpdateAdvancedOptionForLicenseAsync(AdvancedOptionRequest advancedOption)
        {
            this.UpdateAdvancedOptionForLicenseAsync(advancedOption, null);
        }

        public void UpdateAdvancedOptionForLicenseAsync(AdvancedOptionRequest advancedOption, object userState)
        {
            if (this.UpdateAdvancedOptionForLicenseOperationCompleted == null)
            {
                this.UpdateAdvancedOptionForLicenseOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateAdvancedOptionForLicenseOperationCompleted);
            }

            object[] objArray = new object[] { advancedOption };
            base.InvokeAsync("UpdateAdvancedOptionForLicense", objArray,
                this.UpdateAdvancedOptionForLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/UpdateLicense", RequestNamespace = "http://www.metalogix.com/",
            ResponseNamespace = "http://www.metalogix.com/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("ClientVersionHeaderValue")]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public LicenseInfoResponse UpdateLicense(LicenseInfoRequest info)
        {
            object[] objArray = new object[] { info };
            return (LicenseInfoResponse)base.Invoke("UpdateLicense", objArray)[0];
        }

        public void UpdateLicenseAsync(LicenseInfoRequest info)
        {
            this.UpdateLicenseAsync(info, null);
        }

        public void UpdateLicenseAsync(LicenseInfoRequest info, object userState)
        {
            if (this.UpdateLicenseOperationCompleted == null)
            {
                this.UpdateLicenseOperationCompleted = new SendOrPostCallback(this.OnUpdateLicenseOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("UpdateLicense", objArray, this.UpdateLicenseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/UpdateServerProductSystemInfo",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public void UpdateServerProductSystemInfo(ServerProductSystemInfo info)
        {
            object[] objArray = new object[] { info };
            base.Invoke("UpdateServerProductSystemInfo", objArray);
        }

        public void UpdateServerProductSystemInfoAsync(ServerProductSystemInfo info)
        {
            this.UpdateServerProductSystemInfoAsync(info, null);
        }

        public void UpdateServerProductSystemInfoAsync(ServerProductSystemInfo info, object userState)
        {
            if (this.UpdateServerProductSystemInfoOperationCompleted == null)
            {
                this.UpdateServerProductSystemInfoOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateServerProductSystemInfoOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("UpdateServerProductSystemInfo", objArray,
                this.UpdateServerProductSystemInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/UpdateServerSystemInfo",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public void UpdateServerSystemInfo(ServerSystemInfo info)
        {
            object[] objArray = new object[] { info };
            base.Invoke("UpdateServerSystemInfo", objArray);
        }

        public void UpdateServerSystemInfoAsync(ServerSystemInfo info)
        {
            this.UpdateServerSystemInfoAsync(info, null);
        }

        public void UpdateServerSystemInfoAsync(ServerSystemInfo info, object userState)
        {
            if (this.UpdateServerSystemInfoOperationCompleted == null)
            {
                this.UpdateServerSystemInfoOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateServerSystemInfoOperationCompleted);
            }

            object[] objArray = new object[] { info };
            base.InvokeAsync("UpdateServerSystemInfo", objArray, this.UpdateServerSystemInfoOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://www.metalogix.com/ValidateInstallVersion",
            RequestNamespace = "http://www.metalogix.com/", ResponseNamespace = "http://www.metalogix.com/",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        [SoapHeader("TokenAuthenticationHeaderValue")]
        public bool ValidateInstallVersion(string licenseKey,
            Metalogix.Licensing.LicenseServer.Service.Version productVersion)
        {
            object[] objArray = new object[] { licenseKey, productVersion };
            return (bool)base.Invoke("ValidateInstallVersion", objArray)[0];
        }

        public void ValidateInstallVersionAsync(string licenseKey,
            Metalogix.Licensing.LicenseServer.Service.Version productVersion)
        {
            this.ValidateInstallVersionAsync(licenseKey, productVersion, null);
        }

        public void ValidateInstallVersionAsync(string licenseKey,
            Metalogix.Licensing.LicenseServer.Service.Version productVersion, object userState)
        {
            if (this.ValidateInstallVersionOperationCompleted == null)
            {
                this.ValidateInstallVersionOperationCompleted =
                    new SendOrPostCallback(this.OnValidateInstallVersionOperationCompleted);
            }

            object[] objArray = new object[] { licenseKey, productVersion };
            base.InvokeAsync("ValidateInstallVersion", objArray, this.ValidateInstallVersionOperationCompleted,
                userState);
        }

        public event AddAdvancedOptionForLicenseCompletedEventHandler AddAdvancedOptionForLicenseCompleted;

        public event AddProductReleaseCompletedEventHandler AddProductReleaseCompleted;

        public event ConvertOldKeyCompletedEventHandler ConvertOldKeyCompleted;

        public event CreateLicenseCompletedEventHandler CreateLicenseCompleted;

        public event CreateTrialLicenseCompletedEventHandler CreateTrialLicenseCompleted;

        public event EchoCompletedEventHandler EchoCompleted;

        public event GetAdvancedOptionForLicenseCompletedEventHandler GetAdvancedOptionForLicenseCompleted;

        public event GetCustomersCompletedEventHandler GetCustomersCompleted;

        public event GetLatestProductReleaseCompletedEventHandler GetLatestProductReleaseCompleted;

        public event GetLicenseDataCompletedEventHandler GetLicenseDataCompleted;

        public event GetLicensesCompletedEventHandler GetLicensesCompleted;

        public event LoginCompletedEventHandler LoginCompleted;

        public event ModifyLicenseCompletedEventHandler ModifyLicenseCompleted;

        public event ModifyProductReleaseCompletedEventHandler ModifyProductReleaseCompleted;

        public event UpdateAdvancedOptionForLicenseCompletedEventHandler UpdateAdvancedOptionForLicenseCompleted;

        public event UpdateLicenseCompletedEventHandler UpdateLicenseCompleted;

        public event UpdateServerProductSystemInfoCompletedEventHandler UpdateServerProductSystemInfoCompleted;

        public event UpdateServerSystemInfoCompletedEventHandler UpdateServerSystemInfoCompleted;

        public event ValidateInstallVersionCompletedEventHandler ValidateInstallVersionCompleted;
    }
}