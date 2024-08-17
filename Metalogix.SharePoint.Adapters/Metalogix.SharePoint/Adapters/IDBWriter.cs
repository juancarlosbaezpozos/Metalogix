using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public interface IDBWriter : ISharePointReader, ISharePointAdapterCommand
    {
        string HostHeader { get; set; }

        bool AddSiteUser(XmlNode user, Guid sSPSiteID, Guid sCurrentWebID, Guid sUserInfoListID,
            string sCurrentUserName);

        string AddWorkflow(string sListId, string sWorkflowXml);

        byte[] CheckForDeletedItem(string sListGUID, int iItemId);

        Hashtable GetExternalizationMetadata(string[] docIds, string sParentFolder, bool bRecursive);

        Hashtable GetExternalizationMetadata(string sDocId, bool bIncludeVersions);

        int GetNextAvailableID(string sListID);

        int UpdateAvailableItemID(string sListGUID, int iNewAvailableID);

        bool UpdateDocstreamContent(Guid docId, int iUIVersion, int iDocFlags, long lFileSize, byte[] content,
            byte[] rbsId);

        bool UpdateWorkflowAssociationModifiedTime(string sWorkflowID, string sCreatedTime, string sModifiedTime);
    }
}