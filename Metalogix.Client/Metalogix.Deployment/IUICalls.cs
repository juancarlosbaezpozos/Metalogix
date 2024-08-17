using Metalogix.Connectivity.Proxy;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer.Service;
using System;

namespace Metalogix.Deployment
{
    public interface IUICalls
    {
        void ExitApplication();

        MLLicense GetLicense();

        int GetProductId();

        string GetProductName();

        bool InvokeAnotherProcessesRunningDialog(AnotherProcessInfo[] anotherProcesses);

        string InvokeFileDownloadDialog(string fileURL, ref MLProxy proxy);

        void InvokeInstallDialog(string filePath);

        AutomaticUpdater.NewReleaseUserResponseType InvokeNotifyUserAboutNewReleaseDialog(
            LatestProductReleaseInfo releaseInfo, AutomaticUpdater updater);

        MLProxy InvokeProxyDialog(MLProxy proxy);

        void ShowError(string message);

        bool ShowMessage(string message, string title, bool showCancelButton);
    }
}