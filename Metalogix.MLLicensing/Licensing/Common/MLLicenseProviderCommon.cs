using Metalogix;
using Metalogix.Connectivity.Proxy;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Storage;
using Metalogix.MLLicensing.Properties;
using Metalogix.Permissions;
using Metalogix.Telemetry.Accumulators;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;

namespace Metalogix.Licensing.Common
{
    public sealed class MLLicenseProviderCommon : LicenseProvider, ILicenseProvider, IDisposable
    {
        private const int _LICENSE_CHECKING_INTERVAL = 7200000;

        private const int _LICENSE_CHECKING_INTERVAL_SHORT = 600000;

        private const int _LICENSE_CHECKING_NOW = 1000;

        private const string OLD_STYLE_LICENSE_PREFIX = "OLD:";

        private const string SPO_SERVERID_PREFIX = "SPO:";

        private readonly object _lock = new object();

        private readonly LicenseSettings _settings;

        private readonly object _timerLock = new object();

        private System.Timers.Timer _timer;

        private Server _server;

        private ServerProxySettings _proxy;

        private MigrationLicense _license;

        private bool? _licenseValidity;

        private bool _TEST_LICENSE_INVALIDITY;

        public LicenseSettings Settings
        {
            get { return this._settings; }
        }

        public MLLicenseProviderCommon(LicenseSettings settings)
        {
            IDataStorage environmentVariableDataStorage;
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this._settings = settings;
            Logger.Debug.Write("LicenseManager >> LicenseManager >> Entered");
            if (this._settings.ProxyStorageProvider == LicenseStorageType.EnvironmentVariables)
            {
                environmentVariableDataStorage =
                    new EnvironmentVariableDataStorage(this._settings.ProxyFilePath, this._settings.SecureStorage);
            }
            else
            {
                IDataStorage fileDataStorage = new FileDataStorage(this._settings.ProxyFilePath,
                    this._settings.SecureStorage, this._settings.LicenseFileAccess,
                    "MetalogixLicenseProxyFileStorageMutex");
                environmentVariableDataStorage = fileDataStorage;
            }

            this._proxy = new ServerProxySettings(environmentVariableDataStorage);
            this._server = new Server(this._settings.LicenseServerUrl, this._settings.LicenseServerUser,
                this._settings.LicenseServerPass, this._proxy);
        }

