using PreEmptive.SoS.Client.Cache;
using PreEmptive.SoS.Client.MessageProxies;
using PreEmptive.SoS.Client.Messages;
using System;

namespace PreEmptive.SoS.Runtime
{
    public sealed class Access
    {
        public static PreEmptive.SoS.Client.Messages.ApplicationInformation ApplicationInfo;

        public static PreEmptive.SoS.Client.Messages.BusinessInformation BusinessInfo;

        private static CacheService cacheService;

        private static string instanceId;

        private static bool optIn;

        private static bool offlineState;

        private static FeatureCorrelator featureCorrelator;

        public static PreEmptive.SoS.Client.Messages.ApplicationInformation CreateApplicationInformation(
            string string_0, string string_1, string string_2, string string_3)
        {
            return new PreEmptive.SoS.Client.Messages.ApplicationInformation(new Guid(string_0), string_1, string_2,
                string_3);
        }

        public static PreEmptive.SoS.Client.Messages.BusinessInformation CreateBusinessInformation(string string_0,
            string string_1)
        {
            return new PreEmptive.SoS.Client.Messages.BusinessInformation(new Guid(string_0), string_1);
        }

        public static Guid FeatureGroup(string string_0, bool bool_0)
        {
            if (Access.featureCorrelator == null)
            {
                Access.featureCorrelator = new FeatureCorrelator();
            }

            return (bool_0 ? Access.featureCorrelator.Add(string_0) : Access.featureCorrelator.Remove(string_0));
        }

        public static string GetApplicationName()
        {
            string name;
            if (Access.ApplicationInfo == null)
            {
                name = null;
            }
            else
            {
                name = Access.ApplicationInfo.Name;
            }

            return name;
        }

        public static string GetCompanyName()
        {
            string companyName;
            if (Access.BusinessInfo == null)
            {
                companyName = null;
            }
            else
            {
                companyName = Access.BusinessInfo.CompanyName;
            }

            return companyName;
        }

        public static void Send(PreEmptive.SoS.Client.Messages.Message message_0)
        {
            if (Access.cacheService != null &&
                (Access.optIn || message_0.Event.PrivacySetting != PrivacySettings.SupportOptout))
            {
                Access.cacheService.Send(message_0);
            }
        }

        public static void Setup(string string_0, bool bool_0, bool bool_1,
            PreEmptive.SoS.Client.Messages.BinaryInformation binaryInformation_0)
        {
            Access.optIn = bool_0;
            Access.offlineState = bool_1;
            if (Access.cacheService == null)
            {
                Access.instanceId = string_0;
                CacheServiceConfiguration cacheServiceConfiguration = new CacheServiceConfiguration(Access.instanceId);
                CacheServiceConfiguration cacheServiceConfiguration1 = cacheServiceConfiguration;
                cacheServiceConfiguration.SetProperty("webservice.url", "telemetry.metalogix.com");
                cacheServiceConfiguration.SetProperty("offline.support", true);
                cacheServiceConfiguration.SetProperty("offline.state", Access.offlineState);
                cacheServiceConfiguration.UseSSL = true;
                cacheServiceConfiguration.OmitPersonalInformation = false;
                cacheServiceConfiguration.HashSensitiveData = true;
                if (!Access.optIn)
                {
                    cacheServiceConfiguration.SetProperty("fire.lifecycle.events", false);
                }

                Access.SetupRuntime(cacheServiceConfiguration, binaryInformation_0);
                Access.cacheService = CacheServiceFactory.CreateCacheService(cacheServiceConfiguration1);
            }

            if (Access.featureCorrelator == null)
            {
                Access.featureCorrelator = new FeatureCorrelator();
            }
        }

        public static void SetupRuntime(CacheServiceConfiguration cacheServiceConfiguration_0,
            PreEmptive.SoS.Client.Messages.BinaryInformation binaryInformation_0)
        {
            cacheServiceConfiguration_0.Binary = binaryInformation_0;
            cacheServiceConfiguration_0.Business = (Access.BusinessInfo == null
                ? new PreEmptive.SoS.Client.Messages.BusinessInformation(
                    new Guid("3aa12aaa-510d-4047-a19b-4364abbb1767"), "Metalogix - Content Matrix")
                : Access.BusinessInfo);
            cacheServiceConfiguration_0.Application = (Access.ApplicationInfo == null
                ? new PreEmptive.SoS.Client.Messages.ApplicationInformation(
                    new Guid("a2afe538-17f5-4ca0-bd0a-6ab8d9855412"), "Unknown Application", "", "")
                : Access.ApplicationInfo);
        }

        public static void Teardown()
        {
            if (Access.cacheService != null)
            {
                Access.cacheService.ShutDown();
            }
        }
    }
}