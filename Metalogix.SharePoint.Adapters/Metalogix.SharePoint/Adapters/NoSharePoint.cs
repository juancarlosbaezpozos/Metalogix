using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class NoSharePoint : ServerProblem
    {
        public NoSharePoint(string msg) : base(msg)
        {
        }

        public NoSharePoint(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}