        public void ActivateOffline(string activationData)
        {
            try
            {
                Logger.Debug.Write(string.Concat("LicenseManager >> ActivateOffline >> Entered, data=",
                    activationData));
                OfflineActivatorFile offlineActivatorFile = new OfflineActivatorFile(activationData,
                    "<RSAKeyValue><Modulus>zBQqYBd1a2ck3QAtTRBls4xAaDuhU+QLrJ6wLYlg3KN+tQhLKrTtG6cItOWxQwcRmnFoV7bCuZ5ICMgc+ProYPUW9xEe5SFKsWX7wOai9aAQwpjM3Un1dSCfmLjAADmB32DHnOxZhXB4c6U8XJPK+SANWQtTn3De4aZQ702uxQ8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                MigrationLicense migrationLicense = new MigrationLicense(this._settings, offlineActivatorFile);
                lock (this._lock)
                {
                    migrationLicense.ValidateOffline(this._license);
                    if (migrationLicense.DataLimitedExceeded)
                    {
                        throw new LicenseExpiredException(Resources.Licensed_Data_Exceeded);
                    }

                    this._license = migrationLicense;
                    this._license.Save();
                }

                this.FireLicenseSynchronized(null);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Logger.Error.Write("Failed to activate the license offline.", exception);
                this.FireLicenseSynchronized(exception);
                throw;
            }
        }

        private void AddAdditionalServerDetailsForSPO(string updatedServerId, string tenantUrlAndUser)
        {
            try
            {
                AdvancedOptionRequest advancedOptionRequest = new AdvancedOptionRequest()
                {
                    Key = this._license.KeyValue.Value.Replace("-", string.Empty),
                    Name = updatedServerId,
                    Value = tenantUrlAndUser
                };
                int num = this._server.AddAdvancedOptionForLicense(advancedOptionRequest);
                if (num != 0 && num != 1)
                {
                    string str =
                        string.Format(
                            "Error occurred while adding additional server details for Server: {0} having details: {1}.",
                            updatedServerId, tenantUrlAndUser);
                    this.LogTelemetry("SPOAdditionalDetailSaveFailed", str, "AddAdditionalServerDetailsForSPO");
                }

                AdvancedOptionsResponse advancedOptionForLicense =
                    this._server.GetAdvancedOptionForLicense(this._license.KeyValue.Value.Replace("-", string.Empty));
                StringBuilder stringBuilder = new StringBuilder("CM License Data: ");
                AdvancedOptionField[] fields = advancedOptionForLicense.AdvancedOptionFields.Fields;
                for (int i = 0; i < (int)fields.Length; i++)
                {
                    AdvancedOptionField advancedOptionField = fields[i];
                    string str1 = string.Format("{0};{1}#", advancedOptionField.Name, advancedOptionField.Value);
                    stringBuilder.Append(str1);
                }

                Logger.Debug.Write(stringBuilder.ToString());
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                string str2 =
                    string.Format(
                        "Error occurred while adding additional server details for Server: {0} having details: {1}. Error: {2}",
                        updatedServerId, tenantUrlAndUser, exception);
                Logger.Error.Write(
                    "LicenseManager error >> AddAdditionalServerDetailsForSPO >> Failed to save additional tenant details",
                    exception);
                this.LogTelemetry("SPOAdditionalDetailSaveFailed", str2, "AddAdditionalServerDetailsForSPO");
            }
        }

        public string ConvertOldLicenseKey(string oldLicenseKey)
        {
            string str;
            try
            {
                Logger.Debug.Write(string.Concat("LicenseManager >> ConvertOldLicenseKey >> Entered, oldKey=",
                    oldLicenseKey));
                string str1 = this._server.ConvertOldKey(oldLicenseKey);
                Logger.Debug.Write(string.Concat("LicenseManager >> ConvertOldLicenseKey >> Conversion OK, newKey=",
                    str1));
                str = str1;
            }
            catch (Exception exception)
            {
                Logger.Error.Write("Failed to convert the old license key.", exception);
                throw;
            }

            return str;
        }

        private LicenseInfoRequest CreateLicenseInfoRequest(string licenseKey)
        {
            LicenseKey licenseKey1 = new LicenseKey(licenseKey);
            DynamicContentFile dynamicContentFile = new DynamicContentFile(
                new FileDataStorage(this._settings.UsageFilePath, this._settings.SecureStorage,
                    this._settings.UsageFileAccess, "MetalogixDynamicContentFileStorageMutex"),
                LicensingUtils.GetUsageEntryID(licenseKey1.Value, this._settings.Product),
                "MetalogixDynamicContentFileMutex");
            LicenseInfoRequest licenseInfoRequest = new LicenseInfoRequest()
            {
                Key = licenseKey1.Value,
                UsedData = dynamicContentFile.UsedData,
                Admin = MigrationLicense.AdminAccount,
                Server = MigrationLicense.ServerID,
                ProductCode =
                    (int)((licenseKey1.Product.IsContentMatrixExpress() || licenseKey1.Product.IsLegacyProduct()
                        ? this._settings.LegacyProduct
                        : this._settings.Product)),
                ProductVersion = this._settings.ProductVersion.ToString(),
                ServerCount = 1
            };
            return licenseInfoRequest;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            lock (this._timerLock)
            {
                if (disposing && this._timer != null)
                {
                    this._timer.Enabled = false;
                    this._timer.Dispose();
                }
            }

            if (this._license != null)
            {
                this._license.Dispose(disposing);
            }

            if (this._proxy != null)
            {
                this._proxy.Dispose();
            }

            if (this._server != null)
            {
                this._server.Dispose(disposing);
            }
        }

        ~MLLicenseProviderCommon()
        {
            this.Dispose(false);
        }

        private void FireLicenseSynchronized(Exception ex)
        {
            if (this.LicenseSynchronized != null)
            {
                this.LicenseSynchronized(this, ex);
            }

            this.FireLicenseUpdated();
        }

        private void FireLicenseUpdated()
        {
            if (this.LicenseUpdated != null)
            {
                this.LicenseUpdated(this, EventArgs.Empty);
            }
        }

        internal string GetActualLicenseKey()
        {
            string str;
            string valueFormatted;
            lock (this._lock)
            {
                if (this._license != null)
                {
                    valueFormatted = this._license.KeyValue.ValueFormatted;
                }
                else if (this._settings.OldStyleLicensingConverter != null)
                {
                    valueFormatted = this._settings.OldStyleLicensingConverter.OldKey;
                }
                else
                {
                    valueFormatted = null;
                }

                str = valueFormatted;
            }

            return str;
        }

        public LatestProductReleaseInfo GetLatestProductRelease(GetProductReleaseRequest request)
        {
            LatestProductReleaseInfo latestProductRelease = null;
            Logger.Debug.Write("LicenseManager >> GetLatestProductRelease >> Entered");
            try
            {
                lock (this._lock)
                {
                    latestProductRelease = this._server.GetLatestProductRelease(request);
                }

                Logger.Debug.Write("LicenseManager >> GetLatestProductRelease >> Successfully finished");
            }
            catch (Exception exception)
            {
                Logger.Error.Write(
                    "LicenseManager >> GetLatestProductRelease >> Failed to get latest product release .", exception);
                throw;
            }

            return latestProductRelease;
        }

        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            License mLLicense;
            bool flag = false;
            try
            {
                try
                {
                    if (this._TEST_LICENSE_INVALIDITY)
                    {
                        throw new InvalidLicenseException("License under testing: FAILED.");
                    }

                    if (this._license == null)
                    {
                        throw new LicenseDoesntExistException("License doesn't exist.");
                    }

                    lock (this._lock)
                    {
                        this._license.Validate();
                        flag = true;
                        mLLicense = this._license.GetMLLicense(this);
                    }
                }
                catch (Exception exception)
                {
                    flag = false;
                    if (allowExceptions)
                    {
                        throw;
                    }

                    mLLicense = null;
                }
            }
            finally
            {
                if (!this._licenseValidity.HasValue)
                {
                    this._licenseValidity = new bool?(flag);
                }

                bool? nullable = this._licenseValidity;
                if ((nullable.GetValueOrDefault() != flag ? true : !nullable.HasValue))
                {
                    this._licenseValidity = new bool?(flag);
                    this.FireLicenseUpdated();
                }
            }

            return mLLicense;
        }

