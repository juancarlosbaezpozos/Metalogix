using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Versions;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class VersionsService : BaseServiceWrapper
    {
        public VersionsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Versions.Versions();
            base.InitializeWrappedWebService("Versions");
        }

        public XmlNode DeleteAllVersions(string fileName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { fileName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode DeleteVersion(string fileName, string fileVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { fileName, fileVersion };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetVersions(string fileName)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { fileName };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode RestoreVersion(string fileName, string fileVersion)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { fileName, fileVersion };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}