using System;

namespace Metalogix.SharePoint.Adapters
{
    [Flags]
    public enum AdapterSupportedFlags
    {
        SiteScope = 1,
        WebAppScope = 2,
        FarmScope = 4,
        TenantScope = 8,
        LegacyLicense = 4096,
        CurrentLicense = 8192
    }
}