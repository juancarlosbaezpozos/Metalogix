using Metalogix;
using Metalogix.Licensing.CA;
using Metalogix.Licensing.Common;
using Metalogix.Licensing.SK;
using Metalogix.Telemetry;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Metalogix.Licensing
{
    public sealed class MLLicenseProvider : LicenseProvider
    {
        private readonly static object LicenseLock;

        private static volatile ILicenseProvider _instance;

        private static string s_sLicensingAssemblyLocation;

        private static volatile bool _isTelemetryInitialized;

        public static ILicenseProvider Instance
        {
            get
            {
                if (!MLLicenseProvider.IsInitialized)
                {
                    MLLicenseProvider.TryInitialize(null);
                }

                return MLLicenseProvider._instance;
            }
        }

        public static bool IsInitialized
        {
            get { return MLLicenseProvider._instance != null; }
        }

        public static string LicensingAssemblyLocation
        {
            get { return MLLicenseProvider.s_sLicensingAssemblyLocation; }
        }

        static MLLicenseProvider()
        {
            MLLicenseProvider.LicenseLock = new object();
            MLLicenseProvider._instance = null;
            MLLicenseProvider.s_sLicensingAssemblyLocation = null;
        }

        public MLLicenseProvider()
        {
        }

        private static void _instance_LicenseUpdated(object sender, EventArgs e)
        {
            if (MLLicenseProvider.LicenseUpdated != null)
            {
                MLLicenseProvider.LicenseUpdated(sender, e);
                if (!MLLicenseProvider._isTelemetryInitialized)
                {
                    return;
                }

                Client.Update(null, null, true);
            }
        }

        public static void Dispose()
        {
            if (MLLicenseProvider._instance != null)
            {
                MLLicenseProvider._instance.Dispose();
            }

            MLLicenseProvider._instance = null;
            if (MLLicenseProvider.LicenseDisposed != null)
            {
                MLLicenseProvider.LicenseDisposed(typeof(MLLicenseProvider), new EventArgs());
            }
        }

        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            return MLLicenseProvider._instance.GetLicense(context, type, instance, allowExceptions);
        }

        public static void Initialize(LicenseProviderInitializationData data)
        {
            if (MLLicenseProvider.IsInitialized)
            {
                throw new Exception("LicenseProvider instance is already initialized and cannot be re-initialize.");
            }

            Assembly mainAssembly = ApplicationData.MainAssembly;
            LicenseProviderType providerType = LicenseProviderType.CA;
            if (data != null)
            {
                providerType = data.ProviderType;
                MLLicenseProvider.s_sLicensingAssemblyLocation = data.LicensingAssemblyLocation;
            }
            else if (mainAssembly != null)
            {
                data = LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(mainAssembly);
                if (data != null)
                {
                    providerType = data.ProviderType;
                    MLLicenseProvider.s_sLicensingAssemblyLocation = data.LicensingAssemblyLocation;
                }
                else
                {
                    object[] customAttributes =
                        mainAssembly.GetCustomAttributes(typeof(Metalogix.Licensing.LicenseProviderAttribute), true);
                    if ((int)customAttributes.Length > 0)
                    {
                        Metalogix.Licensing.LicenseProviderAttribute licenseProviderAttribute =
                            customAttributes[0] as Metalogix.Licensing.LicenseProviderAttribute;
                        if (licenseProviderAttribute != null)
                        {
                            providerType = licenseProviderAttribute.Type;
                        }
                    }

                    MLLicenseProvider.s_sLicensingAssemblyLocation = mainAssembly.Location;
                }
            }

            switch (providerType)
            {
                case LicenseProviderType.CA:
                {
                    MLLicenseProvider._instance = new MLLicenseProviderCA(data);
                    break;
                }
                case LicenseProviderType.SK:
                {
                    MLLicenseProvider._instance = new MLLicenseProviderSK(data);
                    break;
                }
                case LicenseProviderType.Common:
                {
                    if (!(data is LicenseSettings))
                    {
                        throw new Exception("Initialization data is invalid for the given license provider.");
                    }

                    MLLicenseProvider._instance = new MLLicenseProviderCommon((LicenseSettings)data);
                    break;
                }
                case LicenseProviderType.Custom:
                {
                    if (mainAssembly == null)
                    {
                        throw new Exception("Entry assembly cannot be null when using a custom license provider.");
                    }

                    object[] objArray = mainAssembly.GetCustomAttributes(typeof(LicenseProviderTypeAttribute), true);
                    if ((int)objArray.Length == 0)
                    {
                        throw new Exception(
                            "LicenseProviderType attribute cannot be missing when using a custom license provider.");
                    }

                    Type type = (objArray[0] as LicenseProviderTypeAttribute).ProviderType;
                    object[] objArray1 = new object[] { data };
                    MLLicenseProvider._instance = (ILicenseProvider)Activator.CreateInstance(type, objArray1);
                    break;
                }
                default:
                {
                    throw new Exception(string.Format("Incorrect license provider type '{0}'", providerType));
                }
            }

            MLLicenseProvider._instance.LicenseUpdated += new EventHandler(MLLicenseProvider._instance_LicenseUpdated);
            MLLicenseProvider._instance.Initialize();
        }

        public static void InitializeTelemetry()
        {
            lock (MLLicenseProvider.LicenseLock)
            {
                if (!MLLicenseProvider.IsInitialized)
                {
                    throw new Exception("Telemetry cannot be initialized if LicenseProvider is not initialized.");
                }

                MLLicenseProviderCommon mLLicenseProviderCommon =
                    MLLicenseProvider._instance as MLLicenseProviderCommon;
                if (mLLicenseProviderCommon != null)
                {
                    string actualLicenseKey = mLLicenseProviderCommon.GetActualLicenseKey();
                    if (!string.IsNullOrEmpty(actualLicenseKey))
                    {
                        Client.Initialize(mLLicenseProviderCommon.Settings.ProductName, actualLicenseKey, true, false,
                            false);
                        MLLicenseProvider._isTelemetryInitialized = true;
                        Client.ProfileSystem();
                    }
                }
            }
        }

        public static void Notify()
        {
            MLLicenseProvider._instance_LicenseUpdated(null, null);
        }

        public static void TearDownTelemetry()
        {
            if (!MLLicenseProvider._isTelemetryInitialized)
            {
                return;
            }

            lock (MLLicenseProvider.LicenseLock)
            {
                if (MLLicenseProvider._isTelemetryInitialized)
                {
                    Client.TearDown();
                }

                MLLicenseProvider._isTelemetryInitialized = false;
            }
        }

        public static bool TryInitialize(LicenseProviderInitializationData data)
        {
            bool flag;
            if (!MLLicenseProvider.IsInitialized)
            {
                lock (MLLicenseProvider.LicenseLock)
                {
                    if (MLLicenseProvider.IsInitialized)
                    {
                        return false;
                    }
                    else
                    {
                        MLLicenseProvider.Initialize(data);
                        flag = true;
                    }
                }

                return flag;
            }

            return false;
        }

        public static event EventHandler LicenseDisposed;

        public static event EventHandler LicenseUpdated;
    }
}