using System;
using System.ComponentModel;

namespace Metalogix.Licensing
{
    public interface ILicenseProvider : IDisposable
    {
        License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions);

        void Initialize();

        bool IsSPOServerIdExist(string serverId);

        bool Update();

        bool UpdateLicense(long usedData, string serverId, bool isSPO, string tenantUrlAndUser);

        event EventHandler LicenseUpdated;
    }
}