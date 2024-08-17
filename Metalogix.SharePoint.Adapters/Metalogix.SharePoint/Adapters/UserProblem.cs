using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class UserProblem : BaseExceptions
    {
        public UserProblem(string msg) : base(msg)
        {
        }

        public UserProblem(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}