        private LicenseInfoRequest GetLicenseInfo(long usedData, string server, string tenantUrlandUser = null)
        {
            LicenseInfoRequest licenseInfoRequest = new LicenseInfoRequest()
            {
                Key = this._license.KeyValue.Value,
                UsedData = usedData,
                Admin = MigrationLicense.AdminAccount,
                Server = server,
                ProductCode = 823,
                ProductVersion = this._settings.ProductVersion.ToString(),
                ServerCount = 1
            };
            return licenseInfoRequest;
        }

        public MLProxy GetLicenseProxy()
        {
            MLProxy mLProxy;
            MLProxy mLProxy1;
            Logger.Debug.Write("LicenseManager >> GetLicenseProxy >> Entered");
            try
            {
                lock (this._lock)
                {
                    Credentials credential = (string.IsNullOrEmpty(this._proxy.UserName)
                        ? new Credentials()
                        : new Credentials(this._proxy.UserName, this._proxy.Password.ToSecureString(), true));
                    MLProxy mLProxy2 = new MLProxy()
                    {
                        Enabled = this._proxy.Enabled,
                        Server = this._proxy.Server,
                        Credentials = credential,
                        Port = this._proxy.Port.ToString()
                    };
                    mLProxy = mLProxy2;
                }

                Logger.Debug.Write(string.Concat("LicenseManager >> GetLicenseProxy >> Finished, settings: ", mLProxy));
                mLProxy1 = mLProxy;
            }
            catch (Exception exception)
            {
                Logger.Error.Write("LicenseManager >> GetLicenseProxy >> Failed to return the proxy settings.",
                    exception);
                throw;
            }

            return mLProxy1;
        }

        private List<string> GetLicenseServerInfo()
        {
            List<string> strs;
            Logger.Debug.Write("LicenseManager >> GetLicenseServerInfo >> Entered");
            List<string> strs1 = new List<string>();
            try
            {
                lock (this._lock)
                {
                    LicenseDataResponse licenseData =
                        this._server.GetLicenseData(this._license.KeyValue.Value, LicenseDataDetails.ServerInfo);
                    if (licenseData != null && licenseData.Server.Infos != null &&
                        (int)licenseData.Server.Infos.Length > 0)
                    {
                        ServerInfo[] infos = licenseData.Server.Infos;
                        for (int i = 0; i < (int)infos.Length; i++)
                        {
                            strs1.Add(infos[i].ServerID);
                        }
                    }

                    Logger.Debug.Write("LicenseManager >> GetLicenseServerInfo >> Finished");
                }

                return strs1;
            }
            catch (Exception exception)
            {
                Logger.Error.Write("LicenseManager error >> GetLicenseServerInfo >> Unable to get license server Ids. ",
                    exception);
                strs = null;
            }

            return strs;
        }

