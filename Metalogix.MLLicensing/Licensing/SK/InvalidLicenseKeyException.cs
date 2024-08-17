using System;

namespace Metalogix.Licensing.SK
{
    public class InvalidLicenseKeyException : Exception
    {
        public InvalidLicenseKeyException() : base(
            "The license key you have entered doesn`t exists on the license server or is incorrect.")
        {
        }
    }
}