using Metalogix;
using Metalogix.Client;
using Metalogix.Connectivity.Proxy;
using Metalogix.Interfaces;
using Metalogix.Licensing;
using Metalogix.Licensing.Common;
using Metalogix.Licensing.LicenseServer.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Deployment
{
    public class AutomaticUpdater
    {
        private LatestProductReleaseInfo m_latestProductInfo;

        private string m_downloadLink;

        private string m_versionLink;

        private AutomaticUpdaterSettings m_settings;

        private MLProxy m_proxy;

        private bool m_bByQuiet = true;

        private MLLicenseProviderCommon m_provider;

        public MLLicenseProviderCommon Provider
        {
            get
            {
                if (this.m_provider == null)
                {
                    this.m_provider = MLLicenseProvider.Instance as MLLicenseProviderCommon;
                }

                return this.m_provider;
            }
        }

        public System.Version ServerLatestVersion
        {
            get
            {
                if (this.m_latestProductInfo == null)
                {
                    this.ReadConfiguration();
                }

                System.Version version = null;
                if (this.m_latestProductInfo.LatestVersion != null)
                {
                    version = new System.Version(this.m_latestProductInfo.LatestVersion);
                }

                return version;
            }
        }

        public System.Version ServerVersion
        {
            get
            {
                if (this.m_latestProductInfo == null)
                {
                    this.ReadConfiguration();
                }

                System.Version version = null;
                if (this.m_latestProductInfo.Version != null)
                {
                    version = new System.Version(this.m_latestProductInfo.Version);
                }

                return version;
            }
        }

        public AutomaticUpdaterSettings Settings
        {
            get { return this.m_settings; }
            set { this.m_settings = value; }
        }

        public AutomaticUpdater()
        {
            this.m_settings = new AutomaticUpdaterSettings();
        }

        public AutomaticUpdater(AutomaticUpdaterSettings settings)
        {
            this.m_settings = settings;
        }

        private AnotherProcessInfo[] CheckAnotherRunning()
        {
            Process currentProcess = Process.GetCurrentProcess();
            int id = currentProcess.Id;
            Process[] processesByName = Process.GetProcessesByName(currentProcess.ProcessName);
            processesByName = Array.FindAll<Process>(processesByName, (Process p) => p.Id != id);
            if (processesByName == null || (int)processesByName.Length == 0)
            {
                return null;
            }

            return Array.ConvertAll<Process, AnotherProcessInfo>(processesByName, (Process oneByName) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    Debugger.Break();

                AnotherProcessInfo anotherProcessInfo = new AnotherProcessInfo();
                anotherProcessInfo.Id = oneByName.Id;
                anotherProcessInfo.ProcessName = oneByName.ProcessName;
                anotherProcessInfo.UserName = this.GetProcessUsername(anotherProcessInfo.Id);
                return anotherProcessInfo;
            });
        }

        public bool CheckForUpdate()
        {
            this.ReadConfiguration();
            if (this.m_latestProductInfo == null)
            {
                return false;
            }

            bool flag = false;
            if (this.ServerVersion == null)
            {
                if (this.ServerLatestVersion != null &&
                    this.Settings.InstalledVersion.CompareTo(this.ServerLatestVersion) < 0)
                {
                    flag = true;
                }
            }
            else if (this.Settings.InstalledVersion.CompareTo(this.ServerVersion) < 0)
            {
                flag = true;
            }
            else if (this.ServerLatestVersion != null &&
                     this.Settings.InstalledVersion.CompareTo(this.ServerLatestVersion) < 0)
            {
                flag = true;
            }

            return flag;
        }

        public void CheckForUpdateAsync()
        {
            this.CheckForUpdateAsync(false);
        }

        public void CheckForUpdateAsync(bool notifyIfNoUpdate)
        {
            this.CheckForUpdateAsync(notifyIfNoUpdate, false);
        }

        public void CheckForUpdateAsync(bool notifyIfNoUpdate, bool ignoreTurnOffSettings)
        {
            this.m_bByQuiet = !notifyIfNoUpdate;
            if (!ignoreTurnOffSettings && this.m_settings.AutoUpdateSettings ==
                AutomaticUpdaterSettings.AutoUpdateSettingType.TurnOffCompletely)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem((object dummy) =>
            {
                try
                {
                    bool flag = false;
                    flag = this.CheckForUpdate();
                    if (this.m_latestProductInfo != null)
                    {
                        CheckForUpdateCompletedEventArgs checkForUpdateCompletedEventArg =
                            new CheckForUpdateCompletedEventArgs();
                        if (!(this.ServerVersion != null) ||
                            !this.ServerVersion.Equals(ClientConfigurationVariables.AutoUpdateSkipVersion))
                        {
                            checkForUpdateCompletedEventArg.UpdateNeeded = flag;
                        }
                        else
                        {
                            checkForUpdateCompletedEventArg.UpdateNeeded = false;
                        }

                        checkForUpdateCompletedEventArg.Tag = notifyIfNoUpdate;
                        this.OnCheckForUpdateCompleted(checkForUpdateCompletedEventArg);
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(string.Concat("Failed to get the latest version info: ", exception.ToString()));
                }
            });
        }

        private string GetProcessUsername(int processId)
        {
            string str;
            using (ManagementObjectSearcher managementObjectSearcher =
                   new ManagementObjectSearcher(string.Format("Select * From Win32_Process Where ProcessID = {0}",
                       processId)))
            {
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    string[] empty = new string[] { string.Empty };
                    if (Convert.ToInt32(managementObject.InvokeMethod("GetOwner", empty)) != 0)
                    {
                        continue;
                    }

                    str = empty[0];
                    return str;
                }

                str = "N/A";
            }

            return str;
        }

        private void OnCheckForUpdateCompleted(CheckForUpdateCompletedEventArgs e)
        {
            if (this.CheckForUpdateCompleted != null)
            {
                this.CheckForUpdateCompleted(this, e);
            }
        }

        private void ReadConfiguration()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings["UpdateUrl"] != null)
            {
                this.m_versionLink = configuration.AppSettings.Settings["UpdateUrl"].Value;
                return;
            }

            try
            {
                this.m_proxy = this.Provider.GetLicenseProxy();
                GetProductReleaseRequest getProductReleaseRequest = new GetProductReleaseRequest()
                {
                    LicenseKey = this.m_settings.License.LicenseKey,
                    ProductCode = this.m_settings.ProductId
                };
                bool flag = false;
                do
                {
                    flag = false;
                    try
                    {
                        this.m_latestProductInfo = this.Provider.GetLatestProductRelease(getProductReleaseRequest);
                    }
                    catch (WebException webException)
                    {
                        if (!this.m_bByQuiet && this.m_settings.UICalls.ShowMessage("Would You like to specify proxy ?",
                                "Can't connect to update server !", true))
                        {
                            MLProxy mLProxy = this.m_settings.UICalls.InvokeProxyDialog(this.m_proxy);
                            if (mLProxy != null)
                            {
                                this.m_proxy = mLProxy;
                                this.Provider.SetLicenseProxy(mLProxy);
                                flag = true;
                            }
                        }
                    }
                } while (flag);

                if (this.m_latestProductInfo != null)
                {
                    this.m_downloadLink = this.m_latestProductInfo.InstallationFileUrl;
                    this.m_versionLink = this.m_latestProductInfo.ReleaseNotesUrl;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException("Check for latest product release failed ",
                    exception.Message, exception, ErrorIcon.Error);
                this.m_latestProductInfo = null;
            }
        }

        public bool StartUpdate()
        {
            bool flag;
            AnotherProcessInfo[] anotherProcessInfoArray = this.CheckAnotherRunning();
            if (this.AnotherProcessRunningInfo == null)
            {
                IUICalls uICalls = this.m_settings.UICalls;
                this.AnotherProcessRunningInfo =
                    new AutomaticUpdater.AnotherProcessRunningInfoHandler(uICalls.InvokeAnotherProcessesRunningDialog);
            }

            while (anotherProcessInfoArray != null && (int)anotherProcessInfoArray.Length > 0)
            {
                if (!this.AnotherProcessRunningInfo(anotherProcessInfoArray))
                {
                    return false;
                }

                anotherProcessInfoArray = this.CheckAnotherRunning();
            }

            if (this.NotifyUserAboutNewRelease == null)
            {
                IUICalls uICall = this.m_settings.UICalls;
                this.NotifyUserAboutNewRelease =
                    new AutomaticUpdater.NotifyAboutNewReleaseEventHandler(uICall
                        .InvokeNotifyUserAboutNewReleaseDialog);
            }

            AutomaticUpdater.NewReleaseUserResponseType mLatestProductInfo =
                this.NotifyUserAboutNewRelease(this.m_latestProductInfo, this);
            if (mLatestProductInfo != AutomaticUpdater.NewReleaseUserResponseType.Update)
            {
                switch (mLatestProductInfo)
                {
                    case AutomaticUpdater.NewReleaseUserResponseType.TurnOffCompletely:
                    {
                        this.Settings.AutoUpdateSettings =
                            AutomaticUpdaterSettings.AutoUpdateSettingType.TurnOffCompletely;
                        return false;
                    }
                    case AutomaticUpdater.NewReleaseUserResponseType.AutoUpdate:
                    {
                        this.Settings.AutoUpdateSettings = AutomaticUpdaterSettings.AutoUpdateSettingType.AutoUpdate;
                        return false;
                    }
                    case AutomaticUpdater.NewReleaseUserResponseType.SkipVersion:
                    {
                        this.Settings.AutoUpdateSkipVersion = new System.Version(this.m_latestProductInfo.Version);
                        return false;
                    }
                    case AutomaticUpdater.NewReleaseUserResponseType.RemindLater:
                    {
                        this.Settings.AutoUpdateSettings = AutomaticUpdaterSettings.AutoUpdateSettingType.RemindMeLater;
                        return false;
                    }
                    case AutomaticUpdater.NewReleaseUserResponseType.Update:
                    {
                        return false;
                    }
                    case AutomaticUpdater.NewReleaseUserResponseType.Cancel:
                    {
                        this.Settings.AutoUpdateSettings = AutomaticUpdaterSettings.AutoUpdateSettingType.RemindMeLater;
                        return false;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }

            if (this.DownloadFile == null)
            {
                IUICalls uICalls1 = this.m_settings.UICalls;
                this.DownloadFile = new AutomaticUpdater.DownloadFileHandler(uICalls1.InvokeFileDownloadDialog);
            }

            string mDownloadLink = this.DownloadFile(this.m_downloadLink, ref this.m_proxy);
            this.Provider.SetLicenseProxy(this.m_proxy);
            if (string.IsNullOrEmpty(mDownloadLink))
            {
                return false;
            }

            try
            {
                string str = this.UnpackInstallationFile(mDownloadLink);
                if (string.IsNullOrEmpty(str))
                {
                    throw new DirectoryNotFoundException("Installation Directory not found!");
                }

                if (!Directory.Exists(str))
                {
                    throw new DirectoryNotFoundException(string.Concat("Installation Directory ", str, " not found!"));
                }

                string str1 = Path.Combine(str, this.m_latestProductInfo.InstallationFileName);
                if (!File.Exists(str1))
                {
                    throw new FileNotFoundException(string.Concat("File ", str1, " not found!"));
                }

                if (this.InstallFile == null)
                {
                    IUICalls uICall1 = this.m_settings.UICalls;
                    this.InstallFile = new AutomaticUpdater.InstallFileEventHandler(uICall1.InvokeInstallDialog);
                }

                this.InstallFile(str1);
                return true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException("Failed to start the setup.", exception.Message, exception,
                    ErrorIcon.Error);
                flag = false;
            }

            return flag;
        }

        private string UnpackInstallationFile(string filePath)
        {
            string tempPath = Path.GetTempPath();
            tempPath = Path.Combine(tempPath, this.m_settings.ProductName);
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            Directory.CreateDirectory(tempPath);
            using (ZipStorer zipStorer = ZipStorer.Open(filePath, FileAccess.Read))
            {
                foreach (ZipStorer.ZipFileEntry zipFileEntry in zipStorer.ReadCentralDir())
                {
                    string str = Path.Combine(tempPath, zipFileEntry.FilenameInZip);
                    zipStorer.ExtractFile(zipFileEntry, str);
                }
            }

            return tempPath;
        }

        public event AutomaticUpdater.AnotherProcessRunningInfoHandler AnotherProcessRunningInfo;

        public event EventHandler<CheckForUpdateCompletedEventArgs> CheckForUpdateCompleted;

        public event AutomaticUpdater.DownloadFileHandler DownloadFile;

        public event AutomaticUpdater.InstallFileEventHandler InstallFile;

        public event AutomaticUpdater.NotifyAboutNewReleaseEventHandler NotifyUserAboutNewRelease;

        public delegate bool AnotherProcessRunningInfoHandler(AnotherProcessInfo[] anotherProcesses);

        public delegate string DownloadFileHandler(string filepath, ref MLProxy proxy);

        public delegate void InstallFileEventHandler(string filePath);

        public enum NewReleaseUserResponseType
        {
            TurnOffCompletely,
            AutoUpdate,
            SkipVersion,
            RemindLater,
            Update,
            Cancel
        }

        public delegate AutomaticUpdater.NewReleaseUserResponseType NotifyAboutNewReleaseEventHandler(
            LatestProductReleaseInfo releaseInfo, AutomaticUpdater updater);
    }
}