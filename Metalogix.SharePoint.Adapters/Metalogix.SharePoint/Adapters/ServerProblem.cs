using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class ServerProblem : BaseExceptions
    {
        public ServerProblem(string msg) : base(msg)
        {
        }

        public ServerProblem(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}