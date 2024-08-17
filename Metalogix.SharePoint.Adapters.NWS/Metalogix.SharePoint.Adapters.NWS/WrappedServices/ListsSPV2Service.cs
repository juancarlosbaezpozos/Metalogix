using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.ListsSPV2;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class ListsSPV2Service : BaseServiceWrapper
    {
        public ListsSPV2Service(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.ListsSPV2.Lists();
            base.InitializeWrappedWebService("Lists");
        }

        public string AddAttachment(string listName, string listItemID, string fileName, byte[] attachment)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID, fileName, attachment };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode AddList(string listName, string description, int templateID)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, description, templateID };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteAttachment(string listName, string listItemID, string url)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, listItemID, url };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
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

        public XmlNode GetListItemChanges(string listName, XmlNode viewFields, string since, XmlNode contains)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewFields, since, contains };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetListItems(string listName, string viewName, XmlNode query, XmlNode viewFields,
            string rowLimit, XmlNode queryOptions)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName, query, viewFields, rowLimit, queryOptions };
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