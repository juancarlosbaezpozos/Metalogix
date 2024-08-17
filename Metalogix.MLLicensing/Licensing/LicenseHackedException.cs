using System;

namespace Metalogix.Licensing
{
    public class LicenseHackedException : Exception
    {
        public LicenseHackedException(string message) : base(message)
        {
        }
    }
}