using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class InvalidLicenseException : BaseLicenseException
    {
        public InvalidLicenseException(string message) : base(message)
        {
        }

        protected InvalidLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}