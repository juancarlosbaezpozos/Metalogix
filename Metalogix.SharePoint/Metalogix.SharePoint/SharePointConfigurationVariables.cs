using Metalogix;
using Metalogix.Core;
using Metalogix.Interfaces;
using System;
using System.IO;
using System.Reflection;

namespace Metalogix.SharePoint
{
	public class SharePointConfigurationVariables : ConfigurationVariables
	{
		private const string AllowDBWritingKey = "AllowDBWriting";

		private const string ReferencedPrincipalsMaximumIDQueryKey = "ReferencedPrincipalsMaximumIDQuery";

		private const string IncludeFilteringColumnsInTerseDataKey = "IncludeFilteringColumnsInTerseData";

		private const string LastUsedProxyServerKey = "LastProxyServer";

		private const string LastUsedProxyPortKey = "LastProxyPort";

		private const string TenantPersonalSiteCreationWaitIntervalKey = "TenantPersonalSiteCreationWaitInterval";

		private const string TenantPersonalSiteCreationRetryCountKey = "TenantPersonalSiteCreationRetryCount";

		private const string ShowNWSInformationKey = "ShowNWSInformationDialog";

		private const string ShowReadOnlyConnectionInformationKey = "ShowReadOnlyConnectionInformation";

		private const string ShowManualLoginInformationKey = "ShowManualLoginInformationDialog";

		private const string SiteCollectionAutoloadThresholdKey = "SiteCollectionAutoloadThreshold";

		private const string SiteCollectionAutoloadAllKey = "SiteCollectionAutoloadAll";

		private const string ShowFilterOptionInformationDialogKey = "ShowFilterOptionInformationDialog";

		private const string AllowCheckResultsKey = "AllowCheckResults";

		private const string EnableAdapterLoggingKey = "EnableAdapterLogging";

		private const string CheckModifiedDatesForListsKey = "CheckModifiedDatesForLists";

		private const string RemoveMappedColumnsKey = "RemoveMappedColumns";

		private const string ShowPubInfrastructureWarningKey = "ShowPubInfrastructureWarning";

		private const string UploadManagerAzureStorageConnectionStringKey = "UploadManagerAzureStorageConnectionString";

		private const string UploadManagerLocalTemporaryStorageLocationKey = "UploadManagerLocalTemporaryStorageLocation";

		private const string UploadManagerMaxBatchesToUploadKey = "UploadManagerMaxBatchesToUpload";

		private const string UploadManagerBatchSizeThresholdKey = "UploadManagerBatchSizeThreshold";

		private const string UploadManagerBatchSizeThresholdInMBKey = "UploadManagerBatchSizeThresholdInMB";

		private const string UploadManagerDetermineBatchSizesByMBKey = "UploadManagerDetermineBatchSizesByMB";

		private const string UploadManagerMaxRetryCountThresholdForJobResubmissionKey = "UploadManagerMaxRetryCountThresholdForJobResubmission";

		private const string InternalTestingModeKey = "InternalTestingMode";

		private const string ResolvePrincipalsMethodKey = "ResolvePrincipalsMethod";

		private const string BlobStorageEncryptionKeyFileKey = "BlobsStorageEncryptionKeyFile";

		private const string CleanAzureContainerKey = "CleanAzureContainer";

		private const string FileChunkSizeInMbKey = "FileChunkSizeInMB";

		private const string NintexWorkflowsTempStorageKey = "NintexWorkflowsTempStorage";

		private const string CleanupNintexWorkflowsTempStorageKey = "CleanupNintexWorkflowsTempStorage";

		private const string NintexEndpointUrlKey = "NintexEndpointUrl";

