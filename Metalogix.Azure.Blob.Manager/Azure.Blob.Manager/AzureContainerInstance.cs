using Metalogix.Azure;
using System;

namespace Metalogix.Azure.Blob.Manager
{
	public class AzureContainerInstance : IAzureContainerInstance
	{
		private readonly AzureContainerManager _acm = new AzureContainerManager();

		private readonly string _azureConnectionString;

		private readonly int _batchNo;

		private readonly byte[] _encrpytionKey;

		private readonly Guid _factoryId;

		private SasResource _blobContainer;

		private SasResource _manifestContainer;

		private SasResource _queueContainer;

		public byte[] EncryptionKey
		{
			get
			{
				return this._encrpytionKey;
			}
		}

		public AzureContainerInstance(string azureConnectionString, Guid sessionId, byte[] encryptionKey, int batchNo)
		{
			this._encrpytionKey = encryptionKey;
			this._azureConnectionString = azureConnectionString;
			this._factoryId = sessionId;
			this._batchNo = batchNo;
		}

		private ContainerCreateResponse CreateContainer(string containerName, BlobAccessPermissions permissions)
		{
			ContainerCreateResponse containerCreateResponse = this._acm.CreateContainerIfNotExists(this._azureConnectionString, containerName, permissions);
			if (!containerCreateResponse.Success)
			{
				throw new Exception(containerCreateResponse.Details);
			}
			return containerCreateResponse;
		}

		private CreateQueueResponse CreateQueue(string queueName, QueueAccessPermissions permissions)
		{
			CreateQueueResponse createQueueResponse = this._acm.CreateQueueIfNotExists(this._azureConnectionString, queueName, permissions);
			if (!createQueueResponse.Success)
			{
				throw new Exception(createQueueResponse.Details);
			}
			return createQueueResponse;
		}

		public DeleteContainerReponse DeleteBlobContainer()
		{
			Guid guid = this._factoryId;
			string str = string.Format("{0}-blobs", guid.ToString("D"));
			return this._acm.DeleteContainer(this._azureConnectionString, str);
		}

		public DeleteContainerReponse DeleteManifestContainer()
		{
			string str = this._factoryId.ToString("D");
			int num = this._batchNo;
			string str1 = string.Format("{0}-batch{1}", str, num.ToString("00"));
			return this._acm.DeleteContainer(this._azureConnectionString, str1);
		}

		public DeleteQueueReponse DeleteReportingQueue()
		{
			DeleteQueueReponse deleteQueueReponse = new DeleteQueueReponse()
			{
				Success = false,
				Skipped = true,
				Details = string.Format("'{0}' cannot be deleted due to insufficient permissions on SAS resource.", this._queueContainer.FullAccessUri)
			};
			return deleteQueueReponse;
		}

		public SasResource GetBlobContainer()
		{
			if (this._blobContainer != null)
			{
				return this._blobContainer;
			}
			Guid guid = this._factoryId;
			string str = string.Concat(guid.ToString("D"), "-blobs");
			ContainerCreateResponse containerCreateResponse = this.CreateContainer(str, BlobAccessPermissions.Read | BlobAccessPermissions.Write | BlobAccessPermissions.Delete | BlobAccessPermissions.List);
			ContainerCreateResponse containerCreateResponse1 = this.CreateContainer(str, BlobAccessPermissions.Read | BlobAccessPermissions.List);
			this._blobContainer = new SasResource(string.Concat(containerCreateResponse.PrimaryUri, containerCreateResponse.SharedAccessSignature), string.Concat(containerCreateResponse1.PrimaryUri, containerCreateResponse1.SharedAccessSignature));
			return this._blobContainer;
		}

		public SasResource GetManifestContainer()
		{
			if (this._manifestContainer != null)
			{
				return this._manifestContainer;
			}
			string str = this._factoryId.ToString("D");
			int num = this._batchNo;
			string str1 = string.Concat(str, "-batch", num.ToString("00"));
			ContainerCreateResponse containerCreateResponse = this.CreateContainer(str1, BlobAccessPermissions.Read | BlobAccessPermissions.Write | BlobAccessPermissions.Delete | BlobAccessPermissions.List);
			ContainerCreateResponse containerCreateResponse1 = this.CreateContainer(str1, BlobAccessPermissions.Read | BlobAccessPermissions.Write | BlobAccessPermissions.List);
			if (!containerCreateResponse1.Success)
			{
				throw new Exception(containerCreateResponse1.Details);
			}
			this._manifestContainer = new SasResource(string.Concat(containerCreateResponse.PrimaryUri, containerCreateResponse.SharedAccessSignature), string.Concat(containerCreateResponse1.PrimaryUri, containerCreateResponse1.SharedAccessSignature));
			return this._manifestContainer;
		}

		public SasResource GetReportingQueue()
		{
			if (this._queueContainer != null)
			{
				return this._queueContainer;
			}
			string str = this._factoryId.ToString("D");
			int num = this._batchNo;
			string str1 = string.Concat(str, "-batch", num.ToString("00"));
			CreateQueueResponse createQueueResponse = this.CreateQueue(str1, QueueAccessPermissions.Read | QueueAccessPermissions.Add | QueueAccessPermissions.ProcessMessages);
			if (!createQueueResponse.Success)
			{
				throw new Exception(createQueueResponse.Details);
			}
			this._queueContainer = new SasResource(string.Concat(createQueueResponse.PrimaryUri, createQueueResponse.SharedAccessSignature), string.Concat(createQueueResponse.PrimaryUri, createQueueResponse.SharedAccessSignature));
			return this._queueContainer;
		}
	}
}