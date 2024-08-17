using System;

namespace Metalogix.Licensing
{
    public class MLLicenseException : Exception
    {
        public MLLicenseException(string sMessage) : base(sMessage)
        {
        }
    }
}