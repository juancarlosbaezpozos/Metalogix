using Metalogix.Azure;
using Metalogix.Core.OperationLog;
using Metalogix.SharePoint.Adapters;
using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline
{
	public class SharePointContainerInstance : IAzureContainerInstance
	{
		private readonly object _lock = new object();

		private readonly IMigrationPipeline _pipeline;

		private string _blobContainer;

		private bool _hasProvisioned;

		private string _manifestContainer;

		private string _reportingQueue;

		public byte[] EncryptionKey
		{
			get
			{
				return JustDecompileGenerated_get_EncryptionKey();
			}
			set
			{
				JustDecompileGenerated_set_EncryptionKey(value);
			}
		}

		private byte[] JustDecompileGenerated_EncryptionKey_k__BackingField;

		public byte[] JustDecompileGenerated_get_EncryptionKey()
		{
			return this.JustDecompileGenerated_EncryptionKey_k__BackingField;
		}

		private void JustDecompileGenerated_set_EncryptionKey(byte[] value)
		{
			this.JustDecompileGenerated_EncryptionKey_k__BackingField = value;
		}

		public SharePointContainerInstance(IMigrationPipeline pipeline)
		{
			this._pipeline = pipeline;
		}

		public DeleteContainerReponse DeleteBlobContainer()
		{
			return new DeleteContainerReponse()
			{
				Success = false,
				Details = "The blob migration container cannot be deleted."
			};
		}

		public DeleteContainerReponse DeleteManifestContainer()
		{
			return new DeleteContainerReponse()
			{
				Success = false,
				Details = "This manifest migration container cannot be deleted."
			};
		}

		public DeleteQueueReponse DeleteReportingQueue()
		{
			DeleteQueueReponse deleteQueueReponse = new DeleteQueueReponse()
			{
				Success = false,
				Skipped = true,
				Details = "This migration queue cannot be deleted."
			};
			return deleteQueueReponse;
		}

		public SasResource GetBlobContainer()
		{
			this.ProvisionMigrationContainers();
			return new SasResource(this._blobContainer, this._blobContainer);
		}

		public SasResource GetManifestContainer()
		{
			this.ProvisionMigrationContainers();
			return new SasResource(this._manifestContainer, this._manifestContainer);
		}

		public SasResource GetReportingQueue()
		{
			this.ProvisionMigrationContainers();
			return new SasResource(this._reportingQueue, this._reportingQueue);
		}

		private void ProvisionDataContainers()
		{
			OperationReportingResult operationReportingResult = new OperationReportingResult(this._pipeline.ProvisionMigrationContainer());
			if (operationReportingResult.ErrorOccured)
			{
				throw new FailedToProvisionContainerException(operationReportingResult.AllInformationAsString, operationReportingResult.GetAllErrorsAsString);
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(operationReportingResult.ObjectXml);
			this._blobContainer = xmlDocument.DocumentElement.SelectSingleNode("//MigrationContainer/@DataContainerUri").Value;
			this._manifestContainer = xmlDocument.DocumentElement.SelectSingleNode("//MigrationContainer/@MetadataContainerUri").Value;
			this.EncryptionKey = Convert.FromBase64String(xmlDocument.DocumentElement.SelectSingleNode("//MigrationContainer/@EncryptionKey").Value);
		}

		private void ProvisionMigrationContainers()
		{
			lock (this._lock)
			{
				if (!this._hasProvisioned)
				{
					this.ProvisionDataContainers();
					this.ProvisionReportingQueue();
					this._hasProvisioned = true;
				}
			}
		}

		private void ProvisionReportingQueue()
		{
			OperationReportingResult operationReportingResult = new OperationReportingResult(this._pipeline.ProvisionMigrationQueue());
			if (operationReportingResult.ErrorOccured)
			{
				throw new FailedToProvisionQueueException(operationReportingResult.AllInformationAsString, operationReportingResult.GetAllErrorsAsString);
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(operationReportingResult.ObjectXml);
			this._reportingQueue = xmlDocument.DocumentElement.SelectSingleNode("//MigrationQueue/@JobQueueUri").Value;
		}
	}
}