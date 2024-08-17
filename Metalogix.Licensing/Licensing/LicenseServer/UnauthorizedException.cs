using System;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class UnauthorizedException : BaseLicenseException
    {
        public UnauthorizedException() : base("You are not authorized to access the resource.")
        {
        }
    }
}