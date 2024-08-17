using Metalogix.Azure;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure.Blob.Manager
{
	public class AzureBlobStorageManager : IAzureBlobStorageManager
	{
		public bool IsEncryptionUsed
		{
			get;
			set;
		}

		public AzureBlobStorageManager()
		{
		}

		public DownloadResponse DownloadBlob(string containerUri, string blobName, string localFilePath, byte[] encryptionKey)
		{
			string str;
			if (!this.IsEncryptionUsed)
			{
				object[] objArray = new object[] { ExecutionMethod.DownloadBlob, containerUri, blobName, localFilePath };
				str = string.Format("{0} \"{1}\" \"{2}\" \"{3}\"", objArray);
			}
			else
			{
				object[] objArray1 = new object[] { ExecutionMethod.DownloadBlob, containerUri, blobName, localFilePath, this.IsEncryptionUsed, Convert.ToBase64String(encryptionKey) };
				str = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\"", objArray1);
			}
			return ConsoleUtils.Launch<DownloadResponse>(str);
		}

		public QueueMessageResponse GetQueueMessage(string queueUri)
		{
			string str = string.Format("{0} \"{1}\"", ExecutionMethod.GetQueueMessage, queueUri);
			return ConsoleUtils.Launch<QueueMessageResponse>(str);
		}

		public QueueMessagesResponse GetQueueMessages(string queueUri, int numberOfMessagesToGet)
		{
			string str = string.Format("{0} \"{1}\" \"{2}\"", ExecutionMethod.GetQueueMessages, queueUri, numberOfMessagesToGet);
			return ConsoleUtils.Launch<QueueMessagesResponse>(str);
		}

		public ListResponse ListContainer(string containerUri)
		{
			string str = string.Format("{0} \"{1}\"", ExecutionMethod.ListContainer, containerUri);
			return ConsoleUtils.Launch<ListResponse>(str);
		}

		public UploadResponse UploadBlob(string containerUri, string blobName, string localBlobFilePath, byte[] encryptionKey)
		{
			string str;
			if (!this.IsEncryptionUsed)
			{
				object[] objArray = new object[] { ExecutionMethod.UploadBlob, containerUri, blobName, localBlobFilePath };
				str = string.Format("{0} \"{1}\" \"{2}\" \"{3}\"", objArray);
			}
			else
			{
				object[] objArray1 = new object[] { ExecutionMethod.UploadBlob, containerUri, blobName, localBlobFilePath, this.IsEncryptionUsed, Convert.ToBase64String(encryptionKey) };
				str = string.Format("{0} \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\"", objArray1);
			}
			return ConsoleUtils.Launch<UploadResponse>(str);
		}
	}
}