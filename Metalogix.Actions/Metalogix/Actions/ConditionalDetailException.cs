using System;

namespace Metalogix.Actions
{
    public class ConditionalDetailException : Exception
    {
        public ConditionalDetailException(Exception exception_0) : this(exception_0.Message, null)
        {
        }

        public ConditionalDetailException(string message) : this(message, null)
        {
        }

        public ConditionalDetailException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}