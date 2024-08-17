using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Azure;
using Metalogix.Azure.Blob.Manager;
using Metalogix.Explorer;
using Metalogix.Office365;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration.Permissions;
using Metalogix.SharePoint.Actions.Migration.Pipeline;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[BasicModeViewAllowed(true)]
	[CmdletEnabled(true, "Copy-MLAllListItems", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.PasteAllItems.ico")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste All List Items")]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPFolder), true)]
	[SubActionTypes(typeof(PasteListItemAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPFolder), true)]
	public class PasteAllListItemsAction : PasteAction<PasteFolderOptions>
	{
		private static TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection> s_listTransformerDefinition;

		public new string ValidationSettingsXml
		{
			get;
			set;
		}

		static PasteAllListItemsAction()
		{
			PasteAllListItemsAction.s_listTransformerDefinition = new TransformerDefinition<SPList, PasteListAction, SPListCollection, SPListCollection>("SharePoint Lists", true);
		}

		public PasteAllListItemsAction()
		{
		}

		private static void AddOffice365FolderDependancies(SPFolder targetFolder, IUploadManager uploadManager)
		{
			if (targetFolder.ParentFolder == null)
			{
				return;
			}
			for (SPFolder i = targetFolder; i.ParentFolder != null; i = i.ParentFolder)
			{
				string value = i.FolderXML.Attributes["UniqueId"].Value;
				Guid guid = new Guid(value);
				ManifestFolderItem manifestFolderItem = new ManifestFolderItem(false)
				{
					Foldername = i.Name,
					TargetParentPath = i.ParentFolder.WebRelativeUrl,
					ItemGuid = guid,
					IsReferenceOnly = true
				};
				uploadManager.AddFolderToManifest(manifestFolderItem);
			}
		}

		private void CopyListPermissionsToAzureManifest(SPList sourceList, SPList targetList, IUploadManager uploadManager, BaseManifestItem baseManifestItem)
		{
			LogItem logItem = new LogItem("Copying list permissions", sourceList.Name, sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
			base.FireOperationStarted(logItem);
			try
			{
				CopyRoleAssignmentsAction copyRoleAssignmentsAction = new CopyRoleAssignmentsAction();
				copyRoleAssignmentsAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
				base.SubActions.Add(copyRoleAssignmentsAction);
				object[] objArray = new object[] { sourceList, targetList, false, uploadManager, baseManifestItem };
				copyRoleAssignmentsAction.RunAsSubAction(objArray, new ActionContext(sourceList.ParentWeb, targetList.ParentWeb), null);
				logItem.Status = ActionOperationStatus.Completed;
			}
			catch (Exception exception)
			{
				logItem.Exception = exception;
			}
			base.FireOperationFinished(logItem);
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(PasteAllListItemsAction.s_listTransformerDefinition);
			return supportedDefinitions;
		}

		private void PasteAllItems(IXMLAbleList source, SPFolder targetFolder)
		{
			PasteListItemAction pasteListItemAction = new PasteListItemAction();
			pasteListItemAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
			base.SubActions.Add(pasteListItemAction);
			using (targetFolder)
			{
				foreach (SPFolder sPFolder in source)
				{
					try
					{
						LogItem logItem = new LogItem("Fetching source items", "", "", "", ActionOperationStatus.Running)
						{
							WriteToJobDatabase = false
						};
						base.FireOperationStarted(logItem);
						SPListItemCollection terseItemData = PasteActionUtils.GetTerseItemData(sPFolder, base.SharePointOptions);
						object[] objArray = new object[] { terseItemData, targetFolder, null, false };
						pasteListItemAction.RunAsSubAction(objArray, new ActionContext(sPFolder.ParentList.ParentWeb, targetFolder.ParentList.ParentWeb), null);
						if (pasteListItemAction.WebPartPagesNotCopiedAtItemsLevel.Count > 0)
						{
							foreach (SPListItem key in pasteListItemAction.WebPartPagesNotCopiedAtItemsLevel.Keys)
							{
								base.WebPartPagesNotCopiedAtItemsLevel.Add(key, pasteListItemAction.WebPartPagesNotCopiedAtItemsLevel[key]);
							}
							pasteListItemAction.WebPartPagesNotCopiedAtItemsLevel.Clear();
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						LogItem logItem1 = new LogItem("Fetching List Items", sPFolder.Name, sPFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Failed)
						{
							Exception = exception
						};
						base.FireOperationStarted(logItem1);
						base.FireOperationFinished(logItem1);
					}
					sPFolder.Dispose();
				}
			}
		}

		private void PasteAllItemsOffice365(IXMLAbleList source, SPFolder targetFolder, bool isCopyListPermissionAllowed = false)
		{
			IAzureContainerFactory sharePointContainerFactory;
			PasteListItemAction pasteListItemAction = new PasteListItemAction();
			pasteListItemAction.SharePointOptions.SetFromOptions(base.SharePointOptions);
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
			if (SharePointConfigurationVariables.ResolvePrincipalsMethod.ToEnumValue<ResolvePrincipalsMethod>(ResolvePrincipalsMethod.People) == ResolvePrincipalsMethod.Graph && targetFolder.Adapter.AzureAdGraphCredentials == null)
			{
				throw new Exception(Resources.AzureAdGraphCredentialsNotFoundMessage);
			}
			targetFolder.ParentList.IsUsingMigrationPipeline = true;
			base.SubActions.Add(pasteListItemAction);
			using (targetFolder)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<Metalogix.Office365.Field> fields = new List<Metalogix.Office365.Field>();
				List<string> strs = new List<string>();
				bool flag = (!pasteListItemAction.SharePointOptions.PreserveSharePointDocumentIDs ? false : targetFolder.ParentList.ParentWeb.HasSharePointDocumentIDFeature);
				foreach (SPField fieldCollection in targetFolder.ParentList.FieldCollection)
				{
					if (!Utils.IsWritableColumnForManifest(fieldCollection.Name, fieldCollection.IsReadOnly, fieldCollection.Type, (int)targetFolder.ParentList.BaseTemplate, false, (fieldCollection.FieldXML.Attributes == null ? false : fieldCollection.FieldXML.Attributes["BdcField"] != null), fieldCollection.ID) && (!flag || !fieldCollection.ID.ToString().Equals("ae3e2a36-125d-45d3-9051-744b513536a6", StringComparison.InvariantCultureIgnoreCase) && !fieldCollection.ID.ToString().Equals("3b63724f-3418-461f-868b-7706f69b029c", StringComparison.InvariantCultureIgnoreCase)))
					{
						continue;
					}
					object[] name = new object[] { fieldCollection.Name, fieldCollection.ID, fieldCollection.Type, fieldCollection.Hidden };
					stringBuilder.AppendLine(string.Format("{0} ID={1} Type={2} Hidden={3}", name));
					Metalogix.Office365.Field field1 = new Metalogix.Office365.Field()
					{
						Access = "ReadWrite",
						FieldId = fieldCollection.ID.ToString(),
						Name = fieldCollection.Name,
						Type = fieldCollection.Type,
						IsReadOnly = fieldCollection.IsReadOnly,
						Value = string.Empty
					};
					Metalogix.Office365.Field str = field1;
					string type = fieldCollection.Type;
					string[] strArrays = new string[] { "TaxonomyFieldType", "TaxonomyFieldTypeMulti" };
					if (type.In<string>(strArrays))
					{
						SPField fieldById = targetFolder.ParentList.FieldCollection.GetFieldById(fieldCollection.TaxonomyHiddenTextField);
						str.TaxonomyHiddenTextFieldId = fieldCollection.TaxonomyHiddenTextField.ToString();
						str.TaxonomyHiddenTextFieldName = fieldById.Name;
						strs.Add(str.TaxonomyHiddenTextFieldId);
					}
					fields.Add(str);
				}
				fields.RemoveAll((Metalogix.Office365.Field field) => strs.Contains(field.FieldId));
				IUploadManager azureUploadManager = new AzureUploadManager(base.SharePointOptions.EncryptAzureMigrationJobs);
				string uploadManagerLocalTemporaryStorageLocation = SharePointConfigurationVariables.UploadManagerLocalTemporaryStorageLocation;
				if (string.IsNullOrEmpty(uploadManagerLocalTemporaryStorageLocation))
				{
					uploadManagerLocalTemporaryStorageLocation = ApplicationData.CommonDataPath;
				}
				Guid guid = Guid.NewGuid();
				string str1 = Path.Combine(uploadManagerLocalTemporaryStorageLocation, guid.ToString("N"));
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
					if (!base.SharePointOptions.EncryptAzureMigrationJobs)
					{
						throw new Exception("UploadManagerAzureStorageConnectionString has not been defined. Please ensure this contains a valid storage account key in EnvironmentSettings.xml or Configuration Database");
					}
					sharePointContainerFactory = new SharePointContainerFactory(targetFolder.Adapter as IMigrationPipeline);
				}
				IAzureBlobStorageManager azureBlobStorageManager = new AzureBlobStorageManager();
				BatchSizeMode batchSizeMode = (SharePointConfigurationVariables.UploadManagerDetermineBatchSizesByMB ? BatchSizeMode.InMegaBytes : BatchSizeMode.NumberOfItems);
				string str2 = targetFolder.ParentList.DirName.TrimStart(new char[] { '/' });
				string serverRelativeUrl = targetFolder.ParentList.ParentWeb.ServerRelativeUrl;
				char[] chrArray = new char[] { '/' };
				string str3 = str2.Substring(serverRelativeUrl.TrimStart(chrArray).Length).TrimStart(new char[] { '/' });
				azureUploadManager.Initialise(guid, SharePointConfigurationVariables.UploadManagerMaxBatchesToUpload, SharePointConfigurationVariables.UploadManagerBatchSizeThreshold, SharePointConfigurationVariables.UploadManagerBatchSizeThresholdInMB, batchSizeMode, azureBlobStorageManager, sharePointContainerFactory, SharePointConfigurationVariables.UploadManagerMaxRetryCountThresholdForJobResubmission, SharePointConfigurationVariables.InternalTestingMode, str1, guid1, guid2, guid3, guid4, guid5, guid6, targetFolder.ParentList.ParentWeb.Url, targetFolder.ParentList.ParentWeb.ServerRelativeUrl, str3, targetFolder.ParentList.Name, targetFolder.ParentList.Title, writer, this, this, fields, targetFolder.Created, targetFolder.Modified, targetFolder.ParentList.BaseTemplate.ToString(), targetFolder.ParentList.BaseType.ToString(), targetFolder.ParentList.ParentWeb.Template.Name);
				azureUploadManager.StartProcessing();
				PasteAllListItemsAction.AddOffice365FolderDependancies(targetFolder, azureUploadManager);
				SPList sPList = (source[0] is SPList ? (SPList)source[0] : ((SPFolder)source[0]).ParentList);
				if (isCopyListPermissionAllowed || targetFolder.ParentList.HasUniquePermissions)
				{
					SPList sPList1 = (isCopyListPermissionAllowed ? sPList : targetFolder.ParentList);
					ManifestList manifestList = new ManifestList()
					{
						ItemGuid = new Guid(targetFolder.ParentList.ID),
						ServerRelativeURL = targetFolder.ParentList.ServerRelativeUrl
					};
					this.CopyListPermissionsToAzureManifest(sPList1, targetFolder.ParentList, azureUploadManager, manifestList);
					azureUploadManager.ListManifest = manifestList;
				}
				foreach (SPFolder sPFolder in source)
				{
					try
					{
						LogItem logItem = new LogItem("Fetching source items", "", "", "", ActionOperationStatus.Running)
						{
							WriteToJobDatabase = false
						};
						base.FireOperationStarted(logItem);
						sPFolder.ParentList.IsUsingMigrationPipeline = true;
						SPListItemCollection terseItemData = PasteActionUtils.GetTerseItemData(sPFolder, base.SharePointOptions);
						object[] objArray = new object[] { terseItemData, targetFolder, null, false, azureUploadManager };
						pasteListItemAction.RunAsSubAction(objArray, new ActionContext(sPFolder.ParentList.ParentWeb, targetFolder.ParentList.ParentWeb), null);
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						LogItem logItem1 = new LogItem("Fetching List Items O365", sPFolder.Name, sPFolder.DisplayUrl, targetFolder.DisplayUrl, ActionOperationStatus.Failed)
						{
							Exception = exception1
						};
						base.FireOperationStarted(logItem1);
						base.FireOperationFinished(logItem1);
					}
					sPFolder.Dispose();
				}
				azureUploadManager.EndProcessing();
				azureUploadManager.WaitForAllToComplete();
				pasteListItemAction.CopyDocumentSetsVersionHistory();
				LogItem logItem2 = new LogItem("AzureUploadManager.StatusLog", targetFolder.ParentList.Title, string.Empty, string.Empty, ActionOperationStatus.Running)
				{
					Information = "See details for operation flow",
					Status = ActionOperationStatus.Completed,
					Details = azureUploadManager.GetStatusLog()
				};
				base.FireOperationStarted(logItem2);
				base.FireOperationFinished(logItem2);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPList sPList = (source[0] is SPList ? (SPList)source[0] : ((SPFolder)source[0]).ParentList))
			{
				foreach (SPFolder sPFolder in target)
				{
					try
					{
						Node[] nodeArray = new Node[] { sPFolder };
						this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
						base.RunPreCopyListUpdate(sPList, sPFolder, base.SharePointOptions);
						if (!sPFolder.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload || !sPList.IsMigrationPipelineSupported || !sPFolder.ParentList.IsMigrationPipelineSupportedForTarget)
						{
							this.PasteAllItems(source, sPFolder);
							if (base.SharePointOptions.CopyFolderPermissions || base.SharePointOptions.CopyItemPermissions)
							{
								base.ThreadManager.SetBufferedTasks((new PermissionsBufferedTaskKeyFormatter()).GetKeyFor(sPFolder), false, false);
							}
						}
						else
						{
							this.PasteAllItemsOffice365(source, sPFolder, false);
						}
						sPFolder.UpdateCurrentNode();
					}
					finally
					{
						sPFolder.Dispose();
					}
				}
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			IXMLAbleList nodeCollection;
			SPWeb parentWeb;
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new ArgumentException(string.Format(Resources.ActionOperationMissingParameter, this.Name));
			}
			SPFolder sPFolder = oParams[0] as SPFolder;
			if (sPFolder == null)
			{
				nodeCollection = oParams[0] as IXMLAbleList;
			}
			else
			{
				nodeCollection = new NodeCollection(new Node[] { sPFolder });
			}
			if (nodeCollection == null)
			{
				throw new ArgumentException(string.Format(Resources.ActionOperationMissingParameter, this.Name, oParams[0].GetType(), typeof(SPFolder)), "oParams[0]");
			}
			SPList sPList = (nodeCollection[0] is SPList ? (SPList)nodeCollection[0] : ((SPFolder)nodeCollection[0]).ParentList);
			SPFolder sPFolder1 = oParams[1] as SPFolder;
			base.DisableValidationSettings(sPList, sPFolder1, this.IsValidationSettingDisablingRequired);
			if (!sPFolder1.Adapter.SharePointVersion.IsSharePointOnline || !base.SharePointOptions.UseAzureOffice365Upload || !sPList.IsMigrationPipelineSupported || !sPFolder1.ParentList.IsMigrationPipelineSupportedForTarget)
			{
				this.PasteAllItems(nodeCollection, sPFolder1);
			}
			else
			{
				bool flag = false;
				if ((int)oParams.Length >= 3)
				{
					flag = (bool)oParams[2];
				}
				this.PasteAllItemsOffice365(nodeCollection, sPFolder1, flag);
			}
			if (sPFolder1.ParentList != null)
			{
				parentWeb = sPFolder1.ParentList.ParentWeb;
			}
			else
			{
				parentWeb = null;
			}
			SPWeb sPWeb = parentWeb;
			if (sPWeb != null)
			{
				base.EnableValidationSettings(sPList, sPWeb);
			}
		}
	}
}