using System;

namespace Metalogix.SharePoint.Adapters
{
    public class StoragePointNotAvailableException : Exception
    {
        public StoragePointNotAvailableException() : base("StoragePoint could not be found")
        {
        }
    }
}