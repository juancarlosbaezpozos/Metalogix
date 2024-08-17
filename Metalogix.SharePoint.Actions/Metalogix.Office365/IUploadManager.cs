using Metalogix.Actions;
using Metalogix.Azure;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;

namespace Metalogix.Office365
{
	public interface IUploadManager : ICommonGlobalManifestOperations
	{
		List<Field> FieldNames
		{
			get;
		}

		void AddAlertToManifest(ManifestAlert alert);

		void AddDiscussionItemToManifest(ManifestDiscussionItem manifestDiscussionItem);

		void AddFileToManifest(ManifestFileItem manifestFileItem);

		void AddFolderToManifest(ManifestFolderItem manifestFolderItem);

		void AddListItemToManifest(ManifestListItem manifestListItem);

		void Cancel();

		void EndProcessing();

		int GetNextItemId();

		string GetStatusLog();

		void Initialise(Guid uploadSessionId, int maxBatchesToUpload, int batchSizeThresholdNoOfItems, int batchSizeThresholdInMB, BatchSizeMode batchSizeMode, IAzureBlobStorageManager azureBlobStorageManager, IAzureContainerFactory containerFactory, int maxThresholdCountForResubmission, string internalTestingMode, string tempBaseStorageDirectoryPath, Guid webId, Guid siteId, Guid listId, Guid rootWebFolderId, Guid listFolderId, Guid attachmentsFolderId, string parentWebAbsoluteUrl, string parentWebServerRelativeUrl, string targetBasePath, string listName, string listTitle, IMigrationPipeline migrationPipeline, IOperationLoggingManagement operationLoggingManagement, IOperationState operationState, List<Field> fieldNames, DateTime listCreated, DateTime listLastModified, string listBaseTemplate, string listBaseType, string webTemplate);

		void LogStatusLog(string message);

		string SaveDocument(byte[] fileContents);

		void StartProcessing();

		void Test();

		void WaitForAllToComplete();
	}
}