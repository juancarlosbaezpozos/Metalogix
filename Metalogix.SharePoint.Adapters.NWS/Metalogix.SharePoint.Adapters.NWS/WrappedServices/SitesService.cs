using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Sites;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class SitesService : BaseServiceWrapper
    {
        public SitesService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Sites.Sites();
            base.InitializeWrappedWebService("Sites");
        }

        public int ExportWeb(string jobName, string webUrl, string dataPath, bool includeSubwebs,
            bool includeUserSecurity, bool overWrite, int cabSize)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { jobName, webUrl, dataPath, includeSubwebs, includeUserSecurity, overWrite, cabSize };
            return (int)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public uint GetSiteTemplates(uint LCID, out Template[] templateList)
        {
            object[] lCID = new object[] { LCID, null };
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, lCID);
            templateList = (Template[])lCID[1];
            return num;
        }

        public string GetUpdatedFormDigest()
        {
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public int ImportWeb(string jobName, string webUrl, string[] dataFiles, string logPath,
            bool includeUserSecurity, bool overWrite)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { jobName, webUrl, dataFiles, logPath, includeUserSecurity, overWrite };
            return (int)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}