using Metalogix.SharePoint.Adapters.Properties;
using System;

namespace Metalogix.SharePoint.Adapters
{
    public class ServiceNotAvailableException : ServerProblem
    {
        public ServiceNotAvailableException() : base(Resources.ServiceNotFound)
        {
        }
    }
}