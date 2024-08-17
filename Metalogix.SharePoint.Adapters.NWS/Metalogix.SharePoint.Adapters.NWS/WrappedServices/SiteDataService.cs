using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.SiteData;
using System;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class SiteDataService : BaseServiceWrapper
    {
        public SiteDataService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.SiteData.SiteData();
            base.InitializeWrappedWebService("SiteData");
        }

        public uint EnumerateFolder(string folderUrl, out _sFPUrl[] vUrls)
        {
            object[] objArray = new object[] { folderUrl, null };
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray);
            vUrls = (_sFPUrl[])objArray[1];
            return num;
        }

        public uint GetAttachments(string listName, string itemID, out string[] vAttachments)
        {
            object[] objArray = new object[] { listName, itemID, null };
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray);
            vAttachments = (string[])objArray[2];
            return num;
        }

        public string GetChanges(ObjectType objectType, string contentDatabaseID, ref string lastChangeID,
            ref string currentChangeID, int timeout, out bool moreChanges)
        {
            object[] objArray = new object[]
                { objectType, contentDatabaseID, lastChangeID, currentChangeID, timeout, false };
            object[] objArray1 = objArray;
            string str =
                (string)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, objArray1);
            lastChangeID = (string)objArray1[2];
            currentChangeID = (string)objArray1[3];
            moreChanges = (bool)objArray1[5];
            return str;
        }

        public string GetContent(ObjectType objectType, string objectId, string folderUrl, string itemId,
            bool retrieveChildItems, bool securityOnly, ref string lastItemIdOnPage)
        {
            object[] objArray = new object[]
                { objectType, objectId, folderUrl, itemId, retrieveChildItems, securityOnly, lastItemIdOnPage };
            object[] objArray1 = objArray;
            string str =
                (string)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, objArray1);
            lastItemIdOnPage = (string)objArray1[6];
            return str;
        }

        public uint GetList(string listName, out _sListMetadata sListMetadata, out _sProperty[] sProperty)
        {
            object[] objArray = new object[] { listName, null, null };
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray);
            sListMetadata = (_sListMetadata)objArray[1];
            sProperty = (_sProperty[])objArray[2];
            return num;
        }

        public uint GetListCollection(out _sList[] vLists)
        {
            object[] objArray = new object[1];
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray);
            vLists = (_sList[])objArray[0];
            return num;
        }

        public string GetListItems(string strListName, string strQuery, string strViewFields, uint uRowLimit)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { strListName, strQuery, strViewFields, uRowLimit };
            return (string)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public uint GetSite(out _sSiteMetadata sSiteMetadata, out _sWebWithTime[] vWebs, out string strUsers,
            out string strGroups, out string[] vGroups)
        {
            object[] objArray = new object[5];
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray);
            sSiteMetadata = (_sSiteMetadata)objArray[0];
            vWebs = (_sWebWithTime[])objArray[1];
            strUsers = (string)objArray[2];
            strGroups = (string)objArray[3];
            vGroups = (string[])objArray[4];
            return num;
        }

        public uint GetSiteAndWeb(string strUrl, out string strSite, out string strWeb)
        {
            object[] array = new object[3];
            array[0] = strUrl;
            object[] array2 = array;
            uint result =
                (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, array2);
            strSite = (string)array2[1];
            strWeb = (string)array2[2];
            return result;
        }

        public uint GetSiteUrl(string Url, out string siteUrl, out string siteId)
        {
            object[] url = new object[] { Url, null, null };
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, url);
            siteUrl = (string)url[1];
            siteId = (string)url[2];
            return num;
        }

        public bool GetURLSegments(string strURL, out string strWebID, out string strBucketID, out string strListID,
            out string strItemID)
        {
            object[] objArray = new object[] { strURL, null, null, null, null };
            object[] objArray1 = objArray;
            bool flag = (bool)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray1);
            strWebID = (string)objArray1[1];
            strBucketID = (string)objArray1[2];
            strListID = (string)objArray1[3];
            strItemID = (string)objArray1[4];
            return flag;
        }

        public uint GetWeb(out _sWebMetadata sWebMetadata, out _sWebWithTime[] vWebs, out _sListWithTime[] vLists,
            out _sFPUrl[] vFPUrls, out string strRoles, out string[] vRolesUsers, out string[] vRolesGroups)
        {
            object[] objArray = new object[7];
            uint num = (uint)WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name,
                objArray);
            sWebMetadata = (_sWebMetadata)objArray[0];
            vWebs = (_sWebWithTime[])objArray[1];
            vLists = (_sListWithTime[])objArray[2];
            vFPUrls = (_sFPUrl[])objArray[3];
            strRoles = (string)objArray[4];
            vRolesUsers = (string[])objArray[5];
            vRolesGroups = (string[])objArray[6];
            return num;
        }
    }
}