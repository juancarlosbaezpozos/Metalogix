using System;

namespace Metalogix.Licensing.SK
{
    public interface ILicenseStatus
    {
        DateTime ExpirationDate { get; }

        bool IsLoaded { get; }

        SKLicenseStatus StatusCode { get; }

        ILicenseStatus Clone();
    }
}