using Metalogix.Licensing;
using System;

namespace Metalogix.Licensing.SK
{
    public static class SKLP
    {
        public static MLLicenseProviderSK Get
        {
            get { return (MLLicenseProviderSK)MLLicenseProvider.Instance; }
        }
    }
}