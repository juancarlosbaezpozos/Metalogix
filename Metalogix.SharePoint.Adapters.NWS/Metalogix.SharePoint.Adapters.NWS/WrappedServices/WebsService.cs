using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Webs;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class WebsService : BaseServiceWrapper
    {
        public WebsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Webs.Webs();
            base.InitializeWrappedWebService("Webs");
        }

        public string CreateContentType(string displayName, string parentType, XmlNode newFields,
            XmlNode contentTypeProperties)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { displayName, parentType, newFields, contentTypeProperties };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void CustomizeCss(string cssFile)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { cssFile };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode DeleteContentType(string contentTypeId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { contentTypeId };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string GetActivatedFeatures()
        {
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetAllSubWebCollection()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetColumns()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetContentType(string contentTypeId)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { contentTypeId };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetContentTypes()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public CustomizedPageStatus GetCustomizedPageStatus(string fileUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { fileUrl };
            return (CustomizedPageStatus)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListTemplates()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetWeb(string webUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { webUrl };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetWebCollection()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode RemoveContentTypeXmlDocument(string contentTypeId, string documentUri)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { contentTypeId, documentUri };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RevertAllFileContentStreams()
        {
            WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public void RevertCss(string cssFile)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { cssFile };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RevertFileContentStream(string fileUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { fileUrl };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateColumns(XmlNode newFields, XmlNode updateFields, XmlNode deleteFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { newFields, updateFields, deleteFields };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateContentType(string contentTypeId, XmlNode contentTypeProperties, XmlNode newFields,
            XmlNode updateFields, XmlNode deleteFields)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { contentTypeId, contentTypeProperties, newFields, updateFields, deleteFields };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateContentTypeXmlDocument(string contentTypeId, XmlNode newDocument)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { contentTypeId, newDocument };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string WebUrlFromPageUrl(string pageUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}