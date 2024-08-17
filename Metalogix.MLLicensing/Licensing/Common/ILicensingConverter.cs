using Metalogix.Licensing.LicenseServer;
using System;

namespace Metalogix.Licensing.Common
{
    public interface ILicensingConverter
    {
        bool Exists { get; }

        LicenseKey Key { get; }

        string OldKey { get; }

        void Convert(MLLicenseProviderCommon man);
    }
}