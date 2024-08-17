using Metalogix.Azure;
using System;

namespace Metalogix.Azure.Blob.Manager
{
	public class AzureContainerManager : IAzureContainerManager
	{
		public AzureContainerManager()
		{
		}

		public ContainerCreateResponse CreateContainerIfNotExists(string storageConnectionString, string containerName, BlobAccessPermissions permissions)
		{
			object[] objArray = new object[] { ExecutionMethod.CreateContainer, storageConnectionString, containerName, permissions };
			return ConsoleUtils.Launch<ContainerCreateResponse>(string.Format("{0} \"{1}\" \"{2}\" \"{3}\"", objArray));
		}

		public CreateQueueResponse CreateQueueIfNotExists(string storageConnectionString, string queueName, QueueAccessPermissions permissions)
		{
			object[] objArray = new object[] { ExecutionMethod.CreateQueue, storageConnectionString, queueName, permissions };
			return ConsoleUtils.Launch<CreateQueueResponse>(string.Format("{0} \"{1}\" \"{2}\" \"{3}\"", objArray));
		}

		public DeleteContainerReponse DeleteContainer(string storageConnectionString, string containerName)
		{
			string str = string.Format("{0} \"{1}\" \"{2}\"", ExecutionMethod.DeleteContainer, storageConnectionString, containerName);
			return ConsoleUtils.Launch<DeleteContainerReponse>(str);
		}

		public DeleteQueueReponse DeleteQueue(string queueUri)
		{
			string str = string.Format("{0} \"{1}\"", ExecutionMethod.DeleteQueue, queueUri);
			return ConsoleUtils.Launch<DeleteQueueReponse>(str);
		}
	}
}