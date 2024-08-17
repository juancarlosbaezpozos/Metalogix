using System;
using System.Runtime.Serialization;

namespace Metalogix.Licensing.LicenseServer
{
    [Serializable]
    public class InvalidStorageFileException : BaseLicenseException
    {
        public InvalidStorageFileException(string message) : base(message)
        {
        }

        protected InvalidStorageFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}