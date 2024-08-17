using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class InvalidCredentialsException : BaseLicenseException
    {
        public InvalidCredentialsException() : base("Invalid credentials.")
        {
        }

        protected InvalidCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}