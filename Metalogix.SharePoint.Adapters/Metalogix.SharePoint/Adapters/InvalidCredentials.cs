using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class InvalidCredentials : UserProblem
    {
        public InvalidCredentials(string msg) : base(msg)
        {
        }

        public InvalidCredentials(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}