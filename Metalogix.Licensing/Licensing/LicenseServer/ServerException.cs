using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class ServerException : BaseLicenseException
    {
        public ServerException(string message) : base(message)
        {
        }

        protected ServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}