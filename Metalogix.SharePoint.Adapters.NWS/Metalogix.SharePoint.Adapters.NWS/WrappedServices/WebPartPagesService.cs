using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.WebPartPages;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class WebPartPagesService : BaseServiceWrapper
    {
        public WebPartPagesService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new WebPartPagesWebService();
            base.InitializeWrappedWebService("webpartpages");
        }

        public Guid AddWebPart(string pageUrl, string webPartXml, Storage storage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, webPartXml, storage };
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public Guid AddWebPartToZone(string pageUrl, string webPartXml, Storage storage, string zoneId, int zoneIndex)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, webPartXml, storage, zoneId, zoneIndex };
            return (Guid)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string AssociateWorkflowMarkup(string configUrl, string configVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { configUrl, configVersion };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string ConvertWebPartFormat(string inputFormat, FormatConversionOption formatConversionOption)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { inputFormat, formatConversionOption };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteWebPart(string pageUrl, Guid storageKey, Storage storage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, storageKey, storage };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string ExecuteProxyUpdates(string updateData)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { updateData };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string FetchLegalWorkflowActions()
        {
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetAssemblyMetaData(string assemblyName, string baseTypes)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { assemblyName, baseTypes };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetBindingResourceData(string resourceName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { resourceName };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetCustomControlList()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetDataFromDataSourceControl(string dscXml, string contextUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { dscXml, contextUrl };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetFormCapabilityFromDataSourceControl(string dscXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { dscXml };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetSafeAssemblyInfo()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public string GetWebPart(string pageurl, Guid storageKey, Storage storage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageurl, storageKey, storage };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetWebPart2(string pageurl, Guid storageKey, Storage storage, SPWebServiceBehavior behavior)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageurl, storageKey, storage, behavior };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetWebPartCrossPageCompatibility(string sourcePageUrl, string sourcePageContents,
            string targetPageUrl, string targetPageContents, string providerPartID, string lcid)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { sourcePageUrl, sourcePageContents, targetPageUrl, targetPageContents, providerPartID, lcid };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetWebPartPage(string documentName, SPWebServiceBehavior behavior)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { documentName, behavior };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetWebPartPageConnectionInfo(string sourcePageUrl, string sourcePageContents,
            string providerPartID, string lcid)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { sourcePageUrl, sourcePageContents, providerPartID, lcid };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetWebPartPageDocument(string documentName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { documentName };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetWebPartProperties(string pageUrl, Storage storage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, storage };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetWebPartProperties2(string pageUrl, Storage storage, SPWebServiceBehavior behavior)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, storage, behavior };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetXmlDataFromDataSource(string queryXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { queryXml };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string RemoveWorkflowAssociation(string configUrl, string configVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { configUrl, configVersion };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string RenderWebPartForEdit(string webPartXml)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { webPartXml };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void SaveWebPart(string pageUrl, Guid storageKey, string webPartXml, Storage storage)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, storageKey, webPartXml, storage };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void SaveWebPart2(string pageUrl, Guid storageKey, string webPartXml, Storage storage,
            bool allowTypeChange)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, storageKey, webPartXml, storage, allowTypeChange };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string ValidateWorkflowMarkupAndCreateSupportObjects(string workflowMarkupText, string rulesText,
            string configBlob, string flag)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { workflowMarkupText, rulesText, configBlob, flag };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}