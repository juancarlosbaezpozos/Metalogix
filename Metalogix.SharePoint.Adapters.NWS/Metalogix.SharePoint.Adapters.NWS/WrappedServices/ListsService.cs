using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Lists;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class ListsService : BaseServiceWrapper
    {
        public ListsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Lists.Lists();
            base.InitializeWrappedWebService("Lists");
        }

        public string AddAttachment(string listName, string listItemID, string fileName, byte[] attachment)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID, fileName, attachment };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode AddDiscussionBoardItem(string listName, byte[] message)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, message };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode AddList(string listName, string description, int templateID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, description, templateID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode AddListFromFeature(string listName, string description, Guid featureID, int templateID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, description, featureID, templateID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode ApplyContentTypeToList(string webUrl, string contentTypeID, string listName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { webUrl, contentTypeID, listName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public bool CheckInFile(string pageUrl, string comment, string checkInType)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, comment, checkInType };
            return (bool)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public bool CheckOutFile(string pageUrl, string checkoutToLocal, string lastModified)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl, checkoutToLocal, lastModified };
            return (bool)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public string CreateContentType(string listName, string displayName, string parentType, XmlNode fields,
            XmlNode contentTypeProperties, string addToView)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listName, displayName, parentType, fields, contentTypeProperties, addToView };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteAttachment(string listName, string listItemID, string url)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID, url };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode DeleteContentType(string listName, string contentTypeID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, contentTypeID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode DeleteContentTypeXmlDocument(string listName, string contentTypeId, string documentUri)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, contentTypeId, documentUri };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteList(string listName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetAttachmentCollection(string listName, string listItemID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetList(string listName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListAndView(string listName, string viewName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListCollection()
        {
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetListContentType(string listName, string contentTypeID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, contentTypeID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListContentTypes(string listName, string contentTypeID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, contentTypeID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListItemChanges(string listName, XmlNode viewFields, string since, XmlNode contains)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewFields, since, contains };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListItemChangesSinceToken(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, string changeToken, XmlNode contains)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listName, viewName, query, viewFields, rowLimit, queryOptions, changeToken, contains };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions, string webID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName, query, viewFields, rowLimit, queryOptions, webID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetVersionCollection(string listID, string listItemID, string fieldName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listID, listItemID, fieldName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public bool UndoCheckOut(string pageUrl)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { pageUrl };
            return (bool)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateContentType(string listName, string contentTypeID, XmlNode contentTypeProperties,
            XmlNode newFields, XmlNode updateFields, XmlNode deleteFields, string addToView)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listName, contentTypeID, contentTypeProperties, newFields, updateFields, deleteFields, addToView };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateContentTypesXmlDocument(string listName, XmlNode newDocument)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, newDocument };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateContentTypeXmlDocument(string listName, string contentTypeID, XmlNode newDocument)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, contentTypeID, newDocument };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateList(string listName, XmlNode listProperties, XmlNode newFields, XmlNode updateFields,
            XmlNode deleteFields, string listVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listName, listProperties, newFields, updateFields, deleteFields, listVersion };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateListItems(string listName, XmlNode updates)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, updates };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}