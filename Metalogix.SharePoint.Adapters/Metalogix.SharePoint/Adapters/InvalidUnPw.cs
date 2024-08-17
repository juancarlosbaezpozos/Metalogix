using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class InvalidUnPw : UserProblem
    {
        public InvalidUnPw(string msg) : base(msg)
        {
        }

        public InvalidUnPw(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}