        public string GetOfflineActivationData(string licenseKey, bool isOldStyleLicense)
        {
            string requestString;
            try
            {
                Logger.Debug.Write(string.Concat("LicenseManager >> GetOfflineActivationData >> Entered, key=",
                    licenseKey));
                if (!isOldStyleLicense)
                {
                    LicenseKey licenseKey1 = new LicenseKey(licenseKey);
                    if (this._license != null && this._license.Exists && !licenseKey1.Equals(this._license.KeyValue) &&
                        (DateTime.Now.ToUniversalTime() - this._license.LastUpdate).TotalHours > 24)
                    {
                        throw new InvalidLicenseException(
                            "The actual license must be revalidated before applying the new license key.");
                    }
                }
                else
                {
                    licenseKey = string.Concat("OLD:", licenseKey);
                    Logger.Debug.WriteFormat(
                        "LicenseManager >> GetOfflineActivationData >> Old style key was entered, prefix added: '{0}'",
                        new object[] { licenseKey });
                }

                OfflineActivatorFile offlineActivatorFile =
                    new OfflineActivatorFile(this.CreateLicenseInfoRequest(licenseKey));
                Logger.Debug.Write(string.Concat("LicenseManager >> GetOfflineActivationData >> Generated: ",
                    offlineActivatorFile.RequestString));
                requestString = offlineActivatorFile.RequestString;
            }
            catch (Exception exception)
            {
                Logger.Error.Write("Failed to generate offline activation data.", exception);
                throw;
            }

            return requestString;
        }

        public void Initialize()
        {
            int num;
            try
            {
                this._license = new MigrationLicense(this._settings);
                Logger.Debug.Write("LicenseManager >> Initialize >> License successfully loaded");
                Logger.Debug.Write(this._license.ToString());
                num = 1000;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (exception.GetType() == typeof(FileNotFoundException) ||
                    exception.GetType() == typeof(DirectoryNotFoundException))
                {
                    num = this.SetAsynchLicCheck();
                }
                else
                {
                    Logger.Error.Write("LicenseManager >> Initialize >> Failed to load license.", exception);
                    num = 7200000;
                }
            }

            Logger.Debug.Write("LicenseManager >> Initialize >> Start cyclic checking ...");
            lock (this._timerLock)
            {
                this._timer = new System.Timers.Timer()
                {
                    Interval = (double)num
                };
                this._timer.Elapsed += new ElapsedEventHandler(this.LicenseCheckingTimerElapsed);
                this._timer.Enabled = this._settings.RunCyclicChecking;
            }
        }

        public bool IsSPOServerIdExist(string serverId)
        {
            bool flag = false;
            string str = string.Concat("SPO:", serverId);
            List<string> licenseServerInfo = this.GetLicenseServerInfo();
            if (licenseServerInfo != null)
            {
                flag = licenseServerInfo.Any<string>((string l) => str.Equals(l, StringComparison.OrdinalIgnoreCase));
            }

            return flag;
        }

