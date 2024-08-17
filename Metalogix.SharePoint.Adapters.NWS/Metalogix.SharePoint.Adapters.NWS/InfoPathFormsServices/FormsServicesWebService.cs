using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "FormsServicesWebServiceSoap",
        Namespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices")]
    public class FormsServicesWebService : SoapHttpClientProtocol
    {
        private SendOrPostCallback BrowserEnableUserFormTemplateOperationCompleted;

        private SendOrPostCallback DesignCheckFormTemplateOperationCompleted;

        private SendOrPostCallback SetFormsForListItemOperationCompleted;

        private SendOrPostCallback GetListFormLocationOperationCompleted;

        private SendOrPostCallback SetSchemaChangesForListOperationCompleted;

        private SendOrPostCallback GetUserCodeDeploymentDependenciesOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

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

        public FormsServicesWebService()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_FormsServices_FormsServicesWebService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod(
            "http://schemas.microsoft.com/office/infopath/2007/formsServices/BrowserEnableUserFormTemplate",
            RequestNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            ResponseNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public MessagesResponse BrowserEnableUserFormTemplate(string formTemplateLocation)
        {
            object[] objArray = new object[] { formTemplateLocation };
            return (MessagesResponse)base.Invoke("BrowserEnableUserFormTemplate", objArray)[0];
        }

        public void BrowserEnableUserFormTemplateAsync(string formTemplateLocation)
        {
            this.BrowserEnableUserFormTemplateAsync(formTemplateLocation, null);
        }

        public void BrowserEnableUserFormTemplateAsync(string formTemplateLocation, object userState)
        {
            if (this.BrowserEnableUserFormTemplateOperationCompleted == null)
            {
                this.BrowserEnableUserFormTemplateOperationCompleted =
                    new SendOrPostCallback(this.OnBrowserEnableUserFormTemplateOperationCompleted);
            }

            object[] objArray = new object[] { formTemplateLocation };
            base.InvokeAsync("BrowserEnableUserFormTemplate", objArray,
                this.BrowserEnableUserFormTemplateOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/office/infopath/2007/formsServices/DesignCheckFormTemplate",
            RequestNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            ResponseNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public DesignCheckerInformation DesignCheckFormTemplate(int lcid, string base64FormTemplate,
            string applicationId)
        {
            object[] objArray = new object[] { lcid, base64FormTemplate, applicationId };
            return (DesignCheckerInformation)base.Invoke("DesignCheckFormTemplate", objArray)[0];
        }

        public void DesignCheckFormTemplateAsync(int lcid, string base64FormTemplate, string applicationId)
        {
            this.DesignCheckFormTemplateAsync(lcid, base64FormTemplate, applicationId, null);
        }

        public void DesignCheckFormTemplateAsync(int lcid, string base64FormTemplate, string applicationId,
            object userState)
        {
            if (this.DesignCheckFormTemplateOperationCompleted == null)
            {
                this.DesignCheckFormTemplateOperationCompleted =
                    new SendOrPostCallback(this.OnDesignCheckFormTemplateOperationCompleted);
            }

            object[] objArray = new object[] { lcid, base64FormTemplate, applicationId };
            base.InvokeAsync("DesignCheckFormTemplate", objArray, this.DesignCheckFormTemplateOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/office/infopath/2007/formsServices/GetListFormLocation",
            RequestNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            ResponseNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetListFormLocation(int lcid, string listGuid, string contentTypeId, bool checkDesignPermissions,
            bool checkCustomFormEnabled)
        {
            object[] objArray = new object[]
                { lcid, listGuid, contentTypeId, checkDesignPermissions, checkCustomFormEnabled };
            return (string)base.Invoke("GetListFormLocation", objArray)[0];
        }

        public void GetListFormLocationAsync(int lcid, string listGuid, string contentTypeId,
            bool checkDesignPermissions, bool checkCustomFormEnabled)
        {
            this.GetListFormLocationAsync(lcid, listGuid, contentTypeId, checkDesignPermissions, checkCustomFormEnabled,
                null);
        }

        public void GetListFormLocationAsync(int lcid, string listGuid, string contentTypeId,
            bool checkDesignPermissions, bool checkCustomFormEnabled, object userState)
        {
            if (this.GetListFormLocationOperationCompleted == null)
            {
                this.GetListFormLocationOperationCompleted =
                    new SendOrPostCallback(this.OnGetListFormLocationOperationCompleted);
            }

            object[] objArray = new object[]
                { lcid, listGuid, contentTypeId, checkDesignPermissions, checkCustomFormEnabled };
            base.InvokeAsync("GetListFormLocation", objArray, this.GetListFormLocationOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://schemas.microsoft.com/office/infopath/2007/formsServices/GetUserCodeDeploymentDependencies",
            RequestNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            ResponseNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public UserSolutionActivationStatus GetUserCodeDeploymentDependencies(string siteCollectionLocation)
        {
            object[] objArray = new object[] { siteCollectionLocation };
            return (UserSolutionActivationStatus)base.Invoke("GetUserCodeDeploymentDependencies", objArray)[0];
        }

        public void GetUserCodeDeploymentDependenciesAsync(string siteCollectionLocation)
        {
            this.GetUserCodeDeploymentDependenciesAsync(siteCollectionLocation, null);
        }

        public void GetUserCodeDeploymentDependenciesAsync(string siteCollectionLocation, object userState)
        {
            if (this.GetUserCodeDeploymentDependenciesOperationCompleted == null)
            {
                this.GetUserCodeDeploymentDependenciesOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserCodeDeploymentDependenciesOperationCompleted);
            }

            object[] objArray = new object[] { siteCollectionLocation };
            base.InvokeAsync("GetUserCodeDeploymentDependencies", objArray,
                this.GetUserCodeDeploymentDependenciesOperationCompleted, userState);
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

        private void OnBrowserEnableUserFormTemplateOperationCompleted(object arg)
        {
            if (this.BrowserEnableUserFormTemplateCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.BrowserEnableUserFormTemplateCompleted(this,
                    new BrowserEnableUserFormTemplateCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDesignCheckFormTemplateOperationCompleted(object arg)
        {
            if (this.DesignCheckFormTemplateCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DesignCheckFormTemplateCompleted(this,
                    new DesignCheckFormTemplateCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetListFormLocationOperationCompleted(object arg)
        {
            if (this.GetListFormLocationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetListFormLocationCompleted(this,
                    new GetListFormLocationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserCodeDeploymentDependenciesOperationCompleted(object arg)
        {
            if (this.GetUserCodeDeploymentDependenciesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserCodeDeploymentDependenciesCompleted(this,
                    new GetUserCodeDeploymentDependenciesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetFormsForListItemOperationCompleted(object arg)
        {
            if (this.SetFormsForListItemCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetFormsForListItemCompleted(this,
                    new SetFormsForListItemCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetSchemaChangesForListOperationCompleted(object arg)
        {
            if (this.SetSchemaChangesForListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetSchemaChangesForListCompleted(this,
                    new SetSchemaChangesForListCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/office/infopath/2007/formsServices/SetFormsForListItem",
            RequestNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            ResponseNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public DesignCheckerInformation SetFormsForListItem(int lcid, string base64FormTemplate, string applicationId,
            string listGuid, string contentTypeId)
        {
            object[] objArray = new object[] { lcid, base64FormTemplate, applicationId, listGuid, contentTypeId };
            return (DesignCheckerInformation)base.Invoke("SetFormsForListItem", objArray)[0];
        }

        public void SetFormsForListItemAsync(int lcid, string base64FormTemplate, string applicationId, string listGuid,
            string contentTypeId)
        {
            this.SetFormsForListItemAsync(lcid, base64FormTemplate, applicationId, listGuid, contentTypeId, null);
        }

        public void SetFormsForListItemAsync(int lcid, string base64FormTemplate, string applicationId, string listGuid,
            string contentTypeId, object userState)
        {
            if (this.SetFormsForListItemOperationCompleted == null)
            {
                this.SetFormsForListItemOperationCompleted =
                    new SendOrPostCallback(this.OnSetFormsForListItemOperationCompleted);
            }

            object[] objArray = new object[] { lcid, base64FormTemplate, applicationId, listGuid, contentTypeId };
            base.InvokeAsync("SetFormsForListItem", objArray, this.SetFormsForListItemOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/office/infopath/2007/formsServices/SetSchemaChangesForList",
            RequestNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            ResponseNamespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode SetSchemaChangesForList(int lcid, string listGuid, string contentTypeId, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields)
        {
            object[] objArray = new object[] { lcid, listGuid, contentTypeId, newFields, updateFields, deleteFields };
            return (XmlNode)base.Invoke("SetSchemaChangesForList", objArray)[0];
        }

        public void SetSchemaChangesForListAsync(int lcid, string listGuid, string contentTypeId, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields)
        {
            this.SetSchemaChangesForListAsync(lcid, listGuid, contentTypeId, newFields, updateFields, deleteFields,
                null);
        }

        public void SetSchemaChangesForListAsync(int lcid, string listGuid, string contentTypeId, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields, object userState)
        {
            if (this.SetSchemaChangesForListOperationCompleted == null)
            {
                this.SetSchemaChangesForListOperationCompleted =
                    new SendOrPostCallback(this.OnSetSchemaChangesForListOperationCompleted);
            }

            object[] objArray = new object[] { lcid, listGuid, contentTypeId, newFields, updateFields, deleteFields };
            base.InvokeAsync("SetSchemaChangesForList", objArray, this.SetSchemaChangesForListOperationCompleted,
                userState);
        }

        public event BrowserEnableUserFormTemplateCompletedEventHandler BrowserEnableUserFormTemplateCompleted;

        public event DesignCheckFormTemplateCompletedEventHandler DesignCheckFormTemplateCompleted;

        public event GetListFormLocationCompletedEventHandler GetListFormLocationCompleted;

        public event GetUserCodeDeploymentDependenciesCompletedEventHandler GetUserCodeDeploymentDependenciesCompleted;

        public event SetFormsForListItemCompletedEventHandler SetFormsForListItemCompleted;

        public event SetSchemaChangesForListCompletedEventHandler SetSchemaChangesForListCompleted;
    }
}