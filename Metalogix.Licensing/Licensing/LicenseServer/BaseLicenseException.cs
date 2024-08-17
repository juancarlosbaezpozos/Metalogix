using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class BaseLicenseException : Exception
    {
        public BaseLicenseException(string message) : base(message)
        {
        }

        protected BaseLicenseException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}