		public static bool AllowCheckResults
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("AllowCheckResults");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("AllowCheckResults", value);
			}
		}

		public static bool AllowDBWriting
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("AllowDBWriting");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("AllowDBWriting", value);
			}
		}

		public static string AppLogFilePath
		{
			get
			{
				return Path.Combine(ApplicationData.ApplicationPath, "Logs");
			}
		}

		public static string BlobStorageEncryptionKeyFile
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("BlobsStorageEncryptionKeyFile");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("BlobsStorageEncryptionKeyFile", value);
			}
		}

		public static bool CheckModifiedDatesForLists
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("CheckModifiedDatesForLists");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("CheckModifiedDatesForLists", value);
			}
		}

		public static bool CleanAzureContainer
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("CleanAzureContainer");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("CleanAzureContainer", value);
			}
		}

		public static bool CleanupNintexWorkflowsTempStorage
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("CleanupNintexWorkflowsTempStorage");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("CleanupNintexWorkflowsTempStorage", value);
			}
		}

		public static bool EnableAdapterLogging
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("EnableAdapterLogging");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("EnableAdapterLogging", value);
			}
		}

		public static int FileChunkSizeInMB
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("FileChunkSizeInMB");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("FileChunkSizeInMB", value);
			}
		}

		public static bool IncludeFilteringColumnsInTerseData
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("IncludeFilteringColumnsInTerseData");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("IncludeFilteringColumnsInTerseData", value);
			}
		}

		public static string InternalTestingMode
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("InternalTestingMode");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("InternalTestingMode", value);
			}
		}

		public static string LastUsedProxyPort
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("LastProxyPort");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("LastProxyPort", value);
			}
		}

		public static string LastUsedProxyServer
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("LastProxyServer");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("LastProxyServer", value);
			}
		}

		public static string NintexEndpointUrl
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("NintexEndpointUrl");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("NintexEndpointUrl", value);
			}
		}

		public static string NintexWorkflowsTempStorage
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("NintexWorkflowsTempStorage");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("NintexWorkflowsTempStorage", value);
			}
		}

		public static int ReferencedPrincipalsMaximumIDQuery
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("ReferencedPrincipalsMaximumIDQuery");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("ReferencedPrincipalsMaximumIDQuery", value);
			}
		}

		public static bool RemoveMappedColumns
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("RemoveMappedColumns");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("RemoveMappedColumns", value);
			}
		}

		public static string ResolvePrincipalsMethod
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("ResolvePrincipalsMethod");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("ResolvePrincipalsMethod", value);
			}
		}

		public static IConfigurationVariable ShowFilterOptionInformationDialog
		{
			get
			{
				return ConfigurationVariables.GetConfigurationVariable("ShowFilterOptionInformationDialog");
			}
		}

		public static IConfigurationVariable ShowManualLoginInformationDialog
		{
			get
			{
				return ConfigurationVariables.GetConfigurationVariable("ShowManualLoginInformationDialog");
			}
		}

		public static IConfigurationVariable ShowNWSInformationDialog
		{
			get
			{
				return ConfigurationVariables.GetConfigurationVariable("ShowNWSInformationDialog");
			}
		}

		public static IConfigurationVariable ShowPubInfrastructureWarning
		{
			get
			{
				return ConfigurationVariables.GetConfigurationVariable("ShowPubInfrastructureWarning");
			}
		}

		public static IConfigurationVariable ShowReadOnlyConnectionInformation
		{
			get
			{
				return ConfigurationVariables.GetConfigurationVariable("ShowReadOnlyConnectionInformation");
			}
		}

		public static bool SiteCollectionAutoloadAll
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("SiteCollectionAutoloadAll");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("SiteCollectionAutoloadAll", value);
			}
		}

		public static int SiteCollectionAutoloadThreshold
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("SiteCollectionAutoloadThreshold");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("SiteCollectionAutoloadThreshold", value);
			}
		}

		public static int TenantPersonalSiteCreationRetryCount
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("TenantPersonalSiteCreationRetryCount");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("TenantPersonalSiteCreationRetryCount", value);
			}
		}

		public static int TenantPersonalSiteCreationWaitInterval
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("TenantPersonalSiteCreationWaitInterval");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("TenantPersonalSiteCreationWaitInterval", value);
			}
		}

		public static string UploadManagerAzureStorageConnectionString
		{
			get
			{
				return SharePointConfigurationVariables.GetConnectionStringForAzure();
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("UploadManagerAzureStorageConnectionString", value);
			}
		}

		public static int UploadManagerBatchSizeThreshold
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("UploadManagerBatchSizeThreshold");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("UploadManagerBatchSizeThreshold", value);
			}
		}

		public static int UploadManagerBatchSizeThresholdInMB
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("UploadManagerBatchSizeThresholdInMB");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("UploadManagerBatchSizeThresholdInMB", value);
			}
		}

		public static bool UploadManagerDetermineBatchSizesByMB
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<bool>("UploadManagerDetermineBatchSizesByMB");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<bool>("UploadManagerDetermineBatchSizesByMB", value);
			}
		}

		public static string UploadManagerLocalTemporaryStorageLocation
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<string>("UploadManagerLocalTemporaryStorageLocation");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<string>("UploadManagerLocalTemporaryStorageLocation", value);
			}
		}

		public static int UploadManagerMaxBatchesToUpload
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("UploadManagerMaxBatchesToUpload");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("UploadManagerMaxBatchesToUpload", value);
			}
		}

		public static int UploadManagerMaxRetryCountThresholdForJobResubmission
		{
			get
			{
				return ConfigurationVariables.GetConfigurationValue<int>("UploadManagerMaxRetryCountThresholdForJobResubmission");
			}
			set
			{
				ConfigurationVariables.SetConfigurationValue<int>("UploadManagerMaxRetryCountThresholdForJobResubmission", value);
			}
		}

		static SharePointConfigurationVariables()
		{
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment, "AllowDBWriting", false);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "ReferencedPrincipalsMaximumIDQuery", 50);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment, "IncludeFilteringColumnsInTerseData", true);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "LastProxyServer", string.Empty);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "LastProxyPort", string.Empty);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "TenantPersonalSiteCreationWaitInterval", 30000);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "TenantPersonalSiteCreationRetryCount", 10);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.UserSpecific, "ShowNWSInformationDialog", true);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.UserSpecific, "ShowReadOnlyConnectionInformation", true);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.UserSpecific, "ShowManualLoginInformationDialog", true);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.UserSpecific, "SiteCollectionAutoloadThreshold", 10);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.UserSpecific, "SiteCollectionAutoloadAll", false);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "ShowFilterOptionInformationDialog", true);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "AllowCheckResults", false);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "EnableAdapterLogging", false);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "CheckModifiedDatesForLists", true);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "RemoveMappedColumns", true);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "ShowPubInfrastructureWarning", true);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific, "FileChunkSizeInMB", 8);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific, "NintexWorkflowsTempStorage", Path.Combine(Path.GetTempPath(), "NintexWorkflows"));
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, "CleanupNintexWorkflowsTempStorage", true);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific, "NintexEndpointUrl", "https://nintex.onmetalogix.com/nintex/convert");
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "UploadManagerAzureStorageConnectionString", string.Empty);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "UploadManagerLocalTemporaryStorageLocation", string.Empty);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "UploadManagerMaxBatchesToUpload", 2);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "UploadManagerBatchSizeThreshold", 100);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "UploadManagerBatchSizeThresholdInMB", 50);
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment, "UploadManagerDetermineBatchSizesByMB", false);
			ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "UploadManagerMaxRetryCountThresholdForJobResubmission", 960);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "InternalTestingMode", string.Empty);
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "ResolvePrincipalsMethod", "People");
			ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.Environment, "BlobsStorageEncryptionKeyFile", "encryption_key.dat");
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment, "CleanAzureContainer", true);
			SharePointConfigurationVariables.EnsureNonUISharePointVariables();
			ConfigurationVariables.ConfigurationVariablesChanged += new ConfigurationVariables.ConfigurationVariablesChangedHander(SharePointConfigurationVariables.ConfigurationVariables_ConfigurationVariablesChanged);
		}

		public SharePointConfigurationVariables()
		{
		}

		private static void ConfigurationVariables_ConfigurationVariablesChanged(object sender, ConfigurationVariables.ConfigVarsChangedArgs e)
		{
			bool flag;
			if (string.IsNullOrEmpty(e.VariableName))
			{
				FieldInfo[] fields = typeof(SharePointConfigurationVariables).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
				for (int i = 0; i < (int)fields.Length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					if (fieldInfo.IsLiteral || fieldInfo.FieldType.IsValueType)
					{
						flag = (!fieldInfo.FieldType.IsGenericType ? true : fieldInfo.FieldType != typeof(Nullable<>).MakeGenericType(fieldInfo.FieldType.GetGenericArguments()));
					}
					else
					{
						flag = false;
					}
					if (!flag)
					{
						fieldInfo.SetValue(null, null);
					}
				}
			}
		}

		public static void EnsureNonUISharePointVariables()
		{
			bool allowDBWriting = SharePointConfigurationVariables.AllowDBWriting;
			bool allowCheckResults = SharePointConfigurationVariables.AllowCheckResults;
			bool siteCollectionAutoloadAll = SharePointConfigurationVariables.SiteCollectionAutoloadAll;
			bool includeFilteringColumnsInTerseData = SharePointConfigurationVariables.IncludeFilteringColumnsInTerseData;
			bool checkModifiedDatesForLists = SharePointConfigurationVariables.CheckModifiedDatesForLists;
			bool removeMappedColumns = SharePointConfigurationVariables.RemoveMappedColumns;
			int siteCollectionAutoloadThreshold = SharePointConfigurationVariables.SiteCollectionAutoloadThreshold;
			int tenantPersonalSiteCreationRetryCount = SharePointConfigurationVariables.TenantPersonalSiteCreationRetryCount;
			int tenantPersonalSiteCreationWaitInterval = SharePointConfigurationVariables.TenantPersonalSiteCreationWaitInterval;
			string uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.UploadManagerAzureStorageConnectionString;
			uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.UploadManagerLocalTemporaryStorageLocation;
			int uploadManagerBatchSizeThreshold = SharePointConfigurationVariables.UploadManagerBatchSizeThreshold;
			uploadManagerBatchSizeThreshold = SharePointConfigurationVariables.UploadManagerMaxBatchesToUpload;
			uploadManagerBatchSizeThreshold = SharePointConfigurationVariables.UploadManagerBatchSizeThresholdInMB;
			removeMappedColumns = SharePointConfigurationVariables.UploadManagerDetermineBatchSizesByMB;
			uploadManagerBatchSizeThreshold = SharePointConfigurationVariables.UploadManagerMaxRetryCountThresholdForJobResubmission;
			uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.InternalTestingMode;
			uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.ResolvePrincipalsMethod;
			uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.BlobStorageEncryptionKeyFile;
			removeMappedColumns = SharePointConfigurationVariables.CleanAzureContainer;
			tenantPersonalSiteCreationWaitInterval = SharePointConfigurationVariables.FileChunkSizeInMB;
			uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.NintexWorkflowsTempStorage;
			allowDBWriting = SharePointConfigurationVariables.CleanupNintexWorkflowsTempStorage;
			uploadManagerAzureStorageConnectionString = SharePointConfigurationVariables.NintexEndpointUrl;
		}

		public static string GetConnectionStringForAzure()
		{
			string configurationValue = ConfigurationVariables.GetConfigurationValue<string>("UploadManagerAzureStorageConnectionString");
			try
			{
				char[] chrArray = new char[] { ';' };
				string[] strArrays = configurationValue.Split(chrArray);
				string[] strArrays1 = strArrays;
				int num = 0;
				while (num < (int)strArrays1.Length)
				{
					string str = strArrays1[num];
					if (!str.StartsWith("AccountName=", StringComparison.InvariantCultureIgnoreCase))
					{
						num++;
					}
					else
					{
						chrArray = new char[] { '=' };
						string[] lowerInvariant = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
						if ((int)lowerInvariant.Length > 1)
						{
							lowerInvariant[1] = lowerInvariant[1].ToLowerInvariant();
						}
						strArrays[1] = string.Join("=", lowerInvariant);
						break;
					}
				}
				configurationValue = string.Join(";", strArrays);
			}
			catch (Exception exception)
			{
			}
			return configurationValue;
		}
	}
}