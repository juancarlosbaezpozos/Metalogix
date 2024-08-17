using Metalogix.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;

namespace Metalogix.Azure.ConsoleProcessor
{
	public class AzureContainerManager : IAzureContainerManager
	{
		public AzureContainerManager()
		{
		}

		private SharedAccessBlobPermissions ConvertPermissions(BlobAccessPermissions blobAccessPermissions)
		{
			SharedAccessBlobPermissions sharedAccessBlobPermission = SharedAccessBlobPermissions.None;
			if (blobAccessPermissions.HasFlag(BlobAccessPermissions.Read))
			{
				sharedAccessBlobPermission = SharedAccessBlobPermissions.Read;
			}
			if (blobAccessPermissions.HasFlag(BlobAccessPermissions.Write))
			{
				sharedAccessBlobPermission |= SharedAccessBlobPermissions.Write;
			}
			if (blobAccessPermissions.HasFlag(BlobAccessPermissions.List))
			{
				sharedAccessBlobPermission |= SharedAccessBlobPermissions.List;
			}
			if (blobAccessPermissions.HasFlag(BlobAccessPermissions.Delete))
			{
				sharedAccessBlobPermission |= SharedAccessBlobPermissions.Delete;
			}
			return sharedAccessBlobPermission;
		}

		private SharedAccessQueuePermissions ConvertPermissions(QueueAccessPermissions queueAccessPermissions)
		{
			SharedAccessQueuePermissions sharedAccessQueuePermission = SharedAccessQueuePermissions.None;
			if (queueAccessPermissions.HasFlag(QueueAccessPermissions.Read))
			{
				sharedAccessQueuePermission = SharedAccessQueuePermissions.Read;
			}
			if (queueAccessPermissions.HasFlag(QueueAccessPermissions.Add))
			{
				sharedAccessQueuePermission |= SharedAccessQueuePermissions.Add;
			}
			if (queueAccessPermissions.HasFlag(QueueAccessPermissions.ProcessMessages))
			{
				sharedAccessQueuePermission |= SharedAccessQueuePermissions.ProcessMessages;
			}
			if (queueAccessPermissions.HasFlag(QueueAccessPermissions.Update))
			{
				sharedAccessQueuePermission |= SharedAccessQueuePermissions.Update;
			}
			return sharedAccessQueuePermission;
		}

		public ContainerCreateResponse CreateContainerIfNotExists(string storageConnectionString, string containerName, BlobAccessPermissions permissions)
		{
			ContainerCreateResponse containerCreateResponse = new ContainerCreateResponse();
			try
			{
				CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
				CloudBlobContainer containerReference = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
				containerReference.CreateIfNotExists(null, null);
				SharedAccessBlobPermissions sharedAccessBlobPermission = this.ConvertPermissions(permissions);
				SharedAccessBlobPolicy sharedAccessBlobPolicy = new SharedAccessBlobPolicy()
				{
					Permissions = sharedAccessBlobPermission
				};
				DateTime utcNow = DateTime.UtcNow;
				sharedAccessBlobPolicy.SharedAccessExpiryTime = new DateTimeOffset?(utcNow.AddDays(7));
				string sharedAccessSignature = containerReference.GetSharedAccessSignature(sharedAccessBlobPolicy);
				containerCreateResponse.PrimaryUri = containerReference.StorageUri.PrimaryUri.ToString();
				containerCreateResponse.SecondaryUri = containerReference.StorageUri.SecondaryUri.ToString();
				containerCreateResponse.Success = true;
				containerCreateResponse.Details = string.Empty;
				containerCreateResponse.SharedAccessSignature = sharedAccessSignature;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				containerCreateResponse.Success = false;
				containerCreateResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return containerCreateResponse;
		}

		public CreateQueueResponse CreateQueueIfNotExists(string storageConnectionString, string queueName, QueueAccessPermissions permissions)
		{
			CreateQueueResponse createQueueResponse = new CreateQueueResponse();
			try
			{
				CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
				CloudQueue queueReference = cloudStorageAccount.CreateCloudQueueClient().GetQueueReference(queueName);
				queueReference.CreateIfNotExists(null, null);
				SharedAccessQueuePermissions sharedAccessQueuePermission = this.ConvertPermissions(permissions);
				SharedAccessQueuePolicy sharedAccessQueuePolicy = new SharedAccessQueuePolicy()
				{
					Permissions = sharedAccessQueuePermission
				};
				DateTime utcNow = DateTime.UtcNow;
				sharedAccessQueuePolicy.SharedAccessExpiryTime = new DateTimeOffset?(utcNow.AddYears(1));
				string sharedAccessSignature = queueReference.GetSharedAccessSignature(sharedAccessQueuePolicy);
				createQueueResponse.Name = queueReference.Name;
				createQueueResponse.PrimaryUri = queueReference.StorageUri.PrimaryUri.ToString();
				createQueueResponse.SecondaryUri = queueReference.StorageUri.SecondaryUri.ToString();
				createQueueResponse.SharedAccessSignature = sharedAccessSignature;
				createQueueResponse.Success = true;
				createQueueResponse.Details = string.Empty;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				createQueueResponse.Success = false;
				createQueueResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return createQueueResponse;
		}

		public DeleteContainerReponse DeleteContainer(string storageConnectionString, string containerName)
		{
			DeleteContainerReponse deleteContainerReponse = new DeleteContainerReponse();
			try
			{
				CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
				CloudBlobContainer containerReference = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
				deleteContainerReponse.Success = containerReference.DeleteIfExists(null, null, null);
				deleteContainerReponse.Details = (deleteContainerReponse.Success ? string.Empty : string.Format("Container '{0}' cannot be deleted", containerName));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				deleteContainerReponse.Success = false;
				deleteContainerReponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return deleteContainerReponse;
		}

		public DeleteQueueReponse DeleteQueue(string queueUri)
		{
			DeleteQueueReponse deleteQueueReponse = new DeleteQueueReponse();
			try
			{
				CloudQueue cloudQueue = new CloudQueue(new Uri(queueUri));
				deleteQueueReponse.Success = cloudQueue.DeleteIfExists(null, null);
				deleteQueueReponse.Details = (deleteQueueReponse.Success ? string.Empty : string.Format("Queue container at URI '{0}' cannot be deleted", queueUri));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				deleteQueueReponse.Success = false;
				deleteQueueReponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
			}
			return deleteQueueReponse;
		}
	}
}