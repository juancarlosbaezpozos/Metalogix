using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class NoMLSP : ServerProblem
    {
        public NoMLSP(string msg) : base(msg)
        {
        }

        public NoMLSP(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}