using Metalogix;
using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class AdapterConfigurationVariables : ConfigurationVariables
    {
        private const string Swap2003DocMetaInfoColumnsKey = "Swap2003DocMetaInfoColumns";

        private const string WebServiceTimeoutTimeKey = "WebServiceTimeoutTime";

        private const string WebServiceRetriesNumberKey = "WebServiceRetriesNumber";

        private const string WebServiceRetriesDelayKey = "WebServiceRetriesDelay";

        private const string SQLQueryTimeoutTimeKey = "SQLQueryTimeoutTime";

        private const string EnableChunkedTransferKey = "EnableChunkedTransfer";

        private const string ChunkStreamTypeKey = "ChunkStreamType";

        private const string ChunkRetentionTimeKey = "ChunkRetentionTime";

        private const string EnableConcurrentNWSOffice365ConnectionsKey = "EnableConcurrentNWSOffice365Connections";

        private const string AllowIE7WebBrowserAuthenticationKey = "AllowIE7WebBrowserAuthentication";

        private const string CSOMDocumentRetriesNumberKey = "CSOMDocumentRetriesNumber";

        private const string CSOMDocumentRetriesDelayKey = "CSOMDocumentRetriesDelay";

        private const string CSOMMaximumExecuteQueryRetriesKey = "CSOMMaximumExecuteQueryRetries";

        private const string ListViewToXsltViewConversionKey = "ListViewToXsltViewConversion";

        private const string Allow2013DBKey = "Allow2013DB";

        private const string PipeCloseTimeoutKey = "PipeCloseTimeout";

        private const string PipeOpenTimeoutKey = "PipeOpenTimeout";

        private const string PipeReceiveTimeoutKey = "PipeReceiveTimeout";

        private const string PipeSendTimeoutKey = "PipeSendTimeout";

        private const string EnableAccessRequestItemsNotificationsKey = "EnableAccessRequestItemsNotifications";

        private const string NotificationEmailAddressKey = "NotificationEmailAddress";

        private const string AllowDuplicateSiteCollectionKey = "AllowDuplicateSiteCollection";

        private const string MaxRequestChunkSizeKey = "MaxRequestChunkSize";

        private const string MaxRequestRetryCountKey = "MaxRequestRetryCount";

        private const string ConvertNumericFieldsToTextKey = "ConvertNumericFieldsToText";

        private const string MMDTermsBatchSizeKey = "MMDTermsBatchSize";

        private const string ListNamesToIgnoreForAzureKey = "ListNamesToIgnoreForAzure";

        private const string ListNamesToIncludeForAzureKey = "ListNamesToIncludeForAzure";

        private const string ShowSPOLicenseNoticeKey = "ShowSPOLicenseNotice";

        private const string ChunkSizeForUnshreddingKey = "ChunkSizeForUnshredding";

        private const string LoadConfluenceUsersFromUserMappingKey = "LoadConfluenceUsersFromUserMapping";

        private const string UseExistingTargetTermsKey = "UseExistingTargetTerms";

        private const string Show2007OMConnectionKey = "Show2007OMConnection";

        private const string SupportedFileSizeWithoutChunkKey = "SupportedFileSizeWithoutChunk";

        private const string EnableSslForEmailKey = "EnableSslForEmail";

        private const int MS_DEFAULT_PIPE_CLOSE_TIMEOUT_MIN = 1;

        private const int MS_DEFAULT_PIPE_OPEN_TIMEOUT_MIN = 1;

        private const int MS_DEFAULT_PIPE_RECEIVE_TIMEOUT_MIN = 10;

        private const int MS_DEFAULT_PIPE_SEND_TIMEOUT_MIN = 1;

        private const string MigrateLanguageSettingsKey = "MigrateLanguageSettings";

        private const string MigrateLanguageSettingForViewsKey = "MigrateLanguageSettingForViews";

        private const string LanguageSettingsRefreshIntervalKey = "LanguageSettingsRefreshInterval";

        private const string LanguageSettingsMaximumIntervalKey = "LanguageSettingsMaximumInterval";

        private const string MigrateLanguageSettingForNavigationStructureKey =
            "MigrateLanguageSettingForNavigationStructure";

        private static ThreadSafeSerializableTable<string, string> s_cachedConfigurationVariableOverrides;

        private static string _defaultListNamesToIgnoreForAzure;

        private static List<string> _listNamesToIgnoreForAzure;

        private static string _defaultListNamesToIncludeForAzure;

        private static List<string> _listNamesToIncludeForAzure;

        private static Dictionary<string, string> s_ConfiguredWebServiceVersions;

        private static object o_LockWebService;

        public static bool Allow2013DB
        {
            get { return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>("Allow2013DB"); }
            set { ConfigurationVariables.SetConfigurationValue<bool>("Allow2013DB", value); }
        }

        public static bool AllowDuplicateSiteCollection
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "AllowDuplicateSiteCollection");
            }
        }

        public static bool AllowIE7WebBrowserAuthentication
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "AllowIE7WebBrowserAuthentication");
            }
        }

        public static int ChunkRetentionTime
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("ChunkRetentionTime");
            }
        }

        public static int ChunkSizeForUnshredding
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>(
                    "ChunkSizeForUnshredding");
            }
        }

        public static StreamType ChunkStreamType
        {
            get
            {
                return AdapterConfigurationVariables
                    .GetConfigurationVariableWithOverride<StreamType>("ChunkStreamType");
            }
        }

        public static Dictionary<string, string> ConfiguredWebServiceVersions
        {
            get
            {
                Dictionary<string, string> sConfiguredWebServiceVersions;
                string value;
                string str;
                lock (AdapterConfigurationVariables.o_LockWebService)
                {
                    if (AdapterConfigurationVariables.s_ConfiguredWebServiceVersions == null)
                    {
                        Assembly mainAssembly = ApplicationData.MainAssembly;
                        Dictionary<string, string> strs = new Dictionary<string, string>();
                        FileInfo fileInfo = new FileInfo(mainAssembly.Location);
                        string str1 = string.Concat(fileInfo.DirectoryName, "/WebServiceConfiguration.xml");
                        if (File.Exists(str1))
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(str1);
                            foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("WebServiceConfiguration/Server"))
                            {
                                if (xmlNodes.Attributes["Name"] != null)
                                {
                                    value = xmlNodes.Attributes["Name"].Value;
                                }
                                else
                                {
                                    value = null;
                                }

                                string str2 = value;
                                if (xmlNodes.Attributes["WebServiceVersion"] != null)
                                {
                                    str = xmlNodes.Attributes["WebServiceVersion"].Value;
                                }
                                else
                                {
                                    str = null;
                                }

                                string str3 = str;
                                if (str2 == null || str3 == null)
                                {
                                    continue;
                                }

                                string str4 = str2.Trim();
                                char[] chrArray = new char[] { '/' };
                                strs.Add(str4.TrimEnd(chrArray), str3);
                            }
                        }

                        if (strs.Count == 0)
                        {
                            strs.Add("default", mainAssembly.GetName().Version.ToString());
                        }

                        AdapterConfigurationVariables.s_ConfiguredWebServiceVersions = strs;
                    }

                    sConfiguredWebServiceVersions = AdapterConfigurationVariables.s_ConfiguredWebServiceVersions;
                }

                return sConfiguredWebServiceVersions;
            }
        }

        public static string ConvertNumericFieldsToText
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<string>(
                    "ConvertNumericFieldsToText");
            }
        }

        public static int CSOMDocumentRetriesDelay
        {
            get
            {
                int configurationVariableWithOverride =
                    AdapterConfigurationVariables
                        .GetConfigurationVariableWithOverride<int>("CSOMDocumentRetriesDelay") * 1000;
                if (configurationVariableWithOverride < 0)
                {
                    configurationVariableWithOverride = 0;
                }

                return configurationVariableWithOverride;
            }
        }

        public static int CSOMDocumentRetriesNumber
        {
            get
            {
                int configurationVariableWithOverride =
                    AdapterConfigurationVariables
                        .GetConfigurationVariableWithOverride<int>("CSOMDocumentRetriesNumber");
                if (configurationVariableWithOverride < 0)
                {
                    configurationVariableWithOverride = 0;
                }

                return configurationVariableWithOverride;
            }
        }

        public static int CSOMMaximumExecuteQueryRetries
        {
            get
            {
                int configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>(
                        "CSOMMaximumExecuteQueryRetries");
                if (configurationVariableWithOverride < 0)
                {
                    configurationVariableWithOverride = 0;
                }

                return configurationVariableWithOverride;
            }
        }

        public static bool EnableAccessRequestItemsNotifications
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "EnableAccessRequestItemsNotifications");
            }
        }

        public static bool EnableChunkedTransfer
        {
            get
            {
                return AdapterConfigurationVariables
                    .GetConfigurationVariableWithOverride<bool>("EnableChunkedTransfer");
            }
        }

        public static bool EnableConcurrentNWSOffice365Connections
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "EnableConcurrentNWSOffice365Connections");
            }
        }

        public static bool EnableSslForEmail
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>("EnableSslForEmail");
            }
        }

        public static int LanguageSettingsMaximumInterval
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>(
                    "LanguageSettingsMaximumInterval");
            }
        }

        public static int LanguageSettingsRefreshInterval
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>(
                    "LanguageSettingsRefreshInterval");
            }
        }

        public static List<string> ListNamesToIgnoreForAzure
        {
            get
            {
                if (AdapterConfigurationVariables._listNamesToIgnoreForAzure == null)
                {
                    string configurationVariableWithOverride =
                        AdapterConfigurationVariables.GetConfigurationVariableWithOverride<string>(
                            "ListNamesToIgnoreForAzure");
                    if (!string.IsNullOrEmpty(configurationVariableWithOverride))
                    {
                        string lowerInvariant = configurationVariableWithOverride.ToLowerInvariant();
                        char[] chrArray = new char[] { '|' };
                        AdapterConfigurationVariables._listNamesToIgnoreForAzure =
                            lowerInvariant.Split(chrArray).Distinct<string>().ToList<string>();
                    }
                }

                return AdapterConfigurationVariables._listNamesToIgnoreForAzure;
            }
        }

        public static List<string> ListNamesToIncludeForAzure
        {
            get
            {
                if (AdapterConfigurationVariables._listNamesToIncludeForAzure == null)
                {
                    string configurationVariableWithOverride =
                        AdapterConfigurationVariables.GetConfigurationVariableWithOverride<string>(
                            "ListNamesToIncludeForAzure");
                    if (!string.IsNullOrEmpty(configurationVariableWithOverride))
                    {
                        string lowerInvariant = configurationVariableWithOverride.ToLowerInvariant();
                        char[] chrArray = new char[] { '|' };
                        AdapterConfigurationVariables._listNamesToIncludeForAzure =
                            lowerInvariant.Split(chrArray).Distinct<string>().ToList<string>();
                    }
                }

                return AdapterConfigurationVariables._listNamesToIncludeForAzure;
            }
        }

        public static bool ListViewToXsltViewConversion
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "ListViewToXsltViewConversion");
            }
        }

        public static bool LoadConfluenceUsersFromUserMapping
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "LoadConfluenceUsersFromUserMapping");
            }
        }

        public static int MaxRequestChunkSize
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("MaxRequestChunkSize");
            }
        }

        public static int MaxRequestRetryCount
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("MaxRequestRetryCount");
            }
        }

        public static bool MigrateLanguageSettingForNavigationStructure
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "MigrateLanguageSettingForNavigationStructure");
            }
        }

        public static bool MigrateLanguageSettingForViews
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "MigrateLanguageSettingForViews");
            }
        }

        public static bool MigrateLanguageSettings
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "MigrateLanguageSettings");
            }
        }

        public static int MMDTermsBatchSize
        {
            get { return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("MMDTermsBatchSize"); }
        }

        public static string NotificationEmailAddress
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<string>(
                    "NotificationEmailAddress");
            }
        }

        public static double PipeCloseTimeout
        {
            get
            {
                double configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<double>("PipeCloseTimeout");
                if (configurationVariableWithOverride < 1)
                {
                    configurationVariableWithOverride = 1;
                }

                return configurationVariableWithOverride;
            }
            set { ConfigurationVariables.SetConfigurationValue<double>("PipeCloseTimeout", value); }
        }

        public static double PipeOpenTimeout
        {
            get
            {
                double configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<double>("PipeOpenTimeout");
                if (configurationVariableWithOverride < 1)
                {
                    configurationVariableWithOverride = 1;
                }

                return configurationVariableWithOverride;
            }
            set { ConfigurationVariables.SetConfigurationValue<double>("PipeOpenTimeout", value); }
        }

        public static double PipeReceiveTimeout
        {
            get
            {
                double configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<double>("PipeReceiveTimeout");
                if (configurationVariableWithOverride < 10)
                {
                    configurationVariableWithOverride = 10;
                }

                return configurationVariableWithOverride;
            }
            set { ConfigurationVariables.SetConfigurationValue<double>("PipeReceiveTimeout", value); }
        }

        public static double PipeSendTimeout
        {
            get
            {
                double configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<double>("PipeSendTimeout");
                if (configurationVariableWithOverride < 1)
                {
                    configurationVariableWithOverride = 1;
                }

                return configurationVariableWithOverride;
            }
            set { ConfigurationVariables.SetConfigurationValue<double>("PipeSendTimeout", value); }
        }

        public static bool Show2007OMConnection
        {
            get { return ConfigurationVariables.GetConfigurationValue<bool>("Show2007OMConnection"); }
            set { ConfigurationVariables.SetConfigurationValue<bool>("Show2007OMConnection", value); }
        }

        public static IConfigurationVariable ShowSPOLicenseNotice
        {
            get { return ConfigurationVariables.GetConfigurationVariable("ShowSPOLicenseNotice"); }
        }

        public static int SQLQueryTimeoutTime
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("SQLQueryTimeoutTime");
            }
        }

        public static int SupportedFileSizeWithoutChunk
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>(
                    "SupportedFileSizeWithoutChunk");
            }
        }

        public static bool Swap2003DocMetaInfoColumns
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "Swap2003DocMetaInfoColumns");
            }
        }

        public static bool UseExistingTargetTerms
        {
            get
            {
                return AdapterConfigurationVariables.GetConfigurationVariableWithOverride<bool>(
                    "UseExistingTargetTerms");
            }
        }

        public static int WebServiceRetriesDelay
        {
            get
            {
                int configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("WebServiceRetriesDelay") *
                    1000;
                if (configurationVariableWithOverride < 0)
                {
                    configurationVariableWithOverride = 0;
                }

                return configurationVariableWithOverride;
            }
        }

        public static int WebServiceRetriesNumber
        {
            get
            {
                int configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("WebServiceRetriesNumber");
                if (configurationVariableWithOverride < 0)
                {
                    configurationVariableWithOverride = 0;
                }

                return configurationVariableWithOverride;
            }
        }

        public static int WebServiceTimeoutTime
        {
            get
            {
                int configurationVariableWithOverride =
                    AdapterConfigurationVariables.GetConfigurationVariableWithOverride<int>("WebServiceTimeoutTime") *
                    1000;
                if (configurationVariableWithOverride < 0)
                {
                    configurationVariableWithOverride = 300000;
                }

                return configurationVariableWithOverride;
            }
        }

        static AdapterConfigurationVariables()
        {
            AdapterConfigurationVariables.s_cachedConfigurationVariableOverrides =
                new ThreadSafeSerializableTable<string, string>();
            AdapterConfigurationVariables._defaultListNamesToIgnoreForAzure = "FormServerTemplates|SiteAssets";
            AdapterConfigurationVariables._listNamesToIgnoreForAzure = null;
            AdapterConfigurationVariables._defaultListNamesToIncludeForAzure = string.Empty;
            AdapterConfigurationVariables._listNamesToIncludeForAzure = null;
            AdapterConfigurationVariables.s_ConfiguredWebServiceVersions = null;
            AdapterConfigurationVariables.o_LockWebService = new object();
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment,
                "AllowIE7WebBrowserAuthentication", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment,
                "EnableConcurrentNWSOffice365Connections", false);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "SQLQueryTimeoutTime", 30);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "WebServiceTimeoutTime", 300);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "WebServiceRetriesNumber", 0);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "WebServiceRetriesDelay", 0);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "CSOMDocumentRetriesNumber", 0);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "CSOMDocumentRetriesDelay", 0);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment,
                "EnableChunkedTransfer", false);
            ConfigurationVariables.InitializeConfigurationVariable<StreamType>(ResourceScope.Environment,
                "ChunkStreamType", StreamType.Memory);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "ChunkRetentionTime",
                30);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "Swap2003DocMetaInfoColumns", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "ListViewToXsltViewConversion", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "EnableAccessRequestItemsNotifications", false);
            ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific,
                "NotificationEmailAddress", "CM@CM.CM");
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment, "Allow2013DB",
                false);
            ConfigurationVariables.InitializeConfigurationVariable<double>(ResourceScope.Environment,
                "PipeCloseTimeout", 15);
            ConfigurationVariables.InitializeConfigurationVariable<double>(ResourceScope.Environment, "PipeOpenTimeout",
                15);
            ConfigurationVariables.InitializeConfigurationVariable<double>(ResourceScope.Environment,
                "PipeReceiveTimeout", 20);
            ConfigurationVariables.InitializeConfigurationVariable<double>(ResourceScope.Environment, "PipeSendTimeout",
                15);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "AllowDuplicateSiteCollection", false);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific,
                "MaxRequestChunkSize", 50);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific,
                "MaxRequestRetryCount", 5);
            ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific,
                "ConvertNumericFieldsToText", "Zip,Postal,Telephone,Phone,Tel");
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment, "MMDTermsBatchSize",
                50);
            ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific,
                "ListNamesToIgnoreForAzure", AdapterConfigurationVariables._defaultListNamesToIgnoreForAzure);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "CSOMMaximumExecuteQueryRetries", 10);
            ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific,
                "ListNamesToIncludeForAzure", AdapterConfigurationVariables._defaultListNamesToIncludeForAzure);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "ShowSPOLicenseNotice", true);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific,
                "ChunkSizeForUnshredding", 512);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "LoadConfluenceUsersFromUserMapping", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "UseExistingTargetTerms", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "MigrateLanguageSettings", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "MigrateLanguageSettingForViews", false);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific,
                "LanguageSettingsRefreshInterval", 15);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.ApplicationAndUserSpecific,
                "LanguageSettingsMaximumInterval", 300);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "MigrateLanguageSettingForNavigationStructure", false);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.Environment,
                "Show2007OMConnection", false);
            ConfigurationVariables.InitializeConfigurationVariable<int>(ResourceScope.Environment,
                "SupportedFileSizeWithoutChunk", 200);
            ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific,
                "EnableSslForEmail", false);
            AdapterConfigurationVariables.EnsureNonUISharePointVariables();
        }

        public AdapterConfigurationVariables()
        {
        }

        public static void EnsureNonUISharePointVariables()
        {
            bool swap2003DocMetaInfoColumns = AdapterConfigurationVariables.Swap2003DocMetaInfoColumns;
            bool enableChunkedTransfer = AdapterConfigurationVariables.EnableChunkedTransfer;
            bool allowIE7WebBrowserAuthentication = AdapterConfigurationVariables.AllowIE7WebBrowserAuthentication;
            int sQLQueryTimeoutTime = AdapterConfigurationVariables.SQLQueryTimeoutTime;
            int webServiceTimeoutTime = AdapterConfigurationVariables.WebServiceTimeoutTime;
            int webServiceRetriesNumber = AdapterConfigurationVariables.WebServiceRetriesNumber;
            int webServiceRetriesDelay = AdapterConfigurationVariables.WebServiceRetriesDelay;
            int cSOMDocumentRetriesNumber = AdapterConfigurationVariables.CSOMDocumentRetriesNumber;
            int cSOMDocumentRetriesDelay = AdapterConfigurationVariables.CSOMDocumentRetriesDelay;
            int cSOMMaximumExecuteQueryRetries = AdapterConfigurationVariables.CSOMMaximumExecuteQueryRetries;
            int chunkRetentionTime = AdapterConfigurationVariables.ChunkRetentionTime;
            StreamType chunkStreamType = AdapterConfigurationVariables.ChunkStreamType;
            bool listViewToXsltViewConversion = AdapterConfigurationVariables.ListViewToXsltViewConversion;
            bool allow2013DB = AdapterConfigurationVariables.Allow2013DB;
            double pipeCloseTimeout = AdapterConfigurationVariables.PipeCloseTimeout;
            double pipeOpenTimeout = AdapterConfigurationVariables.PipeOpenTimeout;
            double pipeReceiveTimeout = AdapterConfigurationVariables.PipeReceiveTimeout;
            double pipeSendTimeout = AdapterConfigurationVariables.PipeSendTimeout;
            bool allowDuplicateSiteCollection = AdapterConfigurationVariables.AllowDuplicateSiteCollection;
            bool enableAccessRequestItemsNotifications =
                AdapterConfigurationVariables.EnableAccessRequestItemsNotifications;
            string notificationEmailAddress = AdapterConfigurationVariables.NotificationEmailAddress;
            int maxRequestChunkSize = AdapterConfigurationVariables.MaxRequestChunkSize;
            int maxRequestRetryCount = AdapterConfigurationVariables.MaxRequestRetryCount;
            string convertNumericFieldsToText = AdapterConfigurationVariables.ConvertNumericFieldsToText;
            int mMDTermsBatchSize = AdapterConfigurationVariables.MMDTermsBatchSize;
            int num = AdapterConfigurationVariables.MMDTermsBatchSize;
            List<string> listNamesToIgnoreForAzure = AdapterConfigurationVariables.ListNamesToIgnoreForAzure;
            int chunkSizeForUnshredding = AdapterConfigurationVariables.ChunkSizeForUnshredding;
            List<string> listNamesToIncludeForAzure = AdapterConfigurationVariables.ListNamesToIncludeForAzure;
            bool loadConfluenceUsersFromUserMapping = AdapterConfigurationVariables.LoadConfluenceUsersFromUserMapping;
            bool useExistingTargetTerms = AdapterConfigurationVariables.UseExistingTargetTerms;
            bool migrateLanguageSettings = AdapterConfigurationVariables.MigrateLanguageSettings;
            int languageSettingsRefreshInterval = AdapterConfigurationVariables.LanguageSettingsRefreshInterval;
            int languageSettingsMaximumInterval = AdapterConfigurationVariables.LanguageSettingsMaximumInterval;
            bool migrateLanguageSettingForNavigationStructure =
                AdapterConfigurationVariables.MigrateLanguageSettingForNavigationStructure;
            bool migrateLanguageSettingForViews = AdapterConfigurationVariables.MigrateLanguageSettingForViews;
            bool show2007OMConnection = AdapterConfigurationVariables.Show2007OMConnection;
            int supportedFileSizeWithoutChunk = AdapterConfigurationVariables.SupportedFileSizeWithoutChunk;
            bool enableSslForEmail = AdapterConfigurationVariables.EnableSslForEmail;
        }

        private static T GetConfigurationVariableWithOverride<T>(string configVariableKey)
            where T : IConvertible
        {
            string str;
            if (!AdapterConfigurationVariables.s_cachedConfigurationVariableOverrides.TryGetValue(configVariableKey,
                    out str))
            {
                return ConfigurationVariables.GetConfigurationValue<T>(configVariableKey);
            }

            return (T)Convert.ChangeType(str, typeof(T));
        }

        public static void LoadConfigurationVariables(string XML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(XML);
            AdapterConfigurationVariables.s_cachedConfigurationVariableOverrides =
                new ThreadSafeSerializableTable<string, string>(xmlDocument.DocumentElement);
        }
    }
}