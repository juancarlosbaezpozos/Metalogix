using System;

namespace Metalogix.Azure
{
	public interface IAzureContainerManager
	{
		ContainerCreateResponse CreateContainerIfNotExists(string storageConnectionString, string containerName, BlobAccessPermissions permissions);

		CreateQueueResponse CreateQueueIfNotExists(string storageConnectionString, string queueName, QueueAccessPermissions permissions);

		DeleteContainerReponse DeleteContainer(string storageConnectionString, string containerName);

		DeleteQueueReponse DeleteQueue(string queueUri);
	}
}