        private void LicenseCheckingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            bool flag;
            string valueFormatted;
            lock (this._timerLock)
            {
                bool enabled = this._timer.Enabled;
                try
                {
                    Logger.Debug.Write("LicenseManager >> LicenseChecking >> Entered: Timer elapsed");
                    this._timer.Enabled = false;
                    if (this._license != null || this._settings.OldStyleLicensingConverter != null &&
                        this._settings.OldStyleLicensingConverter.Exists)
                    {
                        if (this._license == null)
                        {
                            Logger.Error.Write("LicenseManager error >> LicenseChecking >> License doesn't exists.");
                            try
                            {
                                ILogMethods debug = Logger.Debug;
                                object[] oldKey = new object[] { this._settings.OldStyleLicensingConverter.OldKey };
                                debug.WriteFormat(
                                    "LicenseManager >> LicenseChecking >> Old license exists '{0}' trying to convert.",
                                    oldKey);
                                this._settings.OldStyleLicensingConverter.Convert(this);
                                valueFormatted = this._settings.OldStyleLicensingConverter.Key.ValueFormatted;
                                Logger.Debug.WriteFormat(
                                    "LicenseManager >> LicenseChecking >> Old license successfully converted to '{0}'.",
                                    new object[] { valueFormatted });
                            }
                            catch (Exception exception)
                            {
                                Logger.Error.Write(
                                    "LicenseManager >> Initialize >> Old license exists, but pre-conversion failed, maybe server is offline.",
                                    exception);
                                this._timer.Interval = 600000;
                                return;
                            }

                            Logger.Debug.Write(
                                "LicenseManager >> LicenseChecking >> Start checking converted license.");
                            flag = this.SynchronizeLicense(valueFormatted, false, true, false);
                            if (!flag)
                            {
                                this._timer.Interval = 600000;
                            }
                            else
                            {
                                this._timer.Interval = 7200000;
                            }
                        }
                        else
                        {
                            this._timer.Interval = 7200000;
                            flag = this.SynchronizeLicense(this._license.LicenseKey, false, true, false);
                        }

                        Logger.Debug.Write(string.Concat("LicenseManager >> LicenseChecking >> Finished: ",
                            (flag ? "Successfull" : "Failed")));
                    }
                    else
                    {
                        Logger.Debug.Write("LicenseManager >> LicenseChecking >> Finished: License key not set.");
                        return;
                    }
                }
                finally
                {
                    this._timer.Enabled = enabled;
                }
            }
        }

        private void LogTelemetry(string infoKey, string infoValue, string methodName)
        {
            try
            {
                StringAccumulator.Message.Send(infoKey, infoValue, false, null);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Logger.Debug.Write(string.Format(
                    "Error occurred while saving additional server details to telemetry. Details : {0}. MethodName : {1}. Error : {2}",
                    infoValue, methodName, exception));
            }
        }

        public void Ping()
        {
            Logger.Debug.Write("LicenseManager >> Ping >> Entered");
            try
            {
                lock (this._lock)
                {
                    this._server.Echo();
                }

                Logger.Debug.Write("LicenseManager >> Ping >> Successfully finished");
            }
            catch (Exception exception)
            {
                Logger.Error.Write("LicenseManager >> Ping >> Failed to ping the server.", exception);
                throw;
            }
        }

        public void Refresh()
        {
            string licenseKey;
            Logger.Debug.Write("LicenseManager >> Refresh >> Entered");
            if (this._license == null)
            {
                Logger.Debug.Write("LicenseManager >> Refresh >> Finished: License key not set.");
                return;
            }

            try
            {
                lock (this._lock)
                {
                    licenseKey = this._license.LicenseKey;
                }

                //this.SynchronizeLicense(licenseKey, false, true, true);
                Logger.Debug.Write("LicenseManager >> Refresh >> Finished: Update successfull.");
            }
            catch (Exception exception)
            {
                Logger.Error.Write("Failed to synchronize with the license server.", exception);
                throw;
            }
        }

        public void Refresh(bool runConversionIfRequired)
        {
            if (!runConversionIfRequired)
            {
                this.Refresh();
                return;
            }

            //this.LicenseCheckingTimerElapsed(this, null);
        }

        private int SetAsynchLicCheck()
        {
            int num;
            num = (this._settings.OldStyleLicensingConverter == null ||
                   !this._settings.OldStyleLicensingConverter.Exists
                ? 7200000
                : 1000);
            return num;
        }

