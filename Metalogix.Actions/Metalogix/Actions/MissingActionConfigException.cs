using System;
using System.Reflection;

namespace Metalogix.Actions
{
    public sealed class MissingActionConfigException : ActionConfigException
    {
        public MissingActionConfigException() : base("Action config is missing.")
        {
        }

        public MissingActionConfigException(Metalogix.Actions.Action action) : this(action.GetType())
        {
        }

        public MissingActionConfigException(Type actionType) : base(string.Concat("'", actionType.Name,
            "' action requires an action config to be specified."))
        {
        }
    }
}