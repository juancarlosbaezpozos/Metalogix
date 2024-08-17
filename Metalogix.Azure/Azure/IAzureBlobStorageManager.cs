using System;

namespace Metalogix.Azure
{
	public interface IAzureBlobStorageManager
	{
		bool IsEncryptionUsed
		{
			get;
			set;
		}

		DownloadResponse DownloadBlob(string containerUri, string blobName, string localFilePath, byte[] encryptionKey);

		QueueMessageResponse GetQueueMessage(string queueUri);

		QueueMessagesResponse GetQueueMessages(string queueUri, int numberOfMessagesToGet);

		ListResponse ListContainer(string containerUri);

		UploadResponse UploadBlob(string containerUri, string blobName, string localBlobFilePath, byte[] encryptionKey);
	}
}