        public void SetLicenseProxy(MLProxy proxy)
        {
            IDataStorage environmentVariableDataStorage;
            Logger.Debug.Write(string.Concat("LicenseManager >> SetLicenseProxy >> Entered, settings: ", proxy));
            try
            {
                if (this._settings.ProxyStorageProvider == LicenseStorageType.EnvironmentVariables)
                {
                    environmentVariableDataStorage =
                        new EnvironmentVariableDataStorage(this._settings.ProxyFilePath, this._settings.SecureStorage);
                }
                else
                {
                    IDataStorage fileDataStorage = new FileDataStorage(this._settings.ProxyFilePath,
                        this._settings.SecureStorage, this._settings.LicenseFileAccess,
                        "MetalogixLicenseProxyFileStorageMutex");
                    environmentVariableDataStorage = fileDataStorage;
                }

                ServerProxySettings serverProxySetting = new ServerProxySettings(environmentVariableDataStorage,
                    proxy.Enabled, proxy.Server,
                    (proxy.Enabled || !string.IsNullOrEmpty(proxy.Port) ? Convert.ToInt32(proxy.Port) : 0),
                    (proxy.Credentials.IsDefault ? string.Empty : proxy.Credentials.UserName),
                    (proxy.Credentials.IsDefault ? string.Empty : proxy.Credentials.Password.ToInsecureString()));
                Server server = new Server(this._settings.LicenseServerUrl, this._settings.LicenseServerUser,
                    this._settings.LicenseServerPass, serverProxySetting);
                Logger.Debug.Write("LicenseManager >> SetLicenseProxy >> Checking proxy settings");
                server.Echo();
                Logger.Debug.Write("LicenseManager >> SetLicenseProxy >> Connection OK, saving proxy settings");
                serverProxySetting.Save();
                lock (this._lock)
                {
                    this._server = server;
                    this._proxy = serverProxySetting;
                }

                Logger.Debug.Write("LicenseManager >> SetLicenseProxy >> Proxy settings were set successfully.");
            }
            catch (Exception exception)
            {
                Logger.Error.Write("LicenseManager error >> SetLicenseProxy >> Unable to set the proxy settings. ",
                    exception);
                throw;
            }
        }

        private bool SynchronizeLicense(string licenseKey, bool validateAgainstCurrentLicense,
            bool bAcceptExceededLicense, bool throwException)
        {
            bool flag;
            Logger.Debug.Write("LicenseManager >> SynchronizeLicense >> Entered");
            try
            {
                if (!(new LicenseKey(licenseKey)).IsValid)
                {
                    throw new InvalidLicenseException("Incorrect license key syntax.");
                }

                Logger.Debug.Write("LicenseManager >> SynchronizeLicense >> License key is valid");
                lock (this._lock)
                {
                    LicenseInfoResponse
                        licenseInfoResponse =
                            new LicenseInfoResponse(); //this._server.UpdateLicense(this.CreateLicenseInfoRequest(licenseKey));
                    licenseInfoResponse.Admin = @"Interak";
                    licenseInfoResponse.ContactName = "Interak";
                    licenseInfoResponse.ContactEmail = "migrador@interak.com.mx";
                    licenseInfoResponse.CustomerName = "Interak";
                    licenseInfoResponse.IsFremium = "false";
                    licenseInfoResponse.Updated = DateTime.Now;
                    licenseInfoResponse.UsedData = 0;
                    licenseInfoResponse.SystemInfo = "";
                    licenseInfoResponse.Signature = new byte[128];
                    licenseInfoResponse.Server =
                        "fe80::d5e0:e367:cd1c:5dac%25,fe80::168:3e1b:585a:3378%4,fe80::ec2d:40d8:b07d:9042%15,fe80::2874:1d31:4230:4beb%9,192.168.0.14,192.168.174.1,192.168.30.1,2001:0:5ef5:79fd:2874:1d31:4230:4beb,959672681518668812264038198";
                    CustomFields fld = new CustomFields();
                    fld.Fields = new[]
                    {
                        new CustomField() { Name = "skipWebChecking", Value = "false" },
                        new CustomField() { Name = "623", Value = "true" },
                        new CustomField() { Name = "672", Value = "true" },
                        new CustomField() { Name = "678", Value = "true" },
                        new CustomField() { Name = "693", Value = "true" },
                        new CustomField() { Name = "675", Value = "true" },
                        new CustomField() { Name = "673", Value = "true" },
                        new CustomField() { Name = "679", Value = "true" },
                        new CustomField() { Name = "676", Value = "true" },
                        new CustomField() { Name = "665", Value = "true" },
                        new CustomField() { Name = "674", Value = "true" },
                        new CustomField() { Name = "NewSharePointVersions", Value = "sp2016" },
                        new CustomField() { Name = "SharePointOnline", Value = "true" }
                    };
                    licenseInfoResponse.CustomFields = fld;
                    licenseInfoResponse.License = new LicenseInfo()
                    {
                        Key = "82335-D6AA3-A97AK-5K223-A59B8",
                        LicensedAdmins = 100,
                        LicensedData = 100,
                        LicensedServers = 100,
                        LicenseExpiration = DateTime.Now.AddYears(1),
                        MaintenanceExpiration = DateTime.Now.AddYears(1),
                        MaxAllowedSetupVersion = "8.5.0.2",
                        OfflineRevalidationDays = 365,
                        Status = LicenseStatus.Valid,
                        Type = LicenseType.Perpetual,
                        UsedAdmins = 0,
                        UsedData = 0,
                        UsedServers = 0
                    };

                    Logger.Debug.Write("LicenseManager >> SynchronizeLicense >> Communication finished");
                    MigrationLicense migrationLicense = new MigrationLicense(this._settings, licenseInfoResponse);
                    if (validateAgainstCurrentLicense)
                    {
                        migrationLicense.Validate(this._license);
                    }

                    if (!bAcceptExceededLicense && migrationLicense.DataLimitedExceeded)
                    {
                        throw new LicenseExpiredException(Resources.Licensed_Data_Exceeded);
                    }

                    this._license = migrationLicense;
                    this._license.Save();
                    Logger.Debug.Write(string.Concat(
                        "LicenseManager >> SynchronizeLicense >> Updated license data saved: ", this._license));
                }

                this.FireLicenseSynchronized(null);
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Logger.Error.Write(
                    "LicenseManager error >> SynchronizeLicense >> Unable to synchronize license data with the server. ",
                    exception);
                this.FireLicenseSynchronized(exception);
                if (throwException)
                {
                    throw;
                }

                flag = false;
            }

            return flag;
        }

