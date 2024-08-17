using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class InvalidSignatureException : BaseLicenseException
    {
        public InvalidSignatureException() : base("Data or signature is corrupt.")
        {
        }

        protected InvalidSignatureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}