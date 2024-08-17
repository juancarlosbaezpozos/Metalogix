using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class FormsService : BaseServiceWrapper
    {
        public FormsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new FormsServicesWebService();
            base.InitializeWrappedWebService("FormsServices");
        }

        public MessagesResponse BrowserEnableUserFormTemplate(string formTemplateLocation)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { formTemplateLocation };
            return (MessagesResponse)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public DesignCheckerInformation DesignCheckFormTemplate(int lcid, string base64FormTemplate,
            string applicationId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { lcid, base64FormTemplate, applicationId };
            return (DesignCheckerInformation)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetListFormLocation(int lcid, string listGuid, string contentTypeId, bool checkDesignPermissions,
            bool checkCustomFormEnabled)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { lcid, listGuid, contentTypeId, checkDesignPermissions, checkCustomFormEnabled };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public UserSolutionActivationStatus GetUserCodeDeploymentDependencies(string siteCollectionLocation)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { siteCollectionLocation };
            return (UserSolutionActivationStatus)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public DesignCheckerInformation SetFormsForListItem(int lcid, string base64FormTemplate, string applicationId,
            string listGuid, string contentTypeId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { lcid, base64FormTemplate, applicationId, listGuid, contentTypeId };
            return (DesignCheckerInformation)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode SetSchemaChangesForList(int lcid, string listGuid, string contentTypeId, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { lcid, listGuid, contentTypeId, newFields, updateFields, deleteFields };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}