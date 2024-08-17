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

namespace Metalogix.SharePoint.Adapters.NWS.UserGroup
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "UserGroupSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/directory/")]
    public class UserGroup : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetUserCollectionFromSiteOperationCompleted;

        private SendOrPostCallback GetUserCollectionFromWebOperationCompleted;

        private SendOrPostCallback GetAllUserCollectionFromWebOperationCompleted;

        private SendOrPostCallback GetUserCollectionFromGroupOperationCompleted;

        private SendOrPostCallback GetUserCollectionFromRoleOperationCompleted;

        private SendOrPostCallback GetUserCollectionOperationCompleted;

        private SendOrPostCallback GetUserInfoOperationCompleted;

        private SendOrPostCallback AddUserToGroupOperationCompleted;

        private SendOrPostCallback AddUserCollectionToGroupOperationCompleted;

        private SendOrPostCallback AddUserToRoleOperationCompleted;

        private SendOrPostCallback AddUserCollectionToRoleOperationCompleted;

        private SendOrPostCallback UpdateUserInfoOperationCompleted;

        private SendOrPostCallback RemoveUserFromSiteOperationCompleted;

        private SendOrPostCallback RemoveUserCollectionFromSiteOperationCompleted;

        private SendOrPostCallback RemoveUserFromWebOperationCompleted;

        private SendOrPostCallback RemoveUserFromGroupOperationCompleted;

        private SendOrPostCallback RemoveUserCollectionFromGroupOperationCompleted;

        private SendOrPostCallback RemoveUserFromRoleOperationCompleted;

        private SendOrPostCallback RemoveUserCollectionFromRoleOperationCompleted;

        private SendOrPostCallback GetGroupCollectionFromSiteOperationCompleted;

        private SendOrPostCallback GetGroupCollectionFromWebOperationCompleted;

        private SendOrPostCallback GetGroupCollectionFromRoleOperationCompleted;

        private SendOrPostCallback GetGroupCollectionFromUserOperationCompleted;

        private SendOrPostCallback GetGroupCollectionOperationCompleted;

        private SendOrPostCallback GetGroupInfoOperationCompleted;

        private SendOrPostCallback AddGroupOperationCompleted;

        private SendOrPostCallback AddGroupToRoleOperationCompleted;

        private SendOrPostCallback UpdateGroupInfoOperationCompleted;

        private SendOrPostCallback RemoveGroupOperationCompleted;

        private SendOrPostCallback RemoveGroupFromRoleOperationCompleted;

        private SendOrPostCallback GetRoleCollectionFromWebOperationCompleted;

        private SendOrPostCallback GetRoleCollectionFromGroupOperationCompleted;

        private SendOrPostCallback GetRoleCollectionFromUserOperationCompleted;

        private SendOrPostCallback GetRoleCollectionOperationCompleted;

        private SendOrPostCallback GetRoleInfoOperationCompleted;

        private SendOrPostCallback AddRoleOperationCompleted;

        private SendOrPostCallback AddRoleDefOperationCompleted;

        private SendOrPostCallback UpdateRoleInfoOperationCompleted;

        private SendOrPostCallback UpdateRoleDefInfoOperationCompleted;

        private SendOrPostCallback RemoveRoleOperationCompleted;

        private SendOrPostCallback GetUserLoginFromEmailOperationCompleted;

        private SendOrPostCallback GetRolesAndPermissionsForCurrentUserOperationCompleted;

        private SendOrPostCallback GetRolesAndPermissionsForSiteOperationCompleted;

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

        public UserGroup()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_UserGroup_UserGroup;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddGroup(string groupName, string ownerIdentifier, string ownerType, string defaultUserLoginName,
            string description)
        {
            object[] objArray = new object[]
                { groupName, ownerIdentifier, ownerType, defaultUserLoginName, description };
            base.Invoke("AddGroup", objArray);
        }

        public void AddGroupAsync(string groupName, string ownerIdentifier, string ownerType,
            string defaultUserLoginName, string description)
        {
            this.AddGroupAsync(groupName, ownerIdentifier, ownerType, defaultUserLoginName, description, null);
        }

        public void AddGroupAsync(string groupName, string ownerIdentifier, string ownerType,
            string defaultUserLoginName, string description, object userState)
        {
            if (this.AddGroupOperationCompleted == null)
            {
                this.AddGroupOperationCompleted = new SendOrPostCallback(this.OnAddGroupOperationCompleted);
            }

            object[] objArray = new object[]
                { groupName, ownerIdentifier, ownerType, defaultUserLoginName, description };
            base.InvokeAsync("AddGroup", objArray, this.AddGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddGroupToRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddGroupToRole(string roleName, string groupName)
        {
            object[] objArray = new object[] { roleName, groupName };
            base.Invoke("AddGroupToRole", objArray);
        }

        public void AddGroupToRoleAsync(string roleName, string groupName)
        {
            this.AddGroupToRoleAsync(roleName, groupName, null);
        }

        public void AddGroupToRoleAsync(string roleName, string groupName, object userState)
        {
            if (this.AddGroupToRoleOperationCompleted == null)
            {
                this.AddGroupToRoleOperationCompleted = new SendOrPostCallback(this.OnAddGroupToRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, groupName };
            base.InvokeAsync("AddGroupToRole", objArray, this.AddGroupToRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddRole(string roleName, string description, int permissionMask)
        {
            object[] objArray = new object[] { roleName, description, permissionMask };
            base.Invoke("AddRole", objArray);
        }

        public void AddRoleAsync(string roleName, string description, int permissionMask)
        {
            this.AddRoleAsync(roleName, description, permissionMask, null);
        }

        public void AddRoleAsync(string roleName, string description, int permissionMask, object userState)
        {
            if (this.AddRoleOperationCompleted == null)
            {
                this.AddRoleOperationCompleted = new SendOrPostCallback(this.OnAddRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, description, permissionMask };
            base.InvokeAsync("AddRole", objArray, this.AddRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddRoleDef",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddRoleDef(string roleName, string description, ulong permissionMask)
        {
            object[] objArray = new object[] { roleName, description, permissionMask };
            base.Invoke("AddRoleDef", objArray);
        }

        public void AddRoleDefAsync(string roleName, string description, ulong permissionMask)
        {
            this.AddRoleDefAsync(roleName, description, permissionMask, null);
        }

        public void AddRoleDefAsync(string roleName, string description, ulong permissionMask, object userState)
        {
            if (this.AddRoleDefOperationCompleted == null)
            {
                this.AddRoleDefOperationCompleted = new SendOrPostCallback(this.OnAddRoleDefOperationCompleted);
            }

            object[] objArray = new object[] { roleName, description, permissionMask };
            base.InvokeAsync("AddRoleDef", objArray, this.AddRoleDefOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddUserCollectionToGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddUserCollectionToGroup(string groupName, XmlNode usersInfoXml)
        {
            object[] objArray = new object[] { groupName, usersInfoXml };
            base.Invoke("AddUserCollectionToGroup", objArray);
        }

        public void AddUserCollectionToGroupAsync(string groupName, XmlNode usersInfoXml)
        {
            this.AddUserCollectionToGroupAsync(groupName, usersInfoXml, null);
        }

        public void AddUserCollectionToGroupAsync(string groupName, XmlNode usersInfoXml, object userState)
        {
            if (this.AddUserCollectionToGroupOperationCompleted == null)
            {
                this.AddUserCollectionToGroupOperationCompleted =
                    new SendOrPostCallback(this.OnAddUserCollectionToGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName, usersInfoXml };
            base.InvokeAsync("AddUserCollectionToGroup", objArray, this.AddUserCollectionToGroupOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddUserCollectionToRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddUserCollectionToRole(string roleName, XmlNode usersInfoXml)
        {
            object[] objArray = new object[] { roleName, usersInfoXml };
            base.Invoke("AddUserCollectionToRole", objArray);
        }

        public void AddUserCollectionToRoleAsync(string roleName, XmlNode usersInfoXml)
        {
            this.AddUserCollectionToRoleAsync(roleName, usersInfoXml, null);
        }

        public void AddUserCollectionToRoleAsync(string roleName, XmlNode usersInfoXml, object userState)
        {
            if (this.AddUserCollectionToRoleOperationCompleted == null)
            {
                this.AddUserCollectionToRoleOperationCompleted =
                    new SendOrPostCallback(this.OnAddUserCollectionToRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, usersInfoXml };
            base.InvokeAsync("AddUserCollectionToRole", objArray, this.AddUserCollectionToRoleOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddUserToGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddUserToGroup(string groupName, string userName, string userLoginName, string userEmail,
            string userNotes)
        {
            object[] objArray = new object[] { groupName, userName, userLoginName, userEmail, userNotes };
            base.Invoke("AddUserToGroup", objArray);
        }

        public void AddUserToGroupAsync(string groupName, string userName, string userLoginName, string userEmail,
            string userNotes)
        {
            this.AddUserToGroupAsync(groupName, userName, userLoginName, userEmail, userNotes, null);
        }

        public void AddUserToGroupAsync(string groupName, string userName, string userLoginName, string userEmail,
            string userNotes, object userState)
        {
            if (this.AddUserToGroupOperationCompleted == null)
            {
                this.AddUserToGroupOperationCompleted = new SendOrPostCallback(this.OnAddUserToGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName, userName, userLoginName, userEmail, userNotes };
            base.InvokeAsync("AddUserToGroup", objArray, this.AddUserToGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/AddUserToRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddUserToRole(string roleName, string userName, string userLoginName, string userEmail,
            string userNotes)
        {
            object[] objArray = new object[] { roleName, userName, userLoginName, userEmail, userNotes };
            base.Invoke("AddUserToRole", objArray);
        }

        public void AddUserToRoleAsync(string roleName, string userName, string userLoginName, string userEmail,
            string userNotes)
        {
            this.AddUserToRoleAsync(roleName, userName, userLoginName, userEmail, userNotes, null);
        }

        public void AddUserToRoleAsync(string roleName, string userName, string userLoginName, string userEmail,
            string userNotes, object userState)
        {
            if (this.AddUserToRoleOperationCompleted == null)
            {
                this.AddUserToRoleOperationCompleted = new SendOrPostCallback(this.OnAddUserToRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, userName, userLoginName, userEmail, userNotes };
            base.InvokeAsync("AddUserToRole", objArray, this.AddUserToRoleOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetAllUserCollectionFromWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetAllUserCollectionFromWeb()
        {
            object[] objArray = base.Invoke("GetAllUserCollectionFromWeb", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetAllUserCollectionFromWebAsync()
        {
            this.GetAllUserCollectionFromWebAsync(null);
        }

        public void GetAllUserCollectionFromWebAsync(object userState)
        {
            if (this.GetAllUserCollectionFromWebOperationCompleted == null)
            {
                this.GetAllUserCollectionFromWebOperationCompleted =
                    new SendOrPostCallback(this.OnGetAllUserCollectionFromWebOperationCompleted);
            }

            base.InvokeAsync("GetAllUserCollectionFromWeb", new object[0],
                this.GetAllUserCollectionFromWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetGroupCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetGroupCollection(XmlNode groupNamesXml)
        {
            object[] objArray = new object[] { groupNamesXml };
            return (XmlNode)base.Invoke("GetGroupCollection", objArray)[0];
        }

        public void GetGroupCollectionAsync(XmlNode groupNamesXml)
        {
            this.GetGroupCollectionAsync(groupNamesXml, null);
        }

        public void GetGroupCollectionAsync(XmlNode groupNamesXml, object userState)
        {
            if (this.GetGroupCollectionOperationCompleted == null)
            {
                this.GetGroupCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetGroupCollectionOperationCompleted);
            }

            object[] objArray = new object[] { groupNamesXml };
            base.InvokeAsync("GetGroupCollection", objArray, this.GetGroupCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetGroupCollectionFromRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetGroupCollectionFromRole(string roleName)
        {
            object[] objArray = new object[] { roleName };
            return (XmlNode)base.Invoke("GetGroupCollectionFromRole", objArray)[0];
        }

        public void GetGroupCollectionFromRoleAsync(string roleName)
        {
            this.GetGroupCollectionFromRoleAsync(roleName, null);
        }

        public void GetGroupCollectionFromRoleAsync(string roleName, object userState)
        {
            if (this.GetGroupCollectionFromRoleOperationCompleted == null)
            {
                this.GetGroupCollectionFromRoleOperationCompleted =
                    new SendOrPostCallback(this.OnGetGroupCollectionFromRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName };
            base.InvokeAsync("GetGroupCollectionFromRole", objArray, this.GetGroupCollectionFromRoleOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetGroupCollectionFromSite",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetGroupCollectionFromSite()
        {
            object[] objArray = base.Invoke("GetGroupCollectionFromSite", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetGroupCollectionFromSiteAsync()
        {
            this.GetGroupCollectionFromSiteAsync(null);
        }

        public void GetGroupCollectionFromSiteAsync(object userState)
        {
            if (this.GetGroupCollectionFromSiteOperationCompleted == null)
            {
                this.GetGroupCollectionFromSiteOperationCompleted =
                    new SendOrPostCallback(this.OnGetGroupCollectionFromSiteOperationCompleted);
            }

            base.InvokeAsync("GetGroupCollectionFromSite", new object[0],
                this.GetGroupCollectionFromSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetGroupCollectionFromUser",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetGroupCollectionFromUser(string userLoginName)
        {
            object[] objArray = new object[] { userLoginName };
            return (XmlNode)base.Invoke("GetGroupCollectionFromUser", objArray)[0];
        }

        public void GetGroupCollectionFromUserAsync(string userLoginName)
        {
            this.GetGroupCollectionFromUserAsync(userLoginName, null);
        }

        public void GetGroupCollectionFromUserAsync(string userLoginName, object userState)
        {
            if (this.GetGroupCollectionFromUserOperationCompleted == null)
            {
                this.GetGroupCollectionFromUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetGroupCollectionFromUserOperationCompleted);
            }

            object[] objArray = new object[] { userLoginName };
            base.InvokeAsync("GetGroupCollectionFromUser", objArray, this.GetGroupCollectionFromUserOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetGroupCollectionFromWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetGroupCollectionFromWeb()
        {
            object[] objArray = base.Invoke("GetGroupCollectionFromWeb", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetGroupCollectionFromWebAsync()
        {
            this.GetGroupCollectionFromWebAsync(null);
        }

        public void GetGroupCollectionFromWebAsync(object userState)
        {
            if (this.GetGroupCollectionFromWebOperationCompleted == null)
            {
                this.GetGroupCollectionFromWebOperationCompleted =
                    new SendOrPostCallback(this.OnGetGroupCollectionFromWebOperationCompleted);
            }

            base.InvokeAsync("GetGroupCollectionFromWeb", new object[0],
                this.GetGroupCollectionFromWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetGroupInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetGroupInfo(string groupName)
        {
            object[] objArray = new object[] { groupName };
            return (XmlNode)base.Invoke("GetGroupInfo", objArray)[0];
        }

        public void GetGroupInfoAsync(string groupName)
        {
            this.GetGroupInfoAsync(groupName, null);
        }

        public void GetGroupInfoAsync(string groupName, object userState)
        {
            if (this.GetGroupInfoOperationCompleted == null)
            {
                this.GetGroupInfoOperationCompleted = new SendOrPostCallback(this.OnGetGroupInfoOperationCompleted);
            }

            object[] objArray = new object[] { groupName };
            base.InvokeAsync("GetGroupInfo", objArray, this.GetGroupInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetRoleCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRoleCollection(XmlNode roleNamesXml)
        {
            object[] objArray = new object[] { roleNamesXml };
            return (XmlNode)base.Invoke("GetRoleCollection", objArray)[0];
        }

        public void GetRoleCollectionAsync(XmlNode roleNamesXml)
        {
            this.GetRoleCollectionAsync(roleNamesXml, null);
        }

        public void GetRoleCollectionAsync(XmlNode roleNamesXml, object userState)
        {
            if (this.GetRoleCollectionOperationCompleted == null)
            {
                this.GetRoleCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetRoleCollectionOperationCompleted);
            }

            object[] objArray = new object[] { roleNamesXml };
            base.InvokeAsync("GetRoleCollection", objArray, this.GetRoleCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetRoleCollectionFromGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRoleCollectionFromGroup(string groupName)
        {
            object[] objArray = new object[] { groupName };
            return (XmlNode)base.Invoke("GetRoleCollectionFromGroup", objArray)[0];
        }

        public void GetRoleCollectionFromGroupAsync(string groupName)
        {
            this.GetRoleCollectionFromGroupAsync(groupName, null);
        }

        public void GetRoleCollectionFromGroupAsync(string groupName, object userState)
        {
            if (this.GetRoleCollectionFromGroupOperationCompleted == null)
            {
                this.GetRoleCollectionFromGroupOperationCompleted =
                    new SendOrPostCallback(this.OnGetRoleCollectionFromGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName };
            base.InvokeAsync("GetRoleCollectionFromGroup", objArray, this.GetRoleCollectionFromGroupOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetRoleCollectionFromUser",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRoleCollectionFromUser(string userLoginName)
        {
            object[] objArray = new object[] { userLoginName };
            return (XmlNode)base.Invoke("GetRoleCollectionFromUser", objArray)[0];
        }

        public void GetRoleCollectionFromUserAsync(string userLoginName)
        {
            this.GetRoleCollectionFromUserAsync(userLoginName, null);
        }

        public void GetRoleCollectionFromUserAsync(string userLoginName, object userState)
        {
            if (this.GetRoleCollectionFromUserOperationCompleted == null)
            {
                this.GetRoleCollectionFromUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetRoleCollectionFromUserOperationCompleted);
            }

            object[] objArray = new object[] { userLoginName };
            base.InvokeAsync("GetRoleCollectionFromUser", objArray, this.GetRoleCollectionFromUserOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetRoleCollectionFromWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRoleCollectionFromWeb()
        {
            object[] objArray = base.Invoke("GetRoleCollectionFromWeb", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetRoleCollectionFromWebAsync()
        {
            this.GetRoleCollectionFromWebAsync(null);
        }

        public void GetRoleCollectionFromWebAsync(object userState)
        {
            if (this.GetRoleCollectionFromWebOperationCompleted == null)
            {
                this.GetRoleCollectionFromWebOperationCompleted =
                    new SendOrPostCallback(this.OnGetRoleCollectionFromWebOperationCompleted);
            }

            base.InvokeAsync("GetRoleCollectionFromWeb", new object[0], this.GetRoleCollectionFromWebOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetRoleInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRoleInfo(string roleName)
        {
            object[] objArray = new object[] { roleName };
            return (XmlNode)base.Invoke("GetRoleInfo", objArray)[0];
        }

        public void GetRoleInfoAsync(string roleName)
        {
            this.GetRoleInfoAsync(roleName, null);
        }

        public void GetRoleInfoAsync(string roleName, object userState)
        {
            if (this.GetRoleInfoOperationCompleted == null)
            {
                this.GetRoleInfoOperationCompleted = new SendOrPostCallback(this.OnGetRoleInfoOperationCompleted);
            }

            object[] objArray = new object[] { roleName };
            base.InvokeAsync("GetRoleInfo", objArray, this.GetRoleInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://schemas.microsoft.com/sharepoint/soap/directory/GetRolesAndPermissionsForCurrentUser",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRolesAndPermissionsForCurrentUser()
        {
            object[] objArray = base.Invoke("GetRolesAndPermissionsForCurrentUser", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetRolesAndPermissionsForCurrentUserAsync()
        {
            this.GetRolesAndPermissionsForCurrentUserAsync(null);
        }

        public void GetRolesAndPermissionsForCurrentUserAsync(object userState)
        {
            if (this.GetRolesAndPermissionsForCurrentUserOperationCompleted == null)
            {
                this.GetRolesAndPermissionsForCurrentUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetRolesAndPermissionsForCurrentUserOperationCompleted);
            }

            base.InvokeAsync("GetRolesAndPermissionsForCurrentUser", new object[0],
                this.GetRolesAndPermissionsForCurrentUserOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetRolesAndPermissionsForSite",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetRolesAndPermissionsForSite()
        {
            object[] objArray = base.Invoke("GetRolesAndPermissionsForSite", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetRolesAndPermissionsForSiteAsync()
        {
            this.GetRolesAndPermissionsForSiteAsync(null);
        }

        public void GetRolesAndPermissionsForSiteAsync(object userState)
        {
            if (this.GetRolesAndPermissionsForSiteOperationCompleted == null)
            {
                this.GetRolesAndPermissionsForSiteOperationCompleted =
                    new SendOrPostCallback(this.OnGetRolesAndPermissionsForSiteOperationCompleted);
            }

            base.InvokeAsync("GetRolesAndPermissionsForSite", new object[0],
                this.GetRolesAndPermissionsForSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserCollection",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserCollection(XmlNode userLoginNamesXml)
        {
            object[] objArray = new object[] { userLoginNamesXml };
            return (XmlNode)base.Invoke("GetUserCollection", objArray)[0];
        }

        public void GetUserCollectionAsync(XmlNode userLoginNamesXml)
        {
            this.GetUserCollectionAsync(userLoginNamesXml, null);
        }

        public void GetUserCollectionAsync(XmlNode userLoginNamesXml, object userState)
        {
            if (this.GetUserCollectionOperationCompleted == null)
            {
                this.GetUserCollectionOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserCollectionOperationCompleted);
            }

            object[] objArray = new object[] { userLoginNamesXml };
            base.InvokeAsync("GetUserCollection", objArray, this.GetUserCollectionOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserCollectionFromGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserCollectionFromGroup(string groupName)
        {
            object[] objArray = new object[] { groupName };
            return (XmlNode)base.Invoke("GetUserCollectionFromGroup", objArray)[0];
        }

        public void GetUserCollectionFromGroupAsync(string groupName)
        {
            this.GetUserCollectionFromGroupAsync(groupName, null);
        }

        public void GetUserCollectionFromGroupAsync(string groupName, object userState)
        {
            if (this.GetUserCollectionFromGroupOperationCompleted == null)
            {
                this.GetUserCollectionFromGroupOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserCollectionFromGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName };
            base.InvokeAsync("GetUserCollectionFromGroup", objArray, this.GetUserCollectionFromGroupOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserCollectionFromRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserCollectionFromRole(string roleName)
        {
            object[] objArray = new object[] { roleName };
            return (XmlNode)base.Invoke("GetUserCollectionFromRole", objArray)[0];
        }

        public void GetUserCollectionFromRoleAsync(string roleName)
        {
            this.GetUserCollectionFromRoleAsync(roleName, null);
        }

        public void GetUserCollectionFromRoleAsync(string roleName, object userState)
        {
            if (this.GetUserCollectionFromRoleOperationCompleted == null)
            {
                this.GetUserCollectionFromRoleOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserCollectionFromRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName };
            base.InvokeAsync("GetUserCollectionFromRole", objArray, this.GetUserCollectionFromRoleOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserCollectionFromSite",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserCollectionFromSite()
        {
            object[] objArray = base.Invoke("GetUserCollectionFromSite", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetUserCollectionFromSiteAsync()
        {
            this.GetUserCollectionFromSiteAsync(null);
        }

        public void GetUserCollectionFromSiteAsync(object userState)
        {
            if (this.GetUserCollectionFromSiteOperationCompleted == null)
            {
                this.GetUserCollectionFromSiteOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserCollectionFromSiteOperationCompleted);
            }

            base.InvokeAsync("GetUserCollectionFromSite", new object[0],
                this.GetUserCollectionFromSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserCollectionFromWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserCollectionFromWeb()
        {
            object[] objArray = base.Invoke("GetUserCollectionFromWeb", new object[0]);
            return (XmlNode)objArray[0];
        }

        public void GetUserCollectionFromWebAsync()
        {
            this.GetUserCollectionFromWebAsync(null);
        }

        public void GetUserCollectionFromWebAsync(object userState)
        {
            if (this.GetUserCollectionFromWebOperationCompleted == null)
            {
                this.GetUserCollectionFromWebOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserCollectionFromWebOperationCompleted);
            }

            base.InvokeAsync("GetUserCollectionFromWeb", new object[0], this.GetUserCollectionFromWebOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserInfo(string userLoginName)
        {
            object[] objArray = new object[] { userLoginName };
            return (XmlNode)base.Invoke("GetUserInfo", objArray)[0];
        }

        public void GetUserInfoAsync(string userLoginName)
        {
            this.GetUserInfoAsync(userLoginName, null);
        }

        public void GetUserInfoAsync(string userLoginName, object userState)
        {
            if (this.GetUserInfoOperationCompleted == null)
            {
                this.GetUserInfoOperationCompleted = new SendOrPostCallback(this.OnGetUserInfoOperationCompleted);
            }

            object[] objArray = new object[] { userLoginName };
            base.InvokeAsync("GetUserInfo", objArray, this.GetUserInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/GetUserLoginFromEmail",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetUserLoginFromEmail(XmlNode emailXml)
        {
            object[] objArray = new object[] { emailXml };
            return (XmlNode)base.Invoke("GetUserLoginFromEmail", objArray)[0];
        }

        public void GetUserLoginFromEmailAsync(XmlNode emailXml)
        {
            this.GetUserLoginFromEmailAsync(emailXml, null);
        }

        public void GetUserLoginFromEmailAsync(XmlNode emailXml, object userState)
        {
            if (this.GetUserLoginFromEmailOperationCompleted == null)
            {
                this.GetUserLoginFromEmailOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserLoginFromEmailOperationCompleted);
            }

            object[] objArray = new object[] { emailXml };
            base.InvokeAsync("GetUserLoginFromEmail", objArray, this.GetUserLoginFromEmailOperationCompleted,
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

        private void OnAddGroupOperationCompleted(object arg)
        {
            if (this.AddGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddGroupToRoleOperationCompleted(object arg)
        {
            if (this.AddGroupToRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddGroupToRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddRoleDefOperationCompleted(object arg)
        {
            if (this.AddRoleDefCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddRoleDefCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddRoleOperationCompleted(object arg)
        {
            if (this.AddRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddUserCollectionToGroupOperationCompleted(object arg)
        {
            if (this.AddUserCollectionToGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddUserCollectionToGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddUserCollectionToRoleOperationCompleted(object arg)
        {
            if (this.AddUserCollectionToRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddUserCollectionToRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddUserToGroupOperationCompleted(object arg)
        {
            if (this.AddUserToGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddUserToGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddUserToRoleOperationCompleted(object arg)
        {
            if (this.AddUserToRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddUserToRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAllUserCollectionFromWebOperationCompleted(object arg)
        {
            if (this.GetAllUserCollectionFromWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAllUserCollectionFromWebCompleted(this,
                    new GetAllUserCollectionFromWebCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupCollectionFromRoleOperationCompleted(object arg)
        {
            if (this.GetGroupCollectionFromRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupCollectionFromRoleCompleted(this,
                    new GetGroupCollectionFromRoleCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupCollectionFromSiteOperationCompleted(object arg)
        {
            if (this.GetGroupCollectionFromSiteCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupCollectionFromSiteCompleted(this,
                    new GetGroupCollectionFromSiteCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupCollectionFromUserOperationCompleted(object arg)
        {
            if (this.GetGroupCollectionFromUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupCollectionFromUserCompleted(this,
                    new GetGroupCollectionFromUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupCollectionFromWebOperationCompleted(object arg)
        {
            if (this.GetGroupCollectionFromWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupCollectionFromWebCompleted(this,
                    new GetGroupCollectionFromWebCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupCollectionOperationCompleted(object arg)
        {
            if (this.GetGroupCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupCollectionCompleted(this,
                    new GetGroupCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetGroupInfoOperationCompleted(object arg)
        {
            if (this.GetGroupInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetGroupInfoCompleted(this,
                    new GetGroupInfoCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRoleCollectionFromGroupOperationCompleted(object arg)
        {
            if (this.GetRoleCollectionFromGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRoleCollectionFromGroupCompleted(this,
                    new GetRoleCollectionFromGroupCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRoleCollectionFromUserOperationCompleted(object arg)
        {
            if (this.GetRoleCollectionFromUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRoleCollectionFromUserCompleted(this,
                    new GetRoleCollectionFromUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRoleCollectionFromWebOperationCompleted(object arg)
        {
            if (this.GetRoleCollectionFromWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRoleCollectionFromWebCompleted(this,
                    new GetRoleCollectionFromWebCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRoleCollectionOperationCompleted(object arg)
        {
            if (this.GetRoleCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRoleCollectionCompleted(this,
                    new GetRoleCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRoleInfoOperationCompleted(object arg)
        {
            if (this.GetRoleInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRoleInfoCompleted(this,
                    new GetRoleInfoCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRolesAndPermissionsForCurrentUserOperationCompleted(object arg)
        {
            if (this.GetRolesAndPermissionsForCurrentUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRolesAndPermissionsForCurrentUserCompleted(this,
                    new GetRolesAndPermissionsForCurrentUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRolesAndPermissionsForSiteOperationCompleted(object arg)
        {
            if (this.GetRolesAndPermissionsForSiteCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRolesAndPermissionsForSiteCompleted(this,
                    new GetRolesAndPermissionsForSiteCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserCollectionFromGroupOperationCompleted(object arg)
        {
            if (this.GetUserCollectionFromGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserCollectionFromGroupCompleted(this,
                    new GetUserCollectionFromGroupCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserCollectionFromRoleOperationCompleted(object arg)
        {
            if (this.GetUserCollectionFromRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserCollectionFromRoleCompleted(this,
                    new GetUserCollectionFromRoleCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserCollectionFromSiteOperationCompleted(object arg)
        {
            if (this.GetUserCollectionFromSiteCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserCollectionFromSiteCompleted(this,
                    new GetUserCollectionFromSiteCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserCollectionFromWebOperationCompleted(object arg)
        {
            if (this.GetUserCollectionFromWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserCollectionFromWebCompleted(this,
                    new GetUserCollectionFromWebCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserCollectionOperationCompleted(object arg)
        {
            if (this.GetUserCollectionCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserCollectionCompleted(this,
                    new GetUserCollectionCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserInfoOperationCompleted(object arg)
        {
            if (this.GetUserInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserInfoCompleted(this,
                    new GetUserInfoCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserLoginFromEmailOperationCompleted(object arg)
        {
            if (this.GetUserLoginFromEmailCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserLoginFromEmailCompleted(this,
                    new GetUserLoginFromEmailCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveGroupFromRoleOperationCompleted(object arg)
        {
            if (this.RemoveGroupFromRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveGroupFromRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveGroupOperationCompleted(object arg)
        {
            if (this.RemoveGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveRoleOperationCompleted(object arg)
        {
            if (this.RemoveRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserCollectionFromGroupOperationCompleted(object arg)
        {
            if (this.RemoveUserCollectionFromGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserCollectionFromGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserCollectionFromRoleOperationCompleted(object arg)
        {
            if (this.RemoveUserCollectionFromRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserCollectionFromRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserCollectionFromSiteOperationCompleted(object arg)
        {
            if (this.RemoveUserCollectionFromSiteCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserCollectionFromSiteCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserFromGroupOperationCompleted(object arg)
        {
            if (this.RemoveUserFromGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserFromGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserFromRoleOperationCompleted(object arg)
        {
            if (this.RemoveUserFromRoleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserFromRoleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserFromSiteOperationCompleted(object arg)
        {
            if (this.RemoveUserFromSiteCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserFromSiteCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveUserFromWebOperationCompleted(object arg)
        {
            if (this.RemoveUserFromWebCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveUserFromWebCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateGroupInfoOperationCompleted(object arg)
        {
            if (this.UpdateGroupInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateGroupInfoCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateRoleDefInfoOperationCompleted(object arg)
        {
            if (this.UpdateRoleDefInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateRoleDefInfoCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateRoleInfoOperationCompleted(object arg)
        {
            if (this.UpdateRoleInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateRoleInfoCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateUserInfoOperationCompleted(object arg)
        {
            if (this.UpdateUserInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateUserInfoCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveGroup(string groupName)
        {
            object[] objArray = new object[] { groupName };
            base.Invoke("RemoveGroup", objArray);
        }

        public void RemoveGroupAsync(string groupName)
        {
            this.RemoveGroupAsync(groupName, null);
        }

        public void RemoveGroupAsync(string groupName, object userState)
        {
            if (this.RemoveGroupOperationCompleted == null)
            {
                this.RemoveGroupOperationCompleted = new SendOrPostCallback(this.OnRemoveGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName };
            base.InvokeAsync("RemoveGroup", objArray, this.RemoveGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveGroupFromRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveGroupFromRole(string roleName, string groupName)
        {
            object[] objArray = new object[] { roleName, groupName };
            base.Invoke("RemoveGroupFromRole", objArray);
        }

        public void RemoveGroupFromRoleAsync(string roleName, string groupName)
        {
            this.RemoveGroupFromRoleAsync(roleName, groupName, null);
        }

        public void RemoveGroupFromRoleAsync(string roleName, string groupName, object userState)
        {
            if (this.RemoveGroupFromRoleOperationCompleted == null)
            {
                this.RemoveGroupFromRoleOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveGroupFromRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, groupName };
            base.InvokeAsync("RemoveGroupFromRole", objArray, this.RemoveGroupFromRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveRole(string roleName)
        {
            object[] objArray = new object[] { roleName };
            base.Invoke("RemoveRole", objArray);
        }

        public void RemoveRoleAsync(string roleName)
        {
            this.RemoveRoleAsync(roleName, null);
        }

        public void RemoveRoleAsync(string roleName, object userState)
        {
            if (this.RemoveRoleOperationCompleted == null)
            {
                this.RemoveRoleOperationCompleted = new SendOrPostCallback(this.OnRemoveRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName };
            base.InvokeAsync("RemoveRole", objArray, this.RemoveRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserCollectionFromGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserCollectionFromGroup(string groupName, XmlNode userLoginNamesXml)
        {
            object[] objArray = new object[] { groupName, userLoginNamesXml };
            base.Invoke("RemoveUserCollectionFromGroup", objArray);
        }

        public void RemoveUserCollectionFromGroupAsync(string groupName, XmlNode userLoginNamesXml)
        {
            this.RemoveUserCollectionFromGroupAsync(groupName, userLoginNamesXml, null);
        }

        public void RemoveUserCollectionFromGroupAsync(string groupName, XmlNode userLoginNamesXml, object userState)
        {
            if (this.RemoveUserCollectionFromGroupOperationCompleted == null)
            {
                this.RemoveUserCollectionFromGroupOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserCollectionFromGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName, userLoginNamesXml };
            base.InvokeAsync("RemoveUserCollectionFromGroup", objArray,
                this.RemoveUserCollectionFromGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserCollectionFromRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserCollectionFromRole(string roleName, XmlNode userLoginNamesXml)
        {
            object[] objArray = new object[] { roleName, userLoginNamesXml };
            base.Invoke("RemoveUserCollectionFromRole", objArray);
        }

        public void RemoveUserCollectionFromRoleAsync(string roleName, XmlNode userLoginNamesXml)
        {
            this.RemoveUserCollectionFromRoleAsync(roleName, userLoginNamesXml, null);
        }

        public void RemoveUserCollectionFromRoleAsync(string roleName, XmlNode userLoginNamesXml, object userState)
        {
            if (this.RemoveUserCollectionFromRoleOperationCompleted == null)
            {
                this.RemoveUserCollectionFromRoleOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserCollectionFromRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, userLoginNamesXml };
            base.InvokeAsync("RemoveUserCollectionFromRole", objArray,
                this.RemoveUserCollectionFromRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserCollectionFromSite",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserCollectionFromSite(XmlNode userLoginNamesXml)
        {
            object[] objArray = new object[] { userLoginNamesXml };
            base.Invoke("RemoveUserCollectionFromSite", objArray);
        }

        public void RemoveUserCollectionFromSiteAsync(XmlNode userLoginNamesXml)
        {
            this.RemoveUserCollectionFromSiteAsync(userLoginNamesXml, null);
        }

        public void RemoveUserCollectionFromSiteAsync(XmlNode userLoginNamesXml, object userState)
        {
            if (this.RemoveUserCollectionFromSiteOperationCompleted == null)
            {
                this.RemoveUserCollectionFromSiteOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserCollectionFromSiteOperationCompleted);
            }

            object[] objArray = new object[] { userLoginNamesXml };
            base.InvokeAsync("RemoveUserCollectionFromSite", objArray,
                this.RemoveUserCollectionFromSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserFromGroup",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserFromGroup(string groupName, string userLoginName)
        {
            object[] objArray = new object[] { groupName, userLoginName };
            base.Invoke("RemoveUserFromGroup", objArray);
        }

        public void RemoveUserFromGroupAsync(string groupName, string userLoginName)
        {
            this.RemoveUserFromGroupAsync(groupName, userLoginName, null);
        }

        public void RemoveUserFromGroupAsync(string groupName, string userLoginName, object userState)
        {
            if (this.RemoveUserFromGroupOperationCompleted == null)
            {
                this.RemoveUserFromGroupOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserFromGroupOperationCompleted);
            }

            object[] objArray = new object[] { groupName, userLoginName };
            base.InvokeAsync("RemoveUserFromGroup", objArray, this.RemoveUserFromGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserFromRole",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserFromRole(string roleName, string userLoginName)
        {
            object[] objArray = new object[] { roleName, userLoginName };
            base.Invoke("RemoveUserFromRole", objArray);
        }

        public void RemoveUserFromRoleAsync(string roleName, string userLoginName)
        {
            this.RemoveUserFromRoleAsync(roleName, userLoginName, null);
        }

        public void RemoveUserFromRoleAsync(string roleName, string userLoginName, object userState)
        {
            if (this.RemoveUserFromRoleOperationCompleted == null)
            {
                this.RemoveUserFromRoleOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserFromRoleOperationCompleted);
            }

            object[] objArray = new object[] { roleName, userLoginName };
            base.InvokeAsync("RemoveUserFromRole", objArray, this.RemoveUserFromRoleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserFromSite",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserFromSite(string userLoginName)
        {
            object[] objArray = new object[] { userLoginName };
            base.Invoke("RemoveUserFromSite", objArray);
        }

        public void RemoveUserFromSiteAsync(string userLoginName)
        {
            this.RemoveUserFromSiteAsync(userLoginName, null);
        }

        public void RemoveUserFromSiteAsync(string userLoginName, object userState)
        {
            if (this.RemoveUserFromSiteOperationCompleted == null)
            {
                this.RemoveUserFromSiteOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserFromSiteOperationCompleted);
            }

            object[] objArray = new object[] { userLoginName };
            base.InvokeAsync("RemoveUserFromSite", objArray, this.RemoveUserFromSiteOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/RemoveUserFromWeb",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveUserFromWeb(string userLoginName)
        {
            object[] objArray = new object[] { userLoginName };
            base.Invoke("RemoveUserFromWeb", objArray);
        }

        public void RemoveUserFromWebAsync(string userLoginName)
        {
            this.RemoveUserFromWebAsync(userLoginName, null);
        }

        public void RemoveUserFromWebAsync(string userLoginName, object userState)
        {
            if (this.RemoveUserFromWebOperationCompleted == null)
            {
                this.RemoveUserFromWebOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveUserFromWebOperationCompleted);
            }

            object[] objArray = new object[] { userLoginName };
            base.InvokeAsync("RemoveUserFromWeb", objArray, this.RemoveUserFromWebOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/UpdateGroupInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateGroupInfo(string oldGroupName, string groupName, string ownerIdentifier, string ownerType,
            string description)
        {
            object[] objArray = new object[] { oldGroupName, groupName, ownerIdentifier, ownerType, description };
            base.Invoke("UpdateGroupInfo", objArray);
        }

        public void UpdateGroupInfoAsync(string oldGroupName, string groupName, string ownerIdentifier,
            string ownerType, string description)
        {
            this.UpdateGroupInfoAsync(oldGroupName, groupName, ownerIdentifier, ownerType, description, null);
        }

        public void UpdateGroupInfoAsync(string oldGroupName, string groupName, string ownerIdentifier,
            string ownerType, string description, object userState)
        {
            if (this.UpdateGroupInfoOperationCompleted == null)
            {
                this.UpdateGroupInfoOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateGroupInfoOperationCompleted);
            }

            object[] objArray = new object[] { oldGroupName, groupName, ownerIdentifier, ownerType, description };
            base.InvokeAsync("UpdateGroupInfo", objArray, this.UpdateGroupInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/UpdateRoleDefInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateRoleDefInfo(string oldRoleName, string roleName, string description, ulong permissionMask)
        {
            object[] objArray = new object[] { oldRoleName, roleName, description, permissionMask };
            base.Invoke("UpdateRoleDefInfo", objArray);
        }

        public void UpdateRoleDefInfoAsync(string oldRoleName, string roleName, string description,
            ulong permissionMask)
        {
            this.UpdateRoleDefInfoAsync(oldRoleName, roleName, description, permissionMask, null);
        }

        public void UpdateRoleDefInfoAsync(string oldRoleName, string roleName, string description,
            ulong permissionMask, object userState)
        {
            if (this.UpdateRoleDefInfoOperationCompleted == null)
            {
                this.UpdateRoleDefInfoOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateRoleDefInfoOperationCompleted);
            }

            object[] objArray = new object[] { oldRoleName, roleName, description, permissionMask };
            base.InvokeAsync("UpdateRoleDefInfo", objArray, this.UpdateRoleDefInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/UpdateRoleInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateRoleInfo(string oldRoleName, string roleName, string description, int permissionMask)
        {
            object[] objArray = new object[] { oldRoleName, roleName, description, permissionMask };
            base.Invoke("UpdateRoleInfo", objArray);
        }

        public void UpdateRoleInfoAsync(string oldRoleName, string roleName, string description, int permissionMask)
        {
            this.UpdateRoleInfoAsync(oldRoleName, roleName, description, permissionMask, null);
        }

        public void UpdateRoleInfoAsync(string oldRoleName, string roleName, string description, int permissionMask,
            object userState)
        {
            if (this.UpdateRoleInfoOperationCompleted == null)
            {
                this.UpdateRoleInfoOperationCompleted = new SendOrPostCallback(this.OnUpdateRoleInfoOperationCompleted);
            }

            object[] objArray = new object[] { oldRoleName, roleName, description, permissionMask };
            base.InvokeAsync("UpdateRoleInfo", objArray, this.UpdateRoleInfoOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/directory/UpdateUserInfo",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/directory/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateUserInfo(string userLoginName, string userName, string userEmail, string userNotes)
        {
            object[] objArray = new object[] { userLoginName, userName, userEmail, userNotes };
            base.Invoke("UpdateUserInfo", objArray);
        }

        public void UpdateUserInfoAsync(string userLoginName, string userName, string userEmail, string userNotes)
        {
            this.UpdateUserInfoAsync(userLoginName, userName, userEmail, userNotes, null);
        }

        public void UpdateUserInfoAsync(string userLoginName, string userName, string userEmail, string userNotes,
            object userState)
        {
            if (this.UpdateUserInfoOperationCompleted == null)
            {
                this.UpdateUserInfoOperationCompleted = new SendOrPostCallback(this.OnUpdateUserInfoOperationCompleted);
            }

            object[] objArray = new object[] { userLoginName, userName, userEmail, userNotes };
            base.InvokeAsync("UpdateUserInfo", objArray, this.UpdateUserInfoOperationCompleted, userState);
        }

        public event AddGroupCompletedEventHandler AddGroupCompleted;

        public event AddGroupToRoleCompletedEventHandler AddGroupToRoleCompleted;

        public event AddRoleCompletedEventHandler AddRoleCompleted;

        public event AddRoleDefCompletedEventHandler AddRoleDefCompleted;

        public event AddUserCollectionToGroupCompletedEventHandler AddUserCollectionToGroupCompleted;

        public event AddUserCollectionToRoleCompletedEventHandler AddUserCollectionToRoleCompleted;

        public event AddUserToGroupCompletedEventHandler AddUserToGroupCompleted;

        public event AddUserToRoleCompletedEventHandler AddUserToRoleCompleted;

        public event GetAllUserCollectionFromWebCompletedEventHandler GetAllUserCollectionFromWebCompleted;

        public event GetGroupCollectionCompletedEventHandler GetGroupCollectionCompleted;

        public event GetGroupCollectionFromRoleCompletedEventHandler GetGroupCollectionFromRoleCompleted;

        public event GetGroupCollectionFromSiteCompletedEventHandler GetGroupCollectionFromSiteCompleted;

        public event GetGroupCollectionFromUserCompletedEventHandler GetGroupCollectionFromUserCompleted;

        public event GetGroupCollectionFromWebCompletedEventHandler GetGroupCollectionFromWebCompleted;

        public event GetGroupInfoCompletedEventHandler GetGroupInfoCompleted;

        public event GetRoleCollectionCompletedEventHandler GetRoleCollectionCompleted;

        public event GetRoleCollectionFromGroupCompletedEventHandler GetRoleCollectionFromGroupCompleted;

        public event GetRoleCollectionFromUserCompletedEventHandler GetRoleCollectionFromUserCompleted;

        public event GetRoleCollectionFromWebCompletedEventHandler GetRoleCollectionFromWebCompleted;

        public event GetRoleInfoCompletedEventHandler GetRoleInfoCompleted;

        public event GetRolesAndPermissionsForCurrentUserCompletedEventHandler
            GetRolesAndPermissionsForCurrentUserCompleted;

        public event GetRolesAndPermissionsForSiteCompletedEventHandler GetRolesAndPermissionsForSiteCompleted;

        public event GetUserCollectionCompletedEventHandler GetUserCollectionCompleted;

        public event GetUserCollectionFromGroupCompletedEventHandler GetUserCollectionFromGroupCompleted;

        public event GetUserCollectionFromRoleCompletedEventHandler GetUserCollectionFromRoleCompleted;

        public event GetUserCollectionFromSiteCompletedEventHandler GetUserCollectionFromSiteCompleted;

        public event GetUserCollectionFromWebCompletedEventHandler GetUserCollectionFromWebCompleted;

        public event GetUserInfoCompletedEventHandler GetUserInfoCompleted;

        public event GetUserLoginFromEmailCompletedEventHandler GetUserLoginFromEmailCompleted;

        public event RemoveGroupCompletedEventHandler RemoveGroupCompleted;

        public event RemoveGroupFromRoleCompletedEventHandler RemoveGroupFromRoleCompleted;

        public event RemoveRoleCompletedEventHandler RemoveRoleCompleted;

        public event RemoveUserCollectionFromGroupCompletedEventHandler RemoveUserCollectionFromGroupCompleted;

        public event RemoveUserCollectionFromRoleCompletedEventHandler RemoveUserCollectionFromRoleCompleted;

        public event RemoveUserCollectionFromSiteCompletedEventHandler RemoveUserCollectionFromSiteCompleted;

        public event RemoveUserFromGroupCompletedEventHandler RemoveUserFromGroupCompleted;

        public event RemoveUserFromRoleCompletedEventHandler RemoveUserFromRoleCompleted;

        public event RemoveUserFromSiteCompletedEventHandler RemoveUserFromSiteCompleted;

        public event RemoveUserFromWebCompletedEventHandler RemoveUserFromWebCompleted;

        public event UpdateGroupInfoCompletedEventHandler UpdateGroupInfoCompleted;

        public event UpdateRoleDefInfoCompletedEventHandler UpdateRoleDefInfoCompleted;

        public event UpdateRoleInfoCompletedEventHandler UpdateRoleInfoCompleted;

        public event UpdateUserInfoCompletedEventHandler UpdateUserInfoCompleted;
    }
}