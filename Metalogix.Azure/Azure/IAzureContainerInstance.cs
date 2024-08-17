using System;

namespace Metalogix.Azure
{
	public interface IAzureContainerInstance
	{
		byte[] EncryptionKey
		{
			get;
		}

		DeleteContainerReponse DeleteBlobContainer();

		DeleteContainerReponse DeleteManifestContainer();

		DeleteQueueReponse DeleteReportingQueue();

		SasResource GetBlobContainer();

		SasResource GetManifestContainer();

		SasResource GetReportingQueue();
	}
}