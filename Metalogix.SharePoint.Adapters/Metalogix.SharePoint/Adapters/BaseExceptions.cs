using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public abstract class BaseExceptions : Exception
    {
        protected BaseExceptions()
        {
        }

        protected BaseExceptions(string msg) : base(msg)
        {
        }

        protected BaseExceptions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}