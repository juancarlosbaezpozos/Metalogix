using System;

namespace Metalogix.Office365
{
    public interface IManifestBuilder
    {
        PackagingStatus Status { get; }

        string TempStorageDirectory { get; }

        void AddAlertToManifest(ManifestAlert alert);

        void AddDiscussionItemToManifest(ManifestDiscussionItem manifestDiscussionItem);

        void AddFileToManifest(ManifestFileItem manifestFileItem);

        void AddFolderToManifest(ManifestFolderItem manifestFolderItem);

        void AddListItemToManifest(ManifestListItem manifestListItem);

        void CreateBasePackageXml();

        void InitialiseManifest(string tempStorageDirectory, string commonDocumentBinaryDirectory, Guid webId,
            Guid siteId, Guid listId, Guid rootWebFolderId, Guid listFolderId, Guid attachmentsFolderId,
            string parentWebAbsoluteUrl, string parentWebServerRelativeUrl, string targetBasePath, string listName,
            string listTitle, ICommonGlobalManifestOperations commonGlobalManifestOperations,
            DateTime targetFolderCreated, DateTime targetFolderLastModified, string listBaseTemplate,
            string listBaseType, string webTemplate);

        void SaveManifest();
    }
}