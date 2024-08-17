using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.UserGroup;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class UserGroupService : BaseServiceWrapper
    {
        public UserGroupService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.UserGroup.UserGroup();
            base.InitializeWrappedWebService("UserGroup");
        }

        public void AddGroup(string groupName, string ownerIdentifier, string ownerType, string defaultUserLoginName,
            string description)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { groupName, ownerIdentifier, ownerType, defaultUserLoginName, description };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddGroupToRole(string roleName, string groupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName, groupName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddRole(string roleName, string description, int permissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName, description, permissionMask };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddRoleDef(string roleName, string description, ulong permissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName, description, permissionMask };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddUserCollectionToGroup(string groupName, XmlNode usersInfoXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName, usersInfoXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddUserCollectionToRole(string groupName, XmlNode usersInfoXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName, usersInfoXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddUserToGroup(string groupName, string userName, string userLoginName, string userEmail,
            string userNotes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName, userName, userLoginName, userEmail, userNotes };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddUserToRole(string groupName, string userName, string userLoginName, string userEmail,
            string userNotes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName, userName, userLoginName, userEmail, userNotes };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetAllUserCollectionFromWeb()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetGroupCollection(XmlNode groupNamesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupNamesXml };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetGroupCollectionFromRole(string roleName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetGroupCollectionFromSite()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetGroupCollectionFromUser(string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetGroupCollectionFromWeb()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetGroupInfo(string groupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetRoleCollection(XmlNode roleNamesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleNamesXml };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetRoleCollectionFromGroup(string groupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetRoleCollectionFromUser(string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetRoleCollectionFromWeb()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetRoleInfo(string roleName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetRolesAndPermissionsForCurrentUser()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetRolesAndPermissionsForSite()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetUserCollection(XmlNode userLoginNamesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginNamesXml };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetUserCollectionFromGroup(string groupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetUserCollectionFromRole(string roleName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetUserCollectionFromSite()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetUserCollectionFromWeb()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetUserInfo(string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetUserLoginFromEmail(XmlNode emailXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { emailXml };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveGroup(string groupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveGroupFromRole(string roleName, string groupName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName, groupName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveRole(string roleName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserCollectionFromGroup(string groupName, XmlNode userLoginNamesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName, userLoginNamesXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserCollectionFromRole(string roleName, XmlNode userLoginNamesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName, userLoginNamesXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserCollectionFromSite(XmlNode userLoginNamesXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginNamesXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserFromGroup(string groupName, string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { groupName, userLoginName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserFromRole(string roleName, string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { roleName, userLoginName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserFromSite(string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveUserFromWeb(string userLoginName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void UpdateGroupInfo(string oldGroupName, string groupName, string ownerIdentifier, string ownerType,
            string description)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { oldGroupName, groupName, ownerIdentifier, ownerType, description };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void UpdateRoleDefInfo(string oldRoleName, string roleName, string description, ulong permissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { oldRoleName, roleName, description, permissionMask };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void UpdateRoleInfo(string oldRoleName, string roleName, string description, int permissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { oldRoleName, roleName, description, permissionMask };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void UpdateUserInfo(string userLoginName, string userName, string userEmail, string userNotes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { userLoginName, userName, userEmail, userNotes };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}