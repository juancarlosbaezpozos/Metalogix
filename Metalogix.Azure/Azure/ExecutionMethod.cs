using System;

namespace Metalogix.Azure
{
	public enum ExecutionMethod
	{
		UploadBlob,
		DownloadBlob,
		CreateContainer,
		CreateContainerSas,
		ListContainer,
		DeleteContainer,
		CreateQueue,
		CreateQueueSas,
		DeleteQueue,
		GetQueueMessage,
		GetQueueMessages
	}
}