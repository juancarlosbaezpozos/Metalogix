using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class Unauthorized : UserProblem
    {
        public Unauthorized(string msg) : base(msg)
        {
        }

        public Unauthorized(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}