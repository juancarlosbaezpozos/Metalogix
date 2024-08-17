using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class NoHost : ServerProblem
    {
        public NoHost(string msg) : base(msg)
        {
        }

        public NoHost(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}