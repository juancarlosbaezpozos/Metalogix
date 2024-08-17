using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Permissions;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class PermissionsService : BaseServiceWrapper
    {
        public PermissionsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Permissions.Permissions();
            base.InitializeWrappedWebService("permissions");
        }

        public void AddPermission(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { objectName, objectType, permissionIdentifier, permissionType, permissionMask };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void AddPermissionCollection(string objectName, string objectType, XmlNode permissionInfoXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { objectName, objectType, permissionInfoXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetPermissionCollection(string objectName, string objectType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { objectName, objectType };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemovePermission(string objectName, string objectType, string permissionIdentifier,
            string permissionType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { objectName, objectType, permissionIdentifier, permissionType };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemovePermissionCollection(string objectName, string objectType, XmlNode membersIdsXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { objectName, objectType, membersIdsXml };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void UpdatePermission(string objectName, string objectType, string permissionIdentifier,
            string permissionType, int permissionMask)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { objectName, objectType, permissionIdentifier, permissionType, permissionMask };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}