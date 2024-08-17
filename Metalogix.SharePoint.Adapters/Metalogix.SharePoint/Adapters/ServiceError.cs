using Metalogix.SharePoint.Adapters.Properties;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public class ServiceError : Exception
    {
        public string ServiceStackTrace { get; set; }

        public override string StackTrace
        {
            get { return this.ServiceStackTrace; }
        }

        public ServiceError(ExceptionFault fault) : base(string.Format(Resources.ServiceError, fault.InnerMessage))
        {
            this.ServiceStackTrace = fault.InnerStackTrace;
        }
    }
}