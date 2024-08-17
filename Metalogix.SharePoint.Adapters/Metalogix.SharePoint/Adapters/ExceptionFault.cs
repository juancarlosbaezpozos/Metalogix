using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class ExceptionFault
    {
        public const string EXCEPTIONACTION = "ExceptionThrown";

        [DataMember] public string InnerMessage { get; set; }

        [DataMember] public string InnerStackTrace { get; set; }

        public ExceptionFault(Exception error)
        {
            this.InnerMessage = error.Message;
            this.InnerStackTrace = error.StackTrace;
        }
    }
}