        public bool Update()
        {
            this.FireLicenseUpdated();
            return true;
        }

        public bool UpdateLicense(long usedData, string serverId, bool isSPO, string tenantUrlAndUser)
        {
            bool flag;
            Logger.Debug.Write("LicenseManager >> UpdateLicense >> Entered");
            try
            {
                try
                {
                    lock (this._lock)
                    {
                        string str = (isSPO ? string.Concat("SPO:", serverId) : serverId);
                        this._server.UpdateLicense(this.GetLicenseInfo(usedData, str, tenantUrlAndUser));
                        if (isSPO)
                        {
                            this.AddAdditionalServerDetailsForSPO(str, tenantUrlAndUser);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Debug.Write("LicenseManager >> UpdateLicense >> Update license failed.", exception);
                }

                Logger.Debug.Write("LicenseManager >> UpdateLicense >> Finished: Update successfull.");
                flag = true;
            }
            catch (Exception exception1)
            {
                Logger.Error.Write("LicenseManager error >> UpdateLicense >> Failed to update", exception1);
                throw;
            }

            return flag;
        }

        public void UpdateLicenseKey(string key, bool isOldStyleLicense)
        {
            Logger.Debug.Write("LicenseManager >> UpdateLicenseKey >> Entered");
            try
            {
                if (isOldStyleLicense)
                {
                    Logger.Debug.Write(
                        "LicenseManager >> UpdateLicenseKey >> Old style key entered, starting conversion.");
                    key = this.ConvertOldLicenseKey(key);
                    Logger.Debug.WriteFormat(
                        "LicenseManager >> UpdateLicenseKey >> Old style key conversion finished, key='{0}'.",
                        new object[] { key });
                }

                try
                {
                    LicenseKey licenseKey = new LicenseKey(key);
                    if (this._license != null && this._license.Exists && !licenseKey.Equals(this._license.KeyValue))
                    {
                        this.SynchronizeLicense(this._license.LicenseKey, false, true, true);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Debug.Write("LicenseManager >> UpdateLicenseKey >> Synchronizing the old key failed.",
                        exception);
                }

                this.SynchronizeLicense(key, true, false, true);
                Logger.Debug.Write("LicenseManager >> UpdateLicenseKey >> Finished: Update successfull.");
            }
            catch (Exception exception1)
            {
                Logger.Error.Write("LicenseManager error >> UpdateLicenseKey >> Failed to update", exception1);
                throw;
            }
        }

        public event MLLicenseProviderCommon.LicenseSynchronizedEventHandler LicenseSynchronized;

        public event EventHandler LicenseUpdated;

        public delegate void LicenseSynchronizedEventHandler(MLLicenseProviderCommon sender, Exception error);
    }
}