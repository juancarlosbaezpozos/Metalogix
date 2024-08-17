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

namespace Metalogix.SharePoint.Adapters.NWS.Permissions
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "PermissionsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/directory/")]
    public class Permissions : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetPermissionCollectionOperationCompleted;

        private SendOrPostCallback AddPermissionOperationCompleted;

        private SendOrPostCallback AddPermissionCollectionOperationCompleted;

        private SendOrPostCallback UpdatePermissionOperationCompleted;

        private SendOrPostCallback RemovePermissionOperationCompleted;

        private SendOrPostCallback RemovePermissionCollectionOperationCompleted;

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

        public Permissions()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Permissions_Permissions;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddPermission",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddPermission(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask)
        {
            object[] objArray = new object[]
                { objectName, objectType, permissionIdentifier, permissionType, permissionMask };
            base.Invoke("AddPermission", objArray);
        }

        public void AddPermissionAsync(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask)
        {
            this.AddPermissionAsync(objectName, objectType, permissionIdentifier, permissionType, permissionMask, null);
        }

        public void AddPermissionAsync(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask, object userState)
        {
            if (this.AddPermissionOperationCompleted == null)
            {
                this.AddPermissionOperationCompleted = new SendOrPostCallback(this.OnAddPermissionOperationCompleted);
            }

            object[] objArray = new object[]
                { objectName, objectType, permissionIdentifier, permissionType, permissionMask };
            base.InvokeAsync("AddPermission", objArray, this.AddPermissionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddPermissionCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddPermissionCollection(string objectName, string objectType, XmlNode permissionsInfoXml)
        {
            object[] objArray = new object[] { objectName, objectType, permissionsInfoXml };
            base.Invoke("AddPermissionCollection", objArray);
        }

        public void AddPermissionCollectionAsync(string objectName, string objectType, XmlNode permissionsInfoXml)
        {
            this.AddPermissionCollectionAsync(objectName, objectType, permissionsInfoXml, null);
        }

        public void AddPermissionCollectionAsync(string objectName, string objectType, XmlNode permissionsInfoXml,
            object userState)
        {
            if (this.AddPermissionCollectionOperationCompleted == null)
            {
                this.AddPermissionCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnAddPermissionCollectionOperationCompleted);
            }

            object[] objArray = new object[] { objectName, objectType, permissionsInfoXml };
            base.InvokeAsync("AddPermissionCollection", objArray, this.AddPermissionCollectionOperationCompleted,
                userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetPermissionCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetPermissionCollection(string objectName, string objectType)
        {
            object[] objArray = new object[] { objectName, objectType };
            return (XmlNode)base.Invoke("GetPermissionCollection", objArray)[0];
        }

        public void GetPermissionCollectionAsync(string objectName, string objectType)
        {
            this.GetPermissionCollectionAsync(objectName, objectType, null);
        }

        public void GetPermissionCollectionAsync(string objectName, string objectType, object userState)
        {
            if (this.GetPermissionCollectionOperationCompleted == null)
            {
                this.GetPermissionCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetPermissionCollectionOperationCompleted);
            }

            object[] objArray = new object[] { objectName, objectType };
            base.InvokeAsync("GetPermissionCollection", objArray, this.GetPermissionCollectionOperationCompleted,
                userState);
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

        private void OnAddPermissionCollectionOperationCompleted(object arg)
        {
            if (this.AddPermissionCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddPermissionCollectionCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddPermissionOperationCompleted(object arg)
        {
            if (this.AddPermissionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddPermissionCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetPermissionCollectionOperationCompleted(object arg)
        {
            if (this.GetPermissionCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetPermissionCollectionCompleted(this,
                    new GetPermissionCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemovePermissionCollectionOperationCompleted(object arg)
        {
            if (this.RemovePermissionCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemovePermissionCollectionCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemovePermissionOperationCompleted(object arg)
        {
            if (this.RemovePermissionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemovePermissionCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdatePermissionOperationCompleted(object arg)
        {
            if (this.UpdatePermissionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdatePermissionCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemovePermission",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemovePermission(string objectName, string objectType, string permissionIdentifier,
            string permissionType)
        {
            object[] objArray = new object[] { objectName, objectType, permissionIdentifier, permissionType };
            base.Invoke("RemovePermission", objArray);
        }

        public void RemovePermissionAsync(string objectName, string objectType, string permissionIdentifier,
            string permissionType)
        {
            this.RemovePermissionAsync(objectName, objectType, permissionIdentifier, permissionType, null);
        }

        public void RemovePermissionAsync(string objectName, string objectType, string permissionIdentifier,
            string permissionType, object userState)
        {
            if (this.RemovePermissionOperationCompleted == null)
            {
                this.RemovePermissionOperationCompleted =
                    new SendOrPostCallback(this.OnRemovePermissionOperationCompleted);
            }

            object[] objArray = new object[] { objectName, objectType, permissionIdentifier, permissionType };
            base.InvokeAsync("RemovePermission", objArray, this.RemovePermissionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemovePermissionCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemovePermissionCollection(string objectName, string objectType, XmlNode memberIdsXml)
        {
            object[] objArray = new object[] { objectName, objectType, memberIdsXml };
            base.Invoke("RemovePermissionCollection", objArray);
        }

        public void RemovePermissionCollectionAsync(string objectName, string objectType, XmlNode memberIdsXml)
        {
            this.RemovePermissionCollectionAsync(objectName, objectType, memberIdsXml, null);
        }

        public void RemovePermissionCollectionAsync(string objectName, string objectType, XmlNode memberIdsXml,
            object userState)
        {
            if (this.RemovePermissionCollectionOperationCompleted == null)
            {
                this.RemovePermissionCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnRemovePermissionCollectionOperationCompleted);
            }

            object[] objArray = new object[] { objectName, objectType, memberIdsXml };
            base.InvokeAsync("RemovePermissionCollection", objArray, this.RemovePermissionCollectionOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/UpdatePermission",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdatePermission(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask)
        {
            object[] objArray = new object[]
                { objectName, objectType, permissionIdentifier, permissionType, permissionMask };
            base.Invoke("UpdatePermission", objArray);
        }

        public void UpdatePermissionAsync(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask)
        {
            this.UpdatePermissionAsync(objectName, objectType, permissionIdentifier, permissionType, permissionMask,
                null);
        }

        public void UpdatePermissionAsync(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask, object userState)
        {
            if (this.UpdatePermissionOperationCompleted == null)
            {
                this.UpdatePermissionOperationCompleted =
                    new SendOrPostCallback(this.OnUpdatePermissionOperationCompleted);
            }

            object[] objArray = new object[]
                { objectName, objectType, permissionIdentifier, permissionType, permissionMask };
            base.InvokeAsync("UpdatePermission", objArray, this.UpdatePermissionOperationCompleted, userState);
        }

        public event AddPermissionCollectionCompletedEventHandler AddPermissionCollectionCompleted;

        public event AddPermissionCompletedEventHandler AddPermissionCompleted;

        public event GetPermissionCollectionCompletedEventHandler GetPermissionCollectionCompleted;

        public event RemovePermissionCollectionCompletedEventHandler RemovePermissionCollectionCompleted;

        public event RemovePermissionCompletedEventHandler RemovePermissionCompleted;

        public event UpdatePermissionCompletedEventHandler UpdatePermissionCompleted;
    }
}