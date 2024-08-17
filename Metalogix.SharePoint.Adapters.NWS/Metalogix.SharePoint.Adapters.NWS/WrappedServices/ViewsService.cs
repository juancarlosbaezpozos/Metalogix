using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Views;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class ViewsService : BaseServiceWrapper
    {
        public ViewsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Views.Views();
            base.InitializeWrappedWebService("Views");
        }

        public XmlNode AddView(string listName, string viewName, XmlNode viewFields, XmlNode query, XmlNode rowLimit,
            string type, bool makeViewDefault)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName, viewFields, query, rowLimit, type, makeViewDefault };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteView(string listName, string viewName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetView(string listName, string viewName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetViewCollection(string listName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetViewHtml(string listName, string viewName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { listName, viewName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateView(string listName, string viewName, XmlNode viewProperties, XmlNode query,
            XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { listName, viewName, viewProperties, query, viewFields, aggregations, formats, rowLimit };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateViewHtml(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter, viewEmpty,
                rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit
            };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateViewHtml2(string listName, string viewName, XmlNode viewProperties, XmlNode toolbar,
            XmlNode viewHeader, XmlNode viewBody, XmlNode viewFooter, XmlNode viewEmpty, XmlNode rowLimitExceeded,
            XmlNode query, XmlNode viewFields, XmlNode aggregations, XmlNode formats, XmlNode rowLimit,
            string openApplicationExtension)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                listName, viewName, viewProperties, toolbar, viewHeader, viewBody, viewFooter, viewEmpty,
                rowLimitExceeded, query, viewFields, aggregations, formats, rowLimit, openApplicationExtension
            };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}