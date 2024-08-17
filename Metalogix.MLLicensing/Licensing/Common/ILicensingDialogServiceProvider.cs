using Metalogix.Connectivity.Proxy;
using Metalogix.Licensing.LicenseServer;
using System;

namespace Metalogix.Licensing.Common
{
    public interface ILicensingDialogServiceProvider
    {
        bool AllowSettingProxy { get; }

        string DataUnitName { get; }

        string DialogTitle { get; }

        string LicenseKey { get; }

        string LicenseOfflineActivationURL { get; }

        Metalogix.Licensing.LicenseServer.Product Product { get; }

        void ActivateLicenseOffline(string activationRequest);

        MLLicenseCommon GetLicenseInformation();

        MLProxy GetLicenseProxy();

        string GetOfflineLicenseActivationData(string key, bool isOldStyle);

        bool IsKeyLegitimate(string keyPrefix, string key);

        void SetLicenseProxy(MLProxy proxy);

        void UpdateLicenseKey(string key, bool isOldStyle);
    }
}