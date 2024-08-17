using System;

namespace Metalogix.Actions
{
    public abstract class ActionConfigException : Exception
    {
        public ActionConfigException(string message) : this(message, null)
        {
        }

        public ActionConfigException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}