using System;

namespace Metalogix.Licensing.SK
{
    public class DisabledLicenseKeyException : Exception
    {
        public DisabledLicenseKeyException() : base(
            "The license key you have entered has been disabled on the license server.")
        {
        }
    }
}