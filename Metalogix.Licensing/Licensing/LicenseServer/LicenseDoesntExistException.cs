using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class LicenseDoesntExistException : BaseLicenseException
    {
        public LicenseDoesntExistException(string message) : base(message)
        {
        }

        protected LicenseDoesntExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}