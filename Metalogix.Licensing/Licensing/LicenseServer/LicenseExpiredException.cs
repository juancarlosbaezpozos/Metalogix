using System;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class LicenseExpiredException : BaseLicenseException
    {
        public LicenseExpiredException(string message) : base(message)
        {
        }
    }
}