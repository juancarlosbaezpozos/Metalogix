using PreEmptive.SoS.Client.Messages;
using System;
using System.Reflection;

namespace PreEmptive.SoS.Client.Cache
{
    public sealed class CacheServiceFactory
    {
        private static EventInformation startEvent;

        private static EventInformation stopEvent;

        private static CacheServiceConfiguration config;

        private static BinaryInformation binary;

        static CacheServiceFactory()
        {
            CacheServiceFactory.startEvent = new EventInformation();
            CacheServiceFactory.stopEvent = new EventInformation();
            CacheServiceFactory.startEvent.Code = "Application.Start";
            CacheServiceFactory.stopEvent.Code = "Application.Stop";
        }

        private CacheServiceFactory()
        {
        }

        public static CacheService CreateCacheService(CacheServiceConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentException("Argument cannot be null", "configuration");
            }

            CacheServiceFactory.config = configuration;
            CacheService cacheService = new CacheService(CacheServiceFactory.config);
            cacheService.Started += new CacheStartedEventHandler(CacheServiceFactory.SendStartUpMessage);
            cacheService.ShuttingDown += new CacheShuttingDownEventHandler(CacheServiceFactory.SendShutdownMessage);
            CacheServiceFactory.binary = CacheServiceFactory.GetBinaryInformation(configuration);
            cacheService.Start();
            return cacheService;
        }

        private static BinaryInformation GetBinaryInformation(CacheServiceConfiguration configuration)
        {
            if (configuration.Binary != null)
            {
                return configuration.Binary;
            }

            if (configuration.Assembly == null)
            {
                return BinaryInformation.CreateFromTaggedAssembly(Assembly.GetCallingAssembly());
            }

            return BinaryInformation.CreateFromTaggedAssembly(configuration.Assembly);
        }

        private static void SendShutdownMessage(object sender, CacheEventArgs e)
        {
            CacheService cacheService = (CacheService)sender;
            ApplicationLifeCycleMessage applicationLifeCycleMessage = new ApplicationLifeCycleMessage();
            applicationLifeCycleMessage.Host.HashSensitiveData = CacheServiceFactory.config.HashSensitiveData;
            applicationLifeCycleMessage.User.HashSensitiveData = CacheServiceFactory.config.HashSensitiveData;
            applicationLifeCycleMessage.Host.OmitPersonalInformation =
                CacheServiceFactory.config.OmitPersonalInformation;
            applicationLifeCycleMessage.User.OmitPersonalInformation =
                CacheServiceFactory.config.OmitPersonalInformation;
            applicationLifeCycleMessage.Binary = CacheServiceFactory.binary;
            applicationLifeCycleMessage.Event = CacheServiceFactory.stopEvent;
            cacheService.Send(applicationLifeCycleMessage);
        }

        private static void SendStartUpMessage(object sender, CacheEventArgs e)
        {
            CacheService cacheService = (CacheService)sender;
            ApplicationLifeCycleMessage applicationLifeCycleMessage = new ApplicationLifeCycleMessage();
            applicationLifeCycleMessage.Host.HashSensitiveData = CacheServiceFactory.config.HashSensitiveData;
            applicationLifeCycleMessage.User.HashSensitiveData = CacheServiceFactory.config.HashSensitiveData;
            applicationLifeCycleMessage.Host.OmitPersonalInformation =
                CacheServiceFactory.config.OmitPersonalInformation;
            applicationLifeCycleMessage.User.OmitPersonalInformation =
                CacheServiceFactory.config.OmitPersonalInformation;
            applicationLifeCycleMessage.Binary = CacheServiceFactory.binary;
            applicationLifeCycleMessage.Event = CacheServiceFactory.startEvent;
            cacheService.Send(applicationLifeCycleMessage);
        }
    }
}