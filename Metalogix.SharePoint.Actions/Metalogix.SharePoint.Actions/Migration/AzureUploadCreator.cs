using Metalogix;
using Metalogix.Actions;
using Metalogix.Azure;
using Metalogix.Azure.Blob.Manager;
using Metalogix.Office365;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration.Pipeline;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	public static class AzureUploadCreator
	{
		private static void AddMetadataFieldToAzure(IUploadManager uploadManager, SPField metadataField)
		{
			Field field = new Field()
			{
				Access = "ReadWrite",
				FieldId = metadataField.ID.ToString(),
				Name = metadataField.Name,
				Type = metadataField.Type,
				IsReadOnly = metadataField.IsReadOnly,
				Value = string.Empty
			};
			uploadManager.FieldNames.Add(field);
		}

		public static void AddMetadataFieldToAzure(IUploadManager uploadManager, SPField metadataField, SPList targetList)
		{
			AzureUploadCreator.AddMetadataFieldToAzure(uploadManager, metadataField);
			if (metadataField.IsTaxonomyField)
			{
				Guid taxonomyHiddenTextField = metadataField.TaxonomyHiddenTextField;
				if (taxonomyHiddenTextField != Guid.Empty)
				{
					SPField fieldById = targetList.FieldCollection.GetFieldById(taxonomyHiddenTextField);
					if (fieldById != null)
					{
						AzureUploadCreator.AddMetadataFieldToAzure(uploadManager, fieldById);
					}
				}
			}
		}

		public static void EndAzurePipeline(IUploadManager uploadManager, LogItem opCompletedItem)
		{
			uploadManager.EndProcessing();
			uploadManager.WaitForAllToComplete();
			opCompletedItem.Information = "See details for operation flow";
			opCompletedItem.Status = ActionOperationStatus.Completed;
			opCompletedItem.Details = uploadManager.GetStatusLog();
		}

		public static IUploadManager InitializeAzurePipeline(SPFolder targetFolder, IOperationLoggingManagement operationLoggingManagement, IOperationState operationState, bool encryptAzureMigrationJobs, bool isAlertsMigration = false)
		{
			IAzureContainerFactory sharePointContainerFactory;
			List<Field> fields = new List<Field>();
			foreach (SPField fieldCollection in targetFolder.ParentList.FieldCollection)
			{
				if (!Utils.IsWritableColumnForManifest(fieldCollection.Name, fieldCollection.IsReadOnly, fieldCollection.Type, (int)targetFolder.ParentList.BaseTemplate, false, (fieldCollection.FieldXML.Attributes == null ? false : fieldCollection.FieldXML.Attributes["BdcField"] != null), fieldCollection.ID))
				{
					continue;
				}
				Field field = new Field()
				{
					Access = "ReadWrite",
					FieldId = fieldCollection.ID.ToString(),
					Name = fieldCollection.Name,
					Type = fieldCollection.Type,
					IsReadOnly = fieldCollection.IsReadOnly,
					Value = string.Empty
				};
				fields.Add(field);
			}
			IMigrationPipeline writer = targetFolder.Adapter.Writer as IMigrationPipeline;
			if (writer == null)
			{
				Exception exception = new Exception("migrationPipeline is null - target does not support Azure migration approach");
				if (targetFolder.Adapter != null)
				{
					exception.Data.Add("targetFolder.Adapter AdapterShortName", targetFolder.Adapter.AdapterShortName);
					exception.Data.Add("targetFolder.Adapter Writer Type", targetFolder.Adapter.Writer.GetType().FullName);
				}
				throw exception;
			}
			IUploadManager azureUploadManager = new AzureUploadManager(encryptAzureMigrationJobs);
			string uploadManagerLocalTemporaryStorageLocation = SharePointConfigurationVariables.UploadManagerLocalTemporaryStorageLocation;
			if (string.IsNullOrEmpty(uploadManagerLocalTemporaryStorageLocation))
			{
				uploadManagerLocalTemporaryStorageLocation = ApplicationData.CommonDataPath;
			}
			Guid guid = Guid.NewGuid();
			string str = Path.Combine(uploadManagerLocalTemporaryStorageLocation, guid.ToString("N"));
			Guid guid1 = new Guid(targetFolder.ParentList.ParentWeb.ID);
			Guid guid2 = new Guid(targetFolder.ParentList.ParentWeb.RootSiteGUID);
			Guid guid3 = new Guid(targetFolder.ParentList.ID);
			Guid guid4 = Guid.NewGuid();
			Guid guid5 = Guid.NewGuid();
			Guid guid6 = Guid.NewGuid();
			string uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.UploadManagerAzureStorageConnectionString;
			if (!string.IsNullOrEmpty(uploadManagerAzureStorageConnectionString))
			{
				AzureContainerFactory azureContainerFactory = new AzureContainerFactory(uploadManagerAzureStorageConnectionString, guid);
				azureContainerFactory.SetEncryptionKeyFromFile(Path.Combine(ApplicationData.CommonApplicationDataPath, SharePointConfigurationVariables.BlobStorageEncryptionKeyFile));
				sharePointContainerFactory = azureContainerFactory;
			}
			else
			{
				if (!encryptAzureMigrationJobs)
				{
					throw new Exception("UploadManagerAzureStorageConnectionString has not been defined. Please ensure this contains a valid storage account key in EnvironmentSettings.xml");
				}
				sharePointContainerFactory = new SharePointContainerFactory(targetFolder.Adapter as IMigrationPipeline);
			}
			IAzureBlobStorageManager azureBlobStorageManager = new AzureBlobStorageManager();
			BatchSizeMode batchSizeMode = (SharePointConfigurationVariables.UploadManagerDetermineBatchSizesByMB ? BatchSizeMode.InMegaBytes : BatchSizeMode.NumberOfItems);
			if (isAlertsMigration)
			{
				batchSizeMode = BatchSizeMode.InMegaBytes;
			}
			string str1 = targetFolder.ParentList.DirName.TrimStart(new char[] { '/' });
			string serverRelativeUrl = targetFolder.ParentList.ParentWeb.ServerRelativeUrl;
			char[] chrArray = new char[] { '/' };
			string str2 = str1.Substring(serverRelativeUrl.TrimStart(chrArray).Length).TrimStart(new char[] { '/' });
			azureUploadManager.Initialise(guid, SharePointConfigurationVariables.UploadManagerMaxBatchesToUpload, SharePointConfigurationVariables.UploadManagerBatchSizeThreshold, SharePointConfigurationVariables.UploadManagerBatchSizeThresholdInMB, batchSizeMode, azureBlobStorageManager, sharePointContainerFactory, SharePointConfigurationVariables.UploadManagerMaxRetryCountThresholdForJobResubmission, SharePointConfigurationVariables.InternalTestingMode, str, guid1, guid2, guid3, guid4, guid5, guid6, targetFolder.ParentList.ParentWeb.Url, targetFolder.ParentList.ParentWeb.ServerRelativeUrl, str2, targetFolder.ParentList.Name, targetFolder.ParentList.Title, writer, operationLoggingManagement, operationState, fields, targetFolder.Created, targetFolder.Modified, targetFolder.ParentList.BaseTemplate.ToString(), targetFolder.ParentList.BaseType.ToString(), targetFolder.ParentList.ParentWeb.Template.Name);
			azureUploadManager.StartProcessing();
			return azureUploadManager;
		}
	}
}