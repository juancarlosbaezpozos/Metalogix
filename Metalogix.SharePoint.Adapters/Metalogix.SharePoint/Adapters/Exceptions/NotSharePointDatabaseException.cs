using Metalogix.SharePoint.Adapters.Properties;
using System;

namespace Metalogix.SharePoint.Adapters.Exceptions
{
    public class NotSharePointDatabaseException : Exception
    {
        public NotSharePointDatabaseException() : base(Resources.NotSharePointDatabase)
        {
        }
    }
}